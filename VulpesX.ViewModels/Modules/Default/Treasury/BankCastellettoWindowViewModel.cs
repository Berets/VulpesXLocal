using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Treasury;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Treasury
{
    public class BankCastellettoWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public BankCastellettoWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        private ObservableCollection<PNPORTAFOGLIO>? items;
        public ObservableCollection<PNPORTAFOGLIO>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        public int ABI;
        public int CAB;
        public int ABINew;
        public int CABNew;
        public required string Bank;
        public required string Agency;
        public string BankFullText => $"{ABI} - {CAB} {Bank.Trim()} - {Agency.Trim()}";


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

        public async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().GetCastellettoDetails(CompanyID,  ABI, CAB, ABINew, CABNew));

            IsBusy = false;

        }
    }
}
