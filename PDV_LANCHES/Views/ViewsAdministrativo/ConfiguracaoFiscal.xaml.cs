using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using PDV_LANCHES.controller;
using ServidorLanches.model;
using System.Globalization;

namespace PDV_LANCHES.Views.ViewsAdministrativo
{
    public partial class ConfiguracaoFiscal : UserControl
    {
        private HomeAdministrativoController homeAdministrativoController = new HomeAdministrativoController();
        private bool ehNovo = false;
        private int idAtual = 0;

        public ConfiguracaoFiscal()
        {
            InitializeComponent();
            CarregarConfiguracoesFiscais();
        }

        private async void CarregarConfiguracoesFiscais()
        {
            try
            {
                ConfiguracoesFiscais config = await homeAdministrativoController.GetConfiguracaoFiscal();

                if (config == null)
                {
                    ehNovo = true;
                    btnSalvar.Content = "Cadastrar Configurações";
                }
                else
                {
                    ehNovo = false;
                    idAtual = config.Id;
                    btnSalvar.Content = "Atualizar Configurações";

                    txtCnpj.Text = config.Cnpj;
                    txtInscricaoEstadual.Text = config.InscricaoEstadual;
                    txtAliquotaIcms.Text = config.AliquotaIcms.ToString("F2", CultureInfo.InvariantCulture);
                    txtCsosn.Text = config.Csosn;
                    txtCstPis.Text = config.CstPis;
                    txtCstCofins.Text = config.CstCofins;
                    txtSerieNf.Text = config.SerieNf;
                    txtUltimoNumeroNf.Text = config.NumeroUltimaNf.ToString();
                    txtCaminhoCertificado.Text = config.CaminhoCertificado;
                    txtValidadeCertificado.Text = config.ValidadeCertificado.ToShortDateString();

                    SelecionarItemCombo(cbRegimeTributario, config.RegimeTributario);
                    cbAmbiente.SelectedIndex = config.AmbienteProducao ? 1 : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar: " + ex.Message);
            }
        }

        private void txtCnpj_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string value = new string(tb.Text.Where(char.IsDigit).ToArray());
            if (value.Length > 14) value = value.Substring(0, 14);

            if (value.Length >= 13) value = value.Insert(12, "-").Insert(8, "/").Insert(5, ".").Insert(2, ".");
            else if (value.Length >= 9) value = value.Insert(8, "/").Insert(5, ".").Insert(2, ".");
            else if (value.Length >= 6) value = value.Insert(5, ".").Insert(2, ".");
            else if (value.Length >= 3) value = value.Insert(2, ".");

            if (tb.Text != value)
            {
                tb.Text = value;
                tb.CaretIndex = tb.Text.Length;
            }
        }

        private void btnSelecionarCertificado_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Certificados (*.pfx)|*.pfx" };
            if (openFileDialog.ShowDialog() == true)
            {
                txtCaminhoCertificado.Text = openFileDialog.FileName;
                txtValidadeCertificado.Text = DateTime.Now.AddYears(1).ToShortDateString();
            }
        }

        private async void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var config = new ConfiguracoesFiscais
                {
                    Id = idAtual,
                    Cnpj = new string(txtCnpj.Text.Where(char.IsDigit).ToArray()), // Salva sem máscara
                    InscricaoEstadual = txtInscricaoEstadual.Text,
                    RegimeTributario = (cbRegimeTributario.SelectedItem as ComboBoxItem)?.Content.ToString(),
                    AliquotaIcms = decimal.TryParse(txtAliquotaIcms.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal aliq) ? aliq : 0,
                    Csosn = txtCsosn.Text,
                    CstPis = txtCstPis.Text,
                    CstCofins = txtCstCofins.Text,
                    SerieNf = txtSerieNf.Text,
                    NumeroUltimaNf = int.TryParse(txtUltimoNumeroNf.Text, out int num) ? num : 0,
                    AmbienteProducao = cbAmbiente.SelectedIndex == 1,
                    CaminhoCertificado = txtCaminhoCertificado.Text,
                    ValidadeCertificado = DateTime.TryParse(txtValidadeCertificado.Text, out DateTime data) ? data : DateTime.MinValue
                };

                bool sucesso = ehNovo ? await homeAdministrativoController.AddConfiguracaoFiscal(config)
                                     : await homeAdministrativoController.UpdateConfiguracaoFiscal(config);

                if (sucesso) MessageBox.Show("Salvo com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar: " + ex.Message);
            }
        }

        private void SelecionarItemCombo(ComboBox combo, string valor)
        {
            foreach (ComboBoxItem item in combo.Items)
            {
                if (item.Content.ToString() == valor) { combo.SelectedItem = item; break; }
            }
        }
    }
}