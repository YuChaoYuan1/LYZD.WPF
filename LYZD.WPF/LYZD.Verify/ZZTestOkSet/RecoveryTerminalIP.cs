using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;

namespace LYZD.Verify.ZZTestOkSet
{
    /// <summary>
    /// 恢复主站IP和端口
    /// </summary>
    public class RecoveryTerminalIP : VerifyBase
    {

        public override void Verify()
        {
            //切换通讯方式
            bool IsSetConnType = false;
            Cus_EmChannelType ConnType = TerminalChannelType;
            if (TerminalChannelType == Cus_EmChannelType.ChannelEther || TerminalChannelType == Cus_EmChannelType.ChannelGPRS)
            {
                IsSetConnType = true;
            }
            if (IsSetConnType)
            {
                TerminalChannelType = Cus_EmChannelType.Channel232;
                Set232ThreadNmber(true);
                InitTerminalTalks();
            }
            base.StartVerify698();

            string[] MasterStationIP = new string[MeterNumber];//主站IP
            string[] MasterStationPort = new string[MeterNumber];//主站端口

            string[] TerminalIP = new string[MeterNumber];//终端IP
            //string[] TerminalPort = new string[MeterNumber];//终端端口

            string path = System.IO.Directory.GetCurrentDirectory() + "\\Ini\\TerminalIPData.ini";
            for (int i = 0; i < meterInfo.Length; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    string str = Core.Function.File.ReadInIString(path, "Data", (i + 1).ToString(), "");
                    if (str != "")
                    {
                        string[] MasterStation = str.Split('#');
                        if (MasterStation.Length > 1)
                        {
                            MasterStationIP[i] = MasterStation[0].Split('|')[0];
                            MasterStationPort[i] = MasterStation[0].Split('|')[1];
                            TerminalIP[i] = MasterStation[1].Split('|')[0];
                            //TerminalPort[i] = MasterStation[1].Split('|')[1];
                        }
                    }
                }
            }




            string SetIP = "";
            string cTaskData = "";

            MessageAdd("正在关闭安全模式",EnumLogType.提示信息);
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



            MessageAdd("正在设置主站IP和端口",EnumLogType.提示信息);
            for (int i = 0; i < meterInfo.Length; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    string[] data = MasterStationIP[i].Split('.');
                    string SetPort = Convert.ToString(int.Parse(MasterStationPort[i]), 16);
                    for (int j = 0; j < 4; j++)
                    {
                        SetIP += Convert.ToString(int.Parse(data[j]), 16) + " ";
                    }
                    cTaskData = "06 01 01 45 10 03 00 01 02 02 02 09 04 " + SetIP + "12" + SetPort + "02 02 09 04 " + SetIP + "12" + SetPort + "00";
                    Talkers[i].Framer698.sAPDU = cTaskData.Replace(" ", "");
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            //TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

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
                    TempData[i].StdData = MasterStationIP[i];
                    if (TalkResult[i] == 0)
                    {
                        string str = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                        //MasterStationIP[i]= new IPAddress(long.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier )).ToString();
                        //这里解析数据--保存到ini中
                        if (str.Length == 8)
                        {
                            for (int j = 0; j < str.Length; j += 2)
                            {
                                TempData[i].Data += Convert.ToInt32(str.Substring(j, 2), 16) + ".";
                            }
                            TempData[i].Data = TempData[i].Data.TrimEnd('.');
                        }
                        if (TempData[i].Data != TempData[i].StdData) TempData[i].Resoult = "不合格";
                    }
                    else
                    {
                        TempData[i].Resoult = "不合格";

                    }
                }
            }


            AddItemsResoult("主站IP", TempData);

            //MasterStationPort[i] = GetData(RecData, i, 6, EnumTerimalDataType.e_string);
            for (int i = 0; i < meterInfo.Length; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = MasterStationPort[i];
                    if (TalkResult[i] == 0)
                    {

                        TempData[i].Data = GetData(RecData, i, 6, EnumTerimalDataType.e_string);
                        if (TempData[i].Data != TempData[i].StdData) TempData[i].Resoult = "不合格";
                    }
                    else
                    {
                        TempData[i].Resoult = "不合格";

                    }
                }
            }
            AddItemsResoult("主站端口", TempData);

            MessageAdd("正在设置终端IP和端口",EnumLogType.提示信息);



            for (int i = 0; i < meterInfo.Length; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    SetIP = "";
                    string[] data = TerminalIP[i].Split('.');
                    //string SetPort = Convert.ToString(int.Parse(TerminalPort[i]), 16);
                    for (int j = 0; j < 4; j++)
                    {
                        SetIP += Convert.ToString(int.Parse(data[j]), 16).PadLeft(2, '0') + " ";
                    }

                    cTaskData = "06 01 01 45 10 04 00 02 06 16 01 09 04 " + SetIP + "09 04 ff ff ff 00 09 04 c0 a8 72 01 0a 00 0a 00 00";
                    Talkers[i].Framer698.sAPDU = cTaskData.Replace(" ", "");
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);



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
                    TempData[i].StdData = TerminalIP[i];
                    if (TalkResult[i] == 0)
                    {
                        string str = GetData(RecData, i, 6, EnumTerimalDataType.e_string);
                        //MasterStationIP[i]= new IPAddress(long.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier )).ToString();
                        //这里解析数据--保存到ini中
                        if (str.Length == 8)
                        {
                            for (int j = 0; j < str.Length; j += 2)
                            {
                                TempData[i].Data += Convert.ToInt32(str.Substring(j, 2), 16) + ".";
                            }
                            TempData[i].Data = TempData[i].Data.TrimEnd('.');
                        }
                        if (TempData[i].Data != TempData[i].StdData) TempData[i].Resoult = "不合格";
                    }
                    else
                    {
                        TempData[i].Resoult = "不合格";

                    }
                }
            }
            AddItemsResoult("终端IP", TempData);


            //MessageAdd("正在启用安全模式",EnumLogType.提示信息);
            //for (int i = 0; i < meterInfo.Length; i++)
            //{
            //    if (meterInfo[i].YaoJianYn)
            //    {
            //        cTaskData = "06 01 01 f1 01 02 00 16 01 00";
            //        Talkers[i].Framer698.sAPDU = cTaskData.Replace(" ", "");
            //        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
            //    }
            //}
            //// 68 19 00 43 05 01 00 00 00 00 00 10 fb bf 06 01 01 f1 01 02 00 16 00 00 25 0d 16
            //TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);


            ///恢复成原来的通讯方式
            if (IsSetConnType)
            {
                TerminalChannelType = ConnType;
                Set232ThreadNmber(false);
            }
            return;

        }
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "主站IP", "主站端口", "终端IP", "结论" };
            return true;
        }
    }
}
