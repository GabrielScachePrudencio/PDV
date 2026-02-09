namespace ServidorLanches.model.dto
{
    public class ItemPedidoCardapioDTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; }
        public string Categoria { get; set; }
        public string pathProdutoImg { get; set; }
        public int Quantidade { get; set; }
        //preco de venda
        public decimal ValorUnitario { get; set; }
        //preco de custo de fabrica
        public decimal CustoDeFabricacao { get; set; }


        public decimal Desconto { get; set; }
        public int DecontoComPorcentagem { get; set; }

    }
}
