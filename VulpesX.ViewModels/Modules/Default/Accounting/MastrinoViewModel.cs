using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class MastrinoViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public MastrinoViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
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

        public string? CurrencyText { get; set; }

        private ObservableCollection<PDCANNI>? items;
        public ObservableCollection<PDCANNI>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        public async Task Load(int Year)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().GetListByYear(CompanyID, Year);

                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetList(CompanyID);
        }

        public void GetCurrency()
        {
            CurrencyText = VulpesServiceProvider.Provider.GetRequiredService<IVALUTERepository>().Get("UIC", UserContext.Instance!.ACCESS!.SelectedCompany!.SOMF02 ?? string.Empty)?.VALDES?.Trim();
        }
    }
}
