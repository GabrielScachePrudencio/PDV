using System.Collections.Generic;
using System.Windows;
using PDV_LANCHES.controller;
using PDV_LANCHES.model;

namespace PDV_LANCHES.Views
{
    public partial class VendasPorCaixa : Window
    {

        private List<Caixa> listaDeCaixas = new();
        private readonly HomeController homeController = new HomeController();

        public VendasPorCaixa()
        {
            InitializeComponent();
            CarregarTodosCaixas();
            // Carrega a lista que veio da Home
            //dgHistoricoCaixas.ItemsSource = listaDeCaixas;
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            Home home = new Home();
            home.Show();
            this.Close();
        }

        private async void CarregarTodosCaixas()
        {
            listaDeCaixas = await homeController.ObterTodosCaixas(); 

            if(listaDeCaixas == null || listaDeCaixas.Count == 0)
            {
                MessageBox.Show("Nenhum caixa finalizado encontrado.");
                return;
            }

            dgHistoricoCaixas.ItemsSource = listaDeCaixas;
        }

        private void VerVendas_Click(object sender, RoutedEventArgs e)
        {
            var caixaSelecionado = (Caixa)((FrameworkElement)sender).DataContext;

            if (caixaSelecionado != null)
            {
                TodosPedidosPorCaixa todosPedidosPorCaixa = new TodosPedidosPorCaixa(caixaSelecionado.id);
                todosPedidosPorCaixa.Show();
                this.Close();
            }
        }

        private void dgHistoricoCaixas_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}