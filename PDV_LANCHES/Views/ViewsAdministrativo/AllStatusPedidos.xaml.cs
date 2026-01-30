using System.Windows;
using System.Windows.Controls;
using PDV_LANCHES.model;
using System.Collections.ObjectModel;
using PDV_LANCHES.controller;

namespace PDV_LANCHES.Views.ViewsAdministrativo
{
    public partial class AllStatusPedidos : UserControl
    {
        public ObservableCollection<TipoStatusPedido> tipoStatusPedido { get; set; }
        private HomeAdministrativoController HomeAdministrativoController = new HomeAdministrativoController();
        public AllStatusPedidos()
        {
            InitializeComponent(); 
            CarregarCategorias();
        }

        private async void CarregarCategorias()
        {
            await Status_Categorias.Instancia.CarregarAsync();
            tipoStatusPedido = new ObservableCollection<TipoStatusPedido>(
                    Status_Categorias.Instancia.TipoStatusPedido
                );
            dgCategorias.ItemsSource = tipoStatusPedido;
        }
        private async void AlternarStatus_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var categoriaOriginal = btn?.DataContext as TipoStatusPedido;

            if (categoriaOriginal != null)
            {
                bool novoStatus = !categoriaOriginal.ativo;

                var categoriaAtualizada = new TipoStatusPedido
                {
                    id = categoriaOriginal.id, 
                    nome = categoriaOriginal.nome,
                    ativo = novoStatus
                };

                if (categoriaAtualizada.id == 0) MessageBox.Show("erro ta nulo");

                if (await HomeAdministrativoController.UpdateStatusPedido(categoriaAtualizada.id, categoriaAtualizada))
                {
                    categoriaOriginal.ativo = novoStatus;

                    dgCategorias.Items.Refresh();

                }
                else
                {
                    MessageBox.Show("Erro ao atualizar o status no servidor.");
                }
            }
        }

        private async void NovaCategoria_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNomeCategoria.Text))
            {
                MessageBox.Show("Digite um nome para a categoria.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nova = new TipoStatusPedido
            {
                nome = txtNomeCategoria.Text,
                ativo = true
            };

            if (await HomeAdministrativoController.AddStatusPedido(nova))
            {
                Status_Categorias.Instancia.TipoStatusPedido.Add(nova);
                txtNomeCategoria.Clear();
                MessageBox.Show("Categoria adicionada com sucesso!");
                tipoStatusPedido.Add(nova);
            }
            else
            {
                MessageBox.Show("erro ao add Categoria");

            }

        }
    }
}