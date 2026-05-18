using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Functions
{
    public class EqualizationWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public EqualizationWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
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

        public async Task<bool> RunEqualization(string EntityType, DateTime UntilDate)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>().RunEqualization(CompanyID, EntityType, UntilDate);
                });

                IsBusy = false;

                return result;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
