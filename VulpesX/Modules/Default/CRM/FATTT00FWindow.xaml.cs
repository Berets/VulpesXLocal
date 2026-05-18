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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for FATTT00FWindow.xaml
    /// </summary>
    public partial class FATTT00FWindow : FluentDefaultWindow
    {
        private FATTT00FWindowViewModel _dataContext;
        public FATTT00FWindow(FATTT00FWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedCustomer = _dataContext.Customers?.Where(w => w.abecod == _dataContext.Data.FTCODC).FirstOrDefault();

                if (_dataContext.SelectedCustomer != null)
                    _dataContext.References = _dataContext.GetDESTINATARIs(_dataContext.Data.FTCODC!.Value);
                if (_dataContext.SelectedCustomer != null && _dataContext.References != null)
                    _dataContext.SelectedReference = _dataContext.References.Where(w => w.cliecod == _dataContext.SelectedCustomer.abecod && w.codesti == _dataContext.Data.FTCODD).FirstOrDefault();

                _dataContext.SelectedCausal = _dataContext.Causals?.Where(w => w.fatcod == _dataContext.Data.FTCAUS).FirstOrDefault();
                _dataContext.SelectedPayment = _dataContext.Payments?.Where(w => w.pclcod == _dataContext.Data.FTPAGA).FirstOrDefault();

                _dataContext.Banks = _dataContext.GetABICABs(_dataContext.SelectedPayment?.Incasso?.icssup);
                if (!string.IsNullOrWhiteSpace(_dataContext.Data.FTBCON))
                    _dataContext.SelectedCustomerBank = _dataContext.Banks?.Where(w => w.ABI == _dataContext.Data.FTABIB && w.CAB == _dataContext.Data.FTCABB && w.Account == _dataContext.Data.FTBCON).FirstOrDefault();
                else
                    _dataContext.SelectedCustomerBank = _dataContext.Banks?.Where(w => w.ABI == _dataContext.Data.FTABIB && w.CAB == _dataContext.Data.FTCABB).FirstOrDefault();

                _dataContext.SelectedFEDocType = _dataContext.FEDocTypes?.Where(w => w.FETDCod == _dataContext.Data.fttdoc).FirstOrDefault();
                _dataContext.SelectedDelivery = _dataContext.Deliveries?.Where(w => w.concod == _dataContext.Data.FTCONS).FirstOrDefault();
                _dataContext.SelectedShipment = _dataContext.Shipments?.Where(w => w.specod == _dataContext.Data.FTSPED).FirstOrDefault();
                _dataContext.SelectedPacking = _dataContext.Packings?.Where(w => w.imbcod == _dataContext.Data.FTIMBL).FirstOrDefault();
                _dataContext.SelectedCarrier = _dataContext.Carriers?.Where(w => w.vetcod == _dataContext.Data.FTCORR).FirstOrDefault();
                _dataContext.SelectedCustomerBank = _dataContext.Banks?.Where(w => w.ABI == _dataContext.Data.FTABIB && w.CAB == _dataContext.Data.FTCABB).FirstOrDefault();
                _dataContext.SelectedCostCenter = _dataContext.CostCenters?.Where(w => w.cecodc == _dataContext.Data.FTCECO).FirstOrDefault();

                spInsertInfo.Visibility = Visibility.Collapsed;
                spEditInfo.Visibility = Visibility.Visible;
            }
            else
            {
                spInsertInfo.Visibility = Visibility.Visible;
                spEditInfo.Visibility = Visibility.Collapsed;
            }

            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli generali fattura {(_dataContext.IsInsert ? "nuova" : _dataContext.Data.PrintFullID)}";
                if (_dataContext.IsReadonly)
                    Title += " - [sola lettura]";
            };
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save())
                {
                    Mouse.OverrideCursor = null;
                    this.DialogResult = true;
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation(validated);
            }
        }
        #endregion

        #region Autocompletes
        private void acCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCustomer != null && _dataContext.SelectedCustomer.abecod > 0)
            {
                if (_dataContext.Data.FTCODC != _dataContext.SelectedCustomer.abecod)
                {
                    _dataContext.Data.FTCODC = _dataContext.SelectedCustomer.abecod;
                    _dataContext.References = _dataContext.GetDESTINATARIs(_dataContext.Data.FTCODC.Value);
                    // set default payment, bank
                    var customerData = _dataContext.GetCLIAMMI(_dataContext.SelectedCustomer.abecod);
                    var clientData = _dataContext.GetCLIENTI(_dataContext.SelectedCustomer.abecod);

                    _dataContext.SelectedPayment = _dataContext.Payments?.Where(w => w.pclcod == customerData?.pclcod).FirstOrDefault();

                    if (_dataContext.SelectedPayment != null)
                    {
                        if (_dataContext.SelectedPayment?.Incasso?.icssup == "R")
                            _dataContext.SelectedCustomerBank = _dataContext.Banks?.Where(w => w.ABI == customerData?.CLABI && w.CAB == customerData?.CLCAB).FirstOrDefault();
                        else
                            _dataContext.SelectedCustomerBank = _dataContext.Banks?.Where(w => w.ABI == customerData?.banabi && w.CAB == customerData?.bancab && w.Account == customerData?.bancoc).FirstOrDefault();
                    }
                    else
                    {
                        _dataContext.Banks = _dataContext.GetABICABs(null);
                        _dataContext.SelectedCustomerBank = _dataContext.Banks?.Where(w => w.ABI == customerData?.banabi && w.CAB == customerData?.bancab).FirstOrDefault();
                    }
                    _dataContext.Data.FTDE25 = clientData?.CLREAC;
                    _dataContext.Data.FTSCCL = customerData?.CLSCON;
                    _dataContext.SelectedShipment = _dataContext.Shipments?.Where(w => w.specod == customerData?.specod).FirstOrDefault();
                    _dataContext.SelectedDelivery = _dataContext.Deliveries?.Where(w => w.concod == customerData?.concod).FirstOrDefault();
                    _dataContext.SelectedCarrier = _dataContext.Carriers?.Where(w => w.vetcod == customerData?.vetcod).FirstOrDefault();
                    _dataContext.SelectedPacking = _dataContext.Packings?.Where(w => w.imbcod == customerData?.CLIMBA).FirstOrDefault();
                }
            }
            else
            {
                _dataContext.Data.FTCODC = null;
                _dataContext.References = null;
            }
        }
        private void acReference_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (_dataContext.SelectedReference != null && _dataContext.SelectedReference.codesti > 0)
            {
                _dataContext.Data.FTCODD = _dataContext.SelectedReference.codesti;
            }
            else
            { _dataContext.Data.FTCODD = null; }
        }

        private void acCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCausal.fatcod))
            {
                _dataContext.Data.FTCAUS = _dataContext.SelectedCausal.fatcod;
                // set default FE type
                if (string.IsNullOrWhiteSpace(_dataContext.Data.fttdoc))
                {
                    _dataContext.SelectedFEDocType = _dataContext.FEDocTypes?.Where(w => w.FETDCod == _dataContext.SelectedCausal.fattido).FirstOrDefault();
                }
                // set document type
                _dataContext.Data.FTTIPO = _dataContext.SelectedCausal.fattif;
            }
            else
            {
                _dataContext.Data.FTCAUS = null;
                _dataContext.SelectedFEDocType = null;
                _dataContext.Data.FTTIPO = null;
            }
        }

        private void acPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedPayment != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedPayment.pclcod))
            {
                if (_dataContext.Data.FTPAGA != _dataContext.SelectedPayment.pclcod)
                {
                    _dataContext.Data.FTPAGA = _dataContext.SelectedPayment.pclcod;
                    // refresh bank
                    _dataContext.SelectedCustomerBank = null;
                    _dataContext.Banks = _dataContext.GetABICABs(_dataContext.SelectedPayment?.Incasso?.icssup);
                }
            }
            else
            {
                _dataContext.Data.FTPAGA = null;
                _dataContext.Banks = null;
                _dataContext.SelectedCustomerBank = null;
            }
        }

        private void acFEDocType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedFEDocType != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedFEDocType.FETDCod))
            {
                _dataContext.Data.fttdoc = _dataContext.SelectedFEDocType.FETDCod;
            }
            else
            { _dataContext.Data.fttdoc = null; }
        }
        private void acCustomerBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCustomerBank != null && _dataContext.SelectedCustomerBank.ABI > 0)
            {
                _dataContext.Data.FTABIB = _dataContext.SelectedCustomerBank.ABI;
                _dataContext.Data.FTCABB = _dataContext.SelectedCustomerBank.CAB;
                _dataContext.Data.FTBCON = _dataContext.SelectedCustomerBank.Account;
            }
            else
            {
                _dataContext.Data.FTABIB = null;
                _dataContext.Data.FTCABB = null;
                _dataContext.Data.FTBCON = null;
            }
        }
        private void acDelivery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedDelivery != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedDelivery.concod))
            {
                _dataContext.Data.FTCONS = _dataContext.SelectedDelivery.concod;
            }
            else
            { _dataContext.Data.FTCONS = null; }
        }

        private void acShipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedShipment != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedShipment.specod))
            {
                _dataContext.Data.FTSPED = _dataContext.SelectedShipment.specod;
            }
            else
            { _dataContext.Data.FTSPED = null; }
        }

        private void acPacking_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedPacking != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedPacking.imbcod))
            {
                _dataContext.Data.FTIMBL = _dataContext.SelectedPacking.imbcod;
            }
            else
            { _dataContext.Data.FTIMBL = null; }
        }

        private void acCarrier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCarrier != null && _dataContext.SelectedCarrier.vetcod > 0)
            {
                _dataContext.Data.FTCORR = _dataContext.SelectedCarrier.vetcod;
            }
            else
            { _dataContext.Data.FTCORR = null; }
        }

        private void acCostCenter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCostCenter != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCostCenter.cecodc))
            {
                _dataContext.Data.FTCECO = _dataContext.SelectedCostCenter.cecodc;
            }
            else
            { _dataContext.Data.FTCECO = null; }
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
        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }
        #endregion
    }
}
