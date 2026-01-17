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
using System.Windows.Shapes;

namespace PDV_LANCHES.Views
{

    public partial class EscolhaQualHome : Window
    {
        private HomeController homeController = new HomeController();
        public EscolhaQualHome()
        {
            InitializeComponent();
        }
        
        private async void Sair_Click(object sender, RoutedEventArgs e)
        {
            await homeController.Logout();
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
        
        private void IrVendas(object sender, RoutedEventArgs e)
        {

            Home home = new Home();
            home.Show();
            this.Close();
        }
        
        private void IrAdministrativo(object sender, RoutedEventArgs e)
        {
            HomeAdministrativo homeAdministrativo = new HomeAdministrativo();
            homeAdministrativo.Show();
            this.Close();
        }
        
    }
 }