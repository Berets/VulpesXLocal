using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Productions;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Tables.Production;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Production
{
    public class CalendarioWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public CalendarioWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            Risorse = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().GetList(CompanyID);
        }

        private bool _IsBusy;
        public bool IsBusy { get { return _IsBusy; } set { _IsBusy = value; NotifyPropertyChanged("IsBusy"); } }

        private DateTime? _Dal;
        public DateTime? Dal { get { return _Dal; } set { _Dal = value; NotifyPropertyChanged("Dal"); } }

        private DateTime? _Al;
        public DateTime? Al { get { return _Al; } set { _Al = value; NotifyPropertyChanged("Al"); } }

        public ObservableCollection<tab_produzione_risorsa>? Risorse { get; set; }
        public ObservableCollection<tab_produzione_risorsa>? SelectedRisorse { get; set; }

        private bool _EUsaComeDefault;
        public bool EUsaComeDefault { get { return _EUsaComeDefault; } set { _EUsaComeDefault = value; NotifyPropertyChanged("EUsaComeDefault"); } }

        private bool _ELunediAperto;
        public bool ELunediAperto { get { return _ELunediAperto; } set { _ELunediAperto = value; NotifyPropertyChanged("ELunediAperto"); } }

        private ObservableCollection<GeneraCalendarioDayModel>? _Lunedi;
        public ObservableCollection<GeneraCalendarioDayModel>? Lunedi { get { return _Lunedi; } set { _Lunedi = value; NotifyPropertyChanged("Lunedi"); } }

        private bool _EMartediAperto;
        public bool EMartediAperto { get { return _EMartediAperto; } set { _EMartediAperto = value; NotifyPropertyChanged("EMartediAperto"); } }

        private ObservableCollection<GeneraCalendarioDayModel>? _Martedi;
        public ObservableCollection<GeneraCalendarioDayModel>? Martedi { get { return _Martedi; } set { _Martedi = value; NotifyPropertyChanged("Martedi"); } }

        private bool _EMercolediAperto;
        public bool EMercolediAperto { get { return _EMercolediAperto; } set { _EMercolediAperto = value; NotifyPropertyChanged("EMercolediAperto"); } }

        private ObservableCollection<GeneraCalendarioDayModel>? _Mercoledi;
        public ObservableCollection<GeneraCalendarioDayModel>? Mercoledi { get { return _Mercoledi; } set { _Mercoledi = value; NotifyPropertyChanged("Mercoledi"); } }

        private bool _EGiovediAperto;
        public bool EGiovediAperto { get { return _EGiovediAperto; } set { _EGiovediAperto = value; NotifyPropertyChanged("EGiovediAperto"); } }

        private ObservableCollection<GeneraCalendarioDayModel>? _Giovedi;
        public ObservableCollection<GeneraCalendarioDayModel>? Giovedi { get { return _Giovedi; } set { _Giovedi = value; NotifyPropertyChanged("Giovedi"); } }

        private bool _EVenerdiAperto;
        public bool EVenerdiAperto { get { return _EVenerdiAperto; } set { _EVenerdiAperto = value; NotifyPropertyChanged("EVenerdiAperto"); } }

        private ObservableCollection<GeneraCalendarioDayModel>? _Venerdi;
        public ObservableCollection<GeneraCalendarioDayModel>? Venerdi { get { return _Venerdi; } set { _Venerdi = value; NotifyPropertyChanged("Venerdi"); } }

        private bool _ESabatoAperto;
        public bool ESabatoAperto { get { return _ESabatoAperto; } set { _ESabatoAperto = value; NotifyPropertyChanged("ESabatoAperto"); } }

        private ObservableCollection<GeneraCalendarioDayModel>? _Sabato;
        public ObservableCollection<GeneraCalendarioDayModel>? Sabato { get { return _Sabato; } set { _Sabato = value; NotifyPropertyChanged("Sabato"); } }

        private bool _EDomenicaAperto;
        public bool EDomenicaAperto { get { return _EDomenicaAperto; } set { _EDomenicaAperto = value; NotifyPropertyChanged("EDomenicaAperto"); } }

        private ObservableCollection<GeneraCalendarioDayModel>? _Domenica;
        public ObservableCollection<GeneraCalendarioDayModel>? Domenica { get { return _Domenica; } set { _Domenica = value; NotifyPropertyChanged("Domenica"); } }

        public bool Save(ObservableCollection<tab_produzione_risorsa_calendario> Save, ObservableCollection<tab_produzione_calendario_chiusura> Stops)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsa_calendarioRepository>().Generate(Save, Stops);
        }
    }
}
