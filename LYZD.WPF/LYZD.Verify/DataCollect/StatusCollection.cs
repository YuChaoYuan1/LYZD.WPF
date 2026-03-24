using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.DataCollect
{
    /// <summary>
    /// 状态量采集
    /// </summary>
    public class StatusCollection : VerifyBase
    {                       
        public override void Verify()
        {
            try
            {

               StartVerify698();

                bool[] bol_VerifyFlags = new bool[MeterNumber];
                for (int i = 0; i < MeterNumber; i++)
                {
                    bol_VerifyFlags[i] = true;
                }

                int ret = 0;
                ConnectLink(false);

                // 参数处理
                //int_RemoteCount = 1;       // 遥信路数
                //int_RemoteCount = 1;
                //str_RemoteType = "常开触点";                      // 触点类型
                DateTime dtHappen = DateTime.Now.AddMinutes(-1);//记录当前时间，事件发生的时间一定比这个晚

                ContnrRemoteSignalingStatusOutput(GetYaoJian(), true, true, false, false, false, false);

                WaitTime("等待,", 10);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        setData[i] = Talkers[i].Framer698.ReadData_05("F1010200");
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                SetTime_698(DateTime.Now, 0);
                SetData_698("06011831040900030100", "设置事件有效标志");

                if (Clfs == WireMode.单相)
                    SetData_698_No("06 01 1E F2 03 04 00 02 02 04 08 80 04 08 80 00", "设置开关量输入状态");
                else
                    SetData_698_No("06 01 1E F2 03 04 00 02 02 04 08 F0 04 08 F0 00", "设置开关量输入状态");


                MessageAdd("读取开关量输入状态",EnumLogType.提示信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        setData[i] = Talkers[i].Framer698.ReadData_05("F2030400");
                        //Talkers[i].Framer698.sAPDU = "05033860000200016001020112000101006001020000";

                        //setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            if (Clfs == WireMode.单相)
                            {
                                TempData[i].StdData = "10000000";
                                TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string) ;
                                if (GetData(RecData, i, 5, EnumTerimalDataType.e_string) != "10000000")
                                    TempData[i].Resoult = "不合格";
                            }
                            else
                            {
                                TempData[i].StdData = "11110000";
                                TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string) ;
                                if (GetData(RecData, i, 5, EnumTerimalDataType.e_string) != "11110000")
                                    TempData[i].Resoult = "不合格";
                            }
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }

                AddItemsResoult("读取开关量输入状态", TempData);

                MessageAdd("读取时钟",EnumLogType.提示信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        setData[i] = Talkers[i].Framer698.ReadData_05("40000200");
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string); ;
                        }
                        else
                        {
                            TempData[i].Tips = "无回复!";
                            TempData[i].Resoult = "不合格";
                        }

                    }
                }
                AddItemsResoult("读取时钟", TempData);


                MessageAdd("读取开关量输入开关量单元",EnumLogType.提示信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        setData[i] = Talkers[i].Framer698.ReadData_05("F2030200");
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 6, EnumTerimalDataType.e_string);
                        }
                        else
                        {
                            TempData[i].Tips = "无回复!";
                            TempData[i].Resoult = "不合格";
                        }

                    }
                }
                AddItemsResoult("读取开关量输入开关量单元", TempData);




                //这里停止遥信
                ContnrRemoteSignalingStatusOutput(GetYaoJian(), false, false, false, false, false, false);
                WaitTime("等待,", 20);


                MessageAdd("读取开关量输入开关量单元",EnumLogType.提示信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        setData[i] = Talkers[i].Framer698.ReadData_05("F2030200");
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 6, EnumTerimalDataType.e_string);
                        }
                        else
                        {
                            TempData[i].Tips = "无回复!";
                            TempData[i].Resoult = "不合格";
                        }

                    }
                }

                AddItemsResoult("读取开关量输入开关量单元", TempData);

                WaitTime("等待", 30);
                MessageAdd("读取上一次终端开关量变位事件",EnumLogType.提示信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);


                        Talkers[i].Framer698.sAPDU = "10000805010F" + "31040201" + "00" + "0110" + Talkers[i].Framer698.cOutRand;

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);


                            ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);



                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }

                            if (GetDateTime(GetData(RecData, i, 8, EnumTerimalDataType.e_datetime)) < dtHappen)
                                TempData[i].Resoult = "不合格";
                            TempData[i].Data = "发生时间:" + GetData(RecData, i, 8, EnumTerimalDataType.e_datetime);
                        }
                    }
                }

                AddItemsResoult("读取上一次终端开关量变位事件", TempData);


            }
            catch (Exception ex)
            {
                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }


            //StartVerify698();

            //RemoteType = "常开触点"; // 触点类型


            //if (OnMeterInfo.MD_TerminalType=="集中器I")
            //{

            //}
            // //TODO这里开启误差板启动遥信
            //for (int i = 0; i < RemoteCount; i++)
            //{

            //}
            //ContnrRemoteSignalingStatusOutput(GetYaoJian(), true, true, false, false, false, false);
            //ResetTerimal(2);

            ////设置终端参数 F9.终端事件记录配置设置
            //  MessageAdd("设置终端事件记录配置...",EnumLogType.流程信息);
            // SetData = Core.Function.UsefulMethods.ConvertStringToBytes("08000000000000000000000000000000");
            // TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 9, SetData, RecData, MaxWaitSeconds_Write);

            ////设置终端参数 F12.终端状态量输入参数
            //MessageAdd("设置终端状态量输入参数...",EnumLogType.流程信息);
            //string str_SetData = "";
            //str_SetData = (((int)(Math.Pow(2, RemoteCount) - 1)).ToString("X")).PadLeft(2, '0');
            //if (RemoteType == "常开触点")
            //{
            //    str_SetData = str_SetData + (((int)(Math.Pow(2,RemoteCount) - 1)).ToString("X")).PadLeft(2, '0');
            //}
            //else
            //{
            //    str_SetData = str_SetData + "00";
            //}

            // SetData = Core.Function.UsefulMethods.ConvertStringToBytes(str_SetData);
            // TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 12,SetData, RecData, MaxWaitSeconds_Write);
            // WaitTime("", 10);

            //MessageAdd("读取终端状态量及变位标志",EnumLogType.流程信息);

            //TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 9, RecData, MaxWaitSeconds_Write);

            //for (int i = 0; i < MeterNumber; i++)
            //{
            //    if (meterInfo[i].YaoJianYn)
            //    {
            //        TempData[i].StdData = "ST:00000000";
            //        if (TalkResult[i] == 0)
            //        {
            //             TempData[i].Data = "ST:" + GetData(RecData, i, 3, EnumTerimalDataType.e_string) ;
            //            if (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(8 - RemoteCount,RemoteCount) != GetStdStatus(false))
            //            {
            //                TempData[i].Tips = "状态量错误：" + RecData[i][3] + "/标准值为" + GetStdStatus(false);
            //                TempData[i].Resoult = "不合格";
            //                //m_str_FailReasons[i] = m_str_FailReasons[i] + "," + "终端状态量错误：" + RecData[i][3] + "/标准值为" + GetStdStatus(true);
            //            }
            //            else
            //            {
            //                //TempData[i].Data = "ST:" + GetData(RecData, i, 3, EnumTerimalDataType.e_string) ;
            //            }
            //        }
            //        else
            //        {
            //            TempData[i].Tips= "状态量及变位标志无回复！";
            //            TempData[i].Resoult = "不合格";
            //            //m_str_FailReasons[i] = m_str_FailReasons[i] + "," + "读取终端状态量及变位标志无回复";
            //        }
            //    }
            //}
            //AddItemsResoult("变位前状态", TempData);

            ////这里停止遥信
            //ContnrRemoteSignalingStatusOutput(GetYaoJian(), false, false, false, false, false, false);
            //WaitTime("正在停止遥信输出,", 20);


            //MessageAdd("读取终端状态量及变位标志",EnumLogType.流程信息);
            //TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 9, RecData, MaxWaitSeconds_Write);
            //for (int i = 0; i < MeterNumber; i++)
            //{
            //    if (meterInfo[i].YaoJianYn)
            //    {
            //        TempData[i].StdData = "ST:" + "0".PadLeft(8 -RemoteCount, '0') + "1".PadLeft(RemoteCount, '1') + ",CD: " + "0".PadLeft(8 - RemoteCount, '0') + "1".PadLeft(RemoteCount, '1');
            //        if (TalkResult[i] == 0)
            //        {

            //        //TempData[i].Data = "ST:" + GetData(RecData, i, 3, EnumTerimalDataType.e_string) + ",CD:" + GetData(RecData, i, 4, EnumTerimalDataType.e_string);
            //            if (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(8 - RemoteCount, RemoteCount) != GetStdStatus(true))// || GetData(RecData[i][4]) != GetStdChangeFlag().PadLeft(8, '0'))
            //            {
            //                TempData[i].Tips = "状态量错误：" + RecData[i][3] + "/标准值为" + GetStdStatus(false);
            //                TempData[i].Resoult = "不合格";
            //                //m_str_FailReasons[i] = m_str_FailReasons[i] + "," + "终端状态量错误：" + RecData[i][3] + "/标准值为" + GetStdStatus(true);
            //            }
            //            else
            //            {
            //                //ResultDictionary["变位后状态"][i] = "ST:" + GetData(RecData, i, 3, EnumTerimalDataType.e_string) + ",CD:" + GetData(RecData, i, 4, EnumTerimalDataType.e_string);
            //            }
            //        }
            //        else
            //        {
            //            TempData[i].Tips = "状态量及变位标志无回复！";
            //            TempData[i].Resoult= "不合格";
            //            //m_str_FailReasons[i] = m_str_FailReasons[i] + "," + "读取终端状态量及变位标志无回复";
            //        }
            //    }
            //}
            //AddItemsResoult("变位后状态", TempData);


            //// 读取终端事件
            //MessageAdd("读取终端最近一条事件",EnumLogType.流程信息);
            //for (int i = 0; i < MeterNumber; i++)
            //{
            //    if (meterInfo[i].YaoJianYn)
            //    {
            //        TempData[i].StdData = "ERC4";
            //        string[] str_ReadData = ReadTerminalEvent(i, false, "ERC4");
            //        if (str_ReadData.Length > 11)
            //        {
            //            TempData[i].Data = GetData(str_ReadData, 7, EnumTerimalDataType.e_string);
            //            if (GetData(str_ReadData, 7, EnumTerimalDataType.e_string) != "ERC4")
            //            {
            //                TempData[i].Tips = "终端未产生状态量变位记录！";
            //                TempData[i].Resoult = "不合格";
            //                //m_str_FailReasons[i] = m_str_FailReasons[i] + "," + "终端未产生状态量变位记录";
            //            }
            //            else
            //            {

            //                TempData[i].Tips = "发生时间:" + GetData(str_ReadData, 9, EnumTerimalDataType.e_string) + " 变位后状态:" + GetData(str_ReadData, 11,EnumTerimalDataType.e_string);
            //                if (GetData(str_ReadData, 11, EnumTerimalDataType.e_bs8).Substring(8 - RemoteCount, RemoteCount) != GetStdStatus(true))
            //                {
            //                    TempData[i].Tips = "终端状态量变位记录不正确：变位后状态为" + str_ReadData[10] + "/标准值应为" + GetStdStatus(true);
            //                    TempData[i].Resoult = "不合格";
            //                    //m_str_FailReasons[i] = m_str_FailReasons[i] + "," + "终端状态量变位记录不正确：变位后状态为" + GetData(str_ReadData, 11, EnumTerimalDataType.e_string) + "/标准值应为" + GetStdStatus(true);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            TempData[i].Tips = "终端未产生状态量变位记录";
            //            TempData[i].Resoult = "不合格";
            //            //m_str_FailReasons[i] = "";
            //        }
            //    }
            //}
            //AddItemsResoult("状态量变位事件", TempData);

        }

        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //三相电压|电能表日历时钟|时段表编程总次数|校时次数|电表运行状态字|正向有功电能示值|反向有功电能示值|正向有功最大需量|反向有功最大需量
            ResultNames = new string[] { "变位前状态", "变位后状态", "状态量变位事件", "结论" };
            return true;
        }

    }
}
