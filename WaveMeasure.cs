using DaqProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestCMSCommon.DataBase;

namespace TestCms1
{
    public enum MeasureType : byte
    {
        MeasureType_P2P,
        MeasureType_PK,
        MeasureType_RMS,
        MeasureType_LiftShock,
        MeasureType_LiftMove,
    }

    public interface ISelectableIdx
    {
        int GetIdx();
    }

    public interface ISelectableChannelIdx
    {
        int GetChannelIdx();
    }

    public interface IWavesMeasure : ISelectableChannelIdx, ISendableConfig, ISelectableIdx
    {
        float CalcMeasureScalar(WaveData wave, SpectrumData spectrum); 
    }

    public class RMSMeasure : IWavesMeasure
    {
        public int MeasureId { get; set; }
        public int ChannelIdx { get; set; }
        public int HighFreq { get; set; }
        public int LowFreq { get; set; }
        
        public RMSMeasure(int id,int ch, int low = 0,int high = 3600)
        {
            MeasureId = id;
            ChannelIdx = ch;
            HighFreq = high;
            LowFreq = low;
        }

        public RMSMeasure(byte[] bytes)
        {
            MeasureId = BitConverter.ToInt32(bytes, 0);
            ChannelIdx = BitConverter.ToInt32(bytes, 4);
            LowFreq = BitConverter.ToInt32(bytes, 8);
            HighFreq = BitConverter.ToInt32(bytes, 12);
        }

        public byte[] ToByte()
        {
            List<byte> buf = new List<byte>();
            buf.AddRange(BitConverter.GetBytes(MeasureId));
            buf.AddRange(BitConverter.GetBytes(ChannelIdx));
            buf.AddRange(BitConverter.GetBytes(LowFreq));
            buf.AddRange(BitConverter.GetBytes(HighFreq));
            return buf.ToArray();
        }
        public float CalcMeasureScalar(WaveData wave, SpectrumData spectrum)
        {
            double sum = 0;
            for (int i = LowFreq; i < HighFreq; i++)
                sum += spectrum.YValues[i] * spectrum.YValues[i];
            return (float)Math.Sqrt(sum / spectrum.XValues.Length);
        }

        public int GetChannelIdx()
        {
            return ChannelIdx;
        }

        public override string ToString()
        {
            return "RMS - Ch" + (ChannelIdx + 1) + ", " + LowFreq + "~" + HighFreq;
        }

        public byte GetConfigType()
        {
            return (byte)MeasureType.MeasureType_RMS;
        }
        public PacketType GetPacketType()
        {
            return PacketType.PacketType_MeasureConfig;
        }

        public int GetIdx()
        {
            return MeasureId;
        }
    }

    public class PeakToPeakMeasure : IWavesMeasure
    {
        public int MeasureId { get; set; }
        public int ChannelIdx { get; set; }
        public float CalcMeasureScalar(WaveData wave, SpectrumData spectrum)
        {
            return wave.AsyncData.Max() - wave.AsyncData.Min(); 
        }

        public PeakToPeakMeasure(int id, int ch)
        {
            MeasureId = id;
            ChannelIdx = ch;
        }

        public PeakToPeakMeasure(byte[] bytes)
        {
            MeasureId = BitConverter.ToInt32(bytes, 0);
            ChannelIdx = BitConverter.ToInt32(bytes, 4);
        }

        public byte[] ToByte()
        {
            List<byte> buf = new List<byte>();
            buf.AddRange(BitConverter.GetBytes(MeasureId));
            buf.AddRange(BitConverter.GetBytes(ChannelIdx));
            buf.AddRange(new byte[4]); //cpp Scalar
            return buf.ToArray();
        }

        public int GetChannelIdx()
        {
            return ChannelIdx;
        }

        public override string ToString()
        {
            return "PtoP - Ch" + (ChannelIdx + 1);
        }

        public byte GetConfigType()
        {
            return (byte)MeasureType.MeasureType_P2P;
        }

        public PacketType GetPacketType()
        {
            return PacketType.PacketType_MeasureConfig;
        }

        public int GetIdx()
        {
            return MeasureId;
        }
    }

    public class PeakMeasure : IWavesMeasure
    {
        public int MeasureId { get; set; }
        public int ChannelIdx { get; set; }
        public float CalcMeasureScalar(WaveData wave, SpectrumData spectrum)
        {
            return wave.AsyncData.Max();
        }

        public PeakMeasure(int id, int ch)
        {
            MeasureId = id;
            ChannelIdx = ch;
        }

        public PeakMeasure(byte[] bytes)
        {
            MeasureId = BitConverter.ToInt32(bytes, 0);
            ChannelIdx = BitConverter.ToInt32(bytes, 4);
        }

        public byte[] ToByte()
        {
            List<byte> buf = new List<byte>();
            buf.AddRange(BitConverter.GetBytes(MeasureId));
            buf.AddRange(BitConverter.GetBytes(ChannelIdx));
            buf.AddRange(new byte[4]); //Cpp Scalar
            return buf.ToArray();
        }

        public int GetChannelIdx()
        {
            return ChannelIdx;
        }

        public override string ToString()
        {
            return "Peak - Ch" + (ChannelIdx + 1);
        }

        public byte GetConfigType()
        {
            return (byte)MeasureType.MeasureType_PK;
        }

        public PacketType GetPacketType()
        {
            return PacketType.PacketType_MeasureConfig;
        }

        public int GetIdx()
        {
            return MeasureId;
        }
    }

    public class Lift_ShockMeasure : IWavesMeasure
    {
        public int MeasureId { get; set; }
        public int ChannelIdx { get; set; }
        private int ImpulseRegion1 { get; set; }
        private int ImpulseRegion2 { get; set; }
        private int MoveRegion1 { get; set; }
        public Lift_ShockMeasure(int id, int ch,int imp1, int imp2, int mov)
        {
            MeasureId = id;
            ChannelIdx = ch;
            ImpulseRegion1 = imp1;
            ImpulseRegion2 = imp2;
            MoveRegion1 = mov;
        }

        public Lift_ShockMeasure(byte[] bytes)
        {
            MeasureId = BitConverter.ToInt32(bytes, 0);
            ChannelIdx = BitConverter.ToInt32(bytes, 4);
            ImpulseRegion1 = BitConverter.ToInt32(bytes, 8);
            ImpulseRegion2 = BitConverter.ToInt32(bytes, 12);
            MoveRegion1 = BitConverter.ToInt32(bytes, 16);
        }

        public byte[] ToByte()
        {
            List<byte> buf = new List<byte>();
            buf.AddRange(BitConverter.GetBytes(MeasureId));
            buf.AddRange(BitConverter.GetBytes(ChannelIdx));
            buf.AddRange(BitConverter.GetBytes(ImpulseRegion1));
            buf.AddRange(BitConverter.GetBytes(ImpulseRegion2));
            buf.AddRange(BitConverter.GetBytes(MoveRegion1));
            return buf.ToArray();
        }

        public float CalcMeasureScalar(WaveData wave, SpectrumData spectrum)
        {
            return 0f;
        }

        public int GetChannelIdx()
        {
            return ChannelIdx;
        }

        public override string ToString()
        {
            return "Lift_Shock - Ch" + (ChannelIdx + 1);
        }

        public byte GetConfigType()
        {
            return (byte)MeasureType.MeasureType_LiftShock;
        }

        public PacketType GetPacketType()
        {
            return PacketType.PacketType_MeasureConfig;
        }

        public int GetIdx()
        {
            return MeasureId;
        }
    }

    public class Lift_MoveMeasure : IWavesMeasure
    {
        public int MeasureId { get; set; }
        public int ChannelIdx { get; set; }
        private int ImpulseRegion1 { get; set; }
        private int ImpulseRegion2 { get; set; }
        private int MoveRegion1 { get; set; }

        public Lift_MoveMeasure(int id, int ch, int imp1, int imp2, int mov)
        {
            MeasureId = id;
            ChannelIdx = ch;
            ImpulseRegion1 = imp1;
            ImpulseRegion2 = imp2;
            MoveRegion1 = mov;
        }

        public Lift_MoveMeasure(byte[] bytes)
        {
            MeasureId = BitConverter.ToInt32(bytes, 0);
            ChannelIdx = BitConverter.ToInt32(bytes, 4);
            ImpulseRegion1 = BitConverter.ToInt32(bytes, 8);
            ImpulseRegion2 = BitConverter.ToInt32(bytes, 12);
            MoveRegion1 = BitConverter.ToInt32(bytes, 16);
        }

        public byte[] ToByte()
        {
            List<byte> buf = new List<byte>();
            buf.AddRange(BitConverter.GetBytes(MeasureId));
            buf.AddRange(BitConverter.GetBytes(ChannelIdx));
            buf.AddRange(BitConverter.GetBytes(ImpulseRegion1));
            buf.AddRange(BitConverter.GetBytes(ImpulseRegion2));
            buf.AddRange(BitConverter.GetBytes(MoveRegion1));
            return buf.ToArray();
        }

        public float CalcMeasureScalar(WaveData wave, SpectrumData spectrum)
        {
            return 0f;
        }

        public int GetChannelIdx()
        {
            return ChannelIdx;
        }

        public override string ToString()
        {
            return "Lift_Move - Ch" + (ChannelIdx + 1);
        }

        public byte GetConfigType()
        {
            return (byte)MeasureType.MeasureType_LiftMove;
        }

        public PacketType GetPacketType()
        {
            return PacketType.PacketType_MeasureConfig;
        }

        public int GetIdx()
        {
            return MeasureId;
        }
    }
}
