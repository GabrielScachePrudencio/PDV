using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using PDV_LANCHES.Views.ViewsAdministrativo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PDV_LANCHES.Views
{
    /// <summary>
    /// Lógica interna para HomeAdministrativo.xaml
    /// </summary>
    public partial class HomeAdministrativo : Window
    {
        private HomeController homeController = new HomeController();
        private Usuario usuarioLogado;
        public HomeAdministrativo()
        {
            InitializeComponent();
            Loaded += HomeAdministrativo_Loaded;
        }

        private async void HomeAdministrativo_Loaded(object sender, RoutedEventArgs e)
        {
            await CarregarDadosUsuario();
        }   

        private async Task CarregarDadosUsuario()
        {
            var usuario = await homeController.pegarUsuarioLogado();
            if (usuario == null)
            {
                MessageBox.Show("Sessão expirada. Faça login novamente.");
                Close();
                return;
            }
            usuarioLogado = usuario;
            usuarioLogadoHome.Text = usuario.Nome;
        }

        private void AllCardapio(Object sender, RoutedEventArgs e)
        {
            ConteudoConfiguracoes.Content = new AllCardapio();
        }
        private void AllRelatorio(Object sender, RoutedEventArgs e)
        {
            ConteudoConfiguracoes.Content = new AllRelatorio();
        }
        private void AllUsuario(Object sender, RoutedEventArgs e)
        {
            ConteudoConfiguracoes.Content = new AllUsuario();
        }
        private void ConfiguracaoFiscal(Object sender, RoutedEventArgs e)
        {
            ConteudoConfiguracoes.Content = new ConfiguracaoFiscal();
        }
        private void ConfiguracaoBanco(Object sender, RoutedEventArgs e)
        {
            ConteudoConfiguracoes.Content = new ConfiguracaoBanco();
        }

        private async void Sair_Click(object sender, RoutedEventArgs e)
        {
            await homeController.Logout();
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
        private void VoltarParaEscolha_Click(object sender, RoutedEventArgs e)
        {
            EscolhaQualHome voltarParaEscolha = new EscolhaQualHome();
            voltarParaEscolha.Show();
            this.Close();
        }
    }
}
