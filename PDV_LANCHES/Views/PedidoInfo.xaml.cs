using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using ServidorLanches.model.dto;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PDV_LANCHES.Views
{
    public partial class PedidoInfo : Window
    {
        private PedidoDTO? pedido;
        private int id;
        private PedidoInfoController pedidoInfoController = new PedidoInfoController();
        bool veioDeTodasVendas = false;

        public PedidoInfo(int id, bool veioDeTodasVendas = false)
        {
            InitializeComponent(); 
            this.id = id;
            this.veioDeTodasVendas = veioDeTodasVendas;
            CarregarPedidoAssincrono();
        }

        public PedidoInfo(PedidoDTO pedidoCompleto)
        {
            InitializeComponent();
            if(pedidoCompleto != null)
            {
                this.pedido = pedidoCompleto;
                this.id = pedidoCompleto.Id;
            }
            CarregarStatusAsync();
            CarregarPedidoAssincrono();
        }

        


        private async void CarregarPedidoAssincrono()
        {
            try
            {
                if (pedido == null)
                {
                    pedido = await pedidoInfoController.getPedidoById(id);
                }

                if (pedido != null)
                {
                    await CarregarStatusAsync();
                    await CarregarFormasDePagamentosAsync();
                    MontarPedidoTela();
                }
                else
                {
                    MessageBox.Show("Não foi possível carregar as informações do pedido.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar: " + ex.Message);
            }
        }
        private async Task CarregarStatusAsync()
        {
            await Status_Categorias.Instancia.CarregarAsync();

            ComboBoxStatusPedido.ItemsSource = Status_Categorias.Instancia.TipoStatusPedido;
            ComboBoxStatusPedido.DisplayMemberPath = "nome";
            ComboBoxStatusPedido.SelectedValuePath = "id";
        }
        private async Task CarregarFormasDePagamentosAsync()
        {
            await Status_Categorias.Instancia.CarregarAsync();

            ComboBoxAlterarFormaDePagamento.ItemsSource = Status_Categorias.Instancia.FormaDePagamentos;

            ComboBoxAlterarFormaDePagamento.SelectedValuePath = "Id";
        }

        public void MontarPedidoTela()
        {
            if (pedido == null) return;

            pedidoCpfInfo.Text = pedido.CpfCliente;
            // Removi a data da edição direta para limpar o layout, mas mantive o valor se necessário
            pedidoTotalInfo.Text = pedido.ValorTotal.ToString("C2");

            itensPedidoStackPanel.Children.Clear();
            ComboBoxStatusPedido.SelectedValue = pedido.IdStatus;
            ComboBoxAlterarFormaDePagamento.SelectedValue = pedido.IdFormaPagamento;

            foreach (var item in pedido.Itens)
            {
                Border bordaItem = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B")),
                    Padding = new Thickness(12),
                    Margin = new Thickness(0, 0, 0, 10),
                    CornerRadius = new CornerRadius(12),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#334155")),
                    BorderThickness = new Thickness(1)
                };

                DockPanel dock = new DockPanel();

                // Botão Excluir (Ícone de lixeira ou X)
                Button excluir = new Button
                {
                    Content = "✕",
                    Width = 28,
                    Height = 28,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#450A0A")),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F87171")),
                    FontWeight = FontWeights.Bold,
                    BorderThickness = new Thickness(0),
                    Cursor = Cursors.Hand,
                    Tag = item
                };
                excluir.Click += ExcluirItem_Click;
                // Arredondar botão excluir
                Style s = new Style(typeof(Border));
                s.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(6)));
                excluir.Resources.Add(typeof(Border), s);

                DockPanel.SetDock(excluir, Dock.Right);

                // Quantidade
                TextBox txtQtd = new TextBox
                {
                    Text = item.Quantidade.ToString(),
                    Width = 35,
                    Height = 28,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#020617")),
                    Foreground = new SolidColorBrush(Colors.White),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#334155")),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(10, 0, 10, 0),
                    Tag = item,

                };
                DockPanel.SetDock(txtQtd, Dock.Right);

                // Nome do Produto
                TextBlock txtNome = new TextBlock
                {
                    Text = item.NomeProduto.ToUpper(),
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold
                };

                Image image = new Image
                {
                    Width = 40,
                    Height = 40,
                    Margin = new Thickness(0, 0, 10, 0),
                    Source = new BitmapImage(new Uri(item.pathProdutoImg, UriKind.RelativeOrAbsolute))
                };

                
                dock.Children.Add(image);
                dock.Children.Add(excluir);
                dock.Children.Add(txtQtd);
                dock.Children.Add(txtNome);

                bordaItem.Child = dock;
                itensPedidoStackPanel.Children.Add(bordaItem);
            }
        }


        private async void Salvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (pedido == null) return;

                pedido.CpfCliente = pedidoCpfInfo.Text;

                if (ComboBoxStatusPedido.SelectedItem is TipoStatusPedido status)
                {
                    pedido.IdStatus = status.id;
                }
                else
                {
                    MessageBox.Show("Status inválido.");
                    return;
                }

                if (ComboBoxAlterarFormaDePagamento.SelectedValue != null)
                {
                    pedido.IdFormaPagamento = (int)ComboBoxAlterarFormaDePagamento.SelectedValue;
                }



                foreach (var child in itensPedidoStackPanel.Children)
                {
                    if (child is Border b && b.Child is DockPanel d)
                    {
                        var input = d.Children.OfType<TextBox>().FirstOrDefault();
                        if (input?.Tag is ItemPedidoCardapioDTO item &&
                            int.TryParse(input.Text, out int novaQtd))
                        {
                            item.Quantidade = novaQtd;
                        }
                    }
                }

                pedido.ValorTotal = pedido.Itens.Sum(i => i.Quantidade * i.ValorUnitario);
                MontarPedidoTela();

                bool sucesso = await pedidoInfoController.AtualizarPedido(pedido);

                if (sucesso)
                {
                    MessageBox.Show("Pedido atualizado com sucesso!");
                    if (veioDeTodasVendas == false)
                    {
                        Home home = new Home();
                        home.Show();
                        this.Close();
                    }
                    else
                    {
                        TodasVendasCompleto todasVendasCompleto = new TodasVendasCompleto();
                        todasVendasCompleto.Show();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar alterações: " + ex.Message);
            }
        }

        public void cardapioCompleto_click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            if (btn?.Tag is ItemPedidoCardapioDTO item)
            {
                pedido.Itens.Remove(item);
                MontarPedidoTela(); 
            }
            else
            {
                MessageBox.Show("Erro ao excluir item.");
            }

        }
        public void ExcluirItem_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            if (btn?.Tag is ItemPedidoCardapioDTO item)
            {
                pedido.Itens.Remove(item);
                MontarPedidoTela(); 
            }
            else
            {
                MessageBox.Show("Erro ao excluir item.");
            }

        }

        private void Fechar_Click(object sender, RoutedEventArgs e) {
            if(veioDeTodasVendas == false)
            {
                Home home = new Home();
                home.Show();
                this.Close();
            }
            else
            {
                TodasVendasCompleto todasVendasCompleto = new TodasVendasCompleto();
                todasVendasCompleto.Show();
                this.Close();
            }

        }
        private void CardapioCompleto_click(object sender, RoutedEventArgs e) {
            CardapioCompleto cardapiocompleto = new CardapioCompleto(pedido);
            cardapiocompleto.Show();
            this.Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lógica para quando a seleção mudar, se necessário
        }
        private void ComboBoxAlterarFormaDePagamento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Lógica para quando a seleção mudar, se necessário
        }
        


    }
}