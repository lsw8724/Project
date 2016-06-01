using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestCms1
{
    public class ConstantMember
    {
        public const int AsyncFMax = 3200;
        public const int AsyncLine = 3200;
    }

    public enum CmdType : byte { Stop = 0x00, Start = 0x01, ReadConfig = 0x02 }

    public enum MeasureType
    {
        RMS = 0,
        PeakToPeak = 1,
        Peak = 2,
        Lift_Shock = 3,
        Lift_Move = 4
    }

    public enum ReceiverType
    {
        Daq5509 = 0,
        File = 1,
        Network = 2
    }

    public enum RecoderType
    {
        File = 0,
        Network = 1
    }

    public enum TrasCmd
    {
        MsgType_Cmd_Start,
        MsgType_Cmd_Stop,
    }
}
