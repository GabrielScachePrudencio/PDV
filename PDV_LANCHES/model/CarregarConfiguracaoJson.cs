using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Text.Json; // Lib nativa da Microsoft

namespace PDV_LANCHES.model
{
    public class CarregarConfiguracaoJson
    {
        public static ConfiguracoesBanco ObterConfiguracao()
        {
            string caminho = @"C:\Users\Flavio\Desktop\PDVSERVIDOR_LANCHES\PDV_LANCHES\PDV_LANCHES\src\date\conexao\configConexao.json";

            try
            {
                if (!File.Exists(caminho))
                {
                    MessageBox.Show("Arquivo de configuração não encontrado!");
                    return null;
                }

                string conteudo = File.ReadAllText(caminho);

                // Regex para pegar o que está entre { }
                var match = Regex.Match(conteudo, @"\{.*\}", RegexOptions.Singleline);

                if (match.Success)
                {
                    string jsonPuro = match.Value;

                    // Configuração para ignorar se o nome no JS for minúsculo e na classe for Maiúsculo
                    var opcoes = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    // Converte usando a lib nativa
                    return JsonSerializer.Deserialize<ConfiguracoesBanco>(jsonPuro, opcoes);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar arquivo com System.Text.Json: " + ex.Message);
            }

            return null;
        }
    }
}