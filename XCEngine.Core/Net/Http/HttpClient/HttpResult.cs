using System.Text.Json;
using System.Text.Json.Nodes;

namespace XCEngine.Core
{
    internal class HttpResult : IHttpResult
    {
        private HttpResponseMessage _httpResponse;

        public HttpResult(HttpResponseMessage httpResponse)
        {
            _httpResponse = httpResponse;
        }

        public bool IsSuccess => _httpResponse != null && _httpResponse.IsSuccessStatusCode;

        public int StatusCode => _httpResponse == null ? 500 : ((int)_httpResponse.StatusCode);

        private Dictionary<string, IEnumerable<string>> _headers;

        public IReadOnlyDictionary<string, IEnumerable<string>> Headers
        {
            get
            {
                if (_headers == null)
                {
                    _headers = new Dictionary<string, IEnumerable<string>>();
                    foreach (var iter in _httpResponse.Headers)
                    {
                        _headers.Add(iter.Key, iter.Value);
                    }
                }

                return _headers;
            }
        }

        public async Task<string> ReadAsStringAsync()
        {
            if (IsSuccess == false)
            {
                return null;
            }

            return await _httpResponse.Content.ReadAsStringAsync();
        }

        public async Task<byte[]> ReadAsBytesAsync()
        {
            if (IsSuccess == false)
            {
                return null;
            }

            return await _httpResponse.Content.ReadAsByteArrayAsync();
        }

        public async Task<T> ReadAsObjectAsync<T>(ISerializer serializer = null) where T : class
        {
            byte[] data = await ReadAsBytesAsync();
            if (data == null)
            {
                return null;
            }

            try
            {
                if (serializer == null)
                {
                    return JsonSerializer.Deserialize<T>(data);
                }
                return serializer.Deserialize<T>(data);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return null;
            }
        }

        public async Task<JsonNode> ReadAsJsonAsync()
        {
            string jsonString = await ReadAsStringAsync();
            if (jsonString == null)
            {
                return null;
            }
            try
            {
                return JsonNode.Parse(jsonString);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return null;
            }
        }

        public async Task<Stream> ReadAsStreamAsync()
        {
            if (IsSuccess == false)
            {
                return null;
            }

            return await _httpResponse.Content.ReadAsStreamAsync();
        }

        public void Dispose()
        {
            _httpResponse?.Dispose();
        }
    }
}
