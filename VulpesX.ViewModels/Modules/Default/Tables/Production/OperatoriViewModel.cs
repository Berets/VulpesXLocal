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
using VulpesX.Models.Reports.Production;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Production
{
    public class OperatoriViewModel : Base
    {
        public required string CompanyID { get; set; }
        public OperatoriViewModel()
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

        private ObservableCollection<tab_produzione_operatore>? items;
        public ObservableCollection<tab_produzione_operatore>? Items
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
                       Items = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_operatoreRepository>().GetList(CompanyID));

            IsBusy = false;

        }

        public bool Delete(tab_produzione_operatore Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_operatoreRepository>().Delete(Item);
        }

        public BadgeOperatoreReport? GetPrintBadge(tab_produzione_operatore Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_operatoreRepository>().PrintBadge(Item);
        }

        public ObservableCollection<tab_produzione_operatore_costo>? GetCosts(string OperatorID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_operatore_costoRepository>().GetList(CompanyID, OperatorID);
        }
    }
}
