using System.Net;

namespace XCEngine.Core
{
    public class HttpServer : IHttpServer
    {
        public int Id { get; set; }

        public Action<HttpListenerContext> OnAcceptCallback { protected get; set; }

        public Action<int, string> OnErrorCallback { protected get; set; }

        private HttpListener _listener;

        public HttpServer(int id)
        {
            Id = id;
        }

        public bool Listen(string host, int port)
        {
            if (_listener != null)
            {
                Log.Error("Already listening");
                return false;
            }

            try
            {
                string prefix = $"http://{host}:{port}/";
                _listener = new HttpListener();
                _listener.Prefixes.Add(prefix);

                _listener.Start();
            }
            catch (Exception ex)
            {
                Log.Error($"Listen Catch Exception: {ex.Message}\n{ex.StackTrace}");
                return false;
            }

            return true;
        }

        public void StartAccept()
        {
            Accept();
        }

        public void Stop()
        {
            _listener.Stop();
            _listener = null;
        }

        #region Other Thread

        void Accept()
        {
            _listener.BeginGetContext((result) =>
            {
                try
                {
                    lock (this)
                    {
                        if (_listener == null || _listener.IsListening == false)
                        {
                            return;
                        }
                    }

                    var context = _listener.EndGetContext(result);
                    OnAccept(context);
                    Accept();
                }
                catch (Exception ex)
                {
                    OnError(-1, $"Accept Catch Exception: {ex.Message}\n{ex.StackTrace}");
                }
            }, null);
        }

        void OnAccept(HttpListenerContext context)
        {
            OnAcceptCallback?.Invoke(context);
        }

        void OnError(int errorId, string desc)
        {
            OnErrorCallback?.Invoke(errorId, desc);
        }

        #endregion
    }
}
