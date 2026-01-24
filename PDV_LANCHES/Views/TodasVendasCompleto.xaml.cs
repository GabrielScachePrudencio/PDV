using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using ServidorLanches.model;
using ServidorLanches.model.dto;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PDV_LANCHES.Views
{
    public partial class TodasVendasCompleto : Window
    {
        private HomeController homeController = new HomeController();
        private Usuario usuarioLogado;
        private ObservableCollection<PedidoDTO> pedidos = new ObservableCollection<PedidoDTO>();

        public TodasVendasCompleto()
        {
            InitializeComponent();
            Loaded += Home_Loaded;
        }

        private async void Home_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.WhenAll(CarregarDadosUsuario(), CarregarDadosPedidos(), CarregarDadosEmpresa());
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

            dgVendas.ItemsSource = pedidos;
        }
        private void VerDetalhes_click(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as DataGrid;

            if (grid != null && grid.SelectedItem != null)
            {
                var pedido = (PedidoDTO)grid.SelectedItem;
                bool veioDeTodasVendas = true;

                PedidoInfo pedidoInfo = new PedidoInfo(pedido.Id, veioDeTodasVendas);
                pedidoInfo.Show();
                this.Close();
            }
        }

        private void novoPedido_Click(object sender, RoutedEventArgs e) { /* Lógica */ }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { /* Filtro Status */ }

        private void txtBuscaCliente_KeyUp(object sender, KeyEventArgs e) { /* Filtro CPF */ }

        private void VoltarPedidos_Click(object sender, RoutedEventArgs e)
        {
            Home home = new Home();
            home.Show();
            this.Close();
        }

        private void Sair_Click(object sender, RoutedEventArgs e) { this.Close(); }

        private void comboStatusPedido_Loaded(object sender, RoutedEventArgs e) { /* Carregar status */ }

        private void ComboBoxPedido_SelectionChanged(object sender, SelectionChangedEventArgs e) { /* Mudar status */ }

        private void VerDetalhes_click(object sender, RoutedEventArgs e) { /* Detalhes */ }

        private void ApagarPedido_Click(object sender, RoutedEventArgs e) { /* Deletar */ }

        private void dgVendas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}