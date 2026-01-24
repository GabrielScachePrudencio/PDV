using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using PDV_LANCHES.Views.ViewsAdministrativo;
using ServidorLanches.model;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PDV_LANCHES.Views
{
    public partial class HomeAdministrativo : Window
    {
        private HomeController homeController = new HomeController();
        private Usuario usuarioLogado;

        public HomeAdministrativo()
        {
            InitializeComponent();
            Loaded += HomeAdministrativo_Loaded;
        }

        private async void HomeAdministrativo_Loaded(object sender, RoutedEventArgs e)
        {
            // Carregar dados simultaneamente
            await Task.WhenAll(CarregarDadosUsuario(), CarregarDadosEmpresa());
            ConteudoConfiguracoes.Content = new ConfiguracoesGeraisVA2();

        }

        private async Task CarregarDadosUsuario()
        {
            try
            {
                var usuario = await homeController.pegarUsuarioLogado();
                if (usuario == null)
                {
                    MessageBox.Show("Sessão expirada. Faça login novamente.");
                    Close();
                    return;
                }

                usuarioLogado = usuario;
                usuarioLogadoHome.Text = usuario.Nome;

                if (usuario.TipoUsuario != TipoUsuario.Administrador)
                {
                    btnUsuarios.Visibility = Visibility.Collapsed;
                    btnBanco.Visibility = Visibility.Collapsed;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar usuário: " + ex.Message);
            }
        }

        private async Task CarregarDadosEmpresa()
        {
            try
            {
                Login login = new Login();
                ConfiguracoesGerais config = await login.ConfiguracoesGerais();

                if (config != null)
                {
                    // Atualiza o Nome Fantasia
                    txtNomeLoja.Text = (config.nomeFantasia ?? config.nome ?? "MINHA LOJA").ToUpper();

                    // Carrega a Imagem da Logo
                    if (!string.IsNullOrEmpty(config.pathImagemLogo) && File.Exists(config.pathImagemLogo))
                    {
                        imgLogoLoja.Source = LoadImage(config.pathImagemLogo);
                        txtLogoPlaceholder.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtLogoPlaceholder.Visibility = Visibility.Visible;
                    }
                }
            }
            catch
            {
                txtNomeLoja.Text = "ERRO AO CARREGAR";
                txtLogoPlaceholder.Visibility = Visibility.Visible;
            }

            
        }

        // Método auxiliar para evitar travar o arquivo de imagem no Windows
        private BitmapImage LoadImage(string path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        private void ConfiguracoesGeraisVA(object sender, RoutedEventArgs e)
        {
            ConteudoConfiguracoes.Content = new ConfiguracoesGeraisVA2();
        }

        private void AllCardapio(object sender, RoutedEventArgs e)
        {
            ConteudoConfiguracoes.Content = new AllProduto();
        }

        private void AllRelatorio(object sender, RoutedEventArgs e)
        {
            ConteudoConfiguracoes.Content = new AllRelatorio();
        }

        private void AllUsuario(object sender, RoutedEventArgs e)
        {
            ConteudoConfiguracoes.Content = new AllUsuario();
        }

        private void ConfiguracaoFiscal(object sender, RoutedEventArgs e)
        {
            ConteudoConfiguracoes.Content = new ConfiguracaoFiscal();
        }

        private void ConfiguracaoBanco(object sender, RoutedEventArgs e)
        {
            ConteudoConfiguracoes.Content = new ConfiguracaoBanco();
        }

        private async void Sair_Click(object sender, RoutedEventArgs e)
        {
            await homeController.Logout();
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void VoltarParaEscolha_Click(object sender, RoutedEventArgs e)
        {
            EscolhaQualHome voltarParaEscolha = new EscolhaQualHome();
            voltarParaEscolha.Show();
            this.Close();
        }
    }
}