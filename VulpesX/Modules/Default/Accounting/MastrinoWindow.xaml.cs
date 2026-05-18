using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Telerik.Windows.Documents.FormatProviders.Html.Parsing.Dom;
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for MastrinoWindow.xaml
    /// </summary>
    public partial class MastrinoWindow : FluentDefaultWindow
    {
        public MastrinoWindowViewModel _dataContext;
        public MastrinoWindow(MastrinoWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);

            this.DataContext = _dataContext;
            this.PreviewKeyDown += async (s, e) =>
            {
                if (e.Key == Key.F5)
                    await LoadData();
            };


            _dataContext.GetIsPatrimonial();
            _dataContext.LimitDate = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            this.Loaded += async (s, e) =>
            {
                await LoadData();
            };
        }

        private async Task LoadData()
        {
            await _dataContext.GetMastrino();

            if (_dataContext.Items != null)
            {
                _dataContext.CurrencyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMF02;

                foreach (var item in _dataContext.Items)
                {
                    item.CurrencyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMF02;
                }
            }

            #region Temporary data
            if (_dataContext.Items == null)
            {
                _dataContext.TotaleDataTempSign = "-";
                _dataContext.TotaleDataTemp = 0;
            }
            else
            {
                decimal tmpDare = 0;
                decimal tmpAvere = 0;

                if (_dataContext.CurrencyID == "EUR")
                {
                    tmpDare = _dataContext.Items.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMEU) ?? 0;
                    tmpAvere = _dataContext.Items.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMEU) ?? 0;
                }
                else
                {
                    tmpDare = _dataContext.Items.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMPO) ?? 0;
                    tmpAvere = _dataContext.Items.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMPO) ?? 0;
                }

                if (tmpDare > tmpAvere)
                {
                    _dataContext.TotaleDataTempSign = "D";
                    _dataContext.TotaleDataTemp = tmpDare - tmpAvere;
                }
                else
                {
                    if (tmpAvere > tmpDare)
                    {
                        _dataContext.TotaleDataTempSign = "A";
                        _dataContext.TotaleDataTemp = tmpAvere - tmpDare;
                    }
                    else
                    {
                        _dataContext.TotaleDataTempSign = "-";
                        _dataContext.TotaleDataTemp = tmpDare - tmpAvere;
                    }
                }
            }
            #endregion

            #region Data
            if (_dataContext.Items == null)
            {
                _dataContext.TotaleDataSign = "-";
                _dataContext.TotaleData = 0;
            }
            else
            {
                decimal dare = 0;
                decimal avere = 0;

                if (_dataContext.CurrencyID == "EUR")
                {
                    dare = _dataContext.Items.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMEU) ?? 0;
                    avere = _dataContext.Items.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMEU) ?? 0;
                }
                else
                {
                    dare = _dataContext.Items.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMPO) ?? 0;
                    avere = _dataContext.Items.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMPO) ?? 0;
                }

                if (dare > avere)
                {
                    _dataContext.TotaleDataSign = "D";
                    _dataContext.TotaleData = dare - avere;
                }
                else
                {
                    if (avere > dare)
                    {
                        _dataContext.TotaleDataSign = "A";
                        _dataContext.TotaleData = avere - dare;
                    }
                    else
                    {
                        _dataContext.TotaleDataSign = "-";
                        _dataContext.TotaleData = dare - avere;
                    }
                }
            }
            #endregion


            #region Temporary data
            if (_dataContext.ItemsPreviousYear == null)
            {
                _dataContext.TotalePreviousYearTemp = 0;
                _dataContext.TotalePreviousYearTempSign = "-";
            }
            else
            {
                decimal tmpDare = 0;
                decimal tmpAvere = 0;

                if (_dataContext.CurrencyID == "EUR")
                {
                    tmpDare = _dataContext.ItemsPreviousYear.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMEU) ?? 0;
                    tmpAvere = _dataContext.ItemsPreviousYear.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMEU) ?? 0;
                }
                else
                {
                    tmpDare = _dataContext.ItemsPreviousYear.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMPO) ?? 0;
                    tmpAvere = _dataContext.ItemsPreviousYear.Where(w => w.N1tmpPNR == "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMPO) ?? 0;
                }

                if (tmpDare > tmpAvere)
                {
                    _dataContext.TotalePreviousYearTempSign = "D";
                    _dataContext.TotalePreviousYearTemp = tmpDare - tmpAvere;
                }
                else
                {
                    if (tmpAvere > tmpDare)
                    {
                        _dataContext.TotalePreviousYearTempSign = "A";
                        _dataContext.TotalePreviousYearTemp = tmpAvere - tmpDare;
                    }
                    else
                    {
                        _dataContext.TotalePreviousYearTempSign = "-";
                        _dataContext.TotalePreviousYearTemp = tmpDare - tmpAvere;
                    }
                }
            }
            #endregion

            #region Data
            if (_dataContext.ItemsPreviousYear == null)
            {
                _dataContext.TotalePreviousYearSign = "-";
                _dataContext.TotalePreviousYear = 0;
            }
            else
            {
                decimal dare = 0;
                decimal avere = 0;

                if (_dataContext.CurrencyID == "EUR")
                {
                    dare = _dataContext.ItemsPreviousYear.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMEU) ?? 0;
                    avere = _dataContext.ItemsPreviousYear.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMEU) ?? 0;
                }
                else
                {
                    dare = _dataContext.ItemsPreviousYear.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "D").Sum(sum => sum.N1IMPO) ?? 0;
                    avere = _dataContext.ItemsPreviousYear.Where(w => w.N1tmpPNR != "S" && w.N1SEGN == "A").Sum(sum => sum.N1IMPO) ?? 0;
                }

                if (dare > avere)
                {
                    _dataContext.TotalePreviousYearSign = "D";
                    _dataContext.TotalePreviousYear = dare - avere;
                }
                else
                {
                    if (avere > dare)
                    {
                        _dataContext.TotalePreviousYearSign = "A";
                        _dataContext.TotalePreviousYear = avere - dare;
                    }
                    else
                    {
                        _dataContext.TotalePreviousYearSign = "-";
                        _dataContext.TotalePreviousYear = dare - avere;
                    }
                }
            }
            #endregion

            #region Current year balance
            if (_dataContext.TotaleDataTempSign == _dataContext.TotaleDataSign)
            {
                _dataContext.TotaleDataBalace = _dataContext.TotaleData + _dataContext.TotaleDataTemp;
                _dataContext.TotaleDataBalanceSign = _dataContext.TotaleDataSign;
            }
            else
            {
                if (_dataContext.TotaleData > _dataContext.TotaleDataTemp)
                {
                    _dataContext.TotaleDataBalace = _dataContext.TotaleData - _dataContext.TotaleDataTemp;
                    _dataContext.TotaleDataBalanceSign = _dataContext.TotaleDataSign;
                }
                else
                {
                    if (_dataContext.TotaleData < _dataContext.TotaleDataTemp)
                    {
                        _dataContext.TotaleDataBalace = _dataContext.TotaleDataTemp - _dataContext.TotaleData;
                        _dataContext.TotaleDataBalanceSign = _dataContext.TotaleDataTempSign;
                    }
                    else
                    {
                        _dataContext.TotaleDataBalace = 0;
                        _dataContext.TotaleDataBalanceSign = "-";
                    }
                }
            }
            #endregion

            #region Previous year balance
            if (_dataContext.TotalePreviousYearTempSign == _dataContext.TotalePreviousYearSign)
            {
                _dataContext.TotalePreviousYearBalance = _dataContext.TotalePreviousYear + _dataContext.TotalePreviousYearTemp;
                _dataContext.TotalePreviousYearBalanceSign = _dataContext.TotalePreviousYearSign;
            }
            else
            {
                if (_dataContext.TotalePreviousYear > _dataContext.TotalePreviousYearTemp)
                {
                    _dataContext.TotalePreviousYearBalance = _dataContext.TotalePreviousYear - _dataContext.TotalePreviousYearTemp;
                    _dataContext.TotalePreviousYearBalanceSign = _dataContext.TotalePreviousYearSign;
                }
                else
                {
                    if (_dataContext.TotalePreviousYear < _dataContext.TotalePreviousYearTemp)
                    {
                        _dataContext.TotalePreviousYearBalance = _dataContext.TotalePreviousYearTemp - _dataContext.TotalePreviousYear;
                        _dataContext.TotalePreviousYearBalanceSign = _dataContext.TotalePreviousYearTempSign;
                    }
                    else
                    {
                        _dataContext.TotalePreviousYearBalance = 0;
                        _dataContext.TotalePreviousYearBalanceSign = "-";
                    }
                }
            }
            #endregion

            #region Partials data
            if (_dataContext.TotalePreviousYearSign == _dataContext.TotaleDataSign)
            {
                _dataContext.PartialBalance = _dataContext.TotaleData + _dataContext.TotalePreviousYear;
                _dataContext.PartialBalanceSign = _dataContext.TotaleDataSign;
            }
            else
            {
                if (_dataContext.TotaleData > _dataContext.TotalePreviousYear)
                {
                    _dataContext.PartialBalance = _dataContext.TotaleData - _dataContext.TotalePreviousYear;
                    _dataContext.PartialBalanceSign = _dataContext.TotaleDataSign;
                }
                else
                {
                    if (_dataContext.TotaleData < _dataContext.TotalePreviousYear)
                    {
                        _dataContext.PartialBalance = _dataContext.TotalePreviousYear - _dataContext.TotaleData;
                        _dataContext.PartialBalanceSign = _dataContext.TotalePreviousYearSign;
                    }
                    else
                    {
                        _dataContext.PartialBalance = 0;
                        _dataContext.PartialBalanceSign = "-";
                    }
                }
            }
            #endregion

            #region Global data
            if (_dataContext.TotalePreviousYearBalanceSign == _dataContext.TotaleDataBalanceSign)
            {
                _dataContext.GlobalBalance = _dataContext.TotaleDataBalace + _dataContext.TotalePreviousYearBalance;
                _dataContext.GlobalBalanceSign = _dataContext.TotaleDataBalanceSign;
                _dataContext.NotifyPropertyChanged(nameof(_dataContext.GlobalBalanceText));
            }
            else
            {
                if (_dataContext.TotaleDataBalace > _dataContext.TotalePreviousYearBalance)
                {
                    _dataContext.GlobalBalance = _dataContext.TotaleDataBalace - _dataContext.TotalePreviousYearBalance;
                    _dataContext.GlobalBalanceSign = _dataContext.TotaleDataBalanceSign;
                    _dataContext.NotifyPropertyChanged(nameof(_dataContext.GlobalBalanceText));
                }
                else
                {
                    if (_dataContext.TotaleDataBalace < _dataContext.TotalePreviousYearBalance)
                    {
                        _dataContext.GlobalBalance = _dataContext.TotalePreviousYearBalance - _dataContext.TotaleDataBalace;
                        _dataContext.GlobalBalanceSign = _dataContext.TotalePreviousYearBalanceSign;
                        _dataContext.NotifyPropertyChanged(nameof(_dataContext.GlobalBalanceText));
                    }
                    else
                    {
                        _dataContext.GlobalBalance = 0;
                        _dataContext.GlobalBalanceSign = "-";
                        _dataContext.NotifyPropertyChanged(nameof(_dataContext.GlobalBalanceText));
                    }
                }
            }
            #endregion

            decimal balance = 0;
            string? balanceSign = "-";

            if (_dataContext.IsPatrimonial)
            {
                balance = _dataContext.TotalePreviousYear;
                balanceSign = _dataContext.TotalePreviousYearSign;
            }

            foreach (var row in _dataContext.Items ?? new System.Collections.ObjectModel.ObservableCollection<PNRIGHE>())
            {
                if (balanceSign == row.N1SEGN || balanceSign == "-")
                {
                    balance = (row.N1IMEU ?? 0) + balance;
                    balanceSign = row.N1SEGN;
                }
                else
                {
                    if (balance > row.N1IMEU)
                    {
                        balance = balance - row.N1IMEU.Value;
                    }
                    else
                    {
                        if (balance < row.N1IMEU)
                        {
                            balance = row.N1IMEU.Value - balance;
                            balanceSign = row.N1SEGN;
                        }
                        else
                        {
                            balance = 0;
                            balanceSign = "-";
                        }
                    }
                }
                row.SaldoMastrino = balance;
                row.SegnoSaldoMastrino = balanceSign;
            }
        }

        private void cmdEditReg_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as PNRIGHE;

            if (item != null)
            {
                var head = _dataContext.GetPNTESTATA(item.N1ANNO, item.N1REGI);

                if (head != null)
                {
                    var causals = _dataContext.GetCAUCONT();
                    var codes = !string.IsNullOrWhiteSpace(head.N1FLCF) ? _dataContext.GetABE(head.N1FLCF) : null;

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNRIGHEWindowViewModel>();
                    windowViewModel.Head = head;
                    windowViewModel.HeadSelectedCausal = causals?.Where(w => w.caucod == head.pncaus).FirstOrDefault();
                    windowViewModel.IsInsert = false;

                    var wPNRIGHE = new PNRIGHEWindow(windowViewModel);
                    wPNRIGHE.Owner = Window.GetWindow(this);
                    wPNRIGHE.ShowDialog();
                }
            }
        }

        private async void rdtLimitDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadData();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {

            var esercizio = _dataContext.GetESERCIZIO();
            if (esercizio != null)
            {

                if (esercizio.eseest != "U" && esercizio.eseest != "A")
                {
                    if (ConfirmHandler.Confirm($"Confermate la ristampa del mastrino selezionato ?"))
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        var reportData = _dataContext.ReprintMastrino();
                        Mouse.OverrideCursor = null;

                        if (reportData != null)
                        {
                            ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_ACCOUNTING, Constants.REPORT_TYPE_ACCOUNTING_MASTRINI, _dataContext.CompanyID, reportData, reportData.ReportTitle ?? string.Empty, reportData.PrintFilename, true);
                        }
                    }
                }
                else
                {
                    if (ConfirmHandler.Confirm($"Confermate la stampa PROVVISORIA del mastrino selezionato ?"))
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        var reportData = _dataContext.PrintMastrino();
                        Mouse.OverrideCursor = null;

                        if (reportData != null)
                        {
                            ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_ACCOUNTING, Constants.REPORT_TYPE_ACCOUNTING_MASTRINI, _dataContext.CompanyID, reportData, reportData.ReportTitle ?? string.Empty, reportData.PrintFilename, true);
                        }
                    }
                }


            }
            else
            {
                ErrorHandler.Validation("Esercizio non trovato");
            }


        }
    }
}
