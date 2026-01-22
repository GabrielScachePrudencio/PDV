using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using ServidorLanches.model.dto;
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

namespace PDV_LANCHES.Views
{
    /// <summary>
    /// Lógica interna para CardapioCompleto.xaml
    /// </summary>
    public partial class CardapioCompleto : Window
    {

        private PedidoDTO pedido;
        private List<Cardapio> cardapio;
        private NovoPedidoController controller = new NovoPedidoController();

        public CardapioCompleto(PedidoDTO pedidoAtual)
        {
            InitializeComponent();
            pedido = pedidoAtual;

            txtCpf.Text = pedido.CpfCliente;
            AtualizarTotal();

            CarregarCardapio();
        }

        private async void CarregarCardapio()
        {
            cardapio = await controller.getAllCardapio();
            ListaCardapioCompleto.ItemsSource = cardapio;
        }

        private void AdicionarItem_Click(object sender, RoutedEventArgs e)
        {
            var produto = (sender as Button)?.Tag as Cardapio;
            if (produto == null) return;

            int qtdAApender = produto.QuantidadeSelecionada <= 0 ? 1 : produto.QuantidadeSelecionada;

            var existente = pedido.Itens.FirstOrDefault(i => i.IdCardapio == produto.Id);

            if (existente != null)
            {
                existente.Quantidade += qtdAApender;
            }
            else
            {
                pedido.Itens.Add(new ItemPedidoCardapioDTO
                {
                    IdCardapio = produto.Id,
                    NomeCardapio = produto.Nome,
                    ValorUnitario = produto.Valor,
                    Quantidade = qtdAApender,
                    pahCardapioImg = produto.pathImg,
                    Categoria = produto.Categoria
                });
            }

            produto.QuantidadeSelecionada = 1;

            AtualizarTotal();

        }

        private void AtualizarTotal()
        {
            pedido.ValorTotal = pedido.Itens.Sum(i => i.Quantidade * i.ValorUnitario);
            txtTotal.Text = $"R$ {pedido.ValorTotal:F2}";
        }

        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            PedidoInfo telaInfo = new PedidoInfo(pedido);
            telaInfo.Show();
            this.Close();
        }
    }
}
