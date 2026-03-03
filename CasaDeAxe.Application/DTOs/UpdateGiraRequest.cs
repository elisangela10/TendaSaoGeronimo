using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasaDeAxe.Application.DTOs
{
    public class UpdateGiraRequest
    {
        public string Descricao { get; set; }
        public string Cura { get; set; }
        public string Responsavel { get; set; }
        public DateTime DataHora { get; set; }
        public int Status { get; set; } // Ativo / Inativo / Deletado
    }
}
