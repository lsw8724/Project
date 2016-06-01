using DaqProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TestCms1
{
    public class RecodeUtil
    {
        public static void WavesRecodeFromQueue(Stream stream, IWaveSerializer serializer, Queue<WaveData[]> queue)
        {
            if (queue.Count <= 0) return;
            WaveData[] waves = null;
            lock ((queue as ICollection).SyncRoot)
            {
                waves = queue.Dequeue();
            }

            using (var bw = new BinaryWriter(stream))
            {
                serializer.Serialize(bw, waves);
            }
        }
    }

    public interface IRecoder : ICancelableTask, ISendableConfig
    {
        Queue<WaveData[]> GetWavesQueue();
    }
    
    public class FileRecoder : IRecoder
    {
        public IWaveSerializer WaveSerializer;
        public string FilePath;
        private Thread RecodeThread;
        public Queue<WaveData[]> WavesQueue = new Queue<WaveData[]>();
        public FileRecoder(string filePath, IWaveSerializer serializer)
        {
            WaveSerializer = serializer;
            FilePath = filePath;
            RecodeThread = new Thread(OnThread);
        }

        public void OnThread()
        {
            while(true)
            {
                try
                {
                    var stream = new FileStream(FilePath, FileMode.Append);
                    RecodeUtil.WavesRecodeFromQueue(stream, WaveSerializer, WavesQueue);
                }
                catch (Exception ex)
                {
                    //TODO Write Log
                }
            }
        }

        public void Start()
        {
            RecodeThread.Start();
        }

        public void Stop()
        {
            if (RecodeThread != null && RecodeThread.IsAlive)
                RecodeThread.Join();
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
    }

    class NetworkRecoder : IRecoder
    {
        private Socket Server;
        public IWaveSerializer WaveSerializer;
        private Thread RecodeThread;
        public Queue<WaveData[]> WavesQueue = new Queue<WaveData[]>();
        public int Port;
        public NetworkRecoder(int port, IWaveSerializer serializer)
        {
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Port = port;
            WaveSerializer = serializer;
            RecodeThread = new Thread(OnThread);
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
                catch (Exception se)
                {
                    //TODO Write Log
                }
            }
        }

        public void Start()
        {
            RecodeThread.Start();
        }

        public void Stop()
        {
            if (RecodeThread != null && RecodeThread.IsAlive)
                RecodeThread.Join();
            if (Server != null)
            {
                Server.Close();
            }
        }

        public void Dispose()
        {
            Server.Dispose();
        }

        public Queue<WaveData[]> GetWavesQueue()
        {
            return WavesQueue;
        }

        public override string ToString()
        {
            return "NetReceiver - " + Port+ ", " + WaveSerializer.ToString(); ;
        }

    }
}
