using System.Net;
using System.Net.Http;
using PDV_LANCHES.model; // Onde está o seu CarregarConfiguracaoJson

public static class ApiClient
{
    private static CookieContainer cookieContainer = new CookieContainer();

    private static HttpClientHandler handler = new HttpClientHandler
    {
        UseCookies = true,
        CookieContainer = cookieContainer
    };

    private static HttpClient _client;

    public static HttpClient Client
    {
        get
        {
            if (_client == null)
            {
                var config = CarregarConfiguracaoJson.ObterConfiguracao();

                // 1. Limpa o host de qualquer espaço e garante que tenha o http://
                string host = config?.Host?.Trim() ?? "localhost";

                if (!host.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    host = "http://" + host;
                }

                // 2. Monta a URL base garantindo que termine com uma barra /
                string urlBase = $"{host}:{config?.Porta ?? 5097}/";

                _client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(urlBase)
                };
            }
            return _client;
        }
    }

    public static void ReiniciarCliente()
    {
        _client = null;
    }
}