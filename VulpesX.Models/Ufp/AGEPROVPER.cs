using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class AGEPROVPER : Base
    {
        private string? _appcod = null!;
        public string? appcod { get => _appcod; set { if (_appcod != value) { _appcod = value; NotifyPropertyChanged(); } } }

        private int _appclie;
        public int appclie { get => _appclie; set { if (_appclie != value) { _appclie = value; NotifyPropertyChanged(); } } }

        private string? _appcau = null!;
        public string? appcau { get => _appcau; set { if (_appcau != value) { _appcau = value; NotifyPropertyChanged(); } } }

        private int _appfor;
        public int appfor { get => _appfor; set { if (_appfor != value) { _appfor = value; NotifyPropertyChanged(); } } }

        private decimal? _appsca1;
        public decimal? appsca1 { get => _appsca1; set { if (_appsca1 != value) { _appsca1 = value; NotifyPropertyChanged(); } } }

        private decimal? _appsca2;
        public decimal? appsca2 { get => _appsca2; set { if (_appsca2 != value) { _appsca2 = value; NotifyPropertyChanged(); } } }

        private decimal? _appsca3;
        public decimal? appsca3 { get => _appsca3; set { if (_appsca3 != value) { _appsca3 = value; NotifyPropertyChanged(); } } }

        private decimal? _appscaper1;
        public decimal? appscaper1 { get => _appscaper1; set { if (_appscaper1 != value) { _appscaper1 = value; NotifyPropertyChanged(); } } }

        private decimal? _appscaper2;
        public decimal? appscaper2 { get => _appscaper2; set { if (_appscaper2 != value) { _appscaper2 = value; NotifyPropertyChanged(); } } }

        private decimal? _appscaper3;
        public decimal? appscaper3 { get => _appscaper3; set { if (_appscaper3 != value) { _appscaper3 = value; NotifyPropertyChanged(); } } }

        private decimal? _appper1;
        public decimal? appper1 { get => _appper1; set { if (_appper1 != value) { _appper1 = value; NotifyPropertyChanged(); } } }

        private decimal? _appper2;
        public decimal? appper2 { get => _appper2; set { if (_appper2 != value) { _appper2 = value; NotifyPropertyChanged(); } } }
    }
}
