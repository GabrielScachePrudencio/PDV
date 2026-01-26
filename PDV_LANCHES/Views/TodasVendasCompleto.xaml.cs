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
            await Task.WhenAll(CarregarDadosUsuario(), CarregarDadosPedidos());
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


        }



        private async Task CarregarDadosPedidos()
        {
            await Status_Categorias.Instancia.CarregarAsync();

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




            //status
            comboStatus.ItemsSource = Status_Categorias.Instancia.TipoStatusPedido;
            comboStatus.DisplayMemberPath = "nome";
            comboStatus.SelectedValuePath = "id";


            //formas de pagamentos 
            comboFormaDePagamento.ItemsSource = Status_Categorias.Instancia.FormaDePagamentos;
            comboFormaDePagamento.DisplayMemberPath = "Descricao";
            comboFormaDePagamento.SelectedValuePath = "Id";


            ListaVendas.ItemsSource = pedidos;
        }

        private void VerDetalhes_click(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as DataGrid;

            if (grid != null && grid.SelectedItem != null)
            {
                var pedido = (PedidoDTO)grid.SelectedItem;

                PedidoInfo pedidoInfo = new PedidoInfo(pedido.Id, true);
                pedidoInfo.Show();
                this.Close();
            }
        }

        private void novoPedido_Click(object sender, RoutedEventArgs e)
        {
            NovoPedido novoPedido = new NovoPedido(true);
            novoPedido.Show();
            this.Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { /* Filtro Status */ }

        private void txtBuscaCliente_KeyUp(object sender, KeyEventArgs e) { /* Filtro CPF */ }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            Home home = new Home();
            home.Show();
            this.Close();
        }

        private void Sair_Click(object sender, RoutedEventArgs e) { this.Close(); }

        private void comboStatusPedido_Loaded(object sender, RoutedEventArgs e) { /* Carregar status */ }

        private void ComboBoxPedido_SelectionChanged(object sender, SelectionChangedEventArgs e) { /* Mudar status */ }

        private void VerDetalhes_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            FrameworkElement elemento = e.OriginalSource as FrameworkElement;

            if (elemento != null)
            {
                // 2. O DataContext desse elemento contém o seu objeto de negócio (ex: Venda)
                // O WPF propaga o DataContext do ItemTemplate para todos os filhos.
                var pedido = elemento.DataContext;

                if (pedido is PedidoDTO pedidoDTO)
                {
                    PedidoInfo pedidoInfo = new PedidoInfo(pedidoDTO.Id, true);
                    pedidoInfo.Show();
                    this.Close();
                }
            }
        }

        private void ApagarPedido_Click(object sender, RoutedEventArgs e) { /* Deletar */ }

        private void dgVendas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Pesquisar_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<PedidoDTO> filtrados = pedidos;

            if (!string.IsNullOrWhiteSpace(txtBuscaCliente.Text))
            {
                string busca = txtBuscaCliente.Text.ToLower().Trim();
                filtrados = filtrados.Where(p =>
                    (p.NomeCliente != null && p.NomeCliente.ToLower().Contains(busca)) ||
                    (p.CpfCliente != null && p.CpfCliente.Contains(busca))
                );
            }

            if (comboStatus.SelectedItem != null)
            {
                dynamic statusSelecionado = comboStatus.SelectedItem;
                string nomeStatus = statusSelecionado.nome;

                if (nomeStatus != "Todos" && nomeStatus != "Não se aplica")
                {
                    int idStatus = (int)comboStatus.SelectedValue;
                    filtrados = filtrados.Where(p => p.IdStatus == idStatus);
                }
            }

            if (comboFormaDePagamento.SelectedItem != null)
            {
                dynamic pgtoSelecionado = comboFormaDePagamento.SelectedItem;
                string descPgto = pgtoSelecionado.Descricao;

                if (descPgto != "Todos")
                {
                    int idPagto = (int)comboFormaDePagamento.SelectedValue;
                    filtrados = filtrados.Where(p => p.IdFormaPagamento == idPagto);
                }
            }

            if (dtInicio.SelectedDate.HasValue)
                filtrados = filtrados.Where(p => p.DataCriacao.Date >= dtInicio.SelectedDate.Value.Date);

            if (dtFim.SelectedDate.HasValue)
                filtrados = filtrados.Where(p => p.DataCriacao.Date <= dtFim.SelectedDate.Value.Date);

            ListaVendas.ItemsSource = filtrados.ToList();
        }
    }
}