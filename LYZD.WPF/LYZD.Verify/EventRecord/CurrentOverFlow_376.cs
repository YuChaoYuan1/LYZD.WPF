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
    /// 电流越限事件
    /// </summary>
    public class CurrentOverFlow_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "禁止终端主动上报", "终端事件记录配置设置", "终端事件计数当前值", "电流越限事件", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();

                 MessageAdd("下发终端参数：禁止主动上报。",EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 37, "", RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit_ReturnOk("禁止终端主动上报", 1);

                SetData = UsefulMethods.ConvertStringToBytes("00000001000000000000000000000000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 9, SetData, RecData, MaxWaitSeconds_Write);

                 MessageAdd("F26.测量点限值参数",EnumLogType.错误信息);
                //SetData = UsefulMethods.ConvertStringToBytes("50225021000500240150800020015000000001015080006000015080006000015080006003015080004003012080200001508020000150800102010203102390200005002401508000200150000000010150800060000150805052000150800063030150805046030120802000015080200001508001");
                SetData = Talkers[0].Framer.GetFnToByte_Afn04(26, Xub.ToString() + "," + xIb.ToString() + "," + ((int)Clfs).ToString() + ",1.03,0.97,0.22,1.09,0.91,6,4,4");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 26, SetData, RecData, MaxWaitSeconds_Write);


                string EventName = "电流越限事件";
                 MessageAdd("设置" + EventName + "有效",EnumLogType.错误信息);
                SetData = UsefulMethods.ConvertStringToBytes("00000001000000000000000000000000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 9, SetData, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit_ReturnOk("终端事件记录配置设置", 2);

                DateTime dtHappen = DateTime.Now.AddMinutes(-1);//记录当前时间，事件发生的时间一定比这个晚

                ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
                WaitTime("测试前抄表", VerifyConfig.WaitTime_CopyMeter);
                ControlVirtualMeter("Cmd,Set,220,220,220,5,8,5,0,0,0,1,0");
                WaitTime("事件发生抄表", VerifyConfig.WaitTime_CopyMeter);

                // 读取终端事件

                 MessageAdd("终端事件计数当前值",EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 7, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit_EventNum(3);


                 MessageAdd("请求一般事件",EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadEvent(14, 0, 2, RecData, NormalEventCount, MaxWaitSeconds_Write);

                BaseVerifyUnit_EventData(EventName, "ERC25", 4, dtHappen);

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
        protected new void BaseVerifyUnit_EventData(string EventName, string EventNum, int int_index, DateTime dtHappen)
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
                            if (GetData(RecData, i, 7, EnumTerimalDataType.e_string) == EventNum && GetDateTime(GetData(RecData, i, 9, EnumTerimalDataType.e_datetime)) > dtHappen && GetData(RecData, i, 10, EnumTerimalDataType.e_string) == "2" && GetData(RecData, i, 11, EnumTerimalDataType.e_string) == "发生" && GetData(RecData, i, 12, EnumTerimalDataType.e_string).Contains("B相电流越上上限"))
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
