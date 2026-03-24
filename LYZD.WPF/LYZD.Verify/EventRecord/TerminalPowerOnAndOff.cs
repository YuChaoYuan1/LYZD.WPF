using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.EventRecord
{
    /// <summary>
    /// 终端停/上电事件
    /// </summary>
    public class TerminalPowerOnAndOff : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "终端停电事件", "终端上电事件", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify698();

                int ret = 0;
                ConnectLink(false);


                DateTime dtHappen = DateTime.Now.AddMinutes(-1);//记录当前时间，事件发生的时间一定比这个晚

                DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:50");
                SetData_698("06010531060900030100", "设置事件有效标志");
                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");
                SetData_698("06010843000800030000", "禁止终端主动上报");
                ResetTerimal_698(2);
                ConnectLink2(false);
                if (OnMeterInfo.MD_UB == 220)
                    SetData_698("06010B310606000202020404080011181105010002061200011210E01200051200011205281206E000", "设置终端停上电配置参数");
                else if (OnMeterInfo.MD_UB == 100)
                    SetData_698("06010B310606000202020404080011181105010002061200011210E012000512000112025212031800", "设置终端停上电配置参数");
                else
                    SetData_698("06010B310606000202020404080011181105010002061200011210E01200051200011201571201c900", "设置终端停上电配置参数");
                SetTime_698(DateTime.Now, 0);

                PowerOn(OnMeterInfo.MD_UB * 0.45f, OnMeterInfo.MD_UB * 0.45f, OnMeterInfo.MD_UB * 0.45f, 0, 0, 0, 0, 240, 120, 0, 240, 120);
                WaitTime("等待", 130);


                MessageAdd("读取停电事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen(" 05 03 16 31 06 02 00 09 01 05 00 20 22 02 00 00 20 1E 02 00 00 20 20 02 00 00 20 24 02 00 00 33 09 02 06 00  ") + "05 03 16 31 06 02 00 09 01 05 00 20 22 02 00 00 20 1E 02 00 00 20 20 02 00 00 20 24 02 00 00 33 09 02 06 00   " + "0110" + Talkers[i].Framer698.cOutRand;

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = dtHappen.ToString();
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);// 19,  EnumTerimalDataType.e_string);
                            ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);
                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }

                            if (GetDateTime(GetData(RecData, i, 15, EnumTerimalDataType.e_datetime)) < dtHappen)
                                TempData[i].Resoult = "不合格";
                            TempData[i].Data = "次数:" + GetData(RecData, i, 14, EnumTerimalDataType.e_int) + "," + "发生时间:" + GetData(RecData, i, 15, EnumTerimalDataType.e_datetime);
                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("终端停电事件", TempData);

                PowerOn(OnMeterInfo.MD_UB * 0.85f, OnMeterInfo.MD_UB * 0.85f, OnMeterInfo.MD_UB * 0.85f, 0, 0, 0, 0, 240, 120, 0, 240, 120);
                WaitTime("等待", 190);
                MessageAdd("读取上电事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen(" 05 03 16 31 06 02 00 09 01 05 00 20 22 02 00 00 20 1E 02 00 00 20 20 02 00 00 20 24 02 00 00 33 09 02 06 00  ") + "05 03 16 31 06 02 00 09 01 05 00 20 22 02 00 00 20 1E 02 00 00 20 20 02 00 00 20 24 02 00 00 33 09 02 06 00   " + "0110" + Talkers[i].Framer698.cOutRand;

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = dtHappen.ToString();
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);// 19,  EnumTerimalDataType.e_string);
                            ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);
                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }

                            if (GetDateTime(GetData(RecData, i, 16, EnumTerimalDataType.e_datetime)) < dtHappen)
                                TempData[i].Resoult = "不合格";
                            TempData[i].Data = "次数:" + GetData(RecData, i, 14, EnumTerimalDataType.e_int) + "," + "发生时间:" + GetData(RecData, i, 16, EnumTerimalDataType.e_datetime);
                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("终端上电事件", TempData);


                PowerOn(OnMeterInfo.MD_UB , OnMeterInfo.MD_UB , OnMeterInfo.MD_UB , 0, 0, 0, 0, 240, 120, 0, 240, 120);
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
