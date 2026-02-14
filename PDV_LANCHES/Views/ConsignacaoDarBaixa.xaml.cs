using PDV_LANCHES.controller;
using PDV_LANCHES.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PDV_LANCHES.Views
{
    public partial class ConsignacaoDarBaixa : Window
    {
        private Consignacao consignacao;
        private int id;
        private bool veioDeListaConsignacoes = false;
        private ConsignacaoController consignacaoController = new ConsignacaoController();

        public ConsignacaoDarBaixa(int id, bool veioDeListaConsignacoes = false)
        {
            InitializeComponent();
            this.id = id;
            this.veioDeListaConsignacoes = veioDeListaConsignacoes;
            CarregarConsignacaoAssincrono();
        }

        public ConsignacaoDarBaixa(Consignacao consignacaoCompleta)
        {
            InitializeComponent();
            if (consignacaoCompleta != null)
            {
                this.consignacao = consignacaoCompleta;
                this.id = consignacaoCompleta.Id;
            }
            CarregarConsignacao();
        }

        private async void CarregarConsignacaoAssincrono()
        {
            try
            {
                if (consignacao == null)
                {
                    consignacao = await consignacaoController.GetByIdConsignacao(id);
                }

                CarregarConsignacao();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar consignação: " + ex.Message);
            }
        }

        private void CarregarConsignacao()
        {
            if (consignacao == null) return;

            // Preencher informações básicas
            txtFornecedor.Text = consignacao.NomeCliente ?? "Cliente não informado";
            txtDataConsignacao.Text = consignacao.DataSaida.ToString("dd/MM/yyyy HH:mm");

            // Mostrar data prevista de acerto se houver
            if (consignacao.DataPrevisaoAcerto.HasValue)
            {
                txtObservacao.Text = $"Previsão de acerto: {consignacao.DataPrevisaoAcerto.Value:dd/MM/yyyy}";
            }

            // Atualizar status visual
            AtualizarStatusVisual();

            // Carregar produtos
            MontarListaProdutos();

            // Calcular e exibir total
            AtualizarTotal();

            // Verificar se consignação já foi finalizada/cancelada
            AtualizarBloqueioTela();
        }

        private void AtualizarStatusVisual()
        {
            string statusTexto = consignacao.NomeStatus ?? "";

            switch (consignacao.IdStatus)
            {
                case 1: // Aberto
                    txtStatusConsignacao.Text = "EM ABERTO";
                    txtStatusConsignacao.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E3A8A"));
                    break;

                case 2: // Finalizado
                    txtStatusConsignacao.Text = "FINALIZADA";
                    txtStatusConsignacao.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#166534"));
                    break;

                case 3: // Cancelado
                    txtStatusConsignacao.Text = "CANCELADA";
                    txtStatusConsignacao.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F1D1D"));
                    break;

                default:
                    txtStatusConsignacao.Text = statusTexto.ToUpper();
                    txtStatusConsignacao.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B"));
                    break;
            }
        }

        private void MontarListaProdutos()
        {
            produtosConsignacaoStackPanel.Children.Clear();

            if (consignacao.Itens == null || !consignacao.Itens.Any())
            {
                TextBlock txtVazio = new TextBlock
                {
                    Text = "Nenhum produto consignado",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B")),
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 40, 0, 0)
                };
                produtosConsignacaoStackPanel.Children.Add(txtVazio);
                return;
            }

            foreach (var item in consignacao.Itens)
            {
                Border bordaProduto = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")),
                    Padding = new Thickness(15),
                    Margin = new Thickness(0, 0, 0, 12),
                    CornerRadius = new CornerRadius(12),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E8F0")),
                    BorderThickness = new Thickness(1)
                };

                Grid gridProduto = new Grid();
                gridProduto.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Nome
                gridProduto.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) }); // Enviada
                gridProduto.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) }); // Vendida
                gridProduto.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) }); // Devolvida
                gridProduto.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) }); // Valor Total
                gridProduto.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) }); // Excluir

                // Nome do Produto
                TextBlock txtNomeProduto = new TextBlock
                {
                    Text = item.NomeProduto.ToUpper(),
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F172A")),
                    FontSize = 13,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 15, 0)
                };
                Grid.SetColumn(txtNomeProduto, 0);
                gridProduto.Children.Add(txtNomeProduto);

                // Quantidade Enviada
                StackPanel stackEnviada = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 10, 0)
                };

                TextBlock lblEnviada = new TextBlock
                {
                    Text = "ENVIADA",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B")),
                    FontSize = 9,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 4)
                };

                TextBox txtEnviada = new TextBox
                {
                    Text = item.QuantidadeEnviada.ToString(),
                    Height = 32,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1F5F9")),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0F172A")),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CBD5E1")),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Tag = item,
                    FontWeight = FontWeights.Bold,
                    FontSize = 13
                };

                txtEnviada.TextChanged += QuantidadeEnviada_Changed;

                Style styleTextBox = new Style(typeof(Border));
                styleTextBox.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(6)));
                txtEnviada.Resources.Add(typeof(Border), styleTextBox);

                stackEnviada.Children.Add(lblEnviada);
                stackEnviada.Children.Add(txtEnviada);
                Grid.SetColumn(stackEnviada, 1);
                gridProduto.Children.Add(stackEnviada);

                // Quantidade Vendida
                StackPanel stackVendida = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 10, 0)
                };

                TextBlock lblVendida = new TextBlock
                {
                    Text = "VENDIDA",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#16A34A")),
                    FontSize = 9,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 4)
                };

                TextBox txtVendida = new TextBox
                {
                    Text = item.QuantidadeVendida.ToString(),
                    Height = 32,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0FDF4")),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#166534")),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86EFAC")),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Tag = item,
                    FontWeight = FontWeights.Bold,
                    FontSize = 13
                };

                txtVendida.TextChanged += QuantidadeVendida_Changed;

                Style styleTextBox2 = new Style(typeof(Border));
                styleTextBox2.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(6)));
                txtVendida.Resources.Add(typeof(Border), styleTextBox2);

                stackVendida.Children.Add(lblVendida);
                stackVendida.Children.Add(txtVendida);
                Grid.SetColumn(stackVendida, 2);
                gridProduto.Children.Add(stackVendida);

                // Quantidade Devolvida
                StackPanel stackDevolvida = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 10, 0)
                };

                TextBlock lblDevolvida = new TextBlock
                {
                    Text = "DEVOLVIDA",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626")),
                    FontSize = 9,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 4)
                };

                TextBox txtDevolvida = new TextBox
                {
                    Text = item.QuantidadeDevolvida.ToString(),
                    Height = 32,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF2F2")),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#991B1B")),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FCA5A5")),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Tag = item,
                    FontWeight = FontWeights.Bold,
                    FontSize = 13
                };

                txtDevolvida.TextChanged += QuantidadeDevolvida_Changed;

                Style styleTextBox3 = new Style(typeof(Border));
                styleTextBox3.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(6)));
                txtDevolvida.Resources.Add(typeof(Border), styleTextBox3);

                stackDevolvida.Children.Add(lblDevolvida);
                stackDevolvida.Children.Add(txtDevolvida);
                Grid.SetColumn(stackDevolvida, 3);
                gridProduto.Children.Add(stackDevolvida);

                // Valor Total (Vendida * Preço)
                StackPanel stackValor = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 10, 0)
                };

                TextBlock lblValor = new TextBlock
                {
                    Text = "VALOR TOTAL",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B")),
                    FontSize = 9,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 4)
                };

                decimal valorTotal = item.QuantidadeVendida * item.PrecoUnitarioAcordado;

                TextBlock txtValorTotal = new TextBlock
                {
                    Text = valorTotal.ToString("C2"),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
                    FontSize = 15,
                    FontWeight = FontWeights.Black,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                TextBlock txtPrecoUnit = new TextBlock
                {
                    Text = $"Un: {item.PrecoUnitarioAcordado:C2}",
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8")),
                    FontSize = 9,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 2, 0, 0)
                };

                stackValor.Children.Add(lblValor);
                stackValor.Children.Add(txtValorTotal);
                stackValor.Children.Add(txtPrecoUnit);
                Grid.SetColumn(stackValor, 4);
                gridProduto.Children.Add(stackValor);

                // Botão Excluir
                Button btnExcluir = new Button
                {
                    Content = "✕",
                    Width = 32,
                    Height = 32,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEE2E2")),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626")),
                    FontWeight = FontWeights.Bold,
                    BorderThickness = new Thickness(0),
                    Cursor = Cursors.Hand,
                    Tag = item,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                btnExcluir.Click += ExcluirProduto_Click;

                Style styleBotao = new Style(typeof(Border));
                styleBotao.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(8)));
                btnExcluir.Resources.Add(typeof(Border), styleBotao);

                Grid.SetColumn(btnExcluir, 5);
                gridProduto.Children.Add(btnExcluir);

                bordaProduto.Child = gridProduto;
                produtosConsignacaoStackPanel.Children.Add(bordaProduto);

                // Mostrar alerta se houver inconsistência
                if (item.QuantidadeVendida + item.QuantidadeDevolvida != item.QuantidadeEnviada)
                {
                    Border alertaBorda = new Border
                    {
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEF3C7")),
                        Padding = new Thickness(12, 8, 12, 8),
                        Margin = new Thickness(0, -8, 0, 12),
                        CornerRadius = new CornerRadius(0, 0, 8, 8),
                        BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FDE047")),
                        BorderThickness = new Thickness(1, 0, 1, 1)
                    };

                    int diferenca = item.QuantidadeEnviada - (item.QuantidadeVendida + item.QuantidadeDevolvida);
                    string msgAlerta = diferenca > 0
                        ? $"⚠️ Faltam {diferenca} unidade(s) para fechar (sugestão: devolver {item.SugestaoDevolucao})"
                        : $"⚠️ Total vendido + devolvido ({item.QuantidadeVendida + item.QuantidadeDevolvida}) excede enviado ({item.QuantidadeEnviada})";

                    TextBlock txtAlerta = new TextBlock
                    {
                        Text = msgAlerta,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#854D0E")),
                        FontSize = 11,
                        FontWeight = FontWeights.SemiBold
                    };

                    alertaBorda.Child = txtAlerta;
                    produtosConsignacaoStackPanel.Children.Add(alertaBorda);
                }
            }
        }

        private void AtualizarTotal()
        {
            if (consignacao.Itens == null || !consignacao.Itens.Any())
            {
                consignacaoTotalInfo.Text = "R$ 0,00";

                if (!string.IsNullOrEmpty(consignacao.Observacao))
                {
                    txtObservacao.Text = consignacao.Observacao;
                }
                else
                {
                    txtObservacao.Text = "Nenhum produto adicionado";
                }
                return;
            }

            // Calcular total com base nas quantidades VENDIDAS
            consignacao.ValorTotalEstimado = consignacao.Itens.Sum(i => i.QuantidadeVendida * i.PrecoUnitarioAcordado);
            consignacaoTotalInfo.Text = consignacao.ValorTotalEstimado.ToString("C2");

            int totalEnviado = consignacao.Itens.Sum(i => i.QuantidadeEnviada);
            int totalVendido = consignacao.Itens.Sum(i => i.QuantidadeVendida);
            int totalDevolvido = consignacao.Itens.Sum(i => i.QuantidadeDevolvida);

            if (!string.IsNullOrEmpty(consignacao.Observacao))
            {
                txtObservacao.Text = $"{consignacao.Itens.Count} produto(s) | Enviado: {totalEnviado} | Vendido: {totalVendido} | Devolvido: {totalDevolvido} - {consignacao.Observacao}";
            }
            else
            {
                txtObservacao.Text = $"{consignacao.Itens.Count} produto(s) | Enviado: {totalEnviado} | Vendido: {totalVendido} | Devolvido: {totalDevolvido}";
            }
        }

        private void AtualizarBloqueioTela()
        {
            // Status 1: Aberto (pode editar)
            // Status 2: Finalizado (não pode editar)
            // Status 3: Cancelado (não pode editar)
            bool bloqueado = consignacao.IdStatus != 1;
            btnEstornar.Visibility = Visibility.Visible;
            // Bloquear campos de edição
            foreach (var child in produtosConsignacaoStackPanel.Children)
            {
                if (child is Border b && b.Child is Grid g)
                {
                    foreach (var txt in g.Children.OfType<StackPanel>()
                        .SelectMany(sp => sp.Children.OfType<TextBox>()))
                    {
                        txt.IsReadOnly = bloqueado;
                    }

                    foreach (var btn in g.Children.OfType<Button>())
                        btn.IsEnabled = !bloqueado;
                }
            }

            // Bloquear botões
            btnAdicionarProdutos.IsEnabled = !bloqueado;
            btnFinalizar.IsEnabled = !bloqueado;
            btnEstornar.Visibility = Visibility.Visible;
            if (bloqueado)
            {
                btnAdicionarProdutos.Visibility = Visibility.Collapsed;

                if (consignacao.IdStatus == 2) // Finalizado
                {
                    btnFinalizar.Visibility = Visibility.Collapsed;
                    btnEstornar.Content = "ESTORNAR CONSIGNAÇÃO";
                    btnEstornar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"));
                    btnEstornar.Visibility = Visibility.Visible;
                    btnEstornar.Visibility = Visibility.Visible;
                }
                else if (consignacao.IdStatus == 3) // Cancelado
                {
                    btnFinalizar.Visibility = Visibility.Collapsed;
                    btnEstornar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void QuantidadeEnviada_Changed(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txt && txt.Tag is ConsignacaoItem item)
            {
                if (int.TryParse(txt.Text, out int novaQtd) && novaQtd >= 0)
                {
                    item.QuantidadeEnviada = novaQtd;
                    AtualizarTotal();
                    MontarListaProdutos(); 
                }
            }
        }

        private void QuantidadeVendida_Changed(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txt && txt.Tag is ConsignacaoItem item)
            {
                if (int.TryParse(txt.Text, out int novaQtd) && novaQtd >= 0)
                {
                    item.QuantidadeVendida = novaQtd;

                    if (item.QuantidadeVendida + item.QuantidadeDevolvida > item.QuantidadeEnviada)
                    {
                        item.QuantidadeDevolvida = Math.Max(0, item.QuantidadeEnviada - item.QuantidadeVendida);
                    }

                    AtualizarTotal();
                    MontarListaProdutos();
                }
            }
        }

        private void QuantidadeDevolvida_Changed(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txt && txt.Tag is ConsignacaoItem item)
            {
                if (int.TryParse(txt.Text, out int novaQtd) && novaQtd >= 0)
                {
                    item.QuantidadeDevolvida = novaQtd;

                    // Validar se não excede o enviado
                    if (item.QuantidadeVendida + item.QuantidadeDevolvida > item.QuantidadeEnviada)
                    {
                        item.QuantidadeVendida = Math.Max(0, item.QuantidadeEnviada - item.QuantidadeDevolvida);
                    }

                    AtualizarTotal();
                    MontarListaProdutos();
                }
            }
        }

        private void ExcluirProduto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ConsignacaoItem item)
            {
                var resultado = MessageBox.Show(
                    $"Deseja realmente excluir '{item.NomeProduto}' da consignação?",
                    "Confirmar Exclusão",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.Yes)
                {
                    consignacao.Itens.Remove(item);
                    MontarListaProdutos();
                    AtualizarTotal();
                }
            }
        }

        private void AdicionarProdutos_Click(object sender, RoutedEventArgs e)
        {
            // Aqui você deve abrir uma tela de seleção de produtos
            // Similar ao CardapioCompleto do PedidoInfo
            MessageBox.Show("Funcionalidade de adicionar produtos - Implementar tela de seleção de produtos");

            // Exemplo de como seria:
            // CardapioConsignacao cardapio = new CardapioConsignacao(consignacao);
            // cardapio.Show();
            // this.Close();
        }

        private async void Finalizar_Click(object sender, RoutedEventArgs e)
        {
            if (consignacao.Itens == null || !consignacao.Itens.Any())
            {
                MessageBox.Show("Não é possível finalizar uma consignação sem produtos.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validar se todas as quantidades estão corretas
            bool temInconsistencia = false;
            string mensagemErro = "Os seguintes produtos têm inconsistências:\n\n";

            foreach (var item in consignacao.Itens)
            {
                int totalContabilizado = item.QuantidadeVendida + item.QuantidadeDevolvida;

                if (totalContabilizado != item.QuantidadeEnviada)
                {
                    temInconsistencia = true;
                    mensagemErro += $"• {item.NomeProduto}: Enviado {item.QuantidadeEnviada}, mas vendido + devolvido = {totalContabilizado}\n";
                }
            }

            if (temInconsistencia)
            {
                mensagemErro += "\nAs quantidades vendidas + devolvidas devem ser igual às enviadas.\nDeseja continuar mesmo assim?";

                var resultadoValidacao = MessageBox.Show(
                    mensagemErro,
                    "Inconsistências Detectadas",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultadoValidacao == MessageBoxResult.No)
                {
                    return;
                }
            }

            var resultado = MessageBox.Show(
                "Deseja realmente FINALIZAR esta consignação?\n\n" +
                "Ao finalizar:\n" +
                "• As quantidades VENDIDAS serão baixadas do estoque\n" +
                "• As quantidades DEVOLVIDAS voltarão ao estoque\n" +
                "• Esta ação NÃO pode ser desfeita!\n\n" +
                $"Total a receber: {consignacao.ValorTotalEstimado:C2}",
                "Confirmar Finalização",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    // Atualizar quantidades dos TextBox antes de salvar
                    foreach (var child in produtosConsignacaoStackPanel.Children)
                    {
                        if (child is Border b && b.Child is Grid g)
                        {
                            var stackPanels = g.Children.OfType<StackPanel>();

                            foreach (var stack in stackPanels)
                            {
                                var txtBox = stack.Children.OfType<TextBox>().FirstOrDefault();
                                if (txtBox?.Tag is ConsignacaoItem item)
                                {
                                    var label = stack.Children.OfType<TextBlock>().FirstOrDefault()?.Text;

                                    if (int.TryParse(txtBox.Text, out int qtd))
                                    {
                                        if (label == "ENVIADA")
                                            item.QuantidadeEnviada = qtd;
                                        else if (label == "VENDIDA")
                                            item.QuantidadeVendida = qtd;
                                        else if (label == "DEVOLVIDA")
                                            item.QuantidadeDevolvida = qtd;
                                    }
                                }
                            }
                        }
                    }

                    // Atualizar status para Finalizado
                    consignacao.IdStatus = 2;

                    // Recalcular total
                    consignacao.ValorTotalEstimado = consignacao.Itens.Sum(i => i.QuantidadeVendida * i.PrecoUnitarioAcordado);
                    var sucesso = await consignacaoController.darBaixaConsignacao(consignacao);

                    if(sucesso == "ok")
                    {
                        MessageBox.Show(
                        $"Consignação finalizada com sucesso!\n\n" +
                        $"Total faturado: {consignacao.ValorTotalEstimado:C2}",
                        "Sucesso",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(sucesso);
                    }
                        
                    VoltarTelaAnterior();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao finalizar consignação: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void Estornar_Click(object sender, RoutedEventArgs e)
        {





            var resultado = MessageBox.Show(
                "Deseja realmente CANCELAR esta consignação?\n\nOs produtos NÃO serão baixados do estoque.\n\nEsta ação NÃO pode ser desfeita!",
                "Confirmar Cancelamento",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {


                    consignacao.IdStatus = 3;
                    string resposta = await consignacaoController.estornarBaixaConsignacao(consignacao.Id);

                    if(resposta == "ok")
                    {
                        MessageBox.Show("Consignação cancelada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    else
                    {
                        MessageBox.Show(resposta);
                    }


                    VoltarTelaAnterior();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao cancelar consignação: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show(
                "Deseja cancelar as alterações?",
                "Confirmar Cancelamento",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                VoltarTelaAnterior();
            }
        }

        private void VoltarTelaAnterior()
        {
            TodasConsignacoes todas = new TodasConsignacoes();
            todas.Show();

            this.Close();
        }
    }
}
