using PDV_LANCHES.model;
using ServidorLanches.model;
using ServidorLanches.model.dto;
using System.Net.Http.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;

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

        public async Task<List<PedidoDTO>?> PegarTodosPedidosToday()
        {
            var response = await ApiClient.Client.GetAsync("api/pedidos/pedidosToday");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<PedidoDTO>>();
        }

        public async Task<List<PedidoDTO>?> PegarPedidosPorCaixa(int idCaixa)
        {
            var response = await ApiClient.Client.GetAsync($"api/pedidos/pedidosPorCaixa/{idCaixa}");
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
        public async Task<bool> AtualizarStatusPedido(int id, int idstatus)
        {
            try
            {
                var response = await ApiClient.Client.PutAsync($"api/pedidos/{id}/status/{idstatus}", null);
                string conteudoResposta = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<string> AtualizarStatusPedido(int id, string status)
        {
            try
            {
                var response = await ApiClient.Client.PutAsync($"api/pedidos/{id}/status/{status.ToString()}", null);
                string conteudoResposta = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return conteudoResposta;
                }
                else
                {
                    return $"Erro {response.StatusCode}: {conteudoResposta}";
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}\n{ex.StackTrace}";
            }
        }



        //caixas

        public async Task<List<TerminalCaixa>?> GetAllTerminaisCaixa()
        {
            var response = await ApiClient.Client.GetAsync("api/caixas/todosCaixasTerminal");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<TerminalCaixa>>();

        }
        
        public async Task<Caixa?> AbrirCaixa(Caixa caixa)
        {
            var response = await ApiClient.Client.PostAsJsonAsync($"api/caixas/abrircaixa", caixa);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<Caixa>();
        }
        public async Task<Caixa?> FecharCaixa(Caixa caixa)
        {
            var response = await ApiClient.Client.PostAsJsonAsync($"api/caixas/fecharcaixa", caixa);
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<Caixa>();
        }
        public async Task<bool> SalvarCaixa(Caixa caixa)
        {
            var response = await ApiClient.Client.PostAsJsonAsync($"api/caixas/salvarfecharcaixa", caixa);
            if (!response.IsSuccessStatusCode)
                return false;
            return true;
        }
        public async Task<List<Caixa>?> ObterTodosCaixas()
        {
            var response = await ApiClient.Client.GetAsync("api/caixas/todosCaixas");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<Caixa>>();

        }

        

    }
}
