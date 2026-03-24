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
    /// 催费告警
    /// </summary>
    public class ReminderFeeWarnning_376 : VerifyBase
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
                //2.设置终端参数 F23  终端催费告警参数

                MessageAdd( "设置终端参数催费告警参数...", EnumLogType.提示与流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 0, 23, UsefulMethods.ConvertStringToBytes("FFFFFF"), RecData, MaxWaitSeconds_Write);

                MessageAdd( "催费告警投入...", EnumLogType.提示与流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 0, 26, UsefulMethods.ConvertStringToBytes(""), RecData, MaxWaitSeconds_Write);

                MessageAdd( "读终端控制设置状态", EnumLogType.提示与流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 5, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                    TempData[i].StdData = "投入";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(5, 1) == "1" ? "投入" : "解除") ;
                            // 判断数据是否正确
                            if (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(5, 1) != "1")
                            {
                               TempData[i].Resoult="不合格";
                               TempData[i].Tips = "不正确";
                            }
                        }
                        else
                        {
                           TempData[i].Resoult="不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("催费告警状态1", TempData);

                MessageAdd( "催费告警解除...", EnumLogType.提示与流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 0, 34, UsefulMethods.ConvertStringToBytes(""), RecData, MaxWaitSeconds_Write);

                MessageAdd( "读终端控制设置状态", EnumLogType.提示与流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 5, RecData, MaxWaitSeconds_Write);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                    TempData[i].StdData = "解除";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(5, 1) == "1" ? "投入" : "解除");
                            // 判断数据是否正确
                            if (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(5, 1) != "0")
                            {
                               TempData[i].Resoult="不合格";
                               TempData[i].Tips = "不正确";
                            }
                        }
                        else
                        {
                           TempData[i].Resoult="不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("催费告警状态2", TempData);


            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
