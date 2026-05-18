using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Productions;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Production
{
    public class CausaliViewModel : Base
    {
        public required string CompanyID { get; set; }
        public CausaliViewModel()
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

        private ObservableCollection<tab_produzione_causale>? items;
        public ObservableCollection<tab_produzione_causale>? Items
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
                       Items = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_causaleRepository>().GetList(CompanyID));

            IsBusy = false;

        }

        public bool Delete(tab_produzione_causale Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_causaleRepository>().Delete(Item);
        }
    }
}
