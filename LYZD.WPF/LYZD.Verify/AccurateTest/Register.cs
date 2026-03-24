using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.AccurateTest
{
    //示值误差
    public class Register : VerifyBase
    {
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "试验前电量", "试验后电量", "电能值" ,"结论" };
            return true;
        }
        public override void Verify()
        {
            string[] FlUp = new string[MeterNumber];     //费率前电量
            string[] FlDown = new string[MeterNumber];  //费率后电量
            base.Verify();
            StartVerify698();
            ConnectLink(false);
            float testI = Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA);   //获取走字的电流
            //试验前电量
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    setData[i] = Talkers[i].Framer698.ReadData_jiaocai("05 01 01 00 00 04 00 00");
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data =Convert.ToDouble( GetData(RecData, i, 5, EnumTerimalDataType.e_string))/10000+",";
                        TempData[i].Data += Convert.ToDouble(GetData(RecData, i, 6, EnumTerimalDataType.e_string)) / 10000 + ",";
                        TempData[i].Data += Convert.ToDouble(GetData(RecData, i, 7, EnumTerimalDataType.e_string)) / 10000 + ",";
                        TempData[i].Data += Convert.ToDouble(GetData(RecData, i, 8, EnumTerimalDataType.e_string)) / 10000 + ",";
                        TempData[i].Data += Convert.ToDouble(GetData(RecData, i, 9, EnumTerimalDataType.e_string)) / 10000;

                        FlUp[i] = TempData[i].Data;
                    }
                }
            }

            AddItemsResoult("试验前电量", TempData);
            //尖峰平谷2341
            string[] TimeD = new string[4];
            #region 读取表当前日时段费率
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    setData[i] = Talkers[i].Framer698.ReadData_jiaocai("05 01 01 40 16 02 00 00");
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        TimeD = new string[4];

                        TimeD[0] = GetData(RecData, i, 5, EnumTerimalDataType.e_float) + ":" + GetData(RecData, i, 6, EnumTerimalDataType.e_float);
                        TimeD[1] = GetData(RecData, i, 8, EnumTerimalDataType.e_string) + ":" + GetData(RecData, i, 9, EnumTerimalDataType.e_float);
                        TimeD[2] = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + ":" + GetData(RecData, i, 12, EnumTerimalDataType.e_float);
                        TimeD[3] = GetData(RecData, i, 14, EnumTerimalDataType.e_float) + ":" + GetData(RecData, i, 15, EnumTerimalDataType.e_float);
                        TempData[i].Data= GetData(RecData, i, 5, EnumTerimalDataType.e_float) + ":" + GetData(RecData, i, 6, EnumTerimalDataType.e_float) + ","; ;
                        TempData[i].Data+= GetData(RecData, i, 8, EnumTerimalDataType.e_string) + ":" + GetData(RecData, i, 9, EnumTerimalDataType.e_float) + ",";
                        TempData[i].Data+= GetData(RecData, i, 11, EnumTerimalDataType.e_string) + ":" + GetData(RecData, i, 12, EnumTerimalDataType.e_float) +",";
                        TempData[i].Data+= GetData(RecData, i, 14, EnumTerimalDataType.e_float) + ":" + GetData(RecData, i, 15, EnumTerimalDataType.e_float);
                    }
                }
            }
            AddItemsResoult("获取表内时段", TempData);
            #endregion

            for (int J = 0; J < TimeD.Length; J++)
            {
                DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day +" "+ FormatTime(TimeD[J]));
                SetTime_698(dttmp, 0);

                
                //更改终端时间
                WaitTime("等待", 5);
     
                //升电流
                if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, testI, testI, testI, Cus_PowerYuanJian.H, PowerWay.正向有功, "1"))
                {
                    MessageAdd("升源失败,退出检定", EnumLogType.提示信息);
                    return;
                }
                WaitTime(string.Format($"正在进行费率{J}走字"), 120);
                if (Stop) return;

                if (PowerOff() == false)
                {
                    MessageAdd("控源失败,退出检定", EnumLogType.提示信息);
                    return;
                }
                WaitTime("关闭电流", 5);
            }

            //试验后电量
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    setData[i] = Talkers[i].Framer698.ReadData_jiaocai("05 01 01 00 00 04 00 00");
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data = Convert.ToDouble(GetData(RecData, i, 5, EnumTerimalDataType.e_string)) / 10000 + ",";
                        TempData[i].Data += Convert.ToDouble(GetData(RecData, i, 6, EnumTerimalDataType.e_string)) / 10000 + ",";
                        TempData[i].Data += Convert.ToDouble(GetData(RecData, i, 7, EnumTerimalDataType.e_string)) / 10000 + ",";
                        TempData[i].Data += Convert.ToDouble(GetData(RecData, i, 8, EnumTerimalDataType.e_string)) / 10000 + ",";
                        TempData[i].Data += Convert.ToDouble(GetData(RecData, i, 9, EnumTerimalDataType.e_string)) / 10000;
                        FlDown[i] = TempData[i].Data;
                    }
                }
            }
            AddItemsResoult("试验后电量", TempData);
            float[] zflcz = new float[MeterNumber]; // 费率电量插值
            float[] zflczAll = new float[MeterNumber]; // 总费率电量插值
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    zflcz[i] = 0f;
                    if (FlUp[i].Split(',').Length == FlDown[i].Split(',').Length)
                    {
                        for (int j = 1; j < FlUp[i].Split(',').Length; j++)
                        {
                            zflcz[i] += (float)(Convert.ToDouble(FlDown[i].Split(',')[j]) - Convert.ToDouble(FlUp[i].Split(',')[j]));
                        }
                    }

                    zflczAll[i]= (float)(Convert.ToDouble(FlDown[i].Split(',')[0]) - Convert.ToDouble(FlUp[i].Split(',')[0]));

                    TempData[i].Data = zflczAll[i].ToString() + ",  " + zflcz[i];
                    if (zflczAll[i]- zflcz[i]>-1 && zflczAll[i] - zflcz[i] < 1)
                    {
                        TempData[i].Tips = "误差合格";
                        TempData[i].Resoult = "合格";
                    }
                   else
                    {
                        TempData[i].Tips = "终端对时无回复";
                        TempData[i].Resoult = "不合格";
                    }
                }
            }
            AddItemsResoult("电能值", TempData);
   
        }


        public string FormatTime(string input)
        {
            string[] para = input.Split(':');
            if (para.Length < 3)
            {
                para = new string[3] { "0", "0", "0" };
                Array.Copy(input.Split(':'), para, input.Split(':').Length);
            }
            return string.Join(":", para.Select(P => int.TryParse(P, out int num) ? num.ToString("D2"):"00"));
        }
    }
}
