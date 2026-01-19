using PDV_LANCHES.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PDV_LANCHES.controller
{
    public class HomeAdministrativoController
    {
        public async Task<List<Usuario>?> getAllUsuarios()
        {
            var response = await ApiClient.Client.GetAsync("api/administrativo/usuarios");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<Usuario>>();
        }

        public async Task<List<Cardapio>?> getAllCardapio()
        {
            var response = await ApiClient.Client.GetAsync("api/cardapio/");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<Cardapio>>();
        }


    }
}
