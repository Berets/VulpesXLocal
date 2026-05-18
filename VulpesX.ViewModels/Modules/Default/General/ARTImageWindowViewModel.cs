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
    public class ARTImageWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ARTImageWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required tab_articolo Data { get; set; }

        private ObservableCollection<tab_articolo_immagine>? _items;
        public ObservableCollection<tab_articolo_immagine>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }

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
                    return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetImmagini(CompanyID, Data.ID);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool Insert(tab_articolo_immagine Image)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().InsertImmagine(Image);
        }

        public bool Update(tab_articolo_immagine Image)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().UpdateImmagine(Image);
        }

        public bool Delete(tab_articolo_immagine Image)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().DeleteImmagine(Image);
        }
    }
}
