using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LYZD.ViewModel.CheckController.DeviceControlS;

namespace LYZD.Verify.EventRecord
{
    /// <summary>
    /// 终端相序异常事件
    /// </summary>
    public class TerminalVoltageReversed : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "终端相序异常事件记录", "结论" };
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

                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");
                DateTime dtHappen = DateTime.Now.AddMinutes(-1);//记录当前时间，事件发生的时间一定比这个晚
                SetData_6982("060108300F0900030100", "设置事件有效标志");
                SetTime_698(DateTime.Now, 0);
                SetData_698_jiaocai("060109300F06000201111E00", "设置事件参数判定延时");

                PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB,xIb, xIb, xIb, 0,240,120,0,240,120);

                ResetTerimal_698(2);
                ConnectLink2(false);

                WaitTime("等待，", 120);

                PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb, 0, 120, 240, 0, 120, 240);
            
                WaitTime("等待，", 120);


                MessageAdd("读取终端相序异常事件记录",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen(" 05 03 0F 30 0F 02 00 02 20 1E 02 00 1C " + Talkers[0].Framer698.SetDateTimeBCD(dtHappen, false) + " 1C" + Talkers[0].Framer698.SetDateTimeBCD(dtHappen.AddMinutes(15), false) + " 54 00 00 00 00 00  ".Replace(" ", "")) + " 05 03 0F 30 0F 02 00 02 20 1E 02 00 1C " + Talkers[0].Framer698.SetDateTimeBCD(dtHappen, false) + " 1C" + Talkers[0].Framer698.SetDateTimeBCD(dtHappen.AddMinutes(15), false) + " 54 00 00 00 00 00  ".Replace(" ", "") + "0110" + Talkers[i].Framer698.cOutRand;

                       setData[i] = Talkers[i].Framer698.ReadData_jiaocai(Talkers[i].Framer698.sAPDU);
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

                            if (GetDateTime(GetMac(RecData[i], 2, "数据长度")) < dtHappen)
                                TempData[i].Resoult = "不合格";
                            TempData[i].Data = "序号:" + GetMac(RecData[i], 1, "数据长度") + "," + "发生时间:" + GetDateTime(GetMac(RecData[i], 2, "数据长度"));
                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("终端相序异常事件记录", TempData);
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
