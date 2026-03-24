using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.Core.Helper;
using LYZD.Core.Model.DnbModel.DnbInfo;
using LYZD.Core.Model.Meter;
using LYZD.Core.Struct;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LYZD.Verify.AccurateTest
{
    /// <summary>
    /// 需量示值误差 ---还不能用，因为终端的需量清空命令和连接命令什么不一样啊
    /// </summary>
    public class MaxDemand : VerifyBase
    {

        private PowerWay powerWay = PowerWay.正向有功;
        private int demandPeriod = 15;//需量周期
        private int slipTimes = 1;//滑差时间
        private int slipPage; //滑差次数
        private float XIB = 1f;   //电流倍数
        private float ib;
        string name = "0.1Ib";

        /// <summary>
        /// IR46电流倍数
        /// </summary>
        //public float XIB_IR46 { get; set; }
        public override void Verify()
        {
            //add yjt 20220305 新增日志提示
            MessageAdd("需量示值误差试验检定开始...", EnumLogType.提示与流程信息);

            //开始检定
            base.Verify();
            StartVerify698();
            ConnectLink(false);

            int maxMinute = demandPeriod + slipTimes * slipPage;
            int maxTime = maxMinute * 60;
            for (int i = 0; i < MeterNumber; i++)
            {
                ResultDictionary["电流"][i] = name;
            }
            RefUIData("电流");


            if (Stop) return;

            if (Stop) return;
            SetTime_698(DateTime.Now, 0);
            if (Stop) return;
            //第一步：清空需量
            if (Stop) return;
            MessageAdd("开始清空需量......", EnumLogType.提示信息);

            //07 01 14 43 00 06 00 00 00
            //08 05 01 00 20 00 02 00 00
            //05 01 01 10 10 02 00 00
            //05 01 01 40 16 02 00 00  /当前套日时段表
            for (int i = 0; i < MeterNumber; i++)
            {
                if (!meterInfo[i].YaoJianYn) continue;
                //int ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                //Talkers[i].Framer698.sAPDU = "100007 01 14 43 00 06 00 00 " + "0110" + Talkers[i].Framer698.cOutRand;//读取版本信息
                Talkers[i].Framer698.sAPDU = "07 01 14 43 00 06 00 00 00";
                setData[i] = Talkers[i].Framer698.ReadData_jiaocai(Talkers[i].Framer698.sAPDU.Replace(" ", ""));

            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "0";
                    if (TalkResult[i] == 0)
                    {
                        string sTmp = GetData(RecData, i, 4, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 21, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 22, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 23, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 24, EnumTerimalDataType.e_string);
                        if (GetData(RecData, i, 4, EnumTerimalDataType.e_string) != "00")
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "需量清零失败";
                        }
                        TempData[i].Data = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                    }
                    else
                    {
                        TempData[i].Resoult = "不合格";
                        TempData[i].Tips = "通讯无回复";
                    }
                }
            }
            AddItemsResoult("需量清零", TempData);

            if (Stop) return;
            MessageAdd("开始做最大需量......", EnumLogType.提示信息);

            if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, ib * XIB, ib * XIB, ib * XIB, Cus_PowerYuanJian.H, powerWay, "1.0"))
            {
                MessageAdd("升源失败,退出检定", EnumLogType.提示信息);
                return;
            }

            if (Stop) return;
            ErrorLimit limit = new ErrorLimit();
            for (int i = 0; i < MeterNumber; i++)
            {
                TestMeterInfo meter = meterInfo[i];
                if (!meter.YaoJianYn) continue;
                float curLevel = Number.SplitKF(meter.MD_Grane, IsYouGong);
                float errorLevel = GetErrorLevel(curLevel);
                limit = new ErrorLimit
                {
                    IsSTM = false,
                    MeterLevel = curLevel,

                    UpLimit = errorLevel,
                    DownLimit = -errorLevel
                };
                ResultDictionary["误差上限"][i] = limit.UpLimit.ToString();
                ResultDictionary["误差下限"][i] = limit.DownLimit.ToString();
            }
            RefUIData("误差上限");
            RefUIData("误差下限");

            StartTime = DateTime.Now;
            float standMeterP = 0;
            int PastMinute = 0;
            float[] meterXL = new float[MeterNumber];    //被检表最大需量
            MessageAdd("开始检定", EnumLogType.提示信息);
            //标准表功率
            while (true)
            {
                if (Stop) return;
                Thread.Sleep(2000);

                int pastTime = (int)VerifyPassTime;
                int tempMinute = (int)(VerifyPassTime / 55);
                if (VerifyPassTime >= maxTime)
                {
                    MessageAdd("检定时间达到方案预定时间，检定完成", EnumLogType.提示信息);
                    break;
                }
                else
                {
                    pastTime *= 100;
                    pastTime /= 60;
                    float curPorcess = pastTime / 100F;
                    //App.CUS.NowMinute = curPorcess;
                    MessageAdd(string.Format("{0}周期误差检定需要{1}分，已经进行{2}分", powerWay.ToString(), maxMinute, curPorcess), EnumLogType.提示信息);
                }
                //有功/无功功率
                float stmPower = EquipmentData.StdInfo.P; //标准有功功率
                if (!IsYouGong)
                    stmPower = EquipmentData.StdInfo.Q;   //标准无功功率

                //这个需要刷新到界面
                standMeterP = (float)(Math.Truncate(stmPower / 1000 * 1000000) / 1000000);//Math.Abs(curStandMeterP) / 1000;

                //每一次分钟读取一次需量数据
                if (tempMinute > PastMinute)
                {
                    PastMinute = tempMinute;
                    byte curPD = (byte)powerWay;
                    float[] dv1 = ReadDemand();
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        TestMeterInfo meter = meterInfo[i];
                        if (!meter.YaoJianYn) continue;
                        float readXl = float.Parse(dv1[i].ToString("#0.000000"));

                        if (meterXL[i] < readXl)
                            meterXL[i] = readXl;    //记录下需量
                        ResultDictionary["实际需量"][i] = meterXL[i].ToString();
                        ResultDictionary["标准需量"][i] = standMeterP.ToString();
                    }
                    RefUIData("实际需量");
                    RefUIData("标准需量");
                }
            }
            float[] demandValue = ReadDemand();

            for (int i = 0; i < MeterNumber; i++)
            {
                TestMeterInfo meter = meterInfo[i];
                if (!meter.YaoJianYn) continue;

                float readXl = float.Parse(demandValue[i].ToString("#0.0000"));
                if (meterXL[i] < readXl)
                    meterXL[i] = readXl;    //记录下需量
                ResultDictionary["实际需量"][i] = meterXL[i].ToString();
            }
            RefUIData("实际需量");
            for (int i = 0; i < MeterNumber; i++)
            {
                TestMeterInfo meter = meterInfo[i];
                if (!meter.YaoJianYn) continue;
                MeterDgn result = SetWuCha(limit, meterXL[i], standMeterP.ToString());
                ResultDictionary["需量误差"][i] = result.Value;
                ResultDictionary["结论"][i] = result.Result;
                if (ResultDictionary["结论"][i] == Const.不合格)
                {
                    NoResoult[i] = "误差超出误差限";
                }


                RefUIData("需量误差");
                RefUIData("结论");
            }

            MessageAdd("清空一次需量", EnumLogType.提示信息);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (!meterInfo[i].YaoJianYn) continue;
                //int ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                //Talkers[i].Framer698.sAPDU = "100007 01 14 43 00 06 00 00 " + "0110" + Talkers[i].Framer698.cOutRand;//读取版本信息
                Talkers[i].Framer698.sAPDU = "07 01 14 43 00 06 00 00 00";
                setData[i] = Talkers[i].Framer698.ReadData_jiaocai(Talkers[i].Framer698.sAPDU.Replace(" ", ""));

            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "0";
                    if (TalkResult[i] == 0)
                    {
                        string sTmp = GetData(RecData, i, 4, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 21, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 22, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 23, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 24, EnumTerimalDataType.e_string);
                        if (GetData(RecData, i, 4, EnumTerimalDataType.e_string) != "00")
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "需量清零失败";
                        }
                        TempData[i].Data = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                    }
                    else
                    {
                        TempData[i].Resoult = "不合格";
                        TempData[i].Tips = "通讯无回复";
                    }
                }
            }
            AddItemsResoult("需量清零", TempData);
            float[] dem = ReadDemand();
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "0.000000";
                    TempData[i].Data = dem[i].ToString("#0.000000");
                    TempData[i].Resoult = "合格";
                }
            }
            AddItemsResoult("清零后读取需量", TempData);
        }

        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {

            //参数：电流，功率方向，需量周期，滑差时间，误差次数
            string[] tem = Test_Value.Split('|');
            ib = Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA);

            name = tem[0];

            switch (tem[0])
            {
                case "0.1Ib":
                    XIB = 0.1f;
                    break;
                case "1.0Ib":
                    XIB = 1.0f;
                    break;
                case "Imax":
                    XIB = Number.GetCurrentByIb("imax", OnMeterInfo.MD_UA) / ib;
                    break;
                default:
                    break;

            }
            powerWay = (PowerWay)Enum.Parse(typeof(PowerWay), tem[1]); //功率方向
            int.TryParse(tem[2], out demandPeriod);
            int.TryParse(tem[3], out slipTimes);
            int.TryParse(tem[4], out slipPage);
            //电流（imax，ib，0.1ib），误差上限，误差下限，结论
            ResultNames = new string[] { "电流", "误差上限", "误差下限", "标准需量", "实际需量", "需量误差", "结论" };
            return true;
        }

        /// <summary>
        /// 根据南网标准计算需量示值误差限
        /// </summary>
        /// <param name="meterLevel">电能表等级</param>
        /// <returns></returns>
        private float GetErrorLevel(float meterLevel)
        {
            //海南计算公式:表等级+0.05*额定功率/实际功率
            //标准功率

            WireMode clfs = (WireMode)Enum.Parse(typeof(WireMode), OnMeterInfo.MD_WiringMode);
            float strandPower = CalculatePower(OnMeterInfo.MD_UB, ib, clfs, Cus_PowerYuanJian.H, "1.0", IsYouGong);
            //负载功率
            float current = Number.GetCurrentByIb("imax", OnMeterInfo.MD_UA);
            if (OnMeterInfo.MD_JJGC == "IR46")
            {
                if (XIB == 1F || XIB == 2F) current = ib;
                if (XIB == 10F) current = ib * XIB;
            }
            else
            {
                if (XIB == 0.1F) current = ib * XIB;
                if (XIB == 1F) current = ib;
            }
            float currentPower = CalculatePower(OnMeterInfo.MD_UB, current, clfs, Cus_PowerYuanJian.H, "1.0", IsYouGong);
            return meterLevel + 0.05F * strandPower / currentPower;


        }

        /// <summary>
        /// 计算电能表最大需量误差
        /// </summary>
        /// <param name="arrNumber">电表实际需量</param>
        /// <returns>多功能数据结构体 MeterDgn </returns>
        public MeterDgn SetWuCha(ErrorLimit ErrLimit, float arrNumber, string OtherData)
        {
            //计算标准功率
            float starndP = float.Parse(OtherData);

            string strStarndP = (float.Parse(OtherData)).ToString("F6");
            if (starndP > 0)
                strStarndP = "+" + strStarndP;

            OtherData = "";

            float xlError = Number.GetRelativeWuCha(arrNumber, starndP); //误差值
            string err = xlError.ToString("F2");
            if (xlError > 0)
                err = "+" + err;

            MeterDgn ret = new MeterDgn();
            ret.Value = string.Format("{0}|{1}|{2}", strStarndP, arrNumber, err);
            if (arrNumber != 0f && Math.Abs(xlError) <= ErrLimit.UpLimit + 0.05F * starndP / arrNumber)
            {
                ret.Result = Const.合格;
            }
            else
            {
                ret.Result = Const.不合格;
            }
            ret.Value = xlError.ToString();
            return ret;
        }

        //05 01 01 10 10 02 00 00
        //08 05 01 0f 20 04 02 00 00
        private float[] ReadDemand()
        {
            for (int i = 0; i < MeterNumber; i++)
            {
                if (!meterInfo[i].YaoJianYn) continue;
                Talkers[i].Framer698.sAPDU = "0501011010020000";//读取版本信息
                setData[i] = Talkers[i].Framer698.ReadData_jiaocai(Talkers[i].Framer698.sAPDU);

            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            float[] demandValue = new float[MeterNumber];
            for (int i = 0; i < MeterNumber; i++)
            {
                if (!meterInfo[i].YaoJianYn) continue;
                //float StdPQ = EquipmentData.StdInfo.P;

                if (TalkResult[i] == 0)
                {
                    if (RecData[i].Length < 10)
                    {
                        MessageAdd("终端【" + (i + 1) + "】1类数据无回复！", EnumLogType.错误信息);
                        //TempData[i].Resoult = "不合格";
                    }
                    else
                    {
                        demandValue[i] = float.Parse(GetData(RecData, i, 5, EnumTerimalDataType.e_float)) / 10000;
                    }
                }
                else
                {
                    MessageAdd("终端【" + (i + 1) + "】1类数据无回复！", EnumLogType.错误信息);
                    //TempData[i].Resoult = "不合格";
                }
            }
            return demandValue;


        }
    }
}
