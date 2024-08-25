namespace XCEngine.Core
{
    /// <summary>
    /// HttpClient接口
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// 异步获取Get请求的结果
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="args">Get参数</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        Task<IHttpResult> GetAsync(string url, Dictionary<string, string> args = null, int timeout = 10);

        /// <summary>
        /// 异步获取Post请求的结果
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">Post数据</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        Task<IHttpResult> PostDataAsync(string url, string data, int timeout = 10);

        /// <summary>
        /// 异步获取Post请求的结果
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">Post数据</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        Task<IHttpResult> PostDataAsync(string url, byte[] data, int timeout = 10);

        /// <summary>
        /// 异步获取Post请求的结果
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">Post数据</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        Task<IHttpResult> PostDataAsync(string url, string data, string contentType, int timeout = 10);

        /// <summary>
        /// 异步获取Post请求的结果
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">Post数据</param>
        /// <param name="contentType">内容类型</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        Task<IHttpResult> PostDataAsync(string url, byte[] data, string contentType, int timeout = 10);

        /// <summary>
        /// 异步方式请求
        /// </summary>
        /// <param name="request">请求消息</param>
        /// <returns></returns>
        Task<IHttpResult> Request(HttpRequestMessage request, int timeout = 10);
    }
}
