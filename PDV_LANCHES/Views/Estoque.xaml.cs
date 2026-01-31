using PDV_LANCHES.controller;
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

        public Estoque()
        {
            InitializeComponent();
            _controller = new EstoqueController();
            novoPedidoController = new NovoPedidoController();
            CarregarDados();
        }

        private async void CarregarDados()
        {
            try
            {
                // Busca os dados da API através do Controller
                List<MovimentacaoEstoqueDTO> dados = await _controller.PegarTodoEstoqueMovimentacoes();

                if (dados != null)
                {
                    // Como seu DTO contém tanto a movimentação quanto o saldo (QuantidadeDepois),
                    // você pode usar a mesma lista ou filtrar para as tabelas.

                    // Tabela de Movimentações (Histórico)
                    dgMovimentacoes.ItemsSource = dados;

                    // Para a tabela de "Saldo Atual", geralmente mostramos apenas o último estado de cada produto.
                    // Aqui estou vinculando a lista completa para fins de visualização das colunas.
                    var saldoAtual = dados
                        .GroupBy(m => m.Produto)
                        .Select(g => g.OrderByDescending(m => m.DataMovimentacao).First())
                        .ToList();

                    dgEstoque.ItemsSource = saldoAtual;
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

        private async void LancarCompra_Click(object sender, RoutedEventArgs e)
        {
            PainelLancamento.Visibility = Visibility.Visible;

            // Carregar produtos no ComboBox
            var produtos = await novoPedidoController.getAllProdutoAtivos();
            cbProdutos.ItemsSource = produtos;
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