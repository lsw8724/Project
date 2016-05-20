using DaqProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TraceTool;
using TSICommon;

namespace DaqProtocol
{
    public class RobinChannel
    {
        public int Id;
        public int PhysicalIndex;
        public int AsyncFMax;
        public int AsyncLine;
        public bool ICP;
        public DaqGain HWGain;
        public float Sensitivity = 1.0f;

        public float ScaleFactorByDisplayUnit() { return 1.0f; }

        public RobinChannel(int physicalIndex, int fmax = 3200, int line = 3200)
        {
            this.PhysicalIndex = physicalIndex;
            this.AsyncFMax = fmax;
            this.AsyncLine = line;

            this.Id = physicalIndex + 1;
            this.ICP = true;
            this.HWGain = DaqGain._1;
        }
    }

    public class RobinMeasure
    {
        public RobinMeasureType Type = RobinMeasureType.Bandpass;
        public Unit2Type SpectrumUnit = Unit2Type.rms;
        public RobinIntegralType Integral = RobinIntegralType.None;

        public float BandLow = 10;
        public float BandHigh = 1000;
        public float Offset = 0;
    }
}
