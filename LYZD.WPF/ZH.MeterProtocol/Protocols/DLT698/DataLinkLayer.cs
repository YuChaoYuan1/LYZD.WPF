using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZH.MeterProtocol.Protocols.ApplicationLayer;
using ZH.MeterProtocol.Protocols.DLT698.ApplicationLayer;
using ZH.MeterProtocol.Protocols.DLT698.Enum;

namespace ZH.MeterProtocol.Protocols.DLT698
{
    public class DataLinkLayer
    {

        public DataLinkLayer(string meteraddress)
        {
            MeterAddress = meteraddress;
        }
        public DataLinkLayer()
        { }
        /// <summary>
        /// 起始字符
        /// </summary>
        public const string StartByte = "68";

        /// <summary>
        /// 长度域
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// 控制域
        /// </summary>
        public string Control { get; set; }

        /// <summary>
        /// 地址域
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 帧头校验
        /// </summary>
        public string HCS { get; set; }

        /// <summary>
        /// 分帧格式域
        /// </summary>
        public string SubFramFormat { get; set; }

        /// <summary>
        /// 应用层
        /// </summary>
        public APDU APDU { get; set; }

        /// <summary>
        /// 帧尾校验
        /// </summary>      
        public string FCS { get; set; }

        /// <summary>
        /// 结束字符
        /// </summary>
        public const string EndByte = "16";



        /// <summary>
        /// 服务器实际地址
        /// </summary>
        public string MeterAddress = string.Empty;

        /// <summary>
        /// 根据服务类型组帧
        /// </summary>
        /// <param name="ServerType">服务类型</param>
        /// <returns>帧</returns>
        public byte[] PackFrame(EmServieType ServerType)
        {
            switch (ServerType)
            {
                case EmServieType.GET_Request:
                    ApduStr = APDU.ReadDataAPDU_Frame();
                    break;
                case EmServieType.SET_Request:
                    ApduStr = APDU.SetDataAPDU_Frame();
                    break;
                case EmServieType.ACTION_Request:
                    ApduStr = APDU.OperationAPDU_Frame();
                    break;
                case EmServieType.CONNECT_Request:
                    ApduStr = APDU.AppConnect();
                    break;
                default:
                    break;
            }
            SubFramFormat = string.Empty;

            Control = "43";//控制域
            Address = GroupAddress();//地址域
            FrameLen = 2 + 1 + Address.Length / 2 + 2 + ApduStr.Length / 2 + 2;
            Length = FrameLen.ToString("X").PadLeft(4, '0');
            Length = CommFun.Reverse(Length);//长度域
            HCS = CommFun.Reverse(CalcuteHCS());//帧头校验
            FCS = CommFun.Reverse(CalcuteFCS());//帧尾校验
            string StrFrame = GroupFrameStr();
            return CommFun.BytesFrom(StrFrame, FrameLen + 2);


        }



        /// <summary>
        /// 操作分帧组帧，针对密钥下装
        /// </summary>   
        /// <returns>帧</returns>
        public List<byte[]> PackOpertionSubFrame()
        {
            List<byte[]> Frames = new List<byte[]>();
            List<string> ApduFrams = new List<string>();
            bool FirstFrameFlag = true;//首帧标志
            bool MidFrameFlag = false;//中间帧标志

            ushort FirstFrame = 0;//首帧
            ushort LastFrame = 1;//末帧
            ushort MiddelFrame = 3;//中间帧
            ApduStr = APDU.OperationAPDU_Frame();
            string StrFrame = GroupFrameStr();
            Control = "63";//控制域
            Address = GroupAddress();//地址域
            if (StrFrame.Length / 2 > 450)//应用层400个字节分帧
            {
                int FrameNUm = (ApduStr.Length / 2) / 450;
                int LastFram = (ApduStr.Length / 2) % 450;
                for (int i = 0; i < FrameNUm; i++)
                {
                    string substr = ApduStr.Substring(i * 2 * 450, 900);
                    ApduFrams.Add(substr);
                }
                if (LastFram != 0)
                {
                    string lastFrame = ApduStr.Substring(FrameNUm * 2 * 450, LastFram * 2);
                    ApduFrams.Add(lastFrame);
                }

            }
            for (ushort SubFrameNo = 0; SubFrameNo < ApduFrams.Count; SubFrameNo++)
            {   //SubFrameNo ：分帧序号

                ApduStr = ApduFrams[SubFrameNo];
                if (MidFrameFlag)
                {
                    SubFramFormat = ((ushort)((MiddelFrame << 14) + SubFrameNo)).ToString("X").PadLeft(4, '0');
                }
                if (FirstFrameFlag)
                {
                    SubFramFormat = ((ushort)((FirstFrame << 14) + SubFrameNo)).ToString("X").PadLeft(4, '0'); ;
                    FirstFrameFlag = false;
                    MidFrameFlag = true;
                }
                if (SubFrameNo == (ushort)(ApduFrams.Count - 1))//末帧
                {
                    SubFramFormat = ((ushort)((LastFrame << 14) + SubFrameNo)).ToString("X").PadLeft(4, '0');
                    MidFrameFlag = false;

                }

                FrameLen = 2 + 1 + Address.Length / 2 + 2 + 2 + ApduStr.Length / 2 + 2;
                Length = FrameLen.ToString("X").PadLeft(4, '0');
                Length = CommFun.Reverse(Length);//长度域
                SubFramFormat = CommFun.Reverse(SubFramFormat);//分帧格式域
                HCS = CommFun.Reverse(CalcuteHCS());//帧头校验
                FCS = CommFun.Reverse(CalcuteFCS());//帧尾校验

                StrFrame = GroupFrameStr();
                Frames.Add(CommFun.BytesFrom(StrFrame, FrameLen + 2));
            }

            return Frames;

        }

        /// <summary>
        /// 计算帧头校验码
        /// </summary>
        /// <returns></returns>
        private string CalcuteHCS()
        {
            byte[] FcsData = CommFun.BytesFrom(Length + Control + Address, (Length + Control + Address).Length / 2);

            ushort hcs = CommFun.CRC16(FcsData, FcsData.Length);

            return hcs.ToString("X").PadLeft(4, '0');//校验和倒序不？？
        }

        /// <summary>
        /// 计算整帧校验码
        /// </summary>
        /// <returns></returns>
        private string CalcuteFCS()
        {
            byte[] FcsData = CommFun.BytesFrom(Length + Control + Address + HCS + SubFramFormat + ApduStr, (Length + Control + Address + HCS + SubFramFormat + ApduStr).Length / 2);

            ushort fcs = CommFun.CRC16(FcsData, FcsData.Length);

            return fcs.ToString("X").PadLeft(4, '0');//校验和倒序不？？
        }

        private string GroupFrameStr()
        {
            StringBuilder StrFrame = new StringBuilder();

            StrFrame.Append(StartByte);
            StrFrame.Append(Length);
            StrFrame.Append(Control);
            StrFrame.Append(Address);
            StrFrame.Append(HCS);
            if (!string.IsNullOrEmpty(SubFramFormat)) StrFrame.Append(SubFramFormat);
            StrFrame.Append(ApduStr);
            StrFrame.Append(FCS);
            StrFrame.Append(EndByte);

            return StrFrame.ToString();

        }

        private string GroupAddress()
        {
            string add;
            if (MeterAddress == "AA" || MeterAddress == "aa")
            {
                add = "C0AA00";
            }
            else if (MeterAddress.ToLowerInvariant() == "aaaaaaaaaaaa")
            {
                add = "45AAAAAAAAAAAA00";
            }
            else
            {
                MeterAddress = CommFun.Reverse(MeterAddress);
                add = "05" + MeterAddress + "10";
            }
            return add;

        }

        /// <summary>
        /// 解析帧
        /// </summary>
        /// <param name="Frame"></param>
        /// <param name="Errcode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ParseFrame(byte[] Frame, ref int Errcode, ref List<object> data)
        {
            Errcode = 0;
            bool isFz = false;
            byte[] adpu;
            if (!SubApduFrame(Frame, ref isFz, out adpu)) return false;

            byte[] ApduFrame;
            if (isFz)
            {
                ApduFrame = new byte[adpu.Length];
                Array.Copy(adpu, 2, ApduFrame, 0, ApduFrame.Length - 2);
            }
            else
                ApduFrame = adpu;

            EmServieType Type = (EmServieType)ApduFrame[0];
            switch (Type)
            {
                case EmServieType.GET_Response:
                    APDU = new ReadDataAPDU();
                    return APDU.ParseReadFrame(ApduFrame, ref data);

                case EmServieType.SET_Response:
                    APDU = new SetDataAPDU();
                    return APDU.ParseSetFrame(ApduFrame, ref Errcode);

                case EmServieType.ACTION_Response:
                    APDU = new OperationAPDU();
                    return APDU.ParseActionFrame(ApduFrame, ref Errcode);

                case EmServieType.CONNECT_Response:
                    APDU = new ApplicationConnectAPDU();
                    return APDU.ParseConnectionFrame(ApduFrame, ref Errcode, ref data);

                //安全传输响应
                case EmServieType.SECURITY_Response:
                    APDU = new SecurityAPDU();
                    return APDU.ParseSecurityResponse(ApduFrame, ref Errcode, ref data);
                default:
                    return false;
            }

        }

        /// <summary>
        /// 解析帧
        /// </summary>
        /// <param name="Frame"></param>
        /// <param name="Errcode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ParseFrame(byte[] Frame, ref int Errcode, ref List<object> CsdData, ref Dictionary<string, List<object>> DicObj)
        {
            Errcode = 0;
            bool isFz = false;
            byte[] adpu;
            if (!SubApduFrame(Frame, ref isFz, out adpu)) return false;

            byte[] ApduFrame;
            if (isFz)
            {
                ApduFrame = new byte[adpu.Length];
                Array.Copy(adpu, 2, ApduFrame, 0, ApduFrame.Length - 2);
            }
            else
                ApduFrame = adpu;

            EmServieType Type = (EmServieType)ApduFrame[0];
            switch (Type)
            {
                case EmServieType.GET_Response:
                    APDU = new ReadDataAPDU();
                    return APDU.ParseReadFrame(ApduFrame, ref CsdData, ref DicObj);

                case EmServieType.SET_Response:
                    APDU = new SetDataAPDU();
                    return APDU.ParseSetFrame(ApduFrame, ref Errcode);

                case EmServieType.ACTION_Response:
                    APDU = new OperationAPDU();
                    return APDU.ParseActionFrame(ApduFrame, ref Errcode);

                case EmServieType.CONNECT_Response:
                    APDU = new ApplicationConnectAPDU();
                    return APDU.ParseConnectionFrame(ApduFrame, ref Errcode, ref CsdData);

                //安全传输响应
                case EmServieType.SECURITY_Response:
                    APDU = new SecurityAPDU();
                    return APDU.ParseSecurityResponse(ApduFrame, ref Errcode, ref CsdData, ref DicObj);
                default:
                    return false;
            }

        }
        /// <summary>
        /// 解析分帧确认帧
        /// </summary>
        /// <param name="frame">帧</param>       
        /// <param name="frameNo">帧序号</param>
        /// <returns></returns>
        public bool ParseSubFrame(byte[] frame, int frameNo)
        {
            if (frame.Length <= 0) return false;
            bool isFz = false;
            byte[] apdu;
            if (!SubApduFrame(frame, ref isFz, out apdu)) return false;

            if (apdu.Length == 2)//确认帧只有分帧格式域
            {
                ushort SubFram = BitConverter.ToUInt16(apdu, 0);
                int RetFrameNo = SubFram & (0x0fff);//帧序号
                int FrameType = SubFram >> 14;//帧类型 2为确认帧
                if (FrameType == 2 && RetFrameNo == frameNo)
                {
                    return true;
                }
            }
            return false;

        }


        /// <summary>
        /// 截取APDU应用层报文
        /// </summary>
        /// <param name="Frame"></param>
        /// <param name="isFZ">分帧标识：true-分帧</param>
        /// <returns></returns>
        private bool SubApduFrame(byte[] Frame, ref bool isFZ, out byte[] apdu)
        {

            isFZ = false;
            apdu = new byte[0];

            int start = Array.IndexOf(Frame, (byte)0x68);

            if (start < 0 || Frame.Length < start + 6) return false; //报文长度不够

            int addrLen = Frame[start + 4] & 0x0F;//地址长度

            if ((Frame[start + 3] & 0x20) == 0x20) //分帧报文
                isFZ = true;

            int len = Frame.Length - (start + 11 + addrLen + 1);

            if (len < 1) return false;  //APDU长度不够

            apdu = new byte[len];
            Array.Copy(Frame, start + 8 + addrLen + 1, apdu, 0, len);

            return true;


        }




        private int FrameLen = 0;//长度域de 


        /// <summary>
        /// 应用层部分帧
        /// </summary>
        private string ApduStr = string.Empty;

    }
}
