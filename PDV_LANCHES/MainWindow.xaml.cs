using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Tls;
using PDV_LANCHES.controller;
using PDV_LANCHES.model;
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
        private Login login = new Login();
        ConfiguracoesBanco configguracao = new ConfiguracoesBanco();
        public MainWindow()
        {
            InitializeComponent();
            CarregarConfiguracoesGerais();
        }


        private async void Entrar_Click(object sender, RoutedEventArgs e)
        {
            // Sugestão: Mostrar um ProgressBar aqui se a conexão demorar
           
            var resultado = await login.VerificarConexaoServidor();

            if (resultado.Sucesso)
            {
                MessageBox.Show("Conectado com sucesso!");
                PainelInicial.Visibility = Visibility.Collapsed;

                InformacoesNaEsquerda.Visibility = Visibility.Visible;
                informacoesNaDireita.Visibility = Visibility.Visible;

                CarregarConfiguracoesGerais();
            }
            else
            {
                MessageBox.Show("Não foi possível conectar ao servidor. Verifique sua rede.", "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Warning);

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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string loginNome = inputLoginNome.Text;
            string loginSenha = inputLoginSenha.Password;

            // Correção da validação: string.IsNullOrWhiteSpace é mais seguro
            if (!string.IsNullOrWhiteSpace(loginNome) && !string.IsNullOrWhiteSpace(loginSenha))
            {
                Login login = new Login();
                Usuario usuario = await login.VerificarLogin(loginNome, loginSenha);

                if (usuario != null)
                {
                    if (usuario.TipoUsuario == TipoUsuario.Administrador || usuario.TipoUsuario == TipoUsuario.Gerente)
                    {
                        new EscolhaQualHome().Show();
                        this.Close();
                    }
                    else
                    {
                        new Home().Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Usuário ou senha inválidos.");
                }
            }
            else
            {
                MessageBox.Show("Preencha todos os campos!");
            }
        }

        private async void CarregarConfiguracoesGerais()
        {
            Login login = new Login();
            var config = await login.ConfiguracoesGerais();

            if(config != null)
            {
                if (!string.IsNullOrWhiteSpace(config.pathImagemLogo))
                {
                    try
                    {
            
                        // 1. Verifica se o arquivo físico realmente existe no disco
                        if (System.IO.File.Exists(config.pathImagemLogo))
                        {
                            pdvLogo.Source = new BitmapImage(new Uri(config.pathImagemLogo, UriKind.Absolute));
                        }
                        else
                        {
                            // 2. Se não existir, carrega uma imagem padrão do seu projeto (Resource)
                            // Ou apenas define como null para não quebrar
                            Console.WriteLine("Arquivo não encontrado: " + config.pathImagemLogo);
                            // pdvLogo.Source = new BitmapImage(new Uri("pack://application:,,,/src/img/sem-foto.png"));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erro ao carregar imagem: " + ex.Message);
                    }
                }

                if (config.nomeFantasia != null)
                {
                    pdvNome.Text = config.nomeFantasia;
                }
                else
                {
                    pdvNome.Text = config.nome;
                }
            }
            


        }

        private void configurarPdvNoServidor_Click(object sender, RoutedEventArgs e)
        {
            ConfiguracaoServidor configuracaoServidor = new ConfiguracaoServidor();
            configuracaoServidor.Show();
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Se o botão esquerdo do mouse for pressionado, permite arrastar
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }


        private async void Fechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}