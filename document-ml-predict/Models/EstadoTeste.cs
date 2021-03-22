using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_cloud_processor.Models
{
    public class EstadoTeste
    {
        public bool? Solicitado { get; set; }
        public bool? Coletado { get; set; }
        public bool? Concluido { get; set; }
        public bool? NaoSolicitado{ get; set; }
    }
}
