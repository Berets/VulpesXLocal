using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Production;
using VulpesX.DAL.Tables.Productions;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Production
{
    public class GanttViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public GanttViewModel()
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

        private ObservableCollection<pro_ordine_composizione>? _TasksResources;
        public ObservableCollection<pro_ordine_composizione>? TasksResources
        {
            get { return _TasksResources; }
            set
            {
                _TasksResources = value;
                NotifyPropertyChanged("TasksResources");
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

        public ObservableCollection<pro_ordine_composizione>? ExpandedTasks { get; set; }

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


        public pro_ordine_composizione? SelectedTask { get; set; }

        private DateTime _VisibleStartTime;
        public DateTime VisibleStartTime
        {
            get { return _VisibleStartTime; }
            set
            {
                _VisibleStartTime = value;
                NotifyPropertyChanged("VisibleStartTime");
            }
        }

        private DateTime _VisibleEndTime;
        public DateTime VisibleEndTime
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

        public async Task Load()
        {
            IsBusy = true;

            try
            {
                Stops = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_calendario_chiusuraRepository>().GetPeriodo(CompanyID, VisibleStartTime, VisibleEndTime);

                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_composizioneRepository>().GetGantt(CompanyID, null, Stops ?? new());
                });

                Tasks = result?.Item1;
                TasksResources = result?.Item2;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public ObservableCollection<tab_produzione_risorsa>? GetTab_Produzione_Risorsas()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().GetList(CompanyID);
        }

        public ObservableCollection<tab_produzione_risorsa>? GetListFullInfo()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().GetListFullInfo(CompanyID);
        }

        public ObservableCollection<tab_produzione_calendario_chiusura>? GetTab_Produzione_Calendario_Chiusuras(string ResourceID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_calendario_chiusuraRepository>().GetRisorsaPeriodo(CompanyID, ResourceID, DateTime.Now, DateTime.Now.AddYears(1));
        }
    }
}
