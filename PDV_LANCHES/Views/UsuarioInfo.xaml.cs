using System;
using System.Windows;
using PDV_LANCHES.controller;
using PDV_LANCHES.model; // Namespace onde está seu Enum

namespace PDV_LANCHES.Views
{
    public partial class UsuarioInfo : Window
    {
        private Usuario _usuario;
        private HomeAdministrativoController controller = new HomeAdministrativoController();   

        public UsuarioInfo(Usuario usuario)
        {
            InitializeComponent();
            _usuario = usuario;

            // Carrega os valores do Enum TipoUsuario no ComboBox
            cbTipoUsuario.ItemsSource = Enum.GetValues(typeof(TipoUsuario));

            PreencherCampos();
        }

        private void PreencherCampos()
        {
            if (_usuario != null)
            {
                txtNomeUsuario.Text = _usuario.Nome;
                txtEmailUsuario.Text = _usuario.Email;
                cbTipoUsuario.SelectedItem = _usuario.TipoUsuario;
                txtDataCriacao.Text = $"Membro desde {_usuario.DataCriacao:dd/MM/yyyy}";
            }
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Salvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Atualiza o objeto com os novos dados da tela
                _usuario.Nome = txtNomeUsuario.Text;
                _usuario.Email = txtEmailUsuario.Text;
                _usuario.TipoUsuario = (TipoUsuario)cbTipoUsuario.SelectedItem;

                // Verificação de Senha: Só altera se o usuário digitou algo
                if (!string.IsNullOrWhiteSpace(txtSenhaUsuario.Password))
                {
                    _usuario.Senha = txtSenhaUsuario.Password;
                }

                if (await controller.updateUsuario(_usuario))
                {
                    MessageBox.Show("Dados do usuário atualizados com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true; // Fecha retornando confirmação
                }
                else
                {
                    MessageBox.Show("Erro ao atualizar");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}