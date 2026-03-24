using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.ParameterSetAndSelect
{
    /// <summary>
    /// 时钟设置和召集
    /// </summary>
    public class ReadSetTime : VerifyBase
    {
        public override void Verify()
        {
            base.Verify();
            StartVerify698();
            ConnectLink(false);

            SetTime_698(DateTime.Now, 0);

            WaitTime("等待", 5);

            MessageAdd("正在读取时钟",EnumLogType.流程信息);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    setData[i] = Talkers[i].Framer698.ReadData_05("40000200");
                }
            }
            DateTime dttmp = DateTime.Now;
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData =dttmp.ToString();
                    if (TalkResult[i] == 0)
                    {
                        long ts = Convert.ToDateTime(GetData(RecData, i, 5, EnumTerimalDataType.e_datetime)).Ticks / 10000000 - dttmp.AddMinutes(-dintervaltime).Ticks / 10000000;
                        //ResultDictionary["读取时钟"][i] = GetData(RecData, i, 5, EnumTerimalDataType.e_datetime) + "," + ts + "|" + dttmp + ",间隔10秒";
                        TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_datetime);
                        if (Math.Abs(ts) > 10)
                        {
                            TempData[i].Resoult = "误差超出,当前误差"+ts;
                            TempData[i].Tips = "终端对时无回复";
                        }
                    }
                    else
                    {
                        TempData[i].Tips = "终端对时无回复";
                        TempData[i].Resoult = "不合格";
                    }
                }
            }
            AddItemsResoult("读取时钟", TempData);

        }

        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "读取时钟", "结论" };
            return true;
        }
    }
}
