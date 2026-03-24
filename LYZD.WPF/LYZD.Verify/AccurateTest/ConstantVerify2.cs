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
using System.Threading;

namespace LYZD.Verify.AccurateTest
{

    //1：升源，只用电压
    //如果费用不是总，就要写时间进去-写对应方案费率时间
    //2:(启动标准表--重置电量)
    //3:启动脉冲计数
    //4:判断：如果手动录入起码，否则自动读取起码
    //5:升源-升电流
    //6：循环读取标准表电量
    //7：判断是时间还是电量到额定值--
    //8:关电流
    //9:再次读取误差版脉冲计数
    //10:在读取标准表
    //读取止码(对时)  ---不是总
    //11:对比判断
    //写时间(对时)---不是总
    //12：停止误差版
    class ConstantVerify2 : VerifyBase
    {
        StPlan_ZouZi curPoint = new StPlan_ZouZi();
        private float _StarandMeterD = 0F;
        //标准表累积电量
        private float _StarandMeterDl = 0F;
        int YgOrWg = 0;//有功还是无功
        private float[] arrayQima = new float[meterInfo.Length];
        private float[] arrayZima = new float[meterInfo.Length];

        //标准表累积电量
        private float _STMPower = 0F;
        private float _StarandMeterDlm = 0F;
        private float _StarandEnergy = 0f;

        private byte mc = 0x06;//脉冲计数

        protected string ItemKey
        {
            get
            {
                return curPoint.itemKey;
            }
        }
        /// <summary>
        /// 当前试验时间，仅针对预热，启动，潜动，走字，多功能有效
        /// </summary>
        public float NowMinute = 0;
        /// <summary>
        /// 走字试验[默认模式]
        /// </summary>
        /// <param name="ItemNumber"></param>
        public override void Verify()
        {
            //GlobalUnit.g_CurTestType = 3;
            //基类确定检定ID
            base.Verify();
            #region 初始化工作
            Cus_ZouZiMethod _ZZMethod;                                              //走字试验方法：标准表法或是头表法
            StPlan_ZouZi _curPoint = (StPlan_ZouZi)curPoint;
            FangXiang = _curPoint.PowerFangXiang;
            //把方案时间分转化为秒
            int _MaxTestTime = (int)(_curPoint.UseMinutes * 60);
            _ZZMethod = _curPoint.ZouZiMethod;
            //设置误差计算器参数
            string[] arrData = new string[0];    //数据数组
            string strDesc = string.Empty;       //描述信息

            CheckOver = false;
            //获取走字的电流
            float testI = Number.GetCurrentByIb(curPoint.xIb, OnMeterInfo.MD_UA);   //获取走字的电流
                                                                                    //初始化相关的电能表走字数据
            long constants = getStdConst(testI);

            StdGear(0x13,constants, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, testI, testI, testI);

            if (Stop)
            {
                return;
            }
            //MessageController.Instance.AddMessage("设置表开关!");
            //Helper.EquipHelper.Instance.SetMeterOnOff(Helper.MeterDataHelper.Instance.GetYaoJian());
            #endregion
            #region //第一步升压,不升电流,因为电表只有在 升源后，才能进行通讯
            MessageAdd("正在升源...", EnumLogType.提示信息);
            if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, 0, 0, 0, curPoint.PowerYj, curPoint.PowerFangXiang, curPoint.Glys))
            {
                MessageAdd("升源失败,退出检定", EnumLogType.提示信息);
                return;
            }
            if (Stop) return;
            #endregion

            #region //第二步 如果不是测量 总费率的话，需要 将电能表的当前时间，改成要测试的费率的对应时间
            if (_curPoint.FeiLv != Cus_FeiLv.总)
            {
                string time = _curPoint.StartTime;
                DateTime dt = DateTime.Now;
                int hh = int.Parse(_curPoint.StartTime.Substring(0, 2));
                int mm = int.Parse(_curPoint.StartTime.Substring(3, 2));
                dt = new DateTime(dt.Year, dt.Month, dt.Day, hh, mm, 0);
                bool[] t = MeterProtocolAdapter.Instance.WriteDateTime(dt);
                for (int i = 0; i < t.Length; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (!t[i])
                            return;
                    }
                }
            }
            #endregion

            #region // 第三步，如果不是 计读脉冲法 读取起始电量
            // 2:(启动标准表--重置电量)
       

            if (Stop) return;

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    switch (curPoint.PowerFangXiang)
                    {
                        case PowerWay.正向有功:
                            setData[i] = Talkers[i].Framer698.ReadData_jiaocai("05 01 01 00 10 02 00 00");
                            break;
                        case PowerWay.反向有功:
                            setData[i] = Talkers[i].Framer698.ReadData_jiaocai("05 01 01 00 20 02 00 00");
                            break;
                    }
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        ResultDictionary["起码"][i] = (Convert.ToDouble(GetData(RecData, i, 5, EnumTerimalDataType.e_float)) / 100).ToString();
                        arrayQima[i] = (float)(Convert.ToDouble(GetData(RecData, i, 5, EnumTerimalDataType.e_float)) / 100);
                    }
                }
            }
            RefUIData("起码");
            #endregion

            if (Stop) return;
            Thread.Sleep(3000);

            #region //第四步，发送走字指令，开始走字
            MessageAdd("启动标准表--重置电量", EnumLogType.提示信息);
            startStdEnergy(31);
            MessageAdd("启动误差板脉冲计数", EnumLogType.提示信息);
            StartWcb(mc, 0xff); //启动误差板脉冲计数
            #endregion

            #region //第五步，升走字电流
            if (Stop) return;
            MessageAdd("正在升电流...", EnumLogType.提示信息);

            if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, testI, testI, testI, curPoint.PowerYj, curPoint.PowerFangXiang, curPoint.Glys))
            {
                MessageAdd("升源失败,退出检定", EnumLogType.提示信息);
                return;
            }
            if (Stop) return;
            #endregion

            #region //第六步，控制 执行步骤
            string stroutmessage = string.Empty;        //外发消息
            DateTime startTime = DateTime.Now.AddSeconds(-15);   //检定开始时间,减掉源等待时间
            _StarandMeterDl = 0;                        //标准表电量
            DateTime lastTime = DateTime.Now.AddSeconds(-15);
            //_ZZMethod = Cus_ZouZiMethod.计读脉冲法;

            while (true)
            {
                Thread.Sleep(1000);
                if (Stop) return;
                int _PastTime = DateTimes.DateDiff(startTime);
                //处理跨天
                if (_PastTime < 0) _PastTime += 43200;      //如果当前PC为12小时制
                if (_PastTime < 0) _PastTime += 43200;      //24小时制
                if (_PastTime < 0)
                {
                    //处理人为修改时间
                    //跨度超过24小时，肯定是系统日期被修改
                    MessageAdd("系统时间被人为修改超过24小时，检定停止", EnumLogType.提示信息);
                    Stop = true;
                    return;
                }

                if (_ZZMethod == Cus_ZouZiMethod.计读脉冲法)
                {
                    #region
                    if (!IsDemo)
                    {
                        if (arrData.Length < MeterNumber)
                        {
                            arrData = new string[MeterNumber];
                        }
                        stError[] errors = ReadWcbData(GetYaoJian(), mc);
                        for (int i = 0; i < errors.Length; i++)
                        {
                            if (errors[i] == null) continue;
                            arrData[i] = errors[i].szError;
                        }
                    }
                    else
                    {
                        //      Helper.VerifyDemoHelper.Instance.GetPulseCount(ref arrData);
                    }
                    //等待所有表都跑完指定的脉冲
                    bool bOver = true;
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        TestMeterInfo _meterInfo = meterInfo[i];

                        if (!_meterInfo.YaoJianYn)
                            continue;
                        if (arrData[i] == null || arrData[i].Length == 0 || int.Parse(arrData[i]) < (int)_curPoint.UseMinutes)
                        {
                            bOver = false;
                            break;
                        }

                    }
                    CheckOver = bOver;
                    NowMinute = float.Parse(arrData[FirstIndex]);
                    //外发检定消息
                    stroutmessage = string.Format("方案设置脉冲：{0}个，第一要检表位已经收到：{1}个", _curPoint.UseMinutes, arrData[FirstIndex]);
                    #endregion
                }
                else if (_ZZMethod == Cus_ZouZiMethod.标准表法 || _ZZMethod == Cus_ZouZiMethod.校核常数)
                {
                    #region
                    if (!IsDemo)
                    {
                        //记录标准表电量
                        float pSum = 0;
                        if (IsYouGong)
                        {
                            pSum = Math.Abs(EquipmentData.StdInfo.P);
                        }
                        else
                        {
                            pSum = Math.Abs(EquipmentData.StdInfo.Q);
                        }

                        float pastSecond = (float)(DateTime.Now - lastTime).TotalMilliseconds;
                        lastTime = DateTime.Now;
                        _StarandMeterDl += pastSecond * pSum / 3600 / 1000 / 1000;
                        //同步记录（读）脉冲数
                        if (arrData.Length < MeterNumber)
                        {
                            arrData = new string[MeterNumber];
                        }
                        stError[] errors = ReadWcbData(GetYaoJian(), mc);
                        for (int i = 0; i < errors.Length; i++)
                        {
                            if (errors[i] == null) continue;
                            arrData[i] = errors[i].szError;

                        }
                        //再算一次电量
                        pastSecond = (int)(DateTime.Now - lastTime).TotalMilliseconds;
                        lastTime = DateTime.Now;
                        _StarandMeterDl += pastSecond * pSum / 3600 / 1000 / 1000;
                    }
                    else
                    {
                        //模拟电量
                        _StarandMeterDl = _PastTime * OnMeterInfo.MD_UB * testI / 3600000F;
                        //同步模拟脉冲数
                    }
                    //如果电量达到设定，停止
                    if (_StarandMeterDl >= _curPoint.UseMinutes - 0.002)
                    {
                        CheckOver = true;
                    }
                    //如果脉冲数达到设定，也停止
                    float flt_C = 0;
                    if (arrData != null && arrData.Length > 0)
                    {
                        float.TryParse(arrData[FirstIndex], out flt_C);
                    }
                    flt_C = flt_C / OnMeterInfo.GetBcs()[0];
                    if (flt_C >= _curPoint.UseMinutes - 0.002)
                    {
                        CheckOver = true;
                    }
                    //外发检定消息
                    NowMinute = _StarandMeterDl;
                    stroutmessage = string.Format("方案设置走字电量：{0}度，已经走字：{1}度", _curPoint.UseMinutes, _StarandMeterDl.ToString("F5"));
                    #endregion
                }
                else
                {
                    #region
                    //走字试验法
                    if (_PastTime >= _MaxTestTime)
                    {
                        CheckOver = true;
                    }
                    NowMinute = _PastTime / 60F;
                    stroutmessage = string.Format("方案设置走字时间：{0}分，已经走字：{1}分", _curPoint.UseMinutes, Math.Round(NowMinute, 2));
                    #endregion
                }
                #region 更新数据
                //缓存数据
                for (int i = 0; i < MeterNumber; i++)
                {
                    TestMeterInfo _meterInfo = meterInfo[i];
                    if (!_meterInfo.YaoJianYn)
                    {
                        continue;
                    }

                    //"表脉冲", "标准表脉冲"
                    if (arrData != null && arrData.Length > i)
                    {
                        ResultDictionary["表脉冲"][i] = arrData[i];
                    }
                    ResultDictionary["标准表脉冲"][i] = (_StarandMeterDl * _meterInfo.GetBcs()[0]).ToString("F2");
                }
                RefUIData("表脉冲");
                RefUIData("标准表脉冲");
                //MessageController.Instance.AddMonitorMessage(EnumMonitorType.ErrorBoard, string.Join(",0|", arrData));
                #endregion
                MessageAdd(stroutmessage, EnumLogType.提示信息);
                if (CheckOver)
                {
                    break;
                }
            }
            #endregion

            #region //第七步升压,不升电流,因为电表只有在 升源后，才能进行通讯
            MessageAdd("正在升源...", EnumLogType.提示信息);
            if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, 0, 0, 0, curPoint.PowerYj, curPoint.PowerFangXiang, curPoint.Glys))
            {
                MessageAdd("升源失败,退出检定", EnumLogType.提示信息);
                return;
            }
            #endregion

            #region //第八步 读终止时的电量或脉冲数

            if (!IsDemo)
            {
                if (arrData.Length < MeterNumber)
                {
                    arrData = new string[MeterNumber];
                }
                stError[] errors = ReadWcbData(GetYaoJian(), mc);
                bool[] twoerrors = new bool[MeterNumber];
                twoerrors.Fill(false);
                for (int i = 0; i < errors.Length; i++)
                {
                    if (errors[i] == null) continue;
                    arrData[i] = errors[i].szError;
                    if (arrData[i] == "0") twoerrors[i] = false;
                }

                if (Array.IndexOf(twoerrors, true) != -1)
                {
                    errors = ReadWcbData(GetYaoJian(), mc);
                    for (int i = 0; i < errors.Length; i++)
                    {
                        if (errors[i] == null) continue;
                        if (twoerrors[i]) continue;
                        arrData[i] = errors[i].szError;
                       
                    }
                }

                int xb = 9;
                if (!IsYouGong) xb = 10;
                float[] bzbdl = ReadStmEnergy();

                if(bzbdl!=null && bzbdl.Length>xb && bzbdl[xb] != 0)
                {
                    _StarandMeterDl = bzbdl[xb];
                }
                if (_StarandMeterDl < 0)
                {
                    _StarandMeterDl = Math.Abs(_StarandMeterDl);
                }

            }
            else
            {
                //      Helper.VerifyDemoHelper.Instance.GetPulseCount(ref arrData);
            }
            if (Stop)
            {
                return;
            }
            if (_ZZMethod != Cus_ZouZiMethod.计读脉冲法)
            {
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        switch (curPoint.PowerFangXiang)
                        {
                            case PowerWay.正向有功:
                                setData[i] = Talkers[i].Framer698.ReadData_jiaocai("05 01 01 00 10 02 00 00");
                                break;
                            case PowerWay.反向有功:
                                setData[i] = Talkers[i].Framer698.ReadData_jiaocai("05 01 01 00 20 02 00 00");
                                break;
                        }
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            ResultDictionary["止码"][i] = (Convert.ToDouble(GetData(RecData, i, 5, EnumTerimalDataType.e_float)) / 100).ToString();
                            arrayZima[i] = (float)(Convert.ToDouble(GetData(RecData, i, 5, EnumTerimalDataType.e_float)) / 100);
                        }
                    }
                }
            }
            RefUIData("止码");
            //缓存数据
            for (int i = 0; i < MeterNumber; i++)
            {
                TestMeterInfo _meterInfo = meterInfo[i];
                if (!_meterInfo.YaoJianYn)
                {
                    continue;
                }
                //"表脉冲", "标准表脉冲"
                if (arrData != null && arrData.Length > i)
                {
                    ResultDictionary["表脉冲"][i] = arrData[i];
                    ResultDictionary["标准表脉冲"][i] = (_StarandMeterDl * _meterInfo.GetBcs()[0]).ToString("F2");
                }
                float flt_QZC = arrayZima[i] - arrayQima[i];
                ResultDictionary["表码差"][i] = flt_QZC.ToString("F2");
            }
            RefUIData("表脉冲");
            RefUIData("表码差");
            RefUIData("标准表脉冲");
            #endregion
            StopWcb(mc, 0xff); //停止误差板计数
            #region //第十一步，计算误差
            try
            {
                //缓存数据
                for (int i = 0; i < MeterNumber; i++)
                {
                    TestMeterInfo meter = meterInfo[i];
                    if (!meter.YaoJianYn) continue;

                    int con = meter.GetBcs()[0];

                    double a = (Convert.ToDouble(ResultDictionary["表脉冲"][i]) - Convert.ToDouble(ResultDictionary["标准表脉冲"][i])) / Convert.ToDouble(ResultDictionary["标准表脉冲"][i]);
                    if (a > -1 && a < 1)
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
                RefUIData("结论");

            }
            catch
            {
                Stop = true;
                MessageAdd("计算走字误差时出现错误", EnumLogType.提示信息);
            }
            #endregion
        }

       

        protected override bool CheckPara()
        {
            string[] _Prams = Test_Value.Split('|');
            if (_Prams.Length < 8) return false;
            //StPlan_ZouZi curPoint = new StPlan_ZouZi();
            curPoint.FeiLv = (Cus_FeiLv)Enum.Parse(typeof(Cus_FeiLv), _Prams[5]);
            curPoint.FeiLvString = _Prams[5];
            curPoint.Glys = _Prams[2];
            curPoint.PowerFangXiang = (PowerWay)Enum.Parse(typeof(PowerWay), _Prams[0]);
            curPoint.PowerYj = (Cus_PowerYuanJian)Enum.Parse(typeof(Cus_PowerYuanJian), _Prams[1]);
            curPoint.xIb = _Prams[3];
            curPoint.ZouZiMethod = (Cus_ZouZiMethod)Enum.Parse(typeof(Cus_ZouZiMethod), _Prams[4]);
            string dufen = _Prams[6] + "度";
            curPoint.UseMinutes = float.Parse(_Prams[6]);
            if (_Prams[7].Trim() != "0")
            {
                curPoint.UseMinutes = float.Parse(_Prams[7]);
                dufen = _Prams[7] + "分";
            }
            curPoint.ZouZiPrj = new List<StPlan_ZouZi.StPrjFellv>() {
                new StPlan_ZouZi.StPrjFellv()
                {
                    FeiLv= (Cus_FeiLv)Enum.Parse(typeof(Cus_FeiLv), _Prams[5]),
                    StartTime="",
                    ZouZiTime=dufen
                }
            };
            curPoint.ZuHeWc = "0";


            bool Result = true;
            TestType testMethod = TestType.默认;
            string[] powerDirect = new string[1];
            //取当前检定方案中的所有功率方向
            powerDirect[0] = ((int)(PowerWay)Enum.Parse(typeof(PowerWay), _Prams[0])).ToString();
            //检测每一个检定方向下的费率
            for (int i = 0; i < powerDirect.Length; i++)
            {
                string[] feilv = new string[1];

                int zNum = 0;
                //取当前功率方向下的费率时段
                feilv[0] = ((int)((Cus_FeiLv)Enum.Parse(typeof(Cus_FeiLv), _Prams[5]))).ToString();
                //当走字方式为总与分同时做时，要求每一个方向只有一个总且第一个费率必须为总，
                if (testMethod == TestType.总与分费率同时做)
                {
                    if (feilv[0] != ((int)Cus_FeiLv.总).ToString())
                    {
                        MessageAdd("当走字方式为[" + testMethod.ToString() + "]时，第一个走字试验方案必须为[总]", EnumLogType.提示信息);
                        Result = false;
                        break;
                    }

                    for (int k = 0; k < feilv.Length; k++)
                    {

                        if (feilv[k] == ((int)Cus_FeiLv.总).ToString())
                            zNum++;
                        if (zNum > 1)
                        {
                            MessageAdd("当走字方式为[" + testMethod.ToString() + "]时，每一个功率方向允许有且仅允许有一个总费率方向方案", EnumLogType.提示信息);
                            return false;
                        }
                    }
                }
                else if (testMethod == TestType.自动检定总时段内的所有分费率)
                {
                    if (feilv[0] != ((int)Cus_FeiLv.总).ToString()) //第一个不为总
                    {
                        Result = false;
                        break;
                    }
                }
                else
                {
                    Result = true;
                }
            }
            //StPlan_ZouZi _curPoint = (StPlan_ZouZi)CurPlan;
            if (curPoint.ZouZiMethod == Cus_ZouZiMethod.基本走字法 && MeterHelper.Instance.TestMeterCount < 3)
            {
                MessageAdd("基本走字法至少要求有三块以上被检表!", EnumLogType.提示信息);
                return false;
            }

            ResultNames = new string[] { "起码", "止码", "表码差", "表脉冲", "标准表脉冲", "误差", "结论", "不合格原因" };
            if (curPoint.PowerFangXiang == PowerWay.正向无功 || curPoint.PowerFangXiang == PowerWay.反向无功)
            {
                YgOrWg = 1;
                mc = 0x07;
            }
            return Result;

        }


        public long getStdConst(float i)
        {
            long constants = 0;
            if (constants == 0)
            {
                if (i < 0) constants = (long)(2 * Math.Pow(10, 9));
                else if (i < 1) constants = (long)(2 * Math.Pow(10, 8));
                else if (i < 10) constants = (long)(2 * Math.Pow(10, 7));
                else constants = (long)(2 * Math.Pow(10, 6));

            }
            return constants;
        }

    }


    enum TestType
    {
        默认 = 0,
        /// <summary>
        /// 总与分同时做是指当有总费率及其它分费率在一起时。先读取总费率的起码，然后走分费率。最后再读取总
        /// 费率的止码。
        /// </summary>
        总与分费率同时做 = 1,
        /// <summary>
        /// 此种走字方式是指只走总时读取所有分费率起码，总走完后再读取所有分费率止码。
        /// 此种方式应用于总走字时间较长，同时跨几个分费率的情况
        /// </summary>
        自动检定总时段内的所有分费率 = 2
    }

    /// <summary>
    /// 走字误差计算器--标准表法
    /// </summary>
    class ZZError
    {
        Core.Struct.ErrorLimit ErrLimit;
        public ZZError(Core.Struct.ErrorLimit wuChaDeal)
        {
            ErrLimit = wuChaDeal;
        }

        public  MeterBase SetWuCha(params float[] arrNumber)
        {
            //第一个参数：是否是总费率: 1-是, 0-不是
            //第二个参数：起码,止码,标准表[头表]电量,标准表[头表]相对误差
            if (arrNumber[0] > 0)
            {
                return SetWuCha1(arrNumber[1], arrNumber[2], arrNumber[3]);
            }
            else
                return SetWuCha2(arrNumber[1], arrNumber[2], arrNumber[3]);
        }

        /// <summary>
        /// 标准表法/电能表常数试验法误差计算
        /// </summary>
        /// <param name="sPower">被检表起码</param>
        /// <param name="ePower">被检表止码</param>
        /// <param name="stmPower">标准表[头表]电量</param>
        /// <param name="starandWCOr">标准表[头表]相对误差</param>
        /// <returns></returns>
        public MeterBase SetWuCha1(float sPower, float ePower, float stmPower)
        {
            MeterZZError rst = new MeterZZError();

            //修正标准表电量
            stmPower = (float)Math.Round(stmPower, 5);

            rst.PowerStart = sPower;
            rst.PowerEnd = ePower;

            rst.WarkPower = (ePower - sPower).ToString("f5");

            float err = (ePower - sPower) - stmPower;
            rst.PowerError = err.ToString("F4");
            if (rst.PowerError != "0.00" && err > 0) //误差大于零且不等于0时，误差加+符号
                rst.PowerError = "+" + rst.PowerError;
            //计算方法参见JJG56-1999 4.4.2
            if (err <= ErrLimit.UpLimit && err > ErrLimit.DownLimit)
                rst.Result = Const.合格;
            else
                rst.Result = Const.不合格;

            return rst;
        }

        /// <summary>
        /// 计算分费率走字误差
        /// </summary>
        /// <param name="sPower">被检表起码</param>
        /// <param name="ePower">被检表止码</param>
        /// <param name="stmPower">总码差</param>
        /// <param name="MeterPrecision">电能表计度器小数位数</param>
        /// <returns></returns>
        private MeterZZError SetWuCha2(float sPower, float ePower, float stmPower)
        {
            MeterZZError rst = new MeterZZError();

            //暂时将MeterPrecision改为2，因为这个一直都为0，实际上645规定电量为2位小数
            int MeterPrecision = 2;

            rst.PowerStart = sPower;
            rst.PowerEnd = ePower;
            rst.WarkPower = Math.Round(ePower - sPower, 5).ToString("F5");

            // |费率码差-总码差| * 10 ^ MeterPrecision <=2 参见JJG596-1999 4.4.4
            float err = (float)((float.Parse(rst.WarkPower) - stmPower) * Math.Pow(10, MeterPrecision));

            rst.PowerError = Math.Round(err, 3).ToString("F3");
            if (Math.Abs(err) < 2)
            {
                rst.Result = Const.合格;
            }
            else
            {
                rst.Result = Const.不合格;
            }

            return rst;
        }
    }
}
