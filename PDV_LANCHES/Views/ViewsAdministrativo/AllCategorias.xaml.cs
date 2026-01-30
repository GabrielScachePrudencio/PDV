using System.Windows;
using System.Windows.Controls;
using PDV_LANCHES.model;
using System.Collections.ObjectModel;
using PDV_LANCHES.controller;

namespace PDV_LANCHES.Views.ViewsAdministrativo
{
    public partial class AllCategorias : UserControl
    {
        public ObservableCollection<CategoriaProduto> Categorias { get; set; }
        private HomeAdministrativoController HomeAdministrativoController = new HomeAdministrativoController();
        public AllCategorias()
        {
            InitializeComponent(); // Este método vincula o XAML ao C# e resolve o erro do "dgCategorias"
            CarregarCategorias();
        }

        private async void CarregarCategorias()
        {
            await Status_Categorias.Instancia.CarregarAsync();
            Categorias = new ObservableCollection<CategoriaProduto>(
                Status_Categorias.Instancia.CategoriaProdutos
            );

            dgCategorias.ItemsSource = Categorias;
            dgCategorias.ItemsSource = Categorias;
        }
        private async void AlternarStatus_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var categoriaOriginal = btn?.DataContext as CategoriaProduto;

            if (categoriaOriginal != null)
            {
                // 1. Inverte o status localmente para enviar o novo valor
                bool novoStatus = !categoriaOriginal.ativo;

                // 2. Cria o objeto de atualização com o ID correto e o NOVO status
                var categoriaAtualizada = new CategoriaProduto
                {
                    id = categoriaOriginal.id, // IMPORTANTE: sem o ID a controller falha
                    nome = categoriaOriginal.nome,
                    ativo = novoStatus
                };

                // 3. Tenta atualizar no Banco/API
                if (await HomeAdministrativoController.UpdateCategoria(categoriaAtualizada.id, categoriaAtualizada))
                {
                    // 4. Se deu certo no banco, atualiza a lista na memória
                    categoriaOriginal.ativo = novoStatus;

                    // 5. Força o DataGrid a redesenhar as cores (já que você usa List)
                    dgCategorias.Items.Refresh();

                    // Opcional: remover o MessageBox para não irritar o usuário em cada clique
                    // MessageBox.Show("Status atualizado!");
                }
                else
                {
                    MessageBox.Show("Erro ao atualizar o status no servidor.");
                }
            }
        }

        private async void NovaCategoria_Click(object sender, RoutedEventArgs e)
        {
            // 1. Validação básica
            if (string.IsNullOrWhiteSpace(txtNomeCategoria.Text))
            {
                MessageBox.Show("Digite um nome para a categoria.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Criar o objeto conforme sua model CategoriaProduto
            var nova = new CategoriaProduto
            {
                nome = txtNomeCategoria.Text,
                ativo = true 
            };

            if (await HomeAdministrativoController.AddCategoria(nova))
            {
                Status_Categorias.Instancia.CategoriaProdutos.Add(nova);
                txtNomeCategoria.Clear();
                MessageBox.Show("Categoria adicionada com sucesso!");
                Categorias.Add(nova);
            }
            else
            {
                MessageBox.Show("erro ao add Categoria");
                
            }
            
        }
    }
}