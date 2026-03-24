using LYZD.DAL.Config;
using LYZD.Mis.Common;
using LYZD.Utility.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Mis
{
    public class MISFactory
    {
        public static IMis Create()
        {

            string ip = ConfigHelper.Instance.Marketing_IP;
            int port = int.Parse(ConfigHelper.Instance.Marketing_Prot);
            string dataSource = ConfigHelper.Instance.Marketing_DataSource;
            string userId = ConfigHelper.Instance.Marketing_UserName;
            string pwd = ConfigHelper.Instance.Marketing_UserPassWord; ;
            string url = ConfigHelper.Instance.Marketing_WebService;

            //string ip = "21.38.56.158";
            //int port = 11521;
            //string dataSource = "orcl";
            //string userId = "sxykjd";
            //string pwd = "JLsczdhxt_nari0";
            //string url = "http://21.38.56.156:10081/services/DetectService";

            IMis mis = null;

            string type = ConfigHelper.Instance.Marketing_Type;
            LogManager.AddMessage(ip+":"+port+"、数据源"+ dataSource+"、"+ userId+ "、" + pwd, EnumLogSource.服务器日志, EnumLevel.Information);
            LogManager.AddMessage(type +"、"+url, EnumLogSource.服务器日志, EnumLevel.Information);
            switch (type)
            {
                case "厚达":
                    //mis = new MDS.MDS(ip, port, dataSource, userId, pwd, url);
                    mis = new Houda.Houda(ip, port, dataSource, userId, pwd, url);
                    break;
                case "南瑞_智慧工控平台2.0":
                    mis = new NanRui.NanRui(ip, port, dataSource, userId, pwd, url);
                    break;
                case "LY数据服务":
                    mis = new LYDataServer.Api(ip, port, dataSource, userId, pwd, url);
                    break;
                case "智慧工控平台":
                    mis = new IMICP.IMICPMis(ip, port, dataSource, userId, pwd, url);
                    break;
                default:
                    mis = new NanRui.NanRui(ip, port, dataSource, userId, pwd, url);
                    break;
            }

            //MisType type = App.UserSetting.MisType;

            //if (type == MisType.东软SG186)
            //    mis = new SG186.SG186(ip, port, dataSource, userId, pwd, url);

            //else if (type == MisType.普华SG186)
            //    mis = new 普华SG186(ip, port, dataSource, userId, pwd, url);

            //else if (type == MisType.生产调度系统)
            //    mis = new MDS.MDS(ip, port, dataSource, userId, pwd, url);

            //else if (type == MisType.天津MIS接口)
            //    mis = new Taida.Taida(ip, port, dataSource, userId, pwd, url);

            //else if (type == MisType.国金源富接口)
            //    mis = new GuoJin.GuoJin(ip, port, dataSource, userId, pwd, url);

            //else if (type == MisType.SG186安徽)
            //    mis = new SG186.AH.SG186(ip, port, dataSource, userId, pwd, url);

            //else if (type == MisType.SG186甘肃)
            //    mis = new SG186.GS.SG186(ip, port, dataSource, userId, pwd, url);

            //else if (type == MisType.外部可执行程序接口)
            //    mis = null;

            //else if (type == MisType.外部动态库接口)
            //    mis = null;



            return mis;
        }
    }
}
