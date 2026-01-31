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

namespace PDV_LANCHES.Views
{
    public partial class NovoPedido : Window
    {
        private NovoPedidoController controller;
        private PedidoDTO pedido;
        private ObservableCollection<Produto> cardapios = new ObservableCollection<Produto>();
        private ObservableCollection<ItemPedidoCardapioDTO> itensPedido = new ObservableCollection<ItemPedidoCardapioDTO>();
        private EtapaPedido etapaAtual = EtapaPedido.InformarCpf;
        private bool veioDeTodosOsPedidos = false;
        private bool darBaixaDepois = false;

        public NovoPedido()
        {
            InitializeComponent();
            controller = new NovoPedidoController();
            pedido = new PedidoDTO();
            CarregarCardapioAoIniciar();
        }

        public NovoPedido(bool veioDeTodosOsPedidos)
        {
            this.veioDeTodosOsPedidos = veioDeTodosOsPedidos;
            InitializeComponent();
            controller = new NovoPedidoController();
            pedido = new PedidoDTO();
            CarregarCardapioAoIniciar();
        }

        private async void CarregarCardapioAoIniciar()
        {
            var lista = await controller.getAllProdutoAtivos();
            if (lista != null)
            {
                foreach (var item in lista)
                {
                    item.QuantidadeSelecionada = 1;
                    cardapios.Add(item);
                }
                ListaCardapioItems.ItemsSource = cardapios;
                listCarrinho.ItemsSource = itensPedido;
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
            await Status_Categorias.Instancia.CarregarAsync();

            if (string.IsNullOrWhiteSpace(inputCpfCliente.Text) || inputCpfCliente.Text.Length < 14)
            {
                MessageBox.Show("Por favor, informe um CPF válido no formato 000.000.000-00.");
                return;
            }

            var usuario = await controller.pegarUsuarioLogado();
            if (usuario == null) { fecharAquiEAbrirHome(); return; }

            pedido.IdUsuario = usuario.Id;
            pedido.NomeUsuario = usuario.Nome;
            pedido.CpfCliente = inputCpfCliente.Text;
            pedido.NomeCliente = string.IsNullOrWhiteSpace(inputNomeCliente.Text) ? "Cliente" : inputNomeCliente.Text;
            pedido.DataCriacao = DateTime.Now;

            var statusPendente = Status_Categorias.Instancia.TipoStatusPedido.FirstOrDefault(s => s.nome == "Pendente");
            pedido.IdStatus = statusPendente?.id ?? 0;
            pedido.StatusPedido = statusPendente?.nome ?? "Pendente";

            inputCpfCliente.IsEnabled = false;
            inputNomeCliente.IsEnabled = false;
            LabelCPF.Text = $"✅ {pedido.NomeCliente}";

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

            // DESIGN: Esconde o cardápio e o carrinho lateral para mostrar o resumo gigante
            scrollCardapio.Visibility = Visibility.Collapsed;
            panelCarrinho.Visibility = Visibility.Collapsed;
            borderResumo.Visibility = Visibility.Visible;
            buttonSairSemDarBaixa.Visibility = Visibility.Visible;

            stackResumoItens.Children.Clear();
            stackResumoItens.Children.Add(new TextBlock { Text = "Resumo do Pedido", FontSize = 22, FontWeight = FontWeights.Bold, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 20) });

            foreach (var item in itensPedido)
            {
                var gridItem = new Grid { Margin = new Thickness(0, 5, 0, 5) };
                gridItem.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                gridItem.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var txtInfo = new TextBlock { Text = $"{item.Quantidade}x {item.NomeProduto}", FontSize = 15, Foreground = System.Windows.Media.Brushes.White };
                var txtPreco = new TextBlock { Text = $"R$ {(item.Quantidade * item.ValorUnitario):F2}", FontSize = 15, FontWeight = FontWeights.SemiBold, Foreground = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom("#60a5fa"), Margin = new Thickness(20, 0, 0, 0) };

                Grid.SetColumn(txtInfo, 0);
                Grid.SetColumn(txtPreco, 1);
                gridItem.Children.Add(txtInfo);
                gridItem.Children.Add(txtPreco);
                stackResumoItens.Children.Add(gridItem);
            }

            comboPagamento.ItemsSource = Status_Categorias.Instancia.FormaDePagamentos;
            if (comboPagamento.Items.Count > 0) comboPagamento.SelectedIndex = 0;

            buttonProximo.Content = "FINALIZAR E IMPRIMIR";
            etapaAtual = EtapaPedido.ConfirmarPedido;
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            if (etapaAtual == EtapaPedido.ConfirmarPedido)
            {
                // VOLTA PARA O CARDÁPIO: Restaura o cardápio E o carrinho lateral
                etapaAtual = EtapaPedido.SelecionarItens;
                borderResumo.Visibility = Visibility.Collapsed;
                buttonSairSemDarBaixa.Visibility = Visibility.Hidden;

                scrollCardapio.Visibility = Visibility.Visible;
                panelCarrinho.Visibility = Visibility.Visible; // <--- AQUI O CARRINHO VOLTA

                buttonProximo.Content = "VER RESUMO";
            }
            else if (etapaAtual == EtapaPedido.SelecionarItens)
            {
                etapaAtual = EtapaPedido.InformarCpf;
                inputCpfCliente.IsEnabled = true;
                inputNomeCliente.IsEnabled = true;
                LabelCPF.Text = "";
                buttonProximo.Content = "PRÓXIMO";
            }
            else fecharAquiEAbrirHome();
        }

        private async Task FinalizarVenda()
        {
            if (comboPagamento.SelectedValue == null)
            {
                MessageBox.Show("Selecione uma forma de pagamento!");
                return;
            }

            try
            {
                pedido.IdFormaPagamento = (int)comboPagamento.SelectedValue;
                var forma = comboPagamento.SelectedItem as FormaDePagamento;
                pedido.FormaPagamento = forma?.Descricao ?? "";
                pedido.Itens = itensPedido.ToList();
                pedido.ValorTotal = itensPedido.Sum(i => i.ValorUnitario * i.Quantidade);

                if (darBaixaDepois)
                {
                    pedido.IdStatus = 1;
                    pedido.StatusPedido = "Pronto";
                }
                else
                {
                    pedido.IdStatus = 2;
                    pedido.StatusPedido = "Finalizado";
                }

                var sucesso = await controller.criarPedido(pedido);
                if (sucesso)
                {
                    MessageBox.Show("Pedido realizado com sucesso!");
                    fecharAquiEAbrirHome();
                }
                else MessageBox.Show("Erro ao finalizar pedido!");
            }
            catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}"); }
        }

        private void AdicionarItem_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var produto = btn.Tag as Produto;
            if (produto == null) return;

            int qtd = produto.QuantidadeSelecionada > 0 ? produto.QuantidadeSelecionada : 1;
            var existente = itensPedido.FirstOrDefault(i => i.IdProduto == produto.Id);

            if (existente != null) existente.Quantidade += qtd;
            else
            {
                var categoriaObj = Status_Categorias.Instancia.CategoriaProdutos.FirstOrDefault(c => c.id == produto.IdCategoria);
                itensPedido.Add(new ItemPedidoCardapioDTO
                {
                    IdProduto = produto.Id,
                    NomeProduto = produto.Nome,
                    Categoria = categoriaObj?.nome ?? "Geral",
                    pathProdutoImg = produto.pathImg,
                    ValorUnitario = produto.Valor,
                    Quantidade = qtd
                });
            }
            AtualizarTotal();
        }

        private void RemoverItem_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var item = btn.Tag as ItemPedidoCardapioDTO;
            if (item != null) { itensPedido.Remove(item); AtualizarTotal(); }
        }

        private void AtualizarTotal()
        {
            pedido.ValorTotal = itensPedido.Sum(i => i.ValorUnitario * i.Quantidade);
            txtTotalDisplay.Text = $"R$ {pedido.ValorTotal:F2}";
        }

        private async void ButtonSairSemDarBaixa_Click(object sender, EventArgs e)
        {
            darBaixaDepois = true;
            await FinalizarVenda();
        }

        private void inputCpfCliente_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;
            string num = new string(textBox.Text.Where(char.IsDigit).ToArray());
            string fmt = "";
            if (num.Length > 0)
            {
                if (num.Length <= 3) fmt = num;
                else if (num.Length <= 6) fmt = $"{num.Substring(0, 3)}.{num.Substring(3)}";
                else if (num.Length <= 9) fmt = $"{num.Substring(0, 3)}.{num.Substring(3, 3)}.{num.Substring(6)}";
                else fmt = $"{num.Substring(0, 3)}.{num.Substring(3, 3)}.{num.Substring(6, 3)}-{num.Substring(9, Math.Min(2, num.Length - 9))}";
            }
            if (textBox.Text != fmt) { textBox.Text = fmt; textBox.CaretIndex = textBox.Text.Length; }
        }

        private void fecharAquiEAbrirHome()
        {
            if (veioDeTodosOsPedidos) new TodasVendasCompleto().Show();
            else new Home().Show();
            this.Close();
        }
    }
}