using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDV_LANCHES.model
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }

        // nunca expor senha fora da API
        public string Senha { get; set; }

        public DateTime DataCriacao { get; set; }

        public TipoUsuario TipoUsuario { get; set; }
    }
}
