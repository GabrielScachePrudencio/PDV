using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PDV_LANCHES.controller;
using ServidorLanches.model;

namespace PDV_LANCHES.Views.ViewsAdministrativo
{
    public partial class AlllCuponsDeDesconto : UserControl
    {
        public List<CupomDesconto> ListaCupons { get; set; }
        private HomeAdministrativoController _controller = new HomeAdministrativoController();

        public AlllCuponsDeDesconto()
        {
            InitializeComponent();
            CarregarCupons();
        }

        private async void CarregarCupons()
        {
            await Status_Categorias.Instancia.CarregarAsync();
            ListaCupons = Status_Categorias.Instancia.CuponsDesconto; 
            dgCupons.ItemsSource = ListaCupons;
        }

        private async void NovaCategoria_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNomeCupom.Text))
            {
                MessageBox.Show("Informe o nome do cupom.");
                return;
            }

            decimal.TryParse(txtPercentual.Text, out decimal perc);
            decimal.TryParse(txtValorFixo.Text, out decimal valor);

            var novo = new CupomDesconto
            {
                Nome = txtNomeCupom.Text,
                Percentual = perc > 0 ? perc : (decimal?)null,
                ValorFixo = valor > 0 ? valor : (decimal?)null,
                DataValidade = dtValidade.SelectedDate,
                Ativo = true
            };

            if (await _controller.AddCupom(novo)) 
            {
                await Status_Categorias.Instancia.RecarregarTudoAsync();
                ListaCupons.Add(novo);
                dgCupons.Items.Refresh();
                LimparCampos();
                MessageBox.Show("Cupom criado!");
            }
            else
            {
                MessageBox.Show("Erro ao adicionar cupom ");
            }
        }

        private async void AlternarStatus_Click(object sender, RoutedEventArgs e)
        {
            var cupom = (sender as Button)?.DataContext as CupomDesconto;
            if (cupom != null)
            {
                bool novoStatus = !cupom.Ativo;
                cupom.Ativo = novoStatus; // Inverte localmente para testar a atualização

                if (await _controller.UpdateCupom(cupom.Id, cupom))
                {
                    dgCupons.Items.Refresh();
                }
                else
                {
                    cupom.Ativo = !novoStatus; // Volta ao original se falhar
                    MessageBox.Show("Erro ao atualizar status.");
                }
            }
        }

        private void LimparCampos()
        {
            txtNomeCupom.Clear();
            txtPercentual.Clear();
            txtValorFixo.Clear();
            dtValidade.SelectedDate = null;
        }
    }
}