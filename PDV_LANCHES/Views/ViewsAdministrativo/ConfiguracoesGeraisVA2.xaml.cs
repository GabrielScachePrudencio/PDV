using Microsoft.Win32;
using PDV_LANCHES.controller;
using ServidorLanches.model;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PDV_LANCHES.Views.ViewsAdministrativo
{
    public partial class ConfiguracoesGeraisVA2 : UserControl
    {
        private ConfiguracoesGerais configuracoesGerais;
        private HomeController homeController = new HomeController();
        private string caminhoImagemSelecionada;

        public ConfiguracoesGeraisVA2()
        {
            InitializeComponent();
            CarregarConfiguracoesGerais();
        }

        private async void CarregarConfiguracoesGerais()
        {
            try
            {
                Login login = new Login();
                configuracoesGerais = await login.ConfiguracoesGerais();

                if (configuracoesGerais == null)
                    return;

                if (!string.IsNullOrEmpty(configuracoesGerais.pathImagemLogo) &&
                    File.Exists(configuracoesGerais.pathImagemLogo))
                {
                    pdvLogo.Source = CarregarImagem(configuracoesGerais.pathImagemLogo);
                    caminhoImagemSelecionada = configuracoesGerais.pathImagemLogo;
                }

                pdvNome.Text = configuracoesGerais.nomeFantasia ?? configuracoesGerais.nome;
                pdvTelefone.Text = configuracoesGerais.telefone;
                pdvEmail.Text = configuracoesGerais.email;
                pdvEndereco.Text = configuracoesGerais.endereco;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados:\n" + ex.Message);
            }
        }

        private BitmapImage CarregarImagem(string caminho)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(caminho, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        private void pdvLogo_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Selecionar Logotipo",
                Filter = "Imagens (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (dialog.ShowDialog() == true)
            {
                caminhoImagemSelecionada = dialog.FileName;
                pdvLogo.Source = CarregarImagem(caminhoImagemSelecionada);
            }
        }

        private async void salvarConfiguracoes_click(object sender, RoutedEventArgs e)
        {
            if (configuracoesGerais == null)
                return;

            try
            {
                configuracoesGerais.nomeFantasia = pdvNome.Text;
                configuracoesGerais.telefone = pdvTelefone.Text;
                configuracoesGerais.email = pdvEmail.Text;
                configuracoesGerais.endereco = pdvEndereco.Text;
                configuracoesGerais.pathImagemLogo = caminhoImagemSelecionada;

                
                await homeController.AtualizarConfiguracoes(configuracoesGerais);

                MessageBox.Show("Configurações salvas com sucesso!",
                                "Sucesso",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar:\n" + ex.Message);
            }
        }
    }
}
