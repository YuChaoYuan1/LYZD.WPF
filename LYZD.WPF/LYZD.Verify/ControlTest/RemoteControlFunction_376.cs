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
    /// 遥控功能
    /// </summary>
    public class RemoteControlFunction_376 : VerifyBase
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

                MessageAdd("保电解除 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 0, 33, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("遥控跳闸 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("00");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 1, 1, SetData, RecData, MaxWaitSeconds_Write);

                WaitTime("等待", 30);
                MessageAdd("读终端当前控制状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 6, RecData, MaxWaitSeconds_Write);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                    TempData[i].StdData = "第1轮遥控跳闸";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(7, 1) == "1" ? "第1轮遥控跳闸" : "第1轮遥控合闸");
                            // 判断数据是否正确
                            if (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(7, 1) != "1")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "不正确";
                            }
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("遥控跳闸1", TempData);

                MessageAdd("遥控合闸 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 1, 2, SetData, RecData, MaxWaitSeconds_Write);
                WaitTime("等待", 30);

                MessageAdd("读终端当前控制状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 6, RecData, MaxWaitSeconds_Write);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                    TempData[i].StdData = "第1轮遥控合闸";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(7, 1) == "1" ? "第1轮遥控跳闸" : "第1轮遥控合闸");
                            // 判断数据是否正确
                            if (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(7, 1) != "0")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "不正确";
                            }
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("遥控合闸1", TempData);


                MessageAdd("遥控跳闸 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("00");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 2, 1, SetData, RecData, MaxWaitSeconds_Write);

                WaitTime("等待", 30);
                MessageAdd("读终端当前控制状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 6, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                    TempData[i].StdData = "第2轮遥控跳闸";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(6, 1) == "1" ? "第2轮遥控跳闸" : "第2轮遥控合闸";
                            // 判断数据是否正确
                            if (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(6, 1) != "1")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "不正确";
                            }
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("遥控跳闸2", TempData);

                MessageAdd("遥控合闸 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 2, 2, SetData, RecData, MaxWaitSeconds_Write);

                WaitTime("等待", 30);
                MessageAdd("读终端当前控制状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 6, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                    TempData[i].StdData = "第2轮遥控合闸";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(6, 1) == "1" ? "第2轮遥控跳闸" : "第2轮遥控合闸") ;
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
                AddItemsResoult("遥控合闸2", TempData);

            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
