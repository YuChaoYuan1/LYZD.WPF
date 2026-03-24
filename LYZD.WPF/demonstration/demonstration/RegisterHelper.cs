using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace demonstration
{
    /// <summary>
    /// 软件注册帮助类
    /// </summary>
    class RegisterHelper
    {
        ///<summary>
        /// 获取硬盘卷标号
        ///</summary>
        ///<returns></returns>
        private string GetDiskVolumeSerialNumber()
        {
            ManagementClass mc = new ManagementClass("win32_NetworkAdapterConfiguration");
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
            disk.Get();
            return disk.GetPropertyValue("VolumeSerialNumber").ToString();
        }

        ///<summary>
        /// 获取CPU序列号
        ///</summary>
        ///<returns></returns>
        private string GetCpu()
        {
            string strCpu = null;
            ManagementClass myCpu = new ManagementClass("win32_Processor");
            ManagementObjectCollection myCpuCollection = myCpu.GetInstances();
            foreach (ManagementObject myObject in myCpuCollection)
            {
                strCpu = myObject.Properties["Processorid"].Value.ToString();
            }
            return strCpu;
        }

        ///<summary>
        /// 生成机器码
        ///</summary>
        ///<returns></returns>
        public string GetMNum()
        {
            string strMNum = GetCpu().PadLeft(8, '0').Substring(0, 8);   //cpu序列号
            strMNum += GetDiskVolumeSerialNumber().PadLeft(8, '0').Substring(0, 8); //硬盘序列号
            string strHostName = GetHostName();
            strMNum += strHostName.Substring(strHostName.Length - 8, 8);    //计算机名称（MD5加密后）
            return strMNum;
        }
        /// <summary>
        /// 获取计算机名称
        /// </summary>
        /// <returns></returns>
        public string GetHostName()
        {
            string strName = System.Net.Dns.GetHostName();
            string key = "LY9999ZD";

            strName = MD5Encrypt(strName, key);

            return strName;
        }

        public string MD5Encrypt(string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }


        /// <summary>
        /// 生成注册码
        /// </summary>
        /// <param name="strMNum"></param>
        /// <returns></returns>
        public string MD5Decrypt(string strMNum)
        {
            string key = "LYZD9999";
            string strRNum = MD5Encrypt(strMNum, key);
            if (strRNum.Length > 24)
            {
                strRNum = strRNum.Substring(strRNum.Length - 24, 24);
            }
            return strRNum;
        }
        /// <summary>
        /// 生成注册码
        /// </summary>
        /// <param name="strMNum"></param>
        /// <returns></returns>
        public string MD5Decrypt()
        {
            string strMNum = GetMNum();
            string key = "LYZD9999";
            string strRNum = MD5Encrypt(strMNum, key);
            if (strRNum.Length > 24)
            {
                strRNum = strRNum.Substring(strRNum.Length - 24, 24);
            }
            return strRNum;
        }
    }
}
