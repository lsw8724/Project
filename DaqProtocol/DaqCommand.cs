using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DaqProtocol
{
    //보드펌웨어에서 데이터 길이를 읽은길이로 판단하기 때문에 송신시 꼭 한번에 전송해야 한다
    //커맨드들의 응답패킷은 커맨드패킷 길이대로 응답한다
    public class DaqCommand
    {
        public const int DefaultPacketSize = 48;

        public DaqCmdType Cmd { get { return (DaqCmdType)sendBuf[0]; } set { sendBuf[0] = (byte)value; } }
        public DaqResponse Response { get { return (DaqResponse)recvBuf[2]; } set { recvBuf[2] = (byte)value; } }   //ushort배열에서 1번 인덱스이므로 byte에서는 2
        protected ushort this[int i]
        {
            get { return BitConverter.ToUInt16(sendBuf, i * 2); }
            set
            {
                var bytes = BitConverter.GetBytes(value);
                sendBuf[i * 2] = bytes[0];
                sendBuf[i * 2 + 1] = bytes[1];
            }
        }
        public byte[] sendBuf;
        public byte[] recvBuf;

        protected DaqClient parent;

        public DaqCommand(DaqClient parent, DaqCmdType cmd, int packetSize = DefaultPacketSize)
        {
            this.parent = parent;
            sendBuf = new byte[packetSize];
            recvBuf = new byte[packetSize];
            Cmd = cmd;
        }

        public virtual bool SendRecv(NetworkStream ns)
        {
            Send(ns);
            Recv(ns);
            return Response == DaqResponse.SUCCESS;
        }

        public void Send(NetworkStream ns)
        {
            ns.Write(sendBuf, 0, sendBuf.Length);
        }

        public byte[] Recv(NetworkStream ns)
        {
            return ReadBlock(ns, recvBuf);
        }

        public static byte[] ReadBlock(System.IO.Stream stream, byte[] bytes)
        {
            int size = bytes.Length;
            int totalReaded = 0;
            while (totalReaded < bytes.Length)
            {
                var leftedSize = size - totalReaded;
                var readed = stream.Read(bytes, totalReaded, leftedSize);
                totalReaded += readed;
            }
            //Debug.WriteLine("totalRead:" + totalReaded);
            return bytes;
        }
    }
    public class DaqSetInputType : DaqCommand
    {
        public int ChannelIndex { get { return this[1]; } set { this[1] = (ushort)value; } }
        public bool IcpEnable { get { return this[2] == 1; } set { this[2] = value ? (ushort)1 : (ushort)0; } }
        public DaqInputType AcDc { get { return (DaqInputType)this[3]; } set { this[3] = (ushort)value; } }

        public DaqSetInputType(DaqClient parent, int ch, bool icp, DaqInputType acdc)
            : base(parent, DaqCmdType.DAQ_IN_TYPE)
        {
            ChannelIndex = ch;
            IcpEnable = icp;
            AcDc = acdc;
        }
    }

    public class DaqSetGain : DaqCommand
    {
        public int ChannelIndex { get { return this[1]; } set { this[1] = (ushort)value; } }
        public bool Atten10 { get { return this[2] == 1; } set { this[2] = value ? (ushort)1 : (ushort)0; } }
        public DaqGain Gain { get { return (DaqGain)this[3]; } set { this[3] = (ushort)value; } }

        public DaqSetGain(DaqClient parent, int ch, bool atten10, DaqGain gain)
            : base(parent, DaqCmdType.DAQ_GAIN)
        {
            ChannelIndex = ch;
            Atten10 = atten10;
            Gain = gain;
        }
    }

    public class DaqSetRunVariable : DaqCommand
    {
        public int Mode { get { return this[1]; } set { this[1] = (ushort)value; } }                //0:async, 1:sync
        public int KP_Revolution { get { return this[2]; } set { this[2] = (ushort)value; } }       //(buf[2]+1)*8;
        public int ResampleSize { get { return this[3]; } set { this[3] = (ushort)value; } }       //(buf[2]+1)*8;
        public int FilterOrder { get { return this[4]; } set { this[4] = (ushort)value; } }
        public int KP_EdgeAndSelect { get { return this[5]; } set { this[5] = (ushort)value; } }    //상위4비트:Select, 하위4비트:Edge
        public int ChannelCount { get { return this[6]; } set { this[6] = (ushort)value; } }      //4비트값(max 15), 채널갯수인듯

        public DaqSetRunVariable(DaqClient parent, int mode, int kpRevol, int resample, int filterOrder = 0, int kpEdgeAdnSelect = 0, int channelCount = 8)
            : base(parent, DaqCmdType.DAQ_OPMODE)
        {
            Mode = mode;
            KP_Revolution = kpRevol;
            ResampleSize = resample;
            FilterOrder = filterOrder;
            KP_EdgeAndSelect = kpEdgeAdnSelect;
            ChannelCount = channelCount;
        }
    }

    public class DaqSetSampleRate : DaqCommand
    {
        public DaqSamplingRate SampleRate { get { return (DaqSamplingRate)this[1]; } set { this[1] = (ushort)value; } }

        public DaqSetSampleRate(DaqClient parent, DaqSamplingRate sampleRate)
            : base(parent, DaqCmdType.DAQ_SAMPLE)
        {
            SampleRate = sampleRate;
        }
    }

    public class DaqStart : DaqCommand
    {
        public int Sense { get { return this[1]; } set { this[1] = (ushort)value; } }
        public int Gap { get { return this[2]; } set { this[2] = (ushort)value; } }

        public DaqStart(DaqClient parent, bool sense, bool gap)
            : base(parent, DaqCmdType.DAQ_START)
        {
            Sense = sense ? 1 : 0;
            Gap = gap ? 1 : 0;
        }
    }
    public class DaqStop : DaqCommand
    {
        bool waitCorrectResponse = false;

        public DaqStop(DaqClient parent, bool waitCorrectResponse)
            : base(parent, DaqCmdType.DAQ_STOP)
        {
            this.waitCorrectResponse = waitCorrectResponse;
        }

        public override bool SendRecv(NetworkStream ns)
        {
            if (waitCorrectResponse)
            {
                Send(ns);
                return RecvUntilResponse(ns);
            }
            return base.SendRecv(ns);
        }

        //5509보드의 프로토콜은 커맨드채널과 데이타 채널의 분리가 안되어 있음
        //Start로 데이타 전송 모드로 들어간 후에는, 커맨드 전송 후 읽은 데이터는 커맨드 응답이 아니라 데이터일 확률이 높음
        //본 함수는 그런 경우를 위해 커맨드 응답이 올때까지 계속 읽는 기능을 담당
        public bool RecvUntilResponse(NetworkStream ns)
        {
            try
            {
                byte[] buff = new byte[48];
                //int loopCount = 0;
                while (true)
                {
                    //var packet = DaqDataPacket.RecvPacket(parent, ns, "stop");      //STOP 요청후 바로 정지되지 않으므로
                    //if (packet.CmdType == DaqCmdType.DAQ_STOP)
                    //    return true;     

                    int readSize = ns.Read(buff, 0, buff.Length);
                    if (readSize != buff.Length)
                    {
                        return true;
                    }

                    //데이터 전송모드에서 Stop보낼때는 응답없어보임
                    //Debug.WriteLine("RecvUntilResponse - Loop Count:" + loopCount++);
                }
            }
            catch (Exception)
            {
                //여기서는 예외 발생해도 정상
                Debug.WriteLine("Data Stopped(정상예외)");
            }
            return false;
        }
    }
}
