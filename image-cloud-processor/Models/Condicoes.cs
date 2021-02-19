using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_cloud_processor.Models
{
    public class Condicoes
    {
        public bool? DoencasRespiratorias { get; set; }
        public bool? DoencasRenais { get; set; }
        public bool? DoencasCromossomicas { get; set; }
        public bool? DoencasCardiacas { get; set; }
        public bool? Diabetes { get; set; }
        public bool? Gestantes { get; set; }
        public bool? Imunossupressao { get; set; }
    }
}
