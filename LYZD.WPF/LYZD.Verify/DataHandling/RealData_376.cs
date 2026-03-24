using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.DataHandling
{
    /// <summary>
    /// 实时和当前数据-
    /// </summary>
    public class RealData_376 : VerifyBase
    {

        public override void Verify()
        {

          
            StartVerify();
            #region 模拟表操作
            StringBuilder sb = new StringBuilder();
            // 设置模拟表电压电流
            ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
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
                sb.Append("00100");
            }
            ControlVirtualMeter(sb.ToString());
            #endregion

            SetTime(DateTime.Now, 0);
            BaseVerifyUnit_ReturnOk("对时命令", 1);

            #region 数据区初始化
            ResetTerimal(2);
            BaseVerifyUnit_ReturnOk("数据区初始化", 2);
            #endregion

            WaitTime("数据发生抄表", VerifyConfig.WaitTime_CopyMeter);

            BaseVerifyUnit("终端日历时钟", new int[] { 3 }, 0, 2, 3);

            BaseVerifyUnit("终端上行通信状态", new int[] { 3 }, 0, 4, 4);

            BaseVerifyUnit("终端状态量及变位标志", new int[] { 3 }, 0, 9, 5);

            BaseVerifyUnit("终端事件计数器当前值", new int[] { 3 }, 0, 7, 6);

            BaseVerifyUnit("终端事件标志状态", new int[] { 3 }, 0, 8, 7);

            BaseVerifyUnit("终端参数状态", new int[] { 3 }, 0, 3, 8);

            BaseVerifyUnit("当前正向有功电能示值", new int[] { 5 }, 2, 129, 9);

            BaseVerifyUnit("当前反向有功电能示值", new int[] { 5 }, 2, 131, 10);

            BaseVerifyUnit("电能表日历时钟", new int[] { 4 }, 2, 162, 11);

            BaseVerifyUnit("电能表开关操作次数及时间", new int[] { 5 }, 2, 165, 12);

            BaseVerifyUnit("当日电压统计数据", new int[] { 5 }, 2, 179, 13);

            BaseVerifyUnit("当月电压统计数据", new int[] { 5 }, 2, 180, 14);


        }

        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //三相电压|电能表日历时钟|时段表编程总次数|校时次数|电表运行状态字|正向有功电能示值|反向有功电能示值|正向有功最大需量|反向有功最大需量
            ResultNames = new string[] { "对时命令", "数据区初始化", "终端日历时钟", "终端上行通信状态", "终端状态量及变位标志", "终端事件计数器当前值", "终端事件标志状态", "终端参数状态", "当前正向有功电能示值", "当前反向有功电能示值", "电能表日历时钟", "电能表开关操作次数及时间", "当日电压统计数据", "当月电压统计数据", "结论" };
            return true;
        }


        private void BaseVerifyUnit(string p_str_Message, int[] p_int_DataIndex, byte p_byt_Pn, byte p_byt_Fn, int int_index)
        {
            MessageAdd("正在进行："+p_str_Message,EnumLogType.提示信息);
            
            TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p_byt_Pn, p_byt_Fn, RecData, MaxWaitSeconds_Read485);

            string[] s1 = new string[MeterNumber];
            string[] s2 = new string[MeterNumber];

            for (int tableNo = 0; tableNo < MeterNumber; tableNo++)
            {
                for (int index = 0; index < p_int_DataIndex.Length; index++)
                {
                    try
                    {
                        if (meterInfo[tableNo].YaoJianYn)
                        {
                            s1[tableNo] += "," + GetData(RecData, tableNo, p_int_DataIndex[index], EnumTerimalDataType.e_string);
                        }
                        else
                        {
                            s1[tableNo] = "";
                            s2[tableNo] = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        s1[tableNo] = "";
                        s2[tableNo] = "";
                        MessageAdd((tableNo + 1) + "号表/错误：" + ex.Message, EnumLogType.错误信息);
                        continue;
                    }
                }
                if (s1[tableNo].Length > 1)
                {

                    s1[tableNo] = s1[tableNo].Remove(0, 1);
                    TempData[tableNo].Data = s1[tableNo];
                }

                if (s1[tableNo].Length < 1)
                {
                    TempData[tableNo].Tips = "数据返回异常";
                    TempData[tableNo].Resoult = Core.Helper.Const.不合格;
                }
            }
            AddItemsResoult(p_str_Message, TempData);
        }

    }
}
