using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.AAPrepareTest
{
    /// <summary>
    /// 终端IP设置
    /// </summary>
    public class SetTerminalIP : VerifyBase
    {
        public override void Verify()
        {
            //切换通讯方式
            bool IsSetConnType = false;
            Cus_EmChannelType ConnType = TerminalChannelType;
            string path = System.IO.Directory.GetCurrentDirectory() + "\\Ini\\TerminalIPData.ini";


            if (TerminalChannelType == Cus_EmChannelType.ChannelEther || TerminalChannelType == Cus_EmChannelType.ChannelGPRS)
            {
                IsSetConnType = true;
            }
            if (IsSetConnType)
            {
                //if (Core.Function.File.ReadInIString(path, "Data", "SetModel", "")=="4852")
                //{
                //    TerminalChannelType = Cus_EmChannelType.ChannelMaintain;
                //}
                //else
                //{
                //    TerminalChannelType = Cus_EmChannelType.Channel232;
                //}
                TerminalChannelType = Cus_EmChannelType.Channel232;
                Set232ThreadNmber(true);
                InitTerminalTalks();
            }
            base.StartVerify698();

            //这里是否可以考虑设置成232通讯，设置完成后在切换回来
            string cTaskData = "";



            MessageAdd("正在关闭安全模式", EnumLogType.提示信息, true);
            for (int i = 0; i < meterInfo.Length; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    cTaskData = "06 01 01 f1 01 02 00 16 00 00";
                    Talkers[i].Framer698.sAPDU = cTaskData.Replace(" ", "");
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }
            // 68 19 00 43 05 01 00 00 00 00 00 10 fb bf 06 01 01 f1 01 02 00 16 00 00 25 0d 16
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

   

          

            ///是否保存IP地址，用于后面恢复
            if (Core.Function.File.ReadInIString(path, "Data", "SavePort", "").ToLower() == "true")
            {
                string[] MasterStationIP = new string[MeterNumber];//主站IP
                string[] MasterStationPort = new string[MeterNumber];//主站端口

                string[] TerminalIP = new string[MeterNumber];//终端IP
                                                              //string[] TerminalPort = new string[MeterNumber];//终端端口

                //这里需要读取目前终端的主站IP和端口保存起来
                MessageAdd("正在获取主站ip和端口", EnumLogType.提示信息, true);
                for (int i = 0; i < meterInfo.Length; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        cTaskData = "05 01 01 45 10 03 00 00";
                        Talkers[i].Framer698.sAPDU = cTaskData.Replace(" ", "");
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < meterInfo.Length; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            string str = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                            //MasterStationIP[i]= new IPAddress(long.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier )).ToString();
                            //这里解析数据--保存到ini中
                            if (str.Length == 8)
                            {
                                for (int j = 0; j < str.Length; j += 2)
                                {
                                    MasterStationIP[i] += Convert.ToInt32(str.Substring(j, 2), 16) + ".";
                                }
                                MasterStationIP[i] = MasterStationIP[i].TrimEnd('.');
                            }
                            MasterStationPort[i] = GetData(RecData, i, 6, EnumTerimalDataType.e_string);
                        }
                    }
                }


                MessageAdd("正在获取终端IP", EnumLogType.提示信息, true);
                for (int i = 0; i < meterInfo.Length; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        cTaskData = "05 01 01 45 10 04 00 00";
                        Talkers[i].Framer698.sAPDU = cTaskData.Replace(" ", "");
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < meterInfo.Length; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            string str = GetData(RecData, i, 6, EnumTerimalDataType.e_string);
                            if (str.Length == 8)
                            {
                                for (int j = 0; j < str.Length; j += 2)
                                {
                                    TerminalIP[i] += Convert.ToInt32(str.Substring(j, 2), 16) + ".";
                                }
                                TerminalIP[i] = TerminalIP[i].TrimEnd('.');
                            }
                            //TerminalIP[i] = new IPAddress(long.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier )).ToString();
                            //这里解析数据--保存到ini中
                        }
                    }
                }
                // 保存主站ip和端口--终端ip
                //TODO 这里应该考虑终端多次读取的情况,有可能回出现多次读取设置ip，导致二次读取的是设置后的ip。应该加个判断
                for (int i = 0; i < meterInfo.Length; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        string str = MasterStationIP[i] + "|" + MasterStationPort[i] + "#" + TerminalIP[i];
                        Core.Function.File.WriteInIString(path, "Data", (i + 1).ToString(), str);
                    }
                }
            }


            string IP = VerifyConfig.Tcp_Ip;
            string Port = VerifyConfig.Tcp_Port;

            string[] data = IP.Split('.');
            if (data.Length < 4)
            {
                MessageAdd("主站IP不正确", EnumLogType.错误信息);
                TryStopTest();
                return;
            }

            string SetIP = "";
            for (int i = 0; i < 4; i++)
            {
                SetIP += Convert.ToString(int.Parse(data[i]), 16) + " ";
            }
            string SetPort = Convert.ToString(int.Parse(Port), 16);

            cTaskData = "";

            MessageAdd("正在设置主站IP和端口", EnumLogType.提示信息, true);
            for (int i = 0; i < meterInfo.Length; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    SetPort = Convert.ToString(int.Parse(Port) + i, 16);
                    cTaskData = "06 01 01 45 10 03 00 01 02 02 02 09 04 " + SetIP + "12" + SetPort + "02 02 09 04 " + SetIP + "12" + SetPort + "00";
                    Talkers[i].Framer698.sAPDU = cTaskData.Replace(" ", "");
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < meterInfo.Length; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "00";
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                        if (GetData(RecData, i, 4, EnumTerimalDataType.e_string) != "00")
                            TempData[i].Tips = "错误" + TempData[i].Data;
                        //TempData[i].Resoult = "不合格";
                    }
                    else
                        TempData[i].Resoult = "不合格";
                }
            }
            AddItemsResoult("主站IP地址和端口", TempData);


            MessageAdd("正在设置终端IP和端口", EnumLogType.提示信息, true);

            SetIP = "";
            for (int i = 0; i < 3; i++)
            {
                SetIP += Convert.ToString(int.Parse(data[i]), 16);
            }

            for (int i = 0; i < meterInfo.Length; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    cTaskData = "06 01 01 45 10 04 00 02 06 16 01 09 04 " + SetIP + Convert.ToString(101 + i, 16) + "09 04 ff ff ff 00 09 04 c0 a8 72 01 0a 00 0a 00 00";
                    Talkers[i].Framer698.sAPDU = cTaskData.Replace(" ", "");
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < meterInfo.Length; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "00";
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                        if (GetData(RecData, i, 4, EnumTerimalDataType.e_string) != "00")
                            TempData[i].Tips = "错误" + TempData[i].Data;

                        //TempData[i].Resoult = "不合格";
                    }
                    else
                        TempData[i].Resoult = "不合格";
                }
            }

            AddItemsResoult("终端IP地址和端口", TempData);


            ///恢复成原来的通讯方式
            if (IsSetConnType)
            {
                TerminalChannelType = ConnType;
                Set232ThreadNmber(false);
                InitTerminalTalks();
                WaitTime("等待终端上线", 90);
            }
            return;
        }
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "主站IP地址和端口", "终端IP地址和端口", "结论" };
            return true;
        }
    }
}


#region MyRegion

//MessageAdd("正在设置主站IP和端口",EnumLogType.提示信息);
//for (int j = 0; j < MeterNumber; j++)
//{
//    if (YaoJian[j])
//    {
//        meterInfo[j].YaoJianYn = true;
//        SetConntype(GetYaoJian(), 1);
//        for (int i = 0; i < meterInfo.Length; i++)
//        {
//            if (meterInfo[i].YaoJianYn)
//            {
//                SetPort = Convert.ToString(int.Parse(Port) + i, 16);
//                cTaskData = "06 01 01 45 10 03 00 01 02 02 02 09 04 " + SetIP + "12" + SetPort + "02 02 09 04 " + SetIP + "12" + SetPort + "00";

//                Talkers[i].Framer698.sAPDU = cTaskData.Replace(" ", "");

//                setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
//            }
//        }
//        TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
//        for (int i = 0; i < meterInfo.Length; i++)
//        {
//            if (meterInfo[i].YaoJianYn)
//            {
//                TempData[i].StdData = "00";
//                if (TalkResult[i] == 0)
//                {
//                    TempData[i].Data = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
//                    if (GetData(RecData, i, 4, EnumTerimalDataType.e_string) != "00")
//                        TempData[i].Tips = "错误" + TempData[i].Data;
//                    //TempData[i].Resoult = "不合格";
//                }
//                else
//                    TempData[i].Resoult = "不合格";
//            }
//        }
//        SetConntype(GetYaoJian(), 0);
//        meterInfo[j].YaoJianYn = false;
//    }
//}
//AddItemsResoult("主站IP地址和端口", TempData);

////string cTaskData = "06 01 01 45 10 03 00 01 02 02 02 09 04 " + SetIP + "12" + SetPort +"02 02 09 04 " + SetIP + "12" + SetPort+"00";
//MessageAdd("正在设置终端IP和端口",EnumLogType.提示信息);

//SetIP = "";
//for (int i = 0; i < 3; i++)
//{
//    SetIP += Convert.ToString(int.Parse(data[i]), 16);
//}

//for (int j = 0; j < MeterNumber; j++)
//{
//    if (YaoJian[j])
//    {
//        meterInfo[j].YaoJianYn = true;
//        SetConntype(GetYaoJian(), 1);
//        for (int i = 0; i < meterInfo.Length; i++)
//        {
//            if (meterInfo[i].YaoJianYn)
//            {
//                cTaskData = "06 01 01 45 10 04 00 02 06 16 01 09 04 " + SetIP + Convert.ToString(101 + i, 16) + "09 04 ff ff ff 00 09 04 c0 a8 72 01 0a 00 0a 00 00";
//                Talkers[i].Framer698.sAPDU = cTaskData.Replace(" ", "");
//                setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
//            }
//        }
//        TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
//        setData[j] = null;
//        for (int i = 0; i < meterInfo.Length; i++)
//        {
//            if (meterInfo[i].YaoJianYn)
//            {
//                TempData[i].StdData = "00";
//                if (TalkResult[i] == 0)
//                {
//                    TempData[i].Data = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
//                    if (GetData(RecData, i, 4, EnumTerimalDataType.e_string) != "00")
//                        TempData[i].Tips = "错误" + TempData[i].Data;

//                    //TempData[i].Resoult = "不合格";
//                }
//                else
//                    TempData[i].Resoult = "不合格";
//            }
//        }

//        SetConntype(GetYaoJian(), 0);
//        meterInfo[j].YaoJianYn = false;
//    }
//}
//AddItemsResoult("终端IP地址和端口", TempData);


#endregion