#define USE_VIBEC
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using data_type = System.Single;
using Real = System.Single;
using MathNet.Numerics.Transformations;

#if USE_VIBEC
using Complex = TSICommon.NadaMath.complex;
using Complex32 = TSICommon.NadaMath.complex;

#else
using Complex = MathNet.Numerics.Complex;
using Complex32 = MathNet.Numerics.Complex;
#endif

namespace TSICommon
{
    public enum WindowFunction
    {
        Hanning,
        Rectangle,
        FlatTop,
        Hamming,
        Triangle,
        Blackman,
        Harris,
        NoWindow
    }

    public enum Unit2Type
    {
        rms = 0,
        pk = 1,
        pp = 2
        //[Description("pk/rms")] pk_rms,
    }

    //DB마이그레이션을 위해 CMS와 일치하도록 맞추었음
    public enum RobinIntegralType
    {
        None = 0,
        Single = 1,
        Double = 2,
        Triple = 3
    }
    public enum RobinBandType
    {
        Frequency = 0,
        Order = 1,
        //Envelop = 2,
        //EnvelopOrder = 3
    }

    //DB마이그레이션을 위해 CMS와 일치하도록 맞추었음
    public enum RobinMeasureType
    {
        Bandpass = 0,
        OrderBandpass = 1,
        EnvelopBandpass = 2,
        CrestFactor = 3,
        Peak = 4,
        DC = 5,
        EnvelopOrderBandpass = 6,
        PtoP = 7,
        Rpm = 10,
        Torque = 11,
        Custom = 100,
    }

    public enum ChannelType
    {
        Keyphasor = 100,
        Displacement = 201,         //변위
        AbsoluteDisplacement = 202, //절대 변위
        Accelate = 205,             //가속도
        DC = 203,
        Thrust = 204,
        Eccentricity = 206,
        DiffExp = 207,
        DiffRamp = 208,
        CaseExp = 209,
        ReverseRotation = 210,
        Robin = 300,
    }

    public static class NadaMath
    {
        public const double UNIT_CONVERT_MMS_GAL = 1.0 / 980.0; // 가속도(m/s2)을 gal로 변환

        [StructLayout(LayoutKind.Sequential)]
        public struct complex : IEquatable<complex>
        {
            public data_type Real;
            public data_type Imag;

            public data_type real { get { return Real; } set { Real = value; } }
            public data_type imag { get { return Imag; } set { Imag = value; } }

            public complex(data_type r, data_type i)
            {
                Real = r;
                Imag = i;
            }

            public bool Equals(complex other)
            {
                return Real == other.Real && Imag == other.Imag;
            }

        }

        //public struct complex
        //{
        //    public data_type Real;
        //    public data_type Imag;

        //    public complex(data_type r, data_type i)
        //    {
        //        Real = r;
        //        Imag = i;
        //    }
        //}
#if !USE_VIBEC
        static ComplexFourierTransformation cft = new ComplexFourierTransformation(TransformationConvention.Matlab);
        public static void FFT(Complex[] xdata)
        {
            Debug.WriteLine("FFT data:" + xdata.Length);
            cft.TransformForward(xdata);
        }
        public static void FFT(Complex[] xdata, int m)
        {
            Debug.WriteLine("FFT data:" + xdata.Length + ", m:" + m);
            cft.TransformForward(xdata);
            //fixed (complex* x = xdata)
            //    FFT(x, m);
        }

#else
#if true

        static RealFourierTransformation rft = new RealFourierTransformation(TransformationConvention.NoScaling);

        public static double[] FFT(float[] floats)
        {
            double[] xdata = new double[floats.Length];
            for (int i = 0; i < floats.Length; i++)
                xdata[i] = floats[i];

            return FFT(xdata);
        }
        public static double[] FFT(double[] xdata)
        {
            double[] reals, imags;
            rft.TransformForward(xdata, out reals, out imags);
            for (int i = 0; i < reals.Length; i++)
                reals[i] = Math.Sqrt(reals[i] * reals[i] + imags[i] * imags[i]);
            return reals;
        }
        public static double[] PositiveFFT(float[] floats)
        {
            var N = floats.Length;
            var rawfft = FFT(floats);
            //var result = rawfft.Take(N / 2).Select(f => (f / N)*2).ToArray();     //스케일링
            var normalizer = 4.0 / N;
            var result = rawfft.Take(N / 2).Select(f => f * normalizer).ToArray();  //스케일링 최적화
            return result;
        }
        public static double[] PositiveFFT(double[] floats)
        {
            var N = floats.Length;
            var rawfft = FFT(floats);
            //var result = rawfft.Take(N / 2).Select(f => (f / N)*2).ToArray();     //스케일링
            var normalizer = 4.0 / N;
            var result = rawfft.Take(N / 2).Select(f => f * normalizer).ToArray();  //스케일링 최적화
            return result;
        }

        static ComplexFourierTransformation cft = new ComplexFourierTransformation(TransformationConvention.Matlab);
        public static void FFT(Complex[] xdata)
        {
             var temp = new MathNet.Numerics.Complex[xdata.Length];
             for (int i = 0; i < temp.Length; i++)
             {
                 temp[i].Real = xdata[i].Real;
                 temp[i].Imag = xdata[i].Imag;
             }

            cft.TransformForward(temp);

            for (int i = 0; i < temp.Length; i++)
            {
                xdata[i].Real = (data_type)temp[i].Real;
                xdata[i].Imag = (data_type)temp[i].Imag;
            }
        }

        public static void FFT(Complex[] xdata, int m)
        {
            var temp = new MathNet.Numerics.Complex[(int)Math.Pow(2, m)];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i].Real = xdata[i].Real;
                temp[i].Imag = xdata[i].Imag;
            }

            cft.TransformForward(temp);

            for (int i = 0; i < temp.Length; i++)
            {
                xdata[i].Real = (data_type)temp[i].Real;
                xdata[i].Imag = (data_type)temp[i].Imag;
            }
        }

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
                xdata[i].Real = (data_type)temp[i].Real;
                xdata[i].Imag = (data_type)temp[i].Imag;
            }
        }

        public static void FFT(MathNet.Numerics.Complex[] xdata, int m)
        {
            var temp = new MathNet.Numerics.Complex[(int)Math.Pow(2, m)];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i].Real = xdata[i].Real;
                temp[i].Imag = xdata[i].Imag;
            }

            cft.TransformForward(temp);

            for (int i = 0; i < temp.Length; i++)
            {
                xdata[i].Real = (data_type)temp[i].Real;
                xdata[i].Imag = (data_type)temp[i].Imag;
            }
        }
        public static void FFT(MathNet.Numerics.Complex[] xdata)
        {
            cft.TransformForward(xdata);
        }
#else
        static complex[] wdata;
        static int mstore = 0;
        static int n = 1;

        public static unsafe void FFT(complex[] xdata, int m)
        {
            fixed (complex* x = xdata)
                FFT(x, m);
        }

        public static void FFT(complex[] xdata)
        {
            FFT(xdata, (int)Math.Log(xdata.Length, 2));
        }

        public static unsafe void FFT(complex* x, int m)// m : length, integer power of 2, index starts from 0
        {
            complex u, temp = new complex(), tm = new complex();
            complex* xip, wptr;

            int i, j, k, l, le, windex;

            data_type arg, w_real, w_imag, wrecur_real, wrecur_imag, wtemp_real;

            if (m != mstore)
            {
                if (mstore != 0) wdata = null;
                mstore = m;
                if (m == 0) return;

                n = 1 << m;
                le = n/2;

                wdata = new complex[le - 1];

                arg = (data_type) (Math.PI/le);
                wrecur_real = w_real = (data_type) Math.Cos((data_type) arg);
                wrecur_imag = w_imag = (data_type) (-Math.Sin((data_type) arg));

                fixed (complex* w = wdata)
                {
                    complex* xj = w;
                    for (j = 1; j < le; j++)
                    {
                        xj->real = (data_type) wrecur_real;
                        xj->imag = (data_type) wrecur_imag;
                        xj++;
                        wtemp_real = wrecur_real*w_real - wrecur_imag*w_imag;
                        wrecur_imag = wrecur_real*w_imag + wrecur_imag*w_real;
                        wrecur_real = wtemp_real;
                    }
                }
            }

            le = n;
            windex = 1;
            for (l = 0; l < m; l++)
            {
                le = le/2;
                for (i = 0; i < n; i = i + 2*le)
                {
                    var xi = x + i;
                    xip = xi + le;
                    temp.real = xi->real + xip->real;
                    temp.imag = xi->imag + xip->imag;
                    xip->real = xi->real - xip->real;
                    xip->imag = xi->imag - xip->imag;
                    *xi = temp;
                }

                //fixed (complex* w = wdata)
                //{
                //    wptr = w + windex - 1;
                //    for (j = 1; j < le; j++)
                //    {
                //        u = *wptr;
                //        for (i = j; i < n; i = i + 2*le)
                //        {
                //            var xi = x + i;
                //            xip = xi + le;
                //            temp.real = xi->real + xip->real;
                //            temp.imag = xi->imag + xip->imag;
                //            tm.real = xi->real - xip->real;
                //            tm.imag = xi->imag - xip->imag;
                //            xip->real = tm.real*u.real - tm.imag*u.imag;
                //            xip->imag = tm.real*u.imag + tm.imag*u.real;
                //            *xi = temp;
                //        }
                //        wptr = wptr + windex;
                //    }
                //}

                int wptri = windex - 1;
                for (j = 1; j < le; j++)
                {
                    u = wdata[wptri];
                    for (i = j; i < n; i = i + 2 * le)
                    {
                        var xi = x + i;
                        xip = xi + le;
                        temp.real = xi->real + xip->real;
                        temp.imag = xi->imag + xip->imag;
                        tm.real = xi->real - xip->real;
                        tm.imag = xi->imag - xip->imag;
                        xip->real = tm.real * u.real - tm.imag * u.imag;
                        xip->imag = tm.real * u.imag + tm.imag * u.real;
                        *xi = temp;
                    }
                    wptri += windex;
                }

                windex = 2*windex;
            }
            j = 0;
            for (i = 1; i < (n - 1); i++)
            {
                k = n/2;
                while (k <= j)
                {
                    j = j - k;
                    k = k/2;
                }
                j = j + k;
                if (i < j)
                {
                    var xi = x + i;
                    var xj = x + j;
                    temp = *xj;
                    *xj = *xi;
                    *xi = temp;
                }
            }
        }
#endif
#endif
        //public static int curIntegral;
        public static double RMS;


        #region 윈도우 함수들

        static double[] Hann(int length)
        {
            var y = new double[length];
            Hann(y);
            return y;
        }

        //해닝 윈도우
        static void Hann(double[] y)
        {
            int length = y.Length;
            int i;
            double coef;

            coef = 2.0 * Math.PI / ((double)(length - 1));

            for (i = 0; i < length; i++)
            {
                y[i] = 0.5 * (1.0 - Math.Cos(coef * ((double)i)));
            }
        }

        static void RectangleWindow(double[] y)
        {
            for (int i = 0; i < y.Length; i++)
                y[i] = 1;
        }
        static void FlatTopWindow(double[] y)
        {
            var factor = 2.0 * Math.PI / (double)(y.Length - 1);

            for (int i = 0; i < y.Length; i++)
            {
                double result = 1 - 1.933 * Math.Cos(factor * i) + 1.286 * Math.Cos(2 * factor * i) - 0.388 * Math.Cos(3 * factor * i) + 0.032 * Math.Cos(4 * factor * i);
                y[i] = result;
            }
        }
        static void HammingWindow(double[] y)
        {
            int n = y.Length;
            double factor = 8.0 * Math.Atan(1.0) / (n - 1);
            for (int i = 0; i < n; i++)
            {
                //double ham = 0.54 - 0.46 * Math.Cos(factor * i);
                double ham = (0.54 - 0.46 * Math.Cos(factor * i))/0.54;
                y[i] = ham;
            }
        }

        static void TriangleWindow(double[] y)
        {
            int length = y.Length;
            int i;
            double tri;
            double a = 2.0 / (length - 1);

            for (i = 0; i <= (length - 1) / 2; i++)
            {
                tri = i * a;
                y[i] = tri;
            }
            for (; i < length; i++)
            {
                tri = 2.0 - i * a;
                y[i] = tri;
            }
        }

        static void BlackmanWindow(double[] y)
        {
            int length = y.Length;
            double factor = 8.0 * Math.Atan(1.0) / (length - 1);
            for (int i = 0; i < length; ++i)
            {
                double black = 0.42 - 0.5 * Math.Cos(factor * i) + 0.08 * Math.Cos(2 * factor * i);
                y[i] = black;
            }
        }

        static void HarrisWindow(double[] y)
        {
            int length = y.Length;
            double factor = 8.0 * Math.Atan(1.0) / length;
            for (int i = 0; i < length; ++i)
            {
                double arg = factor * i;
                double harris = 0.35875 - 0.48829 * Math.Cos(arg) + 0.14128 * Math.Cos(2 * arg) - 0.01168 * Math.Cos(3 * arg);
                y[i] = harris;
            }
        }

        public static double[] CreateWindow(WindowFunction nWindowFn, int length)
        {
            double[] window = new double[length];
            switch (nWindowFn)
            {
                case WindowFunction.Hanning: Hann(window); break;
                case WindowFunction.Rectangle: RectangleWindow(window); break;
                case WindowFunction.FlatTop: FlatTopWindow(window); break;
                case WindowFunction.Hamming: HammingWindow(window); break;
                case WindowFunction.Triangle: TriangleWindow(window); break;
                case WindowFunction.Blackman: BlackmanWindow(window); break;
                case WindowFunction.Harris: HarrisWindow(window); break;
                case WindowFunction.NoWindow: RectangleWindow(window); break;
            }
            return window;
        }

        class WindowPerLength :Dictionary<int,double[]> {}
        static Dictionary<WindowFunction, WindowPerLength> windowCache = new Dictionary<WindowFunction, WindowPerLength>();
        public static double[] GetWindow(WindowFunction func, int length)
        {
            WindowPerLength windowPerLength;
            if (!windowCache.ContainsKey(func))
            {
                var window = CreateWindow(func, length);
                windowPerLength = new WindowPerLength();
                windowPerLength.Add(length, window);
                windowCache.Add(func, windowPerLength);
                return window;
            }

            windowPerLength = windowCache[func];
            if (!windowPerLength.ContainsKey(length))
            {
                var window = CreateWindow(func, length);
                windowPerLength.Add(length, window);
                return window;
            }

            return windowPerLength[length];
        }
        #endregion

        //static UnmanagedLibrary vibecDll;
        //[UnmanagedFunctionPointer(CallingConvention.Winapi)]
        //delegate Real BPF_RMSFunc(Real[] asyncData, int length, Real[] f, Real[] y, Real f1, Real f2, Real fs);
        //static BPF_RMSFunc pfBPF_RMS;
        //public static double BPF_RMS(data_type[] x, double[] f, double[] y, double f1, double f2, double Fs)//length = 2^m, the length of h and x = length, the length of f and y = length/2
        //{
        //    if (vibecDll == null)
        //    {
        //        vibecDll = new UnmanagedLibrary("VibeC.dll");
        //        pfBPF_RMS = vibecDll.GetUnmanagedFunction<BPF_RMSFunc>("BPF_RMS");
        //    }

        //    var tempf = new data_type[f.Length];
        //    var tempy = new data_type[y.Length];
        //    var result = pfBPF_RMS(x, x.Length, tempf, tempy, (data_type)f1, (data_type)f2, (data_type)Fs);

        //    for (int i = 0; i < f.Length; i++)
        //    {
        //        f[i] = tempf[i];
        //        y[i] = tempy[i];
        //    }

        //    return result;
        //}
        
//        //x:async데이타 , f:주파수 length/2개 얻음, y 주파수별 rms, f1: band low 주파수 시작점(Hz), f2: band high주파수 시작점(Hz), Fs: 보드 샘플링 레이트
////plot f, y 하면 바로 스펙트럼
        public static double BPF_RMS(data_type[] x, double[] f, double[] y, double f1, double f2, double Fs, Unit2Type Unit2Type, WindowFunction windowFunc)//length = 2^m, the length of h and x = length, the length of f and y = length/2
        {
            int length = x.Length;
            int i;
            double df, sum, rms;
            int k1, k2, N, half_length;
            var XH = new Complex[65536];
            double scale;
            //var h = Hann(x.Length);
            var h = GetWindow(windowFunc, x.Length);

            df = Fs / ((double)length);
            k1 = (int)Math.Floor(f1 / df) - 1;
            k2 = (int)Math.Ceiling(f2 / df) + 1;

            half_length = length >> 1;

            scale = 0.0;
            for (i = 0; i < length; i++)
            {
                scale = scale + h[i] * h[i];
            }

            scale = ((double)length) * scale;
            scale = 1.0 / scale;

            if (k1 < 0)
                k1 = 0;
            else if (k1 > half_length)
                k1 = half_length;

            if (k2 < 0)
                k2 = 0;
            else if (k2 > half_length)
                k2 = half_length;

            for (i = 0; i < length; i++)
            {
                XH[i].Real = (data_type)(h[i] * x[i]);
                XH[i].Imag = 0.0f;
            }

            N = (int)Math.Round(Math.Log10((double)length) / Math.Log10(2.0));
            FFT(XH, N);

            //RMS scalar generation
            sum = 0.0;
            for (i = k1; i <= k2; i++)
            {
                sum = sum + XH[i].Real * XH[i].Real + XH[i].Imag * XH[i].Imag;
            }

            rms = Math.Sqrt(scale * 2.0 * sum);

            //RMS vector generation
            scale = 0.0;
            for (i = 0; i < length; i++)
            {
                scale = scale + h[i];
            }

            scale = 1.414214 / scale; //*((double)length);

            switch (Unit2Type)
            {
                case Unit2Type.pk: scale = scale * 1.414213; break;
                case Unit2Type.pp: scale = scale * 2.828427; break;
                default: break;
            }


            for (i = 0; i < half_length; i++)
            {
                f[i] = df * ((double)i);
                y[i] = scale * Math.Sqrt(XH[i].Real * XH[i].Real + XH[i].Imag * XH[i].Imag);
            }
            RMS = rms;
            return rms;
        }

        //used in DspAnalyzer
        public static double BPF_RMS(data_type[] x, data_type[] f, data_type[] y, double f1, double f2, double Fs, Unit2Type? unit2Type = null)//length = 2^m, the length of h and x = length, the length of f and y = length/2
        {
            int length = x.Length;
            int i;
            double df, sum, rms;
            int k1, k2, N, half_length;
            var XH = new Complex[65536];
            double scale;
            var h = Hann(x.Length);

            df = Fs / ((double)length);
            k1 = (int)Math.Floor(f1 / df) - 1;
            k2 = (int)Math.Ceiling(f2 / df) + 1;

            half_length = length >> 1;

            scale = 0.0;
            for (i = 0; i < length; i++)
            {
                scale = scale + h[i] * h[i];
            }

            scale = ((double)length) * scale;
            scale = 1.0 / scale;

            if (k1 < 0)
                k1 = 0;
            else if (k1 > half_length)
                k1 = half_length;

            if (k2 < 0)
                k2 = 0;
            else if (k2 > half_length)
                k2 = half_length;

            for (i = 0; i < length; i++)
            {
                XH[i].Real = (data_type)(h[i] * x[i]);
                XH[i].Imag = 0.0f;
            }

            N = (int)Math.Round(Math.Log10((double)length) / Math.Log10(2.0));
            FFT(XH, N);

            //RMS scalar generation
            sum = 0.0;
            for (i = k1; i <= k2; i++)
            {
                sum = sum + XH[i].Real * XH[i].Real + XH[i].Imag * XH[i].Imag;
            }

            rms = Math.Sqrt(scale * 2.0 * sum);
            //RMS vector generation
            scale = 0.0;
            for (i = 0; i < length; i++)
            {
                scale = scale + h[i];
            }

            scale = 1.414214 / scale; //*((double)length);

            if (unit2Type.HasValue)
            {
                switch (unit2Type.Value)
                {
                    case Unit2Type.pk: scale = scale * 1.414213; break;
                    case Unit2Type.pp: scale = scale * 2.828427; break;
                    default: break;
                }
            }

            for (i = 0; i < half_length; i++)
            {
                f[i] = (data_type)(df * ((double)i));
                y[i] = (data_type)(scale * Math.Sqrt(XH[i].Real * XH[i].Real + XH[i].Imag * XH[i].Imag));
            }
            RMS = rms;
            return rms;
        }


        static double scale;
        //static readonly double[] h = Hann(65536);
        static double df;
        static int length_saved = 0;

        //line = N/2.56
        /// <summary>
        /// range of f : -Line*df ~ Line*df   where df = Fs/length
        /// </summary>
        /// <param name="f">frequency 배열. length: 2*line + 1</param>
        /// <param name="y">스펙트럼값 배열. length: 2*line + 1</param>
        /// <param name="Fs"></param>
        public static void FullSpectrum_RMS(data_type[] x1, data_type[] x2, double[] f, double[] y, double Fs, Unit2Type Unit2Type, WindowFunction windowFunc)//length = 2^m, the length of h and x = length, the length of f and y = length/2
        {
            int length = x1.Length;
            int i;
            double sum, rms;
            int k1, k2, N, half_length;
            var XH = new Complex[65536];
            double scale_rms_vec;
            int line, count;
            var h = GetWindow(windowFunc, length);

            line = (int) Math.Round((double) length/2.56);

            if (length != length_saved)
            {
                df = Fs/((double) length);

                scale = 0.0;
                for (i = 0; i < length; i++)
                {
                    scale = scale + h[i]*h[i];

                }

                scale = ((double) length)*scale;
                scale = 1.0/scale;
                length_saved = length;
            }
            //		k1 = floor(f1/df) - 1;
            //		k2 = ceil(f2/df) + 1;

            half_length = length >> 1;

            //		if(k1 < 0)
            //			k1 = 0;
            //		else if(k1 > half_length)
            //			k1 = half_length;
            //		
            //		if(k2 < 0)
            //			k2 = 0;
            //		else if(k2 > half_length)
            //			k2 = half_length;

            for (i = 0; i < length; i++)
            {
                XH[i].Real = (data_type) (h[i]*x1[i]);
                XH[i].Imag = (data_type) (h[i]*x2[i]);
            }

            N = (int) Math.Round(Math.Log10((double) length)/Math.Log10(2.0));
            FFT(XH, N);

            //RMS scalar generation
            //		sum = 0.0;
            //		for(i = k1; i <= k2; i++)
            //		{	
            //			sum = sum + XH[i].real*XH[i].real + XH[i].imag*XH[i].imag; 		
            //		}
            //		
            //		rms = sqrt(scale*2.0*sum);
            //RMS vector generation
            scale = 0.0;
            for (i = 0; i < length; i++)
            {
                scale = scale + h[i];
            }

            scale = 1.0 / 1.414214 / (scale);

            switch (Unit2Type)
            {
                case Unit2Type.pk: scale = scale * 1.414213; break;
                case Unit2Type.pp: scale = scale * 2.828427; break;
                default: break;
            }
        
            count = 0;
            for (i = -line; i <= line; i++)
            {
                f[count] = df*((double) i);
                count++;
            }

            count = 0;
            for (i = length - line; i <= length - 1; i++)
            {
                y[count] = scale * Math.Sqrt(XH[i].Real * XH[i].Real + XH[i].Imag * XH[i].Imag);
                count++;
            }
            for (i = 0; i <= line; i++)
            {
                y[count] = scale * Math.Sqrt(XH[i].Real * XH[i].Real + XH[i].Imag * XH[i].Imag);
                count++;
            }
        }

#if false
        static double[] b_dec = { 0, -0.036913, 0, 0.013762, 0, -0.016713, 0, 0.020483, 0, -0.025495, 
                                    0, 0.032496, 0, -0.043186, 0, 0.06201, 0, -0.1051, 0, 0.31802,
                                    0.5, 0.31802, 0, -0.1051, 0, 0.06201, 0, -0.043186, 0, 0.032496, 
                                    0, -0.025495, 0, 0.020483, 0, -0.016713, 0, 0.013762, 0, -0.036913, 0 };

        static double[,] buffer_dec = new double[256, 41];
        staticDictionary<int,data_type[]> decimateBuff = newDictionary<int,data_type[]>();

        static int Decimation2(data_type[] x, data_type[] y, int channelId, int loopIndex)//the length of Y = 0.5*length of x
        {
            int length = x.Length;
            int instance_number = channelId * 16 + loopIndex;
            //SystemExtension.FindOrAdd(decimateBuff, instance_number, new data_type[]);
            var mybuff = decimateBuff.FindOrAdd(instance_number, new data_type[41]);

            int i, j;
            int even_flag = 1;
            int index_y = 0;

            for (i = 0; i < length; i++)
            {
                for (j = 40; j >= 1; j--)
                {
                    mybuff[j] = mybuff[j - 1];
                    //				mexPrintf("%f\n", buffer_dec[40]);
                }
                mybuff[0] = x[i];

                if (even_flag == 1)
                {
                    y[index_y] = (data_type)(b_dec[1] * mybuff[1] + b_dec[3] * mybuff[3] + b_dec[5] * mybuff[5] + b_dec[7] * mybuff[7] + b_dec[9] * mybuff[9]
                                 + b_dec[11] * mybuff[11] + b_dec[13] * mybuff[13] + b_dec[15] * mybuff[15] + b_dec[17] * mybuff[17] + b_dec[19] * mybuff[19]
                                 + b_dec[20] * mybuff[20] + b_dec[21] * mybuff[21] + b_dec[23] * mybuff[23] + b_dec[25] * mybuff[25] + b_dec[27] * mybuff[27] + b_dec[29] * mybuff[29]
                                 + b_dec[31] * mybuff[31] + b_dec[33] * mybuff[33] + b_dec[35] * mybuff[35] + b_dec[37] * mybuff[37] + b_dec[39] * mybuff[39]);
                    index_y++;
                    even_flag = 0;
                }
                else
                {
                    even_flag = 1;
                }
            }
            return length / 2;
        }

        //static int Decimation2_original(data_type[] x, data_type[] y, int channelIndex, int loopIndex)//the length of Y = 0.5*length of x
        //{
        //    int length = x.Length;
        //    int instance_number = channelIndex*16 + loopIndex;

        //    int i, j;
        //    int even_flag = 1;
        //    int index_y = 0;

        //    for (i = 0; i < length; i++)
        //    {
        //        for (j = 40; j >= 1; j--)
        //        {
        //            buffer_dec[instance_number, j] = buffer_dec[instance_number, j - 1];
        //            //				mexPrintf("%f\n", buffer_dec[40]);
        //        }
        //        buffer_dec[instance_number, 0] = x[i];

        //        if (even_flag == 1)
        //        {
        //            y[index_y] = (data_type)(b_dec[1]*buffer_dec[instance_number, 1] + b_dec[3]*buffer_dec[instance_number, 3] + b_dec[5]*buffer_dec[instance_number, 5] + b_dec[7]*buffer_dec[instance_number, 7] + b_dec[9]*buffer_dec[instance_number, 9]
        //                         + b_dec[11]*buffer_dec[instance_number, 11] + b_dec[13]*buffer_dec[instance_number, 13] + b_dec[15]*buffer_dec[instance_number, 15] + b_dec[17]*buffer_dec[instance_number, 17] + b_dec[19]*buffer_dec[instance_number, 19]
        //                         + b_dec[20]*buffer_dec[instance_number, 20] + b_dec[21]*buffer_dec[instance_number, 21] + b_dec[23]*buffer_dec[instance_number, 23] + b_dec[25]*buffer_dec[instance_number, 25] + b_dec[27]*buffer_dec[instance_number, 27] + b_dec[29]*buffer_dec[instance_number, 29]
        //                         + b_dec[31]*buffer_dec[instance_number, 31] + b_dec[33]*buffer_dec[instance_number, 33] + b_dec[35]*buffer_dec[instance_number, 35] + b_dec[37]*buffer_dec[instance_number, 37] + b_dec[39]*buffer_dec[instance_number, 39]);
        //            index_y++;
        //            even_flag = 0;
        //        }
        //        else
        //        {
        //            even_flag = 1;
        //        }
        //    }
        //    return length/2;
        //}

        public static data_type[] Decimation2(data_type[] x, int channelId, int loopIndex)
        {
            var result = new data_type[x.Length/2];
            Decimation2(x, result, channelId, loopIndex);
            return result;
        }
#endif

        public static int Round(double val)
        {
            double temp = Math.Floor(val);
            if (val >= 0)
            {
                if (val - temp >= 0.5)
                    return (int)(temp + 1.0);
                else
                    return (int)(temp);
            }
            else
            {
                if (val - temp > 0.5)
                    return (int)(temp + 1.0);
                else
                    return (int)(temp);
            }
        }

        static readonly data_type[] si_transducer_unit = { 1, 1e-6f, 1e-3f, 2.54e-2f, 2.54e-5f, 1e-3f, 2.54e-2f, 1, 9.80665f };//related with enum type in nada_unit.h
        static readonly data_type[] transducer_si_unit = { 1, 1e6f, 1e3f, 3.937007874e1f, 3.937007874e4f, 1e3f, 3.937007874e1f, 1, 1.01971621298e-1f };
        const data_type ad_scale = 5000.0f / 32768.0f;
        //static data_type[] hw_gain = { 1, 2, 5, 10, 20, 50, 100, 200 };
        //static data_type[] hw_gain_inv = { 1, 0.5f, 0.2f, 0.1f, 0.05f, 0.02f, 0.01f, 0.005f };
        
        public static data_type ScaleFactor(data_type sensitivity, int hw_gain, int transducer_unit, int display_unit)
        {
            data_type result = 1.0f / sensitivity / hw_gain * ad_scale * si_transducer_unit[transducer_unit] * transducer_si_unit[display_unit];
            return result;
        }

        //public static data_type ScaleFactorByDisplayUnit(this TransducerChannel ch)
        //{
        //    return ScaleFactor(ch.GetCorrectedSensitivity(), ch.HWGain, (int)ch.TransducerUnit, (int)ch.DisplayUnit);
        //}
        //public static data_type ScaleFactorForRobin(this TransducerChannel ch)
        //{
        //    data_type result = 1.0f / ch.GetCorrectedSensitivity();
        //    return result;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="theta"></param>
        /// <param name="grid_intervals"></param>
        /// <returns>인덱스배열</returns>
        public static int[] SelectPoints3(double[] r, double[] theta, int grid_intervals)
        {
            double xmin, xmax, ymin, ymax;
            double dx, dy;
            double margin = 0.1;//20% margin x1+0.1dx~x2-0.1dx 
            double xmargin, ymargin;
            
            double[] x = new double[r.Length];
            double[] y = new double[theta.Length];
            for (int i = 0; i < r.Length; i++)
            {
                x[i] = r[i]*Math.Cos(theta[i]*Math.PI/180.0);
                y[i] = r[i] * Math.Sin(theta[i] * Math.PI / 180.0);
            }
            if (x.Length != y.Length)
            {
                throw new Exception("Size mismatch");
                //return null; //error size mismatch
            }
            double[] xgrid_s = new double[grid_intervals];
            double[] xgrid_e = new double[grid_intervals];
            double[] ygrid_s = new double[grid_intervals];
            double[] ygrid_e = new double[grid_intervals];
            double[] xc = new double[grid_intervals];
            double[] yc = new double[grid_intervals];
            double[,] xsave = new double[grid_intervals, grid_intervals];
            double[,] ysave = new double[grid_intervals, grid_intervals];
            var index = new int[grid_intervals, grid_intervals];
            int[,] save_flag = new int[grid_intervals, grid_intervals];

            //double xmargin, ymargin;

            //int[,] selection_count = new int[grid_intervals, grid_intervals];

            int count = 0;
            double dist1, dist2;
            xmax = xmin = x[0];
            ymax = ymin = y[0];

            for (int i = 1; i < x.Length; i++)
            {
                if (xmax < x[i])
                    xmax = x[i];
                if (xmin > x[i])
                    xmin = x[i];
                if (ymax < y[i])
                    ymax = y[i];
                if (ymin > y[i])
                    ymin = y[i];
            }

            dx = (xmax - xmin) / (double)grid_intervals;
            dy = (ymax - ymin) / (double)grid_intervals;
            xmargin = margin * dx * 0.5;
            ymargin = margin * dy * 0.5;

            for (int i = 0; i < grid_intervals; i++)//grid with margin generation 
            {
                xgrid_s[i] = xmin + dx * (double)i + xmargin;
                xgrid_e[i] = xmin + dx * (double)(i + 1) - xmargin;
                ygrid_s[i] = ymin + dy * (double)i + ymargin;
                ygrid_e[i] = ymin + dy * (double)(i + 1) - ymargin;
                xc[i] = 0.5 * (xgrid_s[i] + xgrid_e[i]);
                yc[i] = 0.5 * (ygrid_s[i] + ygrid_e[i]);
            }

            for (int i = 0; i < x.Length; i++)
            {

                for (int j = 0; j < grid_intervals; j++)
                {
                    if (x[i] >= xgrid_s[j] && x[i] <= xgrid_e[j])
                    {
                        for (int k = 0; k < grid_intervals; k++)
                        {
                            if (y[i] >= ygrid_s[k] && y[i] <= ygrid_e[k])
                            {
                                if (save_flag[j, k] == 0)
                                {
                                    xsave[j, k] = x[i];
                                    ysave[j, k] = y[i];
                                    index[j, k] = i;
                                    save_flag[j, k] = 1;
                                    count++;
                                }
                                else
                                {
                                    dist1 = (xsave[j, k] - xc[j]) * (xsave[j, k] - xc[j]) + (ysave[j, k] - yc[k]) * (ysave[j, k] - yc[k]);
                                    dist2 = (x[i] - xc[j]) * (x[i] - xc[j]) + (y[i] - yc[k]) * (y[i] - yc[k]);
                                    if (dist1 > dist2)
                                    {
                                        xsave[j, k] = x[i];
                                        ysave[j, k] = y[i];
                                        index[j, k] = i;
                                    }
                                }
                            }
                        }
                    }
                }
            }


            var result_index = new int[count];

            count = 0;
            for (int i = 0; i < grid_intervals; i++)
            {
                for (int j = 0; j < grid_intervals; j++)
                {
                    if (save_flag[i, j] != 0)
                    {
                        result_index[count] = index[i, j];
                        count++;
                    }
                }
            }

            return result_index;
        }

        public static int GalToMMIScale(double gal)
        {
            int mmi = 0;    //Modified Mercali Intensity Scale: 미국에서 사용하는 진도계급
            if (MathEx.InRange(gal, 1.0, 2.0))
                mmi = 1;
            else if (MathEx.InRange(gal, 2.0, 5.0))
                mmi = 2;
            else if (MathEx.InRange(gal, 5.0, 15.0))
                mmi = 3;
            else if (MathEx.InRange(gal, 15.0, 30.0))
                mmi = 4;
            else if (MathEx.InRange(gal, 30.0, 60.0))
                mmi = 5;
            else if (MathEx.InRange(gal, 60.0, 100.0))
                mmi = 6;
            else if (MathEx.InRange(gal, 100.0, 250.0))
                mmi = 7;
            else if (MathEx.InRange(gal, 250.0, 500.0))
                mmi = 8;
            else if (MathEx.InRange(gal, 500.0, 600.0))
                mmi = 9;
            else if (gal >= 600)
                mmi = 10;

            return mmi;
        }
    }

}
