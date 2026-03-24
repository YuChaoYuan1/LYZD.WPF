using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZH.MeterProtocol.Enum;
using ZH.MeterProtocol.SocketModule.Packet;
using ZH.MeterProtocol.Struct;

/// <summary>
/// 376.2 电能载波通信协议
/// </summary>
namespace ZH.MeterProtocol.Device
    {
        #region AFN=0x13,F1
        /// <summary>
        /// 路由数据转发召测信息AFN=0x13,F1
        /// </summary>
        internal class CL3762_RequestAFN13Packet : QGDW3762SendPacket
        {
            private int _645Lengh = 0;
            private string _645Frame = "";
            public override int WaiteTime()
            {
                return 12000;
            }
            public CL3762_RequestAFN13Packet(byte[] Frame645)
                : base(true)
            {
                AFN = 0x13;
                Fn = 1;
                _645Frame = BitConverter.ToString(Frame645);

                ByteBuffer buf = new ByteBuffer(Frame645);
                byte head = buf.Get();
                AreaA3 = buf.GetByteArray(6);

                ByteBuffer Pbuf = new ByteBuffer();
                Pbuf.Initialize();
                Pbuf.Put(ProtectType);
                Pbuf.Put(0x00);
                if (App.g_ChannelType ==Cus_ChannelType.通讯无线)
                    Pbuf.Put(0x00);     //无线微功率增加一字节
                _645Lengh = Frame645.Length;
                Pbuf.Put((byte)_645Lengh);
                Pbuf.Put(Frame645);
                PacketUserData = Pbuf.ToByteArray();
            }
            public override string GetPacketName()
            {
                return "CL3762_RequestAFN13Packet";
            }

            public override string GetPacketResolving()
            {
                return "下行，监控载波从节点。AFN:" + Convert.ToString(AFN, 16).PadLeft(2, '0') + " "
                                    + "FN(" + Convert.ToString(DT1, 16).PadLeft(2, '0') + Convert.ToString(DT2, 16).PadLeft(2, '0') + "):" + Fn + " "
                                    + "协议类型:" + ProtectType + " "
                                    + "内报文长度:" + _645Lengh + " "
                                    + "内报文:" + _645Frame + " ";
            }

        }

        /// <summary>
        /// 召测信息返回包
        /// </summary>
        internal class CL3762_RequestAFN13ReplayPacket : QGDW3762RecvPacket
        {
            public byte[] Frame645 { get; private set; }

            public override string GetPacketName()
            {
                return "CL3762_RequestAFN13ReplayPacket";
            }


            protected override void ParseBody(byte[] data)
            {
                //byte byt_AFC = 0x00;
                //byte[] bytRevFrame = new byte[0];
                //byte[] bytData = new byte[0];
                //Explain3762Frame(data, ref byt_AFC, ref bytRevFrame, ref bytData);

                //Frame645 = bytData;
                ByteBuffer buf = new ByteBuffer(data);

                //去掉A1
                byte[] A1 = buf.GetByteArray(6);
                //去掉A2
                byte[] A2 = buf.GetByteArray(6);
                //去掉A3
                byte[] A3 = buf.GetByteArray(5);

                if (data.Length >= 18)
                {
                    Frame645 = buf.GetByteArray(data.Length - 18);
                }
                else
                {
                    Frame645 = null;
                }

            }

            public override string GetPacketResolving()
            {
                return "上行，监控载波从节点。"
                                    + "内报文:" + "标注10001-12002" + " ";
            }

        }
        #endregion

        #region AFN=0x01,F1,2,3
        /// <summary>
        /// 初始化信息AFN=0x01,F1
        /// </summary>
        internal class CL3762_RequestAFN01Packet : QGDW3762SendPacket
        {
            public CL3762_RequestAFN01Packet(int fn)
                : base(true)
            {
                AFN = 0x01;
                AreaR[0] &= 0xFB;
                Fn = fn;

                PacketUserData = null;
            }
            public CL3762_RequestAFN01Packet(byte afn, int fn)
                : base(true)
            {
                AFN = afn;
                AreaR[0] &= 0xFB;
                Fn = fn;

                PacketUserData = null;
            }
            public override string GetPacketName()
            {
                return "CL3762_RequestAFN01Packet";
            }

        }

        /// <summary>
        /// 初始化信息返回包
        /// </summary>
        internal class CL3762_RequestAFN01ReplayPacket : QGDW3762RecvPacket
        {
            public byte r_AFN { get; private set; }
            public int Fn { get; private set; }
            public byte[] Data { get; private set; }
            public override string GetPacketName()
            {
                return "CL3762_RequestAFN01ReplayPacket";
            }


            protected override void ParseBody(byte[] data)
            {
                ByteBuffer buf = new ByteBuffer(data);

                //去掉AFN
                r_AFN = buf.Get();
                //去掉Fn
                byte DT1 = buf.Get();
                byte DT2 = buf.Get();
                Fn = DT2 * 8 + Convert.ToInt32(Math.Log(Convert.ToSingle(DT1), 2)) + 1;
                //
                Data = buf.GetByteArray(data.Length - 3);
            }
        }
        #endregion

        #region AFN=0x02,F1
        /// <summary>
        /// 数据转发召测信息AFN=0x02,F1
        /// </summary>
        internal class CL3762_RequestAFN02Packet : QGDW3762SendPacket
        {
            private int _645Lengh = 0;
            private string _645Frame = "";
            public override int WaiteTime()
            {
                return 2000;
            }

            /// <summary>
            /// 转发通信协议数据帧
            /// </summary>
            /// <param name="data">645或698完整数据指令</param>
            public CL3762_RequestAFN02Packet(byte[] data)
                : base(true)
            {
                AFN = 0x02;
                Fn = 1;

                //AreaR[0] = 0x0;

                _645Frame = BitConverter.ToString(data);

                ByteBuffer buf = new ByteBuffer(data);
                buf.Get();//去掉0x68
                AreaA3 = buf.GetByteArray(6); //获取目标地址


                ByteBuffer Pbuf = new ByteBuffer();
                Pbuf.Initialize();
                Pbuf.Put(ProtectType);

                _645Lengh = data.Length;
                Pbuf.Put((byte)_645Lengh);
                Pbuf.Put(data);
                PacketUserData = Pbuf.ToByteArray();
            }
            public override string GetPacketName()
            {
                return "CL3762_RequestAFN02Packet";
            }

            public override string GetPacketResolving()
            {
                return "下行，转发通信协议数据帧。AFN:" + Convert.ToString(AFN, 16).PadLeft(2, '0') + " "
                                    + "FN(" + Convert.ToString(DT1, 16).PadLeft(2, '0') + Convert.ToString(DT2, 16).PadLeft(2, '0') + "):" + Fn + " "
                                    + "协议类型:" + ProtectType + " "
                                    + "内报文长度:" + _645Lengh + " "
                                    + "内报文:" + _645Frame + " ";
            }
        }

        /// <summary>
        /// 召测信息返回包
        /// </summary>
        internal class CL3762_RequestAFN02ReplayPacket : QGDW3762RecvPacket
        {
            public byte[] Frame645 { get; private set; }

            public override string GetPacketName()
            {
                return "CL3762_RequestAFN02ReplayPacket";
            }


            protected override void ParseBody(byte[] data)
            {
                if (data.Length <= 18) return;
                ByteBuffer buf = new ByteBuffer(data);

                //去掉A1 源地址
                byte[] A1 = buf.GetByteArray(6);
                //去掉A2 目标地址
                byte[] A2 = buf.GetByteArray(6);
                //去掉A3 AFN AN 协议类型，报文长度
                byte[] A3 = buf.GetByteArray(5);

                Frame645 = buf.GetByteArray(data.Length - 18);

            }


            public bool Explain3762Frame(byte[] data, ref byte afn, ref byte[] bytRevFrame, ref byte[] bytData)
            {
                int iStart = -1;
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == 0x68)
                    {
                        iStart = i;
                        break;
                    }
                }
                if (iStart == -1) return false; //无起始码0x68

                int len = data[iStart + 1];    //376.2上行帧的总长度,长度为2字节，这里取1字节
                if (len != data.Length) return false;      //数据长度不够


                int chkSum = 0;
                for (int i = 3; i <= len - 3; i++)
                    chkSum = chkSum + data[i];
                if ((chkSum % 256) != data[iStart + len - 2]) return false; //校验码错误

                if (data[iStart + len - 1] != 0x16) return false;           //结束符


                if ((data[iStart + 4] & 0x4) == 0x4)        //通信模块状态，对从节点操作 有没有地址域
                    afn = data[iStart + 10 + 12];
                else
                    afn = data[iStart + 10];



                if (afn == 0x02)      //如果是转发指令
                {
                    if (len < iStart + 26)
                        return false;

                    int dataLen = data[iStart + 26];         //取出转发指令的下发数据包的长度

                    bytRevFrame = new byte[len - 1];
                    Array.Copy(data, iStart, bytRevFrame, 0, len);

                    bytData = new byte[dataLen - 1];
                    Array.Copy(bytRevFrame, 27, bytRevFrame, 0, dataLen);

                    return true;
                }
                else if (afn == 0x00) //确认/否认
                {
                    bytData = new byte[1];
                    bytData[0] = data[iStart + 10 + 3];
                    return true;
                }
                else if (afn == 0x13) //路由数据转发
                {
                    if (len < iStart + 26)
                        return false;

                    int dataLen = data[iStart + 26];         //取出转发指令的下发数据包的长度

                    bytRevFrame = new byte[len];
                    Array.Copy(data, iStart, bytRevFrame, 0, len);

                    bytData = new byte[dataLen];
                    Array.Copy(bytRevFrame, 27, bytData, 0, dataLen);

                    return true;


                }

                return true;
            }
            public override string GetPacketResolving()
            {
                return "上行，转发命令应答。内报文:" + "标注10001-12001" + " ";
            }

        }
        #endregion

        #region AFN=0x05,F1
        /// <summary>
        /// 控制命令信息AFN=0x05,F1
        /// </summary>
        internal class CL3762_RequestAFN05Packet : QGDW3762SendPacket
        {
            public CL3762_RequestAFN05Packet(int int_Fn, byte[] addr)
                : base(true)
            {
                AFN = 0x05;
                AreaR[0] &= 0xFB; //通信模块标识  为对主节点的操作
                Fn = int_Fn;

                PacketUserData = addr;
            }
            public override string GetPacketName()
            {
                return "CL3762_RequestAFN05Packet";
            }

        }
        /// <summary>
        /// 控制命令信息返回包
        /// </summary>
        internal class CL3762_RequestAFN05ReplayPacket : QGDW3762RecvPacket
        {
            public byte r_AFN { get; private set; }
            public int Fn { get; private set; }
            public byte[] Data { get; private set; }
            public override string GetPacketName()
            {
                return "CL3762_RequestAFN05ReplayPacket";
            }


            protected override void ParseBody(byte[] data)
            {
                ByteBuffer buf = new ByteBuffer(data);

                //去掉AFN
                r_AFN = buf.Get();
                //去掉Fn
                byte DT1 = buf.Get();
                byte DT2 = buf.Get();
                Fn = DT2 * 8 + Convert.ToInt32(Math.Log(Convert.ToSingle(DT1), 2)) + 1;
                //


            }

        }
        #endregion

        #region AFN=0x11,F1
        /// <summary>
        /// 路由设置AFN=0x11,F1
        /// </summary>
        internal class CL3762_RequestAFN11Packet : QGDW3762SendPacket
        {
            public CL3762_RequestAFN11Packet(int bwIndex, int fn, byte[] addr, CarrierWareInfo CarrierInfo)
                : base(true)
            {
                AFN = 0x11;
                AreaR[0] &= 0xFB;//通信模块标识  为对主节点的操作
                Fn = fn;
                //数据单元
                ByteBuffer buf = new ByteBuffer();
                buf.Initialize();

                buf.Put(addr);

                //设置父类数据
                PacketUserData = buf.ToByteArray();
            }
            public override string GetPacketName()
            {
                return "CL3762_RequestAFN11Packet";
            }

        }
        /// <summary>
        /// 控制命令信息返回包
        /// </summary>
        internal class CL3762_RequestAFN11ReplayPacket : QGDW3762RecvPacket
        {
            public byte r_AFN { get; private set; }
            public int Fn { get; private set; }
            public byte[] Data { get; private set; }
            public override string GetPacketName()
            {
                return "CL3762_RequestAFN11ReplayPacket";
            }


            protected override void ParseBody(byte[] data)
            {
                ByteBuffer buf = new ByteBuffer(data);

                //去掉AFN
                r_AFN = buf.Get();
                //去掉Fn
                byte DT1 = buf.Get();
                byte DT2 = buf.Get();
                Fn = DT2 * 8 + Convert.ToInt32(Math.Log(Convert.ToSingle(DT1), 2)) + 1;
                //


            }

        }
        #endregion

        #region AFN=0x10 Fn=F112 载波读取HPLC芯片ID
        /// <summary>
        /// 路由设置AFN=0x10,F112
        /// </summary>
        public class CL3762_RequestAFN10Packet : QGDW3762SendPacket
        {
            public CL3762_RequestAFN10Packet(int fn)
                : base(true)
            {
                AreaC = 0x43;//控制域C 1字节，0x43=‭01000011‬,下行报文，来自启动站，3=宽带载波通信

                AreaR[0] = 0x00;//带路由，无附加节点，对主节点的操作，不进行冲突检测，无中继
                AreaR[1] = 0x00;//不分信道，信道未编码
                AreaR[2] = 0x5F;//预计应答字节点，用于计算延时等待时间，0-为默认时间
                AreaR[3] = 0x00;//通信速率 2字节 ，0-默认通信速率
                AreaR[4] = 0x00;//
                AreaR[5] = 0x00;//报文序列号，

                AFN = 0x10;
                Fn = fn;

                //数据单元
                ByteBuffer buf = new ByteBuffer();
                buf.Initialize();
                buf.Put(0x01);
                buf.Put(0x00);

                //第1个是返回
                //buf.Put(Convert.ToByte(App.CUS.Meters.TestCount + 1));//节点数据n 模块数量


                //应用数据域
                PacketUserData = buf.ToByteArray();
            }

            public override string GetPacketName()
            {
                return "CL3762_RequestAFN10Packet";
            }

        }
        /// <summary>
        /// 控制命令信息返回包
        /// </summary>
        public class CL3762_RequestAFN10ReplayPacket : QGDW3762RecvPacket
        {
            public byte AFN { get; private set; }
            public int Fn { get; private set; }
            public Dictionary<string, string> Data { get; private set; }
            public override string GetPacketName()
            {
                return "CL3762_RequestAFN10ReplayPacket";
            }


            protected override void ParseBody(byte[] data)
            {
                Data = new Dictionary<string, string>();

                ByteBuffer buf = new ByteBuffer(data);

                //去掉AFN
                AFN = buf.Get();
                //去掉Fn
                byte DT1 = buf.Get();
                byte DT2 = buf.Get();
                Fn = DT2 * 8 + Convert.ToInt32(Math.Log(Convert.ToSingle(DT1), 2)) + 1;

                if (AFN != 0x10 || Fn != 112) return;

                buf.GetByteArray(4);

                int len = buf.Get();

                for (int i = 0; i < len; i++)
                {
                    string adds = ByteToString(buf.GetByteArray(6)); //节点地址
                    buf.Get();//节点设备类型 02表示集中器本地通信单元

                    string id = ByteToString(buf.GetByteArray(24));// 芯片ID信息 24字符
                    buf.Get(); //芯片软件版本信息 2字节
                    buf.Get();

                    Data.Add(adds, id);
                }
            }

            /// <summary>
            /// 转字符串
            /// </summary>
            /// <param name="bytes"></param>
            /// <returns></returns>
            private string ByteToString(byte[] bytes)
            {
                if (bytes == null) return "";

                string str = "";
                for (int i = 0; i < bytes.Length; i++)
                {
                    str += bytes[bytes.Length - 1 - i].ToString("X2");
                }
                return str;
            }

        }


        #endregion

        #region 376.2
        /// <summary>
        /// 376.2接收数据包基类
        /// </summary>
        public abstract class QGDW3762RecvPacket : RecvPacket
        {

            /// <summary>
            /// 解析数据包
            /// </summary>
            /// <param name="buf">缓冲区接收到的数据包内容</param>
            /// <returns>解析是否成功</returns>
            public override bool ParsePacket(byte[] buf)
            {
                //第一步，验证包长度
                //第二步，验证包结构
                //第三步，拆帧
                int startIndex = -1; //起始码0x68的位置
                for (int i = 0; i < buf.Length; i++)
                {
                    if (buf[i] == 0x68)
                    {
                        startIndex = i;
                        break;
                    }
                }
                if (startIndex == -1) return false; //无起始码

                int len = buf[startIndex + 2] * 0x100 + buf[startIndex + 1];
                if (buf.Length - startIndex < len) return false; //长度不够
                byte ctl = buf[startIndex + 3];



                ByteBuffer pack = new ByteBuffer(buf)
                {
                    Position = startIndex
                };

                //byte dataLength = pack.Get();
                //if (buf.Length < dataLength) return false;
                pack.Get(); //0x68
                pack.Get(); //长度 2字节
                pack.Get();
                pack.Get(); //控制码
                byte[] R = pack.GetByteArray(6); //信息域
                byte[] data = pack.GetByteArray(len - 11);
                byte chkCode = pack.Get();

                ParseBody(data);
                return true;
            }

            protected abstract void ParseBody(byte[] data);

            /// <summary>
            /// 计算检验码[帧头不进入检验范围]
            /// </summary>
            /// <param name="bytData"></param>
            /// <returns></returns>
            protected byte GetChkSum(byte[] bytData, int startPos, int length)
            {
                byte bytChkSum = 0;
                for (int i = startPos; i < length; i++)
                {
                    bytChkSum ^= bytData[i];
                }
                return bytChkSum;
            }

        }
        /// <summary>
        /// 376.2发送数据包基类
        /// </summary>
        public abstract class QGDW3762SendPacket : SendPacket
        {
            /// <summary>
            /// 控制域C表示报文的传输方向、启动标志和通信模块的通信方式类型信息，由1字节组成
            /// </summary>
            protected byte AreaC { get; set; }

            /// <summary>
            /// 信息域R, 6个字节
            /// </summary>
            public byte[] AreaR = new byte[6] { 0x04, 0x00, 0xFF, 0x00, 0x00, 0x00 };
            /// <summary>
            /// 地址域A1，源地址，6字节
            /// </summary>
            protected byte[] AreaA1 = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 };
            /// <summary>
            /// 地址域A2,中继地址，每个中继地址是6个字节，可以有多个中继器
            /// </summary>
            protected byte[] AreaA2 = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            /// <summary>
            /// 地址域A3，目标地址，6字节
            /// </summary>
            protected byte[] AreaA3 = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 };

            /// <summary>
            /// 应用层功能码 AFN 1字节
            /// </summary>
            protected byte AFN = 0x00;


            /// <summary>
            /// 由1开始，取值：0x01,0x02,0x04,0x08,0x10,0x20,0x40,0x80
            /// </summary>
            protected byte DT1 { get; private set; }

            /// <summary>
            /// 由0开始,取值：0-30值
            /// </summary>
            protected byte DT2 { get; private set; }

            /// <summary>
            /// 数据单元标识Fn:取值1-241
            /// </summary>
            protected int Fn
            {
                get
                {
                    int v = DT2 * 8;
                    if (DT1 == 0x01)
                        return v + 1;
                    else if (DT1 == 0x02)
                        return v + 2;
                    else if (DT1 == 0x04)
                        return v + 3;
                    else if (DT1 == 0x08)
                        return v + 4;
                    else if (DT1 == 0x10)
                        return v + 5;
                    else if (DT1 == 0x20)
                        return v + 6;
                    else if (DT1 == 0x40)
                        return v + 7;
                    else if (DT1 == 0x80)
                        return v + 8;
                    else
                        return 255;
                }
                set
                {
                    DT2 = Convert.ToByte((value - 1) / 8);
                    DT1 = Convert.ToByte(Math.Pow(2, (value - 1) % 8));
                }
            }

            /// <summary>
            /// 通信协议类型: 00H-透明传输, 01H - DL/T645-1997, 02H - DL/T645-2007, 03H - DL/T698.45, 04H - FFH保留
            /// </summary>
            protected byte ProtectType = 0x02;

            /// <summary>
            /// 用户数据域
            /// </summary>
            protected byte[] PacketUserData = null;


            public QGDW3762SendPacket(bool needReplay)
            {
                IsNeedReturn = needReplay;
                AreaR[0] |= App.CarrierInfo.IsRoute ? (byte)0 : (byte)1; //路由标识:0-带路由，1-傍路状态
                AreaC = App.CarrierInfo.IsBroad ? (byte)0x43 : (byte)0x41;
                //ProtectType = App.CUS.Meters.ProtocolName == "CDLT698" ? (byte)0x03 : (byte)0x02;

            }

            public override byte[] GetPacketData()
            {
                ByteBuffer buf1 = new ByteBuffer();
                //控制域C
                buf1.Put(AreaC);
                //信息域R
                buf1.Put(AreaR[0]);
                buf1.Put(AreaR[1]);
                buf1.Put(AreaR[2]);
                buf1.Put(AreaR[3]);
                buf1.Put(AreaR[4]);
                buf1.Put(AreaR[5]);

                //通信模块标识 ，对从节点操作需要源地址和目标地址
                if (0x04 == (AreaR[0] & 0x04))
                {
                    buf1.Put(AreaA1);//地址域
                                     //没有处理中继
                    buf1.Put(AreaA3);//地址域
                }
                //====应用层============================================
                buf1.Put(AFN);   //应用层功能码

                //信息类DT 数据单元标识
                buf1.Put(DT1);
                buf1.Put(DT2);

                if (PacketUserData != null)
                    buf1.Put(PacketUserData);    //数据域 
                                                 //===============================================================


                ByteBuffer buf = new ByteBuffer();
                buf.Initialize();
                buf.Put(0x68);        //帧头

                byte[] body = buf1.ToByteArray();
                ushort packetLength = (ushort)(body.Length + 5);//帧头+L+body+CS+结尾
                buf.PutUShort_S(packetLength);      //长度2字节
                buf.Put(body);

                byte chkSum = GetChkSum(buf.ToByteArray());
                buf.Put(chkSum);    //校验码
                buf.Put(0x16);      //帧尾
                return buf.ToByteArray();
            }

            private byte GetChkSum(byte[] bytData)
            {

                byte bytChkSum = 0;
                for (int i = 3; i < bytData.Length; i++)
                {
                    bytChkSum = (byte)(bytChkSum + bytData[i]);
                }

                return bytChkSum;
            }

        }
        #endregion
    }
