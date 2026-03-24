using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.EventRecord
{
    /// <summary>
    /// 终端停/上电事件(带主动上报)
    /// </summary>
    public class TerminalPowerOnAndOffReport_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "禁止终端主动上报", "终端事件记录配置设置", "终端事件计数当前值", "终端停/上电事件", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();

                if (!(TerminalChannelType == Cus_EmChannelType.ChannelEther || TerminalChannelType == Cus_EmChannelType.ChannelGPRS))
                {
                    MessageAdd("请使用以太网或者GPRS测试!", EnumLogType.错误信息);
                    return;
                }

                MessageAdd("下发终端参数：禁止主动上报。", EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 37, "", RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit_ReturnOk("禁止终端主动上报", 1);


                string EventName = "终端停/上电事件";
                WriteData("设置电表常数事件有效", "00200000000000000020000000000000", 4, 0, 9);
                BaseVerifyUnit_ReturnOk("终端事件记录配置设置", 2);

                DateTime dtHappen = DateTime.Now.AddMinutes(-1);//记录当前时间，事件发生的时间一定比这个晚




                WriteData("设置终端参数 F67.定时发送1类数据任务启动/停止设置 ", "AA", 4, 1, 67);

                WriteData("设置终端参数 F68.定时发送2类数据任务启动/停止设置", "AA", 4, 1, 68);

                WriteData("设置终端参数 F97.停电数据采集配置参数", "0000050000", 4, 0, 97);

                MessageAdd("设置终端参数 F98.停电事件甄别限值参数", EnumLogType.错误信息);
                SetData = Talkers[0].Framer.GetFnToByte_Afn04(98, Xub.ToString() + ",0.6,0.8");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 98, SetData, RecData, MaxWaitSeconds_Write);

                WriteData("下发控制命令", "", 5, 0, 29);

                for (int i = 0; i < MeterNumber; i++)
                {
                    Talkers[i].ReceiveData = "";
                    Talkers[i].AFn = 14;
                    Talkers[i].Fn = 1;
                }

                PowerOn(0.4f);


                TalkResult = TerminalProtocalAdapter.Instance.WaitEventReport(RecData, 120);
                BaseVerifyUnit_EventData(EventName, "ERC14", 3, dtHappen);


                PowerOn(1.0f);
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }


        private void PowerOn(float f_V)
        {
            float ub = OnMeterInfo.MD_UB * f_V;
            PowerOn(ub, ub, ub, 0, 0, 0, Cus_PowerYuanJian.H,PowerWay.正向有功,"1.0");

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
                           
                            if (GetData(RecData, i, 7,EnumTerimalDataType.e_string) == EventNum && GetDateTime(GetData(RecData, i, 11,EnumTerimalDataType.e_datetime)) > dtHappen)
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
