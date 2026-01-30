using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDV_LANCHES.model
{
    public class ResultadoApi
    {
        public bool Sucesso { get; set; }
        public int StatusCode { get; set; }
        public string Mensagem { get; set; }
    }
}
