using PDV_LANCHES.model;
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
        public async Task<Pedido?> getPedidoById(int idPedido)
        {

            var response = await ApiClient.Client.GetAsync("api/pedidos/pedido-info?idPedido=" + idPedido);
            
            if(!response.IsSuccessStatusCode)
                return null;    

            return await response.Content.ReadFromJsonAsync<Pedido>();
        }

        public async Task<bool> AtualizarPedido(Pedido pedido)
        {
            var response = await ApiClient.Client.PutAsJsonAsync("api/pedidos/add-pedido-itens", pedido);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Falha ao atualizar o pedido.");
               
            return true;
        }


    }
}
