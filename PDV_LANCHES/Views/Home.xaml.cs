using Mysqlx;
using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using ServidorLanches.model;
using ServidorLanches.model.dto;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
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


        private async Task PopularFiltroStatus()
        {
            await Status_Categorias.Instancia.CarregarAsync();

            var lista = new List<TipoStatusPedido>();

            lista.AddRange(Status_Categorias.Instancia.TipoStatusPedido);

            comboStatusFiltro.ItemsSource = lista;
            comboStatusFiltro.DisplayMemberPath = "nome";
            comboStatusFiltro.SelectedValuePath = "id";
            comboStatusFiltro.SelectedValue = -1;
        }


            private void AplicarFiltros()
            {
            if (pedidos == null) return;

            var statusSelecionado = comboStatusFiltro.SelectedItem as TipoStatusPedido;
            string buscaTexto = txtBuscaCliente.Text?.Trim().ToLower();

            var listaFiltrada = pedidos.Where(p =>
            {
                bool statusOk = true;
                if (statusSelecionado != null &&
                    statusSelecionado.nome != "Todos" &&
                    statusSelecionado.nome != "Não se aplica")
                {
                    statusOk = p.IdStatus == statusSelecionado.id;
                }

                bool buscaOk = string.IsNullOrEmpty(buscaTexto) ||
                               (p.CpfCliente != null && p.CpfCliente.Contains(buscaTexto)) ||
                               (p.NomeCliente != null && p.NomeCliente.ToLower().Contains(buscaTexto));

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
                this.Close();
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
        public void Estoque_Click(object sender, RoutedEventArgs e)
        {
            Estoque estoque = new Estoque();
            estoque.Show();
            this.Close();
        }
        public async void ApagarPedido_Click(object sender, RoutedEventArgs e)
        {

            

            MessageBoxResult resultado = MessageBox.Show("Tem certeza que deseja excluir o pedido selecionado?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                var botao = sender as Button;
                var pedidoSelecionado = botao.DataContext as PedidoDTO;
                bool sucess = await homeController.ExcluirAlgumPedido(pedidoSelecionado.Id);
                if (sucess)
                {
                    Home home = new Home();
                    home.Show();
                    this.Close();
                }else
                {
                    MessageBox.Show("Erro ao excluir o pedido.");
                }
            }
        }

        private void novoPedido_Click(object sender, RoutedEventArgs e)
        {
            NovoPedido np = new NovoPedido();
            np.Show();
            this.Close();
        }

        private void todasVendas_Click(object sender, RoutedEventArgs e)
        {
            TodasVendasCompleto todasvendacompl = new TodasVendasCompleto();
            todasvendacompl.Show();
            this.Close();
        }

        private async void comboStatusPedido_Loaded(object sender, RoutedEventArgs e)
        {
            await Status_Categorias.Instancia.CarregarAsync();


            var combo = sender as ComboBox;
            var pedido = combo.Tag as PedidoDTO;

            if(Status_Categorias.Instancia.TipoStatusPedido == null)
            {
                MessageBox.Show("erro veio null status pedidos");
            }

            combo.ItemsSource = Status_Categorias.Instancia.TipoStatusPedido;
            combo.DisplayMemberPath = "nome";
            combo.SelectedValuePath = "id";

            if (pedido != null)
                combo.SelectedValue = pedido.IdStatus;
        }
        
        private async void comboFormaPagamento_Loaded(object sender, RoutedEventArgs e)
        {
            await Status_Categorias.Instancia.CarregarAsync();


            var combo = sender as ComboBox;
            var pedido = combo.Tag as PedidoDTO;

            if(Status_Categorias.Instancia.FormaDePagamentos == null)
            {
                MessageBox.Show("erro veio null forma de pagamento");
            }

            combo.ItemsSource = Status_Categorias.Instancia.FormaDePagamentos;
            combo.DisplayMemberPath = "Descricao";
            combo.SelectedValuePath = "Id";

            if (pedido != null)
                combo.SelectedValue = pedido.IdFormaPagamento;
        }

        private async void ComboBoxPedido_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            if (!combo.IsLoaded) return;

            var pedido = combo.Tag as PedidoDTO;
            var statusSelecionado = combo.SelectedItem as TipoStatusPedido;

            if (pedido == null || statusSelecionado == null) return;
            if (pedido.IdStatus == statusSelecionado.id) return;

            bool sucesso = await homeController.AtualizarStatusPedido(
                pedido.Id,
                statusSelecionado.id
            );

            if (sucesso)
            {
                pedido.IdStatus = statusSelecionado.id;
            }
            else
            {
                MessageBox.Show("Erro ao atualizar status.");
                combo.SelectedValue = pedido.IdStatus;
            }
        }

        private async void ComboBoxPedidoFormaDePagamento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            if (!combo.IsLoaded) return;

            var pedido = combo.Tag as PedidoDTO;
            var formaDePagamento = combo.SelectedItem as FormaDePagamento;

            if (pedido == null || formaDePagamento == null) return;
            if (pedido.IdFormaPagamento == formaDePagamento.Id) return;

            bool sucesso = await homeController.AtualizarStatusPedido(
                pedido.Id,
                formaDePagamento.Id
            );

            if (sucesso)
            {
                pedido.IdStatus = formaDePagamento.Id;
            }
            else
            {
                MessageBox.Show("Erro ao atualizar status.");
                combo.SelectedValue = pedido.IdStatus;
            }
        }

        


        private void VoltarParaEscolha_Click(object sender, RoutedEventArgs e)
        {
            EscolhaQualHome tela = new EscolhaQualHome();
            tela.Show();
            Close();
        }
        private async void Sair_Click(object sender, RoutedEventArgs e)
        {
            await homeController.Logout();
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }


        
    }
}