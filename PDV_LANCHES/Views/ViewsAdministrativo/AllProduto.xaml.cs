using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PDV_LANCHES.Views.ViewsAdministrativo
{
    public partial class AllProduto : UserControl
    {
        private List<Produto> listaCardapio;
        private NovoPedidoController novoPedido = new NovoPedidoController();

        public AllProduto()
        {
            InitializeComponent();
            CarregarCardapio();
        }

        private async void CarregarCardapio()
        {
            listaCardapio = await novoPedido.getAllProduto();

            dgCardapio.ItemsSource = listaCardapio;
        }

        private void Adicionar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Abrir modal para adicionar item");
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as Produto;
            if (item == null) return;

            MessageBox.Show($"Editar: {item.Nome}");
        }

        private void Excluir_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as Produto;
            if (item == null) return;

            if (MessageBox.Show($"Excluir {item.Nome}?",
                                "Confirmação",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                listaCardapio.Remove(item);
            }
        }
    }
}
