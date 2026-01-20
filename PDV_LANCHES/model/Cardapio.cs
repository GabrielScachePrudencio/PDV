namespace PDV_LANCHES.model
{
    public class Cardapio
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public decimal Valor { get; set; }
        public bool Disponivel { get; set; }

        public int QuantidadeSelecionada { get; set; } = 1;

        public string pathImg { get; set; }
    }

}
