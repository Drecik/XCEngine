using System;

namespace XCEngine.Core
{
    public interface ISocketConnection
    {
        /// <summary>
        /// Connection Id
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 远端Ip地址
        /// </summary>
        string RemoteIp { get; }

        /// <summary>
        /// 远端端口号
        /// </summary>
        int RemotePort { get; }

        /// <summary>
        /// 收到包时候的回调
        /// </summary>
        Action<INetPackage> OnReceiveCallback { set; }

        /// <summary>
        /// 连接关闭时候的回调
        /// </summary>
        Action OnCloseCallback { set; }

        /// <summary>
        /// 连接出错时候的回调
        /// </summary>
        Action<int, string> OnErrorCallback { set; }

        /// <summary>
        /// 开始接收包
        /// <param name="syncContext">同步上下文</param>
        /// </summary>
        void BeginReceive(INetPackageSerializer netPackageSerializer);

        /// <summary>
        /// 发送网络包
        /// </summary>
        /// <param name="package"></param>
        void Send(INetPackage package);

        /// <summary>
        /// 关闭连接
        /// </summary>
        void Close();
    }
}
