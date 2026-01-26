using PDV_LANCHES.model;
using ServidorLanches.model;
using System.Net.Http.Json;

namespace PDV_LANCHES.controller
{
    public class HomeAdministrativoController
    {
        // ================= USUÁRIOS =================

        public async Task<List<Usuario>?> getAllUsuarios()
        {
            var response = await ApiClient.Client.GetAsync("api/administrativo/usuarios");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<List<Usuario>>();
        }
        public async Task<bool> deletarUsuario(int id)
        {
            var response = await ApiClient.Client.DeleteAsync("api/administrativo/usuarios/"+id);
            if (!response.IsSuccessStatusCode)
                return false;

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> addUsuario(Usuario u)
        {
            var response = await ApiClient.Client.PostAsJsonAsync("api/administrativo/usuarios/", u);
            if (!response.IsSuccessStatusCode)
                return false;

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> updateUsuario(Usuario u)
        {
            var response = await ApiClient.Client.PutAsJsonAsync("api/administrativo/usuarios/", u);
            if (!response.IsSuccessStatusCode)
                return false;

            return response.IsSuccessStatusCode;
        }



        // ================= PRODUTOS =================

        public async Task<List<Produto>?> getAllProdutos()
        {
            var response = await ApiClient.Client.GetAsync("api/produtos/");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<List<Produto>>();
        }
       


        public async Task<bool> addProduto(Produto produto)
        {
            var response = await ApiClient.Client.PostAsJsonAsync("api/produtos/", produto);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> updateProduto(Produto produto)
        {
            var response = await ApiClient.Client.PutAsJsonAsync("api/produtos/", produto);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> deletarProduto(int id)
        {
            var response = await ApiClient.Client.DeleteAsync("api/produtos/"+id);
            return response.IsSuccessStatusCode;
        }

        // ================= CONFIGURAÇÃO FISCAL =================

        public async Task<ConfiguracoesFiscais?> GetConfiguracaoFiscal()
        {
            var response = await ApiClient.Client.GetAsync("api/administrativo/configuracoesFiscais");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ConfiguracoesFiscais>();
        }

        public async Task<bool> AddConfiguracaoFiscal(ConfiguracoesFiscais config)
        {
            var response = await ApiClient.Client.PostAsJsonAsync(
                "api/administrativo/configuracoesFiscais", config);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateConfiguracaoFiscal(ConfiguracoesFiscais config)
        {
            var response = await ApiClient.Client.PutAsJsonAsync(
                "api/administrativo/configuracoesFiscais", config);

            return response.IsSuccessStatusCode;
        }

        // ================= CATEGORIA =================

        public async Task<List<CategoriaProduto>?> getAllCategoria()
        {
            try
            {
                var response = await ApiClient.Client.GetAsync("api/administrativo/categoria");
                return await response.Content.ReadFromJsonAsync<List<CategoriaProduto>>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> AddCategoria(CategoriaProduto categoria)
        {
            var response = await ApiClient.Client.PostAsJsonAsync(
                "api/administrativo/categoria", categoria);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCategoria(int id, CategoriaProduto novoStatus)
        {
            var response = await ApiClient.Client.PutAsJsonAsync(
                $"api/administrativo/categoria/{id}", novoStatus);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategoria(int id)
        {
            var response = await ApiClient.Client.DeleteAsync(
                $"api/administrativo/categoria/{id}");

            return response.IsSuccessStatusCode;
        }

        // ================= STATUS PEDIDO =================

        public async Task<List<TipoStatusPedido>?> getAllStatus()
        {
            try
            {
                var response = await ApiClient.Client.GetAsync("api/administrativo/statuspedido");
                return await response.Content.ReadFromJsonAsync<List<TipoStatusPedido>>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> AddStatusPedido(TipoStatusPedido status)
        {
            var response = await ApiClient.Client.PostAsJsonAsync(
                "api/administrativo/statuspedido", status);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateStatusPedido(int id, TipoStatusPedido status)
        {
            var response = await ApiClient.Client.PutAsJsonAsync(
                $"api/administrativo/statuspedido/{id}", status);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteStatusPedido(int id)
        {
            var response = await ApiClient.Client.DeleteAsync(
                $"api/administrativo/statuspedido/{id}");

            return response.IsSuccessStatusCode;
        }

        // ================= FORMAS DE PAGAMENTO =================

        public async Task<List<FormaDePagamento>> getAllFormasDePagamentos()
        {
            try
            {
                var response = await ApiClient.Client.GetAsync("api/administrativo/formasdepagamentos");
                if (!response.IsSuccessStatusCode)
                    return new List<FormaDePagamento>();

                return await response.Content.ReadFromJsonAsync<List<FormaDePagamento>>();
            }
            catch
            {
                return new List<FormaDePagamento>();
            }
        }

        public async Task<bool> AddFormaPagamento(FormaDePagamento forma)
        {
            var response = await ApiClient.Client.PostAsJsonAsync(
                "api/administrativo/formasdepagamentos", forma);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateFormaPagamento(int id, FormaDePagamento forma)
        {
            var response = await ApiClient.Client.PutAsJsonAsync(
                $"api/administrativo/formasdepagamentos/{id}", forma);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteFormaPagamento(int id)
        {
            var response = await ApiClient.Client.DeleteAsync(
                $"api/administrativo/formasdepagamentos/{id}");

            return response.IsSuccessStatusCode;
        }

        // ================= CUPONS =================

        public async Task<List<CupomDesconto>?> GetAllCuponsDesconto()
        {
            try
            {
                var response = await ApiClient.Client.GetAsync("api/administrativo/cuponsDeDesconto");
                return await response.Content.ReadFromJsonAsync<List<CupomDesconto>>();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> AddCupom(CupomDesconto cupom)
        {
            var response = await ApiClient.Client.PostAsJsonAsync(
                "api/administrativo/cuponsDeDesconto", cupom);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCupom(int id, CupomDesconto cupom)
        {
            var response = await ApiClient.Client.PutAsJsonAsync(
                $"api/administrativo/cuponsDeDesconto/{id}", cupom);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCupom(int id)
        {
            var response = await ApiClient.Client.DeleteAsync(
                $"api/administrativo/cuponsDeDesconto/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
