using DaqProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestCms1
{
    public interface IMeasureCalculator
    {
        float CalcMeasureData(WaveData wave, SpectrumData spectrum);
        int GetChannelIdx();
    }

    [Serializable]
    public class RMSMeasure : IMeasureCalculator
    {
        public int ChannelIdx { private get; set; }
        private int HighFreq { get; set; }
        private int LowFreq { get; set; }

        public RMSMeasure(int low = 0,int high = 3600)
        {
            HighFreq = high;
            LowFreq = low;
        }

        public float CalcMeasureData(WaveData wave, SpectrumData spectrum)
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
    }

    [Serializable]
    public class PeakToPeakMeasure : IMeasureCalculator
    {
        public int ChannelIdx { private get; set; }
        public float CalcMeasureData(WaveData wave, SpectrumData spectrum)
        {
            return wave.AsyncData.Max() - wave.AsyncData.Min(); 
        }


        public int GetChannelIdx()
        {
            return ChannelIdx;
        }
    }

    [Serializable]
    public class PeakMeasure : IMeasureCalculator
    {
        public int ChannelIdx { private get; set; }
        public float CalcMeasureData(WaveData wave, SpectrumData spectrum)
        {
            return wave.AsyncData.Max();
        }


        public int GetChannelIdx()
        {
            return ChannelIdx;
        }
    }
    [Serializable]
    public class Lift_ShockMeasure : IMeasureCalculator
    {
        public int ChannelIdx { private get; set; }
        private int ImpulseRegion1 { get; set; }
        private int ImpulseRegion2 { get; set; }
        private int MoveRegion1 { get; set; }
        public Lift_ShockMeasure(int imp1, int imp2, int mov)
        {
            ImpulseRegion1 = imp1;
            ImpulseRegion2 = imp2;
            MoveRegion1 = mov;
        }
        public float CalcMeasureData(WaveData wave, SpectrumData spectrum)
        {
            return 0f;
        }


        public int GetChannelIdx()
        {
            return ChannelIdx;
        }
    }
    [Serializable]
    public class Lift_MoveMeasure : IMeasureCalculator
    {
        public int ChannelIdx { private get; set; }
        private int ImpulseRegion1 { get; set; }
        private int ImpulseRegion2 { get; set; }
        private int MoveRegion1 { get; set; }
        public Lift_MoveMeasure(int imp1, int imp2, int mov)
        {
            ImpulseRegion1 = imp1;
            ImpulseRegion2 = imp2;
            MoveRegion1 = mov;
        }
        public float CalcMeasureData(WaveData wave, SpectrumData spectrum)
        {
            return 0f;
        }


        public int GetChannelIdx()
        {
            return ChannelIdx;
        }
    }
}
