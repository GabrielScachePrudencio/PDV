using PDV_LANCHES.model;
using ServidorLanches.model;
using ServidorLanches.model.dto;
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

        public async Task<List<PedidoDTO>?> PegarTodosPedidos()
        {
            var response = await ApiClient.Client.GetAsync("api/pedidos");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<PedidoDTO>>();
        }

        public async Task<bool> ExcluirAlgumPedido(int idPedido)
        {
            var response = await ApiClient.Client.DeleteAsync("api/pedidos/" + idPedido);
            return response.IsSuccessStatusCode;
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
        public async Task<string> AtualizarStatusPedido(int id, StatusPedido status)
        {
            try
            {
                // 1. Enviamos o nome do enum como string (ex: "Cancelado")
                // O servidor espera um JSON body contendo apenas a string entre aspas
                // No seu WPF
                var response = await ApiClient.Client.PutAsync($"api/pedidos/{id}/status/{status.ToString()}", null);
                // 2. Lemos o que o servidor escreveu no corpo da resposta (Body)
                string conteudoResposta = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return conteudoResposta; // Retorna "Sucesso" ou o que você deu de Ok()
                }
                else
                {
                    // Retorna a mensagem de erro vinda do servidor (ex: "ID não encontrado")
                    return $"Erro {response.StatusCode}: {conteudoResposta}";
                }
            }
            catch (Exception ex)
            {
                // Em caso de explosão (sem internet, servidor offline), retorna o erro técnico
                return $"Exception: {ex.Message}\n{ex.StackTrace}";
            }
        }
    }


    }

