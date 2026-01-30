using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using ServidorLanches.model.dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PDV_LANCHES.Views.ViewsAdministrativo
{
    /// <summary>
    /// Interação lógica para GetUsuarios.xam
    /// </summary>
    public partial class AllUsuario : UserControl
    {
        private ObservableCollection<Usuario> listaUsuarios;
        private HomeAdministrativoController administrativoController = new HomeAdministrativoController();

        public AllUsuario()
        {
            InitializeComponent();
            _ = CarregarUsuarios();

        }

        public async Task CarregarUsuarios() {
            var usuarios = await administrativoController.getAllUsuarios();

            listaUsuarios = new ObservableCollection<Usuario>(usuarios);
            dgUsuario.ItemsSource = listaUsuarios;
        }

        private void Adicionar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Abrir modal para adicionar item");
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            var usuario = (sender as Button)?.DataContext as Usuario;
            if (usuario == null) return;
            UsuarioInfo usuarioInfo = new UsuarioInfo(usuario);
            usuarioInfo.ShowDialog();

            if (usuarioInfo.ShowDialog() == true)
            {
                _ = CarregarUsuarios();
            }

        }

        private async void Excluir_Click(object sender, RoutedEventArgs e)
        {
            var usuario = (sender as Button)?.DataContext as Usuario;
            if (usuario == null) return;

            if (await administrativoController.deletarUsuario(usuario.Id))
            {
                MessageBox.Show("usuario deletado");
                listaUsuarios.Remove(usuario);
            } else
            {
                MessageBox.Show("erro ao deletar usuario");
            }


        }

        private void verDetalhes(object sender, MouseButtonEventArgs e)
        {
            // Obtém o usuário da linha clicada
            var usuarioSelecionado = dgUsuario.SelectedItem as Usuario;

            if (usuarioSelecionado != null)
            {
                UsuarioInfo infoWindow = new UsuarioInfo(usuarioSelecionado);

                // Abre como diálogo. Se retornar true, atualizamos a lista
                if (infoWindow.ShowDialog() == true)
                {
                    _ = CarregarUsuarios();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NovoUsuario novoUsuario = new NovoUsuario();
            if (novoUsuario.ShowDialog() == true)
            {
                _ = CarregarUsuarios();
            }

        }
    }
}
