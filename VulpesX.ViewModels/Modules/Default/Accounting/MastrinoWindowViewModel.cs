using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using static VulpesX.Models.Models.StockCheckExistance;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class MastrinoWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required int Year { get; set; }
        public required PDCANNI SelectedMastrino { get; set; }

        public MastrinoWindowViewModel()
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

        private ObservableCollection<PNRIGHE>? items;
        public ObservableCollection<PNRIGHE>? Items
        {
            get => items;
            set
            {
                items = value;

                #region Temporary data
                if (items == null)
                {
                    TotaleDataTempSign = "-";
                    TotaleDataTemp = 0;
                }
                else
                {
                    decimal tmpDare = 0;
                    decimal tmpAvere = 0;

                    if (CurrencyID == "EUR")
                    {
                        tmpDare = items.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMEU) ?? 0;
                        tmpAvere = items.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMEU) ?? 0;
                    }
                    else
                    {
                        tmpDare = items.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMPO) ?? 0;
                        tmpAvere = items.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMPO) ?? 0;
                    }

                    if (tmpDare > tmpAvere)
                    {
                        TotaleDataTempSign = "D";
                        TotaleDataTemp = tmpDare - tmpAvere;
                    }
                    else
                    {
                        if (tmpAvere > tmpDare)
                        {
                            TotaleDataTempSign = "A";
                            TotaleDataTemp = tmpAvere - tmpDare;
                        }
                        else
                        {
                            TotaleDataTempSign = "-";
                            TotaleDataTemp = tmpDare - tmpAvere;
                        }
                    }
                }
                #endregion

                #region Data
                if (items == null)
                {
                    TotaleDataSign = "-";
                    TotaleData = 0;
                }
                else
                {
                    decimal dare = 0;
                    decimal avere = 0;

                    if (CurrencyID == "EUR")
                    {
                        dare = items.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMEU) ?? 0;
                        avere = items.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMEU) ?? 0;
                    }
                    else
                    {
                        dare = items.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMPO) ?? 0;
                        avere = items.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMPO) ?? 0;
                    }

                    if (dare > avere)
                    {
                        TotaleDataSign = "D";
                        TotaleData = dare - avere;
                    }
                    else
                    {
                        if (avere > dare)
                        {
                            TotaleDataSign = "A";
                            TotaleData = avere - dare;
                        }
                        else
                        {
                            TotaleDataSign = "-";
                            TotaleData = dare - avere;
                        }
                    }
                }
                #endregion

                NotifyPropertyChanged("Items");
                NotifyPropertyChanged("TotaleData");
                NotifyPropertyChanged("TotaleDataSign");
                NotifyPropertyChanged("TotaleDataTemp");
                NotifyPropertyChanged("TotaleDataTempSign");
            }
        }

        private ObservableCollection<PNRIGHE>? itemsPreviousYear;
        public ObservableCollection<PNRIGHE>? ItemsPreviousYear
        {
            get => itemsPreviousYear;
            set
            {
                itemsPreviousYear = value;


                //#region Temporary data
                //if (itemsPreviousYear == null)
                //{
                //    TotalePreviousYearTemp = 0;
                //    TotalePreviousYearTempSign = "-";
                //}
                //else
                //{
                //    decimal tmpDare = 0;
                //    decimal tmpAvere = 0;

                //    if (CurrencyID == "EUR")
                //    {
                //        tmpDare = itemsPreviousYear.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMEU) ?? 0;
                //        tmpAvere = itemsPreviousYear.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMEU) ?? 0;
                //    }
                //    else
                //    {
                //        tmpDare = itemsPreviousYear.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMPO) ?? 0;
                //        tmpAvere = itemsPreviousYear.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMPO) ?? 0;
                //    }

                //    if (tmpDare > tmpAvere)
                //    {
                //        TotalePreviousYearTempSign = "D";
                //        TotalePreviousYearTemp = tmpDare - tmpAvere;
                //    }
                //    else
                //    {
                //        if (tmpAvere > tmpDare)
                //        {
                //            TotalePreviousYearTempSign = "A";
                //            TotalePreviousYearTemp = tmpAvere - tmpDare;
                //        }
                //        else
                //        {
                //            TotalePreviousYearTempSign = "-";
                //            TotalePreviousYearTemp = tmpDare - tmpAvere;
                //        }
                //    }
                //}
                //#endregion

                //#region Data
                //if (itemsPreviousYear == null)
                //{
                //    TotalePreviousYearSign = "-";
                //    TotalePreviousYear = 0;
                //}
                //else
                //{
                //    decimal dare = 0;
                //    decimal avere = 0;

                //    if (CurrencyID == "EUR")
                //    {
                //        dare = itemsPreviousYear.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMEU) ?? 0;
                //        avere = itemsPreviousYear.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMEU) ?? 0;
                //    }
                //    else
                //    {
                //        dare = itemsPreviousYear.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMPO) ?? 0;
                //        avere = itemsPreviousYear.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMPO) ?? 0;
                //    }

                //    if (dare > avere)
                //    {
                //        TotalePreviousYearSign = "D";
                //        TotalePreviousYear = dare - avere;
                //    }
                //    else
                //    {
                //        if (avere > dare)
                //        {
                //            TotalePreviousYearSign = "A";
                //            TotalePreviousYear = avere - dare;
                //        }
                //        else
                //        {
                //            TotalePreviousYearSign = "-";
                //            TotalePreviousYear = dare - avere;
                //        }
                //    }
                //}
                //#endregion

                //#region Current year balance
                //if (TotaleDataTempSign == TotaleDataSign)
                //{
                //    TotaleDataBalace = TotaleData + TotaleDataTemp;
                //    TotaleDataBalanceSign = TotaleDataSign;
                //}
                //else
                //{
                //    if (TotaleData > TotaleDataTemp)
                //    {
                //        TotaleDataBalace = TotaleData - TotaleDataTemp;
                //        TotaleDataBalanceSign = TotaleDataSign;
                //    }
                //    else
                //    {
                //        if (TotaleData < TotaleDataTemp)
                //        {
                //            TotaleDataBalace = TotaleDataTemp - TotaleData;
                //            TotaleDataBalanceSign = TotaleDataTempSign;
                //        }
                //        else
                //        {
                //            TotaleDataBalace = 0;
                //            TotaleDataBalanceSign = "-";
                //        }
                //    }
                //}
                //#endregion

                //#region Previous year balance
                //if (TotalePreviousYearTempSign == TotalePreviousYearSign)
                //{
                //    TotalePreviousYearBalance = TotalePreviousYear + TotalePreviousYearTemp;
                //    TotalePreviousYearBalanceSign = TotalePreviousYearSign;
                //}
                //else
                //{
                //    if (TotalePreviousYear > TotalePreviousYearTemp)
                //    {
                //        TotalePreviousYearBalance = TotalePreviousYear - TotalePreviousYearTemp;
                //        TotalePreviousYearBalanceSign = TotalePreviousYearSign;
                //    }
                //    else
                //    {
                //        if (TotalePreviousYear < TotalePreviousYearTemp)
                //        {
                //            TotalePreviousYearBalance = TotalePreviousYearTemp - TotalePreviousYear;
                //            TotalePreviousYearBalanceSign = TotalePreviousYearTempSign;
                //        }
                //        else
                //        {
                //            TotalePreviousYearBalance = 0;
                //            TotalePreviousYearBalanceSign = "-";
                //        }
                //    }
                //}
                //#endregion

                //#region Partials data
                //if (TotalePreviousYearSign == TotaleDataSign)
                //{
                //    PartialBalance = TotaleData + TotalePreviousYear;
                //    PartialBalanceSign = TotaleDataSign;
                //}
                //else
                //{
                //    if (TotaleData > TotalePreviousYear)
                //    {
                //        PartialBalance = TotaleData - TotalePreviousYear;
                //        PartialBalanceSign = TotaleDataSign;
                //    }
                //    else
                //    {
                //        if (TotaleData < TotalePreviousYear)
                //        {
                //            PartialBalance = TotalePreviousYear - TotaleData;
                //            PartialBalanceSign = TotalePreviousYearSign;
                //        }
                //        else
                //        {
                //            PartialBalance = 0;
                //            PartialBalanceSign = "-";
                //        }
                //    }
                //}
                //#endregion

                //#region Global data
                //if (TotalePreviousYearBalanceSign == TotaleDataBalanceSign)
                //{
                //    GlobalBalance = TotaleDataBalace + TotalePreviousYearBalance;
                //    GlobalBalanceSign = TotaleDataBalanceSign;
                //}
                //else
                //{
                //    if (TotaleDataBalace > TotalePreviousYearBalance)
                //    {
                //        GlobalBalance = TotaleDataBalace - TotalePreviousYearBalance;
                //        GlobalBalanceSign = TotaleDataBalanceSign;
                //    }
                //    else
                //    {
                //        if (TotaleDataBalace < TotalePreviousYearBalance)
                //        {
                //            GlobalBalance = TotalePreviousYearBalance - TotaleDataBalace;
                //            GlobalBalanceSign = TotalePreviousYearBalanceSign;
                //        }
                //        else
                //        {
                //            GlobalBalance = 0;
                //            GlobalBalanceSign = "-";
                //        }
                //    }
                //}
                //#endregion


                //NotifyPropertyChanged("ItemsPreviousYear");
                //NotifyPropertyChanged("TotalePreviousYear");
                //NotifyPropertyChanged("TotalePreviousYearSign");
                //NotifyPropertyChanged("TotalePreviousYearTemp");
                //NotifyPropertyChanged("TotalePreviousYearTempSign");
                //NotifyPropertyChanged("TotaleDataBalance");
                //NotifyPropertyChanged("TotaleDataBalanceSign");
                //NotifyPropertyChanged("TotalePreviousYear");
                //NotifyPropertyChanged("TotalePreviousYearSign");
                //NotifyPropertyChanged("PartialBalance");
                //NotifyPropertyChanged("PartialBalanceSign");
                //NotifyPropertyChanged("GlobalBalance");
                //NotifyPropertyChanged("GlobalBalanceSign");
            }
        }

        public DateTime LimitDate { get; set; }

        public bool IsPatrimonial { get; set; }

        public string? CurrencyID { get; set; }


        private decimal _TotaleDataTemp;
        public decimal TotaleDataTemp
        {
            get { return _TotaleDataTemp; }
            set
            {
                _TotaleDataTemp = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("TotaleDataTempText");
            }
        }

        private string? _TotaleDataTempSign;
        public string? TotaleDataTempSign
        {
            get { return _TotaleDataTempSign; }
            set
            {
                _TotaleDataTempSign = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("TotaleDataTempText");
            }
        }
        public string TotaleDataTempText => $"{TotaleDataTempSign} {TotaleDataTemp,20:N2}";


        private decimal _TotaleData;
        public decimal TotaleData
        {
            get { return _TotaleData; }
            set
            {
                _TotaleData = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("TotaleDataText");
            }
        }

        private string? _TotaleDataSign;
        public string? TotaleDataSign
        {
            get { return _TotaleDataSign; }
            set
            {
                _TotaleDataSign = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("TotaleDataText");
            }
        }
        public string TotaleDataText => $"{TotaleDataSign} {TotaleData,20:N2}";


        private decimal _TotaleDataBalace;
        public decimal TotaleDataBalace
        {
            get { return _TotaleDataBalace; }
            set
            {
                _TotaleDataBalace = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("TotaleDataBalanceText");
            }
        }

        private string? _TotaleDataBalanceSign;
        public string? TotaleDataBalanceSign
        {
            get { return _TotaleDataBalanceSign; }
            set
            {
                _TotaleDataBalanceSign = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("TotaleDataBalanceText");
            }
        }
        public string TotaleDataBalanceText => $"{TotaleDataBalanceSign} {TotaleDataBalace,20:N2}";


        private decimal _TotalePreviousYearBalance;
        public decimal TotalePreviousYearBalance
        {
            get { return _TotalePreviousYearBalance; }
            set
            {
                _TotalePreviousYearBalance = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("TotalePreviousYearBalanceText");
            }
        }

        private string? _TotalePreviousYearBalanceSign;
        public string? TotalePreviousYearBalanceSign
        {
            get { return _TotalePreviousYearBalanceSign; }
            set
            {
                _TotalePreviousYearBalanceSign = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("TotalePreviousYearBalanceText");
            }
        }
        public string TotalePreviousYearBalanceText => $"{TotalePreviousYearBalanceSign} {TotalePreviousYearBalance,20:N2}";


        private decimal _TotalePreviousYearTemp;
        public decimal TotalePreviousYearTemp
        {
            get { return _TotalePreviousYearTemp; }
            set
            {
                _TotalePreviousYearTemp = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("TotalePreviousYearTempText");
            }
        }

        private string? _TotalePreviousYearTempSign;
        public string? TotalePreviousYearTempSign
        {
            get { return _TotalePreviousYearTempSign; }
            set
            {
                _TotalePreviousYearTempSign = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("TotalePreviousYearTempText");
            }
        }
        public string TotalePreviousYearTempText => $"{TotalePreviousYearTempSign} {TotalePreviousYearTemp,20:N2}";


        private decimal _TotalePreviousYear;
        public decimal TotalePreviousYear
        {
            get { return _TotalePreviousYear; }
            set
            {
                _TotalePreviousYear = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("TotalePreviousYearText");
            }
        }

        private string? _TotalePreviousYearSign;
        public string? TotalePreviousYearSign
        {
            get { return _TotalePreviousYearSign; }
            set
            {
                _TotalePreviousYearSign = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("TotalePreviousYearText");
            }
        }
        public string TotalePreviousYearText => $"{TotalePreviousYearSign} {TotalePreviousYear,20:N2}";


        private decimal _PartialBalance;
        public decimal PartialBalance
        {
            get { return _PartialBalance; }
            set
            {
                _PartialBalance = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("PartialBalanceText");
            }
        }

        private string? _PartialBalanceSign;
        public string? PartialBalanceSign
        {
            get { return _PartialBalanceSign; }
            set
            {
                _PartialBalanceSign = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("PartialBalanceText");
            }
        }
        public string PartialBalanceText => $"{PartialBalanceSign} {PartialBalance,20:N2}";

        private decimal _GlobalBalance;
        public decimal GlobalBalance
        {
            get { return _GlobalBalance; }
            set
            {
                _GlobalBalance = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("GlobalBalanceText");
            }
        }

        private string? _GlobalBalanceSign;
        public string? GlobalBalanceSign
        {
            get { return _GlobalBalanceSign; }
            set
            {
                _GlobalBalanceSign = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("GlobalBalanceText");
            }
        }
        public string GlobalBalanceText => $"{GlobalBalanceSign} {GlobalBalance,20:N2}";

        public string MastrinoFullDescription
        {
            get
            {
                return $"{SelectedMastrino?.Group?.FullDescriptionSearchable} | {SelectedMastrino?.Account?.FullDescriptionSearchable} | {SelectedMastrino?.Subaccount?.FullDescriptionSearchable}";
            }
        }

        public ObservableCollection<CAUCONT>? GetCAUCONT()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList("*");
        }

        public PNTESTATA? GetPNTESTATA(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().Get(CompanyID, Year, ID);
        }

        public ObservableCollection<ABE>? GetABE(string N1FLCF)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList(N1FLCF);
        }

        public async Task GetMastrino()
        {
            IsBusy = true;

            try
            {
                var pnrigheRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();

                var result = await Task.Run(() =>
                {
                    var items = pnrigheRepo.GetMastrinoAtDate(CompanyID, Year, SelectedMastrino.P1GRUP, SelectedMastrino.P2CONT, SelectedMastrino.P3SOTC, LimitDate);

                    ObservableCollection<PNRIGHE>? previous = null;
                    if (IsPatrimonial)
                        previous = pnrigheRepo.GetMastrinoAtDate(CompanyID, Year - 1, SelectedMastrino.P1GRUP, SelectedMastrino.P2CONT, SelectedMastrino.P3SOTC, LimitDate);

                    return new { items, previous };
                });

                Items = result.items;
                itemsPreviousYear = result.previous;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void GetIsPatrimonial()
        {
            IsPatrimonial = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().Get(SelectedMastrino.P1GRUP)?.p1chco == "1" && (VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID) ?? new ObservableCollection<ESERCIZIO>()).Where(w => w.eseann == Year - 1 && w.eseest == "A").Any();

        }

        public ESERCIZIO? GetESERCIZIO()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(CompanyID, Year);
        }

        public MastrinoReport? PrintMastrino()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().PrintMastrino(CompanyID, Year, SelectedMastrino?.P1GRUP, SelectedMastrino?.P2CONT, SelectedMastrino?.P3SOTC, new DateTime(Year,1,1), LimitDate, null, false, false);
        }

        public MastrinoReport? ReprintMastrino()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().ReprintMastrino(CompanyID, Year, SelectedMastrino?.P1GRUP, SelectedMastrino?.P2CONT, SelectedMastrino?.P3SOTC, new DateTime(Year, 1, 1), LimitDate, null, false);
        }
    }
}
