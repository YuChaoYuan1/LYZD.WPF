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
    /// 保电功能
    /// </summary>
    public class ProtectEnergyFunction_376 : VerifyBase
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

                MessageAdd("设置终端参数 F58.终端自动保电参数 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("20000000000000000000000000000000");
                SetData = UsefulMethods.ConvertStringToBytes("20");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 58, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("下发控制命令允许合闸 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 1, 2, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("终端保电投入 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("00");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 0, 25, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("读终端控制设置状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 5, RecData, MaxWaitSeconds_Write);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "保电投入";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(7, 1) == "1" ? "保电投入" : "保电解除");
                            // 判断数据是否正确
                            if (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(7, 1) != "1")
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
                AddItemsResoult("保电状态1", TempData);

                MessageAdd("终端遥控跳闸 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("00");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 1, 2, SetData, RecData, MaxWaitSeconds_Write);


                MessageAdd("读终端控制设置状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 6, RecData, MaxWaitSeconds_Write);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "保电状态下不跳闸";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetData(RecData, i, 8, EnumTerimalDataType.e_bs8).Substring(6, 2) == "00" ? "保电状态下不跳闸" : "有跳闸") + "|保电状态下不跳闸";
                            // 判断数据是否正确
                            if (GetData(RecData, i, 8, EnumTerimalDataType.e_bs8).Substring(6, 2) != "00")
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
                AddItemsResoult("遥控跳闸", TempData);

                MessageAdd("终端保电解除 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 0, 33, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("读终端控制设置状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 5, RecData, MaxWaitSeconds_Write);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "保电解除";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(7, 1) == "0" ? "保电解除" : "保电投入");
                            // 判断数据是否正确
                            if (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(7, 1) != "0")
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
                AddItemsResoult("保电状态2", TempData);

                MessageAdd("设置终端参数 F58.终端自动保电参数 ...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("00");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 58, SetData, RecData, MaxWaitSeconds_Write);

            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
