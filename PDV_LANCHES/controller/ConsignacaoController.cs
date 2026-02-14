using PDV_LANCHES.model;
using ServidorLanches.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace PDV_LANCHES.controller
{
    public class ConsignacaoController
    {
        public async Task<string> CriarConsignacao(Consignacao c)
        {
            try
            {
                var response = await ApiClient.Client.PostAsJsonAsync("api/consignacoes", c);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



        public async Task<bool> UpdateConsignacao(Consignacao c)
        {
            try
            {
                var response = await ApiClient.Client.PutAsJsonAsync("api/consignacoes", c);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Consignacao>?> GetAllConsignacao()
        {
            var response = await ApiClient.Client.GetAsync("api/consignacoes");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<Consignacao>>();

        }
        public async Task<Consignacao> GetByIdConsignacao(int idconsg)
        {
            var response = await ApiClient.Client.GetAsync($"api/consignacoes/{idconsg}");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<Consignacao>();

        }
        public async Task<string> darBaixaConsignacao(Consignacao consig)
        {
            try
            {
                var response = await ApiClient.Client.PostAsJsonAsync("api/consignacoes/dar-baixa", consig);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<String> estornarBaixaConsignacao(int id)
        {
            try
            {
                var response = await ApiClient.Client.DeleteAsync("api/consignacoes/"+id);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }




    }
}
