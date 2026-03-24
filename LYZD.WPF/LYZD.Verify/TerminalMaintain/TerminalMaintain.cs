using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.TerminalMaintain
{
    /// <summary>
    /// 终端维护
    /// </summary>
     public class TerminalMaintain : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //检定项目376和698通用的部分-因为结论差异较大，大部分用检定数据，加上分割就好。主要数据显示在分项结论里面-因为是动态的
            //那个参数用于存储检定数据-这里可能要拼接，拼接一下需要刷新一次
            //分项有数据-名称-标准-读到的数据，这里标准数据存在个差异,结论因为都是用的一个参数，所以都得改-或者刷新完之后重新赋值
            //总结论-目前是一个参数保存，检定项目结束来刷新一下结论--这里可能改到添加分项结论里面-默认合格-有一个不是合格就改成成不合格-加上空的其他情况所以用！=
            //检定完成后需要刷新结论，那么是否可以在基类里面刷新,如果异常或什么情况是否刷个不合格

            //问题：
            //1:用什么分割比较好,|这个不怎么明显，其他的话【】会不会因为后面数据太长影响到--
            //2：详细数据刷新只刷数据呢还是刷结论--比如协议检查【合格】--协议检查【00000】--目前看刷结论比较好-怎么显示待定
            //3：数据显示在一起了,数据管理里面显示怎么办--还有分项结论是否要分割显示
            //4：上传数据和报表打印那块后期需要在调整了--必须改了--报表那个分项结论该怎么办，没办法做模板--后期是否要加上根据分项编号获取


            //新办法：做俩个视图-001和001_376
            //因为检定数据都是存放在一个数据字段里面使用^分割开,所以归根结底他就是一个字段-只要修改显示就行
            //是否可以考虑在方案数据库弄个字段存储使用的协议--开始检定项目的时候保存一下
            //切换显示的时候,直接获取这个显示-这个如果是空就说明还没保存-判断一下显示那个-根据当前的协议--没有读取到就用另一个
            //---暂时不用，因为视图是方案一开始就加载好了--这个可以改-把376和698做到一个视图里面，根据情况需不需要显示
            //确定可用，麻烦的是376和698通用的部分,有可能会因为选错协议导致刷新数据出错,或者一半698检定的一半376检定的情况都有可能
            //目前想的解决方法是在方案里面加个标识,开始检定就根据协议保存这个标识，标识改变就修改结论视图，

            //解决方法3：三种检定数据，一个结论
            //结论设置里面配置3种的结论，根据情况显示哪一种
            //检定数据里面保存3个的检定数据,根据使用的协议来显示那个
            //问题：结论会不会因为之前用698现在用376，这里切换的时候导致结论错误
            ResultNames = new string[] { "上1次终端初始化事件", "终端初始化事件当前记录数", "终端维护", "终端对时", "结论" };
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
                SetData_698("06010531000900030100", "设置事件有效标志");
                DateTime dtHappen = DateTime.Now.AddMinutes(-1);
                SetTime_698(DateTime.Now, 0);
                ResetTerimal_698(2);
                ConnectLink2(false);


                MessageAdd("正在读取上1次终端初始化事件",EnumLogType.流程信息);
                MessageAdd("正在读取上1次终端初始化事件",EnumLogType.提示信息);
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
                        TempData[i].StdData=dtHappen.ToString();
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

                            if (GetDateTime(GetData(RecData, i, 13, EnumTerimalDataType.e_datetime)) < dtHappen)
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



                ResetTerimal_698(1);
                ConnectLink2(false);
                MessageAdd("读取终端初始化事件当前记录数",EnumLogType.流程信息);
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
                ResetTerimal_698(3);
            }
            catch (Exception ex)
            {
                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
                      
