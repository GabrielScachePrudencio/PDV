using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using ServidorLanches.model.dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PDV_LANCHES.Views
{
    public partial class NovaConsignacao : Window
    {
        private NovoPedidoController controller;
        private ConsignacaoController ConsignacaoController;
        private Consignacao consignacao;
        private ObservableCollection<Produto> produtos = new ObservableCollection<Produto>();
        private ObservableCollection<ConsignacaoItemDisplay> itensConsignacao = new ObservableCollection<ConsignacaoItemDisplay>();
        private bool cpfValidado = false;
        private Cliente clienteAtual;

        public NovaConsignacao()
        {
            InitializeComponent();
            controller = new NovoPedidoController();
            consignacao = new Consignacao();
            ConsignacaoController = new ConsignacaoController();
            CarregarProdutosAoIniciar();
            listCarrinho.ItemsSource = itensConsignacao;
        }

        private async void CarregarProdutosAoIniciar()
        {
            var lista = await controller.getAllProdutoAtivos();
            if (lista != null)
            {
                foreach (var item in lista)
                {
                    produtos.Add(item);
                }
                ListaProdutos.ItemsSource = produtos;
            }
        }

        private async void inputCpfCliente_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            // Formatação do CPF
            string num = new string(textBox.Text.Where(char.IsDigit).ToArray());
            string fmt = "";

            if (num.Length > 0)
            {
                if (num.Length <= 3)
                    fmt = num;
                else if (num.Length <= 6)
                    fmt = $"{num.Substring(0, 3)}.{num.Substring(3)}";
                else if (num.Length <= 9)
                    fmt = $"{num.Substring(0, 3)}.{num.Substring(3, 3)}.{num.Substring(6)}";
                else
                    fmt = $"{num.Substring(0, 3)}.{num.Substring(3, 3)}.{num.Substring(6, 3)}-{num.Substring(9, Math.Min(2, num.Length - 9))}";
            }

            if (textBox.Text != fmt)
            {
                textBox.Text = fmt;
                textBox.CaretIndex = textBox.Text.Length;
            }

            // Validação do CPF completo
            if (num.Length == 11)
            {
                await ValidarCliente();
            }
            else
            {
                cpfValidado = false;
                LabelStatusCliente.Text = "";
                btnCriarCliente.Visibility = Visibility.Hidden;

            }
        }
        private async Task ValidarCliente()
        {
            string cpfFormatado = inputCpfCliente.Text;
            string cpfNumerico = new string(cpfFormatado.Where(char.IsDigit).ToArray());

            if (cpfNumerico.Length != 11)
            {
                cpfValidado = false;
                StatusClienteBox.Visibility = Visibility.Collapsed;
                clienteAtual = null;
                LabelStatusCliente.Text = "";
                btnCriarCliente.Visibility = Visibility.Hidden;
                inputNomeCliente.Text = "";
                return;
            }

            var cliente = await ConsignacaoController.verificaCPFEXIste(cpfFormatado);

            var usuario = await controller.pegarUsuarioLogado();
            if (usuario == null)
            {
                fecharAquiEAbrirHome();
                return;
            }

            consignacao.IdUsuario = usuario.Id;
            consignacao.DataSaida = DateTime.Now;
            consignacao.IdStatus = 1;
            consignacao.NomeStatus = "Aberto";

            if (cliente != null)
            {
                clienteAtual = cliente; // 🔥 GUARDA O CLIENTE
                StatusClienteBox.Visibility = Visibility.Visible;
                StatusClienteBox.Background = new SolidColorBrush(Color.FromRgb(240, 253, 244)); // verde claro
                StatusClienteBox.BorderBrush = (Brush)FindResource("CorSucesso");
                inputNomeCliente.Text = cliente.Nome;
                LabelStatusCliente.Text = "Cliente encontrado com sucesso.";
                LabelStatusCliente.Foreground = (Brush)FindResource("CorSucesso");
                cpfValidado = true;
                btnCriarCliente.Visibility = Visibility.Collapsed;

            }
            else
            {
                clienteAtual = null;
                cpfValidado = false;
                StatusClienteBox.Visibility = Visibility.Visible;
                StatusClienteBox.Background = new SolidColorBrush(Color.FromRgb(254, 242, 242)); // vermelho claro
                StatusClienteBox.BorderBrush = (Brush)FindResource("CorErro");

                LabelStatusCliente.Text = "Cliente não encontrado.";
                LabelStatusCliente.Foreground = (Brush)FindResource("CorErro");

                btnCriarCliente.Visibility = Visibility.Visible;

            }
        }



        private void AdicionarItem_Click(object sender, RoutedEventArgs e)
        {
            if (!cpfValidado)
            {
                MessageBox.Show("Por favor, informe um CPF válido no formato 000.000.000-00 antes de adicionar itens.");
                return;
            }

            var btn = sender as Button;
            if (btn == null) return;

            // Pega a TextBox através do Tag
            var txtQtd = btn.Tag as TextBox;
            if (txtQtd == null) return;

            // Pega o produto do DataContext do Border pai
            var border = FindParent<Border>(btn);
            if (border == null) return;

            var produto = border.DataContext as Produto;
            if (produto == null) return;

            // Valida a quantidade
            if (!int.TryParse(txtQtd.Text, out int qtd) || qtd <= 0)
            {
                MessageBox.Show("Quantidade inválida!");
                return;
            }

            // Verifica se o item já existe no carrinho
            var existente = itensConsignacao.FirstOrDefault(i => i.IdProduto == produto.Id);

            if (existente != null)
            {
                existente.QuantidadeEnviada += qtd;
            }
            else
            {
                itensConsignacao.Add(new ConsignacaoItemDisplay
                {
                    IdProduto = produto.Id,
                    NomeProduto = produto.Nome,
                    QuantidadeEnviada = qtd,
                    PrecoUnitarioAcordado = produto.Valor,
                    QuantidadeVendida = 0,
                    QuantidadeDevolvida = 0
                });
            }

            // Reseta a quantidade na UI
            txtQtd.Text = "1";

            AtualizarTotal();
        }

        private async void btnCriarCliente_Click(object sender, RoutedEventArgs e)
        {
            string cpf = inputCpfCliente.Text;

            var telaCliente = new ClienteView(cpf); // passando CPF pelo construtor
            telaCliente.ShowDialog();

            // Depois que fechar, revalida o CPF
            await ValidarCliente();

        }



        private void RemoverItem_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var item = btn?.Tag as ConsignacaoItemDisplay;

            if (item != null)
            {
                itensConsignacao.Remove(item);
                AtualizarTotal();
            }
        }

        private void AtualizarTotal()
        {
            decimal total = itensConsignacao.Sum(i => i.PrecoUnitarioAcordado * i.QuantidadeEnviada);
            consignacao.ValorTotalEstimado = total;
            txtTotalDisplay.Text = $"R$ {total:F2}";
        }

        private async void btnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            if (!cpfValidado)
            {
                MessageBox.Show("Por favor, informe um CPF válido antes de finalizar.");
                return;
            }

            if (!itensConsignacao.Any())
            {
                MessageBox.Show("Adicione pelo menos um item à consignação.");
                return;
            }

            if (string.IsNullOrWhiteSpace(inputNomeCliente.Text))
            {
                MessageBox.Show("Por favor, informe o nome do responsável.");
                return;
            }

            try
            {
                if (clienteAtual == null)
                {
                    MessageBox.Show("Cliente não encontrado.");
                    return;
                }

                if (datePickerDataAcerto.SelectedDate.HasValue)
                {
                    consignacao.DataPrevisaoAcerto = datePickerDataAcerto.SelectedDate.Value;
                }
                else
                {
                    consignacao.DataPrevisaoAcerto = null; // ou DateTime.Now se quiser padrão
                }
                consignacao.IdCliente = clienteAtual.Id;
                consignacao.NomeCliente = clienteAtual.Nome;

                consignacao.Observacao = ".";
                consignacao.Itens = itensConsignacao.Select(i => new ConsignacaoItem
                {
                    IdProduto = i.IdProduto,
                    NomeProduto = i.NomeProduto,
                    QuantidadeEnviada = i.QuantidadeEnviada,
                    QuantidadeVendida = 0,
                    QuantidadeDevolvida = 0,
                    PrecoUnitarioAcordado = i.PrecoUnitarioAcordado
                }).ToList();

                var sucesso = await ConsignacaoController.CriarConsignacao(consignacao);
                
                if (sucesso == "ok")
                {
                    var detalhesItens = string.Join("\n", itensConsignacao.Select(i =>
                        $"  • {i.QuantidadeEnviada}x {i.NomeProduto} - R$ {i.SubTotal:F2}"));

                    MessageBox.Show($"✅ CONSIGNAÇÃO REGISTRADA COM SUCESSO!\n\n" +
                                   $"═══════════════════════════════\n" +
                                   $"CLIENTE: {consignacao.NomeCliente}\n" +
                                   $"CPF: {inputCpfCliente.Text}\n" +
                                   $"═══════════════════════════════\n\n" +
                                   $"ITENS CONSIGNADOS:\n{detalhesItens}\n\n" +
                                   $"═══════════════════════════════\n" +
                                   $"VALOR TOTAL ESTIMADO: R$ {consignacao.ValorTotalEstimado:F2}\n" +
                                   $"DATA DE SAÍDA: {consignacao.DataSaida:dd/MM/yyyy HH:mm}\n" +
                                   $"STATUS: {consignacao.NomeStatus}\n" +
                                   $"═══════════════════════════════",
                                   "Consignação Criada",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Information);

                    TodasConsignacoes todas = new TodasConsignacoes();
                    todas.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(sucesso);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Erro ao registrar consignação:\n\n{ex.Message}",
                              "Erro",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void btnVoltar_Click(object sender, RoutedEventArgs e)
        {
            if (itensConsignacao.Any())
            {
                var resultado = MessageBox.Show(
                    "Existem itens na consignação. Deseja realmente sair sem salvar?",
                    "Confirmar Saída",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.No)
                    return;
            }

            fecharAquiEAbrirHome();
        }

        private void fecharAquiEAbrirHome()
        {
            new TodasConsignacoes().Show();
            this.Close();
        }

        // Método auxiliar para encontrar o parent de um tipo específico
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = System.Windows.Media.VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }

    // Classe auxiliar para exibição dos itens no carrinho com binding
    public class ConsignacaoItemDisplay : ConsignacaoItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _quantidadeEnviada;
        public new int QuantidadeEnviada
        {
            get => _quantidadeEnviada;
            set
            {
                if (_quantidadeEnviada != value)
                {
                    _quantidadeEnviada = value;
                    OnPropertyChanged(nameof(QuantidadeEnviada));
                    OnPropertyChanged(nameof(SubTotal));
                }
            }
        }

        private decimal _precoUnitarioAcordado;
        public new decimal PrecoUnitarioAcordado
        {
            get => _precoUnitarioAcordado;
            set
            {
                if (_precoUnitarioAcordado != value)
                {
                    _precoUnitarioAcordado = value;
                    OnPropertyChanged(nameof(PrecoUnitarioAcordado));
                    OnPropertyChanged(nameof(SubTotal));
                }
            }
        }

        public decimal SubTotal => QuantidadeEnviada * PrecoUnitarioAcordado;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}