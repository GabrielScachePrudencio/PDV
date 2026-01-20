using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using ServidorLanches.model;
using ServidorLanches.model.dto;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PDV_LANCHES.Views
{
    public partial class Home : Window
    {
        private HomeController homeController = new HomeController();
        private Usuario usuarioLogado;
        private ObservableCollection<PedidoDTO> pedidos = new ObservableCollection<PedidoDTO>();

        public Home()
        {
            InitializeComponent();
            Loaded += Home_Loaded;
        }

        private async void Home_Loaded(object sender, RoutedEventArgs e)
        {
            PopularFiltroStatus();
            await Task.WhenAll(CarregarDadosUsuario(), CarregarDadosPedidos(), CarregarDadosEmpresa());
        }


        private void PopularFiltroStatus()
        {
            var opcoes = new List<string> { "Todos" };
            opcoes.AddRange(Enum.GetNames(typeof(StatusPedido)));
            
            comboStatusFiltro.ItemsSource = opcoes;
            comboStatusFiltro.SelectedIndex = 0;
        }

        private void AplicarFiltros()
        {
            if (pedidos == null) return;

            string statusSelecionado = comboStatusFiltro.SelectedItem as string;
            string buscaTexto = txtBuscaCliente.Text?.Trim().ToLower();

            var listaFiltrada = pedidos.Where(p =>
            {
                bool statusOk =
                    statusSelecionado == "Todos" ||
                    p.StatusPedido.ToString() == statusSelecionado;

                bool buscaOk =
                    string.IsNullOrEmpty(buscaTexto) ||
                    (!string.IsNullOrEmpty(p.CpfCliente) &&
                     p.CpfCliente.ToLower().Contains(buscaTexto));

                return statusOk && buscaOk;
            }).ToList();

            ListaPedidos.ItemsSource = listaFiltrada;
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AplicarFiltros();
        }

        private void txtBuscaCliente_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            AplicarFiltros();
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

            // Mostra o botão de voltar apenas se não for apenas vendedor
            if (usuario.TipoUsuario != TipoUsuario.Vendedor)
            {
                voltarHome.Visibility = Visibility.Visible;
            }
        }

        private async Task CarregarDadosEmpresa()
        {
            try
            {
                Login login = new Login();
                ConfiguracoesGerais config = await login.ConfiguracoesGerais();

                if (config != null)
                {
                    // Define o nome da loja
                    txtNomeLoja.Text = (config.nomeFantasia ?? config.nome ?? "MINHA LOJA").ToUpper();

                    // Carrega a imagem da logo
                    if (!string.IsNullOrEmpty(config.pathImagemLogo) && File.Exists(config.pathImagemLogo))
                    {
                        imgLogoMenu.Source = LoadImage(config.pathImagemLogo);
                        txtLogoPlaceholder.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtLogoPlaceholder.Visibility = Visibility.Visible;
                    }
                }
            }
            catch
            {
                txtNomeLoja.Text = "PDV LANCHES";
                txtLogoPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private BitmapImage LoadImage(string path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad; // Importante para não travar o arquivo
            bitmap.EndInit();
            return bitmap;
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
            foreach (var l in lista)
            {
                pedidos.Add(l);
            }

            ListaPedidos.ItemsSource = pedidos;
        }

        public void VerDetalhes_click(object sender, RoutedEventArgs e)
        {
            var botao = sender as Button;
            var pedidoSelecionado = botao.DataContext as PedidoDTO;
            if (pedidoSelecionado != null)
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

        private void VoltarParaEscolha_Click(object sender, RoutedEventArgs e)
        {
            EscolhaQualHome tela = new EscolhaQualHome();
            tela.Show();
            Close();
        }

        
    }
}