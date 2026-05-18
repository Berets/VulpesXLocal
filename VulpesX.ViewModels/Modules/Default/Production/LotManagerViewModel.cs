using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Production;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Reports.Production;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Production
{
    public class LotManagerViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public LotManagerViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        private ObservableCollection<pro_ordine_lotti>? _items;
        public ObservableCollection<pro_ordine_lotti>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }

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
                    return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_lottiRepository>().GetList(CompanyID);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public pro_ordine_lotti? GetFull(string OrderID, string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_lottiRepository>().GetFull(CompanyID, OrderID, ID);
        }

        public FinalLotLabelReport? PrintLabel(pro_ordine_lotti Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_lottiRepository>().PrintLabel(Item);
        }
    }
}
