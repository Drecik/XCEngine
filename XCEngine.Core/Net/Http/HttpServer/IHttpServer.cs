using System.Net;

namespace XCEngine.Core
{
    /// <summary>
    /// http服务器接口
    /// </summary>
    public interface IHttpServer
    {
        int Id { get; }

        /// <summary>
        /// 接受一个连接的回调
        /// </summary>
        Action<HttpListenerContext> OnAcceptCallback { set; }

        /// <summary>
        /// 出错回调
        /// </summary>
        Action<int, string> OnErrorCallback { set; }

        /// <summary>
        /// 监听地址
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        bool Listen(string host, int port);

        /// <summary>
        /// 开始接收请求
        /// </summary>
        void StartAccept();

        /// <summary>
        /// 关闭服务器
        /// </summary>
        void Stop();
    }
}
