using System;
using System.Linq;
using TestCms1.Properties;
using TSICommon;
using MathNet.Numerics.Transformations;
using MathNet.Numerics;

namespace TestCms1
{
    public class SpectrumData
    {
        public DateTime DataUTCTime { get; set; }
        public float[] XValues { get; set; }
        public float[] YValues { get; set; }
    }

    public class FFTCalculator
    {
        public static SpectrumData CreateSpectrumData(float[] data, DateTime dateTime = new DateTime())
        {
            var duration = Settings.Default.AsyncLine / (double)Settings.Default.AsyncFMax;
            float[] fft = PositiveFFT(data);
            int lineCount = fft.Length;
            float[] freq = new float[lineCount];
            for (int i = 0; i < lineCount; i++)
                freq[i] = (float)(i / duration);
            return new SpectrumData()
            {
                DataUTCTime = dateTime,
                YValues = fft,
                XValues = freq
            };
        }

        #region FFT 계산
        private static RealFourierTransformation rft = new RealFourierTransformation(TransformationConvention.NoScaling);
        private static float[] PositiveFFT(float[] timeData)
        {
            var N = timeData.Length;
            double[] rawfft = FFT(timeData);
            var normalizer = 4.0 / N;
            double[] optScale = rawfft.Take(N / 2).Select(f => f * normalizer).ToArray();  //스케일링 최적화
            float[] result = new float[optScale.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = (float)optScale[i];
            return result;
        }

        private static double[] FFT(float[] timeData)
        {
            double[] xdata = new double[timeData.Length];
            for (int i = 0; i < timeData.Length; i++)
                xdata[i] = timeData[i];
            double[] reals, imags;
            rft.TransformForward(xdata, out reals, out imags);
            for (int i = 0; i < reals.Length; i++)
                reals[i] = Math.Sqrt(reals[i] * reals[i] + imags[i] * imags[i]);
            return reals;
        }

        static ComplexFourierTransformation cft = new ComplexFourierTransformation(TransformationConvention.Matlab);
        public static void IFFT(Complex[] xdata, int m)
        {
            var temp = new MathNet.Numerics.Complex[(int)Math.Pow(2, m)];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i].Real = xdata[i].Real;
                temp[i].Imag = xdata[i].Imag;
            }

            cft.TransformBackward(temp);

            for (int i = 0; i < temp.Length; i++)
            {
                xdata[i].Real = temp[i].Real;
                xdata[i].Imag = temp[i].Imag;
            }
        }
        #endregion
    }
}
