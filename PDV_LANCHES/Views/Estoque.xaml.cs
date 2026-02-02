using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using PDV_LANCHES.model.dto;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PDV_LANCHES.Views
{
    public partial class Estoque : Window
    {
        private readonly EstoqueController _controller;
        private readonly NovoPedidoController novoPedidoController;
        private readonly List<PDV_LANCHES.model.Estoque> estoque;
        public Estoque()
        {
            InitializeComponent();
            _controller = new EstoqueController();
            estoque = new List<PDV_LANCHES.model.Estoque>();
            novoPedidoController = new NovoPedidoController();
            CarregarDados();
        } 

        private async void CarregarDados()
        {
            try
            {
    
                // Busca os dados da API através do Controller
                List<MovimentacaoEstoqueDTO> dados = await _controller.PegarTodoEstoqueMovimentacoes();
                List<PDV_LANCHES.model.Estoque> estoques = await _controller.allEstoque();

                if (dados != null)
                {
                    dgMovimentacoes.ItemsSource = dados;


                    dgEstoque.ItemsSource = estoques;

                    AtualizarResumoEstoque(estoques, dados);
                }
                else
                {
                    MessageBox.Show("Não foi possível carregar os dados do estoque.", "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro técnico: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void AtualizarResumoEstoque(
                List<PDV_LANCHES.model.Estoque> estoques,
                List<MovimentacaoEstoqueDTO> movimentacoes)
        {
            if (estoques == null || estoques.Count == 0)
            {
                txtTotalEstoque.Text = "0";
                txtQtdProdutos.Text = "0";
                txtEstoqueBaixo.Text = "0";
                txtEntradasHoje.Text = "0";
                txtSaidasHoje.Text = "0";
                txtUltimaMovimentacao.Text = "-";
                return;
            }

            // TOTAL DE ITENS
            txtTotalEstoque.Text = estoques.Sum(e => e.Quantidade).ToString();

            // TOTAL DE PRODUTOS
            txtQtdProdutos.Text = estoques.Count.ToString();

            // ESTOQUE BAIXO (ex: <= 5)
            txtEstoqueBaixo.Text = estoques.Count(e => e.Quantidade <= 5).ToString();

            // MOVIMENTAÇÕES DE HOJE
            var hoje = DateTime.Today;

            var movHoje = movimentacoes
                .Where(m => m.DataMovimentacao.Date == hoje)
                .ToList();

            txtEntradasHoje.Text = movHoje
                .Where(m => m.Tipo == TipoMovimentacaoEstoque.ENTRADA)
                .Sum(m => m.QuantidadeMovimentada)
                .ToString();

            txtSaidasHoje.Text = movHoje
                .Where(m => m.Tipo == TipoMovimentacaoEstoque.SAIDA)
                .Sum(m => m.QuantidadeMovimentada)
                .ToString();

            // ÚLTIMA MOVIMENTAÇÃO
            var ultima = movimentacoes
                .OrderByDescending(m => m.DataMovimentacao)
                .FirstOrDefault();

            txtUltimaMovimentacao.Text = ultima != null
                ? ultima.DataMovimentacao.ToString("dd/MM HH:mm")
                : "-";
        }




        private async void LancarCompra_Click(object sender, RoutedEventArgs e)
        {
            PainelLancamento.Visibility = Visibility.Visible;

            // Carregar produtos no ComboBox
            var produtos = await novoPedidoController.getAllProdutoAtivos();
            cbProdutos.ItemsSource = produtos;
            CarregarDados();
        }

        private async void Atualizar_Click(object sender, RoutedEventArgs e)
        {
            if (cbProdutos.SelectedValue == null || !int.TryParse(txtQuantidade.Text, out int quantidade))
            {
                MessageBox.Show("Selecione um produto e informe a quantidade.", "Atenção");
                return;
            }

            int idProduto = (int)cbProdutos.SelectedValue;

            bool sucesso = await _controller.AtualizarEstoque(idProduto, quantidade);

            if (sucesso)
            {
                MessageBox.Show("Estoque atualizado com sucesso!");
                PainelLancamento.Visibility = Visibility.Collapsed;
                CarregarDados();
            }
            else
            {
                MessageBox.Show("Erro ao atualizar estoque.");
            }
        }


        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            PainelLancamento.Visibility = Visibility.Collapsed;
            cbProdutos.SelectedIndex = -1;
            txtQuantidade.Clear();
        }


        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            Home home = new Home(); 
            home.Show();
            this.Close();
        }   
    }
}