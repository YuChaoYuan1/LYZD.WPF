using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.EventRecord
{
    /// <summary>
    /// 终端编程事件
    /// </summary>
    public class TerProGramEvent : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "上1次编程事件记", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify698();

                int ret = 0;
                ConnectLink(false);


                DateTime dtHappen = DateTime.Now.AddMinutes(-1);//记录当前时间，事件发生的时间一定比这个晚

                SetData_698("06011D31180900030100", "设置事件有效标志");

                SetData_698("060121410302000A075347434D30303100", "设置集中器参数");

                WaitTime("等待，", 10);

                MessageAdd("读取上1次编程事件记录",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen(" 05 03 22 31 18 02 00 09 01 00 00 ".Replace(" ", "")) + " 05 03 22 31 18 02 00 09 01 00 00 ".Replace(" ", "") + "0110" + Talkers[i].Framer698.cOutRand;

                       setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = dtHappen.ToString();
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);// 19,  EnumTerimalDataType.e_string);
                            ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);
                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }
                            if (GetDateTime(GetMac(RecData[i], 2, "数据长度")) < dtHappen || !GetData(RecData, i, "41030200"))
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Data = "发生时间:" + GetDateTime(GetMac(RecData[i], 2, "数据长度"));
                            }
                            else
                                TempData[i].Data = "发生时间:" + GetDateTime(GetMac(RecData[i], 2, "数据长度")) + "," + "Oad:41030200";


                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("上1次编程事件记", TempData);
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
