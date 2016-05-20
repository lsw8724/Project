using DaqProtocol; 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace TestCms1
{
    class ConfigFileSerializer_Receiver
    {
        public static void Serialize(string filename, BindingList<IWavesReceiver> instance)
        {
            FileStream fileStreamObject = null;
            try
            {
                fileStreamObject = new FileStream(filename, FileMode.Create);
                IFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStreamObject, instance);
            }
            finally
            {
                fileStreamObject.Close();
            }
        }

        public static BindingList<IWavesReceiver> Deserialize(string filename)
        {
            FileStream fileStreamObject = null;
            try
            {
                fileStreamObject = new FileStream(filename, FileMode.Open);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                var obj = binaryFormatter.Deserialize(fileStreamObject);
                return obj as BindingList<IWavesReceiver>;
            }
            finally
            {
                fileStreamObject.Close();
            }
        } 
    }
    class ConfigFileSerializer_Measure
    {
        public static void Serialize(string filename, BindingList<IMeasureCalculator> instance)
        {
            FileStream fileStreamObject = null;
            try
            {
                fileStreamObject = new FileStream(filename, FileMode.Create);
                IFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStreamObject, instance);
            }
            finally
            {
                fileStreamObject.Close();
            }
        }

        public static BindingList<IMeasureCalculator> Deserialize(string filename)
        {
            FileStream fileStreamObject = null;
            try
            {
                fileStreamObject = new FileStream(filename, FileMode.Open);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                var obj = binaryFormatter.Deserialize(fileStreamObject);
                return obj as BindingList<IMeasureCalculator>;
            }
            finally
            {
                fileStreamObject.Close();
            }
        }
    }
    class ConfigFileSerializer_Recoder
    {
        public static void Serialize(string filename, BindingList<IWaveRecoder> instance)
        {
            FileStream fileStreamObject = null;
            try
            {
                fileStreamObject = new FileStream(filename, FileMode.Create);
                IFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStreamObject, instance);
            }
            finally
            {
                fileStreamObject.Close();
            }
        }

        public static BindingList<IWaveRecoder> Deserialize(string filename)
        {
            FileStream fileStreamObject = null;
            try
            {
                fileStreamObject = new FileStream(filename, FileMode.Open);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                var obj = binaryFormatter.Deserialize(fileStreamObject);
                return obj as BindingList<IWaveRecoder>;
            }
            finally
            {
                fileStreamObject.Close();
            }
        }
    }
}
