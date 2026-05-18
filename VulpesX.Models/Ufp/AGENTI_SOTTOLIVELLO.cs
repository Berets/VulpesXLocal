using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class AGENTI_SOTTOLIVELLO : Base
    {
        private string? _agecod = null!;
        public string? agecod { get => _agecod; set { if (_agecod != value) { _agecod = value; NotifyPropertyChanged(); } } }

        private int _agecli;
        public int agecli { get => _agecli; set { if (_agecli != value) { _agecli = value; NotifyPropertyChanged(); } } }

        private string? _ageart = null!;
        public string? ageart { get => _ageart; set { if (_ageart != value) { _ageart = value; NotifyPropertyChanged(); } } }

        private decimal? _ageperc;
        public decimal? ageperc { get => _ageperc; set { if (_ageperc != value) { _ageperc = value; NotifyPropertyChanged(); } } }

        private string? _ageclid = null!;
        public string? ageclid { get => _ageclid; set { if (_ageclid != value) { _ageclid = value; NotifyPropertyChanged(); } } }

        private string? _ageartd = null!;
        public string? ageartd { get => _ageartd; set { if (_ageartd != value) { _ageartd = value; NotifyPropertyChanged(); } } }
    }
}
