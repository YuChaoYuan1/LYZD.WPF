using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using ZH.Struct;
using ZH;
using ZH.SocketModule.Packet;
using E_ZH311;
using System.Text;

namespace ZH
{
    #region 接口部分


    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
     ComVisible(true)]
    public interface IClass_Interface
    {
        /// <summary>
        /// 初始化设备通讯参数UDP
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="MaxWaitTme">最长等待时间</param>
        /// <param name="WaitSencondsPerByte">帧字节间隔时间</param>
        /// <param name="IP">Ip地址</param>
        /// <param name="RemotePort">远程端口</param>
        /// <param name="LocalStartPort">本地端口</param>
        /// <returns>是否注册成功</returns>
        [DispId(1)]
        int InitSetting(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte, string IP, int RemotePort, int LocalStartPort);
        /// <summary>
        /// 注册Com 口
        /// </summary>
        /// <param name="ComNumber"></param>
        /// <param name="strSetting"></param>
        /// <param name="maxWaittime"></param>
        /// <returns></returns>
        [DispId(2)]
        int InitSettingCom(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte);
        /// <summary>
        /// 连机
        /// </summary>
        /// <returns></returns>
        [DispId(3)]
        int Connect(out string[] FrameAry);
        /// <summary>
        /// 断开连机
        /// </summary>
        /// <returns></returns>
        [DispId(4)]
        int DisConnect(out string[] FrameAry);
      
        /// <summary>
        /// 读取瞬时测量数据
        /// </summary>
        /// <param name="instValue"></param>
        /// <returns></returns>
        [DispId(5)]
        int ReadInstMetricAll(out float[] instValue, out string[] FrameAry);

        /// <summary>
        /// 读取累积电量
        /// </summary>
        /// <param name="flaEnergy"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        [DispId(6)]
        int readEnergy(out float[] flaEnergy, out string[] FrameAry);

        /// <summary>
        /// 1004H-谐波含量
        /// </summary>
        /// <param name="flaHarmonic"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        [DispId(7)]
        int readHarmonicEnergy(out float[] flaHarmonic, out string[] FrameAry);

        /// <summary>
        /// 1005H-谐波相角
        /// </summary>
        /// <param name="flaHarmonicAngle"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
       [DispId(8)]
        int readHarmonicAngle(out float[] flaHarmonicAngle, out string[] FrameAry);

        /// <summary>
        /// 1006H-谐波有功功率
        /// </summary>
        /// <param name="flaHarmonicActivePower"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        [DispId(9)]
        int readHarmonicActivePower(out float[] flaHarmonicActivePower, out string[] FrameAry);

        /// <summary>
        /// 1007H-谐波有功电能
        /// </summary>
        /// <param name="flaHarmonicActiveEnergy"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        [DispId(10)]
        int readHarmonicActiveEnergy(out float[] flaHarmonicActiveEnergy, out string[] FrameAry);

        /// <summary>
        /// 1008H- 档位常数 读取与设置
        /// </summary>
        /// <param name="stdCmd">0x10 读 ，0x13写</param>
        /// <param name="stdUIGear"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        [DispId(11)]
        int stdGear(byte stdCmd, ref float[] stdUIGear, out string[] FrameAry);

        /// <summary>
        /// 1009H-电压电流标准值
        /// </summary>
        /// <param name="Ua"></param>
        /// <param name="Ub"></param>
        /// <param name="Uc"></param>
        /// <param name="Ia"></param>
        /// <param name="Ib"></param>
        /// <param name="Ic"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        [DispId(12)]
        int CalibrationPowerAmplitude(double Ua, double Ub, double Uc, double Ia, double Ib, double Ic, out string[] FrameAry);

        /// <summary>
        /// 100aH-相位标准值 下发
        /// </summary>
        /// <param name="PhiUa">A相电压角度</param>
        /// <param name="PhiUb">B相电压角度</param>
        /// <param name="PhiUc">C相电压角度</param>
        /// <param name="PhiIa">A相电流角度</param>
        /// <param name="PhiIb">B相电流角度</param>
        /// <param name="PhiIc">C相电流角度</param>
        /// <returns></returns>
        [DispId(13)]
        int CalibrationPowerAngle(double PhiUa, double PhiUb, double PhiUc, double PhiIa, double PhiIb, double PhiIc, out string[] FrameAry);

        /// <summary>
        ///  100bH-模式设置
        /// </summary>
        /// <param name="stdCmd"></param>
        /// <param name="strModeJxfs"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        [DispId(14)]
        int stdMode(byte stdCmd, ref string[] strModeJxfs, out string[] FrameAry);

        /// <summary>
        /// 100cH-启停标准表累积电能
        /// </summary>
        /// <param name="startOrStopStd"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        [DispId(15)]
        int startStdEnergy(int startOrStopStd, out string[] FrameAry);
        /// <summary>
        /// 设置获取请求报文标志
        /// </summary>
        /// <param name="Flag">True:发送报文,并传出报文,false:不发送,只传出报文</param>
        /// <returns></returns>
        [DispId(16)]
        int SetSendFlag(bool Flag);
        /// <summary>
        /// 解析下行报文
        /// </summary>
        /// <param name="MothedName"></param>
        /// <param name="ReFrameAry"></param>
        /// <param name="ReAry"></param>
        /// <returns></returns>
        [DispId(17)]
        int UnPacket(string MothedName,byte[]ReFrameAry, out string[] ReAry);

    }
    [Guid("5BFBD907-D63A-4c0f-B51A-0AD571100176"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
    ComVisible(true)]
    public interface IClass_Events
    {
        [DispId(80)]
        void MsgCallBack(string szMessage);
    }
    //{19F31930-CD26-433E-9E74-E8935CEFD2A2}
    [Guid("19F31930-CD26-433E-9E74-E8935CEFD2A2"),
    ProgId("ZH.ZH311"),
    ClassInterface(ClassInterfaceType.None),
    ComDefaultInterface(typeof(IClass_Interface)),
    ComSourceInterfaces(typeof(IClass_Events)),
    ComVisible(true)]


    #endregion

    public class ZH311 : IClass_Interface
    {
        public delegate void MsgCallBackDelegate(string szMessage);
        public event MsgCallBackDelegate MsgCallBack;

        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 1;
        /// <summary>
        /// 源控制端口
        /// </summary>
        private StPortInfo m_PowerSourcePort = null;

        private DriverBase driverBase = null;

        //是否发送数据标志
        private bool sendFlag = true;
        /// <summary>
        /// 构造方法
        /// </summary>
        public ZH311()
        {
            m_PowerSourcePort = new StPortInfo();
            driverBase = new DriverBase();
        }

        #region IClass_Interface 成员
        /// <summary>
        /// 初始化UDP端口
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="MaxWaitTime">帧命令最长等待时间</param>
        /// <param name="WaitSencondsPerByte">帧字节等待时间</param>
        /// <param name="IP">Ip地址</param>
        /// <param name="RemotePort">远程端口</param>
        /// <param name="LocalStartPort">本地端口</param>
        /// <returns></returns>
        public int InitSetting(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte, string IP, int RemotePort, int LocalStartPort)
        {
            m_PowerSourcePort.m_Exist = 1;
            m_PowerSourcePort.m_IP = IP;
            m_PowerSourcePort.m_Port = ComNumber;
            m_PowerSourcePort.m_Port_IsUDPorCom = true;
            m_PowerSourcePort.m_Port_Setting = "38400,n,8,1";
            try
            {
                driverBase.RegisterPort(ComNumber, m_PowerSourcePort.m_Port_Setting, m_PowerSourcePort.m_IP, RemotePort, LocalStartPort, MaxWaitTime, WaitSencondsPerByte);
            }
            catch (Exception)
            {
                return 1;
            }

            return 0;
        }
        /// <summary>
        /// 初始化Com 口
        /// </summary>
        /// <param name="ComNumber"></param>
        /// <param name="MaxWaitTime"></param>
        /// <param name="WaitSencondsPerByte"></param>
        /// <returns></returns>
        public int InitSettingCom(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte)
        {
            m_PowerSourcePort.m_Exist = 1;
            m_PowerSourcePort.m_IP = "";
            m_PowerSourcePort.m_Port = ComNumber;
            m_PowerSourcePort.m_Port_IsUDPorCom = false;
            m_PowerSourcePort.m_Port_Setting = "38400,n,8,1";
            try
            {
                driverBase.RegisterPort(ComNumber, "38400,n,8,1", MaxWaitTime, WaitSencondsPerByte);
            }
            catch (Exception)
            {                
                return 1;
            }            
            return 0;
        }
        /// <summary>
        /// 连机
        /// </summary>
        /// <returns></returns>
        public int Connect(out string[] FrameAry)
        {
            FrameAry = new string[1];
            ZH311_RequestLinkPacket rc3 = new ZH311_RequestLinkPacket();
            ZH311_RequestLinkReplyPacket recv3 = new ZH311_RequestLinkReplyPacket();
            //合成的报文
            try
            {
                FrameAry[0] = BytesToString(rc3.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_PowerSourcePort, rc3, recv3))
                    {
                        bool linkClockOk = recv3.ReciveResult == RecvResult.OK;
                        string Clockmessage = string.Format("表联机{0}。", linkClockOk ? "成功" : "失败");
                        return linkClockOk ? 0 : 1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {

                    return 0;
                }
            }
            catch (Exception)
            {
                
                return -1;
            }

        }
        /// <summary>
        /// 脱机
        /// </summary>
        /// <returns></returns>
        public int DisConnect(out string[] FrameAry)
        {
            FrameAry = new string[1];
            return 0;
        }


        /// <summary>
        /// 读取累积电量
        /// </summary>
        /// <param name="flaEnergy"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public  int readEnergy(out float[] flaEnergy, out string[] FrameAry)
        {
            flaEnergy = new float[12];
            FrameAry = new string[1];
            byte[] strRev = new byte[0];
            Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            strDictionary.Add("1003", "");           
            try
            {
                return sendData(0x10, strDictionary, out strRev, out FrameAry);
            }
            catch (Exception)
            {

                return -1;
            }
        }
        /// <summary>
        /// 1004H-谐波含量
        /// </summary>
        /// <param name="flaHarmonic"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int readHarmonicEnergy(out float[] flaHarmonic, out string[] FrameAry)
        {
            flaHarmonic = new float[12];
            FrameAry = new string[1];
            byte[] strRev = new byte[0];
            Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            strDictionary.Add("1004", "");
            try
            {
                int r = sendData(0x10, strDictionary, out strRev, out FrameAry);
                try
                {
                    byte[] byteDa = new byte[10];
                    float[] instValue = new float[192];
                    for (int i = 0; i < instValue.Length; i++)
                    {
                        Array.Copy(strRev, 3 + i * 10, byteDa, 0, 10);
                        instValue[i] = Convert.ToSingle(System.Text.Encoding.ASCII.GetString(byteDa));
                    }
                    flaHarmonic = instValue;

                }
                catch (Exception)
                {
                }
                return r;
            }
            catch (Exception)
            {

                return -1;
            }
        }
        /// <summary>
        /// 1005H-谐波相角
        /// </summary>
        /// <param name="flaHarmonicAngle"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int readHarmonicAngle(out float[] flaHarmonicAngle, out string[] FrameAry)
        {
            flaHarmonicAngle = new float[12];
            FrameAry = new string[1];
            byte[] strRev = new byte[0];
            Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            strDictionary.Add("1005", "");
            try
            {
                int r = sendData(0x10, strDictionary, out strRev, out FrameAry);
                try
                {
                    byte[] byteDa = new byte[10];
                    float[] instValue = new float[192];
                    for (int i = 0; i < instValue.Length; i++)
                    {
                        Array.Copy(strRev, 3 + i * 10, byteDa, 0, 10);
                        instValue[i] = Convert.ToSingle(System.Text.Encoding.ASCII.GetString(byteDa));
                    }
                    flaHarmonicAngle = instValue;

                }
                catch (Exception)
                {
                }
                return r;
            }
            catch (Exception)
            {

                return -1;
            }
        }
        /// <summary>
        /// 1006H-谐波有功功率
        /// </summary>
        /// <param name="flaHarmonicActivePower"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int readHarmonicActivePower(out float[] flaHarmonicActivePower, out string[] FrameAry)
        {
            flaHarmonicActivePower = new float[12];
            FrameAry = new string[1];
            byte[] strRev = new byte[0];
            Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            strDictionary.Add("1006", "");
            try
            {
                return sendData(0x10, strDictionary, out strRev, out FrameAry);
            }
            catch (Exception)
            {

                return -1;
            }
        }
        /// <summary>
        /// 1007H-谐波有功电能
        /// </summary>
        /// <param name="flaHarmonicActiveEnergy"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int readHarmonicActiveEnergy(out float[] flaHarmonicActiveEnergy, out string[] FrameAry)
        {
            flaHarmonicActiveEnergy = new float[12];
            FrameAry = new string[1];
            byte[] strRev = new byte[0];
            Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            strDictionary.Add("1007", "");
            try
            {
                return sendData(0x10, strDictionary, out strRev, out FrameAry);
            }
            catch (Exception)
            {

                return -1;
            }
        }

        /// <summary>
        /// 1008H- 档位常数 读取与设置
        /// </summary>
        /// <param name="stdCmd">0x10 读 ，0x13写</param>
        /// <param name="stdUIGear"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int stdGear( byte stdCmd, ref float[] stdUIGear, out string[] FrameAry)
        {
            //stdUIGear = new float[7];
            FrameAry = new string[1];
            byte[] strRev = new byte[0];
            Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            strDictionary.Add("1008", "");
            if (stdCmd == 0x10)
            {                
                strDictionary.Add("1008", "");
            }
            try
            {
                return sendData(stdCmd, strDictionary, out strRev, out FrameAry);
            }
            catch (Exception)
            {

                return -1;
            }
        }


        #region  设置被检表参数

        /// <summary>
        /// 1008H- 档位常数 读取与设置
        /// </summary>
        /// <param name="stdCmd">0x10 读 ，0x13写</param>
        /// <param name="stdUIGear"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int stdGear2(byte stdCmd,ref long stdConst, ref double[] stdUIGear, out string[] FrameAry)
        {
            //stdUIGear = new float[7];
            FrameAry = new string[1];
            ASCIIEncoding charToASCII = new ASCIIEncoding();
            byte[] strRev = new byte[0];
            Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            strDictionary.Add("1008", "");
            //if (stdCmd == 0x10)
            //{                
            //    strDictionary.Add("1008", "");
            //}


            try
            {
                byte[] strSendAll = new byte[26];
                strSendAll[0] = 0x10;
                strSendAll[1] = 0x08;
                char[] charBuf = new char[2];
                byte[] strSend = new byte[2];
                char[] charBuf2 = new char[10];
                byte[] strSend2 = new byte[10];

                charBuf = stdUIGear[0].ToString().ToCharArray();
                int lenght = 2;
                if (charToASCII.GetBytes(charBuf).Length < 2)
                { lenght = charToASCII.GetBytes(charBuf).Length; }
                Array.Copy(charToASCII.GetBytes(charBuf), 0, strSend, 0, lenght);
                strSend.CopyTo(strSendAll, 2);

                charBuf = stdUIGear[1].ToString().ToCharArray();
                lenght = 2;
                if (charToASCII.GetBytes(charBuf).Length < 2)
                { lenght = charToASCII.GetBytes(charBuf).Length; }
                Array.Copy(charToASCII.GetBytes(charBuf), 0, strSend, 0, lenght);
                strSend.CopyTo(strSendAll, 4);

                charBuf = stdUIGear[2].ToString().ToCharArray();
                lenght = 2;
                if (charToASCII.GetBytes(charBuf).Length < 2)
                { lenght = charToASCII.GetBytes(charBuf).Length; }
                Array.Copy(charToASCII.GetBytes(charBuf), 0, strSend, 0, lenght);
                strSend.CopyTo(strSendAll, 6);

                charBuf = stdUIGear[3].ToString().ToCharArray();
                lenght = 2;
                if (charToASCII.GetBytes(charBuf).Length < 2)
                { lenght = charToASCII.GetBytes(charBuf).Length; }
                Array.Copy(charToASCII.GetBytes(charBuf), 0, strSend, 0, lenght);
                strSend.CopyTo(strSendAll, 8);

                charBuf = stdUIGear[4].ToString().ToCharArray();
                lenght = 2;
                if (charToASCII.GetBytes(charBuf).Length < 2)
                { lenght = charToASCII.GetBytes(charBuf).Length; }
                Array.Copy(charToASCII.GetBytes(charBuf), 0, strSend, 0, lenght);
                strSend.CopyTo(strSendAll, 10);

                charBuf = stdUIGear[5].ToString().ToCharArray();
                lenght = 2;
                if (charToASCII.GetBytes(charBuf).Length < 2)
                { lenght = charToASCII.GetBytes(charBuf).Length; }
                Array.Copy(charToASCII.GetBytes(charBuf), 0, strSend, 0, lenght);
                strSend.CopyTo(strSendAll, 12);

                charBuf = stdConst.ToString().ToCharArray();
                lenght = 12;
                if (charToASCII.GetBytes(charBuf).Length <= 10)
                { lenght = charToASCII.GetBytes(charBuf).Length; }

                //byte[] s= charToASCII.GetBytes(charBuf);
                Array.Copy(charToASCII.GetBytes(charBuf), 0, strSend2, 0, lenght);
                strSend2.CopyTo(strSendAll, 14);

               // return sendData(stdCmd, strDictionary, out strRev, out FrameAry);
                if (sendData( stdCmd, strSendAll, out strRev, out FrameAry) == 0)
                {
                    if (strRev.Length > 26)
                    {
                        byte[] b = new byte[12];
                        int index = 0;
                        for (int i = 15; i < strRev.Length; i++)
                        {
                            b[index] = strRev[i];
                      
                            index++;
                        }
                        string s = Encoding.ASCII.GetString(b);
                        double v;
                        double.TryParse(s, out v);
                        stdConst = (long)v;
                        //long .TryParse(s, out stdConst);
                        //double v;
                        //double.TryParse(s, out v );
                        //if (v>2000000000)
                        //{
                        //    stdConst = 2000000000;
                        //}
                        //else
                        //{ 
                        //   //int.TryParse(s,out stdConst);
                        //    stdConst = (int)v;
                        //}
                    }

                    return 0;
                }
                else
                {
                    return 1;
                }
    


            }
            catch (Exception ex)
            {

                return -1;
            }
        }
        #endregion

        /// <summary>
        /// 1009H-电压电流标准值
        /// </summary>
        /// <param name="Ua"></param>
        /// <param name="Ub"></param>
        /// <param name="Uc"></param>
        /// <param name="Ia"></param>
        /// <param name="Ib"></param>
        /// <param name="Ic"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int CalibrationPowerAmplitude(double Ua, double Ub, double Uc, double Ia, double Ib, double Ic, out string[] FrameAry)
        {
            FrameAry = new string[1];

            
             
            byte[] strRev = new byte[0];
            Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            strDictionary.Add("1009", "");            
            try
            {
                return sendData(0x13, strDictionary, out strRev, out FrameAry);
            }
            catch (Exception)
            {

                return -1;
            }
        }
        /// <summary>
        /// 100aH-相位标准值 下发
        /// </summary>
        /// <param name="PhiUa">A相电压角度</param>
        /// <param name="PhiUb">B相电压角度</param>
        /// <param name="PhiUc">C相电压角度</param>
        /// <param name="PhiIa">A相电流角度</param>
        /// <param name="PhiIb">B相电流角度</param>
        /// <param name="PhiIc">C相电流角度</param>
        /// <returns></returns>
        public int CalibrationPowerAngle(double PhiUa, double PhiUb, double PhiUc, double PhiIa, double PhiIb, double PhiIc, out string[] FrameAry)
        {
            FrameAry = new string[1];

            
            FrameAry = new string[1];
            byte[] strRev = new byte[0];
            Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            strDictionary.Add("100A", "");           
            try
            {
                return sendData(0x10, strDictionary, out strRev, out FrameAry);
            }
            catch (Exception)
            {

                return -1;
            }
        }
        /// <summary>
        /// 读取瞬时测量数据
        /// </summary>
        /// <param name="instValue"></param>
        /// <returns></returns>
        public int ReadInstMetricAll(out float[] instValue, out string[] FrameAry)
        {
            instValue = new float[34];
            FrameAry = new string[1];
            byte[] strRev = new byte[0];
            Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            strDictionary.Add("1000", "");
            strDictionary.Add("1001", "");
            strDictionary.Add("1002", "");
            try
            {
                return sendData(0x10, strDictionary, out strRev, out FrameAry);
            }
            catch (Exception)
            {

                return -1;
            }

        }

        /// <summary>
        ///  100bH-模式设置
        /// </summary>
        /// <param name="stdCmd"></param>
        /// <param name="strModeJxfs"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int stdMode(byte stdCmd, ref string[] strModeJxfs, out string[] FrameAry)
        {
             
            FrameAry = new string[1];
            byte[] strRev = new byte[0];
            Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            strDictionary.Add("100B", "");
            if (stdCmd == 0x10)
            {
                strDictionary.Add("1008", "");
            }
            try
            {
                return sendData(stdCmd, strDictionary, out strRev, out FrameAry);
            }
            catch (Exception)
            {

                return -1;
            }
        }
        /// <summary>
        /// 100cH-启停标准表累积电能
        /// </summary>
        /// <param name="startOrStopStd">字符’1’表示清零当前电能并开始累计（ascii 码读取）</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int startStdEnergy(int startOrStopStd ,out string[] FrameAry)
        {
            FrameAry = new string[1];
            byte[] strRev = new byte[0];
            Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            strDictionary.Add("100C", startOrStopStd.ToString());
            
            try
            {
                return sendData(0x13, strDictionary, out strRev, out FrameAry);
            }
            catch (Exception)
            {

                return -1;
            }
        }


        /// <summary>
        /// 设置挡位
        /// </summary>
        /// <param name="UA"></param>
        /// <param name="UB"></param>
        /// <param name="UC"></param>
        /// <param name="IA"></param>
        /// <param name="IB"></param>
        /// <param name="IC"></param>
        /// <returns></returns>
        public bool setMeterDWZH311(float UA, float UB, float UC, float IA, float IB, float IC)
        {

            //ZH3130_RequestSendDataReplyPacket rcPowerHarmonic = new ZH3130_RequestSendDataReplyPacket();

            //float UMax = Math.Max(Math.Max(UA, UB), UC);
            //int UValue = rcPowerHarmonic.GetDYString(UMax);
            //byte[] bydata = Encoding.ASCII.GetBytes((UValue).ToString());

            //float IMax = Math.Max(Math.Max(IA, IB), IC);
            //int IValue = rcPowerHarmonic.GetDLString(IMax);
            //byte[] bydata1 = Encoding.ASCII.GetBytes((IValue).ToString());

            //string[] FrameAry = new string[1];
            //byte[] strRev = new byte[0];
            //float[] stdUIGear = new float[0];
            ////Dictionary<string, string> strDictionary = new Dictionary<string, string>();

            //byte[] byteDate = new byte[26];
            //byteDate[0] = 0x10;
            //byteDate[1] = 0x08;

            //byteDate[2] = 0x30;
            //byteDate[3] = bydata[0];
            //if (bydata1.Length >= 2)
            //{
            //    byteDate[4] = bydata1[0];
            //    byteDate[5] = bydata1[1];
            //}
            //else
            //{
            //    byteDate[4] = 0x00;
            //    byteDate[5] = bydata1[0];
            //}

            //byteDate[6] = 0x30;
            //byteDate[7] = bydata[0];


            //if (bydata1.Length >= 2)
            //{
            //    byteDate[8] = bydata1[0];
            //    byteDate[9] = bydata1[1];
            //}
            //else
            //{
            //    byteDate[8] = 0x00;
            //    byteDate[9] = bydata1[0];
            //}


            //byteDate[10] = 0x30;
            //byteDate[11] = bydata[0];

            //if (bydata1.Length >= 2)
            //{
            //    byteDate[12] = bydata1[0];
            //    byteDate[13] = bydata1[1];
            //}
            //else
            //{
            //    byteDate[12] = 0x00;
            //    byteDate[13] = bydata1[0];
            //}



            //byteDate[14] = 0x30;
            //byteDate[15] = 0x30;
            //byteDate[16] = 0x30;
            //byteDate[17] = 0x30;
            //byteDate[18] = 0x30;
            //byteDate[19] = 0x30;
            //byteDate[20] = 0x30;
            //byteDate[21] = 0x30;
            //byteDate[22] = 0x30;
            //byteDate[23] = 0x30;
            //byteDate[24] = 0x30;
            //byteDate[25] = 0x30;

            //sendData(0x13, byteDate, out strRev, out FrameAry);

            return true;


        }
        /// <summary>
        /// 读取瞬时测量数据
        /// </summary>
        /// <param name="instValue"></param>
        /// <returns></returns>
        public float[] ReadstdZH311Param()
        {
            
            float[] instValue = new float[29];
            string[] FrameAry = new string[1];
            byte[] strRev = new byte[0];
            Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            strDictionary.Add("1000", "");
            strDictionary.Add("1001", "");
            strDictionary.Add("1002", "");
            sendData(0x10, strDictionary, out strRev, out FrameAry);
            List<byte[]> listbyte = new List<byte[]>();
            byte[] byteDa = new byte[10];
            int intCount = 1;
            if (strRev.Length == 297)
            {
                try
                {
                    for (int i = 0; i < instValue.Length; i++)
                    {
                        if (i == 6 || i == 17) intCount = intCount + 1;
                        Array.Copy(strRev, 1 + 2 * intCount + i * 10, byteDa, 0, 10);
                        instValue[i] = Convert.ToSingle(System.Text.Encoding.ASCII.GetString(byteDa));
                    }
                }
                catch (Exception)
                {
                    for (int i = 0; i < instValue.Length; i++)
                    {
                        instValue[i] =0;
                    }
                }

                //stdZh311.Ua = Convert.ToSingle(instValue[0]);
                //stdZh311.Ub = Convert.ToSingle(instValue[2]);
                //stdZh311.Uc = Convert.ToSingle(instValue[4]);

                //stdZh311.Ia = Convert.ToSingle(instValue[1]);
                //stdZh311.Ib = Convert.ToSingle(instValue[3]);
                //stdZh311.Ic = Convert.ToSingle(instValue[5]);

                //stdZh311.Phi_Ua = Convert.ToSingle(instValue[6]);
                //stdZh311.Phi_Ub = Convert.ToSingle(instValue[8]);
                //stdZh311.Phi_Uc = Convert.ToSingle(instValue[10]);

                //stdZh311.Phi_Ia = Convert.ToSingle(instValue[7]);
                //stdZh311.Phi_Ib = Convert.ToSingle(instValue[9]);
                //stdZh311.Phi_Ic = Convert.ToSingle(instValue[11]);

                //stdZh311.Freq = Convert.ToSingle(instValue[12]);
                //stdZh311.Freq = Convert.ToSingle(instValue[12]);
                //stdZh311.Freq = Convert.ToSingle(instValue[12]);

                //stdZh311.Pa = Convert.ToSingle(instValue[17]);
                //stdZh311.Pb = Convert.ToSingle(instValue[20]);
                //stdZh311.Pc = Convert.ToSingle(instValue[23]);

                //stdZh311.Qa = Convert.ToSingle(instValue[18]);
                //stdZh311.Qb = Convert.ToSingle(instValue[21]);
                //stdZh311.Qc = Convert.ToSingle(instValue[24]);

                //stdZh311.Sa = Convert.ToSingle(instValue[19]);
                //stdZh311.Sb = Convert.ToSingle(instValue[22]);
                //stdZh311.Sc = Convert.ToSingle(instValue[25]);

                //stdZh311.P = Convert.ToSingle(instValue[26]);
                //stdZh311.Q = Convert.ToSingle(instValue[27]);
                //stdZh311.S = Convert.ToSingle(instValue[28]);
            }
          
            return instValue;
        }


        /// <summary>
        ///   设置脉冲
        /// </summary>
        /// <param name="pulseType">
        ///有功脉冲 设置字符’1’
        ///无功脉冲 设置字符’2’
        ///UA脉冲 设置字符’3’
        ///UB脉冲 设置字符’4’
        ///UC脉冲 设置字符’5’
        ///IA脉冲 设置字符’6’
        ///IB脉冲 设置字符’7’
        ///IC脉冲 设置字符’8’
        ///PA脉冲 设置字符’9’
        ///PB脉冲 设置字符’10’
        ///PC脉冲 设置字符’11’
        ///</param>
        /// <returns></returns>
        public int SetPulseType(string pulseType)
        {
            //string strGLFX = "31";
            //if (pulseType==1)
            //{
            //    strGLFX = "32";
            //}
            string[] FrameAry = new string[1];
            byte[] strRev = new byte[0];
            Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            strDictionary.Add("100E", pulseType);
            try
            {
                return sendData(0x13, strDictionary, out strRev, out FrameAry);
            }
            catch (Exception)
            {

                return -1;
            }
        }

        /// <summary>
        /// 脉冲校准误差 字符串形式(10字节)W， 单位 分数值。
        /// 误差板的值 × 1000000 下发，譬如误差板计算的误差0.01525%，则0.01525% ×1000000 = 152.5，
        /// 则下发字符串 ”152.5”。
        /// </summary>
        ///  <param name="Error">误差板的值</param>
        /// <returns></returns>
        public int SetPulseCalibration(string Error)
        {
            string[] FrameAry = new string[1];
            byte[] strRev = new byte[0];
            //Dictionary<string, string> strDictionary = new Dictionary<string, string>();
            //strDictionary.Add("1012", Error);
            try
            {
                ASCIIEncoding charToASCII = new ASCIIEncoding();
                byte[] strSendAll = new byte[12];
                strSendAll[0] = 0x10;
                strSendAll[1] = 0x12;
                char[] charBuf = new char[2];
                byte[] strSend2 = new byte[10];
                charBuf = Error.ToCharArray();
                int lenght = 10;
                if (charToASCII.GetBytes(charBuf).Length < 10)
                { lenght = charToASCII.GetBytes(charBuf).Length; }
                Array.Copy(charToASCII.GetBytes(charBuf), 0, strSend2, 0, lenght);
                strSend2.CopyTo(strSendAll, 2);


                if (sendData(0x13, strSendAll, out strRev, out FrameAry) == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }

                //return sendData(0x13, strDictionary, out strRev, out FrameAry);
            }
            catch (Exception)
            {

                return -1;
            }

        }

        #region 指令


        private int sendData(byte cmd, byte[] strDictionary, out byte[] RevbyteData, out string[] FrameAry)
        {
            FrameAry = new string[1];
            RevbyteData = new byte[1];
            //strDictionary = new Dictionary<string, string>();          
            ZH311_RequestDataPacket rc3 = new ZH311_RequestDataPacket();
            ZH311_RequestDataReplyPacket recv3 = new ZH311_RequestDataReplyPacket();
            rc3.SetPara(cmd, strDictionary);
            FrameAry[0] = BytesToString(rc3.GetPacketData());
            if (SendPacketWithRetry(m_PowerSourcePort, rc3, recv3))
            {
                RevbyteData = recv3.revbyteData;
            }
            else
            {
                return 1;
            }
            return 0;
        }


        /// <summary>
        /// 处理指令类 485端口
        /// </summary>
        /// <param name="cmd">命令码</param>
        /// <param name="strDictionary">数据字典</param>
        /// <param name="RevbyteData"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        private int sendData(int index, byte cmd, byte[] byteSend, out byte[] RevbyteData, out string[] FrameAry)
        {
            FrameAry = new string[1];
            RevbyteData = new byte[1];
            //strDictionary = new Dictionary<string, string>();          
            ZH311_RequestDataPacket rc3 = new ZH311_RequestDataPacket();
            ZH311_RequestDataReplyPacket recv3 = new ZH311_RequestDataReplyPacket();
            rc3.SetPara(cmd, byteSend);
            FrameAry[0] = BytesToString(rc3.GetPacketData());
            //if (SendPacketWithRetry(m_arrRs485Port[index], rc3, recv3))
            //{
            //    RevbyteData = recv3.revbyteData;


            //}
            //else
            //{
            //    return 1;
            //}
           return 0;

        }

        /// <summary>
        /// 处理指令类
        /// </summary>
        /// <param name="cmd">命令码</param>
        /// <param name="strDictionary">数据字典</param>
        /// <param name="RevbyteData"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        private int  sendData(byte  cmd, Dictionary<string, string> strDictionary, out byte[] RevbyteData, out string[] FrameAry)
        {
            FrameAry = new string[1];
            RevbyteData = new byte[1];
            //strDictionary = new Dictionary<string, string>();          
            ZH311_RequestDataPacket rc3 = new ZH311_RequestDataPacket();
            ZH311_RequestDataReplyPacket recv3 = new ZH311_RequestDataReplyPacket();
            rc3.SetPara(cmd, strDictionary);
            FrameAry[0] = BytesToString(rc3.GetPacketData());
            if (SendPacketWithRetry(m_PowerSourcePort, rc3, recv3))
            {
                RevbyteData = recv3.revbyteData;
            }
            else
            {
                return 1;
            }
            return 0;

        }

        /// <summary>
        /// 设置发送标志位
        /// </summary>
        /// <param name="Flag"></param>
        /// <returns></returns>
        public int SetSendFlag(bool Flag)
        {
            sendFlag = Flag;
            return 0;
        }
        /// <summary>
        /// 解析设备返回的的报文
        /// </summary>
        /// <param name="MothedName">方法名称</param>
        /// <param name="ReFrameAry">设备返回报文</param>
        /// <param name="ReAry">解析的数据</param>
        /// <returns></returns>
        public int UnPacket(string MothedName, byte[] ReFrameAry, out string[] ReAry)
        {
            int iRevalue = 0;
            ReAry = new string[1];

            switch (MothedName)
            {
                case "Connect":
                    {
                        try
                        {
                            ZH311_RequestLinkReplyPacket recv = new ZH311_RequestLinkReplyPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                        }
                        catch (Exception)
                        {
                            
                            return -1;
                        }

                    }
                    break;
                case "DisConnect":
                    {
                        ReAry[0] = string.Empty;
                    }
                    break;
                case "ReadGPSTime":
                    {
                       return -1;
                    }
                    break;
                default:
                    {
                        ReAry[0] = "Null this Data";
                    }
                    break;
            }
            return iRevalue;
        }


        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="stPort">端口号</param>
        /// <param name="sp">发送包</param>
        /// <param name="rp">接收包</param>
        /// <returns></returns>
        private bool SendPacketWithRetry(StPortInfo stPort, SendPacket sp, RecvPacket rp)
        {
            for (int i = 0; i < RETRYTIEMS; i++)
            {
                if (driverBase.SendData(stPort, sp, rp) == true)
                {
                    return true;
                }
                Thread.Sleep(300);
            }
            return false;
        }
        /// <summary>
        /// bytes转 string
        /// </summary>
        /// <param name="bytesData"></param>
        /// <returns></returns>
        private string BytesToString(byte[] bytesData)
        {
            string strRevalue =string.Empty;
            if (bytesData == null || bytesData.Length < 1)
                return strRevalue;

            for (int i = 0; i < bytesData.Length; i++)
            {
                strRevalue += Convert.ToString(bytesData[i], 16);
            }
            return strRevalue;
        }


        #endregion

        #endregion

    }
}
