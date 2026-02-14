using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using ServidorLanches.model.dto; // Certifique-se que o DTO de consignação está aqui
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PDV_LANCHES.Views
{
    public partial class TodasConsignacoes : Window
    {
        private ConsignacaoController consignacaoController = new ConsignacaoController();
        private HomeController homeController = new HomeController();
        private Usuario usuarioLogado;
        // Substitua 'ConsignacaoDTO' pelo nome real da sua classe de DTO de consignação
        private ObservableCollection<Consignacao> consignacoes = new ObservableCollection<Consignacao>();

        public TodasConsignacoes()
        {
            InitializeComponent();
            Loaded += TodasConsignacoes_Loaded;
        }

        private async void TodasConsignacoes_Loaded(object sender, RoutedEventArgs e)
        {
            // Carrega ambos em paralelo para melhor performance
            await Task.WhenAll(CarregarDadosUsuario(), CarregarDadosConsignacoes());
        }

        private async Task CarregarDadosUsuario()
        {
            var usuario = await homeController.pegarUsuarioLogado();
            if (usuario == null)
            {
                MessageBox.Show("Sessão expirada. Faça login novamente.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
                return;
            }
            usuarioLogado = usuario;
        }

        private async Task CarregarDadosConsignacoes()
        {
            var lista = await consignacaoController.GetAllConsignacao();

            if (lista == null)
            {
                MessageBox.Show("Erro ao carregar consignações.");
                return;
            }

            consignacoes.Clear();
            foreach (var item in lista)
            {
                consignacoes.Add(item);
            }

            ListaConsignacoes.ItemsSource = consignacoes;
        }


        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            Home home = new Home();
            home.Show();
            this.Close();
        }

        private void NovaConsignacao_Click(object sender, RoutedEventArgs e)
        {
            NovaConsignacao tela = new NovaConsignacao();
            tela.Show();
            this.Close();
        }

        private void Pesquisar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscaCliente.Text))
            {
                ListaConsignacoes.ItemsSource = consignacoes;
                return;
            }

            string busca = txtBuscaCliente.Text.ToLower().Trim();
            var filtrados = consignacoes.Where(c =>
                c.NomeCliente != null && c.NomeCliente.ToLower().Contains(busca) ||
                c.Id.ToString().Contains(busca)
            ).ToList();

            ListaConsignacoes.ItemsSource = filtrados;
        }


        private void RealizarAcerto_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var consignacao = btn.DataContext as Consignacao;

            if (consignacao != null)
            {
                ConsignacaoDarBaixa consignacaoDarBaixa = new ConsignacaoDarBaixa(consignacao.Id);
                consignacaoDarBaixa.Show();
                this.Close();
            }
        }

        private async void CancelarConsignacao_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var consignacao = btn.DataContext as Consignacao;

            if (consignacao != null)
            {
                var result = MessageBox.Show($"Deseja realmente cancelar a consignação #C{consignacao.Id}?",
                    "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // bool sucesso = await homeController.ExcluirConsignacao(consignacao.Id);
                    // if(sucesso) await CarregarDadosConsignacoes();
                }
            }
        }
    }
}