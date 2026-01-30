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
    public partial class ConfiguracaoServidor : Window
    {
        private Login login = new Login();

        public ConfiguracaoServidor()
        {
            InitializeComponent();
            CarregarDoJson();
        }

        private void CarregarDoJson()
        {
            var config = CarregarConfiguracaoJson.ObterConfiguracao();
            if (config != null)
            {
                txtIpServidor.Text = config.Host;
                txtPortaServidor.Text = config.Porta.ToString();
                txtPortaDoBanco.Text = config.PortaBanco.ToString();
                txtNomeDatabase.Text = config.NomeBanco;
                txtUsuarioDatabase.Text = config.UsuarioBanco;
                txtSenhaDatabase.Text = config.senhaBanco;
                
            }
        }

        private async void BtnTestar_Click(object sender, RoutedEventArgs e)
        {
            ConfiguracoesBanco configuracao = new ConfiguracoesBanco
            {
                Host = txtIpServidor.Text,
                Porta = int.Parse(txtPortaServidor.Text),
                PortaBanco = int.Parse(txtPortaDoBanco.Text),
                NomeBanco = txtNomeDatabase.Text,
                UsuarioBanco = txtUsuarioDatabase.Text,
                senhaBanco = txtSenhaDatabase.Text,
                TipoConexao = "MySQL"
            };

            ResultadoApi resultado = await login.VerificarConexaoDireta(configuracao);

            if (resultado.Sucesso)
            {
                MessageBox.Show("Conectado com sucesso!");
            }
            else
            {
                switch (resultado.StatusCode)
                {
                    case 400:
                        MessageBox.Show("Dados incompletos! Verifique se todos os campos (IP, Porta, Banco, Usuário) foram preenchidos.");
                        break;
                    case 404:
                        MessageBox.Show("Não foi possível encontrar o Servidor. Verifique o IP e se o servidor está rodando.");
                        break;
                    case 503:
                        MessageBox.Show("O Servidor está online, mas não conseguiu conectar ao MySQL com esses dados.");
                        break;
                    default:
                        MessageBox.Show($"Erro inesperado: {resultado.StatusCode}");
                        break;
                }
            }
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)

        {

            ConfiguracoesBanco configuracao = new ConfiguracoesBanco
            {
                Host = txtIpServidor.Text,
                Porta = int.Parse(txtPortaServidor.Text),
                PortaBanco = int.Parse(txtPortaDoBanco.Text),
                NomeBanco = txtNomeDatabase.Text,
                UsuarioBanco = txtUsuarioDatabase.Text,
                senhaBanco = txtSenhaDatabase.Text,
                TipoConexao = "MySQL"
            };

            CarregarConfiguracaoJson.setConfiguracoes(configuracao);

            ApiClient.ReiniciarCliente();

            MessageBox.Show("Configurações aplicadas com sucesso!");
            AbrirLogin();
        }

        private void validaSenhaBasica_Click(object sender, RoutedEventArgs e)
        {
            if (senhabasica.Password == "pdvadm")
            {
                ParteDaSenhaBasica.Visibility = Visibility.Hidden;
                ConfiguracaoPara.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Senha incorreta!");
                MainWindow login = new MainWindow();
                login.Show();
                this.Close();
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e) => AbrirLogin();

        private void AbrirLogin()
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Se o botão esquerdo do mouse for pressionado, permite arrastar
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

       
    }
}
