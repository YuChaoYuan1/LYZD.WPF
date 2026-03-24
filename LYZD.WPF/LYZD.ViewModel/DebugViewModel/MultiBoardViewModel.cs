using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.ViewModel.DebugViewModel
{
    #region 多功能板
    /// <summary>
    /// 多功能板
    /// </summary>
    public class MultiBoardViewModel : ViewModelBase
    {

        private int dColor = 0;
        /// <summary>
        /// 开始编号
        /// </summary>
        public int DColor
        {
            get { return dColor; }
            set
            { SetPropertyValue(value, ref dColor, "DColor"); }
        }
        private int dType = 0;
        /// <summary>
        /// 开始编号
        /// </summary>
        public int DType
        {
            get { return dType; }
            set
            { SetPropertyValue(value, ref dType, "DType"); }
        }
        /// <summary>
        /// 切换三色灯
        /// </summary>
        public void SetSSD()
        {

            Utility.TaskManager.AddDeviceAction(() =>
            {
                EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x10, (byte)0x01);
            });

        }

    }
    #endregion
}
