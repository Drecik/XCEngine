using System;

namespace XCEngine.Core
{
    /// <summary>
    /// 协议编码解码接口
    /// </summary>
    public interface IProtoCoder
    {
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="message">发送的消息对象</param>
        /// <returns></returns>
        byte[] Encode(object message);

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="type">解码到的消息对象</param>
        /// <param name="msgBytes">消息二进制数据</param>
        /// <returns></returns>
        object Decode(Type type, byte[] msgBytes);
    }
}
