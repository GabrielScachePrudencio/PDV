using PDV_LANCHES.controller;
using PDV_LANCHES.model;
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
    /// Lógica interna para EscolhaDeCaixas.xaml
    /// </summary>
    public partial class EscolhaDeCaixas : Window
    {
        private HomeController homeController = new HomeController();
        private Usuario usuarioLogado;

        public EscolhaDeCaixas()
        {
            InitializeComponent();
            CarregarDadosUsuario();
            CarregarCaixas();
        }


        private async Task CarregarDadosUsuario()
        {
            var usuario = await homeController.pegarUsuarioLogado();

            if (usuario == null)
            {
                MessageBox.Show("Sessão expirada. Faça login novamente.");
                new MainWindow().Show();
                this.Close();
                return;
            }

            usuarioLogado = usuario;
        }

        private async Task CarregarCaixas()
        {
            await Status_Categorias.Instancia.CarregarAsync();

            var caixas = Status_Categorias.Instancia.caixasTerminaisAbertos;

            dgCaixas.ItemsSource = caixas; 
        }

        private void AbrirCaixa_Click(object sender, RoutedEventArgs e)
        {

            if (dgCaixas.SelectedItem == null)
            {
                MessageBox.Show("Selecione um caixa.");
                return;
            }

            TerminalCaixa selecionado = (TerminalCaixa)dgCaixas.SelectedItem;

            Status_Categorias.Instancia.caixaterminalSelecionado = selecionado;
            Status_Categorias.Instancia.iniciarCaixaNovo(selecionado, usuarioLogado, Convert.ToDecimal(txtValorInicial.Text));

            new Home().Show();
            this.Close();
        }
        private async void Sair_Click(object sender, RoutedEventArgs e) {
            await homeController.Logout(); MainWindow loginWindow = new MainWindow(); loginWindow.Show(); this.Close();
        }

        private void dgCaixas_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            
        }
    }

}
