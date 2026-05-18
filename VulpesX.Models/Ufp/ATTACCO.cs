using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class ATTACCO : Base
    {
        private string? _attacod;
        public string? attacod { get => _attacod; set { if (_attacod != value) { _attacod = value; NotifyPropertyChanged(); } } }
        private string? _attades;
        public string? attades { get => _attades; set { if (_attades != value) { _attades = value; NotifyPropertyChanged(); } } }
    }
}
