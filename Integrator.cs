using MathNet.Numerics.Transformations;
using System;
using TestCms1.Properties;

namespace TestCms1
{
    public class Integrator
    {
        private static RealFourierTransformation rft = new RealFourierTransformation(TransformationConvention.NoScaling);

        public float[] Integration(float[] data)
        {
            float[] result = new float[data.Length];

            var fftReals = FFT(data);
            HighPassFiltering(fftReals);
            var IFFTDatas = IFFT(fftReals);
            float[] filteredAsyncData = Array.ConvertAll(IFFTDatas, x => (float)x);

            result[0] = filteredAsyncData[0];
            for (int i = 1; i < filteredAsyncData.Length; i++)
            {
                result[i] = filteredAsyncData[i] + result[i - 1];
            }
            return result;
        }

        public class Complex
        {
            public double[] Reals;
            public double[] Imags;
        }

        private Complex FFT(float[] asyncData)
        {
            double[] doubleArr = Array.ConvertAll(asyncData, x => (double)x);
            Complex complex = new Complex();
            rft.TransformForward(doubleArr, out complex.Reals, out complex.Imags);
            return complex;
        }

        private double[] IFFT(Complex complex)
        {
            double[] result;
            rft.TransformBackward(complex.Reals, complex.Imags, out result);
            return result;
        }

        public Complex HighPassFiltering(Complex complex)
        {
            float resolution = Settings.Default.AsyncLine / (float)Settings.Default.AsyncFMax;
            int CutOffFreqIdx = Settings.Default.FFTCutOffFrequency * (int)resolution;
            for (int i = 0; i < CutOffFreqIdx; i++)
            {
                complex.Reals[i] = 0;
                complex.Imags[i] = 0;
            }
            return complex;
        }
    }


}
