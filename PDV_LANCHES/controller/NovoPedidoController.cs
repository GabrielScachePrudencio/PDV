using PDV_LANCHES.model;
using ServidorLanches.model.dto;
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
            var response = await ApiClient.Client.GetAsync("api/cardapio");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<Cardapio>>();
        }

        public async Task<bool> criarPedido(PedidoDTO pedido)
        {
            var response = await ApiClient.Client.PostAsJsonAsync("api/pedidos", pedido);
            return response.IsSuccessStatusCode;
        }
    }
}
