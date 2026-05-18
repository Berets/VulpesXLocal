using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class FORILUBRIFICATI : Base
    {
        private string? _FLcod;
        public string? FLcod { get => _FLcod; set { if (_FLcod != value) { _FLcod = value; NotifyPropertyChanged(); } } }
        private string? _FLdes;
        public string? FLdes { get => _FLdes; set { if (_FLdes != value) { _FLdes = value; NotifyPropertyChanged(); } } }
    }
}
