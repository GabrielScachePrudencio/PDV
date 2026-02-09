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

    public partial class EscolhaQualHome : Window
    {
        private HomeController homeController = new HomeController();
        private Caixa caixa = new Caixa();
        private Usuario usuarioLogado;

        public EscolhaQualHome()
        {
            InitializeComponent();
            CarregarDadosUsuario();
            caixa = Status_Categorias.Instancia.caixa;
        }


        private async Task CarregarDadosUsuario()
        {
            var usuario = await homeController.pegarUsuarioLogado();
            if (usuario == null)
            {
                MessageBox.Show("Sessão expirada. Faça login novamente.");
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
                return;
            }

            usuarioLogado = usuario;
            usuarioLogadoHome.Text = usuario.Nome;

            if (usuario.TipoUsuario == TipoUsuario.Vendedor)
            {
                irAdministrativo.Visibility = Visibility.Hidden;
            }
        }
        private async void Sair_Click(object sender, RoutedEventArgs e)
        {
            if (caixa != null)
            {
                RelatorioFechamentoCaixa relatorioFechamentoCaixa = new RelatorioFechamentoCaixa(false, false, true);
                relatorioFechamentoCaixa.Show();
                this.Close();
            }
            else
            {
                await homeController.Logout();
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
            }
        }
        
        private void IrVendas(object sender, RoutedEventArgs e)
        {

            if (caixa == null)
            {
                MessageBox.Show("Nenhum caixa está aberto. Por favor, abra um caixa antes de acessar o sistema de vendas.", "Caixa Fechado", MessageBoxButton.OK, MessageBoxImage.Warning);
                EscolhaDeCaixas EscolhaDeCaixas = new EscolhaDeCaixas();
                EscolhaDeCaixas.Show();
            }
            else
            {
                MessageBox.Show("caixa está " + caixa.id + " esta Aberto");
                Home home = new Home();
                home.Show();
            }
            this.Close();

        }
        
        private void IrAdministrativo(object sender, RoutedEventArgs e)
        {
            HomeAdministrativo homeAdministrativo = new HomeAdministrativo();
            homeAdministrativo.Show();
            this.Close();
        }
        
    }
 }