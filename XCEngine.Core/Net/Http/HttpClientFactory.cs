namespace XCEngine.Core
{
    internal class HttpClientFactory : IHttpClientFactory
    {
        public IHttpClient CreateHttpClient()
        {
            return new HttpClient();
        }
    }
}
