using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDV_LANCHES.model
{
    public class Pedido
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string CpfCliente { get; set; }
        public string StatusPedido { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataEntrega { get; set; }
        public string pahCardapioImg { get; set; }
        public List<ItemPedido> Itens { get; set; }

    }
}
