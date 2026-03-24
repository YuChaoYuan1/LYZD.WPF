using LYZD.Core.Enum;
using LYZD.DAL.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.ViewModel.CheckInfo
{
    /// 台体信息视图
    /// <summary>
    /// 台体信息视图
    /// </summary>
    public class EquipmentViewModel : ViewModelBase
    {

        private TerminalProtocolTypeEnum terminalProtocolType;

        /// <summary>
        /// 台体和终端之间使用的通讯协议
        /// </summary>
        public TerminalProtocolTypeEnum TerminalProtocolType
        {
            get { return terminalProtocolType; }
            set
            {
                terminalProtocolType = value;
                //协议切换，切换显示的数据
                for (int i = 0; i < EquipmentData.CheckResults.ResultCollection.Count; i++)
                {
                    CheckNodeViewModel nodeTemp = EquipmentData.CheckResults.ResultCollection[i];
                    //修改所有表的显示结论
                    for (int j = 0; j < nodeTemp.CheckResults.Count; j++)
                    {
                        var v = nodeTemp.CheckResults[j].GetProperty("分项结论376");
                        if (v!=null)
                        {
                            nodeTemp.CheckResults[j].SetItemResoultProperty("分项结论376", v);
                        }
                        v = nodeTemp.CheckResults[j].GetProperty("分项结论698");
                        if (v != null)
                        {
                            nodeTemp.CheckResults[j].SetItemResoultProperty("分项结论698", v);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 是否是演示模式
        /// </summary>
        public bool IsDemo { get; set; }


        /// <summary>
        /// 启动和潜动的时候同步进行通讯协议检查试验
        /// </summary>
        public bool IsSame = ConfigHelper.Instance.IsSame;

        private string equipmentType = ConfigHelper.Instance.EquipmentType;
        /// 台体类型
        /// <summary>
        /// 台体类型
        /// </summary>
        public string EquipmentType
        {
            get
            {
                return equipmentType;
            }
            set
            {
                SetPropertyValue(value, ref equipmentType, "EquipmentType");
            }
        }

        public bool AutoLogion = ConfigHelper.Instance.AutoLogin;

        private int meterCount = ConfigHelper.Instance.MeterCount;
        /// 表位数量
        /// <summary>
        /// 表位数量
        /// </summary>
        public int MeterCount
        {
            get { return meterCount; }
            set
            {
                SetPropertyValue(value, ref meterCount, "MeterCount");
                CheckController.MeterProtocolAdapter.Instance.SetBwCount(value);
            }
        }

        /// <summary>
        /// 南网设备厂家
        ///// </summary>
        //private string southManufacturers = ConfigHelper.Instance.SouthManufacturers;
        //public string SouthManufacturers
        //{
        //    get { return southManufacturers; }
        //    set { SetPropertyValue(value, ref southManufacturers, "SouthManufacturers"); }
        //}

       
        private string meterType = ConfigHelper.Instance.MeterType;
        private string verifyModel = ConfigHelper.Instance.VerifyModel;


        /// <summary>
        /// 程序检测类型(终端，电能表)
        /// </summary>
        public string MeterType
        {
            get { return meterType; }
            set { SetPropertyValue(value, ref meterType, "MeterType"); }
        }
        /// <summary>
        /// 程序检测类型(终端，电能表)
        /// </summary>
        public string VerifyModel
        {
            get { return verifyModel; }
            set { SetPropertyValue(value, ref verifyModel, "VerifyModel"); }
        }




        private string id = ConfigHelper.Instance.EquipmentNo;
        /// 台体编号
        /// <summary>
        /// 台体编号
        /// </summary>
        public string ID
        {
            get { return id; }
            set { SetPropertyValue(value, ref id, "ID"); }
        }

        private string textLogin;
        /// <summary>
        /// 登录时显示的文本
        /// </summary>
        public string TextLogin
        {
            get { return textLogin; }
            set { SetPropertyValue(value, ref textLogin, "TextLogin"); }
        }

        private int progressBarValue;
        /// <summary>
        /// 登入时候，进度条进度
        /// </summary>
        public int ProgressBarValue
        {
            get { return progressBarValue; }
            set { SetPropertyValue(value, ref progressBarValue, "ProgressBarValue"); }
        }




    }
}
