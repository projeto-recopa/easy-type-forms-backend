using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_cloud_processor.Models
{
    public class ClassificacaoFinal
    {
        public bool? Descartado { get; set; }
        public bool? CofirmadoClinicoImagem { get; set; }
        public bool? CofirmadoClinicoEpidemiologico { get; set; }
        public bool? CofirmadoCriterioClinico { get; set; }
        public bool? CofirmadoLaboratorial { get; set; }
        public bool? SindromeGripal { get; set; }
    }
}
