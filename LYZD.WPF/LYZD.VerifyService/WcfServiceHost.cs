using System;
using System.ServiceModel.Description;
using System.ServiceModel;

namespace LYTest.VerifyService
{
    /// 创建WCF服务
    /// <summary>
    /// 创建WCF服务
    /// </summary>
    public class WcfServiceHost
    {
        private static ServiceHost mySvcHost = null;
        /// 开启WebService形式的WCF服务
        /// <summary>
        /// 开启WebService形式的WCF服务
        /// </summary>
        /// <param name="uriAddress">网站要发布的地址</param>
        /// <param name="wcfClass">wcf类</param>
        /// <param name="wcfClass">wcf接口</param>
        public static void StartService(string uriAddress, Type wcfClass, Type wcfInterface)
        {
            //服务的地址会在APP配置文件里面配置
            Uri address = new Uri(uriAddress);
            mySvcHost = new ServiceHost(wcfClass, address);
            //在创建的ServiceHost对象当中查找，看是否存在ServiceMetadataBehaviour的元数据行为  
            //描述，如果没有找到，创建一个新的ServiceMetadataBehaviour对象。  
            ServiceMetadataBehavior behaviour = mySvcHost.Description.
            Behaviors.Find<ServiceMetadataBehavior>();
            if (behaviour == null)
            {
                behaviour = new ServiceMetadataBehavior();
            }
            //设置允许进行HttpGet操作。  
            behaviour.HttpGetEnabled = true;
            //设置MetadataExporter导出Metadata时遵循WS-Policy 1.5规范。  
            behaviour.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            //将创建好的behaviour加入到宿主实例的行为描述组当中。  
            mySvcHost.Description.Behaviors.Add(behaviour);
            //加入MetadataExchange endpoint.  
            mySvcHost.AddServiceEndpoint(
            ServiceMetadataBehavior.MexContractName,
            MetadataExchangeBindings.CreateMexHttpBinding(),
            "mex"
            );
            //加入应用程序 endpoint，这里必须是BasicHttpBinding ,对应于WebService
            BasicHttpBinding binding = new BasicHttpBinding();
            mySvcHost.AddServiceEndpoint(wcfInterface,
            binding, address);
            //打开 ServiceHost 实例。  
            mySvcHost.Open();
        }
        /// 退出WCF服务
        /// <summary>
        /// 退出WCF服务
        /// </summary>
        public static void StopService()
        {
            if (mySvcHost != null)
            {
                mySvcHost.Abort();
            }
        }
    }
}
