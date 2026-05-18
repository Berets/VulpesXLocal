using DocumentFormat.OpenXml.Spreadsheet;
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
using VulpesX.Models.Models.Production;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Production
{
    public class LotManagerTrackingWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public LotManagerTrackingWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required pro_ordine_lotti Data { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        private FullLotTracking? _lotTracking;
        public FullLotTracking? LotTracking
        {
            get { return _lotTracking; }
            set { _lotTracking = value; NotifyPropertyChanged(); }
        }

        public async Task Load()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_lottiRepository>().Track(Data);
                });

                LotTracking = result;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
