using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Assets;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Assets
{
    public class ASSET00FViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ASSET00FViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        private ObservableCollection<ASSET00F>? _items;
        public ObservableCollection<ASSET00F>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }

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
                    return VulpesServiceProvider.Provider.GetRequiredService<IASSET00FRepository>().GetList(CompanyID);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public ObservableCollection<ASSETCO00F>? GetASSETCO00Fs(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IASSETCO00FRepository>().GetList(CompanyID, ID);
        }

        public ObservableCollection<ASSETAL00F>? GetASSETAL00Fs(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IASSETAL00FRepository>().GetList(CompanyID, ID);
        }

        public bool Update(ASSET00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IASSET00FRepository>().Update(Item);
        }
    }
}

