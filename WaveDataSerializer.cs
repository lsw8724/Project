using DaqProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestCms1
{
    public interface IWaveSerializer
    {
        void Serialize(BinaryWriter binWriter, WaveData[] wave);
        WaveData[] Deserialize(BinaryReader binReader);
    }
    public class WaveDataSerializer_LSW : IWaveSerializer
    {
        public void Serialize(BinaryWriter writer, WaveData[] waves)
        {
            try
            {
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

        public WaveData[] Deserialize(BinaryReader reader)
        {
            WaveData[] waves = new WaveData[8];
            try
            {
                for(int ch=0; ch<waves.Length; ch++)
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
            catch (Exception ex)
            {
                //TODO Write Log
            }
            return waves;
        }
    }
    public class WaveDataSerializer_KHW : IWaveSerializer
    {
        public void Serialize(BinaryWriter writer, WaveData[] waves)
        {
            try
            {
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

        public WaveData[] Deserialize(BinaryReader reader)
        {
            WaveData[] waves = null;
            try
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
            catch (Exception ex)
            {
                //TODO Write Log
            }
            return waves;
        }
    }
    public class WaveDataSerializer_SHK : IWaveSerializer
    {
        public void Serialize(BinaryWriter writer, WaveData[] wave)
        {
        }

        public WaveData[] Deserialize(BinaryReader reader)
        {
            return null;
        }
    }
}
