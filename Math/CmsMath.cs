using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using complex = TSICommon.NadaMath.complex;
using COMPLEX = TSICommon.NadaMath.complex;
using Real = System.Single;

namespace TSICommon
{
    public static class CmsMath
    {
        internal static double pow(int n, int m) { return Math.Pow(n,m); }
        internal static double atan(double d) { return Math.Atan(d); }
        internal static double sin(double a) { return Math.Sin(a); }
        internal static double cos(double a) { return Math.Cos(a); }
        internal static float fabs(float a) { return Math.Abs(a); }
        internal static double fabs(double a) { return Math.Abs(a); }
        internal static double sqrt(double a) { return Math.Sqrt(a); }
        internal static double log10(double a) { return Math.Log10(a); }

        internal static double Cabs(COMPLEX z)
        {
            double x, y, ans, temp;
            x = fabs(z.real);
            y = fabs(z.imag);
            if (x == 0.0)
                ans = y;
            else if (y == 0.0)
                ans = x;
            else if (x > y)
            {
                temp = y / x;
                ans = x * sqrt(1.0 + temp * temp);
            }
            else
            {
                temp = x / y;
                ans = y * sqrt(1.0 + temp * temp);
            }
            return ans;
        }

        internal static void Fill<T>(this T[] array, T value)
        {
            Fill(array, value, array.Length);
        }
        internal static void Fill<T>(this T[] array, T value, int length)
        {
            for (int i = 0; i < length; i++)
                array[i] = value;
        }


        static void rfft(Real[] Sig, complex[] y, int m)
        {
            int num, k;
            double Realsum, Realdif, Imagsum, Imagdif;
            double factor, arg;

            int fft_size = (int)pow(2, m);
            //double[] x = new double[fft_size]; //(double *)calloc( fft_size, sizeof(double) );	// Freed
            complex[] cx = new complex[fft_size];
            //Buffer.BlockCopy(Sig, 0, cx, 0, sizeof(double) * fft_size);//memcpy(x, Sig, sizeof(double)*fft_size);
            for (int i = 0; i < fft_size; i++)
                cx[i].Real = Sig[i];

            // First call the fft routine using the x array but with
            //half the size of the real fft 
            int p = m - 1;
            //cx = (complex *) x;
            //fft(cx,p);
            NadaMath.FFT(cx, p);

            // Next create the coefficients for recombination, if required 

            num = 1 << p;    // num is half the real sequence length.  

            //TODO: rfft_cf를 캐시하여 성능향상
            var rfft_cf = new complex[num - 1];//(complex *) calloc(num - 1,sizeof(complex));

            factor = 4.0 * atan(1.0) / num;
            for (k = 1; k < num; k++)
            {
                arg = factor * k;
                rfft_cf[k - 1].real = (float)cos(arg);
                rfft_cf[k - 1].imag = (float)sin(arg);
            }


            // DC component, no multiplies 
            y[0].real = cx[0].real + cx[0].imag;
            y[0].imag = 0.0f;

            // other frequencies by trig recombination 
            int ick = 0;//ck = cf;
            int ixk = 1;//xk = cx + 1;
            int ixnk = num - 1;//xnk = cx + num - 1;
            for (k = 1; k < num; k++)
            {
                Realsum = (cx[ixk].real + cx[ixnk].real) / 2;
                Imagsum = (cx[ixk].imag + cx[ixnk].imag) / 2;
                Realdif = (cx[ixk].real - cx[ixnk].real) / 2;
                Imagdif = (cx[ixk].imag - cx[ixnk].imag) / 2;

                y[k].real = (float)(Realsum + rfft_cf[ick].real * Imagsum - rfft_cf[ick].imag * Realdif);

                y[k].imag = (float)(Imagdif - rfft_cf[ick].imag * Imagsum - rfft_cf[ick].real * Realdif);
                ick++;
                ixk++;
                ixnk--;
            }

            //rfft_cf = null;//free(cf);
            cx = null;//free(x);
        }

        // C - Weighting Function
        static double Wc_Lin(double freq)
        {
            return 2.242881e16 * Math.Pow(freq, 4) / (Math.Pow((Math.Pow(freq, 2) + Math.Pow(20.598997, 2)), 2) * Math.Pow((Math.Pow(freq, 2) + Math.Pow(12194.22, 2)), 2));
        }
        // A - Weighting Function
        static double Wa_Lin(double freq)
        {
            double temp1;
            temp1 = 2.242881e16 * Math.Pow(freq, 4) / (Math.Pow((Math.Pow(freq, 2) + Math.Pow(20.598997, 2)), 2) * Math.Pow((Math.Pow(freq, 2) + Math.Pow(12194.22, 2)), 2));
            temp1 *= 1.562339 * Math.Pow(freq, 4);
            temp1 /= (Math.Pow(freq, 2) + Math.Pow(107.65265, 2)) * (Math.Pow(freq, 2) + Math.Pow(737.86223, 2));

            return temp1;
        }
        static double GetPeakToPeak(Real[] Signal, int nStart, int nEnd)
        {
            int i;
            double peakMax, peakMin;

            peakMax = -100000;
            peakMin = 100000;
            for (i = nStart; i < nEnd; i++)
            {
                if (peakMax < Signal[i])
                    peakMax = Signal[i];

                if (peakMin > Signal[i])
                    peakMin = Signal[i];
            }

            if ((peakMax > 0) && (peakMin > 0))
            {
                return (peakMax - peakMin);
            }
            else if ((peakMax < 0) && (peakMin < 0))
            {
                return (Math.Abs(peakMin) - Math.Abs(peakMax));

            }

            return (Math.Abs(peakMax) + Math.Abs(peakMin));
        }


        /*
        int DetectorType [In]
          0 RMS
          1 ZeroToPeak
          2 PeakToPeak
        */
        static double BandpassOverall(Real[] x, int m, Real[] Pxx, Unit2Type detector, double bp1, double bp2, double deltaf)
        {
            double freq;
            int nStep;
            double deltat;

            if (detector == Unit2Type.rms)
            {
                return autospectrum_d(x, m, Pxx, WindowFunction.NoWindow, 0, bp1, bp2, deltaf, 0, 0);
            }

            int fft_len = (int)(pow(2, m));
            COMPLEX[] FFTxx = new COMPLEX[fft_len]; //(COMPLEX*)calloc(fft_len, sizeof( COMPLEX ) );

            for (int i = 0; i < fft_len; i++)
            {
                FFTxx[i].real = (Real)x[i];
                FFTxx[i].imag = 0;
            }

            NadaMath.FFT(FFTxx, m);//fft( FFTxx, m );

            int roop = fft_len / 2;

            // Set Positve Frequency

            for (int i = 0; i < roop; i++)
            {
                freq = i * deltaf;

                if (!((bp1 <= freq) && (freq <= bp2)))
                {
                    FFTxx[i].real = 0;
                    FFTxx[i].imag = 0;
                }
            }


            int iPos = 0;
            for (int i = roop; i < fft_len; i++)
            {
                // 512, 511 .... 1

                freq = (roop - iPos) * deltaf; // Nyquist Frequency 고려
                if (!((bp1 <= freq) && (freq <= bp2)))
                {
                    FFTxx[i].real = 0;
                    FFTxx[i].imag = 0;
                }

                iPos++;
            }

            NadaMath.IFFT(FFTxx, m);  //ifft( FFTxx, m );

            for (int i = 0; i < fft_len; i++)
            {
                x[i] = FFTxx[i].real;
            }

            FFTxx = null;//free( FFTxx );

            double Overall = 0;
            switch (detector)
            {
                case Unit2Type.pk:
                    // 30 msec만 측정 하도록 프로그램 수정 삼성 BP용으로 수정 
                    deltat = 1.0 / (fft_len * deltaf);
                    nStep = (int)(0.03 / deltat);

                    for (int i = 0; i < (fft_len / nStep - 3); i++)
                    {
                        var tempOverall = GetPeakToPeak(x, i * nStep, (i + 1) * nStep) / 2.0;//GetZeroToPeak( x, fft_len*0.03);
                        Overall = Overall + tempOverall;
                    }
                    Overall = Overall / (fft_len / nStep - 3);
                    break;
                case Unit2Type.pp:
                    // 30 msec만 측정 하도록 프로그램 수정 삼성 BP용으로 수정 
                    deltat = 1.0 / (fft_len * deltaf);
                    nStep = (int)(0.03 / deltat);
                    for (int i = 0; i < (fft_len / nStep - 3); i++)
                    {
                        var tempOverall = GetPeakToPeak(x, i * nStep, (i + 1) * nStep);//GetZeroToPeak( x, fft_len*0.03);
                        Overall = Overall + tempOverall;
                    }
                    Overall = Overall / (fft_len / nStep - 3);
                    break;
            }

            return Overall;
        }

        public static double envelopBandpass(Real[] x, int m, out double[] Pxx, WindowFunction wintype, Unit2Type spectrumunit, double bp1, double bp2, double deltaf, int bDecibel, int AcouWintype, RobinIntegralType IntType)
        {
            double overall;
            
            int fft_len = (int)Math.Pow(2, m);
            int i, roop;
            /*

            complex[] FFTxx;
            double WinScale;	// for window
            double WinScale2;	// for g-RMS
            double[] window;
            double rms;
            double temp;
            double freq;
            double Overall;
            double OverallTime, tempOverall;
            double omega;

            // detectator Type 에 따른 Overall 계산
            // 30 msec만 측정 하도록 프로그램 수정 삼성 BP용으로 수정

            if ((deltaf != 0) && (spectrumunit != Unit2Type.rms))
            {
                OverallTime = 0;
                double deltat = 1.0 / (fft_len * deltaf);
                int nStep = (int)(0.03 / deltat);
                for (i = 0; i < (fft_len / nStep - 3); i++)
                {

                    tempOverall = GetPeakToPeak(x, i * nStep, (i + 1) * nStep);//GetZeroToPeak( x, fft_len*0.03);
                    OverallTime = OverallTime + tempOverall;
                }
                OverallTime = OverallTime / (fft_len / nStep - 3);
            }
            else
            {
                OverallTime = -1;
            }

            var epsilon = Math.Pow(10, -18);

            rms = 0;
            freq = 0;

            FFTxx = new complex[fft_len];   //(COMPLEX*)calloc(fft_len, sizeof(COMPLEX));
            window = NadaMath.GetWindow(wintype, fft_len);

            WinScale = 0;
            for (i = 0; i < fft_len; i++)
            {
                x[i] = (float)(x[i] * window[i]);
                WinScale += window[i];
            }

            WinScale2 = 0;
            for (i = 0; i < fft_len; i++)
            {
                WinScale2 += window[i] * window[i];
            }

            rfft(x, FFTxx, m);

            // 소음 관련 루틴 
            */

            roop = fft_len / 2;
            Pxx = new double[roop];
            
            /*
            for (i = 0; i < roop; i++)
            {
                freq = i * deltaf;

                Pxx[i] = Cabs(FFTxx[i]) * Cabs(FFTxx[i]);

                // Single side conversion 
                // scale 조정 
                // 
                if (i != 0)
                {
                    Pxx[i] *= 2;
                }

                //Pxx[i] /= ( WinScale * fft_len);
                Pxx[i] /= (WinScale * WinScale);

                // acoustic Weighting
                if (AcouWintype == 1)
                {
                    Pxx[i] = Pxx[i] * Wa_Lin(freq) * Wa_Lin(freq);
                }
                else if (AcouWintype == 3)
                {
                    Pxx[i] = Pxx[i] * Wc_Lin(freq) * Wc_Lin(freq);
                }


                temp = Pxx[i] * (WinScale * WinScale) / (fft_len * WinScale2);

                // Overall 을 구하는 루틴 
                // w = 2*pi*f로 나누어 준다.
                //  Integration Type :
                //    0 - No Integration
                //    1 - Single Integration
                //    2 - Double Integration
                if ((bp1 <= freq) && (freq <= bp2))
                {

                    if (IntType == RobinIntegralType.None)
                    {
                        rms = rms + temp;
                    }
                    else if (IntType == RobinIntegralType.Single)
                    {
                        omega = 2 * 4.0 * atan(1.0) * freq;
                        omega *= omega;
                        temp /= omega; // w^2
                        rms = rms + temp;

                    }
                    else if (IntType == RobinIntegralType.Double)
                    {
                        omega = 2 * 4.0 * atan(1.0) * freq;
                        omega = omega * omega;
                        omega = omega * omega;
                        temp /= omega; // w^4
                        rms = rms + temp;
                    }
                }

                //if (spectrumunit != 1)
                Pxx[i] = sqrt(Pxx[i]);
            }

            //if (spectrumunit != 1)
            Overall = sqrt(rms);
            //else
            //    Overall = rms;

            if (bDecibel == 1) // Decibel Scale
            {

                temp = Overall / (2 * pow(10, -5));
                temp += epsilon;
                Overall = 20 * log10(temp);

                for (i = 0; i < roop; i++)
                {
                    temp = Pxx[i] / (2 * pow(10, -5));
                    temp += epsilon;
                    Pxx[i] = 20 * log10(temp);
                }
            }

            FFTxx = null;
            //free(FFTxx);
            //free(window);

            if (spectrumunit == Unit2Type.rms)
            {
                return Overall;
            }
            else
            {
                var tRes = new Real[fft_len]; //(double*)calloc(fft_len, sizeof(double));
                Overall = BandpassOverall(x, m, tRes, spectrumunit - 1, bp1, bp2, deltaf);
                tRes = null; //free(tRes);
                if (spectrumunit == Unit2Type.pk) // 0- p
                {
                    for (i = 0; i < roop; i++)
                    {
                        Pxx[i] = Pxx[i] * sqrt(2);
                    }

                    return OverallTime / 2.0;
                }
                else if (spectrumunit == Unit2Type.pp) // p-p
                {
                    for (i = 0; i < roop; i++)
                    {
                        Pxx[i] = Pxx[i] * sqrt(2) * 2;
                    }

                    return OverallTime;
                }
                else
                {
                    return -1;
                }
            }
            */
            Random rnd = new Random();
            overall = (rnd.NextDouble() + 0.1) / 2.0;

            return overall;
        }

        public static double autospectrum_d(Real[] x, int m, Real[] Pxx, WindowFunction wintype, Unit2Type spectrumunit, double bp1, double bp2, double deltaf, int bDecibel, int AcouWintype)
        {
            int i, roop;
            int fft_len = (int)(pow(2, m));
            double WinScale;	// for window
            double WinScale2;	// for g-RMS

            double rms;
            double temp;
            double freq;
            double Overall;
            double epsilon;
            double OverallTime, tempOverall, deltat;
            int nStep;


            // detectator Type 에 따른 Overall 계산
            // 30 msec만 측정 하도록 프로그램 수정 삼성 BP용으로 수정

            if ((deltaf != 0) && (spectrumunit != Unit2Type.rms))
            {
                OverallTime = 0;
                deltat = 1.0 / (fft_len * deltaf);
                nStep = (int)(0.03 / deltat);
                for (i = 0; i < (fft_len / nStep - 3); i++)
                {

                    tempOverall = GetPeakToPeak(x, i * nStep, (i + 1) * nStep);//GetZeroToPeak( x, fft_len*0.03);
                    OverallTime = OverallTime + tempOverall;
                }
                OverallTime = OverallTime / (fft_len / nStep - 3);
            }
            else
            {
                OverallTime = -1;
            }

            epsilon = pow(10, -18);

            rms = 0;
            freq = 0;

            var FFTxx = new COMPLEX[fft_len];//(COMPLEX*)calloc(fft_len, sizeof( COMPLEX ) );
            var window = NadaMath.GetWindow(wintype, fft_len);

            WinScale = 0;
            for (i = 0; i < fft_len; i++)
            {
                x[i] = (Real)(x[i]*window[i]);
                WinScale += window[i];
            }

            WinScale2 = 0;
            for (i = 0; i < fft_len; i++)
            {
                WinScale2 += window[i] * window[i];
            }

            rfft(x, FFTxx, m);

            // 소음 관련 루틴 
            roop = fft_len / 2;

            for (i = 0; i < roop; i++)
            {
                freq = i * deltaf;

                Pxx[i] = (Real)(Cabs(FFTxx[i]) * Cabs(FFTxx[i]));

                // Single side conversion 
                // scale 조정 
                // 
                if (i != 0)
                {
                    Pxx[i] *= 2;
                }

                //Pxx[i] /= ( WinScale * fft_len);
                Pxx[i] = (Real)(Pxx[i]/(WinScale * WinScale));

                // acoustic Weighting
                if (AcouWintype == 1)
                {
                    Pxx[i] = (Real)(Pxx[i] * Wa_Lin(freq) * Wa_Lin(freq));
                }
                else if (AcouWintype == 3)
                {
                    Pxx[i] = (Real)(Pxx[i] * Wc_Lin(freq) * Wc_Lin(freq));
                }


                temp = Pxx[i] * (WinScale * WinScale) / (fft_len * WinScale2);
                if ((bp1 <= freq) && (freq <= bp2))
                {
                    rms = rms + temp;
                }

                //if (spectrumunit != 1)
                    Pxx[i] = (Real)sqrt(Pxx[i]);
            }

            //if (spectrumunit != 1)
                Overall = sqrt(rms);
            //else
            //    Overall = rms;

            if (bDecibel == 1) // Decibel Scale
            {

                temp = Overall / (2 * pow(10, -5));
                temp += epsilon;
                Overall = 20 * log10(temp);

                for (i = 0; i < roop; i++)
                {
                    temp = Pxx[i] / (2 * pow(10, -5));
                    temp += epsilon;
                    Pxx[i] = (Real)(20 * log10(temp));
                }
            }

            FFTxx = null;
            //free(FFTxx);
            //free(window); 

            if (spectrumunit == Unit2Type.rms)
                return Overall;

            var tRes = new Real[fft_len];// (double*)calloc(fft_len, sizeof(double));
            Overall = BandpassOverall(x, m, tRes, spectrumunit - 1, bp1, bp2, deltaf);
            //free(tRes);
            if (spectrumunit == Unit2Type.pk) // 0- p
            {
                for (i = 0; i < roop; i++)
                {
                    Pxx[i] = (Real)(Pxx[i] * sqrt(2));
                }

                return OverallTime / 2.0;
            }
            else if (spectrumunit == Unit2Type.pp) // p-p
            {
                for (i = 0; i < roop; i++)
                {
                    Pxx[i] = (Real)(Pxx[i] * sqrt(2) * 2);
                }

                return OverallTime;
            }
            else
            {
                return -1;
            }

        }// end of function

        // Crest Factor 계산 루틴
        public static double crestFactorValue(Real[] values, int valuesLength)
        {
            double max = 0;
            double sum = 0;
            //double val;

            var absValues = values.Select(v => Math.Abs(v)).ToArray();
            max = absValues.OrderByDescending(v => v).Skip(3).First();
  //          Array.Sort(absValues);
            sum = absValues.Sum(v => v * v);
            //for (int i = 0; i < valuesLength; i++)
            //{
            //    val = Math.Abs(values[i]);
            //    sum += (val * val);
            //    //if (val > max)
            //    //    max = val;
            //}
            double rms = Math.Sqrt(sum / valuesLength);
            if (0 == rms)
                return 0;

            return max / rms;
        }

        // Time Domain에서 최대값
        public static double peakValue(Real[] values, int valuesLength)
        {
            //return values.Max(); <== 이렇게 해도 비슷하겠지만, 마이너스 값을 인식 못함
            // 그렇다면 이렇게?
            return Math.Max(Math.Abs(values.Max()), Math.Abs(values.Min()));
            /* 원래 델파이 코드
            double max = 0;
            double val;

            for (int i = 0; i < valuesLength; i++)
            {
                val = Math.Abs(values[i]);
                if (val > max)
                    max = val;
            }
            // Peak to Peak인 경우, 아래와 같이 2배로 계산
            // max *= 2.0;
            return max;
             */
        }

        // Time Domain에서 최대 - 최소
        public static double peak2peak(Real[] values, int valuesLength)
        {
            return (values.Max() - values.Min());
        }

        // DC 값 구하기... 산술평균
        public static double dcValue(Real[] values, int valuesLength)
        {
            return values.Average();
            /* 원래 델파이 코드
            double sum = 0;
            for (int i = 0; i < valuesLength; i++)
            {
                sum += values[i];
            }
            return sum / (1.0 * valuesLength);
             */
        }

  //      public static void Integrator(double[] x, double[] y, double T, int len)//x : input array, y: output array, T: sampling interval, length: length of x and y, chan_number : 0 ~NUMBER_OF_CHANNELS-1
        public static void Integrator(float[] x, float[] y, float T, int len)//x : input array, y: output array, T: sampling interval, length: length of x and y, chan_number : 0 ~NUMBER_OF_CHANNELS-1
        {
            int i;
            float r = 0.995f;
            float temp;
            float y_int_buffer = 0.0f;
            float x_int_buffer = 0.0f;
            float y_notch_buffer1 = 0.0f;
            float y_notch_buffer2 = 0.0f;
            float x_notch_buffer1 = 0.0f;
            float x_notch_buffer2 = 0.0f;
            float[] y_temp = new float[len];

            for (i = 0; i < len; i++)
            {
                y_temp[i] = 0.0f;
            }

            for (i = 0; i < len; i++)
            {

                y_temp[i] = r * y_notch_buffer1 + x[i] - x_notch_buffer1;
                x_notch_buffer1 = x[i];
                y_notch_buffer1 = y_temp[i];

                y_temp[i] = y_int_buffer + 0.5f * T * (y_notch_buffer1 + x_int_buffer);
                x_int_buffer = y_notch_buffer1;
                y_int_buffer = y_temp[i];

                temp = y_temp[i];
                y_temp[i] = r * y_notch_buffer2 + y_temp[i] - x_notch_buffer2;
                x_notch_buffer2 = temp;
                y_notch_buffer2 = y_temp[i];
            }

         //   Array.Copy(y_temp, y, len);
            for (i = 0; i < len; i++)
            {
                y[i] = y_temp[i];
            }
        }

        //http://blog.naver.com/PostView.nhn?blogId=ko_bongkyun&logNo=100013211321&redirect=Dlog&widgetTypeCall=true
        /// <summary>
        /// Least-squares solution(최소제곱법)으로 입력벡터의 근사 곡선을 계산;
        /// </summary>
        /// <param name="x">입력 벡터 X</param>
        /// <param name="y">입력 벡터 Y</param>
        /// <param name="m">근사하려는 차수</param>
        /// <param name="n">데이터 갯수</param>
        /// <returns>근사된 결과(추세선 역할 가능)</returns>
        public static double[] LeastSquaresSolution(double[] x, double[] y, int m, int n)
        {
            //x,y : double 형의 배열 x[n],y[n]에 n개의 점을 준다.
            //m : 근사하려는 다항식의 차수 m을 정수나 변수명으로 준다.
            // 1<=m<=20 , m<n
            //n : 데이타수
            //출력(c): double 형의 배열 c[m+1]에 구하려는 다항식의 계수가 얻어진다.
            // 얻어지는 순서는 다항식의 형식으로 나타내면 다음과 같다
            // y=c[m]x^(m) + c[m-1]x^(m-1) + ... + c[1]x + c[0]

            double[] c = new double[n];

            int i, j, k, l;
            double w1, w2, w3, pivot, aik;
            double[,] a = new double[21, 22];
            double[] w = new double[42];
            w.Initialize();
            a.Initialize();

            if (m >= n || m < 1 || m > 20) return null;

            for (i = 0; i < m * 2; i++)
            {
                w1 = 0.0;
                for (j = 0; j < n; j++)
                {
                    w2 = w3 = x[j];
                    for (k = 0; k < i; k++)
                    {
                        w2 *= w3;
                    }
                    w1 += w2;
                }
                w[i] = w1;
            }
            for (i = 0; i < m + 1; i++)
            {
                for (j = 0; j < m + 1; j++)
                {
                    l = i + j - 1;
                    if (l < 0)
                        l = 0;
                    a[i, j] = w[l];
                }
            }
            a[0, 0] = n;
            w1 = 0;
            for (i = 0; i < n; i++)
                w1 += y[i];
            a[0, m + 1] = w1;
            for (i = 0; i < m; i++)
            {
                w1 = 0;
                for (j = 0; j < n; j++)
                {
                    w2 = w3 = x[j];
                    for (k = 0; k < i; k++)
                        w2 *= w3;
                    w1 += y[j] * w2;
                }
                a[i + 1, m + 1] = w1;
            }
            for (k = 0; k < m + 1; k++)
            {
                pivot = a[k, k];
                for (j = k; j < m + 2; j++)
                    a[k, j] /= pivot;
                for (i = 0; i < m + 1; i++)
                {
                    if (i != k)
                    {
                        aik = a[i, k];
                        for (j = k; j < m + 2; j++)
                            a[i, j] -= aik * a[k, j];
                    }
                }
            }
            for (i = 0; i < m + 1; i++)
                c[i] = a[i, m + 1];

            return c;
        }

        //---------------------------------------------- CXII 알고리즘 시작-----------------------------//
        //(Real[] x, int m, Real[] Pxx, WindowFunction wintype, Unit2Type spectrumunit, double bp1, double bp2, double deltaf, int bDecibel, int AcouWintype)
        static readonly CmsLookupTable lut = new CmsLookupTable();
        static Dictionary<int, Real[]> hannCache = new Dictionary<int, Real[]>();
        static Dictionary<int, double> hannNormalCache = new Dictionary<int, double>();
        public static double autospectrum_cxii(double[] m_uGraphData, int m, double[] Pxx, int wintype, int spectrumunit, double bp1, double bp2, double deltaf, int bDecibel, int AcouWintype, double dSamplingRate)
        {
            double nWinNormal;
            nWinNormal = 0;
            const int D_MAX_BUFF = 65536;
            float[] m_pfReal = new float[D_MAX_BUFF];
            float[] m_pfImg = new float[D_MAX_BUFF];
            float[] m_dSpectrum = new float[D_MAX_BUFF];
            float ROOT_2 = 1.414213f;


            int nTr = m_uGraphData.Length;// m_pGdHeader->nTransCount
            int nTransCount = m_uGraphData.Length;
            double[] pRmsBuffer = new double[nTransCount / 2];

            int i;
            for (i = 0; i < nTr; i++)
            {
                m_pfReal[i] = (float)m_uGraphData[i];
            }

            float fXAvg = (float)m_uGraphData.Average();

            for (i = 0; i < nTr; i++)
            {
                m_pfReal[i] = m_pfReal[i] - fXAvg;
            }

            Real[] window;
            if (!hannCache.ContainsKey(nTr))
            {
                window = CreateWindow(wintype, out nWinNormal, nTr);
                hannCache.Add(nTr, window);
                hannNormalCache.Add(nTr, nWinNormal);
            }
            else
            {
                window = hannCache[nTr];
                nWinNormal = hannNormalCache[nTr];
            }

            float m_fNBW = FftWindowFn(wintype, window, m_pfReal, m_pfImg);

            CmsEnvelop.FftDoTransform(lut.gpfSin, lut.gpfCos, m_pfReal, m_pfImg, nTransCount);



            double dAmp;
            for (i = 0; i < nTransCount / 2; i++)
            {
                dAmp = (double)(m_pfReal[i] * m_pfReal[i] + m_pfImg[i] * m_pfImg[i]);
                dAmp = sqrt(dAmp);
                m_dSpectrum[i] = (float)(dAmp / (nWinNormal) * 2); // 곱하기 2는 single side로 변환시킨 것이다.
            }

            m_dSpectrum[0] = 0;


            for (i = 0; i < nTransCount / 2; i++)
            {
                dAmp = m_dSpectrum[i] / ROOT_2;//rjy 루트 2로 나눈것은 rms로 보기 위함이고 현재 default 나중에 0Peak 으로 바꿀려면 루트 2를 없애면됨
                pRmsBuffer[i] = dAmp;
                Pxx[i] = dAmp;
            }

            float fPeak = 0.0f;
            float fPeakHz = 0.0f;
            float fSumRms = 0.0f;
            double dDeltaHz = dSamplingRate / nTransCount;



            if (bp2 > dSamplingRate / 2)
                bp2 = dSamplingRate / 2;

            int nXMin = (int)(bp1 / dDeltaHz);
            int nXMax = (int)(bp2 * (1 / dDeltaHz));

            if (1 > nXMin)
                nXMin = 1;
            if (nTr < nXMax)
                nXMax = nTr;

            for (i = nXMin; i < nXMax; i++)
            {
                fPeak = (float)pRmsBuffer[i];
                fPeakHz = (float)(i * dDeltaHz);
                fSumRms += fPeak * fPeak;
            }

            //double dwinFactor = FftWindowFn1(GetAcqOpt()->m_nWindow);
            fSumRms = fSumRms / m_fNBW;

            return sqrt(fSumRms);
        }

        public static float[] CreateWindow(int nWindowFn, out double nWinNormal, int nTransCount)
        {
            const float PI = (float)Math.PI;

            double factor = 0, result = 0;
            nWinNormal = 0.0;

            float[] window = new float[nTransCount];
            switch ((WindowFunction)nWindowFn)
            {
                case WindowFunction.Hanning:	//Hanning
                    for (int i = 0; i < nTransCount; i++)
                    {
                        factor = 8.0 * atan(1.0) / (nTransCount - 1);
                        result = 1.0 * (1 - cos(factor * i));
                        //				result = 0.5 * (1-cos(factor * i));
                        nWinNormal = nWinNormal + result;
                        window[i] = (float)result;
                    }
                    break;

                case WindowFunction.Rectangle:	//Rectangular
                    for (int i = 0; i < nTransCount; i++)
                    {
                        result = (double)1;
                        nWinNormal = nWinNormal + result;
                        window[i] = (float)result;
                    }
                    break;

                case WindowFunction.FlatTop:	//Flat-Top	YHS 0602
                    for (int i = 0; i < nTransCount; i++)
                    {
                        factor = 2.0 * PI / (nTransCount - 1);
                        result = 1 - 1.933 * cos(factor * i) + 1.286 * cos(2 * factor * i)
                             - 0.388 * cos(3 * factor * i) + 0.032 * cos(4 * factor * i);
                        nWinNormal = nWinNormal + result;
                        window[i] = (float)result;
                    }
                    break;

                case WindowFunction.Hamming:	//Hamming
                    for (int i = 0; i < nTransCount; i++)
                    {
                        factor = 8.0 * atan(1.0) / (nTransCount - 1);
                        result = (0.54 - 0.46 * cos(factor * i)) / 0.54;
                        nWinNormal = nWinNormal + result;
                        window[i] = (float)result;
                    }
                    break;

                case WindowFunction.Triangle:	//triangle
                    for (int i = 0; i < nTransCount; i++)
                    {
                        factor = 2.0 / (nTransCount - 1);
                        result = factor * i;
                        nWinNormal = nWinNormal + result;
                        window[i] = (float)result;
                    }
                    break;

                case WindowFunction.Blackman:	//Blackman
                    for (int i = 0; i < nTransCount; i++)
                    {
                        factor = 8.0 * atan(1.0) / (nTransCount - 1);
                        result = (0.42 - 0.5 * cos(factor * i) + 0.08 * cos(2 * factor * i)) / 0.42;
                        nWinNormal = nWinNormal + result;
                        window[i] = (float)result;
                    }
                    break;

                case WindowFunction.Harris:	//Blackman-Harris
                    for (int i = 0; i < nTransCount; i++)
                    {
                        factor = 8.0 * atan(1.0) / (nTransCount - 1);
                        result = (0.35875 - 0.48829 * cos(factor * i) + 0.14128 * cos(2 * factor * i)
                            - 0.01168 * cos(3 * factor * i)) / 0.35875;
                        nWinNormal = nWinNormal + result;
                        window[i] = (float)result;
                    }
                    break;
            }
            return window;
        }
        public static float FftWindowFn(int nWindowFn, float[] window, float[] m_pfReal, float[] m_pfImg)
        {
            float fNBW = 0.0f; //noise band width;

            for (int i = 0; i < window.Length; i++)
            {
                m_pfReal[i] = m_pfReal[i] * window[i];
                m_pfImg[i] = m_pfImg[i] * window[i];
            }

            switch ((WindowFunction)nWindowFn)
            {
                case WindowFunction.Hanning:	//Hanning
                    fNBW = 1.5f;
                    break;

                case WindowFunction.Rectangle:	//Rectangular
                    fNBW = 1.00f;
                    break;

                case WindowFunction.FlatTop:	//Flat-Top	YHS 0602
                    fNBW = 3.77f;
                    break;

                case WindowFunction.Hamming:	//Hamming
                    fNBW = 1.36f;
                    break;

                case WindowFunction.Triangle:	//triangle
                    fNBW = 0.01f;
                    break;

                case WindowFunction.Blackman:	//Blackman
                    fNBW = 0.01f;
                    break;

                case WindowFunction.Harris:	//Blackman-Harris
                    fNBW = 0.01f;
                    break;
            }

            return fNBW;
        }

        //---------------------------------------------- CXII 알고리즘 끝-----------------------------//
    }

    public static class CmsEnvelop
    {
        const int MULTI_BUFF = 5;
        const int MAX_ENVBUFF = 65536;
        const int D_MAX_BUFF = 65536;
        //      const int M = 400;
        const int M = 526; // 2 * ceiling (8*4/(dFc2 - dFc1))
        static readonly CmsLookupTable lut = new CmsLookupTable();

#if false // Fmax, line 적용전
        public static bool EnvelopData(out double[] m_dSpectrum, float[] m_pAcqBuffer, int N, int nTotalCh, int nTotalSize, double dSamplingRate, int nLine, int nFt, int nCycle,
                        double dVoltRange, int nResolution, double dMV, int nGain, int nChGain, int nAccelUnit, double m_dRemoveNoise, ChannelType nSensorType, ChannelType nSensorCnvt)
        {
            const int m_nMultiBuff = MULTI_BUFF;

            int nTransCount = LineToTrans(nLine);

            int i, j;
            //int nTotalCh = GetMain()->m_nTotalCh;
            //int nTotalSize = GetMain()->m_nEntireBuffSize;
            //int nFt = GetMain()->m_nFeedPt;
            //int nCycle = GetMain()->m_nCycleCount;

            //m_nZF = GetMain()->m_nMultiBuff;	// Zoom Factor

            //memcpy(&m_GdHeader, m_pGdHeader, sizeof(GDHEADER));


            int m_nZF;
            if (0 < nCycle)
                m_nZF = (int)(dSamplingRate * m_nMultiBuff / nTransCount);
            else
                m_nZF = (int)(nFt / nTotalCh / nTransCount);

            if (5 < m_nZF)
                m_nZF = 10;
            else
                m_nZF = 5;

            m_nZF = 1;
            int nMaxBuffSize = m_nZF * nTransCount;

            // Assamble Zoom fact Buffer
            double[] dMaxRealBuff = new double[nMaxBuffSize];

            int nI = 0;
            double dSum = 0, dAvg = 0;
            for (i = 0; i < nMaxBuffSize; i++)
            {
                nI = nTotalCh * i + N;
                if (nI >= nTotalSize)
                    nI = nI - nTotalSize;

                dMaxRealBuff[i] = m_pAcqBuffer[nI];
                dSum += dMaxRealBuff[i];
            }

            dAvg = dSum / nMaxBuffSize;

            for (i = 0; i < nMaxBuffSize; i++)		// Offset Data
            {
                dMaxRealBuff[i] = dMaxRealBuff[i] - dAvg;
            }

            // BandPass Filter HighPass Coefficeint (M define 400)
 //           double dFc1 = 0.391;		// bandpass-low cutoff frequency  (9000Hz) 
 //           double dFc2 = 0.175;		// bandpass-high cutoff frequency (20000Hz)
   
            double dFc1=0.175;		// bandpass-low cutoff frequency  (9000Hz) 
                double dFc2=0.391;		// bandpass-high cutoff frequency (20000Hz)

 //               double dFc1 = 0.2441;		// bandpass-low cutoff frequency  (9000Hz) 
 //               double dFc2 = 0.9277;		// bandpass-high cutoff frequency (20000Hz)



            double dFc3 = 1.0 / ((double)m_nZF * 2);		// Lowpass frequency after resampling
            // 2000/Fsamplingrate  ( operating speed * 20 )

            double[] nbpf_lc = new double[M + 1];	// sinc BPF_H coefficient ; M+1
            double[] nbpf_hc = new double[M + 1];	// sinc BPF_L coefficient ; M+1

            lpfc_generator(dFc1, nbpf_lc);	// bandpass filter low pass coefficeint
            lpfc_generator(dFc2, nbpf_hc);	// bandpass filter hign pass coefficeint

            for (i = 0; i <= M; i++)		// low--->high pass coefficent
            {
                nbpf_hc[i] = -nbpf_hc[i];
            }
            nbpf_hc[M / 2] = 1 + nbpf_hc[M / 2];

            // Convolution...
            int nConvNum = nMaxBuffSize + M;
            double[] temp1 = new double[nConvNum];
            double[] temp2 = new double[nConvNum];

            // Band-lowpass convolution
            for (i = 0; i < nMaxBuffSize; i++)
            {
                for (j = 0; j <= M; j++)
                {
                    temp1[i + j] = temp1[i + j] + dMaxRealBuff[i] * nbpf_lc[j];
                }
            }

            // Band-highpass convolution
            for (i = 0; i < nMaxBuffSize; i++)
            {
                for (j = 0; j <= M; j++)
                {
                    temp2[i + j] = temp2[i + j] + temp1[i] * nbpf_hc[j];
                }
            }

            // Rectification
            for (i = 0; i < nConvNum; i++)
            {
                temp2[i] = fabs(temp2[i]);
            }

            // Lowpass filter
            double[] nlpf_c = new double[M + 1];		// sinc LPF coefficient ; M+1

            lpfc_generator(dFc3, nlpf_c);	// bandpass filter low pass coefficeint

            //::ZeroMemory(temp1, sizeof(double)*nConvNum);
            Fill(temp1, 0, nConvNum);

            // Bandpass convolution enveloping data 	
            for (i = 0; i < nMaxBuffSize; i++)
            {
                for (j = 0; j <= M; j++)
                {
                    temp1[i + j] = temp1[i + j] + temp2[i] * nlpf_c[j];
                }
            }


            // Resampling...
            float[] fRealBuff = new float[MAX_ENVBUFF];
            float[] fImgBuff = new float[MAX_ENVBUFF];
            j = 0;
            for (i = 0; i < nMaxBuffSize; i++)
            {
                if (0 == (i % m_nZF))
                {
                    if (MAX_ENVBUFF <= j) break;

                    fRealBuff[j] = (float)temp1[i];
                    fImgBuff[j] = 0;
                    j++;
                }
            }

            // FFT
            MakeSpectrumEX(out m_dSpectrum, fRealBuff, fImgBuff, false, nTransCount,
                dVoltRange, nResolution, dMV, nGain, nChGain, nAccelUnit, dSamplingRate, m_dRemoveNoise, nSensorType, nSensorCnvt);
            dSamplingRate = dSamplingRate / (double)m_nZF;

            return true;
        }

        static void MakeSpectrumEX(out double[] m_dSpectrum, float[] fReal, float[] fImg, bool bBothSide, int nTransCount,
            double dVoltRange, int nResolution,
            double dMV, int nGain, int nChGain, int nAccelUnit,
            double dSamplingRate, double m_dRemoveNoise,
            ChannelType nSensorType, ChannelType nSensorCnvt)
        {	// MakeSpectrum과 틀린점 : Real, Img 버퍼를 직접 받고 nTransCount/2 -> nTransCount 범위
	        int i;

            var m_pfReal = new float[D_MAX_BUFF];
            var m_pfImg = new float[D_MAX_BUFF];
            m_dSpectrum = new double[D_MAX_BUFF];

	        //memcpy(m_pfReal, fReal, sizeof(float)*m_pGdHeader->nTransCount);
	        //memcpy(m_pfImg, fImg, sizeof(float)*m_pGdHeader->nTransCount);
            fReal.CopyTo(m_pfReal, 0);
            fImg.CopyTo(m_pfImg, 0);

	        //memset(m_dSpectrum, 0, sizeof(float)*m_pGdHeader->nTransCount);
            //Fill(m_dSpectrum, 0, nTransCount);

	        FftDoTransform(lut.gpfSin, lut.gpfCos, m_pfReal, m_pfImg, nTransCount);

	        double dPeriod, dAmpTemp;
        //	double dCoeff = GetCoeff(m_pGdHeader, TRUE, TRUE);
	        double dVoltCoeff = GetCoeffVolt(dVoltRange, nResolution);	//hsyu
	        double dSensorCoeff = GetCoeffSensor(dMV, nGain, nChGain, nResolution, nAccelUnit, true, true);

	        //int nSensorType = m_pGdHeader->nSensorType;

	        //::ZeroMemory(m_dSpectrum, sizeof(double)*MAX_ENVBUFF);
            Fill(m_dSpectrum, 0, MAX_ENVBUFF);

	        dPeriod = (double) dSamplingRate / nTransCount;   

	        if(bBothSide)
	        {
		        int k=0;
		        for(i = nTransCount / 2; i < nTransCount ; i++)
		        {   
			        dAmpTemp = (double)(m_pfReal[i] * m_pfReal[i] + m_pfImg[i] * m_pfImg[i]);
			        dAmpTemp = sqrt(dAmpTemp);
			        dAmpTemp = (double)(dAmpTemp / (nTransCount / 2));
			
        //			dAmpTemp = dAmpTemp * dCoeff;
			        dAmpTemp = dAmpTemp * dVoltCoeff;	//hsyu
        /*			switch(GetSpcOpt()->m_nRemoveNoise)
			        {
				        case 0:
					        if(dAmpTemp < 2)
						        dAmpTemp = 0;
					        break;
				        case 1:
					        if(dAmpTemp < 1)
						        dAmpTemp = 0;
					        break;
				        case 2:
					        break;
				        default:
					        break;
			        }
        */
			        if(dAmpTemp < m_dRemoveNoise)
				        dAmpTemp = 0;

			        dAmpTemp = dAmpTemp * dSensorCoeff;

			        double dFreq = i*dPeriod;
			
			        m_dSpectrum[k] = (float)dAmpTemp;
			        m_dSpectrum[k] = (double) ConvertSensor(nSensorType, nSensorCnvt, (float)dFreq, (float)dAmpTemp);
			        k++;
		        } 
		
		        for(i = 0; i < nTransCount / 2; i++)
		        {   
			        dAmpTemp = (double)(m_pfReal[i] * m_pfReal[i] + m_pfImg[i] * m_pfImg[i]);
			        dAmpTemp = sqrt(dAmpTemp);
			        dAmpTemp = (double)(dAmpTemp / (nTransCount / 2));
			
        //			dAmpTemp = dAmpTemp * dCoeff;
			        dAmpTemp = dAmpTemp * dVoltCoeff;	//hsyu
        /*			switch(GetSpcOpt()->m_nRemoveNoise)
			        {
				        case 0:
					        if(dAmpTemp < 2)
						        dAmpTemp = 0;
					        break;
				        case 1:
					        if(dAmpTemp < 1)
						        dAmpTemp = 0;
					        break;
				        case 2:
					        break;
				        default:
					        break;
			        }
        */
			        if(dAmpTemp < m_dRemoveNoise)
				        dAmpTemp = 0;

			        dAmpTemp = dAmpTemp * dSensorCoeff;

			        double dFreq = i*dPeriod;
			
			        m_dSpectrum[k] = dAmpTemp;
			        //m_dSpectrum[k] = (double) ConvertSensor(nSensorType, nSensorCnvt, (float)dFreq, (float)dAmpTemp);
                    //m_dSpectrum[k] = (double)ConvertSensor(nSensorType, nSensorCnvt, (float)dFreq, (float)dAmpTemp)*(100000);
                    m_dSpectrum[k] = (double)ConvertSensor(nSensorType, nSensorCnvt, (float)dFreq, (float)dAmpTemp) * (3000000);

			        k++;
		        }
	        }
	        else
	        {
		        for(i = 0; i < nTransCount / 2; i++)
		        {   
			        dAmpTemp = (double)(m_pfReal[i] * m_pfReal[i] + m_pfImg[i] * m_pfImg[i]);
			        dAmpTemp = sqrt(dAmpTemp);
			        dAmpTemp = (double)(dAmpTemp / (nTransCount / 2));
			
        //			dAmpTemp = dAmpTemp * dCoeff;
			        dAmpTemp = dAmpTemp * dVoltCoeff;	//hsyu
        /*			switch(GetSpcOpt()->m_nRemoveNoise)
			        {
				        case 0:
					        if(dAmpTemp < 2)
						        dAmpTemp = 0;
					        break;
				        case 1:
					        if(dAmpTemp < 1)
						        dAmpTemp = 0;
					        break;
				        case 2:
					        break;
				        default:
					        break;
			        }
        */
			        if(dAmpTemp < m_dRemoveNoise)
				        dAmpTemp = 0;

			        dAmpTemp = dAmpTemp * dSensorCoeff;

			        double dFreq = i*dPeriod;
			
			        m_dSpectrum[i] = (float)dAmpTemp;
			        //m_dSpectrum[i] = (double) ConvertSensor(nSensorType, nSensorCnvt, (float)dFreq, (float)dAmpTemp);
                    m_dSpectrum[i] = (double)ConvertSensor(nSensorType, nSensorCnvt, (float)dFreq, (float)dAmpTemp)*3000000;
		        }

		        m_dSpectrum[0] = 0;
	        }
        }

#else // Fmax, line 적용 후

        public static bool EnvelopData(out double[] m_dSpectrum, float[] m_pAcqBuffer, int N, int nTotalCh, int nTotalSize, double dSamplingRate, int nLine, int nFt, int nCycle,
                      double dVoltRange, int nResolution, double dMV, int nGain, int nChGain, int nAccelUnit, double m_dRemoveNoise, ChannelType nSensorType, ChannelType nSensorCnvt)
        {
            const int m_nMultiBuff = MULTI_BUFF;

            int nTransCount = LineToTrans(nLine);

            int i, j;
            //int nTotalCh = GetMain()->m_nTotalCh;
            //int nTotalSize = GetMain()->m_nEntireBuffSize;
            //int nFt = GetMain()->m_nFeedPt;
            //int nCycle = GetMain()->m_nCycleCount;

            //m_nZF = GetMain()->m_nMultiBuff;	// Zoom Factor

            //memcpy(&m_GdHeader, m_pGdHeader, sizeof(GDHEADER));


            int m_nZF;
            if (0 < nCycle)
                m_nZF = (int)(dSamplingRate * m_nMultiBuff / nTransCount);
            else
                m_nZF = (int)(nFt / nTotalCh / nTransCount);

            if (5 < m_nZF)
                m_nZF = 10;
            else
                m_nZF = 5;

            m_nZF = 1;
            int nMaxBuffSize = m_nZF * nTransCount;

            // Assamble Zoom fact Buffer
            double[] dMaxRealBuff = new double[nMaxBuffSize];

            int nI = 0;
            double dSum = 0, dAvg = 0;
            for (i = 0; i < nMaxBuffSize; i++)
            {
                nI = nTotalCh * i + N;
                if (nI >= nTotalSize)
                    nI = nI - nTotalSize;

                dMaxRealBuff[i] = m_pAcqBuffer[nI];
                dSum += dMaxRealBuff[i];
            }

            dAvg = dSum / nMaxBuffSize;

            for (i = 0; i < nMaxBuffSize; i++)		// Offset Data
            {
                dMaxRealBuff[i] = dMaxRealBuff[i] - dAvg;
            }
            /* cxii 설정
            // BandPass Filter HighPass Coefficeint (M define 400)
            //           double dFc1 = 0.391;		// bandpass-low cutoff frequency  (9000Hz) // 이걸로 사용하면 안됨 엉뚱한 값
            //           double dFc2 = 0.175;		// bandpass-high cutoff frequency (20000Hz)

             */
            //기존 설정
            //double dFc1 = 0.175;		// bandpass-low cutoff frequency  (9000Hz) 
            //double dFc2 = 0.391;		// bandpass-high cutoff frequency (20000Hz)

            //               double dFc1 = 0.2441;		// bandpass-low cutoff frequency  (9000Hz) 
            //               double dFc2 = 0.9277;		// bandpass-high cutoff frequency (20000Hz)

            //        // 매틀랩 방법
            //        double dFc1 = 0.05;
            //        double dFc2 = 3200.0 / (dSamplingRate / 2.0);
            ////        int M = 2 * (int)Math.Ceiling(8 * 4 / (3200.0 - 0.0)); 

            double dFc3 = 1.0 / ((double)m_nZF * 2);		// Lowpass frequency after resampling
            double dFc1 = 0.0000244; //  Low cut off 주파수 0.1 / (sample rate /2)
            double dFc2 = 0.12207; //  High cut off 주파수 500 / (sample rate /2)
            //        int M = 2 * (int)Math.Ceiling(8 * 4 / (3200.0 - 0.0)); 

            //dFc3 = 0.2;		// Lowpass frequency after resampling


            // 2000/Fsamplingrate  ( operating speed * 20 )

            double[] nbpf_lc = new double[M + 1];	// sinc BPF_H coefficient ; M+1
            double[] nbpf_hc = new double[M + 1];	// sinc BPF_L coefficient ; M+1

            lpfc_generator(dFc1, nbpf_lc);	// bandpass filter low pass coefficeint
            lpfc_generator(dFc2, nbpf_hc);	// bandpass filter hign pass coefficeint

            for (i = 0; i <= M; i++)		// low--->high pass coefficent
            {
                nbpf_hc[i] = -nbpf_hc[i];
            }
            nbpf_hc[M / 2] = 1 + nbpf_hc[M / 2];

            // Convolution...
            int nConvNum = nMaxBuffSize + M;
            double[] temp1 = new double[nConvNum];
            double[] temp2 = new double[nConvNum];

            // Band-lowpass convolution
            for (i = 0; i < nMaxBuffSize; i++)
            {
                for (j = 0; j <= M; j++)
                {
                    temp1[i + j] = temp1[i + j] + dMaxRealBuff[i] * nbpf_lc[j];
                }
            }

            // Band-highpass convolution
            for (i = 0; i < nMaxBuffSize; i++)
            {
                for (j = 0; j <= M; j++)
                {
                    temp2[i + j] = temp2[i + j] + temp1[i] * nbpf_hc[j];
                }
            }

            // Rectification
            for (i = 0; i < nConvNum; i++)
            {
                temp2[i] = fabs(temp2[i]);
            }

            // Lowpass filter
            double[] nlpf_c = new double[M + 1];		// sinc LPF coefficient ; M+1

            lpfc_generator(dFc3, nlpf_c);	// bandpass filter low pass coefficeint

            //::ZeroMemory(temp1, sizeof(double)*nConvNum);
            Fill(temp1, 0, nConvNum);

            // Bandpass convolution enveloping data 	
            for (i = 0; i < nMaxBuffSize; i++)
            {
                for (j = 0; j <= M; j++)
                {
                    temp1[i + j] = temp1[i + j] + temp2[i] * nlpf_c[j];
                }
            }


            // Resampling...
            float[] fRealBuff = new float[MAX_ENVBUFF];
            float[] fImgBuff = new float[MAX_ENVBUFF];
            j = 0;
            for (i = 0; i < nMaxBuffSize; i++)
            {
                if (0 == (i % m_nZF))
                {
                    if (MAX_ENVBUFF <= j) break;

                    fRealBuff[j] = (float)temp1[i];
                    fImgBuff[j] = 0;
                    j++;
                }
            }

            // FFT
            MakeSpectrumEX(out m_dSpectrum, fRealBuff, fImgBuff, false, nTransCount,
                dVoltRange, nResolution, dMV, nGain, nChGain, nAccelUnit, dSamplingRate, m_dRemoveNoise, nSensorType, nSensorCnvt);
            dSamplingRate = dSamplingRate / (double)m_nZF;

            return true;
        }

        static void MakeSpectrumEX(out double[] m_dSpectrum, float[] fReal, float[] fImg, bool bBothSide, int nTransCount,
            double dVoltRange, int nResolution,
            double dMV, int nGain, int nChGain, int nAccelUnit,
            double dSamplingRate, double m_dRemoveNoise,
            ChannelType nSensorType, ChannelType nSensorCnvt)
        {	// MakeSpectrum과 틀린점 : Real, Img 버퍼를 직접 받고 nTransCount/2 -> nTransCount 범위
            int i;

            var m_pfReal = new float[D_MAX_BUFF];
            var m_pfImg = new float[D_MAX_BUFF];
            m_dSpectrum = new double[D_MAX_BUFF];

            //memcpy(m_pfReal, fReal, sizeof(float)*m_pGdHeader->nTransCount);
            //memcpy(m_pfImg, fImg, sizeof(float)*m_pGdHeader->nTransCount);
            fReal.CopyTo(m_pfReal, 0);
            fImg.CopyTo(m_pfImg, 0);

            //memset(m_dSpectrum, 0, sizeof(float)*m_pGdHeader->nTransCount);
            //Fill(m_dSpectrum, 0, nTransCount);

            FftDoTransform(lut.gpfSin, lut.gpfCos, m_pfReal, m_pfImg, nTransCount);

            double dPeriod, dAmpTemp;
            //	double dCoeff = GetCoeff(m_pGdHeader, TRUE, TRUE);
            double dVoltCoeff = 1;// GetCoeffVolt(dVoltRange, nResolution);	//hsyu
            double dSensorCoeff = 1;// GetCoeffSensor(dMV, nGain, nChGain, nResolution, nAccelUnit, true, true);

            //int nSensorType = m_pGdHeader->nSensorType;

            //::ZeroMemory(m_dSpectrum, sizeof(double)*MAX_ENVBUFF);
            Fill(m_dSpectrum, 0, MAX_ENVBUFF);

            dPeriod = (double)dSamplingRate / nTransCount;

            if (bBothSide)
            {
                int k = 0;
                for (i = nTransCount / 2; i < nTransCount; i++)
                {
                    dAmpTemp = (double)(m_pfReal[i] * m_pfReal[i] + m_pfImg[i] * m_pfImg[i]);
                    dAmpTemp = sqrt(dAmpTemp);
                    dAmpTemp = (double)(dAmpTemp / (nTransCount / 2));

                    //			dAmpTemp = dAmpTemp * dCoeff;
                    dAmpTemp = dAmpTemp * dVoltCoeff;	//hsyu
                    /*			switch(GetSpcOpt()->m_nRemoveNoise)
                                {
                                    case 0:
                                        if(dAmpTemp < 2)
                                            dAmpTemp = 0;
                                        break;
                                    case 1:
                                        if(dAmpTemp < 1)
                                            dAmpTemp = 0;
                                        break;
                                    case 2:
                                        break;
                                    default:
                                        break;
                                }
                    */
                    if (dAmpTemp < m_dRemoveNoise)
                        dAmpTemp = 0;

                    dAmpTemp = dAmpTemp * dSensorCoeff;

                    double dFreq = i * dPeriod;

                    m_dSpectrum[k] = (float)dAmpTemp;
                    m_dSpectrum[k] = (double)ConvertSensor(nSensorType, nSensorCnvt, (float)dFreq, (float)dAmpTemp);
                    k++;
                }

                for (i = 0; i < nTransCount / 2; i++)
                {
                    dAmpTemp = (double)(m_pfReal[i] * m_pfReal[i] + m_pfImg[i] * m_pfImg[i]);
                    dAmpTemp = sqrt(dAmpTemp);
                    dAmpTemp = (double)(dAmpTemp / (nTransCount / 2));

                    //			dAmpTemp = dAmpTemp * dCoeff;
                    dAmpTemp = dAmpTemp * dVoltCoeff;	//hsyu
                    /*			switch(GetSpcOpt()->m_nRemoveNoise)
                                {
                                    case 0:
                                        if(dAmpTemp < 2)
                                            dAmpTemp = 0;
                                        break;
                                    case 1:
                                        if(dAmpTemp < 1)
                                            dAmpTemp = 0;
                                        break;
                                    case 2:
                                        break;
                                    default:
                                        break;
                                }
                    */
                    if (dAmpTemp < m_dRemoveNoise)
                        dAmpTemp = 0;

                    dAmpTemp = dAmpTemp * dSensorCoeff;

                    double dFreq = i * dPeriod;

                    m_dSpectrum[k] = dAmpTemp;
                    //m_dSpectrum[k] = (double) ConvertSensor(nSensorType, nSensorCnvt, (float)dFreq, (float)dAmpTemp);
                    //m_dSpectrum[k] = (double)ConvertSensor(nSensorType, nSensorCnvt, (float)dFreq, (float)dAmpTemp)*(100000);
                    m_dSpectrum[k] = (double)ConvertSensor(nSensorType, nSensorCnvt, (float)dFreq, (float)dAmpTemp) * (3000000);

                    k++;
                }
            }
            else
            {
                for (i = 0; i < nTransCount / 2; i++)
                {
                    dAmpTemp = (double)(m_pfReal[i] * m_pfReal[i] + m_pfImg[i] * m_pfImg[i]);
                    dAmpTemp = sqrt(dAmpTemp);
                    dAmpTemp = (double)(dAmpTemp / (nTransCount / 2));

                    //			dAmpTemp = dAmpTemp * dCoeff;
                    dAmpTemp = dAmpTemp * dVoltCoeff;	//hsyu
                    /*			switch(GetSpcOpt()->m_nRemoveNoise)
                                {
                                    case 0:
                                        if(dAmpTemp < 2)
                                            dAmpTemp = 0;
                                        break;
                                    case 1:
                                        if(dAmpTemp < 1)
                                            dAmpTemp = 0;
                                        break;
                                    case 2:
                                        break;
                                    default:
                                        break;
                                }
                    */
                    if (dAmpTemp < m_dRemoveNoise)
                        dAmpTemp = 0;

                    dAmpTemp = dAmpTemp * dSensorCoeff;

                    double dFreq = i * dPeriod;

                    m_dSpectrum[i] = (float)dAmpTemp;
                    //m_dSpectrum[i] = (double) ConvertSensor(nSensorType, nSensorCnvt, (float)dFreq, (float)dAmpTemp);
                    // m_dSpectrum[i] = (double)ConvertSensor(nSensorType, nSensorCnvt, (float)dFreq, (float)dAmpTemp) * 3000000;
                    m_dSpectrum[i] = (float)dAmpTemp * 30000;
                }

                m_dSpectrum[0] = 0;
            }
        }
#endif
        static void lpfc_generator(double dFc, double[] dFilter)
        {
            int i;
            double[] lpf_c = new double[M + 1];   // lowpassfilter coefficient M+1
            double[] wlpf_c = new double[M + 1];  // windowed lowpassfilter coefficient M+1

            // Filter Coefficient Generate
            for (i = 0; i <= M; i++)
            {
                if (i == M / 2)
                    lpf_c[M / 2] = TWO_PI * dFc;
                else
                    lpf_c[i] = sin(TWO_PI * dFc * (i - M / 2.0)) / (i - M / 2.0);
            }

            double dSum = 0;
            for (i = 0; i <= M; i++)
            {
                wlpf_c[i] = lpf_c[i] * (0.54 - 0.46 * cos(TWO_PI * i / M));//hamming window(2002년 6월에 밝힘)
                dSum += wlpf_c[i];
            }
            // Low Pass Filter Normalizing
            for (i = 0; i <= M; i++)
                dFilter[i] = wlpf_c[i] / dSum;
        }

        const ChannelType ST_VOLT = ChannelType.DC;
        const ChannelType ST_ACCEL = ChannelType.Accelate;
        const ChannelType ST_VELO = ChannelType.Displacement;
        //const int ST_GAP = (int); //GAP 부분들 삭제
        static float ConvertSensor(ChannelType nOriginSensor, ChannelType nConvertSensor, float fFreq, float fAmp)
        {
            float fA2VScale, fA2DScale, fV2AScale, fV2DScale, fD2AScale, fD2VScale;
            float fCvtAmp = 0;

            fA2VScale = (float)25.4;
            fA2DScale = (float)25.4 * 1000;
            fV2AScale = (float)0.03937;
            fV2DScale = (float)1000;
            fD2AScale = (float)0.03937 / 1000;
            fD2VScale = (float)1 / 1000;

            if (nOriginSensor == nConvertSensor)
                return fAmp;

            if (ST_VOLT == nOriginSensor)
                return fAmp;
            /**/
            if (0 == fFreq)
                return 0;

            switch (nOriginSensor)
            {
                case ST_ACCEL:
                    {
                        switch (nConvertSensor)
                        {
                            case ST_VELO:
                                fCvtAmp = (float)(fA2VScale * fAmp * 61.42 / fFreq);
                                //				fCvtAmp = (float)(fA2VScale * fAmp * 61.7 / fFreq);

                                break;
                            //case ST_GAP:
                            //    fCvtAmp = (float)(fA2DScale * fAmp * 9.78 / (fFreq * fFreq));
                            //    break;
                        }
                    }
                    break;

                case ST_VELO:
                    {
                        switch (nConvertSensor)
                        {
                            case ST_ACCEL:
                                fCvtAmp = (float)(fV2AScale * fAmp * fFreq / 61.42);
                                break;
                            //case ST_GAP:
                            //    fCvtAmp = (float)(fV2DScale * fAmp / (fFreq * 6.28));
                            //    break;
                        }
                    }
                    break;

                    //case ST_GAP:
                    //    {
                    //        switch (nConvertSensor)
                    //        {
                    //            case ST_ACCEL:
                    //                fCvtAmp = (float)(fD2AScale * fAmp * (fFreq * fFreq) / 9.78);
                    //                break;
                    //            case ST_VELO:
                    //                fCvtAmp = (float)(fD2VScale * fAmp * 6.28 * fFreq);
                    //                break;
                    //        }
                    //    }
                    break;
                /*	case ST_VOLT:
                        {
                            switch(nConvertSensor)
                            {
                            case ST_VELO:
                                fCvtAmp = (float)(fA2VScale * fAmp / 1  / fFreq);
                                break;
                            case ST_GAP:
                                fCvtAmp = (float)(fA2DScale * fAmp  / (fFreq * fFreq));
                                break;
                            }
                            break;
                        }/**/
                default:
                    {
                        fCvtAmp = fAmp;
                    }
                    break;
            }

            return fCvtAmp;
        }

        public static bool FftDoTransform(float[][] gpfSin, float[][] gpfCos, float[] pfReal, float[] pfImg, int nTransCount)
        {
            int iMaxpower, iArg, iCntr, iPnt0, iPnt1;
            int i, j, k, a, b;
            float fTemp, fProdReal, fProdImg;
            float[] pfSin, pfCos;

            switch (nTransCount)
            {
                case 64:
                    pfSin = gpfSin[0];
                    pfCos = gpfCos[0];
                    iMaxpower = 6;
                    break;
                case 128:
                    pfSin = gpfSin[1];
                    pfCos = gpfCos[1];
                    iMaxpower = 7;
                    break;
                case 256:
                    pfSin = gpfSin[2];
                    pfCos = gpfCos[2];
                    iMaxpower = 8;
                    break;
                case 512:
                    pfSin = gpfSin[3];
                    pfCos = gpfCos[3];
                    iMaxpower = 9;
                    break;
                case 1024:
                    pfSin = gpfSin[4];
                    pfCos = gpfCos[4];
                    iMaxpower = 10;
                    break;
                case 2048:
                    pfSin = gpfSin[5];
                    pfCos = gpfCos[5];
                    iMaxpower = 11;
                    break;
                case 4096:
                    pfSin = gpfSin[6];
                    pfCos = gpfCos[6];
                    iMaxpower = 12;
                    break;
                case 8192:
                    pfSin = gpfSin[7];
                    pfCos = gpfCos[7];
                    iMaxpower = 13;
                    break;
                case 16384:
                    pfSin = gpfSin[8];
                    pfCos = gpfCos[8];
                    iMaxpower = 14;
                    break;
                case 32768:
                    pfSin = gpfSin[9];
                    pfCos = gpfCos[9];
                    iMaxpower = 15;
                    break;
                case 65536:
                    pfSin = gpfSin[10];
                    pfCos = gpfCos[10];
                    iMaxpower = 16;
                    break;
                default:
                    return false;
            }

            j = 0;

            for (i = 0; i <= nTransCount - 2; i++)
            {
                if (i < j)
                {
                    fTemp = pfReal[j];
                    pfReal[j] = pfReal[i];
                    pfReal[i] = fTemp;
                    fTemp = pfImg[j];
                    pfImg[j] = pfImg[i];
                    pfImg[i] = fTemp;
                }

                k = nTransCount / 2;
                while (k <= j)
                {
                    j = j - k;
                    k = k / 2;
                }
                j = j + k;
            }

            a = 2;
            b = 1;
            for (iCntr = 1; iCntr <= iMaxpower; iCntr++)
            {
                iPnt0 = nTransCount / a;
                if (iPnt0 == 1)
                {
                    int ddd = iCntr;
                }
                iPnt1 = 0;
                for (k = 0; k < b; k++)    ///
                {
                    i = k;
                    while (i < nTransCount)
                    {
                        iArg = i + b;
                        if (k == 0)
                        {
                            fProdReal = pfReal[iArg];
                            fProdImg = pfImg[iArg];
                        }
                        else
                        {
                            fProdReal = pfReal[iArg] * pfCos[iPnt1] +
                                        pfImg[iArg] * pfSin[iPnt1];
                            fProdImg = pfImg[iArg] * pfCos[iPnt1] -
                                        pfReal[iArg] * pfSin[iPnt1];
                        }

                        pfReal[iArg] = pfReal[i] - fProdReal;
                        pfImg[iArg] = pfImg[i] - fProdImg;
                        pfReal[i] = pfReal[i] + fProdReal;
                        pfImg[i] = pfImg[i] + fProdImg;
                        i = i + a;
                    }
                    iPnt1 = iPnt1 + iPnt0;
                }
                a = 2 * a;
                b = b * 2;
            }
            return true;
        }

        static double GetCoeffVolt(double dVoltRange, int nResolution)	//hsyu
        {
            double dCoeff;
            double dBoardRange = pow(2, nResolution);

            dCoeff = (dVoltRange * 1000) / dBoardRange;

            return dCoeff;
        }

        static double GetCoeffSensor(double dMV, int nGain, int uChGain, int nResolution, int m_nAccelUnit, bool bUnit, bool bFFt)
        {
            double dCoeff;
            double dBoardRange = pow(2, nResolution);

            if (bUnit)
                dCoeff = 1 / (dMV * (double)(nGain));
            else
                dCoeff = 1;

            dCoeff = dCoeff / pow(2, uChGain);

            if (bFFt)
            {
                if (0 == m_nAccelUnit)	// Rms == 0
                    dCoeff = dCoeff / ROOT_2;
            }

            return dCoeff;
        }

        static double GetCoeff(double dVoltRange, bool bUnit, bool bFFt, double dMV, int nGain, int uChGain, int m_nAccelUnit, int nResolution)
        {
            double dCoeff;
            double dBoardRange = pow(2, nResolution);

            if (bUnit) // (20000mV/65536)1센서단위/100mV : bit -> 센서단위
                dCoeff = (dVoltRange * 1000) / (dMV * dBoardRange * (double)(nGain));
            else	// 20000mV/65536 : bit -> mV
                dCoeff = (dVoltRange * 1000) / dBoardRange;

            dCoeff = dCoeff / pow(2, uChGain);

            if (bFFt)
            {
                if (0 == m_nAccelUnit)	// Rms == 0
                    dCoeff = dCoeff / ROOT_2;
            }

            return dCoeff;
        }

        #region 포팅용 유틸리티들
        const double TWO_PI = Math.PI * 2;
        const double ROOT_2 = 1.414213;

        static int LineToTrans(int line)
        {
            switch (line)
            {
                case 50: return 128;
                case 100: return 256;
                case 200: return 512;
                case 400: return 1024;
                case 800: return 2048;
                case 1600: return 4096;
                case 3200: return 8192;
                case 6400: return 16384;
                case 12800: return 32768;
                case 25600: return 65536;
            }
            throw new Exception("Invalid line");
        }
        internal static double pow(int n, int m) { return Math.Pow(n, m); }
        internal static double atan(double d) { return Math.Atan(d); }
        internal static double sin(double a) { return Math.Sin(a); }
        internal static double cos(double a) { return Math.Cos(a); }
        internal static float fabs(float a) { return Math.Abs(a); }
        internal static double fabs(double a) { return Math.Abs(a); }
        internal static double sqrt(double a) { return Math.Sqrt(a); }
        internal static double log10(double a) { return Math.Log10(a); }
        internal static void Fill<T>(T[] array, T value)
        {
            Fill(array, value, array.Length);
        }
        internal static void Fill<T>(T[] array, T value, int length)
        {
            for (int i = 0; i < length; i++)
                array[i] = value;
        }
        #endregion

        //---------------------------------------------- Envelop bandpass CXII 알고리즘 테스트 버전 시작-----------------------------// 
        static Dictionary<int, Real[]> hannCache = new Dictionary<int, Real[]>();
        static Dictionary<int, double> hannNormalCache = new Dictionary<int, double>();
        public static double envelop_cxii(double[] m_uGraphData, int m, double[] Pxx, int wintype, int spectrumunit, double bp1, double bp2, double deltaf, int bDecibel, int AcouWintype, double dSamplingRate)
        {
            double nWinNormal;
            nWinNormal = 0;
            const int D_MAX_BUFF = 65536;
            float[] m_pfReal = new float[D_MAX_BUFF];
            float[] m_pfImg = new float[D_MAX_BUFF];
            float[] m_dSpectrum = new float[D_MAX_BUFF];
            const float ROOT_2 = 1.414213f;
            const int M = 400;

            int nTr = m_uGraphData.Length;// m_pGdHeader->nTransCount
            int nTransCount = m_uGraphData.Length;
            double[] pRmsBuffer = new double[nTransCount / 2];


            int m_nZF = 1;
            int nMaxBuffSize = m_nZF * nTransCount;

            // Assamble Zoom fact Buffer
            double[] dMaxRealBuff = new double[nMaxBuffSize];

            int i, j;
            for (i = 0; i < nTr; i++)
            {
                dMaxRealBuff[i] = (float)m_uGraphData[i];
            }

            float fXAvg = (float)m_uGraphData.Average();

            for (i = 0; i < nTr; i++)
            {
                dMaxRealBuff[i] = dMaxRealBuff[i] - fXAvg;
            }

            // BandPass Filter HighPass Coefficeint (M define 400)
            //           double dFc1 = 0.391;		// bandpass-low cutoff frequency  (9000Hz) 
            //           double dFc2 = 0.175;		// bandpass-high cutoff frequency (20000Hz)

            double dFc1 = 0.175;		// bandpass-low cutoff frequency  (9000Hz) 
            double dFc2 = 0.391;		// bandpass-high cutoff frequency (20000Hz)

            //               double dFc1 = 0.2441;		// bandpass-low cutoff frequency  (9000Hz) 
            //               double dFc2 = 0.9277;		// bandpass-high cutoff frequency (20000Hz)

            double dFc3 = 1.0 / ((double)m_nZF * 2);		// Lowpass frequency after resampling
            // 2000/Fsamplingrate  ( operating speed * 20 )

            double[] nbpf_lc = new double[M + 1];	// sinc BPF_H coefficient ; M+1
            double[] nbpf_hc = new double[M + 1];	// sinc BPF_L coefficient ; M+1

            lpfc_generator(dFc1, nbpf_lc);	// bandpass filter low pass coefficeint
            lpfc_generator(dFc2, nbpf_hc);	// bandpass filter hign pass coefficeint

            for (i = 0; i <= M; i++)		// low--->high pass coefficent
            {
                nbpf_hc[i] = -nbpf_hc[i];
            }
            nbpf_hc[M / 2] = 1 + nbpf_hc[M / 2];


            // Convolution...
            int nConvNum = nMaxBuffSize + M;
            double[] temp1 = new double[nConvNum];
            double[] temp2 = new double[nConvNum];

            // Band-lowpass convolution
            for (i = 0; i < nMaxBuffSize; i++)
            {
                for (j = 0; j <= M; j++)
                {
                    temp1[i + j] = temp1[i + j] + dMaxRealBuff[i] * nbpf_lc[j];
                }
            }

            // Band-highpass convolution
            for (i = 0; i < nMaxBuffSize; i++)
            {
                for (j = 0; j <= M; j++)
                {
                    temp2[i + j] = temp2[i + j] + temp1[i] * nbpf_hc[j];
                }
            }

            // Rectification
            for (i = 0; i < nConvNum; i++)
            {
                temp2[i] = fabs(temp2[i]);
            }

            // Lowpass filter
            double[] nlpf_c = new double[M + 1];		// sinc LPF coefficient ; M+1

            lpfc_generator(dFc3, nlpf_c);	// bandpass filter low pass coefficeint

            //::ZeroMemory(temp1, sizeof(double)*nConvNum);
            Fill(temp1, 0, nConvNum);

            // Bandpass convolution enveloping data 	
            for (i = 0; i < nMaxBuffSize; i++)
            {
                for (j = 0; j <= M; j++)
                {
                    temp1[i + j] = temp1[i + j] + temp2[i] * nlpf_c[j];
                }
            }

            // Resampling...
            j = 0;
            for (i = 0; i < nMaxBuffSize; i++)
            {
                if (0 == (i % m_nZF))
                {
                    if (MAX_ENVBUFF <= j) break;

                    m_pfReal[j] = (float)temp1[i];
                    m_pfImg[j] = 0;
                    j++;
                }
            }

            Real[] window;
            if (!hannCache.ContainsKey(nTr))
            {
                window = CmsMath.CreateWindow(wintype, out nWinNormal, nTr);
                hannCache.Add(nTr, window);
                hannNormalCache.Add(nTr, nWinNormal);
            }
            else
            {
                window = hannCache[nTr];
                nWinNormal = hannNormalCache[nTr];
            }

            float m_fNBW = CmsMath.FftWindowFn(wintype, window, m_pfReal, m_pfImg);
            //float m_fNBW = CmsMath.FftWindowFn(wintype, out nWinNormal, nTr, m_pfReal, m_pfImg);

            CmsEnvelop.FftDoTransform(lut.gpfSin, lut.gpfCos, m_pfReal, m_pfImg, nTransCount);

            double dAmp;
            for (i = 0; i < nTransCount / 2; i++)
            {
                dAmp = (double)(m_pfReal[i] * m_pfReal[i] + m_pfImg[i] * m_pfImg[i]);
                dAmp = sqrt(dAmp);
                m_dSpectrum[i] = (float)(dAmp / (nWinNormal) * 2); // 곱하기 2는 single side로 변환시킨 것이다.
            }

            m_dSpectrum[0] = 0;


            for (i = 0; i < nTransCount / 2; i++)
            {
                dAmp = m_dSpectrum[i] / ROOT_2;//rjy 루트 2로 나눈것은 rms로 보기 위함이고 현재 default 나중에 0Peak 으로 바꿀려면 루트 2를 없애면됨
                pRmsBuffer[i] = dAmp;
                Pxx[i] = dAmp;
            }

            float fPeak = 0.0f;
            float fPeakHz = 0.0f;
            float fSumRms = 0.0f;
            double dDeltaHz = dSamplingRate / nTransCount;


            if (bp2 > dSamplingRate / 2)
                bp2 = dSamplingRate / 2;

            int nXMin = (int)(bp1 / dDeltaHz);
            int nXMax = (int)(bp2 * (1 / dDeltaHz));

            if (1 > nXMin)
                nXMin = 1;
            if (nTr < nXMax)
                nXMax = nTr;

            for (i = nXMin; i <= nXMax; i++)
            {
                fPeak = (float)pRmsBuffer[i];
                fPeakHz = (float)(i * dDeltaHz);
                fSumRms += fPeak * fPeak;
            }

            //double dwinFactor = FftWindowFn1(GetAcqOpt()->m_nWindow);
            fSumRms = fSumRms / m_fNBW;

            return sqrt(fSumRms);
        }

        //---------------------------------------------- Envelop bandpass CXII 알고리즘 테스트 버전  끝-----------------------------//
    }


    internal class CmsLookupTable
    {
        internal float[][] gpfSin = new float[11][];
        internal float[][] gpfCos = new float[11][];

        public CmsLookupTable()
        {
            int nSize = 64;
	        for(int i=0;i<11;i++)
	        {
		        InitTable(out gpfSin[i], out gpfCos[i], nSize);
		        nSize <<= 1;
	        }
        }

        static void InitTable(out float[] pfSin, out float[] pfCos, int nSize)
        {
            pfSin = new float[nSize];
            pfCos = new float[nSize];

	        double dInc = Math.PI*2 / (double)nSize;
            double dAngle = 0.0;
	        for (int i = 0; i < nSize; i++)
	        {
		        pfSin[i] = (float)Math.Sin(dAngle);
		        pfCos[i] = (float)Math.Cos(dAngle);
		        dAngle += dInc;
	        }
        }
    }

}
