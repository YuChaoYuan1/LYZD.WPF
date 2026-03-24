
namespace LYZD.DAL.Config
{
    /// 配置编号枚举
    /// <summary>
    /// 配置编号枚举
    /// </summary>
    public enum EnumConfigId
    {
        未知编号配置 = 0,
        #region 装置信息
        基本信息 = 01001,
        地区信息 = 01002,
        显示设置=01003,
        日志设置 = 01004,
        #endregion

        #region 检定信息

        标准器设置 = 02001,
        检定有效期 = 02003,
        检定配置=02004,
        时间设置=02005,
        //通讯配置=02006,
        终端参数配置=02006,
        电机配置=02007,
        检定过程配置=02008,
        蓝牙光电模式配置 = 02009,
        特殊配置 = 02010,


        #endregion

        #region 营销接口
        营销接口配置 = 03001,
        #endregion

        #region 加密机
        加密机配置 = 04001,
        #endregion

        #region 网络信息
        集控线配置=05001,
        工控平台上报 = 05002,
        #endregion

    }
}
