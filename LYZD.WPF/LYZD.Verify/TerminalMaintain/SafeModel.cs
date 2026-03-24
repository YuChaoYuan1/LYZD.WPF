using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.TerminalMaintain
{
    public class SafeModel : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "设置安全模式参数", "安全模式参数", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify698();
                ConnectLink(true);
                SetTime_698(DateTime.Now, 0);
                SetData_698("060119F1010200160000", "设置安全模式参数");
                MessageAdd("正在读取安全模式参数", EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        setData[i] = Talkers[i].Framer698.ReadData_05("F1010200");
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
                            string sTmp = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                            TempData[i].Data = sTmp;
                            if (sTmp != "0")
                                TempData[i].Resoult = "不合格";
                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            //MessageAdd("终端" + (i + 1) + "对时无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("安全模式参数", TempData);
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
