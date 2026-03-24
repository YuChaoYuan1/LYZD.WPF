using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LYZD.Verify.EventRecord
{
    /// <summary>
    /// 终端485抄表错误
    /// </summary>
    public class ReadMeterFail_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "禁止终端主动上报", "终端事件记录配置设置", "终端事件计数当前值", "终端485抄表故障事件", "结论" };
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


                string EventName = "终端485抄表故障事件";
                 MessageAdd("设置" + EventName + "有效",EnumLogType.错误信息);
                SetData = UsefulMethods.ConvertStringToBytes("00001000000000000000000000000000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 9, SetData, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit_ReturnOk("终端事件记录配置设置", 2);

                DateTime dtHappen = DateTime.Now.AddMinutes(-1);//记录当前时间，事件发生的时间一定比这个晚


                WaitTime("测试前抄表", VerifyConfig.WaitTime_CopyMeter);
                Kill_Monibiao("VirtualMeter.exe");
                for (int m = 0; m < 3; m++)
                {
                    ControlVirtualMeter("BTL9600");
                    Thread.Sleep(5000);
                }
                WaitTime("事件发生抄表", VerifyConfig.WaitTime_CopyMeter * 2);

                // 读取终端事件

                 MessageAdd("终端事件计数当前值",EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 7, RecData, MaxWaitSeconds_Write);
                BaseVerifyUnit_EventNum(3);


                 MessageAdd("请求一般事件",EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadEvent(14, 0, 2, RecData, NormalEventCount, MaxWaitSeconds_Write);

                BaseVerifyUnit_EventData(EventName, "ERC21", 4, dtHappen);
                Start_Monibiao("VirtualMeter.exe");
                for (int m = 0; m < 3; m++)
                {
                    ControlVirtualMeter("BTL2400");
                    Thread.Sleep(5000);
                }
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
                            if (GetData(RecData, i, 7, EnumTerimalDataType.e_string) == EventNum && GetDateTime(GetData(RecData, i, 9, EnumTerimalDataType.e_datetime)) > dtHappen && GetData(RecData, i, 10, EnumTerimalDataType.e_string).Contains("485抄表故障"))
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
        /// <summary>
        /// 关闭模拟表程序
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="int_index"></param>
        private void Kill_Monibiao(string ProcessName)
        {
            System.Diagnostics.Process[] Ps = System.Diagnostics.Process.GetProcessesByName("VirtualMeter");
            if (Ps.Length > 0)
            {
                try
                {
                    Ps[0].Kill();

                }
                catch (Exception)
                {

                }
            }
        }
        /// <summary>
        /// 打开模拟表程序
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="int_index"></param>
        private void Start_Monibiao(string ProcessName)
        {
            string str_SysInfoIni = System.Windows.Forms.Application.StartupPath + "\\" + ProcessName;
            System.Diagnostics.ProcessStartInfo StInfo = new System.Diagnostics.ProcessStartInfo();
            StInfo.FileName = str_SysInfoIni;
            StInfo.Arguments = "";
            StInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            System.Diagnostics.Process Ps = System.Diagnostics.Process.Start(StInfo);


        }
    }
}
