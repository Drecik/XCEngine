using System.Text.Json.Nodes;

namespace XCEngine.Core
{
    /// <summary>
    /// Http助手类
    /// </summary>
    public class HttpUtils
    {
        public static IHttpClientFactory HttpClientFactory = new HttpClientFactory();

        public static IHttpClient CreateHttpClient()
        {
            return HttpClientFactory.CreateHttpClient();
        }

        /// <summary>
        /// 异步方式发起Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<IHttpResult> GetAsync(string url, Dictionary<string, string> args = null, int timeout = 10)
        {
            return await CreateHttpClient().GetAsync(url, args, timeout);
        }

        /// <summary>
        /// 异步方式发起Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<IHttpResult> PostDataAsync(string url, string data, int timeout = 10)
        {
            return await CreateHttpClient().PostDataAsync(url, data, timeout);
        }

        /// <summary>
        /// 异步方式发起Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<IHttpResult> PostDataAsync(string url, byte[] data, int timeout = 10)
        {
            return await CreateHttpClient().PostDataAsync(url, data, timeout);
        }

        /// <summary>
        /// 异步方式发起Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="contentType">内容类型</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<IHttpResult> PostDataAsync(string url, string data, string contentType, int timeout = 10)
        {
            return await CreateHttpClient().PostDataAsync(url, data, contentType, timeout);
        }

        /// <summary>
        /// 异步方式发起Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="contentType">内容类型</param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<IHttpResult> PostDataAsync(string url, byte[] data, string contentType, int timeout = 10)
        {
            return await CreateHttpClient().PostDataAsync(url, data, contentType, timeout);
        }

        /// <summary>
        /// 异步方式发起请求
        /// </summary>
        /// <param name="request">请求结构</param>
        /// <param name="timeout">超时</param>
        /// <returns></returns>
        public static async Task<IHttpResult> Request(HttpRequestMessage request, int timeout = 10)
        {
            return await CreateHttpClient().Request(request, timeout);
        }


        /// <summary>
        /// 发送飞书消息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task SendFeiShuMsg(string url, string msg)
        {
            JsonObject content = new JsonObject();
            content["text"] = msg;
            JsonObject data = new JsonObject();
            data["msg_type"] = "text";
            data["content"] = content;
            using var result = await PostDataAsync(url, data.ToJsonString(), "application/json");
        }
    }
}
