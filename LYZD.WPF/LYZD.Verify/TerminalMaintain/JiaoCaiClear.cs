using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.TerminalMaintain
{    
    /// <summary>
    /// 交采电量清零
    /// </summary>
    public class JiaoCaiClear : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "清零后电量", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify698();
                ConnectLink(false);
                SetData_6982("060119F1010200160100", "设置安全模式参数");
                MessageAdd("正在读取安全模式参数",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        setData[i] = Talkers[i].Framer698.ReadData_05("F1010200");
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                ResetTerimal_6982(2);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        setData[i] = Talkers[i].Framer698.ReadData_jiaocai("05 01 01 00 10 02 00 00");
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "0";
                        if (TalkResult[i] == 0)
                        {
                            string value = GetData(RecData, i, 5, EnumTerimalDataType.e_float);
                            TempData[i].Data = "正向有功总电量:" + value;
                            if (Convert.ToSingle(value) > 0) //清零后电量大于0就是不合格
                            {
                                TempData[i].Tips = "电量大于0";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("清零后电量", TempData);
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
