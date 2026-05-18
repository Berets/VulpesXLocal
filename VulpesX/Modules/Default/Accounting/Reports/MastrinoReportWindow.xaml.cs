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
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Reports;

namespace VulpesX.Modules.Default.Accounting.Reports
{
    /// <summary>
    /// Interaction logic for MastrinoReportWindow.xaml
    /// </summary>
    public partial class MastrinoReportWindow : FluentDefaultWindow
    {
        private MastrinoReportWindowViewModel _dataContext;
        public MastrinoReportWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<MastrinoReportWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "SelectedGroup")
                {
                    if (_dataContext.SelectedGroup != null)
                    {
                        _dataContext.AccountsList = _dataContext.GetPDCCONTIs(_dataContext.SelectedGroup.P1GRUP);
                    }
                    else
                    {
                        _dataContext.AccountsList = null;
                    }
                }

                if (e.PropertyName == "SelectedAccount")
                {
                    if (_dataContext.SelectedAccount != null)
                    {
                        _dataContext.SubaccountsList = _dataContext.GetPDCSOTTOs(_dataContext.SelectedAccount.P1GRUP, _dataContext.SelectedAccount.P2CONT);
                    }
                    else
                    {
                        _dataContext.SubaccountsList = null;
                    }
                }
            };

            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;

            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            _dataContext.AccountingYear = (cmbAccountingYear.Items[0] as ESERCIZIO)?.eseann;
            _dataContext.MonthlyGroup = false;
            _dataContext.IsDefinitive = false;
            _dataContext.PrintFrom = new DateTime(now.Year, ((cmbAccountingYear.Items[0] as ESERCIZIO)?.eseini ?? 1), 1);
            _dataContext.PrintUntil = now;
            _dataContext.GroupsList = _dataContext.GetPDCGRUPPIs();
            _dataContext.Entities = _dataContext.GetABEs();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.AccountingYear.HasValue && _dataContext.AccountingYear > 0 && _dataContext.PrintUntil.HasValue && _dataContext.PrintFrom.HasValue)
            {
                if ((_dataContext.SelectedGroup != null && _dataContext.SelectedAccount != null && _dataContext.SelectedSubaccount != null) ||
                    (_dataContext.SelectedGroup == null && _dataContext.SelectedAccount == null && _dataContext.SelectedSubaccount == null))
                {
                    if (_dataContext.PrintFrom.Value <= _dataContext.PrintUntil.Value)
                    {
                        if (_dataContext.PrintUntil.Value.Year >= _dataContext.AccountingYear)
                        {
                            var esercizio = _dataContext.GetESERCIZIO();
                            if (esercizio != null)
                            {
                                if (!_dataContext.IsDefinitive ||
                                    (_dataContext.IsDefinitive &&
                                    ((esercizio.eseusm.HasValue && esercizio.eseusm.Value.AddDays(1).Date == _dataContext.PrintFrom.Value.Date) || (!esercizio.eseusm.HasValue && _dataContext.PrintFrom.Value.Date == new DateTime(esercizio.eseann, (esercizio.eseini ?? 1), 1)))))
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
                                        if (ConfirmHandler.Confirm($"Confermate la stampa {(_dataContext.IsDefinitive ? "DEFINITIVA" : "PROVVISORIA")} del mastrino selezionato ?"))
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
                                    var message = "";
                                    if (esercizio.eseusm.HasValue)
                                        message = $"In caso di stampa definitiva la data di partenza deve essere quella successiva all'ultima chiusura [{esercizio.eseusm.Value.AddDays(1).ToString("dd/MM/yyyy")}]";
                                    else
                                        message = $"In caso di prima stampa definitiva per l'esercizio, la data di partenza deve essere uguale al primo giorno di esercizio [{new DateTime(esercizio.eseann, (esercizio.eseini ?? 1), 1)}]";
                                    ErrorHandler.Validation(message);
                                }
                            }
                            else
                            {
                                ErrorHandler.Validation("Esercizio non trovato");
                            }
                        }
                        else
                        {
                            ErrorHandler.Validation("La data fino a cui stampare non può essere inferiore al 1 Gennaio dell'anno contabile selezionato");
                        }
                    }
                    else
                    {
                        ErrorHandler.Validation("La data fino a cui stampare non può essere inferiore a quella da cui stampare");
                    }
                }
                else
                {
                    ErrorHandler.Validation("Gruppo, conto e sottoconto devono essere tutti selezionati o tutti vuoti");
                }
            }
            else
            {
                ErrorHandler.Validation("Anno contabile e date di stampa sono obbligatori");
            }
        }

        private void cmbAccountingYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var esercizio = (cmbAccountingYear.SelectedItem as ESERCIZIO);

            if (esercizio != null)
            {
                _dataContext.AccountingYear = esercizio.eseann;
                _dataContext.PrintFrom = new DateTime(esercizio.eseann, (esercizio.eseini ?? 1), 1);

                if (esercizio.eseest != "A" && esercizio.eseest != "U")
                {
                    togDefinitive.IsEnabled = false;
                }
                else
                {
                    togDefinitive.IsEnabled = true;
                }
            }

        }

        #region Autocompletes
        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }
        private void ac_LostFocus(object sender, RoutedEventArgs e)
        {
            var ac = sender as RadAutoCompleteBox;
            if (ac != null)
            {
                if (ac.SelectedItem == null)
                {
                    ac.SearchText = null;
                }
            }
        }
        #endregion

        private void togDefinitive_Checked(object sender, RoutedEventArgs e)
        {
            PDCEEnabling(false);
        }

        private void togDefinitive_Unchecked(object sender, RoutedEventArgs e)
        {
            PDCEEnabling(true);
        }

        private void PDCEEnabling(bool Enabled)
        {
            acGroup.IsEnabled = Enabled;
            acAccount.IsEnabled = Enabled;
            acSubaccount.IsEnabled = Enabled;
            acEntity.IsEnabled = Enabled;

            _dataContext.SelectedGroup = null;
            _dataContext.SelectedEntity = null;
        }
    }
}
