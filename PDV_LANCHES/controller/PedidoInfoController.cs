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
    public class PedidoInfoController
    {
        public async Task<PedidoDTO?> getPedidoById(int idPedido)
        {

            var response = await ApiClient.Client.GetAsync("api/pedidos/" + idPedido);
            
            if(!response.IsSuccessStatusCode)
                return null;    

            return await response.Content.ReadFromJsonAsync<PedidoDTO>();
        }

        public async Task<string> AtualizarPedido(PedidoDTO pedido)
        {
            // Validação básica antes de enviar
            if (pedido == null || pedido.Id <= 0)
                return "Dados do pedido inválidos para atualização.";

            try
            {
                var response = await ApiClient.Client.PutAsJsonAsync("api/pedidos", pedido);

                // Lê o conteúdo da resposta (seja sucesso ou erro)
                var resultMessage = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Retorna a mensagem de sucesso da API ou um padrão
                    return string.IsNullOrWhiteSpace(resultMessage) ? "Pedido atualizado com sucesso!" : resultMessage;
                }

                // Se chegou aqui, a API retornou erro (ex: 400 Bad Request)
                // O resultMessage conterá o erro enviado pela sua Controller
                return $"Falha ao atualizar: {resultMessage}";
            }
            catch (HttpRequestException ex)
            {
                // Erros de rede (servidor offline, queda de internet)
                return "Erro de conexão com o servidor.";
            }
            catch (Exception ex)
            {
                // Erros inesperados de código
                return $"Erro imprevisto: {ex.Message}";
            }
        }




    }
}
