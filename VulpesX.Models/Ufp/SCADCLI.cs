using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class SCADCLI : Base
    {
        private int _scclicod;
        public int scclicod { get => _scclicod; set { if (_scclicod != value) { _scclicod = value; NotifyPropertyChanged(); } } }

        private short _Scagior;
        public short Scagior { get => _Scagior; set { if (_Scagior != value) { _Scagior = value; NotifyPropertyChanged(); } } }

        private string? _Scnote = null!;
        public string? Scnote { get => _Scnote; set { if (_Scnote != value) { _Scnote = value; NotifyPropertyChanged(); } } }

        private string? _descli = null!;
        public string? descli { get => _descli; set { if (_descli != value) { _descli = value; NotifyPropertyChanged(); } } }
    }
}
