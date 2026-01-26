using System;
using System.Windows;
using System.Windows.Controls;
using PDV_LANCHES.model;

namespace PDV_LANCHES.Views.ViewsAdministrativo
{
    public partial class ConfiguracaoBanco : UserControl
    {
        private ConfiguracoesBanco _config;

        public ConfiguracaoBanco()
        {
            InitializeComponent();
            CarregarDados();
        }

        private void CarregarDados()
        {
            // Usa sua classe que lê o arquivo configConexao.js
            _config = CarregarConfiguracaoJson.ObterConfiguracao();

            if (_config != null)
            {
                // Preenche os campos da tela
                txtHost.Text = _config.Host;
                txtPorta.Text = _config.Porta.ToString();
                txtBanco.Text = _config.NomeBanco;
                txtUsuario.Text = _config.Usuario;
                txtCaminhoBackup.Text = _config.CaminhoBackup;

                lblHost.Text = _config.Host;
                lblDataBackup.Text = _config.UltimoBackup?.ToString("dd/MM/yyyy HH:mm") ?? "Nenhum";
            }
        }

        private void Testar_Click(object sender, RoutedEventArgs e)
        {
            // Aqui você chamaria sua controller para testar o MySQL
            MessageBox.Show($"Testando conexão com {txtHost.Text}...");
        }

        private void SelecionarPasta_Click(object sender, RoutedEventArgs e)
        {
            // Abre seletor de pastas
        }

        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Rotina de backup iniciada...");
        }
    }
}