using System.Net;
using System.Net.Http;

public static class ApiClient
{
    private static CookieContainer cookieContainer = new CookieContainer();

    private static HttpClientHandler handler = new HttpClientHandler
    {
        UseCookies = true,
        CookieContainer = cookieContainer
    };

    public static HttpClient Client { get; } = new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:5097/")
    };
}
