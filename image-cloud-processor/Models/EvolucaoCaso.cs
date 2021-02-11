using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_cloud_processor.Models
{
    public class EvolucaoCaso
    {
        public bool? Cancelado { get; set; }
        public bool? Ignorado { get; set; }
        public bool? TratamentoDomiciliar { get; set; }
        public bool? InternadoUTI { get; set; }
        public bool? Internado { get; set; }
        public bool? Obito { get; set; }
        public bool? Cura { get; set; }
    }
}
