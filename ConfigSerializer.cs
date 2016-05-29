using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using DaqProtocol;
using System.ComponentModel;

namespace TestCms1
{
    public interface IConfigSerializer
    {
        void Serialize<T>(BinaryWriter writer, IList<T> list);
        IList<T> DeSerialize<T>(BinaryReader reader);
    }

    public class ConfigSerializer_LSW : IConfigSerializer
    {
        public void Serialize<T>(BinaryWriter writer, IList<T> list)
        {
            writer.Write(list.Count);
            foreach (var obj in list)
            {
                writer.Write(obj.ToString());
                string json = JsonConvert.SerializeObject(obj);
                writer.Write(json);
            }
        }

        public IList<T> DeSerialize<T>(BinaryReader reader)
        {
            IList<T> list = new BindingList<T>();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string json = reader.ReadString().ToString();
                VDPMReceiver test = JsonConvert.DeserializeObject<VDPMReceiver>(json);              
                dynamic result = JsonConvert.DeserializeObject<dynamic>(json);
                list.Add(result);
            }
            return list;
        }
    }
}
