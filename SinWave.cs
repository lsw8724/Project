using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestCms1
{
    public class SinWave
    {
        public SinWave(float freq, float amp)
        {
            Freq = freq;
            Amplitude = amp;
        }
        public float Freq { get; set; }
        public float Amplitude { get; set; }
    }
}
