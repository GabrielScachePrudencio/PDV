using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PDV_LANCHES.Views
{
    public partial class PedidoInfo : Window
    {
        private Pedido? pedido;
        private int id;
        private PedidoInfoController pedidoInfoController = new PedidoInfoController();

        public PedidoInfo(int id)
        {
            InitializeComponent(); 
            this.id = id;

            CarregarPedidoAssincrono();
        }

        private async void CarregarPedidoAssincrono()
        {
            try
            {
                pedido = await pedidoInfoController.getPedidoById(id);

                if (pedido != null)
                {
                    MontarPedidoTela();
                }
                else
                {
                    MessageBox.Show("Não foi possível carregar as informações do pedido.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar: " + ex.Message);
                this.Close();
            }
        }

        public void MontarPedidoTela()
        {
            if (pedido == null) return;

            pedidoCpfInfo.Text = pedido.CpfCliente;
            pedidoDataInfo.Text = pedido.DataCriacao.ToString("g");

            pedidoDataEntregaInfo.SelectedDate = pedido.DataEntrega;

            pedidoTotalInfo.Text = pedido.ValorTotal.ToString("C2");

            itensPedidoStackPanel.Children.Clear();

            foreach (var item in pedido.Itens)
            {
                Border bordaItem = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#f1f5f9")),
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 8),
                    CornerRadius = new CornerRadius(8)
                };

                DockPanel dock = new DockPanel();

                TextBox txtQtd = new TextBox
                {
                    Text = item.Quantidade.ToString(),
                    Width = 40,
                    Height = 30,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Tag = item
                };

                TextBlock txtNome = new TextBlock
                {
                    Text = $" Produto ID: {item.IdCardapio}",
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 0, 0, 0),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1e3a8a"))
                };

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
                pedido.CpfCliente = pedidoCpfInfo.Text;

                if (pedidoDataEntregaInfo.SelectedDate.HasValue)
                {
                    pedido.DataEntrega = pedidoDataEntregaInfo.SelectedDate.Value;
                }

                foreach (var child in itensPedidoStackPanel.Children)
                {
                    if (child is Border b && b.Child is DockPanel d)
                    {
                        var input = d.Children.OfType<TextBox>().FirstOrDefault();
                        if (input != null && input.Tag is ItemPedido item)
                        {
                            if (int.TryParse(input.Text, out int novaQtd))
                                item.Quantidade = novaQtd;
                        }
                    }
                }

                bool sucesso = await pedidoInfoController.AtualizarPedido(pedido);

                if (sucesso)
                {
                    MessageBox.Show("Pedido atualizado com sucesso!");
                    Home home = new Home(); 
                    home.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar alterações: " + ex.Message);
            }
        }
        private void Fechar_Click(object sender, RoutedEventArgs e) {
            Home home = new Home();
            home.Show();
            this.Close();
        }
    }
}