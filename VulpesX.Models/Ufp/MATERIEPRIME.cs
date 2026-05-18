using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class MATERIEPRIME : Base
    {
        private string? _matpcod;
        public string? matpcod { get => _matpcod; set { if (_matpcod != value) { _matpcod = value; NotifyPropertyChanged(); } } }
        private string? _matpdes;
        public string? matpdes { get => _matpdes; set { if (_matpdes != value) { _matpdes = value; NotifyPropertyChanged(); } } }
    }
}
