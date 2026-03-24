using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.DataHandling
{
    /// <summary>
    ///  实时和当前数据
    /// </summary>
    public class RealData : VerifyBase
    {
        public override void Verify()
        {
            base.Verify();
            StartVerify698();
            int ret = 0;
            ConnectLink(false);

            SetData_698("06013443000800030000", "禁止终端主动上报");
            #region 读取终端主动上报状态
            MessageAdd("读取终端主动上报状态",EnumLogType.流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                    Talkers[i].Framer698.sAPDU = "10000805010F" + "43000800" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
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
                        Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2,EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.ucMac = GetMac(RecData, i);// 10,EnumTerimalDataType.e_string);
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);
                        if (ret == 0)
                            TempData[i].Resoult = "合格";
                        else
                           TempData[i].Resoult="不合格";

                        string stmp = GetData(RecData, i, 7,EnumTerimalDataType.e_string);
                        if (stmp != "0")
                           TempData[i].Resoult="不合格";
                        TempData[i].Data = stmp;
                    }
                }
            }
            AddItemsResoult("读取终端主动上报状态", TempData);
            #endregion


            DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-01 12:00:00");
            SetTime_698(dttmp, 0); ;

            ResetTerimal_698(2);
            ConnectLink2(true);

            MessageAdd("读取通信流量，供电时间，复位次数",EnumLogType.流程信息, true);
            MessageAdd("读取终端实时数据...",EnumLogType.流程信息);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {

                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                    Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen("0502390322000200220302002204020000") + "0502390322000200220302002204020000" + "0110" + Talkers[i].Framer698.cOutRand;

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
                        Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2,EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.ucMac = GetMac(RecData, i);// 20,EnumTerimalDataType.e_string);

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                        if (ret == 0)
                            TempData[i].Resoult =  "合格";
                        else
                           TempData[i].Resoult="不合格";

                        string stmp = GetData(RecData, i, 8,EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 8,EnumTerimalDataType.e_string);

                        TempData[i].Data = stmp;
                    }
                }
            }
            AddItemsResoult("通信流量(日月)", TempData);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = ">0";
                    if (TalkResult[i] == 0)
                    {
                        Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2,EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.ucMac = GetMac(RecData, i);// 20,EnumTerimalDataType.e_string);

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                        if (ret == 0)
                           TempData[i].Resoult= "合格";
                        else
                           TempData[i].Resoult="不合格";

                        string stmp = GetData(RecData, i, 12,EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13,EnumTerimalDataType.e_string);
                        if (Convert.ToInt32(GetData(RecData, i, 12,EnumTerimalDataType.e_string)) < 1 || Convert.ToInt32(GetData(RecData, i, 13,EnumTerimalDataType.e_string)) < 1)
                           TempData[i].Resoult="不合格";
                        TempData[i].Data = stmp ;
                    }
                }
            }
            AddItemsResoult("供电时间(日月)", TempData);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "1,1";
                    if (TalkResult[i] == 0)
                    {
                        Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2,EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.ucMac = GetMac(RecData, i);// 20,EnumTerimalDataType.e_string);


                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                        if (ret == 0)
                            TempData[i].Resoult = "合格";
                        else
                           TempData[i].Resoult="不合格";

                        string stmp = GetData(RecData, i, 16,EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17,EnumTerimalDataType.e_string);
                        if (Convert.ToInt32(GetData(RecData, i, 16,EnumTerimalDataType.e_string)) != 1 || Convert.ToInt32(GetData(RecData, i, 17,EnumTerimalDataType.e_string)) != 1)
                           TempData[i].Resoult="不合格";
                        TempData[i].Data = stmp ;
                    }
                }
            }
            AddItemsResoult("复位次数(日月)", TempData);

            dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-02 23:59:50");
            SetTime_698(dttmp, 0);

            WaitTime("跨日等待", 60);

            ResetTerimal_698(1);
            ConnectLink2(true);


            MessageAdd("读取通信流量，供电时间，复位次数",EnumLogType.流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {

                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                    Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen("0502390322000200220302002204020000") + "0502390322000200220302002204020000" + "0110" + Talkers[i].Framer698.cOutRand;

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
                        Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2,EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.ucMac = GetMac(RecData, i);// 20,EnumTerimalDataType.e_string);


                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                        if (ret == 0)
                            TempData[i].Resoult = "合格";
                        else
                           TempData[i].Resoult="不合格";

                        string stmp = GetData(RecData, i, 8,EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 8,EnumTerimalDataType.e_string);

                        TempData[i].Data = stmp;
                    }
                }
            }
            AddItemsResoult("通信流量(日月)", TempData);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = ">0";
                    if (TalkResult[i] == 0)
                    {
                        Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2,EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.ucMac = GetMac(RecData, i);// 20,EnumTerimalDataType.e_string);


                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                        if (ret == 0)
                            TempData[i].Resoult = "合格";
                        else
                           TempData[i].Resoult="不合格";

                        string stmp = GetData(RecData, i, 12,EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13,EnumTerimalDataType.e_string);
                        if (Convert.ToInt32(GetData(RecData, i, 12,EnumTerimalDataType.e_string)) < 1 || Convert.ToInt32(GetData(RecData, i, 13,EnumTerimalDataType.e_string)) < 1)
                           TempData[i].Resoult="不合格";

                        TempData[i].Data = stmp ;
                    }
                }
            }
            AddItemsResoult("供电时间(日月)", TempData);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "1,2";

                    if (TalkResult[i] == 0)
                    {
                        Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2,EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.ucMac = GetMac(RecData, i);// 20,EnumTerimalDataType.e_string);


                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                        if (ret == 0)
                            TempData[i].Resoult = "合格";
                        else
                           TempData[i].Resoult="不合格";

                        string stmp = GetData(RecData, i, 16,EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17,EnumTerimalDataType.e_string);
                        if (Convert.ToInt32(GetData(RecData, i, 16,EnumTerimalDataType.e_string)) != 1 || Convert.ToInt32(GetData(RecData, i, 17,EnumTerimalDataType.e_string)) != 2)
                           TempData[i].Resoult="不合格";
                        TempData[i].Data = stmp;
                    }
                }
            }
            AddItemsResoult("复位次数(日月)", TempData);

        }
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "读取终端主动上报状态", "通信流量(日月)", "供电时间(日月)", "复位次数(日,月)", "结论" };
            return true;
        }
    }
}
