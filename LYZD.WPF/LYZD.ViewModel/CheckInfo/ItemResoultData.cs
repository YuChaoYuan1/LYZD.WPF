using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LYZD.ViewModel.CheckInfo
{

    ///// <summary>
    ///// 分项结论数据
    ///// </summary>
    //public class ItemResoultData : ViewModelBase
    //{
    //    private ObservableCollection<ItemResoultDataFormat> itemResoulList = new ObservableCollection<ItemResoultDataFormat>();
    //    /// <summary>
    //    /// 虚拟表对象
    //    /// </summary>
    //    public ObservableCollection<ItemResoultDataFormat> ItemResoulList
    //    {
    //        get { return itemResoulList; }
    //        set { SetPropertyValue(value, ref itemResoulList, "ItemResoulList"); }
    //    }
    //}
    [Serializable]
    /// <summary>
    /// 分项结论数据
    /// </summary>
    public class ItemResoultDataFormat : ViewModelBase
    {
        ///// <summary>
        ///// 数量
        ///// </summary>
        //private static int count = 0;

        private int index = 1;
        /// <summary>
        /// 序号
        /// </summary>
        public int Index
        {
            get { return index; }
            set { SetPropertyValue(value, ref index, "Index"); }
        }
        private string name = "";
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetPropertyValue(value, ref name, "Name"); }
        }
        private string time = "";
        /// <summary>
        /// 检测时间
        /// </summary>
        public string Time
        {
            get { return time; }
            set { SetPropertyValue(value, ref time, "Time"); }
        }
        private string standardData = "";
        /// <summary>
        /// 标准数据
        /// </summary>
        public string StandardData
        {
            get { return standardData; }
            set { SetPropertyValue(value, ref standardData, "StandardData"); }
        }
        private string terminalData = "";
        /// <summary>
        /// 终端数据
        /// </summary>
        public string TerminalData
        {
            get { return terminalData; }
            set { SetPropertyValue(value, ref terminalData, "TerminalData"); }
        }
        private string resoult = "";
        /// <summary>
        /// 结论
        /// </summary>
        public string Resoult
        {
            get { return resoult; }
            set { SetPropertyValue(value, ref resoult, "Resoult"); }
        }
        private string tips = "";

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Tips
        {
            get { return tips; }
            set { tips = value; }
        }


        public ItemResoultDataFormat(string str)
        {
            string[] data = str.Split('|');
            Name = data[0];
            Time = data[1];
            StandardData = data[2];
            TerminalData = data[3];
            Resoult = data[4];
            Tips = data[5];
        }
    }
}
