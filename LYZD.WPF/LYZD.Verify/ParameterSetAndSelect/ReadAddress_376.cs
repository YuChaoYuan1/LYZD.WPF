using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.ParameterSetAndSelect
{

    /// <summary>
    /// 逻辑地址查查
    /// </summary>
    public class ReadAddress_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "逻辑地址", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();

                MessageAdd("正在读取终端地址...", EnumLogType.提示与流程信息);

                DateTime dt = DateTime.Now;
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 89, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            if (RecData[i].Length == 5)
                            {
                                TempData[i].Resoult = "合格";
                                TempData[i].Data = GetData(RecData, i, 3, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                                if (VerifyConfig.IsHexAddress)
                                    meterInfo[i].Address = GetData(RecData, i, 3, EnumTerimalDataType.e_string) + GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                                else
                                    meterInfo[i].Address = GetData(RecData, i, 3, EnumTerimalDataType.e_string) + (Convert.ToInt32(GetData(RecData, i, 4, EnumTerimalDataType.e_string), 16)).ToString().PadLeft(5, '0');
                            }
                            else
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "读终端地址无回复";
                            }
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "读终端地址无回复";
                        }
                    }
                }
                AddItemsResoult("逻辑地址", TempData);
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
