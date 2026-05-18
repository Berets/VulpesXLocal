using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Accounting;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using static VulpesX.Models.Models.StockCheckExistance;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class SelectECWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string EntityType { get; set; }
        public required int EntityID { get; set; }
        public bool ExcludeInPaymentExecution { get; set; } = false;

        public SelectECWindowViewModel()
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

        private ObservableCollection<MastrinoECReportItem>? items;
        public ObservableCollection<MastrinoECReportItem>? Items { get => items; set { items = value; NotifyPropertyChanged("Items"); NotifyPropertyChanged("TotalAmount"); } }

        private ABE? selectedEntity;
        public ABE? SelectedEntity { get => selectedEntity; set { selectedEntity = value; NotifyPropertyChanged("SelectedEntity"); } }

        public decimal TotalAmount
        {
            get
            {
                if (items == null || items.Count == 0)
                {
                    TotalSign = "-";
                    NotifyPropertyChanged("TotalSign");
                    return 0;
                }
                var dare = items.Sum(sum => sum.Dare);
                var avere = items.Sum((sum) => sum.Avere);
                if (dare > avere)
                {
                    TotalSign = "D";
                    NotifyPropertyChanged("TotalSign");
                    return dare - avere;
                }
                else
                {
                    if (avere > dare)
                    {
                        TotalSign = "A";
                        NotifyPropertyChanged("TotalSign");
                        return avere - dare;
                    }
                    else
                    {
                        TotalSign = "-";
                        NotifyPropertyChanged("TotalSign");
                        return 0;
                    }
                }
            }
        }
        public string? TotalSign { get; set; }

        private decimal totalSelectedAmount;
        public decimal TotalSelectedAmount { get => totalSelectedAmount; set { totalSelectedAmount = value; NotifyPropertyChanged("TotalSelectedAmount"); } }

        private string? totalSelectedSign;
        public string? TotalSelectedSign { get => totalSelectedSign; set { totalSelectedSign = value; NotifyPropertyChanged("TotalSelectedSign"); } }

        public decimal Amount { get; set; }

        public string? AmountSign { get; set; }

        private DateTime? toDate;
        public DateTime? ToDate { get => toDate; set { toDate = value; NotifyPropertyChanged("ToDate"); } }

        private DateTime? sinceDrawnDate;
        public DateTime? SinceDrawnDate
        {
            get => sinceDrawnDate;
            set
            {
                sinceDrawnDate = value;
                NotifyPropertyChanged("SinceDrawnDate");
            }
        }

        private bool hasDrawn;
        public bool HasDrawn
        {
            get => hasDrawn;
            set
            {
                hasDrawn = value;
                NotifyPropertyChanged("HasDrawn");
            }
        }

        public void GetABE()
        {
            SelectedEntity = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(EntityID);
        }

        public bool SyncEC(string EntityType, string CompanyID, int SourceYear, int SourceID, int SourceRow, int TargetYear, int TargetID, int TargetRow)
        {
            if (EntityType == "C")
                return VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().SyncPartita(CompanyID, SourceYear, SourceID, SourceRow, TargetYear, TargetID, TargetRow);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().SyncPartita(CompanyID, SourceYear, SourceID, SourceRow, TargetYear, TargetID, TargetRow);
        }

        public async Task Load()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    var items = new ObservableCollection<MastrinoECReportItem>();

                    if (EntityType == "C")
                        items = VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().GetECList(CompanyID, SelectedEntity?.abecod ?? 0, ToDate!.Value, HasDrawn, SinceDrawnDate!.Value);
                    else
                        items = VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().GetECList(CompanyID, SelectedEntity?.abecod ?? 0, ToDate!.Value, HasDrawn, SinceDrawnDate!.Value, ExcludeInPaymentExecution);

                    return new { items };
                });

                Items = result.items;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
