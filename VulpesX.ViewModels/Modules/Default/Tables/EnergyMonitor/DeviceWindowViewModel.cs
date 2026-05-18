using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Assets;
using VulpesX.DAL.Tables.EnergyMonitor;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.EnergyMonitor
{
    public class DeviceWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public DeviceWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required EM_DEVICE Data { get; set; }
        public bool IsInsert { get; set; }
        public ObservableCollection<EM_DEVICE_PERIOD>? Costs { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IEM_DEVICERepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IEM_DEVICERepository>().Insert(Data, Costs ?? new ObservableCollection<EM_DEVICE_PERIOD>());

            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IEM_DEVICERepository>().Update(Data, Costs ?? new ObservableCollection<EM_DEVICE_PERIOD>());
            }
        }


    }
}
