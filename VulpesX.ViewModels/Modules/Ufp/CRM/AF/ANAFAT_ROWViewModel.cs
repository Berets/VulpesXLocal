using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM.AF;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.CRM.AF;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Ufp;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Ufp.CRM.AF
{
    public class ANAFAT_ROWViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ANAFAT_ROWViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }


        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        private DateTime year;
        public DateTime Year
        {
            get => year; set
            {
                year = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<ANAFAT_ROW>? items;
        public ObservableCollection<ANAFAT_ROW>? Items
        {
            get { return items; }
            set
            {
                items = value; NotifyPropertyChanged("Items");
            }
        }

        public async Task Load()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_ROWRepository>().GetList(CompanyID, Year.Year);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public ObservableCollection<ANAFAT_CONST>? GetConsts(DateTime Data)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_CONSTRepository>().GetList(CompanyID, Data);
        }

        public ANAFAT_CONST? GetConst(DateTime Data, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_CONSTRepository>().Get(CompanyID, Data, ID);
        }

        public ANAFAT_ROW? GetFull(int Year, long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_ROWRepository>().Get(CompanyID, Year, ID);
        }

        public tab_articolo? GetArticle(string ArticleID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(ArticleID);
        }

        public bool Delete(ANAFAT_ROW Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_ROWRepository>().Delete(Item);
        }

        public void CalculateCost(ANAFAT_CONST Constant, ANAFAT_ROW Item)
        {
            VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_ROWRepository>().CalculateCosts(ref Constant, ref Item);
        }
    }
}
