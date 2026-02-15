using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using ServidorLanches.model.dto;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PDV_LANCHES.Views
{
    public partial class TodosOsClientes : Window
    {
        private List<Cliente> clientes;
        private ConsignacaoController controller = new ConsignacaoController();

        public TodosOsClientes()
        {
            InitializeComponent();
            CarregarClientes();
        }

        private async void CarregarClientes()
        {
            clientes = await controller.listarClientes();

            var listaFiltrada = clientes.Where(c => c.Ativo).ToList(); 

            ListaClientes.ItemsSource = listaFiltrada;
        }

        private void BtnEntrar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is int clienteId)
            {
                Cliente c = clientes.Find(cl => cl.Id == clienteId);
                if (c != null)
                {
                    ClienteView detalhes = new ClienteView(c);
                    detalhes.ShowDialog();
                }
            }
        }

         // ================= MANIPULADORES DE CLICK =================

        private void Pesquisar_Click(object sender, RoutedEventArgs e)
        {
            string filtro = txtBuscaCliente.Text.Trim().ToLower();
            var filtrados = clientes.FindAll(c =>
                c.Nome.ToLower().Contains(filtro) ||
                c.CpfCnpj.ToLower().Contains(filtro) ||
                c.Email.ToLower().Contains(filtro)
            );
            ListaClientes.ItemsSource = filtrados;
        }

        private void EntrarCliente_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button botao && botao.DataContext is Cliente clienteselecionado)
            {
                ClienteView cadastro = new ClienteView(clienteselecionado);
                if (cadastro.ShowDialog() == true)
                {
                    CarregarClientes();
                }

                CarregarClientes();
            }
        }

        private void AdicionarCliente_Click(object sender, RoutedEventArgs e)
        {
            ClienteView cadastro = new ClienteView();
            if (cadastro.ShowDialog() == true)
            {
                CarregarClientes();
            }
        }
    
    
        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            new Home().Show();
            this.Close();
        }


    }
}
