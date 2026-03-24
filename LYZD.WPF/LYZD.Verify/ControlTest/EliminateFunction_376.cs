using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.ControlTest
{
    /// <summary>
    /// 剔除功能
    /// </summary>
    public class EliminateFunction_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "检定数据", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();

                string[] str_Conclusions = new string[MeterNumber];
                string[] str_VerifyDatas = new string[MeterNumber];

                //UsefulMethods.SetData( ref str_Conclusions, "合格");            

                //3.设置终端参数 F58.终端自动保电参数  

                MessageAdd("终端剔除解除 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 0, 36, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("终端保电解除 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 0, 33, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置终端保安定值50 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("00A5");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 17, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("终端剔除投入 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 0, 28, SetData, RecData, MaxWaitSeconds_Write);

                WaitTime("等待,", 10);

                #region 剔除状态1
                MessageAdd("读终端控制设置状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 5, RecData, MaxWaitSeconds_Write);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "投入";
                        if (TalkResult[i] == 0)
                        {
                            ResultDictionary[""][i] = (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(6, 1) == "1" ? "投入" : "解除");
                            // 判断数据是否正确
                            if (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(6, 1) != "1")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "不正确";
                            }
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("剔除状态1", TempData);

                #endregion


                MessageAdd("读终端组地址 ...",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 6, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer.str_MainStation = "03";
                            if (Talkers[i].Framer.LenAddr == 2)
                                Talkers[i].Framer.str_Ter_GroupAddress = "FFFF";
                            else if (Talkers[i].Framer.LenAddr == 4)
                                Talkers[i].Framer.str_Ter_GroupAddress = Convert.ToInt16(GetData(RecData, i, 3, EnumTerimalDataType.e_int)).ToString("X8");
                        }
                    }
                }




                MessageAdd("设置终端保安定值100 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("0081");

                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 17, SetData, RecData, MaxWaitSeconds_Write);

                SetTime(Convert.ToDateTime("2014-10-10 10:10:10"), 0);
                DateTime dt = Convert.ToDateTime("2014-10-10 10:10:10");



                for (int i = 0; i < MeterNumber; i++)
                {
                    Talkers[i].Framer.str_MainStation = "02";
                }

                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 2, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "实际时间：" + dt.ToString();
                        if (TalkResult[i] == 0)
                        {
                            if (RecData[i].Length == 4)
                            {
                                long ts = DateTime.Parse(GetData(RecData, i, 3, EnumTerimalDataType.e_datetime)).Ticks / 10000000 - dt.Ticks / 10000000;
                                if (Math.Abs(ts) > 120)
                                {
                                    TempData[i].Resoult = "不合格";
                                    ResultDictionary[""][i] = "可执行," + GetData(RecData, i, 3, EnumTerimalDataType.e_string) + "|实际时间：" + dt.ToString();
                                }
                            }
                            else
                            {
                                MessageAdd("读终端" + (i + 1) + "时间返回不正确！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "读终端时间返回不正确";
                            }
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "读终端时间未回复";
                        }
                    }
                }
                AddItemsResoult("终端对时命令", TempData);


                #region 终端保安定值设置
                MessageAdd("读终端保安定值",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 17, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "实际值50";
                        if (TalkResult[i] == 0)
                        {
                            // 判断数据是否正确
                            if (GetData(RecData, i, 3, EnumTerimalDataType.e_string) == "100")
                            {
                                TempData[i].Data = "不可设置:" + GetData(RecData, i, 3, EnumTerimalDataType.e_string);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "不正确";
                            }
                            else
                            {
                                TempData[i].Data = "不可设置:" + GetData(RecData, i, 3, EnumTerimalDataType.e_string);
                            }
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("终端保安定值设置", TempData);
                #endregion



                MessageAdd("终端剔除解除 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 0, 36, SetData, RecData, MaxWaitSeconds_Write);

                WaitTime("等待,", 10);

                MessageAdd("读终端控制设置状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 5, RecData, MaxWaitSeconds_Write);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "解除";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(6, 1) == "1" ? "投入" : "解除";
                            // 判断数据是否正确
                            if (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(6, 1) != "0")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "不正确";
                            }
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("剔除状态2", TempData);
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
