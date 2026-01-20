using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using ServidorLanches.model.dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PDV_LANCHES.Views
{
    public partial class NovoPedido : Window
    {
        private NovoPedidoController controller;
        private PedidoDTO pedido;
        private ObservableCollection<Cardapio> cardapios = new ObservableCollection<Cardapio>();
        private ObservableCollection<ItemPedidoCardapioDTO> itensPedido = new ObservableCollection<ItemPedidoCardapioDTO>();
        private EtapaPedido etapaAtual = EtapaPedido.InformarCpf;

        public NovoPedido()
        {
            InitializeComponent();
            controller = new NovoPedidoController();
            pedido = new PedidoDTO();
            CarregarCardapioAoIniciar();
        }

        private async void CarregarCardapioAoIniciar()
        {
            var lista = await controller.getAllCardapio();
            if (lista != null)
            {
                foreach (var item in lista)

                {

                    if (!string.IsNullOrEmpty(item.pathImg))
                    {
                        try
                        {
                            if (!item.pathImg.StartsWith("file:///"))
                            {
                                item.pathImg = new Uri(item.pathImg, UriKind.Absolute).AbsoluteUri;
                            }
                        }
                        catch

                        {
                            item.pathImg = null;
                        }
                    }
                    item.QuantidadeSelecionada = 1;
                    cardapios.Add(item);
                }
                ListaCardapioItems.ItemsSource = cardapios;
            }
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (etapaAtual)
            {
                case EtapaPedido.InformarCpf:
                    await ValidarCpfEAvancar();
                    break;

                case EtapaPedido.SelecionarItens:
                    IrParaResumo();
                    break;

                case EtapaPedido.ConfirmarPedido:
                    await FinalizarVenda();
                    break;
            }
        }

        private async Task ValidarCpfEAvancar()
        {
            if (string.IsNullOrWhiteSpace(inputCpfCliente.Text))
            {
                MessageBox.Show("Por favor, informe o CPF do cliente.");
                return;
            }

            var usuario = await controller.pegarUsuarioLogado();

            if (usuario == null) {
                fecharAquiEAbrirHome();
                return;
            }


            pedido.IdUsuario = usuario.Id;
            pedido.CpfCliente = inputCpfCliente.Text;
            pedido.DataCriacao = DateTime.Now;
            pedido.StatusPedido = StatusPedido.Pendente;

            inputCpfCliente.IsEnabled = false;
            LabelCPF.Text = $"✅ Cliente: {pedido.CpfCliente}";

            buttonProximo.Content = "VER RESUMO";
            etapaAtual = EtapaPedido.SelecionarItens;
        }

        
        private void IrParaResumo()
        {
            if (!itensPedido.Any())
            {
                MessageBox.Show("Adicione itens ao carrinho primeiro!");
                return;
            }

            scrollCardapio.Visibility = Visibility.Collapsed;
            borderResumo.Visibility = Visibility.Visible;
            stackResumoItens.Children.Clear();

            // Título do Resumo
            stackResumoItens.Children.Add(new TextBlock { Text = "Resumo do Pedido", FontSize = 20, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            foreach (var item in itensPedido)
            {
                var nomeProd = cardapios.First(c => c.Id == item.IdCardapio).Nome;
                var linha = new TextBlock
                {
                    Text = $"• {item.Quantidade}x {nomeProd} - R$ {(item.Quantidade * item.ValorUnitario):F2}",
                    FontSize = 14,
                    Margin = new Thickness(0, 5, 0, 5)
                };
                stackResumoItens.Children.Add(linha);
            }

            buttonProximo.Content = "FINALIZAR E IMPRIMIR";
            etapaAtual = EtapaPedido.ConfirmarPedido;
        }

        private async Task FinalizarVenda()
        {
            pedido.Itens = itensPedido.ToList();

            if (pedido.Itens.Count == 0)
            {
                MessageBox.Show("O pedido não possui itens para ser enviado!");
                return;
            }

            pedido.ValorTotal = pedido.Itens.Sum(i => i.ValorUnitario * i.Quantidade);

            var sucesso = await controller.criarPedido(pedido);

            if (sucesso)
            {
                MessageBox.Show("Pedido realizado com sucesso!");
                new Home().Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Erro ao adicionar pedido! Detalhes: ");
            }
        }

        private void AdicionarItem_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var produto = btn.Tag as Cardapio;

            if (produto == null) return;

            int qtd = produto.QuantidadeSelecionada > 0 ? produto.QuantidadeSelecionada : 1;

            var existente = itensPedido.FirstOrDefault(i => i.IdCardapio == produto.Id);
            if (existente != null)
            {
                existente.Quantidade += qtd;
            }
            else
            {
                itensPedido.Add(new ItemPedidoCardapioDTO
                {
                    IdCardapio = produto.Id,
                    NomeCardapio = produto.Nome,
                    Categoria = produto.Categoria,
                    pahCardapioImg = !string.IsNullOrEmpty(produto.pathImg) ? produto.pathImg : "default.png",
                    ValorUnitario = produto.Valor,
                    Quantidade = qtd
                });
            }

            AtualizarTotal();

            // Feedback visual opcional
            // MessageBox.Show($"{qtd}x {produto.Nome} adicionado!"); 
        }

        private void AtualizarTotal()
        {
            pedido.ValorTotal = itensPedido.Sum(i => i.ValorUnitario * i.Quantidade);

            txtTotalDisplay.Text = $"R$ {pedido.ValorTotal:F2}";
        }


        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            switch (etapaAtual)
            {
                case EtapaPedido.InformarCpf:
                    fecharAquiEAbrirHome();
                    break;

                case EtapaPedido.SelecionarItens:
                    etapaAtual = EtapaPedido.InformarCpf;
                    inputCpfCliente.IsEnabled = true;
                    LabelCPF.Text = "";
                    buttonProximo.Content = "PRÓXIMO";
                    break;

                case EtapaPedido.ConfirmarPedido:
                    etapaAtual = EtapaPedido.SelecionarItens;
                    borderResumo.Visibility = Visibility.Collapsed;
                    scrollCardapio.Visibility = Visibility.Visible;
                    buttonProximo.Content = "VER RESUMO";
                    break;
            }
        }

        private void fecharAquiEAbrirHome()
        {
            Home home = new Home();
            home.Show();
            this.Close();
        }
    }
}