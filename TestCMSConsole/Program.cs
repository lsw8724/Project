using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCMSCommon.ADODotNET;

namespace TestCMSConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLRepository.Init();
            string format = "yyyy-MM-dd HH:mm:ss";
            CultureInfo provider = CultureInfo.InvariantCulture;

            int measureId = 4;
            var beginTime = new DateTime(2016, 6, 30, 0, 0, 0);
            var endTime = new DateTime(2016, 7, 1, 0, 0, 0);
            if (args.Length == 3)
            {
                measureId = int.Parse(args[0]);
                beginTime = DateTime.ParseExact(args[1], format, provider);
                endTime = DateTime.ParseExact(args[2], format, provider);
            }

            var table = SQLRepository.TrendData.GetDataRange(beginTime, endTime);
            foreach(var row in table)
            {
                Console.WriteLine(row.MeasureId + "," + row.TimeStamp + "," + row.Scalar);
            }
        }
    }
}
