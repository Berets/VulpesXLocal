using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class LD : Base
    {
        private string? _Ldcod;
        public string? Ldcod { get => _Ldcod; set { if (_Ldcod != value) { _Ldcod = value; NotifyPropertyChanged(); } } }
        private string? _Lddes;
        public string? Lddes { get => _Lddes; set { if (_Lddes != value) { _Lddes = value; NotifyPropertyChanged(); } } }
    }
}
