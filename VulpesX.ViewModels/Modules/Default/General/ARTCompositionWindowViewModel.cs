using CerberoRetrieveAPI;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.DAL.SRM;
using VulpesX.DAL.Tables.Productions;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.General
{
    public class ARTCompositionWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ARTCompositionWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required tab_articolo Data { get; set; }
        public bool IsInsert { get; set; }

        private ObservableCollection<tab_articolo_composizione>? _distinta;
        public ObservableCollection<tab_articolo_composizione>? Distinta
        {
            get { return _distinta; }
            set
            {
                _distinta = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<tab_articolo>? _Revisioni;
        public ObservableCollection<tab_articolo>? Revisioni
        {
            get { return _Revisioni; }
            set
            {
                _Revisioni = value;
                NotifyPropertyChanged("Revisioni");
            }
        }

        private ObservableCollection<tab_produzione_reparto>? _Reparti;
        public ObservableCollection<tab_produzione_reparto>? Reparti
        {
            get { return _Reparti; }
            set
            {
                _Reparti = value;
                NotifyPropertyChanged("Reparti");
            }
        }

        private ObservableCollection<tab_articolo>? _Componenti;
        public ObservableCollection<tab_articolo>? Componenti
        {
            get { return _Componenti; }
            set
            {
                _Componenti = value;
                NotifyPropertyChanged("Componenti");
            }
        }


        private tab_articolo_composizione? _SelectedDistinta;
        public tab_articolo_composizione? SelectedDistinta
        {
            get { return _SelectedDistinta; }
            set
            {
                _SelectedDistinta = value;
                NotifyPropertyChanged("SelectedDistinta");
            }
        }

        #region Costs
        private decimal lastProductCost;
        public decimal LastProductCost { get => lastProductCost; set { lastProductCost = value; NotifyPropertyChanged("LastProductCost"); NotifyPropertyChanged("LastUnitCost"); } }

        private decimal averageProductCost;
        public decimal AverageProductCost { get => averageProductCost; set { averageProductCost = value; NotifyPropertyChanged("AverageProductCost"); NotifyPropertyChanged("AverageUnitCost"); } }

        public decimal LastUnitCost => LastProductCost / (Data.QuantitaDefault ?? 1);
        public decimal AverageUnitCost => AverageProductCost / (Data.QuantitaDefault ?? 1);
        #endregion

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }


        public void LoadDetails()
        {
            Reparti = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_repartoRepository>().GetList(CompanyID);

            var components = new List<tab_articolo>(VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetListComponents(CompanyID, Data.ID) ?? new ObservableCollection<tab_articolo>());
            components.AddRange(VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().GetRevisioni(CompanyID) ?? new ObservableCollection<tab_articolo>());

            Componenti = new ObservableCollection<tab_articolo>(components.Where(o => o.ID != Data.ID).OrderBy(o => o.ID));
        }

        public async Task LoadRevisions()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().GetRevisioni(CompanyID, Data.ID);
                });

                Revisioni = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task Load(string RevisionID)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().Get(CompanyID, Data.ID, RevisionID, null);
                });

                Distinta = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task Load()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().Get(CompanyID, Data.ID, null);
                });

                Distinta = result;
            }
            finally
            {
                IsBusy = false;
            }
        }


        public Tuple<tab_articolo_costi?, decimal> GetCosts(string ProductID, DateTime LimitLastDate, DateTime LimitDate)
        {
            var tabArticoloCostiRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_costiRepository>();

            var lastCosts = tabArticoloCostiRepo.GetLastUpTo(CompanyID, ProductID, LimitLastDate);
            var lastAverage = tabArticoloCostiRepo.GetAverageCostSince(CompanyID, ProductID, LimitLastDate, LimitDate);

            return new Tuple<tab_articolo_costi?, decimal>(lastCosts, lastAverage);
        }

        public ObservableCollection<tab_produzione_risorsa>? GetTab_Produzione_Risorsas(string WardID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().GetListFromReparto(CompanyID, WardID);
        }

        public tab_articolo_composizione? GetComposition(string ProductID, string? RevisionID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().Single(CompanyID, ProductID, RevisionID, null);
        }

        public bool DuplicateRevision(tab_articolo Revision)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().DuplicateRevisione(Revision);
        }

        public bool DeleteRevision(tab_articolo Revision)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().DeleteRevisione(Revision);
        }

        public ObservableCollection<tab_articolo_composizione>? GetCompositions(string ProductID, string RevisionID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().Get(CompanyID, ProductID, RevisionID, null);
        }

        public ObservableCollection<tab_articolo_composizione>? GetDipendenze(string ProductID, string? RevisionID)
        {
            if (string.IsNullOrEmpty(RevisionID))
                return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().GetDipendenze(CompanyID, ProductID);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().GetDipendenze(CompanyID, ProductID, RevisionID);
        }

        public string? Validation()
        {
            string errors = string.Empty;

            bool result = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().IsValid(Distinta!.First(), ref errors);

            return errors;
        }

        public long Save()
        {

            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().Save(CompanyID, Data.ID, Distinta!.First(), 0, Distinta!.First().RevisioneID);

        }
    }
}
