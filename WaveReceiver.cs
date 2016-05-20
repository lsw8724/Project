using DaqProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TestCms1
{
    [Serializable]
    public class FileReceiver : IWavesReceiver
    {
        public event Action<WaveData[]> WavesReceived;
        [NonSerialized]
        private System.Timers.Timer Timer;
        private IWaveSerializer WaveSerializer;
        private string FilePath;
        public FileReceiver(string filePath, IWaveSerializer serializer)
        {
            WaveSerializer = serializer;
            Timer = new System.Timers.Timer();
            Timer.Interval = 1000;
            Timer.Elapsed += TimerElapsed;
            FilePath = filePath;
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            using(var br = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
            {
                if (br.PeekChar() >= 0)
                {
                    WaveData[] waves = WaveSerializer.Deserialize(br);
                    WavesReceived(waves);
                }
                else Stop();
            }
        }

        public void Start()
        {
            Timer.Start();
        }

        public void Stop()
        {
            Timer.Stop();
        }

        public void Dispose()
        {
        }

        public override string ToString()
        {
            return "FileReceiver," + Path.GetFileName(FilePath);
        }
    }

    [Serializable]
    public class NetworkReceiver : IWavesReceiver
    {
        public event Action<WaveData[]> WavesReceived;
        [NonSerialized]
        private System.Timers.Timer Timer;
        private IWaveSerializer WaveSerializer;
        private Socket Client;
        private IPEndPoint LocalEndPoint;
        public NetworkReceiver(string Ip, int port,IWaveSerializer serializer)
        {
            WaveSerializer = serializer;
            LocalEndPoint = new IPEndPoint(IPAddress.Parse(Ip), port);
            Timer = new System.Timers.Timer();
            Timer.Interval = 1000;
            Timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Client.Connected) return;
            try
            {
                int request = 1; //요청 메세지 1이 가면 서버로부터 Wave 데이터 응답
                using (var br = new BinaryReader(new NetworkStream(Client)))
                {
                    Client.Send(BitConverter.GetBytes(request));
                    WaveData[] waves = WaveSerializer.Deserialize(br);
                    WavesReceived(waves);
                }
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
            Client.Connect(LocalEndPoint);
            Timer.Start();
        }

        public void Stop()
        {
            if (Timer != null)
                Timer.Stop();
            if (Client != null)
            {
                Client.Send(BitConverter.GetBytes(-1)); //요청 메세지 -1이면 서버로부터 Disconnect
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
    }
}