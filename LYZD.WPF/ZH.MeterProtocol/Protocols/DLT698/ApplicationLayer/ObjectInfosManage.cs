using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Linq;

namespace ZH.MeterProtocol.Protocols.ApplicationLayer
{
    public class ObjectInfosManage
    {

        public List<ObjectAttributes> AttributeInfos { get; set; }


        private static ObjectInfosManage obXmlInfo = null;
        /// <summary>
        /// 单例
        /// </summary>
        /// <returns></returns>
        public static ObjectInfosManage Instance()
        {
            if (obXmlInfo == null)
            {
                obXmlInfo = new ObjectInfosManage();
            }
            return obXmlInfo;
        }

        public ObjectInfosManage()
        {
            LoadObjectInfos();
        }

        private void LoadObjectInfos()
        {
            AttributeInfos = new List<ObjectAttributes>();
            //XDocument _XDoc = XDocument.Load(ConfigurationManager.AppSettings["Oad"].ToString());
            //XDocument _XDoc = XDocument.Load(@"E:\工作\LY\Resource\Xml\OadInfosConfig.xml");
            // Directory.GetCurrentDirectory()
            XDocument _XDoc = XDocument.Load(Directory.GetCurrentDirectory()+@"\Xml\OadInfosConfig.xml");

            //
            if (_XDoc != null)
            {
                foreach (XElement item in _XDoc.Descendants("OadConfig").Elements("Oad"))
                {
                    ObjectAttributes oad = new ObjectAttributes();
                    oad.Oad = item.Attribute("Oad").Value.ToString();
                    oad.BigType = item.Attribute("BigType").Value.ToString();
                    oad.ItemCount = Convert.ToInt32(item.Attribute("ItemCount").Value.ToString());
                    oad.DataInfo = new List<DataInfos>();
                    if (item.HasElements)
                    {
                        foreach (XElement e in item.Elements("Data"))
                        {
                            DataInfos info = new DataInfos();
                            info.DataTypeCode = Convert.ToInt32(e.Attribute("DataTypeCode").Value.ToString().Trim());
                            info.DataTypeName = e.Attribute("DataTypeName").Value.ToString();
                            info.LengthFlag = (bool)(e.Attribute("LengthFlag").Value.ToString().Trim() == "1");
                            info.DataLength = Convert.ToInt32(e.Attribute("DataLength").Value.ToString().Trim());
                            info.FloatCount = Convert.ToInt32(e.Attribute("FloatCount").Value.ToString().Trim());
                            oad.DataInfo.Add(info);
                        }
                        AttributeInfos.Add(oad);
                    }


                }
            }

        }

    }
}
