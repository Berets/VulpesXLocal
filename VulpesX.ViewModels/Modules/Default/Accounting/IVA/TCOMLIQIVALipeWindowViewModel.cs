using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting.IVA
{
    public class TCOMLIQIVALipeWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public TCOMLIQIVALipeWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public ESERCIZIO? AccountingYear { get; set; } // eseivavenBool >>> false NO IVA cassa | eseliq M mensile/ T trimestrale

        private string? yearInfo;
        public string? YearInfo { get => yearInfo; set { yearInfo = value; NotifyPropertyChanged("YearInfo"); } }

        public string? QuarterID { get; set; }

        private ObservableCollection<GenericIDDescription>? quarters;
        public ObservableCollection<GenericIDDescription>? Quarters { get => quarters; set { quarters = value; NotifyPropertyChanged("Quarters"); } }

        public ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public bool Exists(int Year, int Period)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCOMLIQIVARepository>().Exists(CompanyID, Year, Period);
        }

        public bool ComputeIva()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCOMLIQIVARepository>().ComputeIva(CompanyID, AccountingYear!.eseann, QuarterID!, AccountingYear!.eseliq!, UserID);
        }

        public bool ComputeOrdinary()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCOMLIQIVARepository>().ComputeOrdinary(CompanyID, AccountingYear!.eseann, QuarterID!, AccountingYear!.eseliq!, UserID);
        }
    }
}
