using System;

namespace XCEngine.Core
{
    /// <summary>
    /// Socket连接
    /// </summary>
    public abstract class SocketConnection : ISocketConnection
    {
        /// <summary>
        /// Connection Id
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// 网络序列化对象
        /// </summary>
        protected INetPackageSerializer _netPackageSerializer;

        /// <summary>
        /// 远端Ip地址
        /// </summary>
        public string RemoteIp { get; protected set; }

        /// <summary>
        /// 远端端口号
        /// </summary>
        public int RemotePort { get; protected set; }

        /// <summary>
        /// 收到包时候的回调
        /// </summary>
        public Action<INetPackage> OnReceiveCallback { set; protected get; }

        /// <summary>
        /// 连接出错时候的回调
        /// </summary>
        public Action OnCloseCallback { set; protected get; }

        /// <summary>
        /// 连接出错时候的回调
        /// </summary>
        public Action<int, string> OnErrorCallback { set; protected get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">Connection Id</param>
        public SocketConnection(int id)
        {
            Id = id;
        }

        /// <summary>
        /// 开始接收包
        /// <param name="syncContext">同步上下文</param>
        /// </summary>
        public abstract void BeginReceive(INetPackageSerializer netPackageSerializer);

        /// <summary>
        /// 发送网络包
        /// </summary>
        /// <param name="package"></param>
        public abstract void Send(INetPackage package);

        /// <summary>
        /// 关闭连接
        /// </summary>
        public abstract void Close();
    }
}
