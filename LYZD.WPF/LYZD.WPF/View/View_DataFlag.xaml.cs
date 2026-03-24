using LYZD.DAL;
using LYZD.Utility.Log;
using LYZD.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace LYZD.WPF.View
{
    /// <summary>
    /// View_DataFlag.xaml 的交互逻辑
    /// </summary>
    public partial class View_DataFlag 
    {
        public View_DataFlag()
        {
            InitializeComponent();
            Name = "数据标识";
            LoadXmlFromFile();
        }
        private XmlDataProvider dataFlagDict
        { get { return Resources["DataFlag"] as XmlDataProvider; } }

        /// <summary>
        /// 添加的数据标识的内容
        /// </summary>
        private DataFlagItem flagItemTemp
        {
            get { return Resources["DataItemTemp"] as DataFlagItem; }
        }

        private string filePath = string.Format(@"{0}\Xml\DataFlag.xml", Directory.GetCurrentDirectory());
        /// <summary>
        /// 从文件加载
        /// </summary>
        private void LoadXmlFromFile()
        {
            dataFlagDict.Source = new Uri(filePath, UriKind.Absolute);
        }


        //保存
        private void Click_Save(object sender, RoutedEventArgs e)
        {
            dataFlagDict.Document.Save(filePath);
            CodeDictionary.LoadDataFlagNames();
            LogManager.AddMessage("保存成功", EnumLogSource.用户操作日志, EnumLevel.Tip);
        }

        //添加
        private void Click_Add(object sender, RoutedEventArgs e)
        {
            XmlDocument docTemp = dataFlagDict.Document;
            XmlNode nodeParent = docTemp.DocumentElement;
            for (int i = 0; i < nodeParent.ChildNodes.Count; i++)
            {
                try
                {
                    if (nodeParent.ChildNodes[i].Attributes["DataFlagName"].Value == flagItemTemp.Name)
                    {
                        LogManager.AddMessage("该标识名称已经存在,请更改标识名称!!", EnumLogSource.用户操作日志, EnumLevel.Tip);
                        return;
                    }
                }
                catch
                { }
            }
            XmlElement dataItem = docTemp.CreateElement("R");
            XmlAttribute attributeName = docTemp.CreateAttribute("DataFlagName");
            dataItem.Attributes.Append(attributeName);
            dataItem.Attributes["DataFlagName"].Value = flagItemTemp.Name;

            XmlAttribute attributeDataFlag = docTemp.CreateAttribute("DataFlag");
            dataItem.Attributes.Append(attributeDataFlag);
            dataItem.Attributes["DataFlag"].Value = flagItemTemp.DataFlag;

            XmlAttribute attributeDataFlag698 = docTemp.CreateAttribute("DataFlag698");
            dataItem.Attributes.Append(attributeDataFlag698);
            dataItem.Attributes["DataFlag698"].Value = flagItemTemp.DataFlag698;

            XmlAttribute attributeLength = docTemp.CreateAttribute("DataLength");
            dataItem.Attributes.Append(attributeLength);
            dataItem.Attributes["DataLength"].Value = flagItemTemp.Length;

            XmlAttribute attributeDotNumber = docTemp.CreateAttribute("DataSmallNumber");
            dataItem.Attributes.Append(attributeDotNumber);
            dataItem.Attributes["DataSmallNumber"].Value = flagItemTemp.DotLength;

            XmlAttribute attributeFormat = docTemp.CreateAttribute("DataFormat");
            dataItem.Attributes.Append(attributeFormat);
            dataItem.Attributes["DataFormat"].Value = flagItemTemp.DataFormat;

            XmlAttribute readData = docTemp.CreateAttribute("ReadData");
            dataItem.Attributes.Append(readData);
            dataItem.Attributes["ReadData"].Value = flagItemTemp.ReadData;

            XmlAttribute attributeDefault = docTemp.CreateAttribute("Default");
            dataItem.Attributes.Append(attributeDefault);
            dataItem.Attributes["Default"].Value = flagItemTemp.DataFormat;

            //DataFlag698
            //ReadData
            nodeParent.AppendChild(dataItem);
            LogManager.AddMessage("标识名称已添加,点击保存按钮后生效", EnumLogSource.用户操作日志, EnumLevel.Tip);
        }

        private void Click_Add2(object sender, RoutedEventArgs e)
        {
            XmlDocument docTemp = dataFlagDict.Document;
            XmlNode nodeParent = docTemp.DocumentElement;
            System.Data.DataTable dataTable = ZH.MeterProtocol.MeterProtocal.Protocals;
            string name = "";
            string Flag645 = "";
            string Flag698 = "";
            string len = "";
            string xs = "";
            string gs = "";


            bool t;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                t = false;
                System.Data.DataRow Row = dataTable.Rows[i];
                name = Row.ItemArray[6].ToString();
                Flag645 = Row.ItemArray[0].ToString();
                Flag698 = Row.ItemArray[3].ToString();
                len = Row.ItemArray[1].ToString();
                xs = Row.ItemArray[2].ToString();
                gs = Row.ItemArray[7].ToString();

                if (name=="(上5次)阶梯切换冻结记录")
                {

                }
                for (int j = 0; j < nodeParent.ChildNodes.Count; j++)
                {
                    try
                    {
                        if (nodeParent.ChildNodes[j].Attributes["DataFlagName"].Value == name)
                        {
                            LogManager.AddMessage("该标识名称已经存在,请更改标识名称!!", EnumLogSource.用户操作日志, EnumLevel.Tip);
                            t = true;
                            break;
                        }
                    }
                    catch
                    { }
                }     //判断是否已经存在了吗，存在就不用
                if (t)  continue;

                XmlElement dataItem = docTemp.CreateElement("R");
                XmlAttribute attributeName = docTemp.CreateAttribute("DataFlagName");
                dataItem.Attributes.Append(attributeName);
                dataItem.Attributes["DataFlagName"].Value = name;

                XmlAttribute attributeDataFlag = docTemp.CreateAttribute("DataFlag");
                dataItem.Attributes.Append(attributeDataFlag);
                dataItem.Attributes["DataFlag"].Value = Flag645;

                XmlAttribute attributeDataFlag698 = docTemp.CreateAttribute("DataFlag698");
                dataItem.Attributes.Append(attributeDataFlag698);
                dataItem.Attributes["DataFlag698"].Value = Flag698;

                XmlAttribute attributeLength = docTemp.CreateAttribute("DataLength");
                dataItem.Attributes.Append(attributeLength);
                dataItem.Attributes["DataLength"].Value = len;

                XmlAttribute attributeDotNumber = docTemp.CreateAttribute("DataSmallNumber");
                dataItem.Attributes.Append(attributeDotNumber);
                dataItem.Attributes["DataSmallNumber"].Value = xs;

                XmlAttribute attributeFormat = docTemp.CreateAttribute("DataFormat");
                dataItem.Attributes.Append(attributeFormat);
                dataItem.Attributes["DataFormat"].Value = gs;
                nodeParent.AppendChild(dataItem);
            }
            LogManager.AddMessage("标识名称已添加,点击保存按钮后生效", EnumLogSource.用户操作日志, EnumLevel.Tip);


        }
    }

    /// <summary>
    /// 数据标识项
    /// </summary>
    public class DataFlagItem : ViewModelBase
    {
        private string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetPropertyValue(value, ref name, "Name"); }
        }
        private string dataFlag;
        /// <summary>
        /// 数据标识
        /// </summary>
        public string DataFlag
        {
            get { return dataFlag; }
            set { SetPropertyValue(value, ref dataFlag, "DataFlag"); }
        }
        private string dataFlag698;
        /// <summary>
        /// 数据标识 698
        /// </summary>
        public string DataFlag698
        {
            get { return dataFlag698; }
            set { SetPropertyValue(value, ref dataFlag698, "DataFlag698"); }
        }

        private string length;
        /// <summary>
        /// 数据长度
        /// </summary>
        public string Length
        {
            get { return length; }
            set { SetPropertyValue(value, ref length, "Length"); }
        }
        private string dotLength;
        /// <summary>
        /// 小数位数
        /// </summary>
        public string DotLength
        {
            get { return dotLength; }
            set
            {
                SetPropertyValue(value, ref dotLength, "DotLength");
            }
        }
        private string dataFormat;
        /// <summary>
        /// 数据格式
        /// </summary>
        public string DataFormat
        {
            get { return dataFormat; }
            set { SetPropertyValue(value, ref dataFormat, "DataFormat"); }
        }

        private string readData;
        /// <summary>
        /// 写入内容
        /// </summary>
        public string ReadData
        {
            get { return readData; }
            set { SetPropertyValue(value, ref readData, "ReadData"); }
        }

    }
}
