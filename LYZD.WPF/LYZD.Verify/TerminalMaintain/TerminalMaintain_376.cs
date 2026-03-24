using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.TerminalMaintain
{
    /// <summary>
    ///终端维护
    /// </summary>
     public class TerminalMaintain_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "上1次终端初始化事件","终端初始化事件当前记录数", "终端维护","终端对时", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();
                MessageAdd("正在进行终端对时。",EnumLogType.流程信息);

                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 31, DateTime.Now.ToString(), RecData, MaxWaitSeconds_Write);

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
                                MessageAdd("终端" + (i + 1) + "对时回复否认！",EnumLogType.流程信息);
                                TempData[i].Resoult="不合格";
                            }
                            else
                            {
                                TempData[i].Resoult = "合格";
                                TempData[i].Tips = "终端对时成功";
                            }
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "对时回复否认！",EnumLogType.流程信息);
                            TempData[i].Tips = "对时回复否认";
                            TempData[i].Resoult = "不合格";
                        }

                    }
                }
                AddItemsResoult("终端对时", TempData);



                // 下发终端参数：设置事件有效。
                MessageAdd("恢复出厂设置。",EnumLogType.流程信息);
                ResetTerimal(3);
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
                                MessageAdd("终端" + (i + 1) + "恢复出厂设置失败！",EnumLogType.流程信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "恢复出厂设置失败";
                            }
                            else
                            {
                                TempData[i].Resoult = "合格";
                                TempData[i].Tips = "恢复出厂设置成功";
                            }
                        }
                        else
                        {
                           MessageAdd("终端" + (i + 1) + "恢复出厂设置无回复！",EnumLogType.流程信息);
                           TempData[i].Resoult="不合格";
                           TempData[i].Tips =  "恢复出厂设置无回复";
                        }
                    }
                }
                AddItemsResoult("终端维护", TempData);
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }

}
