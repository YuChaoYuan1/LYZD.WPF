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
    ///  120个/分脉冲量采集
    /// </summary>
    public class PulseCollection120Unitmin : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "有功功率", "当日正向有功电量", "结论" };
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
                bool[] bol_VerifyFlags = new bool[MeterNumber];
                for (int i = 0; i < MeterNumber; i++)
                {
                    bol_VerifyFlags[i] = true;
                }

                Dictionary<int, string[]> m_dic_RecData_F35 = new Dictionary<int, string[]>();
                Dictionary<int, string[]> m_dic_RecData_F17 = new Dictionary<int, string[]>();
                Dictionary<int, string[]> m_dic_RecData_F19 = new Dictionary<int, string[]>();

                //MessageAdd("台体输出脉冲信号",EnumLogType.流程信息);

                //deviceDriver.StopRemoteSignalOutput(bol_VerifyFlags, 0, 2);
                //Thread.Sleep(500);
                //deviceDriver.StopTest(GlobalUnit.g_TerminalVerifyFlags, 4);
                //Thread.Sleep(500);




                #region 数据区初始化czx
                ResetTerimal_698(2);
                ConnectLink2(true);
                #endregion

                SetData_698("06013F24010300020212000A12006400", "设置互感器倍率配置");

                SetData_698("07010024010300020351F20A020116001203E800", "添加脉冲配置单元");

                SetData_698_No("06 01 01 F2 03 04 00 02 02 04 08 00 04 08 00 00", "设置开关量输入状态");


                MessageAdd("台体输出脉冲信号",EnumLogType.流程信息);

                SetPulseOutput(GetYaoJian(), 0x03, 2f, 0.5f, 2160, 2f, 0.5f, 2160);
                Thread.Sleep(500);

                WaitTime("持续输出脉冲中，脉冲输出时间剩余:", 1080);

                MessageAdd("读取有功功率",EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "24010500" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "7200";
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);

                            ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }
                            TempData[i].Data = double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10 + "," + string.Format("{0:0.00}", (Math.Abs((double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10 - 7200) / 72)));

                            if (Math.Abs((double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10 - 7200) / 72) <= 2)
                            {
                                //ResultDictionary["有功功率"][i] = double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10 + "," + string.Format("{0:0.00}", (Math.Abs((double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10 - 7200) / 72))) + "|7200";
                            }
                            else
                            {
                                TempData[i].Resoult = "不合格";
                                //ResultDictionary["有功功率"][i] = double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10 + "," + string.Format("{0:0.00}", (Math.Abs((double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10 - 7200) / 72))) + "|7200";
                                TempData[i].Tips = "误差超限！";
                            }

                        }
                    }
                }
                AddItemsResoult("有功功率", TempData);


                WaitTime("等待60秒后读取当日正向有功电量,剩余等待时间：", 60);

                MessageAdd("读取当日正向有功电量",EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "24010700" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "2160";
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);
                            ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }
                            TempData[i].Data = double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10 + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10 - 2160) / 21.6);

                            if (Math.Abs((double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10 - 2160) / 21.6) <= 2)
                            {
                                //ResultDictionary["当日正向有功电量"][i] = double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10 + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10 - 2160) / 21.6) + "|2160";
                            }
                            else
                            {
                                TempData[i].Resoult = "不合格";
                                //ResultDictionary["当日正向有功电量"][i] = double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10 + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10 - 2160) / 21.6) + "|2160|误差超限2";
                                TempData[i].Tips = "误差超限！";
                            }
                        }
                    }
                }
                AddItemsResoult("当日正向有功电量", TempData);
                SetPulseOutputStop(GetYaoJian());


            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
