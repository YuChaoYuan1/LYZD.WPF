using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LYZD.Core.Function
{
    /// <summary>
    /// 深拷贝帮助类
    /// </summary>
  public   class CopyClassHelper
    {
        /// <summary>
        /// 二进制序列化拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopyByBinary<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
        public static T DeepCopyByXml<T>(T obj)
        {
            object result;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                result = xmlSerializer.Deserialize(ms);
                ms.Close();
            }
            return (T)result;
        }

    }
}
