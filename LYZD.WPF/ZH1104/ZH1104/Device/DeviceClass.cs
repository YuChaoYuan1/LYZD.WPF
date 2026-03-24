using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ZH.SocketModule.Packet;
using ZH;
using ZH.Enum;
using ZH.Struct;

namespace ZH
{
    #region 功能板总指令

    #region 联机指令
    /// <summary>
    /// Z测试联机/脱机请求包
    /// </summary>
    internal class ZH1104_RequestLinkPacket : ZH1104SendPacket
    {
        public bool IsLink = true;

        public ZH1104_RequestLinkPacket(int bw)
            : base()
        {
            this.ToID = Convert.ToByte(bw);
        }

        /*
         * 81 30 PCID 09 a0 02 02 40 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x13);          //命令 
            buf.Put(0x88);
            buf.Put(0x0F);
            buf.Put(0x00);
            return buf.ToByteArray();
        }

        public override string GetPacketResolving()
        {
            string strResolve = "联机功能板。";
            return strResolve;
        }
    }
    /// <summary>
    /// 联机返回指令
    /// </summary>
    internal class ZH1104_RequestLinkReplyPacket : ZH1104RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length < 4)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[4] == 0x93)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
        public override bool ParsePacket(byte[] data)
        {
            if (data == null || data.Length < 4)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[4] == 0x93)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }

            return true;
        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion

    #region 2000H 接地故障试验控制继电器
    /// <summary>
    /// 接地故障试验控制继电器/脱机请求包
    /// </summary>
    internal class ZH1104_RequestJDGZPacket : ZH1104SendPacket
    {
        public int ua = 0;
        public int ub = 0;
        public int uc = 0;
        public int un = 0;
        public ZH1104_RequestJDGZPacket(int bw)
            : base()
        {
            this.ToID = Convert.ToByte(bw);
        }

        public void SetPara(int intUa, int intUb, int intUc, int intUn)
        {
            ua = intUa;
            ub = intUb;
            uc = intUc;
            un = intUn;
        }

        /*
         * ਁ68H+RID+FEH+LEN+13H+2000H+DATA+CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x13);          //命令 
            buf.Put(0x20);
            buf.Put(0x00);
            buf.PutInt(ua);
            buf.PutInt(ub);
            buf.PutInt(uc);
            buf.PutInt(un);
            return buf.ToByteArray();
        }

        public override string GetPacketResolving()
        {
            string strResolve = "联机功能板。";
            return strResolve;
        }
    }
    /// <summary>
    /// 接地故障试验控制继电器/返回指令
    /// </summary>
    internal class ZH1104_RequestJDGZReplyPacket : ZH1104RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length < 4)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[4] == 0x93)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion

    #region 2001H 载波检定台载波信号的链路切换
    /// <summary>
    ///  载波检定台载波信号的链路切换/脱机请求包
    /// </summary>
    internal class ZH1104_RequestZBSJPacket : ZH1104SendPacket
    {
        public int _path1 = 0;
        public int _path2 = 0;
        public int _path3 = 0;
        public int _path4 = 0;
        public ZH1104_RequestZBSJPacket(int bw)
            : base()
        {
            this.ToID = Convert.ToByte(bw);
        }

        public void SetPara(int path1, int path2, int path3, int path4)
        {
            _path1 = path1;
            _path2 = path2;
            _path3 = path3;
            _path4 = path4;
        }

        /*
         * ਁ68H+RID+FEH+LEN+13H+2000H+DATA+CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x13);          //命令 
            buf.Put(0x20);
            buf.Put(0x01);
            buf.PutInt(_path1);
            buf.PutInt(_path1);
            buf.PutInt(_path1);
            buf.PutInt(_path1);
            return buf.ToByteArray();
        }

        public override string GetPacketResolving()
        {
            string strResolve = " 载波检定台载波信号的链路切换能板。";
            return strResolve;
        }
    }
    /// <summary>
    ///  载波检定台载波信号的链路切换/返回指令
    /// </summary>
    internal class ZH1104_RequestZBSJReplyPacket : ZH1104RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length < 4)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[4] == 0x93)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion

    #region 3000H 交流功耗测试读指令
    /// <summary>
    ///  功耗测试联机/脱机请求包
    /// </summary>
    internal class ZH1104_ReadGHDataRelayPacket : ZH1104SendPacket
    {

        //        3000H—读取电压回路、电流回路计算结果参数
        //发送：68H+RID+FEH+LEN+10H+3000H+CS
        //LEN：0x08

        //返回：68H+FEH+RID+LEN+90H+REG+DATA0+DATA1….DATAn+CS

        public ZH1104_ReadGHDataRelayPacket(int bw)
            : base()
        {
            this.ToID = Convert.ToByte(bw);
        }


        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x10);          //命令 
            buf.Put(0x30);
            buf.Put(0x00);
            return buf.ToByteArray();
        }

        public override string GetPacketResolving()
        {
            string strResolve = " 。";
            return strResolve;
        }
    }
    /// <summary>
    ///交流功耗测试，联机返回指令
    /// </summary>
    internal class ZH1104_ReadGHDataReplyPacket : ZH1104RecvPacket
    {

        /// <summary>
        /// 获取源信息
        /// </summary>
        /// <returns></returns>
        //public float[] fldata  { get; private set; }
        //public override bool ParsePacket(byte[] buf)
        public float[] Fldata { get; private set; }
        protected override void ParseBody(byte[] data)
        {

        }

        public override bool ParsePacket(byte[] data)
        {
            bool resul = false;
            ByteBuffer buf = new ByteBuffer(data);
            if (data == null)
                ReciveResult = RecvResult.DataError;
            else
            {

                ReciveResult = RecvResult.Unknow;

                if (data[4] == 0x90)
                {
                    Fldata = new float[15];
                    ReciveResult = RecvResult.OK;
                    resul = true;
                    buf.Get(); //68
                    buf.Get();//toID
                    buf.Get();//myID
                    buf.Get();//len
                    buf.Get();// 90
                    buf.Get();// 30
                    buf.Get();//00

                    for (int i = 0; i < Fldata.Length; i++)
                    {

                        Fldata[i] = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArrayRev(4, true), 0) / 100000.0000f);
                    }

                }
            }
            return resul;
        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion

    #region 3001H 读取直流功耗
    /// <summary>
    ///  读取直流功耗/脱机请求包
    /// </summary>
    internal class ZH1104_RequestZLGHPacket : ZH1104SendPacket
    {
        public int _path1 = 0;
        public int _path2 = 0;
        public int _path3 = 0;
        public int _path4 = 0;
        public ZH1104_RequestZLGHPacket(int bw)
            : base()
        {
            this.ToID = Convert.ToByte(bw);
        }



        /*
         * ਁ68H+RID+FEH+LEN+13H+2000H+DATA+CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x10);          //命令 
            buf.Put(0x30);
            buf.Put(0x01);
            return buf.ToByteArray();
        }

        public override string GetPacketResolving()
        {
            string strResolve = " 载波检定台载波信号的链路切换能板。";
            return strResolve;
        }
    }
    /// <summary>
    ///  读取直流功耗/返回指令
    /// </summary>
    internal class ZH1104_RequestZLGHReplyPacket : ZH1104RecvPacket
    {
        public float Fldata { get; private set; }
        public override bool ParsePacket(byte[] data)
        {
            Fldata = 999.99F;
            bool resul = false;
            ByteBuffer buf = new ByteBuffer(data);
            if (data == null || data.Length < 4)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[4] == 0x90)
                {
                    ReciveResult = RecvResult.OK;
                    resul = true;
                    if (buf.Length > 4)
                    {
                        buf.Get();
                        buf.Get();
                        buf.Get();
                        buf.Get();
                        buf.Get();
                        buf.Get();
                        buf.Get();
                        Fldata = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArrayRev(4, true), 0) / 100000.0000f);
                    }

                }
                else
                    ReciveResult = RecvResult.Unknow;
            }

            return resul;
        }
        protected override void ParseBody(byte[] data)
        {

        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion


    #region 4000H 零线电流切换寄存器
    /// <summary>
    ///  零线电流切换寄存器/脱机请求包
    /// </summary>
    internal class ZH1104_RequestLXDLPacket : ZH1104SendPacket
    {
        public int _iA = 0;
        public int _iN = 0;

        public ZH1104_RequestLXDLPacket(int bw)
            : base()
        {
            this.ToID = Convert.ToByte(bw);
        }

        public void SetPara(int IA, int IN)
        {
            _iA = IA;
            _iN = IN;

        }

        /*
         * ਁ68H+RID+FEH+LEN+13H+4000H+DATA+CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x13);          //命令 
            buf.Put(0x40);
            buf.Put(0x00);
            buf.PutInt(_iA);
            buf.PutInt(_iN);
            return buf.ToByteArray();
        }

        public override string GetPacketResolving()
        {
            string strResolve = " 零线电流切换寄存器。";
            return strResolve;
        }
    }
    /// <summary>
    ///  零线电流切换寄存器/返回指令
    /// </summary>
    internal class ZH1104_RequestLXDLReplyPacket : ZH1104RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 4)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x93)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }





    #endregion

    //add yjt 20230103 新增零线电流控制启停
    #region 零线电流控制启停
    /// <summary>
    /// 零线电流控制启停
    /// </summary>
    internal class ZH1104_StartZeroCurrentLinkPacket : ZeroCurrentSendPacket
    {
        public int _A_kz = 0;
        public int _BC_kz = 0;
        public bool IsLink = true;

        public ZH1104_StartZeroCurrentLinkPacket()
            : base()
        {

        }

        public void SetPara(int A_kz, int BC_kz)
        {
            _A_kz = A_kz;
            _BC_kz = BC_kz;
        }

        /*
         * 68 01 FE 0A 13 40 00 00 01 A7
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x13);          //命令 
            buf.Put(0x40);
            buf.Put(0x00);
            buf.Put(Convert.ToByte(_A_kz));
            buf.Put(Convert.ToByte(_BC_kz));
            return buf.ToByteArray();
        }

        public override string GetPacketResolving()
        {
            string strResolve = "联机标准表。";
            return strResolve;
        }
    }
    /// <summary>
    /// 零线电流控制启停
    /// </summary>
    internal class ZH1104_StartZeroCurrentLinkReplyPacket : ZeroCurrentRecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 4)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x93)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion

    #region 5000H 设置载波模块工作电压
    /// <summary>
    ///  设置载波模块工作电压/脱机请求包
    /// </summary>
    internal class ZH1104_RequestZBGZDYPacket : ZH1104SendPacket
    {
        public byte[] _setData = new byte[12];
        public int _u = 0;

        public ZH1104_RequestZBGZDYPacket(int bw)
            : base()
        {
            this.ToID = Convert.ToByte(bw);
        }

        public void SetPara(int U, byte[] setData)
        {
            _u = U;
            _setData = setData;
        }

        /*
         * ਁ68H+RID+FEH+LEN+13H+5000H+DATA+CS     DATA=12bytes 
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x13);          //命令 
            buf.Put(0x50);
            buf.Put(0x00);
            buf.PutInt(_u);
            buf.Put(_setData);
            return buf.ToByteArray();
        }

        public override string GetPacketResolving()
        {
            string strResolve = " 设置载波模块工作电压。";
            return strResolve;
        }
    }
    /// <summary>
    ///  零线电流切换寄存器/返回指令
    /// </summary>
    internal class ZH1104_RequestZBGZDYReplyPacket : ZH1104RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length < 4)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[4] == 0x93)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion

    #endregion


}
