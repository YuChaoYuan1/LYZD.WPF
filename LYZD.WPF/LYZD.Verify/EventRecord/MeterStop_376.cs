using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.EventRecord
{
    /// <summary>
    /// 电能表停走事件
    /// </summary>
    public class MeterStop_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "电能表停走事件", "禁止终端主动上报", "终端事件记录配置设置", "终端事件计数当前值", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();

                ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
                ControlVirtualMeter("Cmd,DLS,8000,4000,1000,1000,1000,1000");

                MessageAdd("下发终端参数：禁止主动上报。", EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 37, "", RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit_ReturnOk("禁止终端主动上报", 1);


                string EventName = "电能表停走事件";
                MessageAdd("设置" + EventName + "有效", EnumLogType.错误信息);
                SetData = UsefulMethods.ConvertStringToBytes("10000020000000000000000000000000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 9, SetData, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit_ReturnOk("终端事件记录配置设置", 2);

                DateTime dtHappen = DateTime.Now;//记录当前时间，事件发生的时间一定比这个晚


                MessageAdd("设置终端参数 F59.电能表异常判别阈值设定", EnumLogType.错误信息);
                RecData.Clear();  //清除上一次数据
                SetData = UsefulMethods.ConvertStringToBytes("2040012C01");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 59, SetData, RecData, MaxWaitSeconds_Write);

                ResetTerimal(2);


                //ControlVirtualMeter("Cmd,Set,220,220,220,60,60,60,0,0,0,1,0");
                WaitTime("测试前抄表", 60 * 25);

                // 读取终端事件

                MessageAdd("终端事件计数当前值", EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 7, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit_EventNum(3);


                MessageAdd("请求一般事件", EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadEvent(14, 0, 2, RecData, NormalEventCount, MaxWaitSeconds_Write);

                BaseVerifyUnit_EventData(EventName, "ERC30", 4, dtHappen);

            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }

        /// <summary>
        /// 对事件进行处理
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="int_index"></param>
        protected  void BaseVerifyUnit_EventData(string EventName, string EventNum, int int_index, DateTime dtHappen)
        {
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = EventNum + "," + dtHappen.ToString();
                    if (TalkResult[i] == 0)
                    {
                        if (RecData[i].Length > 9)
                        {
                            TempData[i].Data = GetRemoveNumData(RecData[i][7]) + ";" + GetRemoveNumData(RecData[i][9]);
                            if (GetData(RecData, i, 7, EnumTerimalDataType.e_string) == EventNum && GetDateTime(GetData(RecData, i, 9, EnumTerimalDataType.e_datetime)) > dtHappen && GetData(RecData, i, 10, EnumTerimalDataType.e_string) == "2" && GetData(RecData, i, 11, EnumTerimalDataType.e_string) == "发生")

                            {
                                TempData[i].Tips = "产生" + EventName + "！";
                                TempData[i].Resoult = "合格";
                            }
                            else
                            {
                                TempData[i].Tips = "未产生" + EventName + "！";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "未产生" + EventName + "！";
                        }
                    }
                    else
                    {
                        TempData[i].Resoult = "不合格";
                        TempData[i].Tips = "读事件无回复";
                    }
                }
            }
            AddItemsResoult(EventName, TempData);
        }
    }
}
