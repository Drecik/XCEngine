using System;

namespace XCEngine.Core
{
    /// <summary>
    /// Socket监听接口
    /// </summary>
    public interface ISocketListener
    {
        /// <summary>
        /// 唯一Id
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// 接受一个连接的回调
        /// </summary>
        Action<object> OnAcceptCallback { set; }

        /// <summary>
        /// 出错回调
        /// </summary>
        Action<int, string> OnErrorCallback { set; }

        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="listenIp">监听的Ip</param>
        /// <param name="port">监听的端口号</param>
        /// <param name="backlog">监听的队列长度</param>
        /// <returns></returns>
        bool Listen(string listenIp, int port, int backlog);

        /// <summary>
        /// 开始接受连接
        /// </summary>
        void StartAccept();

        /// <summary>
        /// 关闭Socket
        /// </summary>
        void Close();
    }
}