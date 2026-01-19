using PDV_LANCHES.controller;
using PDV_LANCHES.model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PDV_LANCHES.Views.ViewsAdministrativo
{
    /// <summary>
    /// Interação lógica para GetUsuarios.xam
    /// </summary>
    public partial class AllUsuario : UserControl
    {
        private List<Usuario> listaUsuarios;
        private HomeAdministrativoController administrativoController = new HomeAdministrativoController();

        public AllUsuario()
        {
            InitializeComponent();
            CarregarUsuarios();
        }

        public async void CarregarUsuarios() {
            listaUsuarios = await administrativoController.getAllUsuarios();
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

            MessageBox.Show($"Editar: {usuario.Nome}");
        }

        private void Excluir_Click(object sender, RoutedEventArgs e)
        {
            var usuario = (sender as Button)?.DataContext as Usuario;
            if (usuario == null) return;

            MessageBox.Show($"Excluir: {usuario.Nome}");
        }

    }
}
