using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class ANACERTLEVEL1 : Base
    {
        private int _anacli;
        public int anacli { get => _anacli; set { if (_anacli != value) { _anacli = value; NotifyPropertyChanged(); } } }
        private short _anapro;
        public short anapro { get => _anapro; set { if (_anapro != value) { _anapro = value; NotifyPropertyChanged(); } } }
        private string? _anadescer = null!;
        public string? anadescer { get => _anadescer; set { if (_anadescer != value) { _anadescer = value; NotifyPropertyChanged(); } } }
        private string? _anaper;
        public string? anaper { get => _anaper; set { if (_anaper != value) { _anaper = value; NotifyPropertyChanged(); } } }

        private byte[]? _rv;
        public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged(); } } }
    }
}
