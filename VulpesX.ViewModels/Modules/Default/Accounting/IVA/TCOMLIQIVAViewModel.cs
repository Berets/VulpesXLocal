using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.CustomerRating;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.IVA
{
    public class TCOMLIQIVAViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public TCOMLIQIVAViewModel()
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

        private ObservableCollection<TCOMLIQIVA>? items;
        public ObservableCollection<TCOMLIQIVA>? Items { get { return items; } set { items = value; NotifyPropertyChanged("Items"); } }

        public async Task Load(int Year)
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<ITCOMLIQIVARepository>().GetList(CompanyID, Year));

            IsBusy = false;

        }

        public ObservableCollection<TCOMLIQIVA>? GetListDetails(int Year, int Month)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCOMLIQIVARepository>().GetListDetails(CompanyID, Year, Month);

        }

        public bool Delete(TCOMLIQIVA Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCOMLIQIVARepository>().Delete(Item);
        }
    }
}
