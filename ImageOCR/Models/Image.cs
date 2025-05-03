using ImageOCR.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageOCR.Models
{
    public class Image : BaseModel
    {
        private string _name;
        private string _path;
        private string _textEng;
        private string _textVi;
        private double _capacity;

        public Image() { }
        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Path { get => _path; set => SetProperty(ref _path, value); }
        public string TextEng { get => _textEng; set => SetProperty(ref _textEng, value); }
        public string TextVi { get => _textVi; set => SetProperty(ref _textVi, value); }
        public double Capacity { get => _capacity; set => SetProperty(ref _capacity, value); }
    }
}
