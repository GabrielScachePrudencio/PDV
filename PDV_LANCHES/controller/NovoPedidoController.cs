using PDV_LANCHES.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PDV_LANCHES.controller
{
    public class NovoPedidoController
    {
        public async Task<Usuario?> pegarUsuarioLogado()
        {
            var response = await ApiClient.Client.PostAsync("api/auth/usuario-logado", null);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<Usuario>();
        }

        public async Task<List<Cardapio>> getAllCardapio()
        {
            var response = await ApiClient.Client.PostAsync("api/auth/cardapio-completo", null);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<Cardapio>>();
        }

        public async Task<bool> criarPedido(Pedido pedido)
        {
            var response = await ApiClient.Client.PostAsJsonAsync("api/auth/add-pedido-itens", pedido);
            return response.IsSuccessStatusCode;
        }
    }
}
