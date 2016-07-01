using DaqProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using TestCMSCommon.DataBase;
using System.Windows.Forms;

namespace TestCms1
{
    public enum RecoderType : byte
    {
        RecoderType_File = 0x11,
        RecoderType_Network = 0x12,
        RecoderType_DB = 0x13,
    }

    public class RecodeUtil
    {
        public static void WavesRecodeFromQueue(Stream stream, IWavesSerializer serializer, Queue<WaveData[]> queue)
        {
            while (queue.Count <= 0) ;
            WaveData[] waves = null;
            lock ((queue as ICollection).SyncRoot)
            {
                waves = queue.Dequeue();
            }
            serializer.Serialize(stream, waves);
        }
    }

    public interface IWavesRecoder : ICancelableTask, ISendableConfig
    {
        Queue<WaveData[]> GetWavesQueue();
    }

    public class FileRecoder : IWavesRecoder
    {
        public IWavesSerializer WaveSerializer;
        public string FilePath;
        private Thread RecodeThread;
        private Queue<WaveData[]> WavesQueue = new Queue<WaveData[]>();
        private FileStream FStream;

        public FileRecoder(string filePath, IWavesSerializer serializer)
        {
            WaveSerializer = serializer;
            FilePath = filePath;
        }

        public FileRecoder(byte[] bytes)
        {
            WaveSerializer = SerializerUtil.ToSerializer(bytes[0]);
            FilePath = StringUTil.ToString(bytes.Where((x, i) => i > 0).ToArray());
        }

        public byte[] ToByte()
        {
            List<byte> buf = new List<byte>();
            buf.Add((WaveSerializer as IByteConvertable).ToByte());
            buf.AddRange(StringUTil.ToBytes(FilePath));
            return buf.ToArray();
        }

        public void OnThread()
        {
            while (true)
            {
                try
                {
                    RecodeUtil.WavesRecodeFromQueue(FStream, WaveSerializer, WavesQueue);
                }
                catch (Exception ex)
                {
                    //TODO Write Log
                }
            }
        }

        public void Start()
        {
            FStream = new FileStream(FilePath, FileMode.Append);
            RecodeThread = new Thread(OnThread);
            RecodeThread.Start();
        }

        public void Stop()
        {
            if (RecodeThread != null && RecodeThread.IsAlive)
                RecodeThread.Interrupt();
            if (FStream != null)
                FStream.Close();
        }

        public void Dispose()
        {
        }

        public Queue<WaveData[]> GetWavesQueue()
        {
            return WavesQueue;
        }

        public override string ToString()
        {
            return "FileRecoder - " + Path.GetFileName(FilePath) + ", " + WaveSerializer.ToString(); ;
        }

        public byte GetConfigType()
        {
            return (byte)RecoderType.RecoderType_File;
        }

        public PacketType GetPacketType()
        {
            return PacketType.PacketType_RecoderConfig;
        }
    }

    class NetworkRecoder : IWavesRecoder
    {
        private Socket Server;
        public IWavesSerializer WaveSerializer;
        private Thread RecodeThread;
        private Queue<WaveData[]> WavesQueue = new Queue<WaveData[]>();
        public int Port;

        public NetworkRecoder(int port, IWavesSerializer serializer)
        {
            Port = port;
            WaveSerializer = serializer;
        }

        public NetworkRecoder(byte[] bytes)
        {
            WaveSerializer = SerializerUtil.ToSerializer(bytes[0]);
            Port = BitConverter.ToInt32(bytes, 1);
        }

        public byte[] ToByte()
        {
            List<byte> buf = new List<byte>();
            buf.Add((WaveSerializer as IByteConvertable).ToByte());
            buf.AddRange(BitConverter.GetBytes(Port));
            return buf.ToArray();
        }

        public void OnThread()
        {
            while (true)
            {
                Server.Bind(new IPEndPoint(IPAddress.Any, Port));
                Server.Listen(5);
                Socket client = Server.Accept();
                try
                {
                    while (client.Connected)
                    {
                        var stream = new NetworkStream(client);
                        RecodeUtil.WavesRecodeFromQueue(stream, WaveSerializer, WavesQueue);
                    }
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch (SocketException se)
                {
                    //TODO Write Log
                }
            }
        }

        public void Start()
        {
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            RecodeThread = new Thread(OnThread);
            RecodeThread.Start();
        }

        public void Stop()
        {
            if (RecodeThread != null && RecodeThread.IsAlive)
                RecodeThread.Interrupt();
        }

        public void Dispose()
        {
        }

        public Queue<WaveData[]> GetWavesQueue()
        {
            return WavesQueue;
        }

        public override string ToString()
        {
            return "NetworkRecoder - " + Port + ", " + WaveSerializer.ToString(); ;
        }

        public byte GetConfigType()
        {
            return (byte)RecoderType.RecoderType_Network;
        }

        public PacketType GetPacketType()
        {
            return PacketType.PacketType_RecoderConfig;
        }
    }

    class DBRecoder : IWavesRecoder
    {
        public string DbIp;
        public string Account;
        public string Password;
        public string DbName;
        private Thread RecodeThread;
        private Queue<WaveData[]> WavesQueue = new Queue<WaveData[]>();
        public Queue<TrendData[]> MeasureDataQueue = new Queue<TrendData[]>();

        public DBRecoder(string dbIp, string account, string password, string db)
        {
            DbIp = dbIp;
            Account = account;
            Password = password;
            DbName = db;
        }

        public DBRecoder(byte[] bytes)
        {
            //패킷 통신 필요시 구현
        }

        public byte[] ToByte()
        {
            List<byte> buf = new List<byte>();
            //패킷 통신 필요시 구현
            return buf.ToArray();
        }

        public void OnThread()
        {
            while (true)
            {
                try
                {
                    //TODO Wave 저장
                }
                catch (Exception ex)
                {
                    //TODO Write Log
                }
            }
        }

        public void Start()
        {
            RecodeThread = new Thread(OnThread);
            RecodeThread.Start();
        }

        public void Stop()
        {
            if (RecodeThread != null && RecodeThread.IsAlive)
                RecodeThread.Interrupt();
        }

        public void Dispose()
        {
        }

        public Queue<WaveData[]> GetWavesQueue()
        {
            return WavesQueue;
        }

        public override string ToString()
        {
            return "DBRecoder - " + DbIp + ", " + Account + ", " + DbName;
        }

        public byte GetConfigType()
        {
            return (byte)RecoderType.RecoderType_DB;
        }

        public PacketType GetPacketType()
        {
            return PacketType.PacketType_RecoderConfig;
        }
    }
}
