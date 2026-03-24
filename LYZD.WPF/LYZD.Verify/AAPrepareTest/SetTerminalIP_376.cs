using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.AAPrepareTest
{
    /// <summary>
    /// 终端IP设置
    /// </summary>
    public class SetTerminalIP_376 : VerifyBase
    {
        public override void Verify()
        {

            //切换通讯方式
            bool IsSetConnType = false;
            Cus_EmChannelType ConnType = TerminalChannelType;
            if (TerminalChannelType == Cus_EmChannelType.ChannelEther || TerminalChannelType == Cus_EmChannelType.ChannelGPRS)
            {
                IsSetConnType = true;
                //MessageAdd("请使用维护口或232通讯",EnumLogType.错误信息);
                //TryStopTest();//停止检定
                //return;
            }
            if (IsSetConnType)
            {
                TerminalChannelType = Cus_EmChannelType.Channel232;
                InitTerminalTalks();
            }
            base.StartVerify();

            //这里是否可以考虑设置成232通讯，设置完成后在切换回来

            //SetData_698("06 01 02 45 10 01 00 00 09 06 C0 A8 72 9A", "设置终端IP");


            // 终端对时
            MessageAdd("正在进行终端对时...",EnumLogType.流程信息);

            byte[][] arr_byt_setData = new byte[MeterNumber][];

            string strAddress = VerifyConfig.Tcp_Ip;
            string strPort = VerifyConfig.Tcp_Port;


            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {

                    arr_byt_setData[i] = Talkers[0].Framer.GetFnToByte_Afn04(3, strAddress + "," + (Convert.ToInt16(strPort) + i));
                }
            }

            TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 3, arr_byt_setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "Fn1";
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                        if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                        {
                            TempData[i].Tips = "回复不正确！";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                    else
                    {
                        TempData[i].Tips = "无回复！";
                        TempData[i].Resoult = "不合格";
                    }

                }
            }
            AddItemsResoult("主站IP地址和端口", TempData);
            WaitTime("等待", 5);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (TalkResult[i] == 0)
                {
                    strAddress = strAddress.Split('.')[0] + "." + strAddress.Split('.')[1] + "." + strAddress.Split('.')[2] + "." + (101 + i);
                    arr_byt_setData[i] = Talkers[0].Framer.GetFnToByte_Afn04(7, strAddress);
                }
            }

            TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 7, arr_byt_setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (TalkResult[i] == 0)
                {
                    TempData[i].StdData = "Fn1";
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                        if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                        {
                            TempData[i].Tips = "回复不正确！";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                    else
                    {
                        TempData[i].Tips = "无回复！";
                        TempData[i].Resoult = "不合格";
                    }

                }
            }

            AddItemsResoult("终端IP地址和端口", TempData);



            ///恢复成原来的通讯方式
            if (IsSetConnType)
            {
                TerminalChannelType = ConnType;
                InitTerminalTalks();
            }

        }
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "主站IP地址和端口", "终端IP地址和端口", "结论" };
            return true;
        }
    }
}
