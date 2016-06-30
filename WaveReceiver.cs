using DaqProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using TestCms1.Properties;
using System.Linq;

namespace TestCms1
{
    public enum ReceiverType
    {
        ReceiverType_Vdpm,
        ReceiverType_File,
        ReceiverType_Network,
        ReceiverType_Simulate,
    }

    public interface IHeritableWavesReceiver : IWavesReceiver, ISendableConfig { }
    
    public class VDPMReceiver : IHeritableWavesReceiver
    {
        private Daq5509Receiver DaqReceiver;
        public string ModuleIp;
        public event Action<WaveData[]> WavesReceived;

        public VDPMReceiver(string ip)
        {
            ModuleIp = ip;
        }

        private void DaqReceiver_WaveReceived(WaveData[] waves)
        {
            WavesReceived(waves);
        }
        public VDPMReceiver(byte[] bytes)
        {
            //ModuleIp = StringUTil.ToString(bytes.Where((x, i) => i > 3).ToArray());
            ModuleIp = StringUTil.ToString(bytes);
        }
        public byte[] ToByte()
        {
            List<byte> buf = new List<byte>();
            buf.AddRange(StringUTil.ToBytes(ModuleIp));
            return buf.ToArray();
        }

        public void Start()
        {
            if (DaqReceiver == null)
            {
                RobinChannel[] channels = new RobinChannel[8];
                for (int i = 0; i < channels.Length; i++)
                    channels[i] = new RobinChannel(i, Settings.Default.AsyncFMax, Settings.Default.AsyncLine);
                int packetCountFor1Sec = (int)(Settings.Default.AsyncFMax / 3200.0 * 16.0);
                DaqReceiver = new Daq5509Receiver(ModuleIp, channels, packetCountFor1Sec);
                DaqReceiver.WavesReceived += DaqReceiver_WaveReceived;
            }
            DaqReceiver.Start();
        }

        public void Stop()
        {
            if (DaqReceiver != null)
                DaqReceiver.Stop();
        }

        public void Dispose()
        {
        }

        public override string ToString()
        {
            return "VDPMReceiver - " + ModuleIp;
        }

        public byte GetConfigType()
        {
            return (byte)ReceiverType.ReceiverType_Vdpm;
        }

        public PacketType GetPacketType()
        {
            return PacketType.PacketType_ReceiverConfig;
        }
    }

    public class FileReceiver : IHeritableWavesReceiver
    {
        public event Action<WaveData[]> WavesReceived;
        private System.Timers.Timer Timer;
        public IWavesSerializer WaveSerializer;
        public string FilePath;
        Stream FileStream;
      
        public FileReceiver(string filePath, IWavesSerializer serializer)
        {
            WaveSerializer = serializer;
            FilePath = filePath;
        }

        public FileReceiver(byte[] bytes)
        {
            WaveSerializer = SerializerUtil.ToSerializer(bytes[0]);
            //FilePath = StringUTil.ToString(bytes.Where((x, i) => i > 4 && x != '\0').ToArray());
            FilePath = StringUTil.ToString(bytes.Where((x, i) => i > 0 && x != '\0').ToArray());
        }
        public byte[] ToByte()
        {
            List<byte> buf = new List<byte>();
            buf.Add((WaveSerializer as IByteConvertable).ToByte());
            buf.AddRange(StringUTil.ToBytes(FilePath));
            return buf.ToArray();
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            WaveData[] waves = WaveSerializer.Deserialize(FileStream);
            if (waves != null)
                WavesReceived(waves);
        }

        public void Start()
        {
            FileStream = new FileStream(FilePath, FileMode.Open);
            Timer = new System.Timers.Timer();
            Timer.Interval = 1000;
            Timer.Elapsed += TimerElapsed;
            Timer.Start();
        }

        public void Stop()
        {
            if (Timer == null) return;
            Timer.Stop();
            Timer.Dispose();
            Timer = null;
            FileStream.Close();
        }

        public void Dispose()
        {
        }

        public override string ToString()
        {
            return "FileReceiver - " + Path.GetFileName(FilePath)+", "+WaveSerializer.ToString();
        }

        public byte GetConfigType()
        {
            return (byte)ReceiverType.ReceiverType_File;
        }
        public PacketType GetPacketType()
        {
            return PacketType.PacketType_ReceiverConfig;
        }
    }

    public class NetworkReceiver : IHeritableWavesReceiver
    {
        public event Action<WaveData[]> WavesReceived;
        private System.Timers.Timer Timer;
        public IWavesSerializer WaveSerializer;
        private Socket Client;
        public string ServerIp;
        public int Port;
      
        public NetworkReceiver(string Ip, int port, IWavesSerializer serializer)
        {
            WaveSerializer = serializer;
            ServerIp = Ip;
            Port = port;
            Timer = new System.Timers.Timer();
            Timer.Interval = 1000;
            Timer.Elapsed += TimerElapsed;
        }

        public NetworkReceiver(byte[] bytes)
        {
            WaveSerializer = SerializerUtil.ToSerializer(bytes[0]);
            Port = BitConverter.ToInt32(bytes, 4);
            var strByte = bytes.Where((x, i) => i > 7 && x != '\0').ToArray();
            ServerIp = StringUTil.ToString(strByte);
        }

        public byte[] ToByte()
        {
            List<byte> buf = new List<byte>();
            buf.Add((WaveSerializer as IByteConvertable).ToByte());
            buf.AddRange(new byte[3]);  //reserved bytes
            buf.AddRange(BitConverter.GetBytes(Port));
            buf.AddRange(StringUTil.ToBytes(ServerIp));
            return buf.ToArray();
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Client.Connected) return;
            try
            {
                WaveData[] waves = WaveSerializer.Deserialize(new NetworkStream(Client));
                WavesReceived(waves);
            }
            catch (SocketException se)
            {
                //TODO Write Log
            }
        }

        public void Start()
        {
            if (Client == null)
            {
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Client.ReceiveTimeout = 5000;
                Client.SendTimeout = 5000;
            }
            Client.Connect(ServerIp,Port);
            Timer.Start();
        }

        public void Stop()
        {
            if (Timer != null)
                Timer.Stop();
            if (Client != null)
            {
                Client.Disconnect(false);
                Client.Shutdown(SocketShutdown.Both);
                Client.Close();
                Client.Dispose();
                Client = null;
            }
        }

        public void Dispose()
        {
            Timer.Dispose();
            Client.Dispose();
        }

        public override string ToString()
        {
            return "NetReceiver - " + ServerIp + ":" + Port+ ", " + WaveSerializer.ToString(); ;
        }
       
        public byte GetConfigType()
        {
            return (byte)ReceiverType.ReceiverType_Network;
        }

        public PacketType GetPacketType()
        {
            return PacketType.PacketType_ReceiverConfig;
        }
    }

    public class SimulateReceiver : IHeritableWavesReceiver
    {
        public event Action<WaveData[]> WavesReceived;
        private System.Timers.Timer Timer;
        private int DataCount = (int)(Settings.Default.AsyncLine * 2.56);
        public SinWave[] SinWaves;

        public SimulateReceiver(params SinWave[] sinWaves)
        {
            SinWaves = new SinWave[] { new SinWave(60, 1) };
        }
        public SimulateReceiver(byte[] bytes)
        {
            SinWaves = new SinWave[] { new SinWave(60, 1) };
        }

        public byte[] ToByte()
        {
            return new byte[0];
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            WaveData[] waves = new WaveData[8];
            for(int i =0; i<waves.Length; i++)
            {
                waves[i] = new WaveData();
                waves[i].ChannelId = i + 1;
                waves[i].DateTime = DateTime.UtcNow;
                waves[i].AsyncDataCount = DataCount;
                waves[i].AsyncData = CreateSimulateFloatDataArr(SinWaves);
            }
            if (waves != null)
                WavesReceived(waves);
        }

        public void Start()
        {
            Timer = new System.Timers.Timer();
            Timer.Interval = 1000;
            Timer.Elapsed += TimerElapsed;
            Timer.Start();
        }

        public void Stop()
        {
            if (Timer == null) return;
            Timer.Stop();
            Timer.Dispose();
            Timer = null;
        }

        public void Dispose()
        {
        }

        public override string ToString()
        {
            return "SimulateReceiver - 60Hz 100mV";
        }

        private double CalcWaveMomentData(float freq, float amp, double t)
        {
            return (amp * Math.Sin(freq * 2 * Math.PI * t));
        }

        public float[] CreateSimulateFloatDataArr(SinWave[] sinWaves) //[진동수, 진폭]
        {
            float resolution = Settings.Default.AsyncLine / (float)Settings.Default.AsyncFMax;
            float[] dataArr = new float[DataCount];

            for (int i = 0; i < DataCount; i++)
            {
                float time = (i / (float)DataCount) * resolution;
                double sinSum = 0.0;
                for (int j = 0; j < sinWaves.Length; j++)
                {
                    sinSum += CalcWaveMomentData(sinWaves[j].Freq, sinWaves[j].Amplitude, time);
                }
                dataArr[i] = Convert.ToSingle(sinSum);
            }
            return dataArr;
        }

        public byte GetConfigType()
        {
            return (byte)ReceiverType.ReceiverType_Simulate;
        }

        public PacketType GetPacketType()
        {
            return PacketType.PacketType_ReceiverConfig;
        }
    }
}