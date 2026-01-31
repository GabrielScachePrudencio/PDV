using PDV_LANCHES.model;
using PDV_LANCHES.model.dto;
using ServidorLanches.model.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PDV_LANCHES.controller
{
    public class EstoqueController
    {

        public async Task<List<MovimentacaoEstoqueDTO>?> PegarTodoEstoqueMovimentacoes()
        {
            var response = await ApiClient.Client.GetAsync("api/estoques");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<MovimentacaoEstoqueDTO>>();
        }
        public async Task<MovimentacaoEstoqueDTO?> PegarEstoqueMovimentacoes(int id)
        {
            var response = await ApiClient.Client.GetAsync("api/estoques/" + id);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<MovimentacaoEstoqueDTO>();
        }
        public async Task<bool> DeletarEstoqueMovimentacoes(int id)
        {
            var response = await ApiClient.Client.DeleteAsync("api/estoques/" + id);
            if (!response.IsSuccessStatusCode)
                return false;
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AtualizarEstoque(int idP, int qtdd)
        {
            var response = await ApiClient.Client
                .PostAsync($"api/estoques/atualizarQuantidade/{idP}/{qtdd}", null);

            return response.IsSuccessStatusCode;
        }


    }
}
