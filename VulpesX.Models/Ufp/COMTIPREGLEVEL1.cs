using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class COMTIPREGLEVEL1 : Base
    {
        private string? _Cocodso = null!;
        public string? Cocodso { get => _Cocodso; set { if (_Cocodso != value) { _Cocodso = value; NotifyPropertyChanged(); } } }

        private string? _causcon;
        public string? causcon { get => _causcon; set { if (_causcon != value) { _causcon = value; NotifyPropertyChanged(); } } }

        private short? _cauprco;
        public short? cauprco { get => _cauprco; set { if (_cauprco != value) { _cauprco = value; NotifyPropertyChanged(); } } }

        private short? _Conriga;
        public short? Conriga { get => _Conriga; set { if (_Conriga != value) { _Conriga = value; NotifyPropertyChanged(); } } }

        private string? _Cosegno;
        public string? Cosegno { get => _Cosegno; set { if (_Cosegno != value) { _Cosegno = value; NotifyPropertyChanged(); } } }

        private string? _Cogrup;
        public string? Cogrup { get => _Cogrup; set { if (_Cogrup != value) { _Cogrup = value; NotifyPropertyChanged(); } } }

        private string? _Cocont;
        public string? Cocont { get => _Cocont; set { if (_Cocont != value) { _Cocont = value; NotifyPropertyChanged(); } } }

        private string? _CoSotc;
        public string? CoSotc { get => _CoSotc; set { if (_CoSotc != value) { _CoSotc = value; NotifyPropertyChanged(); } } }

        private string? _condesc;
        public string? condesc { get => _condesc; set { if (_condesc != value) { _condesc = value; NotifyPropertyChanged(); } } }
    }
}
