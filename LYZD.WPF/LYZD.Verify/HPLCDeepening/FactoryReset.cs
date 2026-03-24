using LYZD.Core.Enum;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.HPLCDeepening
{
    /// <summary>
    /// 恢复出厂默认参数
    /// </summary>
    public class FactoryReset : VerifyBase
    {
        public override void Verify()
        {
            base.Verify();
              
            ConnectLink(false);         //建立应用连接；

            SetData_698("06010531000900030100", "设置事件有效标志");//设置事件有效标志(31000900)；

            #region 读取终端时钟
            MessageAdd("正在读取终端时钟...", EnumLogType.错误信息);
            DateTime dt = DateTime.Now;
            TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 2, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].StdData = dt.ToString();
                        if (RecData[i].Length == 4)
                        {
                            long ts = DateTime.Parse(GetData(RecData, i, 3, EnumTerimalDataType.e_datetime)).Ticks / 10000000 - dt.Ticks / 10000000;
                            TempData[i].Data = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
                            if (Math.Abs(ts) > 10)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "误差超差(限制为2；误差值为)" + ts.ToString();
                            }
                        }
                        else
                        {
                            TempData[i].Tips = "读终端时间返回不正确！";
                            TempData[i].Resoult = "不合格";
                            TempData[i].Data = "----/--/-- --:--:--";
                        }
                    }
                    else
                    {
                        TempData[i].Tips = "读终端时间无回复";
                        TempData[i].Resoult = "不合格";
                        TempData[i].Data = "----/--/-- --:--:--";
                    }
                }
            }
            AddItemsResoult("读取终端时钟", TempData);
            #endregion 

            ResetTerimal_698(3);   //恢复出厂默认参数(43000400)，通信参数保持现状，不在恢复之列；

            WaitTime("延时等待", 120);

            ConnectLink2(false);

            #region 读取上1次终端初始化事件(31000200)；
            int ret = 0;
            MessageAdd("正在读取上1次终端初始化事件", EnumLogType.提示与流程信息);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                    Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen("05 03 09 31 00 02 00 09 01 03 00 20 22 02 00 00 20 1E 02 00 00 20 20 02 00 00 ") + "05 03 09 31 00 02 00 09 01 03 00 20 22 02 00 00 20 1E 02 00 00 20 20 02 00 00 " + "0110" + Talkers[i].Framer698.cOutRand;
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = dt.ToString();
                    if (TalkResult[i] == 0)
                    {
                        Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.ucMac = GetMac(RecData, i);// 19, EnumTerimalDataType.e_string);

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                        if (ret == 0)
                            TempData[i].Resoult = "合格";
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "抄读失败";
                        }

                        if (GetDateTime(GetData(RecData, i, 13, EnumTerimalDataType.e_datetime)) < dt)
                        {
                            TempData[i].Tips = "时间超出";
                            TempData[i].Resoult = "不合格";
                        }
                        TempData[i].Data = "次数:" + GetData(RecData, i, 12, EnumTerimalDataType.e_int) + "," + "发生时间:" + GetData(RecData, i, 13, EnumTerimalDataType.e_datetime);
                    }
                    else
                    {
                        MessageAdd("终端" + (i + 1) + "对时无回复！", EnumLogType.错误信息);
                        TempData[i].Tips = "无回复";
                        TempData[i].Resoult = "不合格";
                    }
                }
            }
            AddItemsResoult("上1次终端初始化事件", TempData);
            #endregion

            #region 读取终端初始化事件当前记录数
            MessageAdd("读取终端初始化事件当前记录数", EnumLogType.提示与流程信息);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {

                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                    Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen("05 01 0C 31 00 04 00 00 ") + "05 01 0C 31 00 04 00 00 " + "0110" + Talkers[i].Framer698.cOutRand;

                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "1";

                    if (TalkResult[i] == 0)
                    {
                        Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.ucMac = GetMac(RecData, i);// 19, EnumTerimalDataType.e_string);

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);
                        if (ret == 0)
                        {
                            TempData[i].Resoult = "合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "抄读失败";

                        }
                        TempData[i].Data = "次数:" + GetData(RecData, i, 7, EnumTerimalDataType.e_int);
                        if (GetData(RecData, i, 7, EnumTerimalDataType.e_string) != "1")
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "次数不等于1";
                        }
                    }
                    else
                    {
                        MessageAdd("终端" + (i + 1) + "对时无回复！", EnumLogType.错误信息);
                        TempData[i].Resoult = "不合格";
                        TempData[i].Tips = "无回复";
                    }
                }
            }
            AddItemsResoult("终端初始化事件当前记录数", TempData);
            #endregion
        }
        protected override bool CheckPara()
        {
            //1）	读取安全模式参数(F1010200)，如果未启用安全模式，则设置为启用；
            //2）	读取ESAM序列号(F1000200)、对称密钥版本(F1000400)、计数器(F1000700)；
            //3）	读取主站证书(F1000C00)；
            //4）	读取终端证书(F1000A00)；
            //5）	建立应用连接；
            //6）	设置事件有效标志(31000900)；
            //7）	读取终端时钟(40000200)；
            //8）	恢复出厂默认参数(43000400)，通信参数保持现状，不在恢复之列；
            //9）	延时等待120秒；
            //10）	建立应用连接；
            //11）	读取上1次终端初始化事件(31000200)；
            //12）	读取终端初始化事件当前记录数(31000400)，判断记录数是否正确。
            ResultNames = new string[] { "上1次终端初始化事件", "终端初始化事件当前记录数", "读取终端时钟", "恢复出厂默认参数", "终端对时", "结论" };
            return true;
        }

    }


}
