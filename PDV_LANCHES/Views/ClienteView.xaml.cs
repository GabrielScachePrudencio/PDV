using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using System;
using System.Windows;

namespace PDV_LANCHES.Views
{
    public partial class ClienteView : Window
    {
        private int _clienteId;
        private bool _modoEdicao = false;
        private ConsignacaoController controller = new ConsignacaoController();
        // 🔹 Construtor para CRIAR
        public ClienteView(string cpf)
        {
            InitializeComponent();
            _clienteId = -1;
            txtTitulo.Text = "Novo Cliente";
            txtCpf.Text = cpf;
            txtCpf.IsEnabled = false;
            _modoEdicao = false;
        }
        public ClienteView()
        {
            InitializeComponent();
            _clienteId = -1;
            txtTitulo.Text = "Novo Cliente";
            _modoEdicao = false;
        }

        // 🔹 Construtor para EXIBIR / EDITAR
        public ClienteView(Cliente cliente)
        {
            InitializeComponent();

            _clienteId = cliente.Id;
            _modoEdicao = true;

            txtTitulo.Text = "Detalhes do Cliente";

            CarregarCliente(cliente);
        }

        private async void CarregarCliente(Cliente cliente)
        {

            if (cliente == null)
            {
                MessageBox.Show("Cliente não encontrado.");
                Close();
                return;
            }

            txtNome.Text = cliente.Nome;
            txtCpf.Text = cliente.CpfCnpj;
            txtTelefone.Text = cliente.Telefone;
            txtEmail.Text = cliente.Email;
            txtEndereco.Text = cliente.Endereco;
            chkAtivo.IsChecked = cliente.Ativo;
        }

        private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            var cliente = new PDV_LANCHES.model.Cliente
            {
                Id = _clienteId,
                Nome = txtNome.Text,
                CpfCnpj = txtCpf.Text,
                Telefone = txtTelefone.Text,
                Email = txtEmail.Text,
                Endereco = txtEndereco.Text,
                Ativo = chkAtivo.IsChecked ?? false
            };

            try
            {
                bool sucesso;

                if (_modoEdicao)
                {
                    sucesso = await controller.atualizarCliente(cliente);
                }
                else
                {
                    sucesso = await controller.criarCliente(cliente);
                }

                if (!sucesso)
                {
                    MessageBox.Show("CPF já cadastrado.");
                    return;
                }

                MessageBox.Show("Cliente salvo com sucesso!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
