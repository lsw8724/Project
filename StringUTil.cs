using System;
using System.IO;
using System.Text;
using System.Linq;
using DaqProtocol;

namespace TestCms1
{
    public class StringUTil
    {
        public static byte[] ToBytes(string data)
        {
            if(data.Last() != '\0')
                data += '\0'; //C++에서 문자열 끝 처리
            ASCIIEncoding encode = new ASCIIEncoding();
            return encode.GetBytes(data); ;
        }

        public static string ToString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
