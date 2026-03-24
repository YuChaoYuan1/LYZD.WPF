using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.ParameterSetAndSelect
{
    /// <summary>
    /// 时钟设置和召集
    /// </summary>
    public class ReadSetTime_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "终端对时", "读取终端时间", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();
                // 终端对时
                 MessageAdd("正在进行终端对时...",EnumLogType.错误信息);


                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 31, DateTime.Now.ToString(), RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "Fn1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                            {
                                TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                                //MessageAdd("终端" + (i + 1) + "对时回复否认！",EnumLogType.错误信息);
                                TempData[i].Tips = "对时回复否认";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "对时无回复！",EnumLogType.错误信息);
                            TempData[i].Tips = "对时无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("终端对时", TempData);

                WaitTime("等待", 5);

                // 读时间，对比
                 MessageAdd("正在读取终端时钟...",EnumLogType.错误信息);
                DateTime dt = DateTime.Now;
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 2, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].StdData = dt.ToString();
                            if (RecData[i].Length == 4)
                            {
                                long ts = DateTime.Parse(GetData(RecData, i, 3, EnumTerimalDataType.e_datetime)).Ticks / 10000000 - dt.Ticks / 10000000;
                                TempData[i].Data = GetData(RecData, i, 3, EnumTerimalDataType.e_string) ;
                                if (Math.Abs(ts) > 10)
                                {
                                    TempData[i].Resoult = "不合格";
                                    TempData[i].Tips = "误差超差(限制为2；误差值为)" + ts.ToString();
                                }
                            }
                            else
                            {
                                TempData[i].Tips = "读终端时间返回不正确！";
                                TempData[i].Resoult = "不合格";
                                TempData[i].Data = "----/--/-- --:--:--";
                            }
                        }
                        else
                        {
                            TempData[i].Tips = "读终端时间无回复";
                            TempData[i].Resoult = "不合格";
                            TempData[i].Data = "----/--/-- --:--:--";
                        }
                    }
                }
                AddItemsResoult("读取终端时间", TempData);
            }
            catch (Exception ex)
            {
                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
