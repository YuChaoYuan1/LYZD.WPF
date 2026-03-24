
namespace ZH.MeterProtocol.Protocols.DLT698.Enum
{
    /// <summary>
    /// 参数安全模式
    /// </summary>
    public enum EmSecurityMode
    {
        /// <summary>
        /// 明文
        /// </summary>
        ClearText,
        /// <summary>
        /// 明文＋随机数
        /// </summary>
        ClearTextRand,
        /// <summary>
        ///  明文+数据验证码
        /// </summary>
        ClearTextMac,
        /// <summary>
        ///密文
        /// </summary>
        Ciphertext,
        /// <summary>
        /// 密文+数据验证码
        /// </summary>
        CiphertextMac
    }
}
