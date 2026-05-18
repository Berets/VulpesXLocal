using Itenso.TimePeriod;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.IVA;

namespace VulpesX.Modules.Default.Accounting.IVA
{
    /// <summary>
    /// Interaction logic for TCOMLIQIVALipeWindow.xaml
    /// </summary>
    public partial class TCOMLIQIVALipeWindow : FluentDefaultWindow
    {
        private TCOMLIQIVALipeWindowViewModel _dataContext;
        public TCOMLIQIVALipeWindow(TCOMLIQIVALipeWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            CultureInfo noSeparator = new CultureInfo("it-IT");
            noSeparator.NumberFormat.NumberGroupSeparator = "";
            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;

            _dataContext.Quarters = CommonsService.LIPEQuarters;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            cmdCancel.IsEnabled = false;
            cmdSave.IsEnabled = false;

            if (_dataContext.AccountingYear != null)
            {
                if (!string.IsNullOrWhiteSpace(_dataContext.QuarterID))
                {
                    if (!AlreadyExists())
                    {
                        if (_dataContext.AccountingYear.eseivavenBool)
                        {
                            // IVA per cassa
                            _dataContext.ComputeIva();
                        }
                        else
                        {
                            // NO IVA per cassa
                            _dataContext.ComputeOrdinary();
                        }
                        Mouse.OverrideCursor = null;
                        this.DialogResult = true;
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation("Uno o piu' mesi inclusi in questa elaborazione sono gia' presenti, eliminarli per poter proseguire");
                        cmdCancel.IsEnabled = true;
                        cmdSave.IsEnabled = true;
                    }
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("E' necessario selezionare un periodo da elaborare");
                    cmdCancel.IsEnabled = true;
                    cmdSave.IsEnabled = true;
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("E' necessario selezionare un esercizio");
                cmdCancel.IsEnabled = true;
                cmdSave.IsEnabled = true;
            }
        }

        private bool AlreadyExists()
        {
            if (_dataContext.AccountingYear != null)
            {
                if (_dataContext.AccountingYear.eseliq == "M")
                {
                    // monthly : check every month 
                    switch (_dataContext.QuarterID)
                    {
                        case "I":
                            return _dataContext.Exists(_dataContext.AccountingYear.eseann, 1) ||
                                _dataContext.Exists(_dataContext.AccountingYear.eseann, 2) ||
                                _dataContext.Exists(_dataContext.AccountingYear.eseann, 3);
                        case "II":
                            return _dataContext.Exists(_dataContext.AccountingYear.eseann, 4) ||
                                _dataContext.Exists(_dataContext.AccountingYear.eseann, 5) ||
                                _dataContext.Exists(_dataContext.AccountingYear.eseann, 6);
                        case "III":
                            return _dataContext.Exists(_dataContext.AccountingYear.eseann, 7) ||
                                _dataContext.Exists(_dataContext.AccountingYear.eseann, 8) ||
                                _dataContext.Exists(_dataContext.AccountingYear.eseann, 9);
                        case "IV":
                            return _dataContext.Exists(_dataContext.AccountingYear.eseann, 10) ||
                                _dataContext.Exists(_dataContext.AccountingYear.eseann, 11) ||
                                _dataContext.Exists(_dataContext.AccountingYear.eseann, 12);
                    }
                }
                else
                {
                    // quarterly : check only first month of the quarter 
                    switch (_dataContext.QuarterID)
                    {
                        case "I":
                            return _dataContext.Exists(_dataContext.AccountingYear.eseann, 1);
                        case "II":
                            return _dataContext.Exists(_dataContext.AccountingYear.eseann, 4);
                        case "III":
                            return _dataContext.Exists(_dataContext.AccountingYear.eseann, 7);
                        case "IV":
                            return _dataContext.Exists(_dataContext.AccountingYear.eseann, 10);
                    }
                }
            }

            return true;
        }

        private void cmbAccountingYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataContext.AccountingYear = cmbAccountingYear.SelectedItem as ESERCIZIO;

            if (_dataContext.AccountingYear != null)
            {
                _dataContext.YearInfo = $"{(!_dataContext.AccountingYear.eseivavenBool ? "Regime IVA ordinario" : "Regime IVA per cassa")} > {(CommonsService.LIPELiquidationTypes.Where(w => w.ID == _dataContext.AccountingYear?.eseliq).FirstOrDefault()?.Description)}";
            }

        }
    }
}
