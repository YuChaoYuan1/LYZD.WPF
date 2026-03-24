using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.ViewModel.CheckController
{
    public class TestTempData
    {

        private string data = "";

        /// <summary>
        /// 数据
        /// </summary>
        public string Data
        {
            get { return data; }
            set { data = value; }
        }

        private string resoult = "合格";   //默认合格

        /// <summary>
        /// 结论
        /// </summary>
        public string Resoult
        {
            get { return resoult; }
            set { resoult = value; }
        }


        private string stdData = "";
        /// <summary>
        /// 标准数据
        /// </summary>
        public string StdData
        {
            get { return stdData; }
            set { stdData = value; }
        }

        private string tips = "";
        /// <summary>
        /// 信息
        /// </summary>
        public string Tips
        {
            get { return tips; }
            set { tips = value; }
        }

        public override string ToString()
        {
            return $"{StdData}|{Data}|{Resoult}|{Tips}";
        }
    }
}
