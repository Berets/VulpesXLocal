using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class ABICABViewModel : Base
    {
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

        private int? abiFilter;
        public int? AbiFilter
        {
            get => abiFilter; set
            {
                abiFilter = value;
                NotifyPropertyChanged();
            }
        }

        private int? cabFilter;
        public int? CabFilter
        {
            get => cabFilter; set
            {
                cabFilter = value;
                NotifyPropertyChanged();
            }
        }

        private string? textFilter;
        public string? TextFilter
        {
            get => textFilter; set
            {
                textFilter = value;
                NotifyPropertyChanged();
            }
        }

        private int pageSize;
        public int PageSize
        {
            get => pageSize; set
            {
                pageSize = value;
                NotifyPropertyChanged();
            }
        }

        private int pageRequested;
        public int PageRequested
        {
            get => pageRequested; set
            {
                pageRequested = value;
                NotifyPropertyChanged();
            }
        }

        private int totalCount;
        public int TotalCount
        {
            get => totalCount; set
            {
                totalCount = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<ABICAB>? items;
        public ObservableCollection<ABICAB>? Items
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

            try
            {
                var result = await Task.Run(() =>
                {
                    int itemsCount = 0;
                    var items = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetList(AbiFilter, CabFilter, TextFilter, PageSize, PageRequested, out itemsCount);
                    return  new { items, itemsCount };
                });

                Items = result.items;
                TotalCount = result.itemsCount;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
