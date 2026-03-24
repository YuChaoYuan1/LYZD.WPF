using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.ParameterSetAndSelect
{
    /// <summary>
    /// 控制参数---国网
    /// 参数：无
    /// </summary>
    public class ParameterOfControl_376 : VerifyBase
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
            try
            {
                base.Verify();
                StartVerify();


                byte GroupTotalCount = VerifyConfig.GroupTotalCount;

                byte GroupTotal1Count = VerifyConfig.GroupTotal1Count;

                byte GroupTotal2Count = VerifyConfig.GroupTotal2Count;

                string[] GroupTotal1Pn = VerifyConfig.GroupTotal1Pn.Split('|');

                string[] GroupTotal2Pn = VerifyConfig.GroupTotal2Pn.Split('|');

                byte PulseCount = VerifyConfig.PulseCount;


                #region F11.终端脉冲配置参数

                //设置终端参数 F11.终端脉冲配置参数
                MessageAdd("下发控制参数：终端脉冲配置参数。", EnumLogType.错误信息);

                //SetData = Core.Function.UsefulMethods.ConvertStringToBytes("02010300E803020400E803");
                SetData = new byte[PulseCount * 5 + 1];
                SetData[0] = PulseCount;
                for (byte i = 0; i < PulseCount; i++)
                {
                    SetData[i * 5 + 1] = Convert.ToByte(i + 1);
                    SetData[i * 5 + 2] = Convert.ToByte(i + 3);
                    SetData[i * 5 + 3] = 0;
                    SetData[i * 5 + 4] = 0xe8;
                    SetData[i * 5 + 5] = 0x03;
                }
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 11, SetData, RecData, MaxWaitSeconds_Write);

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
                                //MessageAdd("终端" + (i + 1) + "设置终端脉冲配置参数失败！", EnumLogType.错误信息);  
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置终端脉冲配置参数失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置终端脉冲配置参数无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips ="设置终端脉冲配置参数无回复";
                        }
                    }
                }
                AddItemsResoult("F11.终端脉冲配置参数", TempData);
                #endregion

                #region F14.终端总加组配置参数
                MessageAdd("下发终端参数：设置测量点3基本参数。", EnumLogType.错误信息);
                for (byte p1 = 3; p1 <= PulseCount + 2; p1++)
                {
                    TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(4, p1, 25, "220,1.5,9.9", RecData, MaxWaitSeconds_Write);
                }
                //设置终端参数 F14.终端总加组配置参数
                MessageAdd("下发控制参数：终端总加组配置参数。", EnumLogType.错误信息);
                //SetData = Core.Function.UsefulMethods.ConvertStringToBytes("02010102020101");
                if (GroupTotalCount == 1)
                    SetData = new byte[GroupTotal1Count + 2 + 1];
                else
                    SetData = new byte[GroupTotal1Count + 2 + GroupTotal2Count + 2 + 1];

                SetData[0] = Convert.ToByte(GroupTotalCount);

                int intPnCount = GroupTotal1Count;

                if (intPnCount < GroupTotal2Count) intPnCount = GroupTotal2Count;

                for (byte i = 0; i < GroupTotalCount; i++)
                {
                    if (i == 0)
                    {
                        SetData[i * (intPnCount + 2) + 1] = Convert.ToByte(i + 1);
                        SetData[i * (intPnCount + 2) + 2] = Convert.ToByte(GroupTotal1Count);

                        for (int intInc = 0; intInc < GroupTotal1Pn.Length && GroupTotal1Count >= 1; intInc++)
                            SetData[i * (intPnCount + 2) + 3 + intInc] = Convert.ToByte(int.Parse(GroupTotal1Pn[intInc]) - 1);
                    }
                    else
                    {
                        SetData[i * (intPnCount + 2) + 1] = Convert.ToByte(i + 1);
                        SetData[i * (intPnCount + 2) + 2] = Convert.ToByte(GroupTotal2Count);

                        for (int intInc = 0; intInc < GroupTotal2Pn.Length && GroupTotal2Count >= 1; intInc++)
                            SetData[i * (intPnCount + 2) + 3 + intInc] = Convert.ToByte(int.Parse(GroupTotal2Pn[intInc]) - 1);

                    }
                }
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 14, SetData, RecData, MaxWaitSeconds_Write);

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
                                //MessageAdd("终端" + (i + 1) + "设置终端总加组配置参数失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置终端总加组配置参数失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置终端总加组配置参数无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置终端总加组配置参数无回复";
                        }
                    }
                }
                AddItemsResoult("F14.终端总加组配置参数", TempData);
                #endregion

                #region F17.终端保安定值
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("020102");
                TalkResult = TerminalProtocalAdapter.Instance.ReadData(10, 0, 14, SetData, RecData, MaxWaitSeconds_Write);

                //设置终端参数 F17.终端保安定值
                MessageAdd("下发控制参数：终端保安定值。", EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("0081");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 17, SetData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            //MessageAdd("终端" + (i + 1) + "设置终端保安定值失败！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置终端保安定值失败";
                        }
                    }
                    else
                    {
                        //MessageAdd("终端" + (i + 1) + "设置终端终端保安定值无回复！", EnumLogType.错误信息);
                        TempData[i].Resoult = "不合格";
                        TempData[i].Tips = "设置终端保安定值无回复";
                    }
                    //
                }
                AddItemsResoult("F17.终端保安定值", TempData);
                #endregion

                #region F18.终端功控时段
                //设置终端参数 F18.终端功控时段
                MessageAdd("下发控制参数：终端功控时段。", EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("55A5AA55A5AA55A5AA55A5AA");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 18, SetData, RecData, MaxWaitSeconds_Write);

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
                                //MessageAdd("终端" + (i + 1) + "设置终端功控时段失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置终端功控时段失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置终端功控时段无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置终端功控时段无回复";
                        }
                    }
                }
                AddItemsResoult("F18.终端功控时段", TempData);
                #endregion

                #region F19.终端时段功控定值浮动系数

                //设置终端参数 F19.终端时段功控定值浮动系数
                MessageAdd("下发控制参数：终端时段功控定值浮动系数。", EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("00");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 19, SetData, RecData, MaxWaitSeconds_Write);

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
                                //MessageAdd("终端" + (i + 1) + "设置终端时段功控定值浮动系数失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置终端时段功控定值浮动系数失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置终端时段功控定值浮动系数无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置终端时段功控定值浮动系数无回复";
                        }
                    }
                }
                AddItemsResoult("F19.终端时段功控定值浮动系数", TempData);
                #endregion

                #region F20.终端月电能量控定值浮动系数

                //设置终端参数 F20.终端月电能量控定值浮动系数
                MessageAdd("下发控制参数：终端月电能量控定值浮动系数。", EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("00");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 20, SetData, RecData, MaxWaitSeconds_Write);

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
                                //MessageAdd("终端" + (i + 1) + "设置终端月电能量控定值浮动系数失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置终端月电能量控定值浮动系数失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置终端月电能量控定值浮动系数无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置终端月电能量控定值浮动系数无回复";
                        }
                    }
                }
                AddItemsResoult("F20.终端月电能量控定值浮动系数", TempData);
                #endregion

                #region F41.时段功控定值
                //设置终端参数 F41.时段功控定值
                MessageAdd("下发控制参数：时段功控定值。", EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("07FF00620062006200620062006200620062FF10671067106710671067106710671067FF00680068006800680068006800680068");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 41, SetData, RecData, MaxWaitSeconds_Write);

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
                                //MessageAdd("终端" + (i + 1) + "设置时段功控定值失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置时段功控定值失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置时段功控定值无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置时段功控定值无回复";
                        }
                    }
                }
                AddItemsResoult("F41.时段功控定值", TempData);
                #endregion

                #region F42.厂休功控参数
                //设置终端参数 F42.厂休功控参数
                MessageAdd("下发控制参数：厂休功控参数。", EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("006200081EFE02010205006200081EFE");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 42, SetData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "Fn1";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                            {
                                //MessageAdd("终端" + (i + 1) + "设置厂休功控参数失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置厂休功控参数失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置厂休功控参数无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置厂休功控参数无回复";
                        }
                    }
                }
                AddItemsResoult("F42.厂休功控参数", TempData);
                #endregion

                #region F43.功率控制的功率计算滑差时间
                //设置终端参数 F43.功率控制的功率计算滑差时间
                MessageAdd("下发控制参数：功率计算滑差时间。", EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("010201040501");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 43, SetData, RecData, MaxWaitSeconds_Write);

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
                                //MessageAdd("终端" + (i + 1) + "设置功率计算滑差时间失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置功率计算滑差时间失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置功率计算滑差时间无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置功率计算滑差时间无回复";
                        }
                    }
                }
                AddItemsResoult("F43.功率控制的功率计算滑差时间", TempData);
                #endregion

                #region F44.营业报停控参数

                //设置终端参数 F44.营业报停控参数
                MessageAdd("下发控制参数：营业报停控参数。", EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("0101080201080062020108050101080201080062");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 44, SetData, RecData, MaxWaitSeconds_Write);

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
                                //MessageAdd("终端" + (i + 1) + "设置营业报停控参数失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置营业报停控参数失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置营业报停控参数无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置营业报停控参数无回复";
                        }
                    }
                }
                AddItemsResoult("F44.营业报停控参数", TempData);
                #endregion

                #region F45.功控轮次设定
                //设置终端参数 F45.功控轮次设定
                MessageAdd("下发控制参数：功控轮次设定。",EnumLogType.流程信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("030201100503");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 45, SetData, RecData, MaxWaitSeconds_Write);

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
                                //MessageAdd("终端" + (i + 1) + "设置功控轮次设定失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置功控轮次设定失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置功控轮次设定无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置功控轮次设定无回复";
                        }
                        //
                    }
                }
                AddItemsResoult("F45.功控轮次设定", TempData);
                #endregion

                #region F46.月电量控定值
                //设置终端参数 F46.月电量控定值
                MessageAdd("下发控制参数：月电量控定值。", EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("0030000080020120050030000080");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 2, 46, SetData, RecData, MaxWaitSeconds_Write);

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
                                //MessageAdd("终端" + (i + 1) + "设置月电量控定值失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置月电量控定值失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置月电量控定值无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置月电量控定值无回复";
                        }
                    }
                }
                AddItemsResoult("F46.月电量控定值", TempData);
                #endregion

                #region F47.购电量控参数
                //设置终端参数 F47.购电量控参数
                MessageAdd("下发控制参数：购电量控参数。", EnumLogType.错误信息);
                DateTime dtTmp = DateTime.Now;
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes(UsefulMethods.OrgBinFun(dtTmp.Minute + dtTmp.Hour * 100 + dtTmp.Day * 10000 + dtTmp.Month * 1000000, 4) + "AA002000000002000000010000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 2, 47, SetData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "Fn1";
                        if (TalkResult[i] == 0 && RecData[i] != null)
                        {
                            TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                            {
                                //MessageAdd("终端" + (i + 1) + "设置购电量控参数失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                 TempData[i].Tips = "设置购电量控参数失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置购电量控参数无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                             TempData[i].Tips = "设置购电量控参数无回复";
                        }
                    }
                }
                AddItemsResoult("F47.购电量控参数", TempData);
                #endregion

                #region F48.电控轮次设定
                //设置终端参数 F48.电控轮次设定
                MessageAdd("下发控制参数：电控轮次设定。", EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("030201800503");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 48, SetData, RecData, MaxWaitSeconds_Write);

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
                                //MessageAdd("终端" + (i + 1) + "设置电控轮次设定失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                 TempData[i].Tips = "设置电控轮次设定失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置电控轮次设定无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                             TempData[i].Tips = "设置电控轮次设定无回复";
                        }
                    }
                }
                AddItemsResoult("F48.电控轮次设定", TempData);
                #endregion

                #region F49.功控告警时间

                //设置终端参数 F49.功控告警时间
                MessageAdd("下发控制参数：功控告警时间。", EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("030201010603");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 49, SetData, RecData, MaxWaitSeconds_Write);

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
                                //MessageAdd("终端" + (i + 1) + "设置功控告警时间失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                 TempData[i].Tips = "设置功控告警时间失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置功控告警时间无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                             TempData[i].Tips = "设置功控告警时间无回复";
                        }
                    }
                }
                AddItemsResoult("F49.功控告警时间", TempData);
                #endregion

                #region F58.终端自动保电参数
                //设置终端参数 F58.终端自动保电参数
                MessageAdd("下发控制参数：终端自动保电参数。", EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("00");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 58, SetData, RecData, MaxWaitSeconds_Write);
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
                                //MessageAdd("终端" + (i + 1) + "设置终端自动保电参数失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                 TempData[i].Tips = "设置终端自动保电参数失败";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置终端自动保电参数无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                             TempData[i].Tips = "设置终端自动保电参数无回复";
                        }
                    }
                }
                AddItemsResoult("F58.终端自动保电参数", TempData);
                #endregion
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
