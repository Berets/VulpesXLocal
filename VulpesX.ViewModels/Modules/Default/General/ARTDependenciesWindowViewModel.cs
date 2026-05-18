using DocumentFormat.OpenXml.Spreadsheet;
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
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.General
{
    public class ARTDependenciesWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ARTDependenciesWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required string ProductID { get; set; }
        public required string ProductDescription { get; set; }
        public string ProductFullDescription => $"{ProductID} {ProductDescription}";
        private ObservableCollection<tab_articolo_composizione>? dependencies { get; set; }
        public ObservableCollection<tab_articolo_composizione>? Dependencies { get => dependencies; set { dependencies = value; NotifyPropertyChanged("Dependencies"); } }


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
                    return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().GetDipendenze(CompanyID, ProductID);
                });

                Dependencies = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public tab_articolo? GetArticle(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(CompanyID, ProductID);
        }
    }
}
