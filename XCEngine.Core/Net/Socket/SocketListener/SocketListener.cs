using System;

namespace XCEngine.Core
{
    public abstract class SocketListener : ISocketListener
    {
        public int Id { get; }

        /// <summary>
        /// 接受一个连接的回调
        /// </summary>
        public Action<object> OnAcceptCallback { set; protected get; }

        /// <summary>
        /// Socket出错回调
        /// </summary>
        public Action<int, string> OnErrorCallback { set; protected get; }

        public SocketListener(int id)
        {
            Id = id;
        }

        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="listenIp">监听的Ip</param>
        /// <param name="port">监听的端口号</param>
        /// <param name="backlog">监听的队列长度</param>
        /// <returns></returns>
        public abstract bool Listen(string listenIp, int port, int backlog);

        /// <summary>
        /// 开始接受连接
        /// </summary>
        public abstract void StartAccept();

        /// <summary>
        /// 关闭Socket
        /// </summary>
        public abstract void Close();
    }
}
