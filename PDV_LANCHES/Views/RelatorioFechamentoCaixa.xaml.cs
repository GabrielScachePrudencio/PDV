using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace PDV_LANCHES.Views
{
    public partial class RelatorioFechamentoCaixa : Window
    {
        private HomeController homeController = new HomeController();
        private bool veioDoHome, veioDoHomeAdmnistrativo, veioDoEscolha;

        public RelatorioFechamentoCaixa(bool veioDoHome = false, bool veioDoHomeAdmnistrativo = false, bool veioDoEscolha = false)
        {
            InitializeComponent();

            this.veioDoHome = veioDoHome;
            this.veioDoHomeAdmnistrativo = veioDoHomeAdmnistrativo;
            this.veioDoEscolha = veioDoEscolha;

            // Chamada assíncrona para carregar os dados
            Loaded += (s, e) => CarregarCaixaCompleto();
        }

        public async void CarregarCaixaCompleto()
        {
            var dados = await Status_Categorias.Instancia.ObterDadosFechamento();

            if (dados != null)
            {
                this.DataContext = dados;

                if (dados.valorFinal == 0)
                {
                    txtValorFinal.Text = dados.valor_calculado.ToString("F2"); 
                }
                else
                {
                    txtValorFinal.Text = dados.valorFinal.ToString("F2");
                }

                AtualizarLabelsDiferenca();
            }
            else
            {
                MessageBox.Show("Erro ao calcular dados de fechamento.");
                this.Close();
            }
        }
        
        private void AtualizarLabelsDiferenca()
        {
            var caixa = Status_Categorias.Instancia.caixa;
            if (caixa != null)
            {
                txtDiferenca.Text = caixa.diferença.ToString("C2");
                // Lógica de cor simples
                txtDiferenca.Foreground = caixa.diferença < 0 ?
                    System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.DarkGreen;
            }
        }

        private async void FecharCaixa_Click(object sender, RoutedEventArgs e)
        {
            var caixa = Status_Categorias.Instancia.caixa;

            if (!decimal.TryParse(
                txtValorFinal.Text,
                System.Globalization.NumberStyles.Number,
                new System.Globalization.CultureInfo("pt-BR"),
                out decimal valorDigitado))
            {
                MessageBox.Show("Valor inválido");
                return;
            }

            caixa.valorFinal = valorDigitado;
            caixa.diferença = valorDigitado - caixa.valor_calculado;

            var confirmacao = MessageBox.Show(
                $"Confirmar encerramento?\nValor Gaveta: {caixa.valorFinal:C2}\nDiferença: {caixa.diferença:C2}",
                "Confirmação",
                MessageBoxButton.YesNo
            );

            if (confirmacao == MessageBoxResult.Yes)
            {
                bool sucesso = await Status_Categorias.Instancia.salvarCaixa();

                if (sucesso)
                {
                    MessageBox.Show("Caixa Salvo e Fechado com Sucesso!");
                    await homeController.Logout();
                    new MainWindow().Show();
                    Close();
                }
            }
        }







        public void voltar_click(object sender, RoutedEventArgs e)

        {

            if (veioDoHome)

            {

                Home home = new Home();

                home.Show();

            }
            else if (veioDoHomeAdmnistrativo)

            {

                HomeAdministrativo HomeAdministrativo = new HomeAdministrativo();

                HomeAdministrativo.Show();

            }

            else

            {

                EscolhaQualHome escolhaQualHome = new EscolhaQualHome();

                escolhaQualHome.Show();

            }



            this.Close();



        }





    }
}