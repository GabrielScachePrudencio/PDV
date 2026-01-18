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

        public async Task<ConfiguracoesGerais> ConfiguracoesGerais()
        {
            try
            {
                var response = await ApiClient.Client.GetAsync("api/auth/configuracoes-gerais");
                if (response.IsSuccessStatusCode)
                {
                    var config = await response.Content.ReadFromJsonAsync<ConfiguracoesGerais>();
                    return config;
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

    }
}
