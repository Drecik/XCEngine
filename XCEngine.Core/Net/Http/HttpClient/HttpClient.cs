using System.Text;

namespace XCEngine.Core
{
    internal class HttpClient : IHttpClient
    {
        private System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();

        public async Task<IHttpResult> GetAsync(string url, Dictionary<string, string> args = null, int timeout = 10)
        {
            try
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(timeout);
                string kvString = StringUtils.DictionaryToString(args, "&", "=");
                if (kvString.Length > 0)
                {
                    url = url + '?' + kvString;
                }
                return new HttpResult(await _httpClient.GetAsync(url));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return new HttpResult(null);
            }
        }

        public async Task<IHttpResult> PostDataAsync(string url, string data, int timeout = 10)
        {
            return await PostDataAsync(url, Encoding.UTF8.GetBytes(data), timeout);
        }

        public async Task<IHttpResult> PostDataAsync(string url, byte[] data, int timeout = 10)
        {
            try
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(timeout);
                HttpContent content = new ByteArrayContent(data);
                return new HttpResult(await _httpClient.PostAsync(url, content));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return new HttpResult(null);
            }
        }

        public async Task<IHttpResult> PostDataAsync(string url, string data, string contentType, int timeout = 10)
        {
            return await PostDataAsync(url, Encoding.UTF8.GetBytes(data), contentType, timeout);
        }

        public async Task<IHttpResult> PostDataAsync(string url, byte[] data, string contentType, int timeout = 10)
        {
            try
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(timeout);
                HttpContent content = new ByteArrayContent(data);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                return new HttpResult(await _httpClient.PostAsync(url, content));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return new HttpResult(null);
            }
        }

        public async Task<IHttpResult> Request(HttpRequestMessage request, int timeout = 10)
        {
            try
            {
                _httpClient.Timeout = TimeSpan.FromSeconds(timeout);
                return new HttpResult(await _httpClient.SendAsync(request));
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return new HttpResult(null);
            }
        }
    }
}
