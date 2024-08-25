using System.Net;
using System.Net.Sockets;

namespace XCEngine.Core
{
    /// <summary>
    /// Tcp连接
    /// </summary>
    public class TcpSocketConnection : SocketConnection
    {
        private Socket _socket;

        /// <summary>
        /// 用户同步的上下文
        /// </summary>
        private SynchronizationContext _syncContext;

        /// <summary>
        /// 首包的长度
        /// </summary>
        const int ReceiveBufferSize = 10240;

        /// <summary>
        /// 首包缓冲区
        /// </summary>
        private byte[] _receiveBuffer = new byte[ReceiveBufferSize];

        /// <summary>
        /// 发送队列
        /// </summary>
        private Queue<byte[]> _sendPackageQueue = new Queue<byte[]>();

        /// <summary>
        /// 是否在发送
        /// </summary>
        private bool _isSending = false;

        public TcpSocketConnection(int id, Socket socket)
            : base(id)
        {
            _socket = socket;

            var endPoint = (IPEndPoint)socket.RemoteEndPoint;
            RemoteIp = endPoint.Address.ToString();
            RemotePort = endPoint.Port;
        }

        /// <summary>
        /// 开始收包
        /// </summary>
        public override void BeginReceive(INetPackageSerializer netPackageSerializer, SynchronizationContext syncContext)
        {
            _netPackageSerializer = netPackageSerializer;
            _syncContext = syncContext;
            ReceivePackage();
        }

        /// <summary>
        /// 发送网络包
        /// </summary>
        /// <param name="package"></param>
        public override void Send(INetPackage package)
        {
            byte[] sendData = _netPackageSerializer.Serialize(package);

            lock (_sendPackageQueue)
            {
                if (_isSending)
                {
                    _sendPackageQueue.Enqueue(sendData);
                    return;
                }

                _isSending = true;
            }

            SendPackage(sendData, 0);
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public override void Close()
        {
            lock (_sendPackageQueue)
            {
                if (_isSending)
                {
                    _sendPackageQueue.Enqueue(null);
                }
            }

            SendPackage(null, 0);
        }

        #region Other Thread

        void ReceivePackage()
        {
            lock (this)
            {
                _socket?.BeginReceive(_receiveBuffer, 0, ReceiveBufferSize, SocketFlags.None, (result) =>
                {
                    try
                    {
                        SocketError socketError;
                        int readLength = 0;

                        lock (this)
                        {
                            if (_socket == null)
                            {
                                return;
                            }

                            readLength = _socket.EndReceive(result, out socketError);
                        }

                        if (socketError != SocketError.Success || readLength == 0)
                        {
                            if (socketError != SocketError.Success)
                            {
                                OnError((int)socketError, $"Receive Got Socket Error: {socketError}");
                            }

                            CloseInternal();
                            OnClose();
                            return;
                        }

                        //Log.Info($"--------Receive: {readLength}");
                        var packageList = _netPackageSerializer.Deserialize(_receiveBuffer, readLength);
                        OnReceive(packageList);
                        ReceivePackage();
                    }
                    catch (Exception ex)
                    {
                        OnError(-1, $"Receive Catch Exception: {ex.Message}\n{ex.StackTrace}");
                        CloseInternal();
                        OnClose();
                    }
                }, null);
            }
        }

        void SendPackage(byte[] data, int offest)
        {
            // data == null表示是关闭包，用来确保Close之前所有发送队列里面的数据都发送
            if (data == null)
            {
                CloseInternal();
                return;
            }

            int leftSize = data.Length - offest;

            lock (this)
            {
                _socket?.BeginSend(data, offest, leftSize, SocketFlags.None, (result) =>
                {
                    try
                    {
                        SocketError socketError;
                        int sendLength = 0;

                        lock (this)
                        {
                            if (_socket == null)
                            {
                                return;
                            }

                            sendLength = _socket.EndSend(result, out socketError);
                        }

                        if (socketError != SocketError.Success || sendLength == 0)
                        {
                            if (socketError != SocketError.Success)
                            {
                                OnError((int)socketError, $"Send Got Socket Error: {socketError}");
                            }

                            CloseInternal();
                            OnClose();
                            return;
                        }

                        if (sendLength == leftSize)
                        {
                            lock (_sendPackageQueue)
                            {
                                if (_sendPackageQueue.Count == 0)
                                {
                                    _isSending = false;
                                    return;
                                }

                                data = _sendPackageQueue.Dequeue();
                            }
                            SendPackage(data, 0);
                        }
                        else
                        {
                            SendPackage(data, offest + sendLength);
                        }
                    }
                    catch (Exception ex)
                    {
                        OnError(-1, $"Send Catch Exception: {ex.Message}\n{ex.StackTrace}");
                        CloseInternal();
                        OnClose();
                    }
                }, null);
            }
        }

        void CloseInternal()
        {
            lock (this)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket.Dispose();
                    _socket = null;

                    lock (_sendPackageQueue)
                    {
                        _sendPackageQueue.Clear();
                    }
                }
            }
        }

        void OnReceive(List<INetPackage> packageList)
        {
            SendOrPostCallback action = (_) =>
            {
                for (int i = 0; i < packageList.Count; i++)
                {
                    OnReceiveCallback?.Invoke(packageList[i]);
                }
            };

            if (_syncContext != null)
            {
                _syncContext?.Post(action, null);
            }
            else
            {
                action(null);
            }
        }

        void OnClose()
        {
            SendOrPostCallback action = (_) =>
            {
                OnCloseCallback?.Invoke();
            };

            if (_syncContext != null)
            {
                _syncContext?.Post(action, null);
            }
            else
            {
                action(null);
            }
        }

        void OnError(int errorId, string desc)
        {
            SendOrPostCallback action = (_) =>
            {
                OnErrorCallback?.Invoke(errorId, desc);
            };

            if (_syncContext != null)
            {
                _syncContext?.Post(action, null);
            }
            else
            {
                action(null);
            }
        }
        #endregion
    }
}
