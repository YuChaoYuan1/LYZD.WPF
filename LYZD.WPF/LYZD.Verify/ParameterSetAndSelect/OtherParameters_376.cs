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
    /// 其他参数
    /// </summary>
    public class OtherParameters_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "F65.定时发送1类数据任务设置", "F66.定时发送2类数据任务设置", "F67.定时发送1类数据任务启动/停止设置", "F68.定时发送2类数据任务启动/停止设置", "F91.终端地理位置信", "F6.组地址信息", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();

                #region F65.定时发送1类数据任务设置
                //设置终端参数 F65.定时发送1类数据任务设置
                MessageAdd("下发控制参数：定时发送1类数据任务。",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("0100420920510701020000020000000800");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 65, SetData, RecData, MaxWaitSeconds_Write);
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
                                //MessageAdd( "终端" + (i + 1) + "设置定时发送1类数据回复否认！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置定时发送1类数据回复否认";
                            }
                        }
                        else
                        {
                            //MessageAdd( "终端" + (i + 1) + "设置定时发送1类数据无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置定时发送1类数据无回复";
                        }

                    }
                }
                AddItemsResoult("F65.定时发送1类数据任务设置", TempData);
                #endregion

                #region F66.定时发送2类数据任务设置
                //设置终端参数 F66.定时发送2类数据任务设置
                MessageAdd("下发控制参数：定时发送2类数据任务。",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("0100000001210701020000010600000406");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 66, SetData, RecData, MaxWaitSeconds_Write);
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
                                //MessageAdd( "终端" + (i + 1) + "设置定时发送2类数据回复否认！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置定时发送2类数据回复否认";
                            }
                        }
                        else
                        {
                            //MessageAdd( "终端" + (i + 1) + "设置定时发送2类数据无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置定时发送2类数据无回复";
                        }

                    }
                }
                AddItemsResoult("F66.定时发送2类数据任务设置", TempData);
                #endregion

                #region F67.定时发送1类数据任务启动/停止设置
                //设置终端参数 F67.定时发送1类数据任务启动/停止设置
                MessageAdd("下发控制参数：定时发送1类数据任务启动/停止。",EnumLogType.流程信息);

                SetData = UsefulMethods.ConvertStringToBytes("55");
                ;
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 67, SetData, RecData, MaxWaitSeconds_Write);
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
                                //MessageAdd( "终端" + (i + 1) + "设置定时发送1类数据任务启动/停止回复否认！", EnumLogType.错误信息);
                               TempData[i].Resoult="不合格";
                                TempData[i].Tips = "设置定时发送1类数据任务启动/停止回复否认";
                            }
                        }
                        else
                        {
                            //MessageAdd( "终端" + (i + 1) + "设置定时发送1类数据任务启动/停止无回复！", EnumLogType.错误信息);
                           TempData[i].Resoult="不合格";
                            TempData[i].Tips = "设置定时发送1类数据任务启动/停止无回复";
                        }
                    }
                }
                AddItemsResoult("F67.定时发送1类数据任务启动/停止设置", TempData);
                #endregion

                #region F68.定时发送2类数据任务启动/停止设置
                //设置终端参数 F68.定时发送2类数据任务启动/停止设置
                MessageAdd("下发控制参数：定时发送2类数据任务启动/停止。",EnumLogType.流程信息);

                SetData = UsefulMethods.ConvertStringToBytes("55");
                ;
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 68, SetData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "Fn1";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data= GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                            {
                                //MessageAdd( "终端" + (i + 1) + "设置定时发送2类数据任务启动/停止回复否认！", EnumLogType.错误信息);
                               TempData[i].Resoult="不合格";
                                TempData[i].Tips = "设置定时发送2类数据任务启动/停止回复否认";
                            }
                        }
                        else
                        {
                            //MessageAdd( "终端" + (i + 1) + "设置定时发送2类数据任务启动/停止无回复！", EnumLogType.错误信息);
                           TempData[i].Resoult="不合格";
                            TempData[i].Tips = "设置定时发送2类数据任务启动/停止无回复";
                        }

                    }
                }
                AddItemsResoult("F68.定时发送2类数据任务启动/停止设置", TempData);
                #endregion

                #region F91.终端地理位置信
                //设置终端参数 F91.终端地理位置信息
                MessageAdd("下发控制参数：终端地理位置信息。",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("22292316013726543900");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 91, SetData, RecData, MaxWaitSeconds_Write);
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
                                //MessageAdd( "终端" + (i + 1) + "设置终端地理位置信息回复否认！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置终端地理位置信息回复否认";
                            }
                        }
                        else
                        {
                            //MessageAdd( "终端" + (i + 1) + "设置终端地理位置信息无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置终端地理位置信息无回复";
                        }

                    }
                }
                AddItemsResoult("F91.终端地理位置信", TempData);
                #endregion

                if (OnMeterInfo.MD_TerminalType== "集中器I")
                {
                     MessageAdd("下发控制参数：组地址信息。",EnumLogType.流程信息);

                    if (VerifyConfig.AddressLen== 2)
                        SetData = UsefulMethods.ConvertStringToBytes("01000200030004000500060007000800");
                    else
                        SetData = UsefulMethods.ConvertStringToBytes("0100000002000000030000000400000005000000060000000700000008000000");
                    TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 6, SetData, RecData, MaxWaitSeconds_Write);

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
                                    //MessageAdd( "终端" + (i + 1) + "设置组地址信息回复否认！", EnumLogType.错误信息);
                                   TempData[i].Resoult="不合格";
                                    TempData[i].Tips = "设置组地址信息回复否认";
                                }
                            }
                            else
                            {
                                //MessageAdd("终端" + (i + 1) + "设置组地址信息无回复！", EnumLogType.错误信息);
                               TempData[i].Resoult="不合格";
                                TempData[i].Tips = "设置组地址信息无回复";
                            }

                        }
                    }
                    //MessageHelper.Instance.AddVerifyData(0, m_int_SchemeId, m_str_ItemName, "F6.组地址信息", m_str_ParameterID, 6, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);
                    AddItemsResoult("F6.组地址信息", TempData);

                }
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
