using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Text.Json; // Lib nativa da Microsoft

namespace PDV_LANCHES.model
{
    public class CarregarConfiguracaoJson
    {
        // Método auxiliar para pegar o caminho correto dinamicamente
        private static string ObterCaminhoArquivo()
        {
            // Em produção, é melhor salvar em AppData para evitar bloqueios do Windows
            string pastaAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string pastaApp = Path.Combine(pastaAppData, "PDV_Lanches_Config");

            if (!Directory.Exists(pastaApp)) Directory.CreateDirectory(pastaApp);

            return Path.Combine(pastaApp, "configConexao.json");
        }

        public static ConfiguracoesBanco ObterConfiguracao()
        {
            string caminho = ObterCaminhoArquivo();

            try
            {
                // Se o arquivo não existir, tenta criar a pasta para não dar erro de diretório
                if (!File.Exists(caminho))
                {
                    Console.WriteLine("Arquivo não encontrado em: " + caminho);
                    return null;
                }

                string jsonPuro = File.ReadAllText(caminho);
                return JsonSerializer.Deserialize<ConfiguracoesBanco>(jsonPuro, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro no JSON: " + ex.Message);
                return null;
            }
        }

        public static void setConfiguracoes(ConfiguracoesBanco configuracoesBanco)
        {
            string caminho = ObterCaminhoArquivo();

            try
            {
                // Garante que as pastas existam antes de salvar
                string pasta = Path.GetDirectoryName(caminho);
                if (!Directory.Exists(pasta)) Directory.CreateDirectory(pasta);

                var config = configuracoesBanco; // Salva o que recebeu diretamente

                var opcoes = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(config, opcoes);

                File.WriteAllText(caminho, jsonString);
                MessageBox.Show("Configurações salvas com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar arquivo: " + ex.Message);
            }
        }
    }
}