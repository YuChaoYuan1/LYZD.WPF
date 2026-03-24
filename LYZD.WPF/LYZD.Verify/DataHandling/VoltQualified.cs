using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.DataHandling
{
    /// <summary>
    /// 电压合格率统计
    /// </summary>
    public class VoltQualified : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {

            ResultNames = new string[] { "日冻结A相电压越上限时间", "日冻结A相电压越下限时间", "日冻结A相电压监测时间", "日冻结B相电压越上限时间", "日冻结B相电压越下限时间", "日冻结B相电压监测时间", "日冻结C相电压越上限时间", "日冻结C相电压越下限时间", "日冻结C相电压监测时间", "月冻结A相电压越上限时间", "月冻结A相电压越下限时间", "月冻结A相电压监测时间", "月冻结B相电压越上限时间", "月冻结B相电压越下限时间", "月冻结B相电压监测时间", "月冻结C相电压越上限时间", "月冻结C相电压越下限时间", "月冻结C相电压监测时间", "结论" };
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

                if (Xub == 220)
                    SetData_698_jiaocai("0601374030020002041209601207D01208CA12086600", "设置电压合格率参数");
                else if (Xub == 100)
                    SetData_698_jiaocai("06013740300200020412044c12038d1203ff1203d100", "设置电压合格率参数");
                else
                    SetData_698_jiaocai("06013740300200020412027612020d12024e12023400", "设置电压合格率参数");
                SetData_698_jiaocai("07013850040500512131020100", "删除日冻结关联列1");
                SetData_698_jiaocai("07013950040500512132020100", "删除日冻结关联列2");
                SetData_698_jiaocai("07013A50040500512133020100", "删除日冻结关联列3");
                SetData_698_jiaocai("07013B50060500512131020200", "删除月冻结关联列1");
                SetData_698_jiaocai("07013C50060500512132020200", "删除月冻结关联列2");
                SetData_698_jiaocai("07013D50060500512133020200", "删除月冻结关联列3");

                SetData_698_jiaocai("07013E5004070001030203120001512131020112003E0203120001512132020112003E0203120001512133020112003E00", "配置日冻结关联对象属性列表");
                SetData_698_jiaocai("07013F5006070001030203120001512131020212000C0203120001512132020212000C0203120001512133020212000C00", "配置月冻结关联对象属性列表");



                MessageAdd("升A,B,C正常电压",EnumLogType.流程信息);
                PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, 0, 0, 0, 0, 240, 120, 0, 240, 120);


                DateTime dttmp = Convert.ToDateTime("2017-11-30 23:57:50");
                SetTime_698(dttmp, 0);

                ResetTerimal_698(2);
                //ConnectLink2(false);

                MessageAdd("升A越上限电压，B相正常电压，C相越下限电压",EnumLogType.流程信息);
                PowerOn(OnMeterInfo.MD_UB * 1.086f, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB * 0.95f, 0, 0, 0, 0, 240, 120, 0, 240, 120);
                WaitTime("等待", 310);

                MessageAdd("升A,B,C正常电压",EnumLogType.流程信息);
                PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, 0, 0, 0, 0, 240, 120, 0, 240, 120);
                WaitTime("等待", 80);


                dttmp = Convert.ToDateTime("2017-12-01 23:59:50");
                SetTime_698(dttmp, 0);
                WaitTime("等待", 300);
                MessageAdd("升A越上限电压，B相正常电压，C相越下限电压",EnumLogType.流程信息);
                PowerOn(OnMeterInfo.MD_UB * 1.086f, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB * 0.95f, 0, 0, 0, 0, 240, 120, 0, 240, 120);

                WaitTime("等待", 430);
                MessageAdd("升A,B,C正常电压",EnumLogType.流程信息);
                PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, 0, 0, 0, 0, 240, 120, 0, 240, 120);

                WaitTime("等待，", 10);
                dttmp = Convert.ToDateTime("2017-12-31 23:59:50");
                SetTime_698(dttmp, 0);

                WaitTime("等待", 30);

                MessageAdd("读取日冻结电压越限数据",EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        if (Clfs== Core.Enum.WireMode.单相)
                        {
                            Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen(" 05 03 06 50 04 02 00 01 20 21 02 00 1C 07 E1 0C 02 00 00 00 01 00 21 31 02 01 00 ".Replace(" ", "")) + " 05 03 06 50 04 02 00 01 20 21 02 00 1C 07 E1 0C 02 00 00 00 01 00 21 31 02 01 00 ".Replace(" ", "") + "0110" + Talkers[i].Framer698.cOutRand;
                        }
                        else
                            Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen(" 05 03 06 50 04 02 00 01 20 21 02 00 1C 07 E1 0C 02 00 00 00 03 00 21 31 02 01 00 21 32 02 01 00 21 33 02 01 00 ".Replace(" ", "")) + " 05 03 06 50 04 02 00 01 20 21 02 00 1C 07 E1 0C 02 00 00 00 03 00 21 31 02 01 00 21 32 02 01 00 21 33 02 01 00 ".Replace(" ", "") + "0110" + Talkers[i].Framer698.cOutRand;

                        setData[i] = Talkers[i].Framer698.ReadData_jiaocai(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                if (Clfs == Core.Enum.WireMode.单相)
                {
                    BaseVerifyUnit("日冻结A相电压越上限时间", 13, RecData, 6, 4);   //6/1440    8/1440  4/1440
                    BaseVerifyUnit("日冻结A相电压越下限时间", 14, RecData, 1, 0);             //1/1440
                    BaseVerifyUnit("日冻结A相电压监测时间", 10, RecData, 1440, 1440);
                }
                else
                {
                    BaseVerifyUnit("日冻结A相电压越上限时间", 15, RecData, 6, 4);   //6/1440    8/1440  4/1440
                    BaseVerifyUnit("日冻结A相电压越下限时间", 16, RecData, 1, 0);             //1/1440
                    BaseVerifyUnit("日冻结A相电压监测时间", 12, RecData, 1440, 1440);
                }
                if (Clfs== Core.Enum.WireMode.三相四线)
                {
                    BaseVerifyUnit("日冻结B相电压越上限时间", 20, RecData, 0, 0);
                    BaseVerifyUnit("日冻结B相电压越下限时间", 21, RecData, 1, 0);
                    BaseVerifyUnit("日冻结B相电压监测时间", 17, RecData, 1440, 1440);
                }
                if (Clfs != Core.Enum.WireMode.单相)
                {
                    BaseVerifyUnit("日冻结C相电压越上限时间", 25, RecData, 0, 0);
                    BaseVerifyUnit("日冻结C相电压越下限时间", 26, RecData, 6, 4);
                    BaseVerifyUnit("日冻结C相电压监测时间", 22, RecData, 1440, 1440);
                }


                MessageAdd("读取月冻结电压越限数据",EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        if (Clfs == Core.Enum.WireMode.单相)
                        {
                            Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen(" 05 03 07 50 06 02 00 01 20 21 02 00 1C 07 E2 01 01 00 00 00 01 00 21 31 02 02  00 ".Replace(" ", "")) + " 05 03 07 50 06 02 00 01 20 21 02 00 1C 07 E2 01 01 00 00 00 01 00 21 31 02 02  00  ".Replace(" ", "") + "0110" + Talkers[i].Framer698.cOutRand;
                        }
                        else
                        {
                            Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen(" 05 03 07 50 06 02 00 01 20 21 02 00 1C 07 E2 01 01 00 00 00 03 00 21 31 02 02 00 21 32 02 02 00 21 33 02 02 00 ".Replace(" ", "")) + " 05 03 07 50 06 02 00 01 20 21 02 00 1C 07 E2 01 01 00 00 00 03 00 21 31 02 02 00 21 32 02 02 00 21 33 02 02 00  ".Replace(" ", "") + "0110" + Talkers[i].Framer698.cOutRand;
                        }

                        setData[i] = Talkers[i].Framer698.ReadData_jiaocai(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                if (Clfs == Core.Enum.WireMode.单相)
                {
                    BaseVerifyUnit("月冻结A相电压越上限时间", 13, RecData, 14, 12);// 20/44660    
                    BaseVerifyUnit("月冻结A相电压越下限时间", 14, RecData, 2, 0);
                    BaseVerifyUnit("月冻结A相电压监测时间", 10, RecData, 44640, 44640);
                }
                else
                {
                    BaseVerifyUnit("月冻结A相电压越上限时间", 15, RecData, 14, 12);// 20/44660    
                    BaseVerifyUnit("月冻结A相电压越下限时间", 16, RecData, 2, 0);
                    BaseVerifyUnit("月冻结A相电压监测时间", 12, RecData, 44640, 44640);
                }
                if (Clfs == Core.Enum.WireMode.三相四线)

                {
                    BaseVerifyUnit("月冻结B相电压越上限时间", 20, RecData, 0, 0);
                    BaseVerifyUnit("月冻结B相电压越下限时间", 21, RecData, 2, 0);
                    BaseVerifyUnit("月冻结B相电压监测时间", 17, RecData, 44640, 44640);
                }
                if (Clfs != Core.Enum.WireMode.单相)
                {
                    BaseVerifyUnit("月冻结C相电压越上限时间", 25, RecData, 0, 0);
                    BaseVerifyUnit("月冻结C相电压越下限时间", 26, RecData, 14, 12);
                    BaseVerifyUnit("月冻结C相电压监测时间", 22, RecData, 44640, 44640);

                    
                }
                MessageAdd("正在恢复为当前时间", EnumLogType.提示信息);
                SetTime_698(DateTime.Now,0);
                MessageAdd("检定完成", EnumLogType.提示信息);
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }

        private void BaseVerifyUnit(string p_str_BaseMessage, int p_int_DataIndex, Dictionary<int, string[]> m_dic_RecDataTmp, Single sx, Single xx)
        {
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].StdData = xx + "~" + sx;
                        if (m_dic_RecDataTmp[i].Length > p_int_DataIndex)
                        {
                            TempData[i].Data = GetData(m_dic_RecDataTmp, i, p_int_DataIndex, EnumTerimalDataType.e_string) ;
                            // 判断数据是否正确
                            if (float.Parse(GetData(m_dic_RecDataTmp, i, p_int_DataIndex, EnumTerimalDataType.e_float)) > sx || float.Parse(GetData(m_dic_RecDataTmp, i, p_int_DataIndex, EnumTerimalDataType.e_float)) < xx)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i] .Tips= p_str_BaseMessage + "不正确";
                            }
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = p_str_BaseMessage + "不正确";
                        }
                    }
                    else
                    {
                        TempData[i].Resoult = "不合格";
                        TempData[i].Tips = p_str_BaseMessage + "无回复";
                    }
                }
            }
            AddItemsResoult(p_str_BaseMessage, TempData);
        }

    }
}
