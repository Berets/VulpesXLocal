using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;
using static VulpesX.Shared.Utilities.TelerikGridService;

namespace VulpesX.ViewModels.Modules.Default.Stores
{
    public class StoreMovementsViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public StoreMovementsViewModel()
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

        public DateTime? Year { get; set; }
        public int PageSize { get; set; } = 20;
        public int TotalCount { get; set; }
        public string? FullTextSearch { get; set; }

        private ObservableCollection<store_movements>? items;
        public ObservableCollection<store_movements>? Items { get => items; set { items = value; NotifyPropertyChanged("Items"); } }

        public async Task Load(int PageRequested, List<GenericIDDescription> Sorts, List<FilterEntry> Wheres)
        {
            IsBusy = true;

            try
            {

                int totalCount = 0;
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().GetList(CompanyID, Year!.Value.Year, false, FullTextSearch ?? string.Empty, PageSize, PageRequested, Sorts, Wheres, out totalCount);
                });

                TotalCount = totalCount;
                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public long GetMovementNumerator()
        {
            var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            return numRegRepository.GetFullLongID(now.Year, numRegRepository.GetNumber(CompanyID, now.Year, Constants.STORE_MOVEMENTS, true));
        }
    }
}
