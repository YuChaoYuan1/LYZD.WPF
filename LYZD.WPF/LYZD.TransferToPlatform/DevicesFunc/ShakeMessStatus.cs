using LYZD.Core.Enum;
using LYZD.TransferToPlatform.SocketEven;
using LYZD.TransferToPlatform.Test;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.DevicesFunc
{
    public class ShakeMessStatus
    {
       

        /// <summary>
        /// 设置d数据
        /// </summary>
        public static byte[][] setData = new byte[0][];

        /// <summary>
        /// 通讯返回结果 0：正确，非0：不正确 
        /// </summary>
        public static int[] TalkResult = new int[0];

        /// <summary>
        /// 通讯返回数据内容
        /// </summary>
        public static Dictionary<int, string[]> RecData = new Dictionary<int, string[]>();

        

        /// <summary>
        /// 读取表位遥信
        /// </summary>
        /// <param name="EpitopeNo">表位号</param>
        /// <returns></returns>
        public static string ReadShakeStauts(int EpitopeNo)
        {
            VerifyBase.InitTerminalTalks();

            setData = new byte[16][];


            //先去读取地址
            setData[EpitopeNo] = VerifyBase.Talkers[EpitopeNo].Framer698.ReadData_address("40010200");

            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, 5000);

            VerifyBase.Talkers[EpitopeNo].Framer698.str_Ter_Address = GetData(RecData, EpitopeNo, 5, EnumTerimalDataType.e_string);


            //再去读取遥信状态
            setData[EpitopeNo] = VerifyBase.Talkers[EpitopeNo].Framer698.ReadData_05("F2030400");

            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, 20000);

            string hex = Convert.ToInt32("11110000", 2).ToString("x2");
            return hex;
        }

        public static string ReadShakeStautsT(int EpitopeNo)
        {

            return "";
        }

        /// <summary>
        /// 设置遥信状态
        /// </summary>
        /// <param name="EpitopeNo"></param>
        /// <param name="ShakeSate_InOut"></param>
        public static void SETShhakeSate(int EpitopeNo, Dictionary<int, string> ShakeSate_InOut)
        {
            bool RS1 = false; ;
            bool RS2 = false;
            bool RS3 = false;
            bool RS4 = false;
            bool RS5 = false;
            bool RS6 = false;

            foreach (var item in ShakeSate_InOut)
            {
                switch (item.Key)
                {
                    case 0:
                        if (item.Value == "1")
                        {
                            RS1 = true;
                        }
                        break;
                    case 1:
                        if (item.Value == "1")
                        {
                            RS2 = true;
                        }
                        break;
                    case 2:
                        if (item.Value == "1")
                        {
                            RS3 = true;
                        }
                        break;
                    case 3:
                        if (item.Value == "1")
                        {
                            RS4 = true;
                        }
                        break;
                    case 4:
                        if (item.Value == "1")
                        {
                            RS5 = true;
                        }
                        break;
                    case 5:
                        if (item.Value == "1")
                        {
                            RS6 = true;
                        }
                        break;
                }
            }

            EquipmentData.DeviceManager.ContnrRemoteSignalingStatusOutput(Convert.ToByte(EpitopeNo), RS1, RS2, RS3, RS4, RS5, RS6, Convert.ToByte(EpitopeNo) > LoadIni.LoadIni.AllMeterNumber ? 1 : 0);

        }


        public static void SETShhakeSate(int EpitopeNo)
        {
            List<bool> rs = new List<bool>();
            
            foreach (var item in ShakeStatusInfo.ShakeStatus[EpitopeNo - 1])
            {
                if (item.ToString()=="1") { 
                    rs.Add(true);
                }
                else
                {
                    rs.Add(false);
                }
            }
            DevOrderEven.DevOrderMsg("设置遥信状态：表位：" + EpitopeNo.ToString()+"状态:"+ rs[7].ToString() + "--" + rs[6].ToString() + "--" + rs[5].ToString() + "--" + rs[4].ToString() + "--" + rs[3].ToString() + "--" + rs[2].ToString()); 
            EquipmentData.DeviceManager.ContnrRemoteSignalingStatusOutput(Convert.ToByte(EpitopeNo),  rs[7], rs[6], rs[5], rs[4], rs[3], rs[2], Convert.ToByte(EpitopeNo) > LoadIni.LoadIni.AllMeterNumber ? 1 : 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EpitopeNo">表位号 FF广播</param>
        /// <param name="OutTriggerMode">触发方式:0电平1脉冲 |\告警 \-轮次1 \-轮次2\-轮次3\-轮次4</param>
        /// <param name="OutPutValue"></param>
        /// <param name="ID"></param>
        public static void ReadRemote(int EpitopeNo, out int[] OutTriggerMode, out int[] OutPutValue, int ID = 0)
        {
            EquipmentData.DeviceManager.ReadRemoteControl(Convert.ToByte(EpitopeNo), out OutTriggerMode, out OutPutValue, Convert.ToByte(EpitopeNo) > LoadIni.LoadIni.AllMeterNumber ? 1 : 0);

        }


        public static void SetRemote(int EpitopeNo,Dictionary<int,string> OutPutValue)
        {
            VerifyBase.InitTerminalTalks();

            setData = new byte[16][];


            //先去读取地址
            setData[EpitopeNo] = VerifyBase.Talkers[EpitopeNo].Framer698.ReadData_address("40010200");

            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, 5000);

            VerifyBase.Talkers[EpitopeNo].Framer698.str_Ter_Address = GetData(RecData, EpitopeNo, 5, EnumTerimalDataType.e_string);


            SetSessionData_698("070104800180000001" + VerifyBase.Talkers[0].Framer698.SetDateTimeBCD(DateTime.Now, false) + "010005", "下发保电解除命令", EpitopeNo);

            SetData_698("0602050231150800160031150900030100", "设置遥控事件有效", EpitopeNo);

            //         80 00 81 00代表跳闸 01 02代表有2个轮次 02 02代表第一个轮次里面的参数 51 f2 05 02 01 代表第一个轮次即继电器1号
            //07 01 2e 80 00 81 00 01 02 02 02 51 f2 05 02 01 16 00 02 02 51 f2 02 02 01 16 00 00
            int i = 0;
            string msg = null;
            foreach (var item in OutPutValue)
            {
                if (item.Value=="1")
                {
                    i++;
                    msg += "020251f205020" + i + "1600";
                }
            }

            string data = "07012e80008100010"+i+ msg+"01";
            SetSessionData_698(data + VerifyBase.Talkers[0].Framer698.SetDateTimeBCD(DateTime.Now, false) + "010005", "遥控拉闸命令", EpitopeNo);

        }



        public static bool SetSessionData_698(string cTaskData, string itemName,int EpitopeNo)
        {
            int ret = 0;
            VerifyBase.Talkers[EpitopeNo].Framer698.cTaskData = cTaskData.Replace(" ", "");


            VerifyBase.Talkers[EpitopeNo].Analysiser698.Analysis_apdu(VerifyBase.Talkers[EpitopeNo].Framer698.cTaskData, ref VerifyBase.Talkers[EpitopeNo].AnalysisedString, ref VerifyBase.Talkers[EpitopeNo].AlalysisedData);

            ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetSessionData(EpitopeNo, 3, VerifyBase.Talkers[EpitopeNo].Framer698.cTESAMNO, VerifyBase.Talkers[EpitopeNo].Framer698.cOutSessionKey, 3, VerifyBase.Talkers[EpitopeNo].Framer698.cTaskData, ref VerifyBase.Talkers[EpitopeNo].Framer698.cOutSID, ref VerifyBase.Talkers[EpitopeNo].Framer698.cOutAttachData, ref VerifyBase.Talkers[EpitopeNo].Framer698.cOutTaskData, ref VerifyBase.Talkers[EpitopeNo].Framer698.cOutTaskMAC);
            VerifyBase.Talkers[EpitopeNo].Framer698.sAPDU = "1001" + GetMiWenLen(VerifyBase.Talkers[EpitopeNo].Framer698.cOutTaskData) + VerifyBase.Talkers[EpitopeNo].Framer698.cOutTaskData + "00" + VerifyBase.Talkers[EpitopeNo].Framer698.cOutSID + "02" + VerifyBase.Talkers[EpitopeNo].Framer698.cOutAttachData + "04" + VerifyBase.Talkers[EpitopeNo].Framer698.cOutTaskMAC;

            setData[EpitopeNo] = VerifyBase.Talkers[EpitopeNo].Framer698.ReadData(VerifyBase.Talkers[EpitopeNo].Framer698.sAPDU);//应用连接
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, 20000);

            VerifyBase.Talkers[EpitopeNo].Framer698.cOutAttachData = "";
            VerifyBase.Talkers[EpitopeNo].Framer698.cOutMAC = "";
            if (TalkResult[EpitopeNo] == 0)
            {
                VerifyBase.Talkers[EpitopeNo].Framer698.cOutAttachData = GetData(RecData, EpitopeNo, 2, EnumTerimalDataType.e_string);
                VerifyBase.Talkers[EpitopeNo].Framer698.cOutMAC = GetMac(RecData, EpitopeNo);

                ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(EpitopeNo, VerifyBase.Talkers[EpitopeNo].Framer698.iKeyState, 3, VerifyBase.Talkers[EpitopeNo].Framer698.cTESAMNO, VerifyBase.Talkers[EpitopeNo].Framer698.cOutSessionKey, VerifyBase.Talkers[EpitopeNo].Framer698.cOutAttachData, VerifyBase.Talkers[EpitopeNo].Framer698.cOutMAC, ref VerifyBase.Talkers[EpitopeNo].Framer698.cOutData);
                VerifyBase.Talkers[EpitopeNo].Analysiser698.Analysis_apdu(VerifyBase.Talkers[EpitopeNo].Framer698.cOutData, ref VerifyBase.Talkers[EpitopeNo].AnalysisedString, ref VerifyBase.Talkers[EpitopeNo].AlalysisedData);

                if (GetMac(VerifyBase.Talkers[EpitopeNo].AnalysisedString, 0, "结果类型") != "00")
                {
                    return true;
                }
            }
            return false;

        }


        /// <summary>
        /// 设置数据通用方法
        /// </summary>
        /// <param name="cTaskData"></param>
        /// <param name="itemName"></param>
        public static void SetData_698(string cTaskData, string itemName, int EpitopeNo)
        {
            int ret = 0;

            VerifyBase.Talkers[EpitopeNo].Framer698.cTaskData = cTaskData.Replace(" ", "");


            VerifyBase.Talkers[EpitopeNo].Analysiser698.Analysis_apdu(VerifyBase.Talkers[EpitopeNo].Framer698.cTaskData, ref VerifyBase.Talkers[EpitopeNo].AnalysisedString, ref VerifyBase.Talkers[EpitopeNo].AlalysisedData);

            ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetTerminalSetData(EpitopeNo, 3, VerifyBase.Talkers[EpitopeNo].Framer698.cTESAMNO, VerifyBase.Talkers[EpitopeNo].Framer698.cOutSessionKey, VerifyBase.Talkers[EpitopeNo].Framer698.cTaskData, ref VerifyBase.Talkers[EpitopeNo].Framer698.cOutSID, ref VerifyBase.Talkers[EpitopeNo].Framer698.cOutAttachData, ref VerifyBase.Talkers[EpitopeNo].Framer698.cOutTaskData, ref VerifyBase.Talkers[EpitopeNo].Framer698.cOutTaskMAC);
            VerifyBase.Talkers[EpitopeNo].Framer698.sAPDU = "1001" + GetMiWenLen(VerifyBase.Talkers[EpitopeNo].Framer698.cOutTaskData) + VerifyBase.Talkers[EpitopeNo].Framer698.cOutTaskData + "00" + VerifyBase.Talkers[EpitopeNo].Framer698.cOutSID + "02" + VerifyBase.Talkers[EpitopeNo].Framer698.cOutAttachData + "04" + VerifyBase.Talkers[EpitopeNo].Framer698.cOutTaskMAC;

            setData[EpitopeNo] = VerifyBase.Talkers[EpitopeNo].Framer698.ReadData(VerifyBase.Talkers[EpitopeNo].Framer698.sAPDU);//应用连接
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, 20000);

            VerifyBase.Talkers[EpitopeNo].Framer698.cOutAttachData = "";
            VerifyBase.Talkers[EpitopeNo].Framer698.cOutMAC = "";
            VerifyBase.TempData[EpitopeNo].StdData = "00";
            if (TalkResult[EpitopeNo] == 0)
            {

                VerifyBase.Talkers[EpitopeNo].Framer698.cOutAttachData = GetData(RecData, EpitopeNo, 2, EnumTerimalDataType.e_string);
                VerifyBase.Talkers[EpitopeNo].Framer698.cOutMAC = GetMac(RecData, EpitopeNo);
                ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(EpitopeNo, VerifyBase.Talkers[EpitopeNo].Framer698.iKeyState, 3, VerifyBase.Talkers[EpitopeNo].Framer698.cTESAMNO, VerifyBase.Talkers[EpitopeNo].Framer698.cOutSessionKey, VerifyBase.Talkers[EpitopeNo].Framer698.cOutAttachData, VerifyBase.Talkers[EpitopeNo].Framer698.cOutMAC, ref VerifyBase.Talkers[EpitopeNo].Framer698.cOutData);


                VerifyBase.Talkers[EpitopeNo].Analysiser698.Analysis_apdu(VerifyBase.Talkers[EpitopeNo].Framer698.cOutData, ref VerifyBase.Talkers[EpitopeNo].AnalysisedString, ref VerifyBase.Talkers[EpitopeNo].AlalysisedData);

                VerifyBase.TempData[EpitopeNo].Data = GetMac(VerifyBase.Talkers[EpitopeNo].AnalysisedString, 0, "结果类型");
                if (GetMac(VerifyBase.Talkers[EpitopeNo].AnalysisedString, 0, "结果类型") == "00")
                    VerifyBase.TempData[EpitopeNo].Resoult = "合格";
                else
                {
                    VerifyBase.TempData[EpitopeNo].Tips = "通讯结果不正确";
                    VerifyBase.TempData[EpitopeNo].Resoult = "不合格";
                }
            }

        }
        public static string GetMac(Dictionary<int, string[]> RecData, int index)
        {
            try
            {
                string strReturn = "";
                string[] sRecData = RecData[index];
                for (int i = 0; i < sRecData.Length; i++)
                {
                    if (sRecData[i].Contains("MAC"))
                    {
                        strReturn = sRecData[i].Split('：')[1];
                        break;
                    }
                }
                return strReturn;
            }
            catch
            {
                return "9999";
            }
        }

        public static string GetMac(string[] sRecData, int index, string sData)
        {
            try
            {
                string strReturn = "";
                for (int i = 0; i < sRecData.Length; i++)
                {
                    if (sRecData[i].Contains(sData))
                    {
                        strReturn = sRecData[i + index].Split('：')[1];
                        break;
                    }
                }
                return strReturn;
            }
            catch
            {
                return "9999";
            }
        }


        /// <summary>
        /// 获取密文长度
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static string GetMiWenLen(string strData)
        {
            strData = strData.Replace(" ", "");
            string slen = ""; ;
            if (strData.Length / 2 > 127)
            {
                slen = "82" + (strData.Length / 2).ToString("x4");
            }
            else
                slen = (strData.Length / 2).ToString("x2");
            return slen;
        }

        public static string GetData(Dictionary<int, string[]> RecData, int index, int serinalNumber, EnumTerimalDataType e_type)
        {
            try
            {
                string strReturn = "";
                strReturn = RecData[index][serinalNumber].Split('：')[1];
                switch (e_type)
                {
                    case EnumTerimalDataType.e_byte:
                        return Convert.ToByte(strReturn).ToString();
                    case EnumTerimalDataType.e_int:
                        return Convert.ToInt32(strReturn).ToString();
                    case EnumTerimalDataType.e_float:
                        return Convert.ToSingle(strReturn).ToString();
                    case EnumTerimalDataType.e_double:
                        return Convert.ToDouble(strReturn).ToString();
                    case EnumTerimalDataType.e_datetime:
                        return Convert.ToDateTime(strReturn).ToString();
                    case EnumTerimalDataType.e_bs8:
                        if (strReturn.Length == 8)
                            return strReturn;
                        else
                            return "99999999";
                    case EnumTerimalDataType.e_bs16:
                        if (strReturn.Length == 16)
                            return strReturn;
                        else
                            return "9999999999999999";
                    case EnumTerimalDataType.e_string:
                        return strReturn;
                    default:
                        return "99999";
                }
            }
            catch
            {
                switch (e_type)
                {
                    case EnumTerimalDataType.e_byte:
                        return "99";
                    case EnumTerimalDataType.e_int:
                        return "99999";
                    case EnumTerimalDataType.e_float:
                        return "99999.9";
                    case EnumTerimalDataType.e_double:
                        return "99999.99";
                    case EnumTerimalDataType.e_datetime:
                        return "2000-1-1";
                    case EnumTerimalDataType.e_bs8:
                        return "99999999";
                    case EnumTerimalDataType.e_bs16:
                        return "9999999999999999";
                    case EnumTerimalDataType.e_string:
                        return "99999";
                    default:
                        return "99999";
                }
            }
        }
    }
}
