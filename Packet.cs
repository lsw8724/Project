using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaqProtocol;
using System.IO;

namespace TestCms1
{
    public interface ISendableConfig
    {
        byte GetConfigType();
        PacketType GetPacketType();
        byte[] ToByte();
    }

    public enum PacketType : byte
    {
        PacketType_Invalid = 0,
        PacketType_WaveData,
        PacketType_ReceiverConfig,
        PacketType_MeasureConfig,
        PacketType_RecoderConfig,
    }

    public class ConfigPacketHeader
    {
        public const byte Prefix = 0xab;
        public PacketType PacketTypeByte;
        public byte SubType;
        public byte[] Reserved = new byte[1];
        public int PayloadSize;

        public void Read(Stream s)
        {
            byte[] buf = new byte[8];
            s.Read(buf, 0, buf.Length);
            if (Prefix != buf[0]) return;
            PacketTypeByte = (PacketType)buf[1];
            SubType = buf[2];
            PayloadSize = BitConverter.ToInt32(buf, 4);
        }

        public byte[] ToByte()
        {
            List<byte> buf = new List<byte>();
            buf.Add(Prefix);
            buf.Add((byte)PacketTypeByte);
            buf.Add(SubType);
            buf.AddRange(Reserved);
            buf.AddRange(BitConverter.GetBytes(PayloadSize));
            return buf.ToArray();
        }
    }

    public class ConfigPacket
    {
        public ConfigPacketHeader Header = new ConfigPacketHeader();
        public byte[] Payload;

        public void Read(Stream s)
        {
            Header.Read(s);
            Payload = new byte[Header.PayloadSize];
            s.Read(Payload, 0, Payload.Length);
        }

        public void Write(Stream s)
        {
            List<byte> buf = new List<byte>();
            buf.AddRange(Header.ToByte());
            buf.AddRange(Payload);
            s.Write(buf.ToArray(), 0, buf.Count);
        }
    }

    public class ConfigReader
    {
        public ISendableConfig Read(ConfigPacket Packet)
        {
            switch(Packet.Header.PacketTypeByte)
            {
                case PacketType.PacketType_ReceiverConfig:
                    switch (Packet.Header.SubType)
                    {
                        case (byte)ReceiverType.ReceiverType_Vdpm: return new VDPMReceiver(Packet.Payload);
                        case (byte)ReceiverType.ReceiverType_File: return new FileReceiver(Packet.Payload);
                        case (byte)ReceiverType.ReceiverType_Network: return new NetworkReceiver(Packet.Payload);
                        case (byte)ReceiverType.ReceiverType_Simulate: return new SimulateReceiver(Packet.Payload);
                    }
                   
                    break;
                case PacketType.PacketType_RecoderConfig:
                    switch (Packet.Header.SubType)
                    {
                        case (byte)RecoderType.RecoderType_File: return new FileRecoder(Packet.Payload);
                        case (byte)RecoderType.RecoderType_Network: return new NetworkRecoder(Packet.Payload);
                    }
                    break;
                case PacketType.PacketType_MeasureConfig:
                    switch (Packet.Header.SubType)
                    {
                        case (byte)MeasureType.MeasureType_RMS: return new RMSMeasure(Packet.Payload);
                        case (byte)MeasureType.MeasureType_PK: return new PeakMeasure(Packet.Payload);
                        case (byte)MeasureType.MeasureType_P2P: return new PeakToPeakMeasure(Packet.Payload);
                        case (byte)MeasureType.MeasureType_LiftShock: return new Lift_ShockMeasure(Packet.Payload);
                        case (byte)MeasureType.MeasureType_LiftMove: return new Lift_MoveMeasure(Packet.Payload);
                    }
                    break;                
            }
            return null;
        }

    }
}
