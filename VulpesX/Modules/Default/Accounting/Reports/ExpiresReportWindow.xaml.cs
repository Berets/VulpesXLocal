using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client;
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
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Reports;

namespace VulpesX.Modules.Default.Accounting.Reports
{
    /// <summary>
    /// Interaction logic for ExpiresReportWindow.xaml
    /// </summary>
    public partial class ExpiresReportWindow : FluentDefaultWindow
    {
        private ExpiresReportWindowViewModel _dataContext;
        public ExpiresReportWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ExpiresReportWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;
            this.Loaded += (sender, e) =>
            {
                cmbEntityType.SelectedIndex = 0;
                cmbGroupingType.SelectedIndex = 0;

                MoveFocus(new TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
            };

            _dataContext.PropertyChanged += (sender, e) =>
            {
                if(e.PropertyName == "SelectedEntityType")
                {
                    if (_dataContext.SelectedEntityType != null)
                    {
                        _dataContext.Entities = _dataContext.GetABE(_dataContext.SelectedEntityType.ID!);
                        _dataContext.EntityDescription = $"{_dataContext.SelectedEntityType.Description} (tutti se lasciato vuoto)";

                        if (_dataContext.SelectedEntityType.ID == "C")
                        {
                            _dataContext.SelectedEntityPaymentDescription = $"Tipo incasso";
                            _dataContext.PaymentTypes = _dataContext.GetTIPINC();
                        }
                        else
                        {
                            _dataContext.SelectedEntityPaymentDescription = $"Tipo pagamento";
                            _dataContext.PaymentTypes = _dataContext.GetTIPINC();
                        }

                        _dataContext.SelectedPaymentType = _dataContext.PaymentTypes?.Where(w => string.IsNullOrWhiteSpace(w.ID)).FirstOrDefault();
                    }
                }
            };
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.SelectedEntityType != null && _dataContext.SelectedGroupingType != null)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (_dataContext.RunEqualization)
                {
                    // pareggiamento partite
                    _dataContext.RunEqualizationStoredProcedure();
                }
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                ExpiresReport? reportData = _dataContext.SelectedEntityType.ID == "C" ? _dataContext.GetCustomers() : _dataContext.GetSuppliers();
                if (reportData != null)
                {
                    reportData.ReportTitle = $"SCADENZIARIO {(_dataContext.SelectedEntityType.ID == "C" ? "CLIENTI" : "FORNITORI")} AL {now.ToString("dd/MM/yyyy")}";
                    reportData.PrintedText = $"Stampato il {now.ToString("dd/MM/yyyy HH:mm:ss")}";

                    Mouse.OverrideCursor = null;
                    ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_ACCOUNTING, (_dataContext.SelectedGroupingType.ID == "C" ? Constants.REPORT_TYPE_ACCOUNTING_EXPIRES : (_dataContext.SelectedGroupingType.ID == "S" ? Constants.REPORT_TYPE_ACCOUNTING_EXPIRES_DATE : Constants.REPORT_TYPE_ACCOUNTING_EXPIRES_PAYMENT)), _dataContext.CompanyID, reportData, reportData.ReportTitle, $"Scadenziario_{_dataContext.SelectedEntityType.ID}_{now.ToString("dd_MM_yyyy_HH_mm_ss")}.pdf", true);
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
    }
}
