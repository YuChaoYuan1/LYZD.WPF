using LYZD.Core.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZH485;

namespace LYZD.ViewModel.CheckController
{
    public class Rs485Port
    {
        public RS485 rs485port = new RS485();
        public int int_TableNo = 0;

        public Rs485Port(int p_int_TableNo)
        {
            int_TableNo = p_int_TableNo;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="p_int_PortNo"></param>
        /// <param name="p_str_SerialPort"></param>
        /// <param name="p_int_FrameInterval"></param>
        /// <param name="p_int_ByteInterval"></param>
        /// <returns></returns>
        public int InitSettingCom(int p_int_PortNo, string p_str_SerialPort, int p_int_FrameInterval, int p_int_ByteInterval)
        {
            int int_Return = -1;

            int_Return = rs485port.InitSettingCom(p_int_PortNo, p_str_SerialPort, p_int_FrameInterval, p_int_ByteInterval);

            return int_Return;
        }

        public int InitSetting(int p_int_PortNo, string p_str_SerialPort, int p_int_FrameInterval, int p_int_ByteInterval, string p_str_ServerIp, int p_int_RemotePort, int p_int_LocalPort, string p_bol_Protocol, ZH485.Enum.StCanParams sc)
        {
            int int_Return = -1;
            int_Return = rs485port.InitSetting(p_int_PortNo, p_str_SerialPort, p_int_FrameInterval, p_int_ByteInterval, p_str_ServerIp, p_int_RemotePort, p_int_LocalPort, p_bol_Protocol, sc);
            return int_Return;
        }


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="p_byt_SendData"></param>
        /// <param name="p_byt_OutFrame"></param>
        /// <returns></returns>
        public int SendData(byte[] p_byt_SendData, out byte[] p_byt_OutFrame)
        {
            int int_Return = -1;
            int_Return = rs485port.SendData(p_byt_SendData, out p_byt_OutFrame);
            return int_Return;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="p_byt_SendData"></param>
        /// <param name="p_byt_OutFrame"></param>
        /// <returns></returns>
        public int SendData(byte[] p_byt_SendData, out byte[] p_byt_OutFrame, int MaxWaitSeconds)
        {
            int int_Return = -1;
            int_Return = rs485port.SendData(p_byt_SendData, out p_byt_OutFrame, MaxWaitSeconds);
            return int_Return;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="p_byt_SendData"></param>
        /// <param name="p_byt_OutFrame"></param>
        /// <returns></returns>
        public int SendData_(byte[] p_byt_SendData, out byte[] p_byt_OutFrame)
        {
            int int_Return = -1; int iTmp = 0;


            int_Return = rs485port.SendData(p_byt_SendData, out p_byt_OutFrame);
            if (ReturnFlagFrame(p_byt_OutFrame))
                return int_Return;
            else
                int_Return = -1;
            while (iTmp < 5 && int_Return == -1)
            {
                iTmp++;
                int_Return = rs485port.SendData(p_byt_SendData, out p_byt_OutFrame);
                if (ReturnFlagFrame(p_byt_OutFrame))
                    return int_Return;
                else
                    int_Return = -1;
            }

            return int_Return;
        }

        /// <summary>
        /// 返回376.1是否含有有效帧
        /// </summary>
        /// <returns></returns>
        public bool ReturnFlagFrame(byte[] p_byt_OutFrame)
        {
            bool boolfalg = true;
            string strTmp = UsefulMethods.ConvertBytesToString(p_byt_OutFrame).Replace(" ", "");
            int a = strTmp.IndexOf("68");
            try
            {
                if (a > -1)//1.先判断有没有帧头68
                {
                    if (strTmp.Substring(a + 10, 2) == "68")//2.判断第2个68
                    {
                        int ilen = Convert.ToInt16(strTmp.Substring(a + 2, 2), 16) / 4 + Convert.ToInt16(strTmp.Substring(a + 4, 2), 16) * 64;//3.获取帧长

                        if (strTmp.Substring(a + 14 + ilen * 2, 2) == "16")//4.判断帧尾16
                        {
                            if (UsefulMethods.GetChkSum(strTmp.Substring(a + 12, ilen * 2)) != Convert.ToByte(strTmp.Substring(a + 12 + ilen * 2, 2), 16))//5.判断校验和
                                boolfalg = false;
                        }
                        else
                        {
                            boolfalg = false;
                        }
                    }
                    else
                        boolfalg = false;
                }
                else
                    boolfalg = false;
            }
            catch
            {
                boolfalg = false;//未对长度作详细判断，长度不够时，直接判无效帧
            }

            if (!boolfalg)
            {

                UsefulMethods.WriteLog(DateTime.Now.ToString() + " " + strTmp, "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\错误通讯帧")
                    ;
            }

            return boolfalg;
        }
    }
}
