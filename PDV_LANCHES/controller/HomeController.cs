using PDV_LANCHES.model;
using ServidorLanches.model;
using System.Net.Http.Json;

namespace PDV_LANCHES.controller
{
    public class HomeController
    {
        public async Task<Usuario?> pegarUsuarioLogado()
        {
            var response = await ApiClient.Client.PostAsync("api/auth/usuario-logado", null);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<Usuario>();
        }

        public async Task Logout()
        {
            await ApiClient.Client.PostAsync("api/auth/logout", null);
        }

        public async Task<List<Pedido>?> PegarTodosPedidos()
        {
            var response = await ApiClient.Client.GetAsync("api/pedidos");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<Pedido>>();
        }

        public async Task<bool> AtualizarConfiguracoes(ConfiguracoesGerais config)
        {
            try
            {
                var response = await ApiClient.Client.PutAsJsonAsync("api/administrativo/configuracoes", config);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
