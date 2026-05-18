using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class RIVESTIMENTI : Base
    {
        private string? _rivecod;
        public string? rivecod { get => _rivecod; set { if (_rivecod != value) { _rivecod = value; NotifyPropertyChanged(); } } }
        private string? _rivedes;
        public string? rivedes { get => _rivedes; set { if (_rivedes != value) { _rivedes = value; NotifyPropertyChanged(); } } }
    }
}
