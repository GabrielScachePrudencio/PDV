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
        private HomeAdministrativoController homeAdministrativoController = new HomeAdministrativoController();

        public AllProduto()
        {
            InitializeComponent();
            CarregarCardapio();
        }

        private async void CarregarCardapio()
        {
            listaCardapio = await novoPedido.getAllProduto();

            await Status_Categorias.Instancia.CarregarAsync();
            var categorias = Status_Categorias.Instancia.CategoriaProdutos;

            foreach (var produto in listaCardapio)
            {
                var cat = categorias.FirstOrDefault(c => c.id == produto.IdCategoria);
                produto.NomeCategoria = cat != null ? cat.nome : "Sem Categoria";
            }

            dgCardapio.ItemsSource = null;
            dgCardapio.ItemsSource = listaCardapio;
        }

        private void Adicionar_Click(object sender, RoutedEventArgs e)
        {
            NovoProduto np = new NovoProduto();
            np.Show();
            
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as Produto;
            if (item == null) return;
            NovoProduto novoProduto = new NovoProduto(item);
            novoProduto.Closed += (s, args) =>
            {
                CarregarCardapio();
            };
            novoProduto.ShowDialog();

        }

        private async void Excluir_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as Produto;
            if (item == null) return;

            if (MessageBox.Show($"Excluir {item.Nome}?",
                                "Confirmação",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (await homeAdministrativoController.deletarProduto(item.Id)){
                    MessageBox.Show("produto excluido");
                }
                else { 
                
                    MessageBox.Show("erro ao excluir produto");
                }

                    listaCardapio.Remove(item);
            }
        }
    }
}
