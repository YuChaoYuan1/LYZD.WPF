using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LYZD.Verify.DataCollect
{
    /// <summary>
    /// 状态量采集
    /// </summary>
    public class StatusCollection_376 : VerifyBase
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
            //try
            //{
            //    base.Verify();
            //    StartVerify();

            //    Thread.Sleep(300);

            //    // 参数处理
            //    if (CLOU_Comm.GlobalUnit.g_TerminalType == EnumTerminalType.集中器I)
            //        m_int_RemoteCount = 4;       // 遥信路数
            //    else
            //        m_int_RemoteCount = 2;

            //    m_str_RemoteType = "常开触点";                      // 触点类型

            //    m_int_RemoteCount = int.Parse(CLOU_Comm.GlobalUnit.g_SystemConfig.SystemMode.getValue("RemoteCommunicationCount"));

            //    if (CLOU_Comm.GlobalUnit.g_TerminalType == EnumTerminalType.专变III && m_int_RemoteCount > 2)
            //        m_int_RemoteCount = 2;       // 遥信路数

            //    // 改变台体状态，输出遥信信号,输出相当于关闭，188误差板问题
            //    MessageAdd("台体关闭" + m_int_RemoteCount + "路遥信信号。", EnumLogType.错误信息);
            //    for (int i = 0; i < m_int_RemoteCount; i++)
            //    {
            //        deviceDriver.StartRemoteSignalOutput(GlobalUnit.g_TerminalVerifyFlags, i, 0, 0, 0, 0);
            //        Thread.Sleep(500);
            //    }
            //    deviceDriver.StopTest(GlobalUnit.g_TerminalVerifyFlags, 4);

            //    Thread.Sleep(500);

            //    #region 数据区初始化
            //    ResetTerimal(2);
            //    #endregion

            //    //设置终端参数 F9.终端事件记录配置设置
            //    MessageAdd("设置终端事件记录配置...", EnumLogType.错误信息);
            //    SetData = Core.Function.UsefulMethods.ConvertStringToBytes("08000000000000000000000000000000");
            //    TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 9, SetData, RecData, MaxWaitSeconds_Write);


            //    //设置终端参数 F12.终端状态量输入参数
            //    MessageAdd("设置终端状态量输入参数...", EnumLogType.错误信息);
            //    string str_SetData = "";
            //    str_SetData = (((int)(Math.Pow(2, m_int_RemoteCount) - 1)).ToString("X")).PadLeft(2, '0');
            //    if (m_str_RemoteType == "常开触点")
            //    {
            //        str_SetData = str_SetData + (((int)(Math.Pow(2, m_int_RemoteCount) - 1)).ToString("X")).PadLeft(2, '0');
            //    }
            //    else
            //    {
            //        str_SetData = str_SetData + "00";
            //    }
            //    SetData = Core.Function.UsefulMethods.ConvertStringToBytes(str_SetData);
            //    TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 12, SetData, RecData, MaxWaitSeconds_Write);


            //    WaitTime("等待", 10);

            //    MessageAdd("读取终端状态量及变位标志", EnumLogType.错误信息);
            //    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 9, RecData, MaxWaitSeconds_Write);

            //    for (int i = 0; i < MeterNumber; i++)
            //    {
            //       TempData[i].Resoult="合格";
            //    }

            //    for (int i = 0; i < MeterNumber; i++)
            //    {
            //        if (meterInfo[i].YaoJianYn)
            //        {
            //            if (TalkResult[i] == 0)
            //            {
            //                if (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(8 - m_int_RemoteCount, m_int_RemoteCount) != GetStdStatus(false))
            //                {
            //                    MessageAdd("终端" + (i + 1) + "状态量错误：" + RecData[i][3] + "/标准值为" + GetStdStatus(false), DateTime.Now.ToString());
            //                   TempData[i].Resoult="不合格";
            //                    m_str_FailReasons[i] = m_str_FailReasons[i] + "," + "终端状态量错误：" + RecData[i][3] + "/标准值为" + GetStdStatus(true);
            //                }
            //                else
            //                {
            //                    ResultDictionary[""][i] = "ST:" + GetData(RecData, i, 3, EnumTerimalDataType.e_string) + "|ST:00000000";
            //                }
            //            }
            //            else
            //            {
            //                MessageAdd("读取终端" + (i + 1) + "状态量及变位标志无回复！", EnumLogType.错误信息);
            //               TempData[i].Resoult="不合格";
            //                m_str_FailReasons[i] = m_str_FailReasons[i] + "," + "读取终端状态量及变位标志无回复";
            //            }
            //        }
            //    }

            //    RefUIData("变位前状态");


            //    // 误差板停止输出遥信信号，停止相当于输出
            //    MessageAdd("误差板输出遥信信号", EnumLogType.错误信息);

            //    if (deviceDriver.m_strErrorBoardType == "CL321")
            //    {
            //        int int_yx = 0;

            //        if (CLOU_Comm.GlobalUnit.g_TerminalType == EnumTerminalType.集中器I && m_int_RemoteCount == 4)
            //        {
            //            for (int i = 0; i < 8; i++)
            //            {
            //                int_yx += Convert.ToInt16(Math.Pow(2, i));
            //            }
            //        }
            //        else
            //        {
            //            for (int i = 0; i < m_int_RemoteCount; i++)
            //            {
            //                int_yx += Convert.ToInt16(Math.Pow(2, i));
            //            }
            //        }


            //        deviceDriver.StopRemoteSignalOutput(GlobalUnit.g_TerminalVerifyFlags, 0, int_yx - 1);
            //        Thread.Sleep(500);
            //    }
            //    else
            //    {
            //        for (int i = 0; i < m_int_RemoteCount; i++)
            //        {
            //            deviceDriver.StopRemoteSignalOutput(GlobalUnit.g_TerminalVerifyFlags, 0, i);
            //            Thread.Sleep(500);
            //        }
            //    }

            //    WaitTime("等待,", 20);

            //    MessageAdd("读取终端状态量及变位标志", EnumLogType.错误信息);
            //    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 9, RecData, MaxWaitSeconds_Write);


            //    for (int i = 0; i < MeterNumber; i++)
            //    {
            //       TempData[i].Resoult="合格";
            //    }
            //    for (int i = 0; i < MeterNumber; i++)
            //    {
            //        if (meterInfo[i].YaoJianYn)
            //        {
            //            if (TalkResult[i] == 0)
            //            {
            //                if (GetData(RecData, i, 3, EnumTerimalDataType.e_bs8).Substring(8 - m_int_RemoteCount, m_int_RemoteCount) != GetStdStatus(true))// || GetData(RecData[i][4]) != GetStdChangeFlag().PadLeft(8, '0'))
            //                {
            //                    MessageAdd("终端" + (i + 1) + "状态量错误：" + RecData[i][3] + "/标准值为" + GetStdStatus(false), DateTime.Now.ToString());
            //                   TempData[i].Resoult="不合格";
            //                    m_str_FailReasons[i] = m_str_FailReasons[i] + "," + "终端状态量错误：" + RecData[i][3] + "/标准值为" + GetStdStatus(true);
            //                }
            //                else
            //                {
            //                    ResultDictionary[""][i] = "ST:" + GetData(RecData, i, 3, EnumTerimalDataType.e_string) + ",CD:" + GetData(RecData, i, 4, EnumTerimalDataType.e_string) + "|ST:" + "0".PadLeft(8 - m_int_RemoteCount, '0') + "1".PadLeft(m_int_RemoteCount, '1') + ",CD:" + "0".PadLeft(8 - m_int_RemoteCount, '0') + "1".PadLeft(m_int_RemoteCount, '1');

            //                }
            //            }
            //            else
            //            {
            //                MessageAdd("读取终端" + (i + 1) + "状态量及变位标志无回复！", EnumLogType.错误信息);
            //               TempData[i].Resoult="不合格";
            //                m_str_FailReasons[i] = m_str_FailReasons[i] + "," + "读取终端状态量及变位标志无回复";
            //            }
            //        }
            //    }

            //    RefUIData("变位后状态");


            //    // 读取终端事件
            //    MessageAdd("读取终端最近一条事件", EnumLogType.错误信息);
            //    for (int i = 0; i < MeterNumber; i++)
            //    {
            //        if (meterInfo[i].YaoJianYn)
            //        {
            //            string[] str_ReadData = ReadTerminalEvent(i, false, "ERC4");
            //            if (str_ReadData.Length > 11)
            //            {
            //                if (GetData(str_ReadData, 7, EnumTerimalDataType.e_string) != "ERC4")
            //                {
            //                    MessageAdd("终端未产生状态量变位记录！", EnumLogType.错误信息);
            //                   TempData[i].Resoult="不合格";
            //                    m_str_FailReasons[i] = m_str_FailReasons[i] + "," + "终端未产生状态量变位记录";
            //                }
            //                else
            //                {
            //                    ResultDictionary[""][i] = "发生时间:" + GetData(str_ReadData, 9, EnumTerimalDataType.e_string) + " 变位后状态:" + GetData(str_ReadData, 11, EnumTerimalDataType.e_string) + "|ERC4";
            //                    if (GetData(str_ReadData, 11, EnumTerimalDataType.e_bs8).Substring(8 - m_int_RemoteCount, m_int_RemoteCount) != GetStdStatus(true))
            //                    {
            //                        MessageAdd("终端状态量变位记录不正确：变位后状态为" + str_ReadData[10] + "/标准值应为" + GetStdStatus(EnumLogType.错误信息), DateTime.Now.ToString());
            //                       TempData[i].Resoult="不合格";
            //                        m_str_FailReasons[i] = m_str_FailReasons[i] + "," + "终端状态量变位记录不正确：变位后状态为" + GetData(str_ReadData, 11, EnumTerimalDataType.e_string) + "/标准值应为" + GetStdStatus(true);
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                MessageAdd("终端未产生状态量变位记录", EnumLogType.错误信息);
            //               TempData[i].Resoult="不合格";
            //                m_str_FailReasons[i] = "";
            //            }
            //        }
            //    }


            //    RefUIData("状态量变位事件");
            //    Thread.Sleep(300);

            //    deviceDriver.StopTest(GlobalUnit.g_TerminalVerifyFlags, 4);

            //    // 通知UI层检定流程完成

            //    for (int i = 0; i < MeterNumber; i++)
            //    {
            //        if (!meterInfo[i].YaoJianYn) continue;
            //        ResultDictionary["结论"][i] = Resoult[i];
            //    }
            //    RefUIData("结论");
            //    MessageAdd("检定完成",EnumLogType.提示信息);
            //}
            //catch (Exception ex)
            //{

            //    MessageAdd(ex.ToString(), EnumLogType.错误信息);
            //}
        }
    }
}
