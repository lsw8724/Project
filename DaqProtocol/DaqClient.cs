using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TraceTool;
using Real = System.Single;

//더미 클래스들
namespace TraceTool
{
    public class WinTrace
    {
        public WinTrace(string a, string b) { }
        public void Send(string msg) { }
    }

    public class WinWatch
    {
        public WinWatch(string a, string b) { }
        public void Send(string l, object r) { }
    }
}

namespace DaqProtocol
{
    public class DaqClient
    {
        internal TcpClient tcp;
        internal NetworkStream ns;
        WinTrace trace;
        WinWatch watch;

        private float[] scaleFactors;

        private int _nSamplingRate = 0;
        private DaqSamplingRate _samplerate = DaqSamplingRate._8192;
        private DaqSamplingRate samplerate
        {
            get { return _samplerate; }
            set { _samplerate = value; _nSamplingRate = int.Parse(value.ToString().Substring(1)); }
        }

        public int SamplingRateAsInt { get { return _nSamplingRate; } }

        public void WriteDebug(string msg)
        {
            Debug.WriteLine(DateTime.Now.ToShortTimeString() + ", " + msg);
        }
        public void WriteWatch(string name, object obj)
        {
            if (watch != null)
                watch.Send(name, obj);
        }
        public void WriteTrace(string msg)
        {
            if (trace != null)
                trace.Send(msg);
        }

        public DaqClient(float[] scaleFactors = null)
        {
            if (scaleFactors == null)
                this.scaleFactors = new float[] { 1, 1, 1, 1, 1, 1, 1, 1 };
            else
                this.scaleFactors = scaleFactors;
        }

        public void Connect(string ip, int port, int timeout = 2000)
        {
            watch = new WinWatch("wDaq" + ip, "wDaq" + ip);
            trace = new WinTrace("tDaq" + ip, "tDaq" + ip);

            tcp = new TcpClient();
            try
            {
                var result = tcp.BeginConnect(ip, port, null, null);
                tcp.ReceiveTimeout = 5000;
                bool success = result.AsyncWaitHandle.WaitOne(timeout, true);
                if (!success)
                    throw new DaqException("Connect Failed - WaitOne Failed");

                if (!tcp.Connected)
                    throw new DaqException("Connect Failed - Not Established");

                //tcp.LingerState = new LingerOption(true, 0);
                //tcp.NoDelay = true;
                ns = tcp.GetStream();
                qs = new ByteQueuedStream(ns);
                WriteDebug("Connected");
            }
            catch (Exception)
            {
                tcp.Close();
                throw;
            }
        }

        public void Close()
        {
            try
            {
                tcp.ReceiveTimeout = 500;
                new DaqStop(this, true).SendRecv(ns);
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                WriteDebug("Close - Stop " + ex.ToString());
            }

            try
            {
                tcp.Client.Shutdown(SocketShutdown.Both);
                tcp.Close();
            }
            catch (Exception ex)
            {
                WriteDebug("Close - Stop " + ex.ToString());
            }

            //lastGapPacket = null;
        }

        public void Start()
        {
            DoCommand(new DaqStart(this, true, true));
        }

        public bool Stop(bool consumeLeftPacket = false)
        {
            //DoCommand(new DaqStop(this, consumeLeftPacket));
            try
            {
                var command = new DaqStop(this, consumeLeftPacket);
                if (consumeLeftPacket)
                {
                    command.Send(ns);
                    return command.RecvUntilResponse(ns);
                }
                else
                {
                    command.SendRecv(ns);
                    return true;
                }
            }
            catch (Exception ex)
            {
                WriteDebug("DoCommand - Stop, " + ex);
                Close();
                throw;
            }
        }

        public void SetInputType(int iCh, bool icp = true, DaqInputType acdc = DaqInputType.AC)
        {
            DoCommand(new DaqSetInputType(this, iCh, icp, acdc));
        }

        public void SetGain(int iCh, bool atten10 = false, DaqGain gain = DaqGain._1)
        {
            DoCommand(new DaqSetGain(this, iCh, false, gain));
        }

        public void SetRunVariable(int mode = 0, int kpRevol = 0, int resample = 0, int filterOrder = 0, int kpEdgeAdnSelect = 0, int channelCount = 8)
        {
            DoCommand(new DaqSetRunVariable(this, mode, kpRevol, resample, filterOrder, kpEdgeAdnSelect, channelCount));
        }

        public void SetSampleMode(DaqSamplingRate samplerate = DaqSamplingRate._16384)
        {
            this.samplerate = samplerate;
            DoCommand(new DaqSetSampleRate(this, samplerate));
        }

        protected virtual void DoCommand(DaqCommand command)
        {
            try
            {
                command.SendRecv(ns);
                var responseCmd = (DaqCmdType)command.recvBuf[0];
                WriteDebug("Send:" + command + "," + responseCmd + " Response:" + command.Response);
            }
            catch (Exception ex)
            {
                WriteDebug("DoCommand - " + command + ", " + ex);
                Close();
                throw;
            }
        }

        //여러번 Fetch한 데이터를 모음
        public DaqData FetchDatas(int count = 8)
        {
            var datas = new List<DaqData>();
            for (int i = 0; i < count; i++)
                datas.Add(FetchData());

            var channelAsyncs = new float[8][];
            for (int iCh = 0; iCh < 8; iCh++)
            {
                channelAsyncs[iCh] = new float[datas.Sum(d => d.ChannelsAsyncs[iCh].Length)];
                int pos = 0;
                for (int i = 0; i < count; i++)
                {
                    var src = datas[i].ChannelsAsyncs[iCh];
                    Array.Copy(src, 0, channelAsyncs[iCh], pos, src.Length);
                    pos += src.Length;
                }
            }
            var lastData = datas.Last();
            return new DaqData { ChannelsAsyncs = channelAsyncs, ChannelsGap = lastData.ChannelsGap, Rpm1 = lastData.Rpm1, Rpm2 = lastData.Rpm2 };
        }

        //private DaqDataPacket lastGapPacket = null;
        ByteQueuedStream qs;
        float lastRpm1 = 0;
        float lastRpm2 = 0;
        float[] lastGaps = null;

        public DaqData FetchData()
        {
            List<DaqDataPacket> packets = new List<DaqDataPacket>();
            DaqData result = null;
            while (result == null)
            {
                var packet = DaqDataPacket.RecvDataPacket(this, qs, "fetch", trace);
                result = ProcessPacket(packets, packet);
            }
            return result;
        }

        private DaqData ProcessPacket(List<DaqDataPacket> packets, DaqDataPacket packet)
        {
            DaqData result = null;

            if (packet.DataType == DaqDataType.DAQ_DATA)
            {
                packets.Add(packet);
                if (lastGaps != null && packets.Count == PacketCountFor1Sec)
                    result = PopAndProcessAsyncPackets(packets);
            }
            else if (packet.DataType == DaqDataType.GAP_DATA)
            {
                if (lastGaps == null)
                {
                    //연결 후 처음 gap 받은 시점(이전의 DAQ_DATA는 미완성 데이타)
                    packets.Clear();
                }

                var gapAndRpm = packet.ToWords();
                lastGaps = gapAndRpm.Take(8).Select(w => ConvertGap(w)).ToArray();
                lastRpm1 = ConvertRpm(packet.Bytes, 16);
                lastRpm2 = ConvertRpm(packet.Bytes, 20);
            }
            return result;
        }

        public int PacketCountFor1Sec = 16; //3200일때 16
        //private DaqData ProcessPacket(List<DaqDataPacket> packets, DaqDataPacket packet)
        //{
        //    DaqData result = null;

        //    if (packet.DataType == DaqDataType.DAQ_DATA)
        //    {
        //        packets.Add(packet);
        //        if (lastGaps != null && packets.Count == PacketCountFor1Sec)
        //            result = PopAndProcessAsyncPackets(packets);
        //    }
        //    else if (packet.DataType == DaqDataType.GAP_DATA)
        //    {
        //        if (lastGaps == null)
        //        {
        //            //연결 후 처음 gap 받은 시점(이전의 DAQ_DATA는 미완성 데이타)
        //            packets.Clear();
        //        }

        //        var gapAndRpm = packet.ToWords();
        //        lastGaps = gapAndRpm.Take(8).Select(w => ConvertGap(w)).ToArray();
        //        lastRpm1 = ConvertRpm(packet.Bytes, 16);
        //        lastRpm2 = ConvertRpm(packet.Bytes, 20);
        //    }
        //    //if (result != null && result.ChannelsAsyncs[0].Length != 1024)
        //    //    Debug.WriteLine("asd");
        //    return result;
        //}

        DateTime lastProcessTime = DateTime.UtcNow;
        private DaqData PopAndProcessAsyncPackets(List<DaqDataPacket> packets)
        {
            var now = DateTime.UtcNow;
            trace.Send("PopAndProcessAsyncPackets");
            trace.Send("Elapsed - " + (now - lastProcessTime).TotalMilliseconds + " miliseconds");
            
            DaqData data = new DaqData();
            //var gapAndRpm = packet.ToWords();
            //data.ChannelsGap = gapAndRpm.Take(8).Select(w => ConvertGap(w)).ToArray();
            //data.Rpm1 = ConvertRpm(packet.Bytes, 16);
            //data.Rpm2 = ConvertRpm(packet.Bytes, 20);
            data.ChannelsGap = lastGaps;
            data.Rpm1 = lastRpm1;
            data.Rpm2 = lastRpm2;

            var totalDataCount = packets.Sum(p => p.Bytes.Length) / 2;  //byte니까 2로 나눔
            var iCurDataIndex = 0;
            for (int iPacket = 0; iPacket < packets.Count; iPacket++)
            {
                var interleaved = packets[iPacket].ToShorts();

                for (int iCh = 0; iCh < 8; iCh++)
                {
                    if (data.ChannelsAsyncs[iCh] == null)
                        data.ChannelsAsyncs[iCh] = new float[totalDataCount / 8];

                    for (int i = 0; i < interleaved.Length / 8; i++)
                        data.ChannelsAsyncs[iCh][iCurDataIndex + i] = ConvertAsync(interleaved[i * 8 + iCh], iCh);
                }

                iCurDataIndex += interleaved.Length / 8;
            }

            lastProcessTime = now;
            return data;
        }

        #region 데이타 처리

        private static float ConvertGap(ushort adcValue)
        {
            //return (float)Math.Round(((adcValue + 1)-500)/20.0, 3);    //오프셋0일때 499정도가 나옴
            return (Real)(((((Real)adcValue) - 512) * 24.72692 / 512) + 0.55);
        }

        private static float ConvertRpm(byte[] bytes, int startIndex)
        {
            // 버퍼값은 KEY_PHASOR 1주기 신호에 대한 33.554432MHZ 클럭 카운트
            var rpm = (float)(Math.Round(33554432.0 / BitConverter.ToUInt32(bytes, startIndex), 2) * 60.0);
            if (rpm < 1.0)
                rpm = 0;
            return rpm;
        }

        private float ConvertAsync(short adcValue, int iCh)
        {
            if (false)
                return adcValue * 5000.0f / 32768.0f;
            else
            {
                //var miliVolt = adcIntBuffer[ch + channelCount * i] * 5000.0f / 32768.0f;
                //chBuffer[i] = miliVolt/scaleFactors[ch]/nGain;
                return adcValue * scaleFactors[iCh];
            }
        }

        #endregion 데이타 처리
    }
}