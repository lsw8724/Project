using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using AxTeeChart;
//using MathNet.Numerics.Transformations;

namespace TSICommon
{
    public class MathEx
    {
        //static RealFourierTransformation rft = new RealFourierTransformation(); // default convention

        //public static void Transform(double[] data, out double[] freqReal, out double[] freqImag)
        //{
        //    rft.TransformForward(data, out freqReal, out freqImag);
        //}

        /// <summary>
        /// min~max범위를 넘지않는 v를 리턴
        /// </summary>
        public static double MinMax(double min, double max, double v)
        {
            if (v < min)
                return min;
            if (v > max)
                return max;
            return v;
        }

        public static double CalAmplitude(double re, double im, int perion)
        {
            //return Math.Sqrt((re * re) + (im * im)) / perion * 2 * Math.Sqrt(2);
            return Math.Sqrt((re * re) + (im * im)) / perion * 2 * 2;
        }

        /// <summary>
        /// Phase값을 계산
        /// </summary>
        /// <param name="re">실수부</param>
        /// <param name="im">허수부</param>
        /// <param name="option">128 or 256</param>
        /// <param name="order">1X이면 1, 2X이면 2, nX이면 n</param>
        /// <returns>Phase값(degree)</returns>
        public static double CalPhase(double re, double im, int option, double rpm, double order, bool Absflag)
        {
            var temp = option == 128 ? 1.83333e-3 : 9.16666e-4;
            var interpol = temp * rpm * order;
            //double W = 6.3916E-6 * rpm * order;
            //double r = 0.9999;

            double ReturnVal;

            if (Absflag)
                ReturnVal = -(Math.Atan2(im, re) * 180.0 / Math.PI + interpol) + 1311.0 / (rpm * order) + 0.1244;// + 816.3 / rpm + 0.00959 ;
            else
                ReturnVal = -(Math.Atan2(im, re) * 180.0 / Math.PI + interpol) + 1311.0 / (rpm * order) + 0.1244;

            //if (ReturnVal < 0)
            //    ReturnVal += 360.0;
            //else if (ReturnVal >= 360.0)
            //    ReturnVal -= 360.0;

            return ReturnVal;
        }

        public const double ToRadianConst = Math.PI / 180.0; 
        public const double ToDegreeConst = 180.0 / Math.PI;

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        public static double Radius(double hw, double hh)
        {
            //double hw = width / 2.0;
            //double hh = height / 2.0;
            return Math.Sqrt(hw * hw + hh * hh);// / 2;
        }

        public static bool Intersect(PointLf ap, PointLf bp, ref PointLf ip)
        {
            var zero = new PointLf(0,0);
            return Intersect(zero, ap, zero, bp, ref ip);
        }

        public static bool Intersect(PointLf ap1, PointLf ap2, PointLf bp1, PointLf bp2, ref PointLf ip)
        {
            const double intersectionEpsilon = 1.0e-30;

            double under = (bp2.Y - bp1.Y)*(ap2.X - ap1.X) - (bp2.X - bp1.X)*(ap2.Y - ap1.Y);
            if (under < intersectionEpsilon) return false;

            double _t = (bp2.X - bp1.X)*(ap1.Y - bp1.Y) - (bp2.Y - bp1.Y)*(ap1.X - bp1.X);
            double _s = (ap2.X - ap1.X)*(ap1.Y - bp1.Y) - (ap2.Y - ap1.Y)*(ap1.X - bp1.X);

            double t = _t/under;
            double s = _s/under;
            if (t < 0.0 || t > 1.0 || s < 0.0 || s > 1.0) return false;
            if (_t == 0 && _s == 0) return false;

            ip.X = ap1.X + t*(double) (ap2.X - ap1.X);
            ip.Y = ap1.Y + t*(double) (ap2.Y - ap1.Y);

            return true;
        }


        public static void UnwrapDegree(double[] x)//unwrap in degree unit
        {
            int length = x.Length;
            double[] y = x;

            int i, done_flag;
            y[0] = x[0];
            done_flag = 0;
            for (i = 1; i < length; i++)
            {
                y[i] = x[i];
                while ((y[i] - y[i - 1]) > 180.0)
                {
                    y[i] = y[i] - 360.0;
                    done_flag = 1;
                }
                if (done_flag == 1)
                {
                    done_flag = 0;
                    continue;
                }
                while ((y[i] - y[i - 1]) < -180.0)
                {
                    y[i] = y[i] + 360.0;
                }
            }
        }

        
        public static void Smax(double[] x1, double[] x2, out double smax, out double angle) //smax, sppmax are scalar values
        {
            double s, spp, s_max, spp_max;
            int half_length, opposite;
            int i;
            int index;

            s_max = x1[0] * x1[0] + x2[0] * x2[0];
            index = 0;

            for (i = 1; i < x1.Length; i++)
            {
                s = x1[i] * x1[i] + x2[i] * x2[i];
                if (s > s_max)
                {
                    index = i;
                    s_max = s;
                }
            }

            smax = Math.Sqrt(s_max);
            angle = ((Math.Atan2(x2[index], x1[index]) -0.5*Math.PI)* 180.0 / Math.PI); //return angle of smax
        }

        public static PointLf CalRotatedPointLf(PointLf ptStartPointLf, double fRotationDegree, bool cw)
        {
            var ptRotationCenter = new PointLf(0,0);

            if (cw)
                return CalCWRotatedPointLf(ptStartPointLf, fRotationDegree, ptRotationCenter);
            else
                return CalCCWRotatedPointLf(ptStartPointLf, fRotationDegree, ptRotationCenter);
        }

        public static PointLf CalCWRotatedPointLf(PointLf ptStartPointLf, double fRotationDegree)
        {
            var ptRotationCenter = new PointLf(0, 0);
            return CalCWRotatedPointLf(ptStartPointLf, fRotationDegree, ptRotationCenter);
        }
        public static PointLf CalCWRotatedPointLf(PointLf ptStartPointLf, double fRotationDegree, PointLf ptRotationCenter)
        {
            PointLf ptDestPointLf = new PointLf(0, 0); //시작점과 회전각도로 계산할 끝점

            const double TWOPI = Math.PI * 2;
            //끝점 계산
            //원점을 기준으로 {x, y}를 시계방향으로 a(radian)만큼 회전시킨 점을 구하는 공식.
            //x' = x*Math.Cos(a) - y*Math.Sin(a)
            //y' = x*Math.Sin(a) + y*Math.Cos(a)
            double fTotalRotationRadian = fRotationDegree * TWOPI / 360;
            ptDestPointLf.X = (ptStartPointLf.X - ptRotationCenter.X) * Math.Cos(fTotalRotationRadian)
             - (ptStartPointLf.Y - ptRotationCenter.Y) * Math.Sin(fTotalRotationRadian) + ptRotationCenter.X;
            ptDestPointLf.Y = (ptStartPointLf.X - ptRotationCenter.X) * Math.Sin(fTotalRotationRadian)
             + (ptStartPointLf.Y - ptRotationCenter.Y) * Math.Cos(fTotalRotationRadian) + ptRotationCenter.Y;

            return ptDestPointLf;
        }

        public static PointLf CalCCWRotatedPointLf(PointLf ptStartPointLf, double fRotationDegree)
        {
            var ptRotationCenter = new PointLf(0,0);
            return CalCCWRotatedPointLf(ptStartPointLf, fRotationDegree, ptRotationCenter);
        }

        public static PointLf CalCCWRotatedPointLf(PointLf ptStartPointLf, double fRotationDegree, PointLf ptRotationCenter)
        {
            PointLf ptDestPointLf = new PointLf(0, 0); //시작점과 회전각도로 계산할 끝점
            const double TWOPI = Math.PI * 2;
            //끝점 계산
            //원점을 기준으로 {x, y}를 시계방향으로 a(radian)만큼 회전시킨 점을 구하는 공식.
            //x' = x*Math.Cos(a) + y*Math.Sin(a)
            //y' = -x*Math.Sin(a) + y*Math.Cos(a)
            double fTotalRotationRadian = fRotationDegree * TWOPI / 360;
            ptDestPointLf.X = (ptStartPointLf.X - ptRotationCenter.X) * Math.Cos(fTotalRotationRadian)
             + (ptStartPointLf.Y - ptRotationCenter.Y) * Math.Sin(fTotalRotationRadian) + ptRotationCenter.X;
            ptDestPointLf.Y = -(ptStartPointLf.X - ptRotationCenter.X) * Math.Sin(fTotalRotationRadian)
             + (ptStartPointLf.Y - ptRotationCenter.Y) * Math.Cos(fTotalRotationRadian) + ptRotationCenter.Y;
            return ptDestPointLf;
        }


        public static IEnumerable<double> GenerateSin(double fo, double Fs)
        {
            double Ts = 1/Fs;
            double step = Ts;//1.0 - Ts;
            double t = 0;
            while (t < 1.0 - Ts)
            {
                yield return 2*Math.Sin(2*Math.PI*fo*t);
                t += step;
            }
        }

        public static System.Drawing.Rectangle CalculateAABB(System.Drawing.Size size, double radian)
        {
            return CalculateAABB(0, 0, size.Width / 2, size.Height / 2, radian);
        }

        public static System.Drawing.Rectangle CalculateAABB(System.Drawing.Rectangle rect, double radian)
        {
            return CalculateAABB(rect.X, rect.Y, rect.Width/2, rect.Height/2, radian);
        }

        public static System.Drawing.Rectangle CalculateAABB(int m_PosX, int m_PosY, int m_HalfSizeX, int m_HalfSizeY, double radian)
        {
            // corner_1 is right-top corner of unrotated rectangle, relative to m_Pos.
            // corner_2 is right-bottom corner of unrotated rectangle, relative to m_Pos.
            var corner_1_x = m_HalfSizeX;
            var corner_2_x = m_HalfSizeX;
            var corner_1_y = -m_HalfSizeY;
            var corner_2_y = m_HalfSizeY;

            var sin_o = Math.Sin(radian);
            var cos_o = Math.Cos(radian);

            // xformed_corner_1, xformed_corner_2 are points corner_1, corner_2 rotated by angle radian.
            var xformed_corner_1_x = corner_1_x*cos_o - corner_1_y*sin_o;
            var xformed_corner_1_y = corner_1_x*sin_o + corner_1_y*cos_o;
            var xformed_corner_2_x = corner_2_x*cos_o - corner_2_y*sin_o;
            var xformed_corner_2_y = corner_2_x*sin_o + corner_2_y*cos_o;

            // ex, ey are extents (half-sizes) of the final AABB.
            var ex = Math.Max(Math.Abs(xformed_corner_1_x), Math.Abs(xformed_corner_2_x));
            var ey = Math.Max(Math.Abs(xformed_corner_1_y), Math.Abs(xformed_corner_2_y));

            var aabb_min_x = m_PosX - ex;
            var aabb_max_x = m_PosX + ex;
            var aabb_min_y = m_PosY - ey;
            var aabb_max_y = m_PosY + ey;
            return new System.Drawing.Rectangle((int)aabb_min_x, (int)aabb_min_y, (int)(aabb_max_x - aabb_min_x), (int)(aabb_max_y - aabb_min_y));
        }

        public static bool InRange(double v, double lowerBound, double max)
        {
            return v >= lowerBound && v < max;
        }
    }

    public class PointLf
    {
        public double X;
        public double Y;

        public PointLf(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class RectangleLf
    {
        public double Left { get; set; }
        public double Right { get; set; }
        public double Top { get; set; }
        public double Bottom { get; set; }
        public double Depth { get; set; }

        public RectangleLf Extand(double s)
        {
            Left *= s;
            Right *= s;
            Top *= s;
            Bottom *= s;
            Depth *= s;

            return this;
        }

        public double Min2D()
        {
            var values = new double[] { Left, Right, Top, Bottom };
            return values.Min();
        }
        public double Max2D()
        {
            var values = new double[] { Left, Right, Top, Bottom };
            return values.Max();
        }

        //public void SetEqualAxis()
        //{
        //    var minVal = Math.Min(Left, Top);
        //    var maxVal = Math.Max(Right, Bottom);
        //    var centerX = Left + (Right - Left) / 2;
        //    var centerY = Top + (Bottom - Top) / 2;
        //    Top = centerY - minVal / 2;
        //    Bottom = centerY + maxVal / 2;
        //    Left = centerX + minVal/2;
        //    Right = centerX + maxVal / 2;
        //}
    }
}
