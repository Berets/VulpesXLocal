using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class TIPMATPRI : Base
    {
        private string? _tmpcod;
        public string? tmpcod { get => _tmpcod; set { if (_tmpcod != value) { _tmpcod = value; NotifyPropertyChanged(); } } }
        private string? _tmpdes;
        public string? tmpdes { get => _tmpdes; set { if (_tmpdes != value) { _tmpdes = value; NotifyPropertyChanged(); } } }
    }
}
