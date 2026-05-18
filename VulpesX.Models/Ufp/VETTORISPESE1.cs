using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class VETTORISPESE1 : Base
    {
        private int _vetcod;
        public int vetcod { get => _vetcod; set { if (_vetcod != value) { _vetcod = value; NotifyPropertyChanged(); } } }

        private string? _vetiso;
        public string? vetiso { get => _vetiso; set { if (_vetiso != value) { _vetiso = value; NotifyPropertyChanged(); } } }

        private decimal? _Vetapes;
        public decimal? Vetapes { get => _Vetapes; set { if (_Vetapes != value) { _Vetapes = value; NotifyPropertyChanged(); } } }

        private decimal? _vetpes;
        public decimal? vetpes { get => _vetpes; set { if (_vetpes != value) { _vetpes = value; NotifyPropertyChanged(); } } }

        private decimal? _vetpre;
        public decimal? vetpre { get => _vetpre; set { if (_vetpre != value) { _vetpre = value; NotifyPropertyChanged(); } } }

        private decimal? _vetcos;
        public decimal? vetcos { get => _vetcos; set { if (_vetcos != value) { _vetcos = value; NotifyPropertyChanged(); } } }

    }
}
