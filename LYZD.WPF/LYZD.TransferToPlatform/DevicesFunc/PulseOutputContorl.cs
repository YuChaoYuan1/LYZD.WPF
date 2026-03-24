using LYZD.TransferToPlatform.SocketEven;
using LYZD.TransferToPlatform.Test;
using LYZD.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.DevicesFunc
{
    public class PulseOutputContorl
    {
        public static void SETShhakeSate(PulseOutputType outputType)
        {
            DevOrderEven.DevOrderMsg("设置脉冲：组数"+ outputType.contrnlType.ToString()+" 表位号："+ outputType.bwNum .ToString()+ " 第一组频率：" + outputType.GuoupOneFreq.ToString() + " 占空比" + outputType.GuoupOnePWM.ToString() + " 脉冲个数" + outputType.GuoupOneNum.ToString() + " 第二组频率：" + outputType.GuoupTweFreq.ToString() + " 占空比" + outputType.GuoupTwePWM.ToString() + " 脉冲个数：" + outputType.GuoupTweNum.ToString());
            EquipmentData.DeviceManager.SetPulseOutput(outputType.contrnlType, outputType.bwNum, outputType.GuoupOneFreq, outputType.GuoupOnePWM, outputType.GuoupOneNum, outputType.GuoupTweFreq, outputType.GuoupTwePWM, outputType.GuoupTweNum, outputType.DevicId);
        }

        /// <summary>
        /// 启动误差版
        /// </summary>
        /// <param name="ControlType">控制类型（00：正向有功，01：正向无功，02：反向有功，03：反向无功，04：日计时，05：需量， 06：正向有功脉冲计数， 07：正向无功脉冲计数， 08：反向有功脉冲计数，09 反向无功脉冲计数）</param>
        /// <param name="MeterNo">表位号，FF为广播</param>
        /// <returns></returns>
        public static bool StartWcb(int MeterNo)
        {
            return EquipmentData.DeviceManager.StartWcb(04, (byte)MeterNo, MeterNo > LoadIni.LoadIni.AllMeterNumber ? 1 : 0);
        }

        /// <summary>
        /// 停止误差版
        /// </summary>
        /// <param name="ControlType">控制类型（00：正向有功，01：正向无功，02：反向有功，03：反向无功，04：日计时，05：需量， 06：正向有功脉冲计数， 07：正向无功脉冲计数， 08：反向有功脉冲计数，09 反向无功脉冲计数）</param>
        /// <param name="MeterNo">表位号，FF为广播</param>
        /// <returns></returns>
        public static bool StopWcb(int MeterNo)
        {
            return EquipmentData.DeviceManager.StopWcb(04, 0xff, MeterNo > LoadIni.LoadIni.AllMeterNumber ? 1 : 0);
        }

        /// <summary>
        /// 设置误差版标准常数
        /// </summary>
        /// <param name="EpitopeNo"></param>
        public static void SetStandardConst(int EpitopeNo)
        {
            EquipmentData.DeviceManager.SetStandardConst(1, 500000, 0, (byte)EpitopeNo, EpitopeNo > LoadIni.LoadIni.AllMeterNumber ? 1 : 0);
        }

        /// <summary>
        /// 设置误差版被检常数
        /// </summary>
        /// <param name="EpitopeNo"></param>
        public static void SetTestedConst(int EpitopeNo, int qs)
        {
             EquipmentData.DeviceManager.SetTestedConst(04, 1, 0, qs, (byte)EpitopeNo, EpitopeNo > LoadIni.LoadIni.AllMeterNumber ? 1 : 0);
        }

        /// <summary>
        ///  读取误差 
        /// </summary>
        /// <param name="readType">读取类型(00--正向有功，01--正向无功，02--反向有功，03--反向无功，04--日计时误差</param>
        /// <param name="EpitopeNo">表位编号0xFF为广播</param>
        public static stError ReadWcbData(int EpitopeNo)
        {
            //这几个都是返回值
            string[] OutWcData = new string[1] { "0" };
            int OutBwNul = 0;
            int OutGroup = 0;
            int OutWcNul = 0;

            EquipmentData.DeviceManager.ReadWcbData(4, (byte)EpitopeNo, out OutWcData, out OutBwNul, out OutGroup, out OutWcNul, EpitopeNo > LoadIni.LoadIni.AllMeterNumber ? 1 : 0);


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

        }
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
