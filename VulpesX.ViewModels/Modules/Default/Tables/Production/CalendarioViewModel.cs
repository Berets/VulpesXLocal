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
    public class CalendarioViewModel : Base
    {
        public required string CompanyID { get; set; }
        public CalendarioViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;

            Risorse = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().GetList(CompanyID);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<tab_produzione_risorsa>? _Risorse;
        public ObservableCollection<tab_produzione_risorsa>? Risorse
        {
            get { return _Risorse; }
            set
            {
                _Risorse = value;
                NotifyPropertyChanged("Risorse");
            }
        }

        private tab_produzione_risorsa? _SelectedRisorsa;
        public tab_produzione_risorsa? SelectedRisorsa
        {
            get { return _SelectedRisorsa; }
            set
            {
                _SelectedRisorsa = value;
                NotifyPropertyChanged("SelectedRisorsa");
            }
        }

        private IEnumerable<AppointmentModel>? _Calendario;
        public IEnumerable<AppointmentModel>? Calendario
        {
            get { return _Calendario; }
            set
            {
                _Calendario = value;
                NotifyPropertyChanged("Calendario");
            }
        }

        public async Task GetAppointmentModels(DateTime From, DateTime To)
        {
            IsBusy = true;

            await Task.Run(() =>
            {
                if (SelectedRisorsa != null)
                {
                    var periods = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsa_calendarioRepository>().GetPeriod(CompanyID, SelectedRisorsa.ID, From, To);

                    if (periods != null)
                    {
                        Calendario = new ObservableCollection<AppointmentModel>(periods.Select(s => new AppointmentModel
                        {
                            Start = s.Dalle,
                            End = s.Alle,
                            Subject = s.Tipo == Constants._CALENDARIO_OPEN ? $"{s.Dalle.ToString("HH:mm")} : {s.Alle.ToString("HH:mm")}" : "Chiuso",
                            Body = s.Tipo == Constants._CALENDARIO_OPEN ? $"{s.Dalle.ToString("HH:mm")} : {s.Alle.ToString("HH:mm")}" : "Chiuso",
                            Importance = s.Tipo == Constants._CALENDARIO_CLOSE ? true : false
                        }));
                    }
                }
            });

            IsBusy = false;

        }
    }
}
