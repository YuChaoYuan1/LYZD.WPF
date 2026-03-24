using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.ParameterSetAndSelect
{
    /// <summary>
    /// 限值与阈值参数
    /// </summary>
    public class ParameterOfThreshold_376 : VerifyBase
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


                // 设置终端参数 F26.测量点限值参数
                 MessageAdd("下发限值与阈值参数：测量点限值参数。",EnumLogType.错误信息);
                //SetData = Core.Function.UsefulMethods.ConvertStringToBytes("50225021000500240150800020015000000001015080006000015080006000015080006003015080004003012080200001508020000150800102010203502250210005002401508000200150000000010150800060000150800060000150800060030150800040030120802000015080200001508001");
                SetData = Talkers[0].Framer.GetFnToByte_Afn04(26, Xub.ToString() + "," + xIb.ToString() + "," + ((int)Clfs).ToString() + ",1.03,0.97,0.22,1.09,0.91,6,4,4");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 26, SetData, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F26.测量点限值参数");

                // 设置终端参数 F26.测量点限值参数
                 MessageAdd("下发限值与阈值参数：测量点限值参数。",EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 1, 26, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F26.测量点限值参数");

                // 设置终端参数 F26.测量点限值参数
                 MessageAdd("下发限值与阈值参数：测量点限值参数。",EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("502250210005002401508000200150000000010150800060000150800060000150800060030150800040030120802000015080200001508001");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 2, 26, SetData, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F26.测量点限值参数");

                // 设置终端参数 F26.测量点限值参数
                 MessageAdd("下发限值与阈值参数：测量点限值参数。",EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 2, 26, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F26.测量点限值参数");


                //设置终端参数 F28.测量点功率因数分段限值
                 MessageAdd("下发限值与阈值参数：测量点功率因数分段限值。",EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("000800100201080300080010");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 28, SetData, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F28.测量点功率因数分段限值");

                 MessageAdd("下发限值与阈值参数：测量点限值参数。",EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 1, 28, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F26.测量点限值参数");


                //设置终端参数 F36.终端上行通信流量门限设置
                 MessageAdd("下发限值与阈值参数：终端上行通信流量门限。",EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("00000000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 36, SetData, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F36.终端上行通信流量门限设置");

                 MessageAdd("下发限值与阈值参数：测量点限值参数。",EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 36, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F36.终端上行通信流量门限设置");

                //设置终端参数 F59.电能表异常判别阈值设定
                 MessageAdd("下发限值与阈值参数：电能表异常判别阈值。",EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("2040012C01");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 59, SetData, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit("F59.电能表异常判别阈值");

                 MessageAdd("下发限值与阈值参数：测量点限值参数。",EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 59, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit2("F59.电能表异常判别阈值");

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
                    TempData[i].StdData = "Fn1";
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                        if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                        {
                             //MessageAdd("终端" + (i + 1) + Name + "设置失败！",EnumLogType.错误信息);
                            TempData[i].Resoult="不合格";
                            TempData[i].Tips =  Name + "设置失败";
                        }
                    }
                    else
                    {
                        TempData[i].Tips = Name+"设置无回复";
                         //MessageAdd("终端" + (i + 1) + Name + "设置无回复！",EnumLogType.错误信息);
                       TempData[i].Resoult="不合格";
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
                            //MessageAdd("终端" + (i + 1) + Name + "设置失败！",EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = Name + "设置失败";
                        }
                    }
                    else
                    {
                        //MessageAdd("终端" + (i + 1) + Name + "设置无回复！",EnumLogType.错误信息);
                        TempData[i].Resoult = "不合格";
                        TempData[i].Tips = Name + "设置无回复";

                    }
                }
            }
            AddItemsResoult(Name, TempData);
        }
    }
}
