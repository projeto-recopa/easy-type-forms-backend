using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace image_cloud_processor.Models
{
    public class Bloco
    {
        public List<Paragrafo> Paragrafos { get; set; }

        public Bloco()
        {
            Paragrafos = new List<Paragrafo>();
        }
    }
}
