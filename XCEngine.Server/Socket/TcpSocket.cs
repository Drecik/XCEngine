using System;
using System.Linq;
using System.Net.Sockets;

namespace XCEngine.Server
{
    /// <summary>
    /// Tcp Socket封装
    /// </summary>
    public static class TcpSocket
    {
        private static Dictionary<int, object> _socketDict = new();

        /// <summary>
        /// Id生成
        /// </summary>
        private static IIdGenerator _idGenerator = new ThreadSafeIdGenerator();

        /// <summary>
        /// 监听端口
        /// </summary>
        /// <param name="listenIp">监听的ip</param>
        /// <param name="port">监听的端口号</param>
        /// <param name="backlog">队列长度</param>
        /// <param name="netPackageSerializerFactory">序列化工厂</param>
        /// <returns></returns>
        public static int Listen(string listenIp, int port, int backlog)
        {
            var listener = new TcpSocketListener(_idGenerator.GenerateId());
            if (false == listener.Listen(listenIp, port, backlog))
            {
                return 0;
            }

            _socketDict.Add(listener.Id, listener);
            return listener.Id;
        }

        /// <summary>
        /// 开始接受连接
        /// </summary>
        /// <param name="fd"></param>
        public static void StartAccept(int fd)
        {
            if (_socketDict.TryGetValue(fd, out var obj))
            {
                if (obj is ISocketListener socketListener)
                {
                    int actorId = Actor.Self();
                    socketListener.OnAcceptCallback = (socket) =>
                    {
                        ISocketConnection connection = new TcpSocketConnection(_idGenerator.GenerateId(), socket as Socket);
                        _socketDict.Add(connection.Id, connection);

                        Actor.Send(actorId, "OnAccept", connection.Id);
                    };

                    socketListener.OnErrorCallback = (error, errorDesc) =>
                    {
                        Actor.Send(actorId, "OnError", fd, error, errorDesc);
                    };

                    socketListener.StartAccept();
                }
            }
        }

        /// <summary>
        /// 开始接受消息
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="serializerFactory"></param>
        public static void StartReceive(int fd, INetPackageSerializerFactory serializerFactory)
        {
            if (_socketDict.TryGetValue(fd, out var obj))
            {
                if (obj is ISocketConnection socketConnection)
                {
                    int actorId = Actor.Self();

                    socketConnection.OnReceiveCallback = (package) =>
                    {
                        Actor.SendRaw(actorId, "OnReceive", new object[2] { fd, package });
                    };

                    socketConnection.OnErrorCallback = (error, errorDesc) =>
                    {
                        Actor.Send(actorId, "OnError", fd, error, errorDesc);
                    };

                    socketConnection.OnCloseCallback = () =>
                    {
                        _socketDict.Remove(fd);
                        Actor.Send(actorId, "OnClose", fd);
                    };
                }
            }
        }

        /// <summary>
        /// 发送网络请求
        /// </summary>
        /// <param name="fd"></param>
        public static void Send(int fd, INetPackage pacakge)
        {
            if (_socketDict.TryGetValue(fd, out var obj))
            {
                if (obj is ISocketConnection socketConnection)
                {
                    socketConnection.Send(pacakge);
                }
            }
        }

        /// <summary>
        /// 关闭socket
        /// </summary>
        /// <param name="fd"></param>
        public static void Close(int fd)
        {
            if (_socketDict.TryGetValue(fd, out var obj))
            {
                if (obj is ISocketConnection socketConnection)
                {
                    socketConnection.Close();
                    _socketDict.Remove(fd);
                }
                else if (obj is ISocketListener socketListener)
                {
                    socketListener.Close();
                    _socketDict.Remove(fd);
                }
            }
        }
    }
}
