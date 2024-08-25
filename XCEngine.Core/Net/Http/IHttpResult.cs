using System.Text.Json.Nodes;

namespace XCEngine.Core
{
    /// <summary>
    /// HttpResult接口
    /// </summary>
    public interface IHttpResult : IDisposable
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// 状态码
        /// </summary>
        int StatusCode { get; }

        /// <summary>
        /// 获取Header头
        /// </summary>
        IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }

        /// <summary>
        /// 异步方式读取字符串数据
        /// </summary>
        /// <returns></returns>
        Task<string> ReadAsStringAsync();

        /// <summary>
        /// 异步方式读取二进制数据
        /// </summary>
        /// <returns></returns>
        Task<byte[]> ReadAsBytesAsync();

        /// <summary>
        /// 异步方式读取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> ReadAsObjectAsync<T>(ISerializer serializer = null) where T : class;

        /// <summary>
        /// 异步方式读取Json数据
        /// </summary>
        /// <returns></returns>
        Task<JsonNode> ReadAsJsonAsync();

        /// <summary>
        /// 异步方式读取流
        /// </summary>
        /// <returns></returns>
        Task<Stream> ReadAsStreamAsync();
    }
}
