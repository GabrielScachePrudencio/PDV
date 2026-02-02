using PDV_LANCHES.model;
using ServidorLanches.model.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public async Task<List<Produto>> getAllProduto()
        {
            var response = await ApiClient.Client.GetAsync("api/produtos");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<Produto>>();
        }
        public async Task<List<Produto>> getAllProdutoAtivos()
        {
            var response = await ApiClient.Client.GetAsync("api/produtos/ativos");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<Produto>>();
        }

        public async Task<string> CriarPedido(PedidoDTO pedido)
        {
            try
            {
                var response = await ApiClient.Client.PostAsJsonAsync("api/pedidos", pedido);

                if (response.IsSuccessStatusCode)
                {
                    return "Pedido criado com sucesso!";
                }

                // Lê a mensagem de erro vinda da API (aquela que configuramos na Controller)
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"Erro na API: {errorContent}";
            }
            catch (HttpRequestException ex)
            {
                return "Erro de conexão: Verifique se o servidor está ligado.";
            }
            catch (Exception ex)
            {
                return $"Erro inesperado: {ex.Message}";
            }
        }
    }
}
