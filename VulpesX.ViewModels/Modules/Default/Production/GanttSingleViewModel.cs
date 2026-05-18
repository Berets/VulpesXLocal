using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Production
{
    public class GanttSingleViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public GanttSingleViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                _IsBusy = value;
                NotifyPropertyChanged("IsBusy");
                NotifyPropertyChanged("IsVisible");
                NotifyPropertyChanged("IsEnabledLoading");
            }
        }

        public bool IsEnabledLoading
        {
            get { return (IsBusy) ? false : true; }
        }

        public bool IsVisible
        {
            get { return (IsBusy) ? false : true; }
        }

        private ObservableCollection<pro_ordine_composizione>? _Tasks;
        public ObservableCollection<pro_ordine_composizione>? Tasks
        {
            get { return _Tasks; }
            set
            {
                _Tasks = value;
                NotifyPropertyChanged("Tasks");
            }
        }


        private ObservableCollection<tab_produzione_calendario_chiusura>? _Stops;
        public ObservableCollection<tab_produzione_calendario_chiusura>? Stops
        {
            get { return _Stops; }
            set
            {
                _Stops = value;
                NotifyPropertyChanged("Stops");
            }
        }


        public required tab_produzione_risorsa Risorsa { get; set; }

        public pro_ordine_composizione? SelectedTask { get; set; }

        private DateTime? _VisibleStartTime;
        public DateTime? VisibleStartTime
        {
            get { return _VisibleStartTime; }
            set
            {
                _VisibleStartTime = value;
                NotifyPropertyChanged("VisibleStartTime");
            }
        }

        private DateTime? _VisibleEndTime;
        public DateTime? VisibleEndTime
        {
            get { return _VisibleEndTime; }
            set
            {
                _VisibleEndTime = value;
                NotifyPropertyChanged("VisibleEndTime");
            }
        }

        private TimeSpan _PixelLength;
        public TimeSpan PixelLength
        {
            get { return _PixelLength; }
            set
            {
                _PixelLength = value;
                NotifyPropertyChanged("PixelLength");
            }
        }


        private string? _OccupazioneDisponibile;
        public string? OccupazioneDisponibile
        {
            get { return _OccupazioneDisponibile; }
            set
            {
                _OccupazioneDisponibile = value;
                NotifyPropertyChanged("OccupazioneDisponibile");
            }
        }

        private string? _OccupazioneImpegni;
        public string? OccupazioneImpegni
        {
            get { return _OccupazioneImpegni; }
            set
            {
                _OccupazioneImpegni = value;
                NotifyPropertyChanged("OccupazioneImpegni");
            }
        }

        private string? _OccupazioneImpegniPercentuale;
        public string? OccupazioneImpegniPercentuale
        {
            get { return _OccupazioneImpegniPercentuale; }
            set
            {
                _OccupazioneImpegniPercentuale = value;
                NotifyPropertyChanged("OccupazioneImpegniPercentuale");
            }
        }

        private string? _OccupazioneResidua;
        public string? OccupazioneResidua
        {
            get { return _OccupazioneResidua; }
            set
            {
                _OccupazioneResidua = value;
                NotifyPropertyChanged("OccupazioneResidua");
            }
        }
    }
}
