using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify
{

    //MeterNumber    m_int_TerminalCount
    //meterInfo[i].YaoJianYn   CLOU_Model.TerminalModels.TerminalInfos[i + 1].IsVerify
    //TalkResult   m_int_TalkResult  
    //RecData  m_dic_RecData
    //setData  arr_byt_setData
    // CLOU_Comm.Enums.
    //VerifyControler.Instance.
    //TerminalProtocol.Encryption   CLOU_TerminalProtocal.CLEncryption
    //Resoult    m_str_Conclusions
    //ResultDictionary[""]   m_str_VerifyDatas

    //  StartVerify698();
    public class TTTTTTTTTT : VerifyBase
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




                for (int i = 0; i < MeterNumber; i++)
                {
                    if (!meterInfo[i].YaoJianYn) continue;
                    ResultDictionary["结论"][i] = Resoult[i];
                }
                RefUIData("结论");
                MessageAdd("检定完成",EnumLogType.提示信息);
            }
            catch (Exception ex)
            {   
                //MessageAdd\((.*)true\)
                //MessageShow\((.*)\);
                //MessageAdd($1,EnumLogType.流程信息);

                MessageAdd("正在读取终端1类数据", EnumLogType.错误信息);
                MessageAdd(ex.ToString(), EnumLogType.流程信息);
                MessageAdd("正在读取终端1类数据",EnumLogType.流程信息);
            }
        }
    }
    //68 8A 00 8A 00 68 41 18  05 09 61 02 01 F0 00 00 01 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00 00 00 00 55 53 18 03 00  7F 16                  
    public class Test : VerifyBase
    {
        public static string str { get; set; }
        public override void Verify()
        {
            base.Verify();

            SetData = Core.Function.UsefulMethods.ConvertStringToBytes("0000");
            TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 12, SetData, RecData, 5000);


            MessageAdd("读取电表当前数据",EnumLogType.错误信息);

            DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:50");
            SetTime_698(dttmp, 0);
            Talkers[0].Framer698.sAPDU = "05 03 18 60 12 03 00 07 " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 04 00 20 2A 02 00 00 00 10 02 00 00 20 00 02 00 00 20 01 02 00 00  ".Replace(" ", "");
                    // Talkers[i].Framer698.sAPDU = " 09 01 0A 00 78 02 07 05 20 16 01 20 00 01 00 3C 01 00 10 02 00 07 05 20 16 01 20 00 02 00 3C 01 00 10 02 00 00".Replace(" ", "");
            setData[0] = Talkers[0].Framer698.ReadData(Talkers[0].Framer698.sAPDU);//代理读取电能量类数据
  
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, 20000);

            Talkers[0].Framer698.sAPDU = "05 03 12 60 12 03 00 05 " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 03 02 07 05 00 00 00 00 00 01 07 05 90 00 00 00 00 02 05 00 20 2A 02 00 00 60 40 02 00 00 60 41 02 00 00 60 42 02 00 01 50 04 02 00 03 20 21 02 00 00 10 02 00 00 20 02 00 00 ".Replace(" ", "");
            setData[0] = Talkers[0].Framer698.ReadData(Talkers[0].Framer698.sAPDU);//代理读取电能量类数据

            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, 20000);



            Talkers[0].Framer698.sAPDU = "05 03 12 60 12 03 00 05 " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 03 02 07 05 00 00 00 00 00 01 07 05 00 00 00 00 00 02 05 00 20 2A 02 00 00 00 10 02 00 00 20 00 02 00 00 20 01 02 00 00 ".Replace(" ", "");
            setData[0] = Talkers[0].Framer698.ReadData(Talkers[0].Framer698.sAPDU);//代理读取电能量类数据

            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, 20000);
            return;






            //61是端口号1--要改成62
            SetData = Core.Function.UsefulMethods.ConvertStringToBytes("020001000100010200000000000000000000000004090000000000000002000200611E010000000000000000000000040900000000000000");
            TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 10, SetData, RecData, 20000);
            return;


            StartVerify();

            //TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 25, RecData, MaxWaitSeconds_Read485);
            TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 1, 25, RecData, MaxWaitSeconds_Read485);
           

          //TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 2, 27, RecData, MaxWaitSeconds_Read485);

            TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 2, RecData, MaxWaitSeconds_Read485);

            //1.读取终端电压、电流、功率、功率因数
            MessageAdd("正在读取终端1类数据",EnumLogType.流程信息);
            TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 1, 25, RecData, MaxWaitSeconds_Read485);
            return;






            //ResetTerimal_698(3);
            //ResetTerimal_698(2);
            //ResetTerimal_698(1);

            SetData_698_No("07 01 13 60 00 83 00 12 00 03 00  ", "删除采集档案");


            ResetTerimal(2);
            ResetTerimal(1);

            //设置脉冲输出
            SetPulseOutput(GetYaoJian(), 0x03, 0.584f, 4, 2100, 0.584f, 4, 2100);
            //停止脉冲输出
            SetPulseOutputStop(GetYaoJian());
            return;

            string[] t = new string[MeterNumber];
            string[] s = new string[MeterNumber];
            string[] r = new string[MeterNumber];
            Random rd = new Random();  //无参即为使用系统时钟为种子
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Data = rd.Next(1, 10).ToString();
                        TempData[i].StdData= rd.Next(1, 1000).ToString();
                        TempData[i].Resoult = "合格";
                        TempData[i].Tips = "提示信息";
                        if (rd.Next(1, 1000) > 500)
                        {
                            TempData[i].Resoult = "不合格";
                        }
                    }
                    TempData[i].Data = "检定数据";      
                    TempData[i].StdData = "Fn1";
                    TempData[i].Resoult = "合格";
                    TempData[i].Tips = "无回复";
                }
                //AddItemsResoult("测试" + r[0], t, s, r);
                AddItemsResoult("检定数据",TempData);    
            }

            //RefUIData("结论");

            return;
            #region 载波测试

            ControlVirtualMeter("Cmd,DLS,5000,4000,1000,1000,1000,1000");
            ControlVirtualMeter("Cmd,Set,220,220,220,50,50,50,0,0,0,1,0");
            string strzbaddr = "000000000010";
            SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
            SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");
            SetData_698_No("07 01 13 60 00 83 00 12 00 03 00  ", "删除采集档案");
            SetData_698_No("07 01 0E 60 00 7F 00 02 04 12 00 03 02 0a 55 07 05 " + strzbaddr + " 16 03 16 03 51 F2 09 02 01 09 02 00 00 11 04 11 00 16 03 12 08 98 12 00 0F 02 04 55 07 05 00 00 00 00 00 00 09 06 00 00 00 00 00 00 12 00 01 12 00 01 01 00 00", "下发698载波电表档案");
            WaitTime("等待组网", 240);


            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.sAPDU = "09 01 02 00 64 01 07 05  " + strzbaddr + " 00 00 01 00 10 02 00 00".Replace(" ", "");
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//代理读取电能量类数据
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, 60000);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        string sTmp = GetData(RecData, i, 8, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 9, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 10, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 12, EnumTerimalDataType.e_string);
                        //if (sTmp != "500000,125000,125000,125000,125000")
                        //Resoult[i] = "不合格";
                        //ResultDictionary[""][i] = sTmp + "|500000,125000,125000,125000,125000";
                    }
                    else
                    {
                        // MessageAdd("终端" + (i + 1) + "对时无回复！", EnumLogType.错误信息);
                        //Resoult[i] = "不合格";
                    }
                }
            }
            // RefUIData("载波698正向有功电能示值");







            //byte[][] setData = new byte[MeterNumber][];
            byte[] s2;
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.sAPDU = ("09 01 02 00 64 01 07 05  " + strzbaddr + " 00 00 01 00 10 02 00 00").Replace(" ", "");
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//代理读取电能量类数据
                }
            }
            //int[] TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, 120000);

            #endregion










            ResetTerimal(0x01);

            StringBuilder sb = new StringBuilder();

            #region 模拟表操作
            // 设置模拟表电流
            ControlVirtualMeter("SCu000000000000000");
            // 设置模拟表断相信息
            sb.Clear();
            sb.Append("SQT");
            sb.Append("2015-10-12 15:04:00,");
            sb.Append("2015-10-12 15:04:00,");
            for (int i = 0; i < 6; i++)
            {
                sb.Append("0008,");
            }
            ControlVirtualMeter(sb.ToString());
            // 设置模拟表状态信息
            ControlVirtualMeter("SZT0000000000000000000000000000");
            // 设置模拟表电量
            sb.Clear();
            sb.Append("DLS");
            for (int i = 0; i < 8; i++)
            {
                sb.Append("00010");

            }
            ControlVirtualMeter(sb.ToString());
            ControlVirtualMeter("QLT");
            ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,45,45,45,1,0");
            ControlVirtualMeter("Cmd,SZT,0100,0200,0300,0400,0500,0600,0700");
            ControlVirtualMeter("Cmd,MeterTimeCheck,1");
            #endregion

            int[] a = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 2, 28, RecData, 60000);

            ResetTerimal(0x01);
            if (Stop) return;


            SetData_698_No("07 01 0D 60 00 7F 00 02 04 12 00 0A 02 0A 55 07 05 90 00 00 00 00 01 16 03 16 02 51 F2 01 02 01 09 02 00 00 11 04 11 00 16 01 12 08 98 12 00 0F 02 04 55 07 05 00 00 00 00 00 00 09 06 00 00 00 00 00 00 12 00 01 12 00 01 01 00 00 ", "下发645电表档案");
            if (Stop) return;

            SetData_698_No("07 01 0E 60 00 7F 00 02 04 12 00 0B 02 0A 55 07 05 90 00 00 00 00 02 16 03 16 02 51 F2 01 02 01 09 02 00 00 11 04 11 00 16 01 12 08 98 12 00 0F 02 04 55 07 05 00 00 00 00 00 00 09 06 00 00 00 00 00 00 12 00 01 12 00 01 01 00 00 ", "下发645电表档案");
            if (Stop) return;




            ControlVirtualMeter("Cmd,RunFlag,0");
            ControlVirtualMeter("BTL2400");
            ControlVirtualMeter("Cmd,boolIsReturn,1");
            ControlVirtualMeter("Cmd,ProtocalType,1");
            ControlVirtualMeter("Cmd,MeterTimeCheck,0");
            //Dictionary<int, string[]> RecData = new Dictionary<int, string[]>();     // 通讯返回数据内容
            int MaxWaitSeconds_Write = 20000;//参数设置，查询，读内存区数据最大延时
            //int[] TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 89, RecData, MaxWaitSeconds_Write);

            //string[] ResultDictionary[""] = new string[MeterNumber];
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        if (RecData[i].Length == 5)
                        {
                            //Resoult[i] = "合格";
                            ResultDictionary[""][i] = GetData(RecData, i, 3, EnumTerimalDataType.e_string) + "|" + GetData(RecData, i, 4, EnumTerimalDataType.e_string);

                            if (true)
                                meterInfo[i].Address = GetData(RecData, i, 3, EnumTerimalDataType.e_string) + GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                            else
                                meterInfo[i].Address = GetData(RecData, i, 3, EnumTerimalDataType.e_string) + (Convert.ToInt32(GetData(RecData, i, 4, EnumTerimalDataType.e_string), 16)).ToString().PadLeft(5, '0');
                        }
                        else
                        {
                            // MessageAdd("读终端地址" + (i + 1) + "返回不正确！",EnumLogType.错误信息);
                        }
                    }
                    else
                    {
                        // MessageAdd("读终端地址" + (i + 1) + "无回复！",EnumLogType.错误信息);
                    }

                }
            }


            //string[] ResultDictionary[""] = new string[0];        // 检定数据数组
            //for (int i = 0; i < MeterNumber; i++)
            //{
            //    ResultDictionary[""][i] = GetData(RecData, i, 3, Core.Enum.EnumTerimalDataType.e_string) + "|" + GetData(RecData, i, 4, Core.Enum.EnumTerimalDataType.e_string);

            //    if (true)
            //      meterInfo[i].MD_PostalAddress = GetData(RecData, i, 3, Core.Enum.EnumTerimalDataType.e_string) + GetData(RecData, i, 4, Core.Enum.EnumTerimalDataType.e_string);
            //    else
            //     meterInfo[i].MD_PostalAddress = GetData(RecData, i, 3, Core.Enum.EnumTerimalDataType.e_string) + (Convert.ToInt32(GetData(RecData, i, 4, Core.Enum.EnumTerimalDataType.e_string), 16)).ToString().PadLeft(5, '0');


            //}

            string str_TimeDiff = ((DateTime.Now - DateTime.Now).TotalMinutes + 0).ToString();
            ControlVirtualMeter("Tim" + str_TimeDiff);

            // 终端对时
            // MessageAdd("终端对时",EnumLogType.错误信息);
            TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 31, 0.ToString(), RecData, MaxWaitSeconds_Write);

            //string[] str = new string[12];
            //RefUIData("检定节点编号", "列名", str);//str数值
            //MessageAdd("获得结论", false); //提示信息
            //MessageAdd("获得结论",EnumLogType.提示信息); //提示信息--显示在最下面的，临时查看
            if (Stop) return;
        }

        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "检定数据", "分项结论", "结论" };
            return true;
        
        }
    }

}
