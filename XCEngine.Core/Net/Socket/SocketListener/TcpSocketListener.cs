using System.Net;
using System.Net.Sockets;

namespace XCEngine.Core
{
    /// <summary>
    /// Tcp监听
    /// </summary>
    public class TcpSocketListener : SocketListener
    {
        /// <summary>
        /// 监听Socket
        /// </summary>
        private Socket _socket;

        public TcpSocketListener(int id)
            : base(id)
        {
        }

        public override bool Listen(string listenIp, int port, int backlog)
        {
            try
            {
                if (_socket != null)
                {
                    Log.Warning("Socket Has Listen, Now Close It");
                    Close();
                }

                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                if (_socket == null)
                {
                    Log.Error("Create Socket Failed!");
                    return false;
                }

                _socket.Bind(new IPEndPoint(IPAddress.Parse(listenIp), port));
                _socket.Listen(backlog);
                Log.Info($"Listen {listenIp}:{port} Succ");
                return true;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return false;
            }
        }

        public override void StartAccept()
        {
            Accept();
        }

        public override void Close()
        {
            lock (this)
            {
                if (_socket != null)
                {
                    Log.Info($"Close Accept");
                    _socket.Close();
                    _socket.Dispose();
                    _socket = null;
                }
            }
        }

        #region Other Thread
        void Accept()
        {
            _socket.BeginAccept((result) =>
            {
                try
                {
                    Socket socket = null;
                    lock (this)
                    {
                        if (_socket == null)
                        {
                            return;
                        }

                        socket = _socket.EndAccept(result);
                    }

                    OnAccept(socket);
                    Accept();
                }
                catch (Exception ex)
                {
                    OnError(-1, $"Accept Catch Exception: {ex.Message}\n{ex.StackTrace}");
                }
            }, null);
        }

        void OnAccept(Socket socket)
        {
            OnAcceptCallback?.Invoke(socket);
        }

        void OnError(int errorId, string desc)
        {
            OnErrorCallback?.Invoke(errorId, desc);
        }
        #endregion
    }
}
