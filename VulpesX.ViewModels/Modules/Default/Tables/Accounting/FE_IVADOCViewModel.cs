using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class FE_IVADOCViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public FE_IVADOCViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID!;
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

        private ObservableCollection<FE_IVADOC>? items;
        public ObservableCollection<FE_IVADOC>? Items
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
                       Items = VulpesServiceProvider.Provider.GetRequiredService<IFE_IVADOCRepository>().GetList());

            IsBusy = false;

        }

        public bool Delete(FE_IVADOC Item, string SelectedReason)
        {
            Item.canceled = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
            Item.canceledUserID = UserID;
            Item.canceledNote = SelectedReason;

            return VulpesServiceProvider.Provider.GetRequiredService<IFE_IVADOCRepository>().Update(Item);
        }
    }
}
