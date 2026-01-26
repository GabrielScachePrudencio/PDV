using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using ServidorLanches.model;
using ServidorLanches.model.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PDV_LANCHES.Views.ViewsAdministrativo
{
    public partial class AllRelatorio : UserControl
    {
        private HomeController homeController = new HomeController();
        private List<PedidoDTO> todosPedidos = new List<PedidoDTO>();

        public AllRelatorio()
        {
            InitializeComponent();
            this.Loaded += AllRelatorio_Loaded;
        }

        private async void AllRelatorio_Loaded(object sender, RoutedEventArgs e)
        {
            await CarregarDados();
        }

        private async Task CarregarDados()
        {
            try
            {
                await Status_Categorias.Instancia.CarregarAsync();
                var lista = await homeController.PegarTodosPedidos();

                if (lista != null)
                {
                    todosPedidos = lista.ToList();
                    dgRelatorios.ItemsSource = todosPedidos;
                    ConfigurarFiltros();
                    AtualizarCards(todosPedidos);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}");
            }
        }

        private void ConfigurarFiltros()
        {
            // Status do Banco (1-6 Status Reais, 7-Todos, 8-N/A)
            var listaStatus = Status_Categorias.Instancia.TipoStatusPedido.ToList();
            comboStatus.ItemsSource = listaStatus;
            // Seleciona o item "Todos" (ID 7) por padrão se ele existir na lista
            comboStatus.SelectedValue = 7;

            // Pagamento do Banco (1-5 Tipos Reais, 6-Todos)
            var listaPagto = Status_Categorias.Instancia.FormaDePagamentos.ToList();
            comboFormaDePagamento.ItemsSource = listaPagto;
            // Seleciona o item "Todos" (ID 6) por padrão
            comboFormaDePagamento.SelectedValue = 6;
        }

        private void AtualizarCards(List<PedidoDTO> lista)
        {
            if (lista == null) return;

            // 6 é o ID de Cancelado no seu banco pelo que entendi da lista
            decimal totalVendido = lista.Where(p => p.IdStatus != 6).Sum(p => p.ValorTotal);
            decimal totalCancelado = lista.Where(p => p.IdStatus == 6).Sum(p => p.ValorTotal);

            lblTotalVendido.Text = totalVendido.ToString("C2");
            lblTotalPedidos.Text = lista.Count.ToString();
            lblTotalCancelado.Text = totalCancelado.ToString("C2");
        }

        private void Pesquisar_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<PedidoDTO> filtrados = todosPedidos;

            // 1. Filtro Texto: Cliente ou CPF
            if (!string.IsNullOrWhiteSpace(txtBuscaCliente.Text))
            {
                string busca = txtBuscaCliente.Text.ToLower();
                filtrados = filtrados.Where(p =>
                    (p.NomeCliente != null && p.NomeCliente.ToLower().Contains(busca)) ||
                    (p.CpfCliente != null && p.CpfCliente.Contains(busca)));
            }

            // 2. Filtro Texto: Vendedor
            if (!string.IsNullOrWhiteSpace(txtBuscaVendedor.Text))
            {
                string busca = txtBuscaVendedor.Text.ToLower();
                filtrados = filtrados.Where(p => p.NomeUsuario != null && p.NomeUsuario.ToLower().Contains(busca));
            }

            // 3. Filtro: Datas
            if (dtInicio.SelectedDate.HasValue)
                filtrados = filtrados.Where(p => p.DataCriacao.Date >= dtInicio.SelectedDate.Value.Date);

            if (dtFim.SelectedDate.HasValue)
                filtrados = filtrados.Where(p => p.DataCriacao.Date <= dtFim.SelectedDate.Value.Date);

            // 4. Filtro: Status (ID 7 é "Todos", ID 8 é "N/A")
            if (comboStatus.SelectedValue != null)
            {
                int idStatus = Convert.ToInt32(comboStatus.SelectedValue);
                // Se NÃO for "Todos" e NÃO for "Não se aplica", filtramos pelo ID
                if (idStatus != 7 && idStatus != 8)
                {
                    filtrados = filtrados.Where(p => p.IdStatus == idStatus);
                }
            }

            // 5. Filtro: Pagamento (ID 6 é "Todos")
            if (comboFormaDePagamento.SelectedValue != null)
            {
                int idPgto = Convert.ToInt32(comboFormaDePagamento.SelectedValue);
                // Se NÃO for "Todos", filtramos pelo ID selecionado
                if (idPgto != 6)
                {
                    filtrados = filtrados.Where(p => p.IdFormaPagamento == idPgto);
                }
            }

            var listaFinal = filtrados.ToList();
            dgRelatorios.ItemsSource = listaFinal;
            AtualizarCards(listaFinal);
        }

        private void LimparFiltros_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaCliente.Clear();
            txtBuscaVendedor.Clear();
            dtInicio.SelectedDate = null;
            dtFim.SelectedDate = null;
            comboStatus.SelectedValue = 7; // Volta para "Todos"
            comboFormaDePagamento.SelectedValue = 6; // Volta para "Todos"

            dgRelatorios.ItemsSource = todosPedidos;
            AtualizarCards(todosPedidos);
        }

        private void VerDetalhes_Click(object sender, MouseButtonEventArgs e)
        {
            if (dgRelatorios.SelectedItem is PedidoDTO pedido)
            {
                PedidoInfo info = new PedidoInfo(pedido.Id, true);
                info.ShowDialog();
            }
        }

        private void Imprimir_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new PrintDialog();
            if (printDlg.ShowDialog() == true)
            {
                printDlg.PrintVisual(dgRelatorios, "Relatório de Vendas");
            }
        }


    }
}