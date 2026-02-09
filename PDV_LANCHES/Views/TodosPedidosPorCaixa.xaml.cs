using PDV_LANCHES.controller;
using ServidorLanches.model.dto;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PDV_LANCHES.Views
{
    public partial class TodosPedidosPorCaixa : Window
    {
        private int _idCaixa;
        private HomeController homeController = new HomeController();
        private List<PedidoDTO> listaDePedidos = new List<PedidoDTO>();   

        public TodosPedidosPorCaixa(int idCaixa)
        {
            InitializeComponent();
            this._idCaixa = idCaixa;
            txtIdCaixa.Text = idCaixa.ToString("D2");

            CarregarPedidos();
        }

        private async void CarregarPedidos()
        {
            listaDePedidos = await homeController.PegarPedidosPorCaixa(_idCaixa);

            if(listaDePedidos == null || listaDePedidos.Count == 0)
            {
                MessageBox.Show("Nenhum pedido encontrado para este caixa.");
                return;
            }

            dgPedidos.ItemsSource = listaDePedidos;

            carregarTotal();
        }

        private async void carregarTotal()
        {
            if (listaDePedidos != null && listaDePedidos.Count > 0)
            {
                decimal total = listaDePedidos.Sum(p => p.ValorTotal);

                txtTotalVendas.Text = total.ToString("C2");
            }
            else
            {
                txtTotalVendas.Text = "R$ 0,00";
            }
        }

        private void Fechar_Click(object sender, RoutedEventArgs e)
        {
            VendasPorCaixa vendas = new VendasPorCaixa();
            vendas.Show();
            this.Close();
        }

        private void VerDetalhesPedido_Click(object sender, RoutedEventArgs e)
        {
            var pedido = (PedidoDTO)((FrameworkElement)sender).DataContext;

            if (pedido != null)
            {
                PedidoInfo pedidoinfo = new PedidoInfo(pedido.Id, false, false, true, _idCaixa);
                pedidoinfo.Show();
                this.Close();
            }
        }
    }
}