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
    public interface IWaveRecoder : ICancelableTask
    {
        void OnRecode();
    }

    [Serializable]
    public class FileRecoder : IWaveRecoder
    {
        private IWaveSerializer WaveSerializer;
        private string FilePath;
        private Thread RecodeThread;
        public Queue<WaveData[]> WavesQueue;
        public FileRecoder(string filePath, IWaveSerializer serializer, Queue<WaveData[]> queue)
        {
            WavesQueue = queue;
            WaveSerializer = serializer;
            FilePath = filePath;
            RecodeThread = new Thread(OnRecode);
        }

        public void OnRecode()
        {
            while(true)
            {
                try
                {
                    WaveData[] waves = null;
                    lock ((WavesQueue as ICollection).SyncRoot)
                    {
                        if (WavesQueue.Count <= 0) continue;
                        waves = WavesQueue.Dequeue();
                    }
                    using (var bw = new BinaryWriter(new FileStream(FilePath, FileMode.Append)))
                        WaveSerializer.Serialize(bw, waves);
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
                RecodeThread.Abort();
        }

        public void Dispose()
        {
        }
    }

    [Serializable]
    class NetworkRecoder : IWaveRecoder
    {
        private Socket Server { get; set; }
        private IWaveSerializer WaveSerializer;
        [NonSerialized]
        private Thread RecodeThread;
        public Queue<WaveData[]> WavesQueue;
        public NetworkRecoder(int port, IWaveSerializer serializer, Queue<WaveData[]> queue)
        {
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Server.Bind(new IPEndPoint(IPAddress.Any, port));
            Server.Listen(5);

            WaveSerializer = serializer;
            WavesQueue = queue;
            RecodeThread = new Thread(OnRecode);
        }

        public void OnRecode()
        {
            while (true)
            {
                Socket client = Server.Accept();
                try
                {
                    while (client.Connected)
                    {
                        using (var bw = new BinaryWriter(new NetworkStream(client)))
                        {
                            WaveData[] waves = null;
                            lock ((WavesQueue as ICollection).SyncRoot)
                            {
                                if (WavesQueue.Count <= 0) break;
                                waves = WavesQueue.Dequeue();
                            }
                            WaveSerializer.Serialize(bw, waves);
                        }
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
                RecodeThread.Abort();
            if (Server != null)
            {
                Server.Close();
            }
        }

        public void Dispose()
        {
            Server.Dispose();
        }

        
    }
}
