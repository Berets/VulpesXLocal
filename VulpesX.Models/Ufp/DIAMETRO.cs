using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class DIAMETRO : Base
    {
        private string? _Diamcod;
        public string? Diamcod { get => _Diamcod; set { if (_Diamcod != value) { _Diamcod = value; NotifyPropertyChanged(); } } }
        private string? _diamdes;
        public string? diamdes { get => _diamdes; set { if (_diamdes != value) { _diamdes = value; NotifyPropertyChanged(); } } }
    }
}
