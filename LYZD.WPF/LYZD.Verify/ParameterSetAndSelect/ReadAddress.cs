using LYZD.Core.Enum;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.ParameterSetAndSelect
{
    /// <summary>
    /// 终端逻辑地址查询
    /// </summary>
    public class ReadAddress : VerifyBase
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
                StartVerify698();
                MessageAdd("正在读取终端地址",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        setData[i] = Talkers[i].Framer698.ReadData_address("40010200");
                    }
                }
               TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, 5000);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            if (RecData[i].Length >= 8)
                            {
                                TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string);

                                Talkers[i].Framer698.str_Ter_Address = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                                meterInfo[i].Address = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                            }
                            else
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "地址长度不正确";
                            }
                        }
                        else
                        {
                            //MessageAdd( "终端" + (i + 1) + "对时无回复！");
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "读取失败,无回复";
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
