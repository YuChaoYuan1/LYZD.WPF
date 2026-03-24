using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.DataCollect
{
    /// <summary>
    /// HPLC载波测试
    /// </summary>
    public class HPLCarrierCommunication : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "载波698正向有功电能示值", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {

                //首先要把发送的数据端口改到载波板的端口--端口26
                //虚拟表需要监听端口26
                //载波发送如何数据应该都是往26号端口发送--然后虚拟表监听到26号端口数据-解析--返回到26号端口
                //不对--载波是电压发送的--发送的到的应该都是我们那个载波板--载波板通过26号端口接收数据




                base.Verify();
                base.StartVerify698();
                ConnectLink(false);
                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");
                SetData_698_No("07 01 13 60 00 83 00 12 00 03 00  ", "删除采集档案");

                bool[] YaoJian = new bool[MeterNumber];
                //开始保存检定标志
                for (int i = 0; i < MeterNumber; i++)   //保存检定的标志，并且把所有表位置为不要检定--因为载波只能一个一个来
                {
                    YaoJian[i] = meterInfo[i].YaoJianYn;
                    meterInfo[i].YaoJianYn = false;
                }

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (YaoJian[i])
                    {
                        meterInfo[i].YaoJianYn = true;
                        string strzbaddr = "000000000099";//载波地址
                        if (DeviceControl.IsHaveDevice("载波信号控制板"))    //判断是否有载波信号控制板，用于载波断电使用
                        {
                            DeviceControl.SetZBGZDYContrnl(false);
                            WaitTime("正在关闭载波供电", 30);
                            DeviceControl.SetZBGZDYContrnl(true);
                            WaitTime("正在恢复载波供电", 30);
                        }
         


                        ControlVirtualMeter("Cmd,DLS,5000,4000,1000,1000,1000,1000");
                        ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
                        SetTime_698(DateTime.Now, 0);

                        SetData_698_No("07 01 0E 60 00 7F 00 02 04 12 00 03 02 0a 55 07 05 " + strzbaddr + " 16 06 16 03 51 F2 09 02 01 09 02 00 00 11 04 11 00 16 03 12 08 98 12 00 0F 02 04 55 07 05 00 00 00 00 00 00 09 06 00 00 00 00 00 00 12 00 01 12 00 01 01 00 00", "下发698载波电表档案");
                        //SetData_698_No("07010E60007F000204120003020a5507050000000000101603160351F20902010902000011041100160312089812000F02045507050000000000000906000000000000120001120001010000","下发698载波电表档案");
                        //SetData_698_No("07013760007F000204120001020A5507050000000000011606160351F2010201090600000000000011041100160312089812000F02045507050000000000000 906000000000000120001120001010000", "下发一块采集档案");

                        WaitTime("等待组网", 300);

                        MessageAdd("代理电能量数据",EnumLogType.流程信息, true);
                        for (int j = 0; j < MeterNumber; j++)
                        {
                            if (meterInfo[j].YaoJianYn)
                            {
                                Talkers[j].Framer698.sAPDU = "09 01 02 00 64 01 07 05  " + strzbaddr + " 00 00 01 00 10 02 00 00".Replace(" ", "");
                                setData[j] = Talkers[j].Framer698.ReadData(Talkers[j].Framer698.sAPDU);//代理读取电能量类数据
                            }
                        }
                        TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                        for (int j = 0; j < MeterNumber; j++)
                        {
                            if (meterInfo[j].YaoJianYn)
                            {
                                if (TalkResult[j] == 0)
                                {
                                    TempData[i].StdData = "500000,125000,125000,125000,125000";
                                    TempData[i].Data = GetData(RecData, i, 8, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 9, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 10, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 12, EnumTerimalDataType.e_string);
                                    if (TempData[i].Data != "500000,125000,125000,125000,125000")
                                        TempData[i].Resoult = "不合格";
                                }
                                else
                                {
                                    TempData[i].Tips = "无回复！";
                                    TempData[i].Resoult = "不合格";
                                }
                            }
                        }
                        AddItemsResoult("载波698正向有功电能示值", TempData);

                        //for (int j = 0; j < MeterNumber; j++)
                        //{
                        //    if (meterInfo[j].YaoJianYn)
                        //    {
                        //        if (TalkResult[j] == 0)
                        //        {
                        //            TempData[i].Data = GetData(RecData, i, 15, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 16, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 18, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 19, EnumTerimalDataType.e_string);
                        //            TempData[i].StdData = "400000,100000,100000,100000,100000";
                        //            if (TempData[i].Data != TempData[i].StdData)
                        //                TempData[i].Resoult = "不合格";
                        //        }
                        //        else
                        //        {
                        //            TempData[i].Tips = "无回复！";
                        //            TempData[i].Resoult = "不合格";
                        //        }
                        //    }
                        //}
                        //AddItemsResoult("载波698反向有功电能示值", TempData);

                        SetData_698_No("07 01 13 60 00 83 00 12 00 03 00  ", "删除采集档案");
                        //ResetTerimal_698(2);
                        //ConnectLink(false);

                        if (!DeviceControl.IsHaveDevice("载波信号控制板"))    //如果没有载波信号控制板，那么就需要重启终端
                        {
                            ResetTerimal_698(2);
                            ConnectLink(false);
                        }



                        meterInfo[i].YaoJianYn = false;
                    }
                }
                //恢复要检标记
                for (int i = 0; i < MeterNumber; i++)
                {
                    meterInfo[i].YaoJianYn = YaoJian[i];
                }
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
