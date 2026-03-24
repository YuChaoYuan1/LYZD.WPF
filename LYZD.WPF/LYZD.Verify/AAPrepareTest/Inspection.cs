using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LYZD.Verify.AAPrepareTest
{
    public class Inspection : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "液晶屏和铭牌及其它外观结构", "结论" };
            return true;
        }

        public override void Verify()
        {
            try
            {
                base.Verify();
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "完好";
                        TempData[i].Data = "完好";
                        TempData[i].Resoult = "合格";
                    }
                }
                AddItemsResoult("液晶屏和铭牌及其它外观结构", TempData);
                
            }
            catch (Exception ex)
            {
                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
