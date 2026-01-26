using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PDV_LANCHES.Views.ViewsAdministrativo
{
    /// <summary>
    /// Lógica interna para AllFormasDePagamentos.xaml
    /// </summary>
    public partial class AllFormasDePagamentos : UserControl
    {
        public List<FormaDePagamento> pgmt { get; set; }
        private HomeAdministrativoController HomeAdministrativoController = new HomeAdministrativoController();
        public AllFormasDePagamentos()
        {
            InitializeComponent(); 
            Carregar();
        }

        private async void Carregar()
        {
            await Status_Categorias.Instancia.CarregarAsync();
            pgmt = Status_Categorias.Instancia.FormaDePagamentos;
            dgCategorias.ItemsSource = pgmt;
        }
        private async void AlternarStatus_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var FormaOriginal = btn?.DataContext as FormaDePagamento;

            if (FormaOriginal != null)
            {
                bool novoStatus = !FormaOriginal.Ativo;

                var formaAtualizada = new FormaDePagamento
                {
                    Id = FormaOriginal.Id, 
                    Descricao = FormaOriginal.Descricao,
                    Ativo = novoStatus
                };

                if (await HomeAdministrativoController.UpdateFormaPagamento(formaAtualizada.Id, formaAtualizada))
                {
                    FormaOriginal.Ativo = novoStatus;

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
            // 1. Validação básica
            if (string.IsNullOrWhiteSpace(txtNomeCategoria.Text))
            {
                MessageBox.Show("coloque um nome para a nova forma de pagamento");
                return;
            }

            // 2. Criar o objeto conforme sua model CategoriaProduto
            var nova = new FormaDePagamento
            {
                Descricao = txtNomeCategoria.Text,
                Ativo = true
            };

            if (await HomeAdministrativoController.AddFormaPagamento(nova))
            {
                Status_Categorias.Instancia.FormaDePagamentos.Add(nova);

                dgCategorias.Items.Refresh();

                txtNomeCategoria.Clear();
                MessageBox.Show("Forma de pagamento adicionada com sucesso!");
            }
            else
            {
                MessageBox.Show("erro ao add forma de pagamentos");

            }

        }
    }
}
