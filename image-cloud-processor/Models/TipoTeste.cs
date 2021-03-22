using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_cloud_processor.Models
{
    public class TipoTeste
    {
        public bool? RTPCR { get; set; }
        public bool? TesteRapidoAnticorpo { get; set; }
        public bool? TesteRapidoAntigeno { get; set; }
        public bool? ELISA { get; set; }
        public bool? ECLIA { get; set; }
    }
}
