using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.CRM;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class OFFET00FCloseReasonWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public OFFET00FCloseReasonWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public string? Note { get; set; }
        public ObservableCollection<TAB_CRM_CAUOFFCLO>? Causals { get; set; }
        public TAB_CRM_CAUOFFCLO? SelectedCausal { get; set; }

        public string? SelectedNote { get; set; }

        private int maxSize = 500;

        private int currentSize;
        public int CurrentSize
        {
            get { return currentSize; }
            set
            {
                currentSize = value;
                NotifyPropertyChanged("CurrenSize");
                NotifyPropertyChanged("CurrentSizeText");
            }
        }
        public string CurrentSizeText
        {
            get => $"{currentSize} / {maxSize}";
        }

        public ObservableCollection<TAB_CRM_CAUOFFCLO>? GetTAB_CRM_CAUOFFCLOs()
        {
            return  VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_CAUOFFCLORepository>().GetList(CompanyID);
        }
    }
}
