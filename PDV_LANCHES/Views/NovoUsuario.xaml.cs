using System;
using System.Windows;
using PDV_LANCHES.model;
using PDV_LANCHES.controller;

namespace PDV_LANCHES.Views
{
    public partial class NovoUsuario : Window
    {
        private HomeAdministrativoController controller = new HomeAdministrativoController();
        public NovoUsuario()
        {
            InitializeComponent();
            // Carrega o Enum TipoUsuario
            cbTipo.ItemsSource = Enum.GetValues(typeof(TipoUsuario));
            cbTipo.SelectedIndex = 0; // Define 'Vendedor' como padrão
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            // Validações simples
            if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtSenha.Password))
            {
                MessageBox.Show("Por favor, preencha o nome e a senha.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var novo = new Usuario
                {
                    
                    Nome = txtNome.Text,
                    Email = txtEmail.Text,
                    Senha = txtSenha.Password, // Lembre-se de criptografar na controller/API
                    TipoUsuario = (TipoUsuario)cbTipo.SelectedItem,
                    DataCriacao = DateTime.Now
                };


                if (await controller.addUsuario(novo))
                {
                    MessageBox.Show("Usuário cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true; // Fecha a janela e sinaliza sucesso para a tela anterior
                }
                else MessageBox.Show("Erro ao cadastrar");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao cadastrar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}