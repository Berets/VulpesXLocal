using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class DENTI : Base
    {
        private string? _Dencod;
        public string? Dencod { get => _Dencod; set { if (_Dencod != value) { _Dencod = value; NotifyPropertyChanged(); } } }
        private string? _Dendes;
        public string? Dendes { get => _Dendes; set { if (_Dendes != value) { _Dendes = value; NotifyPropertyChanged(); } } }
    }
}
