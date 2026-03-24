using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.EventRecord
{
    /// <summary>
    /// 电能表时间超差事件
    /// </summary>
    public class MeterTimeError : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "电能表时间超差事件", "结论" };
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
                SetTime_698(DateTime.Now, 0);
                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");


                WaitTime("打开模拟表采集数据开关，", 2);

                MessageAdd("打开模拟表采集数据开关", EnumLogType.提示与流程信息, true);
                ControlVirtualMeter("MOP");

                ResetTerimal_698(2);
                ConnectLink2(false);

                DateTime dtHappen = DateTime.Now.AddMinutes(-1);//记录当前时间，事件发生的时间一定比这个晚

                SetData_698("06012D31050900030100", "设置事件有效标志");
                SetData_698("06012E310506000202120064110100", "设置电能表时钟超差事件参数");


                SetTime_698(DateTime.Now, 60);
                SetData_698_No("07 01 32 60 14 7F 00 01 01 02 06 11 01 12 01 00 02 02 11 00 00 01 01 5B 00 40 00 02 00 5C 01 16 02 00", "下装普通采集方案");
                SetData_698_No("07 01 33 60 12 7F 00 01 01 02 0C 11 01 54 03 00 01 16 01 11 01 1C 07 E1 07 1C 00 02 00 1C 08 33 09 09 09 09 09 54 00 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00", "下装采集任务");


                WaitTime("等待事件发生,", 300);



                MessageAdd("读取电能表时钟超差事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen(" 05 03 35 31 05 02 00 09 01 04 00 20 22 02 00 00 20 1E 02 00 00 20 20 02 00 00 20 24 02 00 00 ".Replace(" ", "")) + "05 03 35 31 05 02 00 09 01 04 00 20 22 02 00 00 20 1E 02 00 00 20 20 02 00 00 20 24 02 00 00".Replace(" ", "") + "0110" + Talkers[i].Framer698.cOutRand;

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

                            if (GetDateTime(GetData(RecData, i, 14, EnumTerimalDataType.e_datetime)) < dtHappen)
                                TempData[i].Resoult = "不合格";
                            TempData[i].Data = "次数:" + GetData(RecData, i, 13, EnumTerimalDataType.e_int) + "," + "发生时间:" + GetData(RecData, i, 14, EnumTerimalDataType.e_datetime);

                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("电能表时间超差事件", TempData);


                MessageAdd("关闭模拟表采集数据开关", EnumLogType.提示与流程信息, true);
                ControlVirtualMeter("MCL");

                WaitTime("关闭模拟表采集数据开关", 2);

            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
