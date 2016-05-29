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

    enum Monitor_Cmd
    {
        Stop = 0x0,
        Start = 0x1,
        ConfigReceive = 0x2 
    }

    enum Wave_Cmd
    {

    }

    enum MeasureType
    {
        RMS = 0,
        PeakToPeak = 1,
        Peak = 2,
        Lift_Shock = 3,
        Lift_Move = 4
    }

    enum ReceiverType
    {
        Daq5509 = 0,
        File = 1,
        Network = 2
    }
    enum RecoderType
    {
        File = 0,
        Network = 1
    }

    enum TrasCmd
    {
        MsgType_Cmd_Start,
        MsgType_Cmd_Stop,
    }
}
