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
    public class PedidoInfoController
    {
        public async Task<PedidoDTO?> getPedidoById(int idPedido)
        {

            var response = await ApiClient.Client.GetAsync("api/pedidos/" + idPedido);
            
            if(!response.IsSuccessStatusCode)
                return null;    

            return await response.Content.ReadFromJsonAsync<PedidoDTO>();
        }

        public async Task<bool> AtualizarPedido(PedidoDTO pedido)
        {
            var response = await ApiClient.Client.PutAsJsonAsync("api/pedidos/add-pedido-itens", pedido);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Falha ao atualizar o pedido.");
               
            return true;
        }


    }
}
