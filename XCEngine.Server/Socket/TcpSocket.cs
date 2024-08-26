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
        static void AddSocket(int fd, object socket)
        {
            lock (_socketDict)
            {
                _socketDict.Add(fd, socket);
            }
        }

        static object GetSocket(int fd)
        {
            lock (_socketDict)
            {
                if (_socketDict.TryGetValue(fd, out object socket))
                {
                    return socket;
                }
                return null;
            }
        }

        static void RemoveSocket(int fd)
        {
            lock (_socketDict)
            {
                _socketDict.Remove(fd);
            }
        }

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

            AddSocket(listener.Id, listener);
            return listener.Id;
        }

        /// <summary>
        /// 连接远端
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static Task<int> Connect(string ip, int port)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            socket.BeginConnect(ip, port, (result) =>
            {
                try
                {
                    socket.EndConnect(result);
                    ISocketConnection connection = new TcpSocketConnection(_idGenerator.GenerateId(), socket);
                    tcs.SetResult(connection.Id);
                    AddSocket(connection.Id, connection);
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    tcs.SetResult(0);
                }
            }, null);
            return tcs.Task;
        }

        /// <summary>
        /// 开始接受连接
        /// </summary>
        /// <param name="fd"></param>
        public static void StartAccept(int fd)
        {
            var socket = GetSocket(fd);
            if (socket == null)
            {
                return;
            }

            if (socket is ISocketListener socketListener)
            {
                int actorId = Actor.Self();
                socketListener.OnAcceptCallback = (socket) =>
                {
                    ISocketConnection connection = new TcpSocketConnection(_idGenerator.GenerateId(), socket as Socket);
                    AddSocket(connection.Id, connection);

                    Actor.PushMessage(actorId, new ActorMessage()
                    {
                        MessageType = ActorMessage.EMessageType.System,
                        MessageId = "OnAccept",
                        MessageData = new object[2] { fd, connection.Id }
                    });
                };

                socketListener.OnErrorCallback = (error, errorDesc) =>
                {
                    Actor.PushMessage(actorId, new ActorMessage()
                    {
                        MessageType = ActorMessage.EMessageType.System,
                        MessageId = "OnError",
                        MessageData = new object[3] { fd, error, errorDesc }
                    });
                };

                socketListener.StartAccept();
            }
        }

        /// <summary>
        /// 开始接收消息
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="serializerFactory"></param>
        public static void StartReceive(int fd, INetPackageSerializer netPackageSerializer)
        {
            var socket = GetSocket(fd);
            if (socket == null)
            {
                return;
            }

            if (socket is ISocketConnection socketConnection)
            {
                int actorId = Actor.Self();

                socketConnection.OnReceiveCallback = (package) =>
                {
                    Actor.PushMessage(actorId, new ActorMessage()
                    {
                        MessageType = ActorMessage.EMessageType.System,
                        MessageId = "OnReceive",
                        MessageData = new object[2] { fd, package }
                    });
                };

                socketConnection.OnErrorCallback = (error, errorDesc) =>
                {
                    Actor.PushMessage(actorId, new ActorMessage()
                    {
                        MessageType = ActorMessage.EMessageType.System,
                        MessageId = "OnError",
                        MessageData = new object[3] { fd, error, errorDesc }
                    });
                };

                socketConnection.OnCloseCallback = () =>
                {
                    RemoveSocket(fd);
                    Actor.PushMessage(actorId, new ActorMessage()
                    {
                        MessageType = ActorMessage.EMessageType.System,
                        MessageId = "OnClose",
                        MessageData = new object[1] { fd }
                    });
                };

                socketConnection.BeginReceive(netPackageSerializer);
            }
        }

        /// <summary>
        /// 发送网络请求
        /// </summary>
        /// <param name="fd"></param>
        public static void Send(int fd, INetPackage pacakge)
        {
            var socket = GetSocket(fd);
            if (socket == null)
            {
                return;
            }

            if (socket is ISocketConnection socketConnection)
            {
                socketConnection.Send(pacakge);
            }
        }

        /// <summary>
        /// 关闭socket
        /// </summary>
        /// <param name="fd"></param>
        public static void Close(int fd)
        {
            var socket = GetSocket(fd);
            if (socket == null)
            {
                return;
            }

            if (socket is ISocketConnection socketConnection)
            {
                socketConnection.Close();
                RemoveSocket(fd);
            }
            else if (socket is ISocketListener socketListener)
            {
                socketListener.Close();
                RemoveSocket(fd);
            }
        }
    }
}
