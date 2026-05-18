using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class ANALOGIE : Base
    {
        private string? _angcod;
        public string? angcod { get => _angcod; set { if (_angcod != value) { _angcod = value; NotifyPropertyChanged(); } } }
        private string? _angdesc;
        public string? angdesc { get => _angdesc; set { if (_angdesc != value) { _angdesc = value; NotifyPropertyChanged(); } } }
    }
}
