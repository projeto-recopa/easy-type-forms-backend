using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_cloud_processor.Models
{
    public class Sintomas
    {
        public bool? Febre { get; set; }
        public bool? Tosse { get; set; }
        public bool? DorGarganta { get; set; }
        public bool? Dispneia { get; set; }
        public bool? Outros { get; set; }
    }
}
