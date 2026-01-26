using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDV_LANCHES.model
{
    public class TipoStatusPedido
    {
        public int id { get; set; }
        public string nome { get; set; }

        public bool ativo { get; set; }
        public override string ToString()
        {
            return nome; 
        }

    }


}
