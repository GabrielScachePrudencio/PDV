using PDV_LANCHES.Views;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PDV_LANCHES.controller
{
    internal class Login
    {
        private static readonly HttpClient cliente = new HttpClient();

        public async Task<bool> VerificarLogin(string nomeI, string senhaI)
        {
            try
            {
                var response = await ApiClient.Client.PostAsJsonAsync(
                "api/auth/login",
                new { Nome = nomeI, Senha = senhaI }
                );

                if (response.IsSuccessStatusCode)
                    return true;

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return false;



                return false;
            }
            catch (HttpRequestException)
            {
                return false;
            }


        }
    }
}
