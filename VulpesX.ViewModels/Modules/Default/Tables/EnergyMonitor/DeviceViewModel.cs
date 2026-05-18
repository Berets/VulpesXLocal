using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Assets;
using VulpesX.DAL.Tables.EnergyMonitor;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.EnergyMonitor
{
    public class DeviceViewModel : Base
    {
        public required string CompanyID { get; set; }
        public DeviceViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
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

        private ObservableCollection<EM_DEVICE>? items;
        public ObservableCollection<EM_DEVICE>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        public async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<IEM_DEVICERepository>().GetList(CompanyID));

            IsBusy = false;

        }

        public ObservableCollection<EM_DEVICE_PERIOD>? GetEM_DEVICE_PERIODs(string DeviceID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IEM_DEVICE_PERIODRepository>().GetList(CompanyID, DeviceID);
        }
    }
}
