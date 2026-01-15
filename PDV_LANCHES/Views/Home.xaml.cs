using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PDV_LANCHES.Views
{
    public partial class Home : Window
    {
        private HomeController homeController = new HomeController();
        private Usuario usuarioLogado;
        private ObservableCollection<Pedido> pedidos = new ObservableCollection<Pedido>();
        public Home()
        {
            InitializeComponent();
            Loaded += Home_Loaded;
        }

        private async void Home_Loaded(object sender, RoutedEventArgs e)
        {
            await CarregarDadosUsuario();
            await CarregarDadosPedidos();
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

        private async Task CarregarDadosPedidos()
        {
            var lista = await homeController.PegarTodosPedidos();
            if (lista == null)
            {
                MessageBox.Show("Erro ao carregar pedidos.");
                return;
            }

            pedidos.Clear();
            foreach(var l in lista)
            {
                pedidos.Add(l);
            }
            
            ListaPedidos.ItemsSource = pedidos;
            

        }

        public void VerDetalhes_click(object sender, RoutedEventArgs e)
        {
            var botao = sender as Button;
            var pedidoSelecionado = botao.DataContext as Pedido;
            if(pedidos != null)
            {
                PedidoInfo telaInfo = new PedidoInfo(pedidoSelecionado.Id);
                telaInfo.Show();
                this.Close();
            }

        }

        private async void Sair_Click(object sender, RoutedEventArgs e)
        {
            await homeController.Logout();
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void novoPedido_Click(object sender, RoutedEventArgs e)
        {
            NovoPedido np = new NovoPedido();
            np.Show();
            this.Close();
        }
    }
}
