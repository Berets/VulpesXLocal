using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Reports;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.General
{
    public class ARTViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ARTViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        private ObservableCollection<tab_articolo>? _items;
        public ObservableCollection<tab_articolo>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        public async Task Load()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetList(CompanyID);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public ProductLabelReport? GetPrintProductLabel(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().PrintProductLabel(CompanyID, ProductID);
        }

        public ObservableCollection<tab_articolo_immagine>? GetImmagini(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetImmagini(CompanyID, ProductID);
        }

        public ObservableCollection<tab_articolo_allegato>? GetAllegati(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetAllegati(CompanyID, ProductID);
        }

        public ObservableCollection<tab_articolo_composizione>? GetComposizione(string ProductID, string RevisionID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().Get(CompanyID, ProductID,RevisionID,null);
        }
    }
}
