using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ClientCloud
{
   public static class ConvertList
    {
       public static byte[] ListToByteArray(List<string> obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

       public static List<string> ByteArrayToList(byte[] arrBytes)
        {
            try
            {
                MemoryStream memStream = new MemoryStream();
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                List<string> obj = (List<string>)binForm.Deserialize(memStream);
                return obj;
            }
            catch(Exception ex)
            {
                List<string> tempList = new List<string>();
                tempList.Add("false");
                return tempList;
            }
        }

        public static byte[] FileWaysToByteArray(Dictionary<string, string> obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        public static Dictionary<string, string> ByteArrayToFileWays(byte[] arrBytes)
        {
            try
            {
                MemoryStream memStream = new MemoryStream();
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                Dictionary<string, string> obj = (Dictionary<string, string>)binForm.Deserialize(memStream);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
