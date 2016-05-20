using DaqProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TSICommon;


namespace TSICommon
{
    public class RobinMath
    {
        //public static double[] CalculateScalars(DateTime now, TsiMsg.WaveData wave, RobinChannel ch, RobinMeasure[] measures, float gap, WinTrace trace)
        //{
        //    return CalculateScalars(now, wave.AsyncData, wave.Rpm, ch, measures, gap, trace, null);
        //}

        public static double[] CalculateScalars(DateTime now, float[] values, float rpm, RobinChannel ch, RobinMeasure[] measures, float gap)
        {
            //var valuesLength = wave.AsyncDataCount;
            var valuesLength = values.Length;

            int m = (int)Math.Round(Math.Log10((double)valuesLength) / Math.Log10(2.0));
            var Fs = ch.AsyncFMax * 2.56;
            var freq = new double[valuesLength / 2];
            var rms = new double[valuesLength / 2];
            //var windowFunc = WindowFunction.Hamming;

            var measList = new double[measures.Length];

            var logStr = new StringBuilder();
            logStr.Append("CH" + ch.Id + ",");
            double[] Pxx;
            for (int i = 0; i < measList.Length; i++)
            {
                var measure = measures[i];

                switch (measure.Type)
                {
                    case RobinMeasureType.Bandpass:
                        measList[i] = GetBandpass(values,
                                                    ch.AsyncFMax,
                                                    WindowFunction.Hanning,
                                                    measure.SpectrumUnit,  // Unit2Type.rms,
                                                    measure.BandLow,
                                                    measure.BandHigh,
                                                    measure.Integral);

                        break;

                    case RobinMeasureType.OrderBandpass:
                        double bandLow = measure.BandLow * rpm / 60.0;
                        double bandHigh = measure.BandHigh * rpm / 60.0;
                        measList[i] = GetBandpass(values,
                                                    ch.AsyncFMax,
                                                    WindowFunction.Hanning,
                                                    measure.SpectrumUnit,  //Unit2Type.rms,
                                                    bandLow,
                                                    bandHigh,
                                                    measure.Integral);

                        break;

                    case RobinMeasureType.CrestFactor:
                        measList[i] = CmsMath.crestFactorValue(values, valuesLength);
                        break;

                    case RobinMeasureType.Peak:
                        measList[i] = CmsMath.peakValue(values, valuesLength);
                        break;

                    case RobinMeasureType.DC:
                        measList[i] = gap * 1000.0 / ch.Sensitivity + measure.Offset; //TODO: Sensitivity에 보정치 적용(대신 저 *1000을 업애면서 기존DB와 호환되게)
                        break;

                    case RobinMeasureType.PtoP:
                        int CmsLowPeakIndex = 5;
                        if (CmsLowPeakIndex <= 0)
                        {
                            measList[i] = CmsMath.peak2peak(values, valuesLength);
                        }
                        else
                        {
                            var orderedValues = values.OrderBy(v => v).ToArray();
                            var lowerPeak = orderedValues[CmsLowPeakIndex];
                            var upperPeak = orderedValues[orderedValues.Length - 1];
                            measList[i] = upperPeak - lowerPeak;
                        }
                        break;

                    case RobinMeasureType.Custom:
                        measList[i] = GetBandpass(values,
                                                    ch.AsyncFMax,
                                                    WindowFunction.Hanning,
                                                    measure.SpectrumUnit,  // Unit2Type.rms,
                                                    measure.BandLow,
                                                    measure.BandHigh,
                                                    measure.Integral);

                        break;

                    default:
                        logStr.Append("### UNDEFINED FUNCTION TYPE ###");
                        break;
                }

                //logStr.Append(measure.Name + "-" + measList[i].ToString("F3") + ",");
            }
            
            return measList;
        }

        internal static double GetBandpass(float[] timeData, double maxFrequency, WindowFunction windowType, Unit2Type unitType/*무시*/, double bandLow, double bandHigh, RobinIntegralType integralType)
        {
            double result = 0.0f;
            int dataSize = timeData.Length;

            // Sampling Rate와 데이터 개수 계산
            float maxFreq = (float)maxFrequency;
            float srate = (float)(maxFreq * 2.56);
            int _size = dataSize;

            // Time 데이터 버퍼
            var fftdata = new double[_size];
            for (int index = 0; index < _size; index++)
                fftdata[index] = timeData[index];

            // FFT 결과 데이터 버퍼
            var fftresult = new double[_size];

            double deltaf = srate / _size;
            // DSP의 autospectrum_d 함수 호출
#if false // nadamath 알고리즘
            result = nadaMath.autospectrum_d(ref fftdata[0],
                                                    (int)(Math.Log(_size, 2)),
                                                    ref fftresult[0],
                                                    (int)(windowType),
                                                    (int)Unit2Type.rms,     // (int)unitType,    rms가 default임
                                                    bandLow,
                                                    bandHigh,
                                                    deltaf,
                                                    0,
                                                    0);
#else //CXII 알고리즘
            //result = CmsMath.autospectrum_d(fftdata,
            //                            (int)(Math.Log(_size, 2)),
            //                            fftresult,
            //                            windowType,
            //                            Unit2Type.rms,     // (int)unitType,    rms가 default임
            //                            bandLow,
            //                            bandHigh,
            //                            deltaf,
            //                            0,
            //                            0);
            result = CmsMath.autospectrum_cxii(fftdata,
                                        (int)(Math.Log(_size, 2)),
                                        fftresult,
                                        (int)(windowType),
                                        (int)Unit2Type.rms,     // (int)unitType,    rms가 default임
                                        bandLow,
                                        bandHigh,
                                        deltaf,
                                        0,
                                        0,
                                        srate);

#endif
            if (integralType != RobinIntegralType.None)
            {
                double[] window = new double[_size];
                double factor = 8.0 * Math.Atan(1.0) / (_size - 1);

                for (int i = 0; i < _size; i++)
                    window[i] = 0.5 - 0.5 * Math.Cos(factor * i);

                double WinScale = 0;
                for (int i = 0; i < _size; i++)
                    WinScale += window[i];

                double WinScale2 = 0;
                for (int i = 0; i < _size; i++)
                    WinScale2 += (window[i] * window[i]);

                double rms = 0.0;
                int length = (int)(_size / 2.56);
                double freq, omega, tempResult;
                for (int i = 0; i < length; i++)
                {
                    freq = i * deltaf;
                    omega = 2 * 4 * Math.Atan(1) * freq;

                    if (0 == i)
                        fftresult[i] = 0;
                    else
                        fftresult[i] = (float)(fftresult[i] * 9.80665f * 1000.0f / omega);

                    if (integralType == RobinIntegralType.Double)
                    {
                        if (0 == i)
                            fftresult[i] = 0;
                        else
                        {
                            fftresult[i] = (float)(fftresult[i] * 1000.0f / omega);
                            /// [김보근]
                            /// 변위인 경우 rms --> peak --> peak to peak로 변환하기 위해서 fft 결과 데이터에 루트 2로 나누고 2를 곱해줌
                            /// fftresult[i] = fftresult[i] / sqrt(2.0) * 2.0
                            fftresult[i] = (float)(fftresult[i] / Math.Sqrt(2.0) * 2.0);
                            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        }
                    }

                    if ((bandLow <= freq) && (freq <= bandHigh))
                    {
                        tempResult = fftresult[i] * fftresult[i];
                        tempResult *= (WinScale * WinScale) / (_size * WinScale2);
                        rms += tempResult;
                    }
                }
                result = Math.Sqrt(rms);
            }
            return result;
        }
    }
}