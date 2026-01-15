using PDV_LANCHES.controller;
using PDV_LANCHES.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PDV_LANCHES
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            String loginNome = inputLoginNome.Text;
            String loginSenha = inputLoginSenha.Password;
        
            if(loginNome != null || loginNome != "" || loginSenha != null || loginSenha != "")             
            {
                Login login = new Login();  
                if (await login.VerificarLogin(loginNome, loginSenha))
                {
                    Home home = new Home();
                    MessageBox.Show("Login Efetuado com Sucesso!");
                    home.Show();
                    this.Close();   
                }
                else
                {
                    MessageBox.Show("Login ou Senha Incorretos!");
                }
            }
            else
            {
                MessageBox.Show("Login ou senha Nulos ou Vazios!");
            }


        }
    }
}