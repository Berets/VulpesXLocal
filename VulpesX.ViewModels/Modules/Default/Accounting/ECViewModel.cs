using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class ECViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ECViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
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

        public ObservableCollection<GenericIDDescription> CFFlags => CommonsService.EntityTypes;

        private string? entityType;
        public string? EntityType { get => entityType; set { entityType = value; NotifyPropertyChanged("EntityType"); } }

        private ObservableCollection<MastrinoECReportItem>? items;
        public ObservableCollection<MastrinoECReportItem>? Items { get => items; set { items = value; NotifyPropertyChanged("Items"); NotifyPropertyChanged("TotalAmount"); } }

        private ObservableCollection<MastrinoECReportItem>? selectedItems;
        public ObservableCollection<MastrinoECReportItem>? SelectedItems
        {
            get => selectedItems; set
            {
                if (selectedItems != null)
                    selectedItems.CollectionChanged -= SelectedItems_CollectionChanged;

                selectedItems = value;

                if (selectedItems != null)
                    selectedItems.CollectionChanged += SelectedItems_CollectionChanged;

                NotifyPropertyChanged("SelectedItems");
                NotifyPropertyChanged("SelectedAmount");
            }
        }

        private void SelectedItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(SelectedAmount));
            NotifyPropertyChanged(nameof(SelectedSign));
            NotifyPropertyChanged(nameof(IsAccountingRegistrationVisible));
        }

        private ObservableCollection<ABE>? codes;
        public ObservableCollection<ABE>? Codes { get => codes; set { codes = value; NotifyPropertyChanged("Codes"); } }

        private ABE? selectedEntity;
        public ABE? SelectedEntity { get => selectedEntity; set { selectedEntity = value; NotifyPropertyChanged("SelectedEntity"); } }


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
                    return Math.Round(dare - avere, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    if (avere > dare)
                    {
                        TotalSign = "A";
                        NotifyPropertyChanged("TotalSign");
                        return Math.Round(avere - dare, 2, MidpointRounding.AwayFromZero);
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

        public decimal SelectedAmount
        {
            get
            {
                if (selectedItems == null || selectedItems.Count == 0)
                {
                    SelectedSign = "-";
                    NotifyPropertyChanged("SelectedSign");
                    return 0;
                }
                var dare = selectedItems.Sum(sum => sum.Dare);
                var avere = selectedItems.Sum((sum) => sum.Avere);
                if (dare > avere)
                {
                    SelectedSign = "D";
                    NotifyPropertyChanged("SelectedSign");
                    return Math.Round(dare - avere, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    if (avere > dare)
                    {
                        SelectedSign = "A";
                        NotifyPropertyChanged("SelectedSign");
                        return Math.Round(avere - dare, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        SelectedSign = "-";
                        NotifyPropertyChanged("SelectedSign");
                        return 0;
                    }
                }
            }
        }

        public string? SelectedSign { get; set; }

        public decimal? Amount { get; set; }
        public string? AmountSign { get; set; }

        public bool IsAccountingRegistrationVisible { get { return (SelectedItems ?? new ObservableCollection<MastrinoECReportItem>()).Any(); } }

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
                        items = VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().GetECList(CompanyID, SelectedEntity?.abecod ?? 0, ToDate!.Value, HasDrawn, SinceDrawnDate!.Value);

                    return new { items };

                });

                Items = result.items;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool SyncEC(string EntityType, string CompanyID, int SourceYear, int SourceID, int SourceRow, int TargetYear, int TargetID, int TargetRow)
        {
            if (EntityType == "C")
                return VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().SyncPartita(CompanyID, SourceYear, SourceID, SourceRow, TargetYear, TargetID, TargetRow);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().SyncPartita(CompanyID, SourceYear, SourceID, SourceRow, TargetYear, TargetID, TargetRow);
        }

        public ObservableCollection<ABE>? GetABEs(string Type)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList(Type);
        }

        public ABE? GetABE(int CustomerID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(CustomerID);
        }

        public AZIENDA? GetAZIENDA()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);
        }

        public ObservableCollection<CAUCONT>? GetCAUCONT()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
        }

        public PNTESTATA? GetPNTESTATA(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().Get(CompanyID, Year, ID);
        }

        public bool IsReadOnly(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().PrintedOnGeneralJournal(CompanyID, Year, ID);
        }

        public bool Lock(MastrinoECReportItem Item, string Reason)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().Lock(Item.CompanyID, Item.Year, Item.Number, UserID, Reason);
        }

        public bool Unlock(MastrinoECReportItem Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().Unlock(Item.CompanyID, Item.Year, Item.Number, UserID);
        }
    }
}
