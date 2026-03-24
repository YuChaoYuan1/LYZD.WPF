using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.Core.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace LYZD.ViewModel.CheckController
{

    /// <summary>
    ///设备控制命令封装，给检定基类调用。。包括了控制设备时候需要使用的方法
    ///这类中调用的方法是在设备类中调用反射的方法
    /// </summary>
    public class DeviceControlS
    {

        #region 功率源
        /// <summary>
        /// 源控制结构体
        /// </summary>
        public struct PowerParam
        {
            /// <summary>
            /// 测量方式
            /// </summary>
            public WireMode clfs;
            /// <summary>
            /// 标定电压(V)
            /// </summary>
            public float UB { get; set; }
            /// <summary>
            /// 标定电流(A)
            /// </summary>
            public float IB { get; set; }
            /// <summary>
            /// 最大电流(A)(用于防止电压过高损坏台体)
            /// </summary>
            public float IMax { get; set; }
            /// <summary>
            /// A相电压(V)
            /// </summary>
            public float Ua { get; set; }
            /// <summary>
            /// B相电压(V)
            /// </summary>
            public float Ub { get; set; }
            /// <summary>
            /// C相电压(V)
            /// </summary>
            public float Uc { get; set; }
            /// <summary>
            /// A相电流(A)
            /// </summary>
            public float Ia { get; set; }
            /// <summary>
            /// B相电(A)
            /// </summary>
            public float Ib { get; set; }
            /// <summary>
            /// C相电流(A)
            /// </summary>
            public float Ic { get; set; }
            /// <summary>
            /// 元件
            /// </summary>
            public Cus_PowerYuanJian Element { get; set; }
            /// <summary>
            /// A相电压角度
            /// </summary>
            public float UaPhi { get; set; }
            /// <summary>
            /// B相电压角度
            /// </summary>
            public float UbPhi { get; set; }
            /// <summary>
            /// C相电压角度
            /// </summary>
            public float UcPhi { get; set; }
            /// <summary>
            /// A相电流角度
            /// </summary>
            public float IaPhi { get; set; }
            /// <summary>
            /// B相电流角度
            /// </summary>
            public float IbPhi { get; set; }
            /// <summary>
            /// C相电流角度
            /// </summary>
            public float IcPhi { get; set; }
            /// <summary>
            /// 频率HZ
            /// </summary>
            public float Freq { get; set; }
            /// <summary>
            /// 是否逆相序
            /// </summary>
            public Cus_PowerPhase IsNxx { get; set; }
            /// <summary>
            /// 是否是对标
            /// </summary>
            public bool DuiBiao { get; set; }
            /// <summary>
            /// 是否是潜动
            /// </summary>
            public bool IsQiangDong { get; set; }
            /// <summary>
            /// 角度值
            /// </summary>
            public float Cos { get; set; }
        }



        /// <summary>
        /// 快速升源
        /// </summary>
        public bool PowerOn()
        {
            return EquipmentData.DeviceManager.PowerOn();
        }

        /// <summary>
        /// 关源
        /// </summary>
        public bool PowerOff()
        {
            return EquipmentData.DeviceManager.PowerOff();
        }


        /// <summary>
        /// 通用控源
        /// 测量方式:GlobalUnit.Clfs
        /// </summary>
        /// <param name="Ub">电压V</param>
        /// <param name="Ib">电流A</param>
        /// <param name="ele">元件</param>
        /// <param name="glys">功率因素,如果是反向请在前加负号</param>
        /// <param name="bYouGong">是否是有功</param>
        /// <returns>操作结果</returns>
        public bool PowerOn(float Ua, float Ub, float Uc, float Ia, float Ib, float Ic, Cus_PowerYuanJian
 ele, PowerWay glfx, string glys,float pl=50)
        {
            //if (IsDemo) return true;
            //执行
            PowerWay tmp = glfx;
            if (PowerWay.第一象限无功 == glfx || PowerWay.第二象限无功 == glfx)
            {
                tmp = PowerWay.正向无功;
            }
            else if (PowerWay.第三象限无功 == glfx || PowerWay.第四象限无功 == glfx)
            {
                tmp = PowerWay.反向无功;
            }
            return PowerOn(Ua, Ub, Uc, Ia, Ib, Ic, ele, pl, glys, Cus_PowerPhase.正相序 , tmp);
        }

        /// <summary>
        /// 特殊检定升源
        /// </summary>
        /// <param name="Ua">A相电压V</param>
        /// <param name="Ub">B相电压V</param>
        /// <param name="Uc">C相电压V</param>
        /// <param name="Ia">A相电流A</param>
        /// <param name="Ib">B相电流A</param>
        /// <param name="Ic">C相电流A</param>
        /// <param name="glys">功率因素，反向请加-</param>
        /// <param name="nxx">是否逆相序</param>
        /// <returns></returns>
        public bool PowerOn(float Ua, float Ub, float Uc, float Ia, float Ib, float Ic, Cus_PowerYuanJian
 yuanjian, float feq, string glys, Cus_PowerPhase nxx, PowerWay glfx)
        {
            PowerParam param = GetPowerPara(Ua, Ub, Uc, Ia, Ib, Ic, yuanjian, glys, glfx, nxx, feq);
            //param.DuiBiao = isDuiBiao;
            if (glfx == PowerWay.反向有功 || glfx == PowerWay.反向无功)
            {
                if (glys.IndexOf('-') == -1)
                    glys = "-" + glys;
            }
            return PowerOn(param, glys, glfx);
        }

        /// <summary>
        /// 升源
        /// </summary>
        /// <param name="tagPara">控源参数</param>
        /// <returns>结果</returns>
        public bool PowerOn(PowerParam tagPara, string strGlys, PowerWay powerWay)
        {
            //进行参数检查
            float maxU = Max(tagPara.Ua, tagPara.Ub, tagPara.Uc);
            float maxI = Max(tagPara.Ia, tagPara.Ib, tagPara.Ic);


            

            float ProtectedVoltage = float.Parse(EquipmentData.LastCheckInfo.ProtectedVoltage.Trim('V'));  //保护电压
            float ProtectedCurrent = float.Parse(EquipmentData.LastCheckInfo.ProtectedCurrent.Trim('A')); //保护电流
            if (maxU > ProtectedVoltage)
            {
                Utility.Log.LogManager.AddMessage("当前检定项目的最大电压:" + maxU + "超过保护电压:" + ProtectedVoltage + "已停止升源", Utility.Log.EnumLogSource.设备操作日志, Utility.Log.EnumLevel.Warning);
                return false;
            }
            if (maxI > ProtectedCurrent)
            {
                Utility.Log.LogManager.AddMessage("当前检定项目的最大电流:" + maxU + "超过保护电流:" + ProtectedCurrent + "已停止升源", Utility.Log.EnumLogSource.设备操作日志, Utility.Log.EnumLevel.Warning);
                return false;
            }

            if (!EquipmentData.DeviceManager.Devices.ContainsKey(Device.DeviceName.功率源))
            {
                return false;
            }

            Device.DeviceData device = EquipmentData.DeviceManager.Devices[Device.DeviceName.功率源][0];// EquipmentData.DeviceManager.Devices.FirstOrDefault(item => item.Model == "E_CL309");
            if (device==null) return false;
            bool result;
            if (device.Model!="ZH3001")
            {
                bool t=false;
                if (tagPara.IsNxx==Cus_PowerPhase.逆相序 || tagPara.IsNxx == Cus_PowerPhase.电流逆相序)
                {
                    t = true;
                }

                result = EquipmentData.DeviceManager.CL309PowerOn((int)tagPara.clfs, (int)powerWay, strGlys, tagPara.Ua, tagPara.Ub, tagPara.Uc, tagPara.Ia,
                                          tagPara.Ib, tagPara.Ic, (int)tagPara.Element, tagPara.Freq, t);
            }
            else
            {
                 result = EquipmentData.DeviceManager.PowerOn(GetClfs(tagPara.clfs, powerWay), tagPara.Ua, tagPara.Ub, tagPara.Uc, tagPara.Ia,
                                          tagPara.Ib, tagPara.Ic, tagPara.UaPhi, tagPara.UbPhi, tagPara.UcPhi, tagPara.IaPhi,
                                          tagPara.IbPhi, tagPara.IcPhi, tagPara.Freq, 1);
            }
            Thread.Sleep(300);
            return result;
        }
        /// <summary>
        ///  设置谐波
        /// </summary>
        /// <param name="ua"> 谐波设置对象 Ua ；1-设置 ；0-不设置</param>
        /// <param name="ub">谐波设置对象 Ub ；1-设置 ；0-不设置</param>
        /// <param name="uc">谐波设置对象 Uc ；1-设置 ；0-不设置</param>
        /// <param name="ia">谐波设置对象 ia ；1-设置 ；0-不设置</param>
        /// <param name="ib">谐波设置对象 ib ；1-设置 ；0-不设置</param>
        /// <param name="ic">谐波设置对象 ic ；1-设置 ；0-不设置</param>
        /// <param name="harmonicType"> 特殊谐波：0表示正常谐波；1表示方形波，2表示尖顶波，3表示次谐波，4表示奇次谐波。</param>
        /// <returns></returns>
        public bool SetPowerHarmonic(string ua, string ub, string uc, string ia, string ib, string ic, int harmonicType, int ID = 0)
        {
            return EquipmentData.DeviceManager.ZH3001SetPowerHarmonic(ua,ub,uc,ia,ib,ic,harmonicType);
        }

        private int GetClfs(WireMode Clfs, PowerWay pd)
        {
            bool IsYouGong = (pd == PowerWay.正向有功) || (pd == PowerWay.反向有功);
            int clfs = (int)Clfs;

            if (Clfs == WireMode.单相)//单相台统一划分为三相四线
                clfs = 0;

            clfs += 2;              //先保证后面对齐
            if (clfs < 4)           //处理前面没有对齐部分
            {
                if (clfs == 3)
                {
                    if (IsYouGong) clfs--;
                }
                else
                {
                    clfs--;
                    if (IsYouGong) clfs--;
                }
            }
            return clfs;
        }

        #region CL309方法


        #endregion


        #endregion

        #region 误差板
        /// <summary>
        /// 启动误差版
        /// </summary>
        /// <param name="ControlType">控制类型（00：正向有功，01：正向无功，02：反向有功，03：反向无功，04：日计时，05：需量， 06：正向有功脉冲计数， 07：正向无功脉冲计数， 08：反向有功脉冲计数，09 反向无功脉冲计数）</param>
        /// <param name="MeterNo">表位号，FF为广播</param>
        /// <returns></returns>
        public bool StartWcb(int ControlType,byte MeterNo,int ID=0)
        {
             return EquipmentData.DeviceManager.StartWcb(ControlType, MeterNo, ID);
        }
        /// <summary>
        /// 停止误差版
        /// </summary>
        /// <param name="ControlType">控制类型（00：正向有功，01：正向无功，02：反向有功，03：反向无功，04：日计时，05：需量， 06：正向有功脉冲计数， 07：正向无功脉冲计数， 08：反向有功脉冲计数，09 反向无功脉冲计数）</param>
        /// <param name="MeterNo">表位号，FF为广播</param>
        /// <returns></returns>
        public bool StopWcb(int ControlType, byte MeterNo, int ID = 0)
        {
            return EquipmentData.DeviceManager.StopWcb(ControlType, MeterNo, ID);
        }


        /// <summary>
        /// 设置标准常数
        /// </summary>
        /// <param name="ControlType">控制类型（00-标准电能误差相关； 01-标准时钟日记时需量）</param>
        /// <param name="constant">常数</param>
        /// <param name="magnification">放大倍数-2就是缩小100倍</param>
        /// <param name="EpitopeNo">表位编号</param>
        public bool  SetStandardConst(int ControlType, int constant, int magnification, byte EpitopeNo, int ID = 0)
        {

            return EquipmentData.DeviceManager.SetStandardConst(ControlType, constant, magnification, EpitopeNo, ID);
        }

        /// <summary>
        /// 设置被检常数
        /// </summary>
        /// <param name="ControlType">控制类型(6组被检:00-有功(含正向、反向，以下同,01-无功(正向、反向，以下同),04-日计时,05-需量）</param>
        /// <param name="constant">常数</param>
        /// <param name="magnification">放大倍数-2就是缩小100倍</param>
        /// <param name="qs">圈数</param>
        /// <param name="EpitopeNo">表位编号</param>
        public bool SetTestedConst(int ControlType, int constant, int magnification,int qs, byte EpitopeNo, int ID = 0)
        {
            return EquipmentData.DeviceManager.SetTestedConst(ControlType, constant, magnification, qs, EpitopeNo, ID);
        }

        /// <summary>
        ///  控制表位继电器 
        /// </summary>
        /// <param name="contrnlType">控制类型--1开启-2关闭</param>
        /// <param name="EpitopeNo">表位编号0xFF为广播</param>
        public void ControlMeterRelay(int contrnlType, byte EpitopeNo, int ID = 0)
        { 
             EquipmentData.DeviceManager.ControlMeterRelay(contrnlType, EpitopeNo, ID);
        }

        /// <summary>
        /// 控制通讯继电器切换
        /// </summary>
        /// <param name="contrnlType">0—表位485通信；1—表位 232 或者蓝牙48通信；2—蓝牙232通信；</param>
        /// <param name="EpitopeNo">表位编号0xFF为广播</param>
        public void ControlConnrRelay(int contrnlType, byte EpitopeNo, int ID = 0)
        {
            EquipmentData.DeviceManager.ControlConnrRelay(contrnlType, EpitopeNo, ID);
        }

        /// <summary>
        /// 设置脉冲输出（两组脉冲频率可以不一样，但不能超过 500 倍）
        /// </summary>
        /// <param name="contrnlType">0x00=两组都不输出脉冲；0x01=仅第一组输出设定脉冲；0x02=仅第二组输出设定脉冲；0x03=两组都输出设定脉冲；</param>
        /// <param name="bwNum">表位号</param>
        /// <param name="fq1">第一组脉冲-频率--比如1000HZ</param>
        /// <param name="PWM1">第一组脉冲-占空比-0.5就是百分50</param>
        /// <param name="PulseNum1">第一组脉冲-脉冲个数--0表示连续输出</param>
        /// <param name="fq2">第二组脉冲-频率--比如1000HZ</param>
        /// <param name="PWM2">第二组脉冲-占空比-0.5就是百分50</param>
        /// <param name="PulseNum2">第二组脉冲-脉冲个数--0表示连续输出</param>
        /// <returns></returns>
        public bool SetPulseOutput(byte contrnlType, byte bwNum, float fq1, float PWM1, int PulseNum1, float fq2, float PWM2, int PulseNum2, int ID = 0)
        {
            return EquipmentData.DeviceManager.SetPulseOutput(contrnlType, bwNum, fq1, PWM1, PulseNum1, fq2, PWM2, PulseNum2,ID);
        }

        /// <summary>
        /// 控制门继电器切换
        /// </summary>
        /// <param name="contrnlType">0—继电器断开，Door信号断开；默认值；1—继电器断开，Door信号闭合；</param>
        /// <param name="EpitopeNo">表位编号0xFF为广播</param>
        public void ControlDoorSignalRelayrRelay(int contrnlType, byte EpitopeNo, int ID = 0)
        {
            EquipmentData.DeviceManager.ControlDoorSignalRelayrRelay(contrnlType, EpitopeNo, ID);
        }

        /// <summary>
        /// 控制遥信状态输出
        /// <param name="EpitopeNo">表位号</param>
        /// <param name="RS1">遥信1 true输出，false不输出</param>
        /// <param name="RS2">遥信2 true输出，false不输出</param>
        /// <param name="RS3">遥信3 true输出，false不输出</param>
        /// <param name="RS4">遥信4 true输出，false不输出</param>
        /// <param name="RS5">遥信5 true输出，false不输出</param>
        /// <param name="RS6">遥信6 true输出，false不输出</param>
        /// <param name="ID"></param>
        public void ContnrRemoteSignalingStatusOutput(byte EpitopeNo, bool RS1, bool RS2, bool RS3, bool RS4, bool RS5, bool RS6, int ID = 0)
        {
            EquipmentData.DeviceManager.ContnrRemoteSignalingStatusOutput( EpitopeNo, RS1, RS2, RS3, RS4, RS5, RS6,ID);
        }
        /// <summary>
        /// 读取遥控信息
        /// </summary>
        /// <param name="EpitopeNo">表位号 FF广播</param>
        /// <param name="OutTriggerMode">触发方式0电平1脉冲 告警 -轮次 1 -轮次2-轮次 3 -轮次4  </param>
        /// <param name="OutPutValue">输出值 0没有1输出 告警 -轮次 1 -轮次2-轮次 3 -轮次4</param>
        /// <param name="FrameAry"></param>
        public void ReadRemoteControl(byte EpitopeNo, out int[] OutTriggerMode, out int[] OutPutValue, int ID = 0)
        {
            EquipmentData.DeviceManager.ReadRemoteControl(EpitopeNo, out OutTriggerMode, out OutPutValue, ID);
        }

        /// <summary>
        ///  读取误差 
        /// </summary>
        /// <param name="readType">读取类型(00--正向有功，01--正向无功，02--反向有功，03--反向无功，04--日计时误差</param>
        /// <param name="EpitopeNo">表位编号0xFF为广播</param>
        public stError ReadWcbData(int readType, byte EpitopeNo, int ID = 0)
        {        
            //这几个都是返回值
            string[] OutWcData = new string[1]{"0"};
            int  OutBwNul = 0;
            int  OutGroup = 0;
            int  OutWcNul = 0;

            EquipmentData.DeviceManager.ReadWcbData(readType, EpitopeNo,out  OutWcData, out OutBwNul, out OutGroup, out OutWcNul, ID);


            //ErrorItem error = new ErrorItem() { };

            stError stError = new stError();
            stError.Index = OutWcNul;
            if (stError.Index > 5)
            {
                stError.szError = OutWcData[4];
            }
            else
            {
                if (stError.Index > 0)
                {
                    stError.szError = OutWcData[stError.Index - 1];
                }
                else
                {
                    stError.szError = OutWcData[0];
                }

            }
           
            return stError;


            //TODO2  1001
            //stError[] value = new stError[OutWcData.Length];
            //value[OutBwNul] = new stError();
            //value[OutBwNul].szError = OutWcData[OutBwNul]; 
            //return value;

        }
        /// <summary>
        /// 电机控制
        /// </summary>
        /// <param name="ControlType">控制类型01压入表  00取出表</param>
        /// <param name="EpitopeNo">表位号--FF广播</param>
        public bool ElectricmachineryContrnl(int ControlType, byte EpitopeNo, int ID = 0)
        {
            return EquipmentData.DeviceManager.ElectricmachineryContrnl(ControlType, EpitopeNo, ID);
        }

        /// <summary>
        /// 读取表位状态
        /// </summary>
        /// <param name="reg">命令03读取表位电压短路和电流开路标志</param>
        /// <param name="bwNum">表位</param>
        /// <param name="OutResult">返回状态0:电压短路标志，00表示没短路，01表示短路；DATA1-电流开路标志，1:00表示没开路，01表示开路。</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public MeterState Read_Fault(byte bwNum, int ID = 0)
        {
            //MeterState
            byte[] OutResult;
            bool t = EquipmentData.DeviceManager.Read_Fault(03, (byte)bwNum, out OutResult,ID);

            MeterState meterState = new MeterState();
            if (t)  //读取成功
            {
                meterState.U = (MeterState_U)OutResult[0];
                meterState.I = (MeterState_I)OutResult[1];
                meterState.Motor = (MeterState_Motor)OutResult[2];
                meterState.YesOrNo = (MeterState_YesOrNo)OutResult[3];
                meterState.CT = (MeterState_CT)OutResult[4];
                meterState.Trip = (MeterState_Trip)OutResult[5];
                meterState.TemperatureI = (MeterState_TemperatureI)OutResult[6];
                return meterState;
            }
            return null;
        }

        #endregion

        #region 标准表
        /// <summary>
        /// 初始化标准表
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="MaxWaitTime">最大等待时间</param>
        /// <param name="WaitSencondsPerByte">帧最大等待时间</param>
        /// <returns></returns>
        public bool InitSettingCom(int ComNumber, string MaxWaitTime, string WaitSencondsPerByte)
        {
            return EquipmentData.DeviceManager.InitSettingCom(ComNumber, int.Parse(MaxWaitTime), int.Parse(WaitSencondsPerByte));
        }

        /// <summary>
        /// 脉冲校准误差 字符串形式(10字节)W， 单位 分数值。
        /// 0.01525%，则0.01525% ×1000000 = 152.5，
        /// 则下发字符串 ”152.5”。
        /// </summary>
        ///  <param name="Error">误差板的值</param>
        /// <returns></returns>
        public bool SetPulseCalibration(double Error)
        {
            return EquipmentData.DeviceManager.SetPulseCalibration((Error*1000000).ToString());
        }

        /// <summary>
        ///   设置脉冲
        /// </summary>
        /// <param name="pulseType">
        ///有功脉冲 设置字符’1’
        ///无功脉冲 设置字符’2’
        ///UA脉冲 设置字符’3’
        ///UB脉冲 设置字符’4’
        ///UC脉冲 设置字符’5’
        ///IA脉冲 设置字符’6’
        ///IB脉冲 设置字符’7’
        ///IC脉冲 设置字符’8’
        ///PA脉冲 设置字符’9’
        ///PB脉冲 设置字符’10’
        ///PC脉冲 设置字符’11’
        ///</param>
        /// <returns></returns>
        public bool SetPulseType(string pulseType)
        {
            return EquipmentData.DeviceManager.SetPulseType(pulseType);
        }

        /// <summary>
        ///读取累积电量
        /// </summary>
        /// <returns >返回值float数组</returns>
        public float[] ReadEnergy()
        {
            return EquipmentData.DeviceManager.ReadEnergy();
        }
        /// <summary>
        ///档位常数 读取与设置
        /// </summary>
        /// <param name="stdCmd">>0x10 读 ，0x13写</param>
        /// <param name="stdConst">常数</param>
        /// <param name="stdUIGear">电压电流挡位UA，ub，uc，ia，ib，ic</param>
        public bool StdGear(byte stdCmd,ref long stdConst,  double[] stdUIGear)
        {
            return EquipmentData.DeviceManager.StdGear(stdCmd,ref stdConst,ref stdUIGear);
        }
        /// <summary>
        /// 100cH-启停标准表累积电能
        /// </summary>
        /// <param name="startOrStopStd">字符’1’表示清零当前电能并开始累计（ascii 码读取）</param>
        public bool startStdEnergy(int startOrStopStd)
        {
            return EquipmentData.DeviceManager.startStdEnergy(startOrStopStd);
        }


        /// <summary>
        ///  100bH-模式设置--底层有问题
        /// </summary>
        /// <param name="stdCmd"> </param>
        /// <param name="strModeJxfs">自动手动模式标识字符’1’表示手动模式，字符’0’表示自动模式（ascii 码读取）
        ///  接线方式模式标识字符’1’表示3相4线制，字符’2’表示3相3线制（ascii 码读取）
        ///标准表模式标识字符’1’表示单相表，字符’3’表示三相表，写操作此位置填0（ascii 码读取）</param>
        public bool stdMode(byte stdCmd, ref string[] strModeJxfs)
        {
            return EquipmentData.DeviceManager.stdMode(stdCmd, ref strModeJxfs);
        }
        #endregion

        #region 雷电标准表

        /// <summary>
        ///  设置接线方式 
        /// </summary>
        /// <param name="MetricIndex"></param>
        public void SetPuase(string BncCode, string MetricIndex, string aseCodex)
        {
            EquipmentData.DeviceManager.SetPuase(BncCode,MetricIndex, aseCodex);
        }
        /// <summary>
        ///  设置常数
        /// </summary>
        /// <param name="MetricIndex"></param>
        /// <param name="Constant"></param>
        /// 
        public void SetConstant(string MetricIndex, float Constant)
        {
            EquipmentData.DeviceManager.SetConstant(MetricIndex, Constant);
        }

        /// <summary>
        ///   读取标准表瞬时值
        /// </summary>
        /// 
        //public StandarMeterInfo ReadStMeterInfo()
        //{
        //    return EquipmentData.DeviceManager.ReadStMeterInfo();
        //}
        #endregion

        #region Function
        private PowerParam GetPowerPara(float ua, float ub, float uc, float ia, float ib, float ic,
                                Cus_PowerYuanJian
 ele, string glys, PowerWay glfx, Cus_PowerPhase bNxx, float feq)
        {
            PowerParam param = new PowerParam
            {
                clfs = VerifyBase.Clfs,
                UB = VerifyBase.U,              //Ub
                //IB = App.CUS.Meters.Ib,     //Ib
                //IMax = App.CUS.Meters.Imax,

                Element = ele,
                //U
                Ua = ua,
                Ub = ub,
                Uc = uc,
                //I
                Ia = ia,
                Ib = ib,
                Ic = ic,
                //相序
                IsNxx = bNxx,
                //频率
                Freq = feq
            };

            #region 去掉不需要的
            if (ele == Cus_PowerYuanJian
.H)
            {
                if (param.clfs == WireMode.单相)
                {
                    param.Ub = 0;
                    param.Uc = 0;
                    param.Ib = 0;
                    param.Ic = 0;
                }
                else if (param.clfs == WireMode.三相三线)
                {
                    param.Ub = 0;
                    param.Ib = 0;
                }
            }
            else if (ele == Cus_PowerYuanJian.A)
            {
                //I
                param.Ib = 0;
                param.Ic = 0;
            }
            else if (ele == Cus_PowerYuanJian.B)
            {
                //I
                param.Ia = 0;
                param.Ic = 0;
            }
            else if (ele == Cus_PowerYuanJian.C)
            {
                //I
                param.Ia = 0;
                param.Ib = 0;
            }
            #endregion



            //角度转换
            float[] arrPhi = Common.GetPhiGlys(param.clfs, glfx, ele, glys, bNxx);
            param.UaPhi = arrPhi[0];
            param.UbPhi = arrPhi[1];
            param.UcPhi = arrPhi[2];
            param.IaPhi = arrPhi[3];
            param.IbPhi = arrPhi[4];
            param.IcPhi = arrPhi[5];
            param.Cos = arrPhi[6];

            if (param.clfs == WireMode.三相三线)
            {
                param.Ub = 0;
                param.Ib = 0;
            }

            return param;
        }



        /// <summary>
        /// 求数据中的最大数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T Max<T>(params T[] values) where T : IComparable
        {
            List<T> va = new List<T>();
            va.AddRange(values);
            va.Sort();
            return va[va.Count - 1];
        }
        #endregion

        #region 设备反射方法调用
        public  object DeviceControl(Type type,object obj, string functionName, params object[] value)
        {

            try
            {
                if (type == null)//联机出现问题，没有实例化
                {
                    //LogManager.AddMessage($"{deviceName}还未连接", EnumLogSource.设备操作日志, EnumLevel.Information);
                    return false;
                }
                MethodInfo mInfo = type.GetMethod(functionName);
                return mInfo.Invoke(obj, value);
            }
            catch (Exception )
            {
                //LogManager.AddMessage("调用设备方法失败!错误代码20001-001:" + e, EnumLogSource.设备操作日志, EnumLevel.Error);
                return null;
            }
        }
        #endregion

        #region 其他
        /// <summary>
        /// 连接加密机
        /// </summary>
        public static bool LinkPasswordMatchine()
        {
            return EquipmentData.DeviceManager.LinkDog();
        }
        #endregion

        #region 载波
        /// <summary>
        /// 载波供电 
        /// </summary>
        /// <param name="IsOn">开启还是关闭</param>
        public void SetZBGZDYContrnl(bool IsOn)
        {
            bool[] t = new bool[1];
            t[0] = IsOn;
            if (EquipmentData.DeviceManager.Devices.ContainsKey(Device.DeviceName.载波信号控制板))
            {
                EquipmentData.DeviceManager.SetZBGZDYContrnl(255, 12, t);
            }
        }

        /// <summary>
        /// 是否存在这个设备
        /// </summary>
        /// <returns></returns>
        public bool IsHaveDevice(string type)
        {
            bool value = false;
            switch (type)
            {
                case "载波信号控制板":
                    if (EquipmentData.DeviceManager.Devices.ContainsKey(Device.DeviceName.载波信号控制板))
                        value = true;
                    break;
                default:
                    break;
            }


            return value;
        }
        #endregion

        #region 功耗板
        public bool Read_GH_Dissipation(int bwIndex, out float[] pd)
        {
            return  EquipmentData.DeviceManager.Read_GH_Dissipation(bwIndex, out pd);
        }
        #endregion

        #region 耐压仪及耐压测试板
        /// <summary>
        ///耐压哟 启动，停止测试
        /// </summary>
        /// <param name="ValueType">01开始，02结束<param>
        /// <returns></returns>
        public bool Ainuo_Start(byte ValueType)
        {
            return EquipmentData.DeviceManager.Ainuo_Start(ValueType);
        }

        #endregion

        #region 零线电流板
        //add yjt 20230103 新增零线电流控制启停
        /// <summary>
        /// 启停零线电流板
        /// </summary>
        /// <param name="startOrStopStd">字符‘1’开启，字符‘0’关闭</param>
        public bool StartZeroCurrent(int A_kz, int BC_kz)
        {
            return EquipmentData.DeviceManager.StartZeroCurrent(A_kz, BC_kz);
        }
        #endregion
    }


    #region 读取到的数据类
    /// <summary>
    /// 读取的误差数据
    /// </summary>
    public class stError
    {
        public stError()
        {
            szError = "0";
            Index = 0;
            MeterIndex = 0;
        }
        /// <summary>
        /// 误差值
        /// </summary>
        public string szError;

        /// <summary>
        /// 标识当前属于第几次误差
        /// </summary>
        public int Index;

        /// <summary>
        /// 表位号
        /// </summary>
        public int MeterIndex;
        /// <summary>
        /// 状态类型
        /// </summary>
        public int MeterConst;
        /// <summary>
        /// 电压回路状态,0x00表示直接接入式电表电压回路选择，0x01表示互感器接入式电表电压回路选择，0x02表示本表位无电表接入
        /// </summary>
        //public Cus_BothVRoadType vType;
        /// <summary>
        /// 通讯口状态,0x00表示选择第一路普通485通讯；0x01表示选择第二路普通485通讯；0x02表示选择红外通讯；
        /// </summary>
        public int ConnType;

        /*
         * 状态类型分为四种：接线故障状态（Bit0）、预付费跳闸状态（Bit1）、报警信号状态（Bit2）、对标状态（Bit3）的参数
         * 分别由一个字节中的Bit0、Bit1、Bit2、Bit3标示，为1则表示该表位有故障/跳闸/报警/对标完成，为0则表示正常/正常/正常/未完成对标。
        */
        /// <summary>
        /// 接线故障状态,为true则表示该表位有故障,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Jxgz;

        /// <summary>
        /// 预付费跳闸状态,为true则表示该表位跳闸,为false正常
        /// </summary>
        public bool statusTypeIsOnErr_Yfftz;

        /// <summary>
        /// 报警信号状态,为true则表示该表位报警,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Bjxh;

        /// <summary>
        /// 对标状态,为true则表示该表位对标完成,false为未完成对标
        /// </summary>
        public bool statusTypeIsOnOver_Db;
        /// <summary>
        /// 温度过高故障状态（false：正常；true：故障）。温度过高时，会自动短接隔离继电器
        /// </summary>
        public bool statusTypeIsOnErr_Temp;
        /// <summary>
        /// 光电信号状态（false：未挂表；true：已挂表）
        /// </summary>
        public bool statusTypeIsOn_HaveMeter;

        /// <summary>
        /// 表位上限限位状态（false：未就位；true：就位）
        /// </summary>
        public bool statusTypeIsOn_PressUpLimit;

        /// <summary>
        /// 表位下限限位状态（false：未就位；true：就位）
        /// </summary>
        public bool statusTypeIsOn_PressDownLimt;
        /// <summary>
        /// true :读到数据 FALSE ：没有读到
        /// </summary>
        public bool statusReadFlog;


        public string QdTime;
    }
    #endregion
}
