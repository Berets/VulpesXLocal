using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class INCASSO : Base
    {
        private string? _iclcod;
        public string? iclcod { get => _iclcod; set { if (_iclcod != value) { _iclcod = value; NotifyPropertyChanged(); } } }

        private string? _icldes;
        public string? icldes { get => _icldes; set { if (_icldes != value) { _icldes = value; NotifyPropertyChanged(); } } }

        private string? _icltip;
        public string? icltip { get => _icltip; set { if (_icltip != value) { _icltip = value; NotifyPropertyChanged(); } } }

        private byte[]? _rv;
        public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged(); } } }
    }
}
