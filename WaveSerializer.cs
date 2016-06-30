using DaqProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace TestCms1
{
    public enum SerializerType : byte
    {
        Serializer_Unknow = 0x00,
        Serializer_LSW = 0x01,
        Serializer_KHW = 0x02,
        Serializer_SHK = 0x03,
        Serializer_CPP = 0x04,
    }

    public class SerializerUtil
    {
        public static IWavesSerializer ToSerializer(byte type)
        {
            IWavesSerializer serializer = null;
            switch ((SerializerType)type)
            {
                case SerializerType.Serializer_LSW: serializer = new WaveDataSerializer_LSW(); break;
                case SerializerType.Serializer_KHW: serializer = new WaveDataSerializer_KHW(); break;
                case SerializerType.Serializer_SHK: serializer = new WaveDataSerializer_SHK(); break;
                case SerializerType.Serializer_CPP: serializer = new WaveDataSerializer_Cpp(); break;
            }
            return serializer;
        }
    }

    public interface IByteConvertable
    { 
        byte ToByte();
    }

    public interface IWavesSerializer : IByteConvertable
{
        void Serialize(Stream stream, WaveData[] waves);
        WaveData[] Deserialize(Stream stream);
    }

    public class WaveDataSerializer_LSW : IWavesSerializer
    {
        public void Serialize(Stream stream, WaveData[] waves)
        {
            try
            {
                var writer = new BinaryWriter(stream);
                foreach (var wave in waves)
                {
                    writer.Write(wave.AsyncDataCount); //int32
                    writer.Write(wave.ChannelId); //int32
                    writer.Write(wave.DateTime.ToOADate()); //double64

                    foreach (var v in wave.AsyncData)
                        writer.Write(v);
                }
            }
            catch (Exception ex)
            {
                //TODO Write Log
            }
        }

        public WaveData[] Deserialize(Stream stream)
        {
            WaveData[] waves = null;
            try
            {
                var reader = new BinaryReader(stream);
                if (reader.PeekChar() >= 0)
                {
                    waves = new WaveData[8];
                    for (int ch = 0; ch < waves.Length; ch++)
                    {
                        waves[ch] = new WaveData()
                        {
                            AsyncDataCount = reader.ReadInt32(),
                            ChannelId = reader.ReadInt32(),
                            DateTime = DateTime.FromOADate(reader.ReadDouble())
                        };
                        waves[ch].AsyncData = new float[waves[ch].AsyncDataCount];

                        for (int i = 0; i < waves[ch].AsyncDataCount; i++)
                            waves[ch].AsyncData[i] = reader.ReadSingle();
                    }
                }
                else (reader.BaseStream as FileStream).Seek(0, SeekOrigin.Begin);
                reader.Close();
            }
            catch (Exception ex)
            {
                //TODO Write Log
            }
            return waves;
        }
        public override string ToString()
        {
            return "LSW";
        }

        public byte ToByte()
        {
            return (byte)SerializerType.Serializer_LSW;
        }
    }

    public class WaveDataSerializer_KHW : IWavesSerializer
    {
        public void Serialize(Stream stream, WaveData[] waves)
        {
            try
            {
                var writer = new BinaryWriter(stream);
                writer.Write(waves.Length);
                foreach (var v in waves)
                {
                    writer.Write(v.AsyncDataCount); //int32
                    writer.Write(v.ChannelId); //int32
                    writer.Write(v.DateTime.ToOADate()); //double64

                    foreach (var w in v.AsyncData)
                        writer.Write(w);
                }
            }
            catch (Exception ex)
            {
                //TODO Write Log
            }
        }

        public WaveData[] Deserialize(Stream stream)
        {
            WaveData[] waves = null;
            try
            {
                var reader = new BinaryReader(stream);
                if (reader.PeekChar() >= 0)
                {
                    var chCount = reader.ReadInt32();

                    waves = new WaveData[chCount];
                    for (int ch = 0; ch < chCount; ch++)
                    {
                        waves[ch] = new WaveData()
                        {
                            AsyncDataCount = reader.ReadInt32(),
                            ChannelId = reader.ReadInt32(),
                            DateTime = DateTime.FromOADate(reader.ReadDouble()),
                        };
                        waves[ch].AsyncData = new float[waves[ch].AsyncDataCount];
                        for (int i = 0; i < waves[ch].AsyncDataCount; i++)
                            waves[ch].AsyncData[i] = reader.ReadSingle();
                    }
                }
                else (reader.BaseStream as FileStream).Seek(0, SeekOrigin.Begin);
                reader.Close();
            }
            catch (Exception ex)
            {
                //TODO Write Log
            }
            return waves;
        }
        public override string ToString()
        {
            return "KHW";
        }

        public byte ToByte()
        {
            return (byte)SerializerType.Serializer_KHW;
        }
    }

    public class WaveDataSerializer_SHK : IWavesSerializer
    {
        public void Serialize(Stream writer, WaveData[] wave)
        {
        }

        public WaveData[] Deserialize(Stream reader)
        {
            return null;
        }

        public override string ToString()
        {
            return "SHK";
        }

        public byte ToByte()
        {
            return (byte)SerializerType.Serializer_SHK;
        }
    }

    public class WaveDataSerializer_Cpp : IWavesSerializer
    {
        const int WAVE_SIZE = 262272;
        const int CHANNEL_COUNT = 8;
        public void Serialize(Stream stream, WaveData[] waves)
        {
            List <byte> buf = new List<byte>();
            foreach (var wave in waves)
            {
                buf.AddRange(BitConverter.GetBytes(wave.AsyncDataCount));
                buf.AddRange(BitConverter.GetBytes(wave.ChannelId));
                buf.AddRange(BitConverter.GetBytes(wave.DateTime.ToOADate()));

                foreach (var data in wave.AsyncData)
                    buf.AddRange(BitConverter.GetBytes(data));
            }
            stream.Write(buf.ToArray(), 0, buf.Count);
        }

        public WaveData[] Deserialize(Stream stream)
        {
            WaveData[] waves = new WaveData[CHANNEL_COUNT]; ;
            try
            {
                byte[] buf = new byte[WAVE_SIZE];
                stream.Read(buf, 0, buf.Length);

                for (int ch = 0; ch < CHANNEL_COUNT; ch++)
                {
                    int startIdx = ch * (16 + (8192 * 4));
                    waves[ch] = new WaveData()
                    {
                        AsyncDataCount = BitConverter.ToInt32(buf, startIdx),
                        ChannelId = BitConverter.ToInt32(buf, startIdx + 4),
                        DateTime = DateTime.FromOADate(BitConverter.ToDouble(buf, startIdx + 8))
                    };
                    waves[ch].AsyncData = new float[waves[ch].AsyncDataCount];
                    for (int i = 0; i < waves[ch].AsyncDataCount; i++)
                        waves[ch].AsyncData[i] = BitConverter.ToSingle(buf, startIdx + 16 + (i * 4));
                }
            }
            catch (Exception ex)
            {
                //TODO Write Log
            }
            return waves;
        }

        public override string ToString()
        {
            return "CPP";
        }

        public byte ToByte()
        {
            return (byte)SerializerType.Serializer_CPP;
        }
    }
}
