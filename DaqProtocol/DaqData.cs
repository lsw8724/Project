using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TraceTool;

namespace DaqProtocol
{
    public class DaqData
    {
        public float[] ChannelsGap = new float[8]; //1초에 8번 측정
        public float Rpm1;
        public float Rpm2;
        public float[][] ChannelsAsyncs = new float[8][];   //8개 채널의 데이타들
    }
    public class ByteQueuedStream
    {
        Stream stream;
        public LinkedList<byte> byteQueue = new LinkedList<byte>();

        public ByteQueuedStream(Stream stream)
        {
            this.stream = stream;
        }

        //모자른 만큼만 추가로 스트림에서 읽음
        public byte[] PullLast(int size)
        {
            var readSize = size - byteQueue.Count;
            if(readSize<= 0)
                return byteQueue.Take(size).ToArray();

            var block = DaqDataPacket.ReadBlock(stream, readSize);
            foreach (var b in block)
                byteQueue.AddLast(b);
            return byteQueue.Take(size).ToArray();
        }
        public byte[] PopFront(int size)
        {
            var bytes = byteQueue.Take(size).ToArray();
            RemoveFront(size);
            return bytes;
        }
        public void RevertFront(byte[] bytes)
        {
            for (int i = bytes.Length - 1; i >= 0; i--)
                byteQueue.AddFirst(bytes[i]);
        }

        public void PopFrontByte() { byteQueue.RemoveFirst(); }
        public void RemoveFront(int size)
        {
            for (int i = 0; i < size; i++)
                byteQueue.RemoveFirst();
        }
    }
    public class DaqDataPacket
    {
        public DaqDataType DataType { get; private set; }
        //public ushort WordCount { get; private set; }

        public byte[] Header { get; private set; }
        public byte[] Bytes { get; private set; }

        public DaqCmdType CmdType { get { return (DaqCmdType)Header[0]; } set { Header[0] = (byte)value; } }
        public DaqResponse Response { get { return (DaqResponse)Header[2]; } set { Header[2] = (byte)value; } }   //ushort배열에서 1번 인덱스이므로 byte에서는 2

        public ushort[] ToWords()
        {
            var words = new ushort[Bytes.Length / 2];
            Buffer.BlockCopy(Bytes, 0, words, 0, Bytes.Length);
            return words;
        }
        public short[] ToShorts()
        {
            var shorts = new short[Bytes.Length / 2];
            Buffer.BlockCopy(Bytes, 0, shorts, 0, Bytes.Length);
            return shorts;
        }

        static DaqHeaderType IsValidHeader(DaqClient parent, byte[] header)
        {
            var type = header[0];
            if (header[1] != 0)
            {
                //System.Diagnostics.Trace.WriteLine("Header[1]: " + header[1]);
                //return DaqHeaderType.Invalid;
            }

            if (type == (byte)DaqDataType.DAQ_DATA)
            {
                var samplerate = (header[3] << 8) | header[2];
                return samplerate == 512 ? DaqHeaderType.Data : DaqHeaderType.Invalid;
            }
            else if (type == (byte)DaqDataType.GAP_DATA)
            {
                var dataCount = (header[3] << 8) | header[2];
                //System.Diagnostics.Trace.WriteLine("Gap: " + dataCount);
                return dataCount == 12 ? DaqHeaderType.Data : DaqHeaderType.Invalid;
            }
            else if (type == (byte)DaqDataType.SYNC_AMP)
            {
                var dataCount = (header[3] << 8) | header[2];
                return dataCount == 2 ? DaqHeaderType.Data : DaqHeaderType.Invalid; //2인지 불확실
            }
            else
                return DaqHeaderType.Invalid;
        }
               

        public static DaqDataPacket RecvDataPacket(DaqClient parent, ByteQueuedStream ns, string commandNameForLog, WinTrace trace = null)
        {
        _START:
            int invalidHeaderCount = 0;
            byte[] firstInvalidHeader = null;
            while (IsValidHeader(parent, ns.PullLast(4)) == DaqHeaderType.Invalid)
            {
                ns.PopFrontByte();
                if (invalidHeaderCount == 0)
                    firstInvalidHeader = ns.byteQueue.ToArray();
                invalidHeaderCount++;
            }

            var header = ns.PopFront(4);
            var type = (DaqDataType)header[0];
            var wordCount = (ushort)(BitConverter.ToUInt16(header, 2) * 2);
            if (trace != null && type == DaqDataType.DAQ_DATA)
                trace.Send("Daq DATA Packet - wordCount:" + wordCount);

            int readSize = 0;
            if (type == DaqDataType.DAQ_DATA)
                readSize = 1024;
            else if (type == DaqDataType.GAP_DATA)
            {
                //if (wordCount == 12)
                readSize = 48 - 4;
            }
            else if (type == DaqDataType.SYNC_AMP)
            {
                readSize = 2;
            }
            else if (Enum.IsDefined(typeof(DaqCmdType), (ushort)type))
            {
                readSize = DaqCommand.DefaultPacketSize;
                ns.PullLast(readSize);
                var data_ = ns.PopFront(readSize);
                ns.RevertFront(header);
                ns.RevertFront(data_);
                ns.PopFrontByte();
                goto _START;
            }
            else
            {
                ns.RevertFront(header);
                ns.PopFrontByte();
                goto _START;
            }

            ns.PullLast(readSize);
            var data = ns.PopFront(readSize);
            if (IsValidHeader(parent, ns.PullLast(4)) == DaqHeaderType.Invalid)
            {
                Debug.WriteLine("====================");
                ns.RevertFront(header);
                ns.RevertFront(data);
                ns.PopFrontByte();
                goto _START;
            }

            //Debug.WriteLine(" " + type + "," + invalidHeaderCount);
            //if(invalidHeaderCount != 0)
            //    Debug.WriteLine(" " + type + "," + invalidHeaderCount + "," + firstInvalidHeader.Length);
            return new DaqDataPacket { DataType = type, Header = header, Bytes = data };
        }

        public static DaqDataPacket RecvPacket(DaqClient parent, NetworkStream ns, string commandNameForLog)
        {
            int nTryCount = 0;
            _START:
            var byteQueue = new Queue<byte>(ReadBlock(ns, 4));

            while(IsValidHeader(parent, byteQueue.ToArray()) == DaqHeaderType.Invalid)
            {
                byteQueue.Dequeue();
                var ch = ns.ReadByte();
                if(ch < 0)
                    throw new DaqException("Header Read Failed - length:" + byteQueue.Count);
                byteQueue.Enqueue((byte)ch);
            }

            var header = byteQueue.ToArray();
            var type = (DaqDataType)header[0];
            var wordCount = (ushort)(BitConverter.ToUInt16(header, 2) * 2);

            int readSize = 0;
            if (type == DaqDataType.DAQ_DATA)
                readSize = 1024;
            else if (type == DaqDataType.GAP_DATA)
            {
                //if (wordCount == 12)
                readSize = 48 - 4;
            }
            else if (type == DaqDataType.SYNC_AMP)
            {
                readSize = 2;
            }
            else if (Enum.IsDefined(typeof(DaqCmdType), (ushort)type))
            {
                readSize = DaqCommand.DefaultPacketSize;
            }
            //else
            //{
            //    if (nTryCount++ < 5)
            //    {
            //        System.Diagnostics.Trace.WriteLine("RecvPacket Retry " + nTryCount);
            //        goto _START;
            //    }
            //    System.Diagnostics.Trace.WriteLine("Wrong Data Type - " + type + ", command:" + commandNameForLog);
            //    throw new DaqException("Wrong Data Type - " + type + ", command:" + commandNameForLog);
            //}

            var data = ReadBlock(ns, readSize);

            return new DaqDataPacket { DataType = type, Header = header, Bytes = data };
        }

        public static byte[] ReadBlock(Stream stream, int size)
        {
            var bytes = new byte[size];
            int totalReaded = 0;
            while (totalReaded < bytes.Length)
            {
                var leftedSize = size - totalReaded;
                var readed = stream.Read(bytes, totalReaded, leftedSize);
                totalReaded += readed;
            }
            //Debug.WriteLine("totalRead:" + totalReaded);
            return bytes;
        }
    }

}
