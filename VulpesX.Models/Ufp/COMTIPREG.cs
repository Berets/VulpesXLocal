using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class COMTIPREG : Base
    {
        private string? _Cocodso = null!;
        public string? Cocodso { get => _Cocodso; set { if (_Cocodso != value) { _Cocodso = value; NotifyPropertyChanged(); } } }

        private string? _causcon;
        public string? causcon { get => _causcon; set { if (_causcon != value) { _causcon = value; NotifyPropertyChanged(); } } }

        private short? _cauprco;
        public short? cauprco { get => _cauprco; set { if (_cauprco != value) { _cauprco = value; NotifyPropertyChanged(); } } }

        private short? _corigtot;
        public short? corigtot { get => _corigtot; set { if (_corigtot != value) { _corigtot = value; NotifyPropertyChanged(); } } }

        private string? _cocodes;
        public string? cocodes { get => _cocodes; set { if (_cocodes != value) { _cocodes = value; NotifyPropertyChanged(); } } }
    }
}
