using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOCR.Models
{
    public class Image
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Text { get; set; }
        public double Capacity { get; set; }
    }
}
