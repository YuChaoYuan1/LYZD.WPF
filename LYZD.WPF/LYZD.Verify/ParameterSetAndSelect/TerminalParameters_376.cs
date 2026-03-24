using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.ParameterSetAndSelect
{
    /// <summary>
    /// 终端参数 --基本参数
    /// </summary>
      public class TerminalParameters_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "F4.参数及全体数据区初始化", "F9.终端事件记录配置(写)", "F9.终端事件记录配置(读)", "F10.终端电能表/交流采样装置配置(写)", "F10.终端电能表/交流采样装置配置(读)", "F12.终端状态量输入参数(写)", "F12.终端状态量输入参数(读)", "F25.测量点基本参数(写)", "F25.测量点基本参数(读)", "F33.终端抄表运行参数(写)", "F33.终端抄表运行参数(读)", "F57.终端声音告警(写)", "F57.终端声音告警(读)", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();

                MessageAdd("终端数据区初始化...",EnumLogType.流程信息);
                ResetTerimal(4);
                BaseVerifyUnit("F4.参数及全体数据区初始化");

                MessageAdd("下发终端参数：设置事件有效。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 9, Core.Function.UsefulMethods.ConvertStringToBytes("00200000000000000020000000000000"), RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F9.终端事件记录配置(写)");

                 MessageAdd("下发终端参数：读事件有效。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 9, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F9.终端事件记录配置(读)");

                 MessageAdd("下发终端参数：测量点。",EnumLogType.流程信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("020001000100010200000000000000000000000004090000000000000002000200621E010000000000000000000000040900000000000000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 10, SetData, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F10.终端电能表/交流采样装置配置(写)");

                 MessageAdd("下发终端参数：读事件有效。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData(10, 0, 10, Core.Function.UsefulMethods.ConvertStringToBytes("020001000200"), RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F10.终端电能表/交流采样装置配置(读)");

                 MessageAdd("下发终端参数：设置状态量2轮。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(4, 0, 12, "2", RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F12.终端状态量输入参数(写)");

                 MessageAdd("下发终端参数：读事件有效。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 12, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F12.终端状态量输入参数(读)");

                 MessageAdd("下发终端参数：设置测量点1基本参数。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(4, 1, 25, Xub.ToString() + "," + xIb.ToString() + "," + "9.9", RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F25.测量点基本参数(写)");

                 MessageAdd("下发终端参数：读事件有效。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 1, 25, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F25.测量点基本参数(读)");

                 MessageAdd("下发终端参数：设置测量点2基本参数。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(4, 2, 25, "220,1.5,9.9", RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F25.测量点基本参数(写)");

                 MessageAdd("下发终端参数：读事件有效。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 2, 25, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F25.测量点基本参数(读)");

                 MessageAdd("下发终端参数：设置测量点2基本参数。",EnumLogType.流程信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("02010000FFFFFFFF57230200000103000000080008001600165923020000FFFFFFFF57230200000103000000080008001600165923");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 33, SetData, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F33.终端抄表运行参数(写)");

                 MessageAdd("下发终端参数：读事件有效。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData(10, 0, 33, new byte[] { 2, 1, 2 }, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F33.终端抄表运行参数(读)");

                 MessageAdd("下发终端参数：设置声音告警参数。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(4, 0, 57, "", RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F57.终端声音告警(写)");

                 MessageAdd("下发终端参数：读事件有效。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 57, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F57.终端声音告警(读)");

            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }

        public void BaseVerifyUnit(string Name)
        {
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "设置成功";
                    if (TalkResult[i] == 0)
                    {
                        if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                        {
                             MessageAdd("终端" + (i + 1) + Name + "设置失败！",EnumLogType.错误信息);
                           TempData[i].Resoult="不合格";
                            TempData[i].Data = "设置失败";
                            TempData[i].Tips = Name + "设置失败";

                        }
                        else
                        {
                            TempData[i].Data = "设置成功";
                            TempData[i].Resoult = "合格";
                        }
                    }
                    else
                    {
                         MessageAdd("终端" + (i + 1) + Name + "设置无回复！",EnumLogType.错误信息);
                       TempData[i].Resoult="不合格";
                        TempData[i].Tips = Name + "设置无回复";
                    }
                }
            }
            AddItemsResoult(Name, TempData);
        }

        public void BaseVerifyUnit2(string Name)
        {
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "读取成功";
                    if (TalkResult[i] == 0)
                    {
                        if (RecData[i].Length < 3)
                        {
                           MessageAdd("终端" + (i + 1) + Name + "设置失败！",EnumLogType.错误信息);
                           TempData[i].Resoult="不合格";
                           TempData[i].Data = "读取失败";
                            TempData[i].Tips = Name+"设置失败";
                        }
                        else
                        {
                            TempData[i].Data= "读取成功";
                            TempData[i].Resoult = "合格";
                        }
                    }
                    else
                    {
                         MessageAdd("终端" + (i + 1) + Name + "设置无回复！",EnumLogType.错误信息);
                        TempData[i].Resoult="不合格";
                        TempData[i].Tips = Name + "设置无回复";
                    }
                }
            }
            AddItemsResoult(Name, TempData);
        }
    }
}
