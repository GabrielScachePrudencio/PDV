namespace PDV_LANCHES.model
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int IdCategoria { get; set; }

        public decimal Valor { get; set; }
        public bool Disponivel { get; set; }
        public string pathImg { get; set; }


        //nao ta no banco
        public int QuantidadeSelecionada { get; set; } = 1;
        public string NomeCategoria { get; set; }

    }

}
