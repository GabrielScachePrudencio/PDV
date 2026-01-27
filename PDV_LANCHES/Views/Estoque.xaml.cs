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

        public Estoque()
        {
            InitializeComponent();
            _controller = new EstoqueController();
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

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            Home home = new Home(); 
            home.Show();
            this.Close();
        }   
    }
}