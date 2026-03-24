using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.ParameterSetAndSelect
{
    /// <summary>
    /// 抄表参数
    /// </summary
    public class MeterReadingParameters_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "F21.终端电能量费率时段和费率数(写)", "F22.终端电能量费率(读)", "F33.终端抄表运行参数设置(写)", "F33.终端抄表运行参数设置(读)", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();
                // 设置终端参数 F21.终端电能量费率时段和费率数
                 MessageAdd("下发抄表与费率参数：费率时段和费率数。",EnumLogType.流程信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("00000000000000000000000001010101010101010101010102020202020202020202020203030303030303030303030304");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 21, SetData, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F21.终端电能量费率时段和费率数(写)");

                 MessageAdd("下发终端参数：读事件有效。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 21, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F21.终端电能量费率时段和费率数(写)");

                // 设置终端参数 F22.终端电能量费率
                 MessageAdd("下发抄表与费率参数：终端电能量费率。",EnumLogType.流程信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("0401000000010000000100000001000000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 22, SetData, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F22.终端电能量费率(读)");

                 MessageAdd("下发终端参数：读事件有效。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 22, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F22.终端电能量费率(读)");

                // 设置终端参数 F33.终端抄表运行参数设置
                 MessageAdd("下发抄表与费率参数：终端抄表运行参数。",EnumLogType.流程信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("02010000FFFFFF7F57230200000103000000080008001600165923020000FFFFFF7F57230200000103000000080008001600165923");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 33, SetData, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F33.终端抄表运行参数设置(写)");

                 MessageAdd("下发终端参数：读事件有效。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData(10, 0, 33, new byte[] { 2, 1, 2 }, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F33.终端抄表运行参数设置(读)");


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
                    if (TalkResult[i] == 0)
                    {
                        if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                        {
                            //MessageAdd("终端" + (i + 1) + Name + "设置失败！",EnumLogType.流程信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = Name + "设置失败";
                        }
                    }
                    else
                    {
                        //MessageAdd("终端" + (i + 1) + Name + "设置无回复！",EnumLogType.流程信息);
                        TempData[i].Resoult = "不合格";
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
                    if (TalkResult[i] == 0)
                    {
                        if (RecData[i].Length < 3)
                        {
                            //MessageAdd("终端" + (i + 1) + Name + "设置失败！",EnumLogType.流程信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = Name + "设置失败";
                        }
                    }
                    else
                    {
                        //MessageAdd("终端" + (i + 1) + Name + "设置无回复！",EnumLogType.流程信息);
                        TempData[i].Resoult = "不合格";
                        TempData[i].Tips = Name + "设置无回复";
                    }
                }
            }
            AddItemsResoult(Name, TempData);

        }
    }
}
