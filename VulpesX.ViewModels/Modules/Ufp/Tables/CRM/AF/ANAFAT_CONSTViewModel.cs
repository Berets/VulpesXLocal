using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.CRM.AF;
using VulpesX.Models;
using VulpesX.Models.Ufp;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Ufp.Tables.CRM.AF
{
    public class ANAFAT_CONSTViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public ANAFAT_CONSTViewModel()
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

        private ObservableCollection<ANAFAT_CONST>? items;
        public ObservableCollection<ANAFAT_CONST>? Items
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
                       Items = VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_CONSTRepository>().GetList());

            IsBusy = false;
        }

        public ANAFAT_CONST? Get(string CompanyID, DateTime Date, int Version)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_CONSTRepository>().Get(CompanyID,Date,Version);
        }
    }
}
