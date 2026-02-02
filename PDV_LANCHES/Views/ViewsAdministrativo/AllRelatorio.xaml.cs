using PDV_LANCHES.controller;
using PDV_LANCHES.model;
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
        private List<PedidoDTO> todosPedidos = new();

        public AllRelatorio()
        {
            InitializeComponent();
            Loaded += AllRelatorio_Loaded;
        }

        private async void AllRelatorio_Loaded(object sender, RoutedEventArgs e)
        {
            await CarregarDados();
        }

        private async Task CarregarDados()
        {
            var lista = await homeController.PegarTodosPedidos();
            todosPedidos = lista.ToList();
            dgRelatorios.ItemsSource = todosPedidos;
            ConfigurarFiltros();
            AtualizarCards(todosPedidos);
        }

        private void ConfigurarFiltros()
        {
            comboStatus.ItemsSource = Status_Categorias.Instancia.TipoStatusPedido;
            comboStatus.SelectedValue = 7;

            comboFormaDePagamento.ItemsSource = Status_Categorias.Instancia.FormaDePagamentos;
            comboFormaDePagamento.SelectedValue = 6;
        }

        private void AtualizarCards(List<PedidoDTO> lista)
        {
            if (lista == null || lista.Count == 0)
            {
                ZerarTudo();
                return;
            }

            const int PRONTO = 1;
            const int FINALIZADO = 2;
            const int CANCELADO = 3;
            const int ESTORNADO = 4;

            // --- CÁLCULOS FINANCEIROS ---
            var pedidosVendaSucesso = lista.Where(p => p.IdStatus == FINALIZADO).ToList();
            decimal faturadoGeral = pedidosVendaSucesso.Sum(p => p.ValorTotal);

            decimal custoTotal = pedidosVendaSucesso
                .Where(p => p.Itens != null)
                .Sum(p => p.Itens.Sum(i => i.CustoDeFabricacao * i.Quantidade));

            // --- CÁLCULOS POR STATUS (VALOR E QUANTIDADE) ---
            var listaProntos = lista.Where(p => p.IdStatus == PRONTO).ToList();
            var listaFinalizados = lista.Where(p => p.IdStatus == FINALIZADO).ToList();
            var listaCancelados = lista.Where(p => p.IdStatus == CANCELADO).ToList();
            var listaEstornados = lista.Where(p => p.IdStatus == ESTORNADO).ToList();

            // Dados de Hoje
            var listaHoje = pedidosVendaSucesso.Where(p => p.DataCriacao.Date == DateTime.Today).ToList();

            // --- ATUALIZAÇÃO DA INTERFACE (DESTAQUES) ---
            lblFaturadoGeral.Text = faturadoGeral.ToString("C2");
            lblLucroGeral.Text = (faturadoGeral - custoTotal).ToString("C2");
            lblTotalPedidos.Text = lista.Count.ToString();

            // --- ATUALIZAÇÃO DOS STATUS (VALOR + QUANTIDADE) ---

            // Cancelados
            lblTotalCancelado.Text = listaCancelados.Sum(p => p.ValorTotal).ToString("C2");
            lblQtdeCancelado.Text = $"({listaCancelados.Count})";

            // Prontos
            lblTotalProntos.Text = listaProntos.Sum(p => p.ValorTotal).ToString("C2");
            lblQtdeProntos.Text = $"({listaProntos.Count})";

            // Finalizados
            lblTotalFinalizados.Text = listaFinalizados.Sum(p => p.ValorTotal).ToString("C2");
            lblQtdeFinalizados.Text = $"({listaFinalizados.Count})";

            // Estornados
            lblTotalEstornados.Text = listaEstornados.Sum(p => p.ValorTotal).ToString("C2");
            lblQtdeEstornados.Text = $"({listaEstornados.Count})";

            // Hoje
            lblTotalVendido.Text = listaHoje.Sum(p => p.ValorTotal).ToString("C2");
            lblVendasHoje.Text = $"({listaHoje.Count})";
        }

        private void ZerarTudo()
        {
            lblFaturadoGeral.Text = lblLucroGeral.Text = "R$ 0,00";
            lblTotalPedidos.Text = "0";

            lblTotalCancelado.Text = lblTotalProntos.Text = lblTotalFinalizados.Text = lblTotalEstornados.Text = lblTotalVendido.Text = "R$ 0,00";
            lblQtdeCancelado.Text = lblQtdeProntos.Text = lblQtdeFinalizados.Text = lblQtdeEstornados.Text = lblVendasHoje.Text = "(0)";
        }



        private void Pesquisar_Click(object sender, RoutedEventArgs e)
        {
            if (todosPedidos == null) return;

            var f = todosPedidos.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(txtBuscaCliente.Text))
            {
                string busca = txtBuscaCliente.Text.ToLower();
                f = f.Where(p => (p.NomeCliente?.ToLower().Contains(busca) == true)
                              || (p.CpfCliente?.Contains(busca) == true));
            }

            if (!string.IsNullOrWhiteSpace(txtBuscaVendedor.Text))
            {
                string buscaVendedor = txtBuscaVendedor.Text.ToLower();
                f = f.Where(p => p.NomeUsuario?.ToLower().Contains(buscaVendedor) == true);
            }

            if (dtInicio.SelectedDate.HasValue)
                f = f.Where(p => p.DataCriacao.Date >= dtInicio.SelectedDate.Value.Date);

            if (dtFim.SelectedDate.HasValue)
                f = f.Where(p => p.DataCriacao.Date <= dtFim.SelectedDate.Value.Date);

            if (comboStatus.SelectedValue != null)
            {
                int statusSelecionado = (int)comboStatus.SelectedValue;
                if (statusSelecionado != 7) 
                    f = f.Where(p => p.IdStatus == statusSelecionado);
            }

            if (comboFormaDePagamento.SelectedValue != null)
            {
                int pgtoSelecionado = (int)comboFormaDePagamento.SelectedValue;
                if (pgtoSelecionado != 6) 
                    f = f.Where(p => p.IdFormaPagamento == pgtoSelecionado);
            }

            var listaFiltrada = f.ToList();
            dgRelatorios.ItemsSource = listaFiltrada;
            AtualizarCards(listaFiltrada);
        }

        private void LimparFiltros_Click(object sender, RoutedEventArgs e)
        {
            txtBuscaCliente.Clear();
            txtBuscaVendedor.Clear();
            dtInicio.SelectedDate = null;
            dtFim.SelectedDate = null;
            comboStatus.SelectedValue = 7;
            comboFormaDePagamento.SelectedValue = 6;

            dgRelatorios.ItemsSource = todosPedidos;
            AtualizarCards(todosPedidos);
        }

        private void VerDetalhes_Click(object sender, MouseButtonEventArgs e)
        {
            if (dgRelatorios.SelectedItem is PedidoDTO p)
                new PedidoInfo(p.Id, false, true).ShowDialog();
        }

        private void Imprimir_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new PrintDialog();
            if (dlg.ShowDialog() == true)
                dlg.PrintVisual(dgRelatorios, "Relatório de Vendas");
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Pesquisar_Click(null, null);
            if (e.Key == Key.Escape) LimparFiltros_Click(null, null);
        }
    }
}
