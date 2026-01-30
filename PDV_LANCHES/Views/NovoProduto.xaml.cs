using Microsoft.Win32;
using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PDV_LANCHES.Views.ViewsAdministrativo
{
    public partial class NovoProduto : Window
    {
        private string caminhoImagemSelecionada = "";
        private Produto atualizar;
        private HomeAdministrativoController homeAdmController = new HomeAdministrativoController();
        private HomeController homeController = new HomeController();

        public NovoProduto()
        {
            InitializeComponent();
            CarregarCategorias();
        }
        public NovoProduto(Produto atualizar)
        {
            this.atualizar = atualizar;
            InitializeComponent();
            CarregarCategorias();
            CarregarProduto();
        }

        private async void CarregarCategorias()
        {
            await Status_Categorias.Instancia.CarregarAsync();
            var lista = Status_Categorias.Instancia.CategoriaProdutos;

            if (lista != null)
            {
                comboCategoria.ItemsSource = lista.Where(c => c.nome.ToLower() != "todos").ToList();
            }
        }

        private async void CarregarProduto()
        {

            txtNome.Text = atualizar.Nome;

            comboCategoria.SelectedValue = atualizar.IdCategoria;

            txtValor.Text = atualizar.Valor.ToString();

            chkDisponivel.IsChecked = atualizar.Disponivel;

            caminhoImagemSelecionada = atualizar.pathImg;

            if (!string.IsNullOrEmpty(atualizar.pathImg))
            {
                try
                {
                    imgPreview.Source = new BitmapImage(new Uri(atualizar.pathImg, UriKind.RelativeOrAbsolute));
                    pnlPlaceholder.Visibility = Visibility.Collapsed;
                }
                catch { /* Caso o caminho seja inválido, mantém o placeholder */ }
            }
        }

        private void SelecionarImagem_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenFileDialog buscarArquivo = new OpenFileDialog();
            buscarArquivo.Filter = "Imagens (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
            buscarArquivo.Title = "Selecione a foto do produto";

            if (buscarArquivo.ShowDialog() == true)
            {
                caminhoImagemSelecionada = buscarArquivo.FileName;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(caminhoImagemSelecionada);
                bitmap.EndInit();

                imgPreview.Source = bitmap;
                pnlPlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        private async void Salvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Produto produtoAProcessar = atualizar ?? new Produto();

                if (string.IsNullOrWhiteSpace(txtNome.Text))
                {
                    MessageBox.Show("Digite o nome do produto!");
                    return;
                }
                if (comboCategoria.SelectedValue == null)
                {
                    MessageBox.Show("Selecione uma categoria!");
                    return;
                }

                produtoAProcessar.Nome = txtNome.Text;
                produtoAProcessar.IdCategoria = (int)comboCategoria.SelectedValue;
                decimal.TryParse(txtValor.Text, out decimal valorConvertido);
                produtoAProcessar.Valor = valorConvertido;
                produtoAProcessar.Disponivel = chkDisponivel.IsChecked ?? false;
                produtoAProcessar.pathImg = caminhoImagemSelecionada;
                int quantidadeInicial = 0;
                int.TryParse(txtQuantidade.Text, out quantidadeInicial);
                produtoAProcessar.quantidade = quantidadeInicial;

                bool resultado;
                if (atualizar == null)
                    resultado = await homeAdmController.addProduto(produtoAProcessar);
                else
                    resultado = await homeAdmController.updateProduto(produtoAProcessar);

                if (resultado)
                {
                    MessageBox.Show("Operação realizada com sucesso!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Falha ao salvar dados no banco.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro técnico: " + ex.Message);
            }
        }

        private void Fechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}