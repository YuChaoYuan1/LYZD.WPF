using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.ParameterSetAndSelect
{
    /// <summary>
    /// 抄表参数
    /// </summary>
    public class MeterReadingParameters : VerifyBase
    {
        public override void Verify()
        {
            base.Verify();
            StartVerify698();
            ConnectLink(false);

            SetData_698_No("070136600086000000", "清空采集档案配置表");

            SetData_698_No("07013760007F000204120001020A5507050000000000011606160351F2010201090600000000000011041100160312089812000F02045507050000000000000906000000000000120001120001010000", "下发一块采集档案");
            MessageAdd("读取一块采集档案",EnumLogType.流程信息);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.sAPDU = "05033860000200016001020112000101006001020000";
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data = GetData(RecData, i, 4, Core.Enum.EnumTerimalDataType.e_string);
                        //if (GetData(RecData, i, 4, EnumTerimalDataType.e_string) != "00")
                        //   TempData[i].Resoult="不合格";
                    }
                    else
                    {
                       TempData[i].Resoult="不合格";
                        TempData[i].Tips = "无回复";
                    }
                }
            }
            AddItemsResoult("一块采集档案", TempData);

            SetData_698_No("0701396000800001020204120001020A5507050000000000011606160351F2010201090600000000000011041100160312089812000F0204550705000000000000090600000000000012000112000101000204120002020A5507050000000000021606160351F2010201090600000000000011041100160312089812000F02045507050000000000000906000000000000120001120001010000", "下发采集档案");
            MessageAdd("读取采集档案",EnumLogType.流程信息);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {

                    Talkers[i].Framer698.sAPDU = "05013A6000020000";
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data = GetData(RecData, i, 4, Core.Enum.EnumTerimalDataType.e_string);

                        //if (GetData(RecData, i, 4, EnumTerimalDataType.e_string) != "00")
                        //TempData[i].Resoult = "不合格";
                    }
                    else
                    {
                        TempData[i].Resoult = "不合格";
                        TempData[i].Tips = "无回复";
                    }
                }
            }
            AddItemsResoult("采集档案", TempData);

        }
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "一块采集档案", "采集档案", "结论" };
            return true;
        }
    }
}
