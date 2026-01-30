using PDV_LANCHES.model;
using PDV_LANCHES.Views;
using ServidorLanches.model;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PDV_LANCHES.controller
{
    internal class Login
    {
        private static readonly HttpClient cliente = new HttpClient();


        //primeior teste simples so para ver se ta funcionando
        public async Task<ResultadoApi> VerificarConexaoServidor()
        {

            try
            {
                using var response = await ApiClient.Client.GetAsync("api/auth/teste-conexao");
                var conteudo = await response.Content.ReadAsStringAsync();

                return new ResultadoApi
                {
                    Sucesso = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode,
                    Mensagem = conteudo
                };
            }
            catch (HttpRequestException ex)
            {
                return new ResultadoApi
                {
                    Sucesso = false,
                    StatusCode = 404,
                    Mensagem = "Servidor não encontrado ou offline."
                };
            }
        }


        //esse aqui é para testar tudo inclusive configurar o banco
        private static readonly HttpClient cliente2 = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        public async Task<ResultadoApi> VerificarConexaoDireta(ConfiguracoesBanco configuracao)
        {
            // 1. Normaliza o Host
            if (!configuracao.Host.StartsWith("http"))
                configuracao.Host = "http://" + configuracao.Host;

            // 2. Monta a URL completa aqui, sem mexer no BaseAddress do cliente2
            string urlCompleta = $"{configuracao.Host}:{configuracao.Porta}/api/auth/atualizar-banco";

            try
            {
                // 3. Envia para a URL completa
                var response = await cliente2.PostAsJsonAsync(urlCompleta, configuracao);

                var conteudo = await response.Content.ReadAsStringAsync();

                return new ResultadoApi
                {
                    Sucesso = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode,
                    Mensagem = conteudo
                };
            }
            catch (HttpRequestException ex)
            {
                return new ResultadoApi
                {
                    Sucesso = false,
                    StatusCode = 404,
                    Mensagem = "Servidor não encontrado ou offline."
                };
            }
            catch (TaskCanceledException)
            {
                return new ResultadoApi { Sucesso = false, StatusCode = 408, Mensagem = "Tempo de conexão esgotado" };
            }
            catch (Exception ex)
            {
                return new ResultadoApi { Sucesso = false, StatusCode = 500, Mensagem = ex.Message };
            }
        }
        public async Task<Usuario> VerificarLogin(string nomeI, string senhaI)
        {
            try
            {

                var response = await ApiClient.Client.PostAsJsonAsync(
                    "api/auth/login",
                    new { Nome = nomeI, Senha = senhaI }
                );

                if (response.IsSuccessStatusCode)
                {
                    var usuario = await response.Content.ReadFromJsonAsync<Usuario>();
                    return usuario;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return null;



                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }


        }

        public async Task<ConfiguracoesGerais?> ConfiguracoesGerais()
        {
            try
            {
                var response = await ApiClient.Client
                    .GetAsync("api/administrativo/configuracoes-gerais");

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content
                    .ReadFromJsonAsync<ConfiguracoesGerais>();
            }
            catch
            {
                return null;
            }
        }

    }
}
