﻿using DaqProtocol;
using Newtonsoft.Json;
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
    public class VDPMReceiver : IWavesReceiver
    {
        private Daq5509Receiver DaqReceiver;
        public string ModuleIp;
        public event Action<WaveData[]> WavesReceived;
        public VDPMReceiver() { }
        public VDPMReceiver(string ip)
        {
            ModuleIp = ip;
        }

        private void DaqReceiver_WaveReceived(WaveData[] waves)
        {
            WavesReceived(waves);
        }

        public void Start()
        {
            if (DaqReceiver == null)
            {
                RobinChannel[] channels = new RobinChannel[8];
                for (int i = 0; i < channels.Length; i++)
                    channels[i] = new RobinChannel(i, ConstantMember.AsyncFMax, ConstantMember.AsyncLine);
                DaqReceiver = new Daq5509Receiver(ModuleIp, channels);
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
    }
    public class FileReceiver : IWavesReceiver
    {
        public event Action<WaveData[]> WavesReceived;
        private System.Timers.Timer Timer;
        public IWaveSerializer WaveSerializer;
        public string FilePath;
        public FileReceiver() { }
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
            using (var br = new BinaryReader(new FileStream(FilePath, FileMode.Open)))
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
            return "FileReceiver - " + Path.GetFileName(FilePath)+", "+WaveSerializer.ToString();
        }
    }

    public class NetworkReceiver : IWavesReceiver
    {
        public event Action<WaveData[]> WavesReceived;
        [NonSerialized]
        private System.Timers.Timer Timer;
        public IWaveSerializer WaveSerializer;
        private Socket Client;
        public string ServerIp;
        public int Port;
        public NetworkReceiver() { }
        public NetworkReceiver(string Ip, int port, IWaveSerializer serializer)
        {
            WaveSerializer = serializer;
            ServerIp = Ip;
            Port = port;
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
            Client.Connect(ServerIp,Port);
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

        public override string ToString()
        {
            return "NetReceiver - " + ServerIp + ":" + Port+ ", " + WaveSerializer.ToString(); ;
        }
    }
}