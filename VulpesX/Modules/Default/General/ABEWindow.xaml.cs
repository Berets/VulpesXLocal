using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Telerik.Windows.Persistence.Core;
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.General;

namespace VulpesX.Modules.Default.General
{
    /// <summary>
    /// Interaction logic for ABEWindow.xaml
    /// </summary>
    public partial class ABEWindow : FluentDefaultWindow, IWindowFactory
    {
        private ABEWindowViewModel _dataContext;

        public ABEWindow(ABEWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);
            this.DataContext = _dataContext;

            this.Title = !_dataContext.IsInsert ? $"Dettagli anagrafica unica {_dataContext.Data.abers1}" : $"Dettagli nuova anagrafica unica";
            // default to first free ID if inserting
            if (_dataContext.IsInsert)
            {
                _dataContext.Data.abecod = _dataContext.GetFreeID();
            }
            // company related lists

            _dataContext.LoadLists();

            if (_dataContext.AZIENDA?.azfatfile ?? false)
                grdFatFile.Visibility = Visibility.Collapsed;

            _dataContext.SelectedInvoicePersonalizationFile = _dataContext.InvoicePersonalizationFiles?.Where(w => w.txtsoci == _dataContext.CompanyID && w.txtid == _dataContext.Data.abfatfileid).FirstOrDefault();

            if (_dataContext.SupplierData != null)
            {
                _dataContext.SelectedSupplierRate = _dataContext.RatesList?.Where(w => w.asscod == _dataContext.SupplierData.foaass?.Trim() && w.assali == _dataContext.SupplierData.foaali?.Trim()).FirstOrDefault();
                _dataContext.SelectedSupplierPayment = _dataContext.SupplierPayments?.Where(w => w.pfocod == _dataContext.SupplierData.pfocod?.Trim()).FirstOrDefault();
                _dataContext.SelectedSupplierExpire = _dataContext.Expires?.Where(w => w.scacod == _dataContext.SupplierData.scacod?.Trim()).FirstOrDefault();
                _dataContext.SelectedSupplierShipment = _dataContext.Shipments?.Where(w => w.specod == _dataContext.SupplierData.specod).FirstOrDefault();
                _dataContext.SelectedSupplierDelivery = _dataContext.Deliveries?.Where(w => w.concod == _dataContext.SupplierData.concod).FirstOrDefault();
                _dataContext.SelectedSupplierZone = _dataContext.Zones?.Where(w => w.zoncod == _dataContext.SupplierData.zoncod).FirstOrDefault();
                _dataContext.SelectedSupplierCarrier = _dataContext.Carriers?.Where(w => w.vetcod == _dataContext.SupplierData.vetcod).FirstOrDefault();
                _dataContext.SelectedSupplierDealer = _dataContext.Dealers?.Where(w => w.rivcod == _dataContext.SupplierData.rivcod).FirstOrDefault();
                _dataContext.SelectedSupplierStore = _dataContext.Stores?.Where(w => w.company_id == _dataContext.CompanyID && w.id == _dataContext.SupplierData.codmag).FirstOrDefault();
                _dataContext.SelectedInternalBank = _dataContext.InternalBanks?.Where(w => w.ABI == _dataContext.SupplierData.FOAABI && w.CAB == _dataContext.SupplierData.FOACAB && w.Account == _dataContext.SupplierData.FOACCN).FirstOrDefault();
                _dataContext.SelectedSupplierCommercialRegion = _dataContext.Regions?.Where(w => w.regcod == _dataContext.SupplierData.FOREGI).FirstOrDefault();
                _dataContext.SelectedSupplierCommercialPacking = _dataContext.Packings?.Where(w => w.imbcod == _dataContext.SupplierData.FOIMBA).FirstOrDefault();
                _dataContext.SelectedBank = _dataContext.Banks?.Where(w => w.ABI == _dataContext.SupplierData.FOABIF && w.CAB == _dataContext.SupplierData.FOCABF).FirstOrDefault();
            }
            if (_dataContext.SupplierCommercialData != null)
            {
                _dataContext.SelectedSupplierCommercialBirthCity = _dataContext.Data.Cities?.Where(w => w.comdes == _dataContext.SupplierCommercialData.FONASC?.Trim()).FirstOrDefault();
                _dataContext.SelectedSupplierCommercialBirthState = _dataContext.Data.States?.Where(w => w.cappro == _dataContext.SupplierCommercialData.FOONAF).FirstOrDefault();
                _dataContext.SelectedSupplierCommercialResidentialCity = _dataContext.Data.Cities?.Where(w => w.comdes == _dataContext.SupplierCommercialData.FOLOCR?.Trim()).FirstOrDefault();
                _dataContext.SelectedSupplierCommercialResidentialState = _dataContext.Data.States?.Where(w => w.cappro == _dataContext.SupplierCommercialData.FOPROR).FirstOrDefault();
                _dataContext.SelectedSupplierCommercialFiscalCity = _dataContext.Data.Cities?.Where(w => w.comdes == _dataContext.SupplierCommercialData.FOLOCD?.Trim()).FirstOrDefault();
                _dataContext.SelectedSupplierCommercialFiscalState = _dataContext.Data.States?.Where(w => w.cappro == _dataContext.SupplierCommercialData.FOPROD).FirstOrDefault();
            }
            if (_dataContext.CustomerData != null)
            {
                _dataContext.SelectedCustomerPayment = _dataContext.CustomerPayments?.Where(w => w.pclcod == _dataContext.CustomerData.pclcod?.Trim()).FirstOrDefault();
                _dataContext.SelectedCustomerExpire = _dataContext.Expires?.Where(w => w.scacod == _dataContext.CustomerData.scacod?.Trim()).FirstOrDefault();
                _dataContext.SelectedCustomerCarrier = _dataContext.Carriers?.Where(w => w.vetcod == _dataContext.CustomerData.vetcod).FirstOrDefault();
                _dataContext.SelectedCustomerDealer = _dataContext.Dealers?.Where(w => w.rivcod == _dataContext.CustomerData.rivcod).FirstOrDefault();
                _dataContext.SelectedCustomerShipment = _dataContext.Shipments?.Where(w => w.specod == _dataContext.CustomerData.specod).FirstOrDefault();
                _dataContext.SelectedCustomerDelivery = _dataContext.Deliveries?.Where(w => w.concod == _dataContext.CustomerData.concod).FirstOrDefault();
                _dataContext.SelectedCustomerReliability = _dataContext.Reliabilities?.Where(w => w.affcod == _dataContext.CustomerData.affcod).FirstOrDefault();
                _dataContext.SelectedCustomerArea = _dataContext.Areas?.Where(w => w.arecod == _dataContext.CustomerData.arecod).FirstOrDefault();
                _dataContext.SelectedCustomerBranch = _dataContext.Branches?.Where(w => w.filcod == _dataContext.CustomerData.filcod).FirstOrDefault();
                _dataContext.SelectedCustomerCategory = _dataContext.Categories?.Where(w => w.catcod == _dataContext.CustomerData.catcod).FirstOrDefault();
                _dataContext.SelectedCustomerClassification = _dataContext.Classifications?.Where(w => w.csfcod == _dataContext.CustomerData.csfcod).FirstOrDefault();
                _dataContext.SelectedCustomerInternalBank = _dataContext.InternalBanks?.Where(w => w.ABI == _dataContext.CustomerData.banabi && w.CAB == _dataContext.CustomerData.bancab && w.Account == _dataContext.CustomerData.bancoc).FirstOrDefault();
                _dataContext.SelectedCustomerCommercialPacking = _dataContext.Packings?.Where(w => w.imbcod == _dataContext.CustomerData.CLIMBA).FirstOrDefault();
                _dataContext.SelectedCustomerFirstAgent = _dataContext.Agents?.Where(w => w.agecod == _dataContext.CustomerData.CLAGEN).FirstOrDefault();
                _dataContext.SelectedCustomerSecondAgent = _dataContext.Agents?.Where(w => w.agecod == _dataContext.CustomerData.CLAGEN2).FirstOrDefault();
                _dataContext.SelectedCustomerZone = _dataContext.Zones?.Where(w => w.zoncod == _dataContext.CustomerData.CLZONE).FirstOrDefault();
                _dataContext.SelectedCustomerDeposit = _dataContext.Deposits?.Where(w => w.depcod == _dataContext.CustomerData.CLDEPO).FirstOrDefault();
                _dataContext.SelectedCustomerRegion = _dataContext.Regions?.Where(w => w.regcod == _dataContext.CustomerData.CLREGI).FirstOrDefault();
                _dataContext.SelectedCustomerCommodity = _dataContext.Commodities?.Where(w => w.smecod == _dataContext.CustomerData.CLSETM).FirstOrDefault();
                _dataContext.SelectedCustomerBank = _dataContext.Banks?.Where(w => w.ABI == _dataContext.CustomerData.CLABI && w.CAB == _dataContext.CustomerData.CLCAB).FirstOrDefault();
            }
            if (_dataContext.CustomerCommercialData != null)
            {
                _dataContext.SelectedCustomerRate = _dataContext.RatesList?.Where(w => w.asscod == _dataContext.CustomerCommercialData.classo?.Trim() && w.assali == _dataContext.CustomerCommercialData.classa?.Trim()).FirstOrDefault();
                _dataContext.SelectedCustomerReminder = _dataContext.Reminders?.Where(w => w.solcod == _dataContext.CustomerCommercialData.solcod).FirstOrDefault();
            }


            if (_dataContext.Data.abold != null)
                _dataContext.SelectedObsoleto = _dataContext.Obsoleti?.Where(o => o.abecod == _dataContext.Data.abold).FirstOrDefault();

            this.DataContext = _dataContext;
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
        }



        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var validate = _dataContext.Validate();
            if (string.IsNullOrWhiteSpace(validate))
            {
                if (!_dataContext.IsInsert)
                {
                    _dataContext.Data.abold = _dataContext.SelectedObsoleto?.abecod;
                    _dataContext.Data.updatedUserID = UserContext.Instance.ACCESS!.USRID;

                    if (_dataContext.Update())
                    {
                        Mouse.OverrideCursor = null;
                        this.DialogResult = true;
                    }
                }
                else
                {
                    _dataContext.Data.abold = _dataContext.SelectedObsoleto?.abecod;
                    _dataContext.Data.addedUserID = UserContext.Instance.ACCESS!.USRID;

                    if (_dataContext.Data.abecfe == "C" || _dataContext.Data.abecfe == "E" || _dataContext.Data.abecfe == "P")
                    {
                        if (_dataContext.CustomerData != null)
                            _dataContext.CustomerData.Cliacod = _dataContext.Data.abecod;
                        if (_dataContext.CustomerCommercialData != null)
                            _dataContext.CustomerCommercialData.CLIENT = _dataContext.Data.abecod;
                    }
                    if (_dataContext.Data.abecfe == "F" || _dataContext.Data.abecfe == "E")
                    {
                        if (_dataContext.SupplierData != null)
                            _dataContext.SupplierData.Foraco = _dataContext.Data.abecod;
                        if (_dataContext.SupplierCommercialData != null)
                            _dataContext.SupplierCommercialData.FOCLIF = _dataContext.Data.abecod;
                    }
                    if (_dataContext.Insert())
                    {
                        Mouse.OverrideCursor = null;
                        _dataContext.AssignedCustomerID = _dataContext.Data.abecod;
                        this.DialogResult = true;
                    }
                }
            }
            else
            { Mouse.OverrideCursor = null; ErrorHandler.Show(validate); }
        }
        #endregion

        #region Autocompletes ABE
        private void acBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierData != null)
            {
                if (_dataContext.SelectedBank != null && _dataContext.SelectedBank.ABI > 0)
                {
                    _dataContext.SupplierData.FOABIF = _dataContext.SelectedBank.ABI;
                    _dataContext.SupplierData.FOCABF = _dataContext.SelectedBank.CAB;
                    if (!string.IsNullOrWhiteSpace(_dataContext.SupplierData.FOCOCO))
                    {
                        var bankInfoComputed = BankHelper.ComputeCINBBANIBAN(_dataContext.SupplierData.FOABIF.Value, _dataContext.SupplierData.FOCABF.Value, _dataContext.SupplierData.FOCOCO, _dataContext.Data.isocod ?? string.Empty);
                        _dataContext.SupplierData.FOCIN = bankInfoComputed.CIN;
                        _dataContext.SupplierData.FOBBAN = bankInfoComputed.BBAN;
                        _dataContext.SupplierData.FOIBAN = bankInfoComputed.IBAN;
                    }
                }
                else
                {
                    _dataContext.SupplierData.FOABIF = null;
                    _dataContext.SupplierData.FOCABF = null;
                    _dataContext.SupplierData.FOCOCO = null;
                    _dataContext.SupplierData.FOCIN = null;
                    _dataContext.SupplierData.FOBBAN = null;
                    _dataContext.SupplierData.FOIBAN = null;
                }
            }
        }
        private void acCustomerBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerBank != null && _dataContext.SelectedCustomerBank.ABI > 0)
                {
                    _dataContext.CustomerData.CLABI = _dataContext.SelectedCustomerBank.ABI;
                    _dataContext.CustomerData.CLCAB = _dataContext.SelectedCustomerBank.CAB;
                    if (!string.IsNullOrWhiteSpace(_dataContext.CustomerData.CLNUCC))
                    {
                        var bankInfoComputed = BankHelper.ComputeCINBBANIBAN(_dataContext.CustomerData.CLABI.Value, _dataContext.CustomerData.CLCAB.Value, _dataContext.CustomerData.CLNUCC, _dataContext.Data.isocod ?? string.Empty);
                        _dataContext.CustomerData.clcin = bankInfoComputed.CIN;
                        _dataContext.CustomerData.clbban = bankInfoComputed.BBAN;
                        _dataContext.CustomerData.cliban = bankInfoComputed.IBAN;
                    }
                }
                else
                {
                    _dataContext.CustomerData.CLABI = null;
                    _dataContext.CustomerData.CLCAB = null;
                    _dataContext.CustomerData.CLNUCC = null;
                    _dataContext.CustomerData.clcin = null;
                    _dataContext.CustomerData.clbban = null;
                    _dataContext.CustomerData.cliban = null;
                }
            }
        }
        private void acInternalBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierData != null)
            {
                if (_dataContext.SelectedInternalBank != null && _dataContext.SelectedInternalBank.ABI > 0)
                {

                    _dataContext.SupplierData.FOAABI = _dataContext.SelectedInternalBank.ABI;
                    _dataContext.SupplierData.FOACAB = _dataContext.SelectedInternalBank.CAB;
                    _dataContext.SupplierData.FOACCN = _dataContext.SelectedInternalBank.Account;
                }
                else
                {
                    _dataContext.SupplierData.FOAABI = null;
                    _dataContext.SupplierData.FOACAB = null;
                    _dataContext.SupplierData.FOACCN = null;
                }
            }
        }
        private void acCustomerInternalBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerInternalBank != null && _dataContext.SelectedCustomerInternalBank.ABI > 0)
                {
                    _dataContext.CustomerData.banabi = _dataContext.SelectedCustomerInternalBank.ABI;
                    _dataContext.CustomerData.bancab = _dataContext.SelectedCustomerInternalBank.CAB;
                    _dataContext.CustomerData.bancoc = _dataContext.SelectedCustomerInternalBank.Account;
                }
                else
                {
                    _dataContext.CustomerData.banabi = null;
                    _dataContext.CustomerData.bancab = null;
                    _dataContext.CustomerData.bancoc = null;
                }
            }
        }
        private void acSupplierRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierData != null)
            {
                if (_dataContext.SelectedSupplierRate != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierRate.asscod))
                {
                    _dataContext.SupplierData.foaass = _dataContext.SelectedSupplierRate.asscod;
                    _dataContext.SupplierData.foaali = _dataContext.SelectedSupplierRate.assali;
                }
                else
                {
                    _dataContext.SupplierData.foaass = null;
                    _dataContext.SupplierData.foaali = null;
                }
            }
        }
        private void acSupplierPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierData != null)
            {
                if (_dataContext.SelectedSupplierPayment != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierPayment.pfocod))
                {
                    _dataContext.SupplierData.pfocod = _dataContext.SelectedSupplierPayment.pfocod;
                }
                else
                { _dataContext.SupplierData.pfocod = null; }
            }
        }
        private void acCustomerPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerPayment != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCustomerPayment.pclcod))
                {
                    _dataContext.CustomerData.pclcod = _dataContext.SelectedCustomerPayment.pclcod;
                }
                else
                { _dataContext.CustomerData.pclcod = null; }
            }
        }
        private void acSupplierExpire_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierData != null)
            {
                if (_dataContext.SelectedSupplierExpire != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierExpire.scacod))
                {
                    _dataContext.SupplierData.scacod = _dataContext.SelectedSupplierExpire.scacod;
                }
                else
                {
                    _dataContext.SupplierData.scacod = null;
                }
            }
        }
        private void acCustomerExpire_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerExpire != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCustomerExpire.scacod))
                {
                    _dataContext.CustomerData.scacod = _dataContext.SelectedCustomerExpire.scacod;
                }
                else
                {
                    _dataContext.CustomerData.scacod = null;
                }
            }
        }
        private void acSupplierShipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierData != null)
            {
                if (_dataContext.SelectedSupplierShipment != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierShipment.specod))
                {
                    _dataContext.SupplierData.specod = _dataContext.SelectedSupplierShipment.specod;
                }
                else
                { _dataContext.SupplierData.specod = null; }
            }
        }
        private void acSupplierDelivery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierData != null)
            {
                if (_dataContext.SelectedSupplierDelivery != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierDelivery.concod))
                {
                    _dataContext.SupplierData.concod = _dataContext.SelectedSupplierDelivery.concod;
                }
                else
                { _dataContext.SupplierData.concod = null; }
            }
        }
        private void acSupplierCarrier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierData != null)
            {
                if (_dataContext.SelectedSupplierCarrier != null)
                {
                    _dataContext.SupplierData.vetcod = _dataContext.SelectedSupplierCarrier.vetcod;
                }
                else
                { _dataContext.SupplierData.vetcod = null; }
            }
        }
        private void acSupplierStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierData != null)
            {
                if (_dataContext.SelectedSupplierStore != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierStore.id))
                {
                    _dataContext.SupplierData.codmag = _dataContext.SelectedSupplierStore.id;
                }
                else
                { _dataContext.SupplierData.codmag = null; }
            }
        }
        private void acSupplierZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierData != null)
            {
                if (_dataContext.SelectedSupplierZone != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierZone.zoncod))
                {
                    _dataContext.SupplierData.zoncod = _dataContext.SelectedSupplierZone.zoncod;
                }
                else
                { _dataContext.SupplierData.zoncod = null; }
            }
        }
        private void acSupplierDealer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierData != null)
            {
                if (_dataContext.SelectedSupplierDealer != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierDealer.rivcod))
                {
                    _dataContext.SupplierData.rivcod = _dataContext.SelectedSupplierDealer.rivcod;
                }
                else
                { _dataContext.SupplierData.rivcod = null; }
            }
        }
        private void acCustomerShipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerShipment != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCustomerShipment.specod))
                {
                    _dataContext.CustomerData.specod = _dataContext.SelectedCustomerShipment.specod;
                }
                else
                { _dataContext.CustomerData.specod = null; }
            }
        }
        private void acCustomerDelivery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerDelivery != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCustomerDelivery.concod))
                {
                    _dataContext.CustomerData.concod = _dataContext.SelectedCustomerDelivery.concod;
                }
                else
                { _dataContext.CustomerData.concod = null; }
            }
        }
        private void acCustomerCarrier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerCarrier != null)
                {
                    _dataContext.CustomerData.vetcod = _dataContext.SelectedCustomerCarrier.vetcod;
                }
                else
                { _dataContext.CustomerData.vetcod = null; }
            }
        }
        private void acCustomerDealer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerDealer != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCustomerDealer.rivcod))
                {
                    _dataContext.CustomerData.rivcod = _dataContext.SelectedCustomerDealer.rivcod;
                }
                else
                { _dataContext.CustomerData.rivcod = null; }
            }
        }
        private void acCustomerReliability_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerReliability != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCustomerReliability.affcod))
                {
                    _dataContext.CustomerData.affcod = _dataContext.SelectedCustomerReliability.affcod;
                }
                else
                { _dataContext.CustomerData.affcod = null; }
            }
        }
        private void acCustomerArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerArea != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCustomerArea.arecod))
                {
                    _dataContext.CustomerData.arecod = _dataContext.SelectedCustomerArea.arecod;
                }
                else
                { _dataContext.CustomerData.arecod = null; }
            }
        }
        private void acCustomerBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerBranch != null)
                {
                    _dataContext.CustomerData.filcod = _dataContext.SelectedCustomerBranch.filcod;
                }
                else
                { _dataContext.CustomerData.filcod = null; }
            }
        }
        private void acCustomerCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerCategory != null)
                {
                    _dataContext.CustomerData.catcod = _dataContext.SelectedCustomerCategory.catcod;
                }
                else
                { _dataContext.CustomerData.catcod = null; }
            }
        }
        private void acCustomerClassification_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerClassification != null)
                {
                    _dataContext.CustomerData.csfcod = _dataContext.SelectedCustomerClassification.csfcod;
                }
                else
                { _dataContext.CustomerData.csfcod = null; }
            }
        }
        private void acCustomerRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerCommercialData != null)
            {
                if (_dataContext.SelectedCustomerRate != null)
                {
                    _dataContext.CustomerCommercialData.classo = _dataContext.SelectedCustomerRate.asscod;
                    _dataContext.CustomerCommercialData.classa = _dataContext.SelectedCustomerRate.assali;
                }
                else
                {
                    _dataContext.CustomerCommercialData.classo = null;
                    _dataContext.CustomerCommercialData.classa = null;
                }
            }
        }
        private void acCustomerFirstAgent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerFirstAgent != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCustomerFirstAgent.agecod))
                {
                    if (_dataContext.CustomerData.CLAGEN != _dataContext.SelectedCustomerFirstAgent.agecod)
                    {
                        _dataContext.CustomerData.CLAGEN = _dataContext.SelectedCustomerFirstAgent.agecod;
                        _dataContext.CustomerData.CLAGENP = _dataContext.SelectedCustomerFirstAgent.agepvg;
                        _dataContext.CustomerData.CLAGENPT = _dataContext.SelectedCustomerFirstAgent.agepvgt;
                    }
                }
                else
                {
                    _dataContext.CustomerData.CLAGEN = null;
                    _dataContext.CustomerData.CLAGENP = null;
                    _dataContext.CustomerData.CLAGENPT = null;
                }
            }
        }
        private void acCustomerSecondAgent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerSecondAgent != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCustomerSecondAgent.agecod))
                {
                    if (_dataContext.CustomerData.CLAGEN2 != _dataContext.SelectedCustomerSecondAgent.agecod)
                    {
                        _dataContext.CustomerData.CLAGEN2 = _dataContext.SelectedCustomerSecondAgent.agecod;
                        _dataContext.CustomerData.CLAGEN2P = _dataContext.SelectedCustomerSecondAgent.agepvg;
                        _dataContext.CustomerData.CLAGEN2PT = _dataContext.SelectedCustomerSecondAgent.agepvgt;
                    }
                }
                else
                {
                    _dataContext.CustomerData.CLAGEN2 = null;
                    _dataContext.CustomerData.CLAGEN2P = null;
                    _dataContext.CustomerData.CLAGEN2PT = null;
                }
            }
        }
        private void acCustomerReminder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerCommercialData != null)
            {
                if (_dataContext.SelectedCustomerReminder != null && _dataContext.SelectedCustomerReminder.solcod > 0)
                {
                    _dataContext.CustomerCommercialData.solcod = _dataContext.SelectedCustomerReminder.solcod;
                }
                else
                { _dataContext.CustomerCommercialData.solcod = null; }
            }
        }
        private void acCustomerDeposit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerDeposit != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCustomerDeposit.depcod))
                {
                    _dataContext.CustomerData.CLDEPO = _dataContext.SelectedCustomerDeposit.depcod;
                }
                else
                { _dataContext.CustomerData.CLDEPO = null; }
            }
        }
        private void acCustomerRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerRegion != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCustomerRegion.regcod))
                {
                    _dataContext.CustomerData.CLREGI = _dataContext.SelectedCustomerRegion.regcod;
                }
                else
                { _dataContext.CustomerData.CLREGI = null; }
            }
        }
        private void acCustomerZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerZone != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCustomerZone.zoncod))
                {
                    _dataContext.CustomerData.CLZONE = _dataContext.SelectedCustomerZone.zoncod;
                }
                else
                { _dataContext.CustomerData.CLZONE = null; }
            }
        }
        private void acCustomerCommodity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerCommodity != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCustomerCommodity.smecod))
                {
                    _dataContext.CustomerData.CLSETM = _dataContext.SelectedCustomerCommodity.smecod;
                }
                else
                { _dataContext.CustomerData.CLSETM = null; }
            }
        }
        private void acSupplierBirthCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierCommercialData != null)
            {
                if (_dataContext.SelectedSupplierCommercialBirthCity != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierCommercialBirthCity.comdes))
                {
                    _dataContext.SupplierCommercialData.FONASC = _dataContext.SelectedSupplierCommercialBirthCity.comdes;
                }
                else
                { _dataContext.SupplierCommercialData.FONASC = null; }
            }
        }
        private void acSupplierBirthState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierCommercialData != null)
            {
                if (_dataContext.SelectedSupplierCommercialBirthState != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierCommercialBirthState.cappro))
                {
                    _dataContext.SupplierCommercialData.FOONAF = _dataContext.SelectedSupplierCommercialBirthState.cappro;
                }
                else
                {
                    _dataContext.SupplierCommercialData.FOONAF = null;
                }
            }
        }
        private void acSupplierResidentialState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierCommercialData != null)
            {
                if (_dataContext.SelectedSupplierCommercialResidentialState != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierCommercialResidentialState.cappro))
                {
                    _dataContext.SupplierCommercialData.FOPROR = _dataContext.SelectedSupplierCommercialResidentialState.cappro;
                }
                else
                {
                    _dataContext.SupplierCommercialData.FOPROR = null;
                }
            }
        }
        private void acSupplierResidentialCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierCommercialData != null)
            {
                if (_dataContext.SelectedSupplierCommercialResidentialCity != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierCommercialResidentialCity.comdes))
                {
                    _dataContext.SupplierCommercialData.FOLOCR = _dataContext.SelectedSupplierCommercialResidentialCity.comdes;
                }
                else
                { _dataContext.SupplierCommercialData.FOLOCR = null; }
            }
        }
        private void acSupplierFiscalCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierCommercialData != null)
            {
                if (_dataContext.SelectedSupplierCommercialFiscalCity != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierCommercialFiscalCity.comdes))
                {
                    _dataContext.SupplierCommercialData.FOLOCD = _dataContext.SelectedSupplierCommercialFiscalCity.comdes;
                }
                else
                { _dataContext.SupplierCommercialData.FOLOCD = null; }
            }
        }
        private void acSupplierFiscalState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierCommercialData != null)
            {
                if (_dataContext.SelectedSupplierCommercialFiscalState != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierCommercialFiscalState.cappro))
                {
                    _dataContext.SupplierCommercialData.FOPROD = _dataContext.SelectedSupplierCommercialFiscalState.cappro;
                }
                else
                {
                    _dataContext.SupplierCommercialData.FOPROD = null;
                }
            }
        }
        private void acSupplierCommercialRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierData != null)
            {
                if (_dataContext.SelectedSupplierCommercialRegion != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierCommercialRegion.regcod))
                {
                    _dataContext.SupplierData.FOREGI = _dataContext.SelectedSupplierCommercialRegion.regcod;
                }
                else
                { _dataContext.SupplierData.FOREGI = null; }
            }
        }
        private void acSupplierCommercialPacking_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SupplierData != null)
            {
                if (_dataContext.SelectedSupplierCommercialPacking != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSupplierCommercialPacking.imbcod))
                {
                    _dataContext.SupplierData.FOIMBA = _dataContext.SelectedSupplierCommercialPacking.imbcod;
                }
                else
                { _dataContext.SupplierData.FOIMBA = null; }
            }
        }
        private void acCustomerPacking_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.CustomerData != null)
            {
                if (_dataContext.SelectedCustomerCommercialPacking != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCustomerCommercialPacking.imbcod))
                {
                    _dataContext.CustomerData.CLIMBA = _dataContext.SelectedCustomerCommercialPacking.imbcod;
                }
                else
                { _dataContext.CustomerData.CLIMBA = null; }
            }
        }
        private void acFatFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedInvoicePersonalizationFile != null)
            {
                _dataContext.Data.abfatfileid = _dataContext.SelectedInvoicePersonalizationFile.txtid;
            }
            else
            { _dataContext.Data.abfatfileid = null; }
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


        #region Customer recipients grid events
        private void rgvRecipients_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {

            var data = new DESTINATARI { ragisoc = _dataContext.CompanyID };
            var items = rgvRecipients.Items.Cast<DESTINATARI>();
            data.cliecod = _dataContext.Data.abecod;
            data.codesti = items.Any() ? items.Max(max => max.codesti) + 1 : 1;
            data.Cities = _dataContext.Data.Cities;
            data.States = _dataContext.Data.States;
            data.AgentsList = _dataContext.Agents;
            data.Isos = _dataContext.Data.ISOList;

            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[1];
        }
        private void rgvRecipients_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType == Telerik.Windows.Controls.GridView.GridViewEditOperationType.Edit)
            {
                var data = e.Row.Item as DESTINATARI;
                var validated = _dataContext.DestinatariValidate(data!, true);
                if (!string.IsNullOrWhiteSpace(validated))
                {
                    Dispatcher.BeginInvoke(() => { ErrorHandler.Show(validated); });
                }
                e.IsValid = string.IsNullOrWhiteSpace(validated);
            }
            else
            {
                e.IsValid = true;
            }
        }
        private void rgvRecipients_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvRecipients.ScrollIntoView(e.Row.Item, rgvRecipients.Columns[0]);
        }

        private void rgvRecipients_Deleting(object sender, GridViewDeletingEventArgs e)
        {
            if (e.Items != null && e.Items.Count() > 0)
            {
                var errorMessage = _dataContext.DestinatariCanDelete(e.Items.Cast<DESTINATARI>().First());
                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    // check temporary extern
                    if (!_dataContext.ExternalCodes?.Any(any => any.Destinations?.Any(inany => inany.abedestid == e.Items.Cast<DESTINATARI>().First().codesti) ?? false) ?? false)
                    {
                        e.Handled = true;
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Show("Impossibile eliminare questa destinazione perchè ha riferimenti nei codici esterni"); });
                        e.Cancel = true;
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(() => { ErrorHandler.Show(errorMessage); });
                    e.Cancel = true;
                }
            }
        }
        #endregion

        #region Supplier recipients grid events
        private void rgvSupplierRecipients_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {

            var data = new DESFOR();
            var items = rgvSupplierRecipients.Items.Cast<DESFOR>();
            data.fornicod = _dataContext.Data.abecod;
            data.fodesti = items.Any() ? items.Max(max => max.fodesti) + 1 : 1;
            data.Cities = _dataContext.Data.Cities;
            data.States = _dataContext.Data.States;
            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[1];
        }
        private void rgvSupplierRecipients_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType == Telerik.Windows.Controls.GridView.GridViewEditOperationType.Edit)
            {
                var data = e.Row.Item as DESFOR;
                var validated = _dataContext.DesForValidate(data!, true);
                if (!string.IsNullOrWhiteSpace(validated))
                {
                    Dispatcher.BeginInvoke(() => { ErrorHandler.Show(validated); });
                }
                e.IsValid = string.IsNullOrWhiteSpace(validated);
            }
            else
            {
                e.IsValid = true;
            }
        }
        private void rgvSupplierRecipients_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvSupplierRecipients.ScrollIntoView(e.Row.Item, rgvSupplierRecipients.Columns[0]);
        }
        #endregion

        #region Notes
        private void cmdEditCustomerNote_Click(object sender, RoutedEventArgs e)
        {
            var selected = (sender as Button)!.DataContext as NOTECLI1;

            if (selected != null)
            {
                NOTECLI1 older = new NOTECLI1()
                {
                    NOTCLI = selected.NOTCLI,
                    notdes = selected.notdes,
                    NOTETI = selected.NOTETI,
                    NOTRAG = selected.NOTRAG
                };

                var noteCli1WindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<NOTECLI1WindowViewModel>();
                noteCli1WindowViewModel.Data = selected;
                noteCli1WindowViewModel.IsInsert = false;

                var wNOTECLI1 = new NOTECLI1Window(noteCli1WindowViewModel);
                wNOTECLI1.Owner = Window.GetWindow(this);
                if (wNOTECLI1.ShowDialog() == false)
                {
                    if (_dataContext.CustomerNotes != null)
                    {
                        _dataContext.CustomerNotes.Remove(selected);
                        _dataContext.CustomerNotes.Add(older);
                    }
                }
            }
        }
        private void cmdEditSupplierNote_Click(object sender, RoutedEventArgs e)
        {
            var selected = (sender as Button)!.DataContext as NOTEFOR;

            if (selected != null)
            {
                NOTEFOR older = new NOTEFOR()
                {
                    Nofcod = selected.Nofcod,
                    Nofnot = selected.Nofnot
                };

                var noteForWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<NOTEFORWindowViewModel>();
                noteForWindowViewModel.Data = selected;
                noteForWindowViewModel.IsInsert = false;


                var wNOTEFOR = new NOTEFORWindow(noteForWindowViewModel);
                wNOTEFOR.Owner = Window.GetWindow(this);
                if (wNOTEFOR.ShowDialog() == false)
                {
                    if (_dataContext.SupplierNotes != null)
                    {
                        _dataContext.SupplierNotes.Remove(selected);
                        _dataContext.SupplierNotes.Add(older);
                    }
                }
            }
        }
        private void cmdInsertCustomerNote_Click(object sender, RoutedEventArgs e)
        {
            var data = new NOTECLI1()
            {
                NOTETI = "G",
                NOTCLI = _dataContext.Data.abecod
            };
            var noteCli1WindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<NOTECLI1WindowViewModel>();
            noteCli1WindowViewModel.Data = data;
            noteCli1WindowViewModel.IsInsert = true;

            var wNOTECLI1 = new NOTECLI1Window(noteCli1WindowViewModel);
            wNOTECLI1.Owner = Window.GetWindow(this);
            if (wNOTECLI1.ShowDialog() == true)
            {
                if (_dataContext.CustomerNotes != null)
                    _dataContext.CustomerNotes.Add(data);
            }
        }
        private void cmdInsertSupplierNote_Click(object sender, RoutedEventArgs e)
        {
            var data = new NOTEFOR()
            {
                Nofcod = _dataContext.Data.abecod
            };

            var noteForWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<NOTEFORWindowViewModel>();
            noteForWindowViewModel.Data = data;
            noteForWindowViewModel.IsInsert = true;

            var wNOTEFOR = new NOTEFORWindow(noteForWindowViewModel);
            wNOTEFOR.Owner = Window.GetWindow(this);
            if (wNOTEFOR.ShowDialog() == true)
            {
                if (_dataContext.SupplierNotes != null)
                    _dataContext.SupplierNotes.Add(data);
            }
        }
        private void cmdDeleteCustomerNote_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as NOTECLI1;
            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminazione della nota selezionata ?"))
                {
                    if (_dataContext.CustomerNotes != null)
                        _dataContext.CustomerNotes.Remove(item);
                }
            }
        }
        private void cmdDeleteSupplierNote_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as NOTEFOR;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminazione della nota selezionata ?"))
                {
                    if (_dataContext.SupplierNotes != null)
                        _dataContext.SupplierNotes.Remove(item);
                }
            }
        }
        #endregion

        #region Supplier references
        private void rgvSupplierRefs_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            e.NewObject = new RFFTB00F { rffcgn = string.Empty, rffnom = string.Empty, rffqal = string.Empty };
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }
        private void rgvSupplierRefs_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as RFFTB00F;
                var error = _dataContext.RFTTB00FValidate(item!, true);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    ErrorHandler.Show(error);
                }
                e.IsValid = string.IsNullOrWhiteSpace(error);
            }
            else
            { e.IsValid = true; }
        }
        private void rgvSupplierRefs_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvSupplierRefs.ScrollIntoView(e.Row.Item, rgvSupplierRefs.Columns[0]);
        }
        #endregion

        #region Customer references
        private void rgvCustomerRefs_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            e.NewObject = new ANDEFRES { clirco = string.Empty, clirte = string.Empty };
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }
        private void rgvCustomerRefs_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as ANDEFRES;
                var error = _dataContext.ANDEFRESValidate(item!, true);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    ErrorHandler.Show(error);
                }
                e.IsValid = string.IsNullOrWhiteSpace(error);
            }
            else
            { e.IsValid = true; }
        }
        private void rgvCustomerRefs_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvCustomerRefs.ScrollIntoView(e.Row.Item, rgvCustomerRefs.Columns[0]);
        }
        #endregion

        private void cmdFreeIDs_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var abeFreeWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ABEFreeWindowViewModel>();

            var wABEFreeIDs = new ABEFreeWindow(abeFreeWindowViewModel);
            wABEFreeIDs.Owner = GetWindow(this);
            Mouse.OverrideCursor = null;
            if (wABEFreeIDs.ShowDialog() == true)
            {
                _dataContext.Data.abecod = int.Parse(wABEFreeIDs.Tag.ToString()!);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.UndoAction != UndoAction.Clear && _dataContext.CustomerData != null)
            {
                if (_dataContext.CustomerData.CLABI.HasValue && _dataContext.CustomerData.CLCAB.HasValue && !string.IsNullOrWhiteSpace((e.Source as TextBox)!.Text))
                {
                    var bankInfoComputed = BankHelper.ComputeCINBBANIBAN(_dataContext.CustomerData.CLABI.Value, _dataContext.CustomerData.CLCAB.Value, (e.Source as TextBox)!.Text, _dataContext.Data.isocod ?? string.Empty);
                    _dataContext.CustomerData.clcin = bankInfoComputed.CIN;
                    _dataContext.CustomerData.clbban = bankInfoComputed.BBAN;
                    _dataContext.CustomerData.cliban = bankInfoComputed.IBAN;
                }
            }
        }

        #region Counterparts customer grid
        private void rgvCounterpartsRowsCustomer_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {

            var data = new CUSTOMER_GROUPS { cccont = string.Empty, cccsoc = _dataContext.CompanyID, ccgrup = string.Empty, ccsott = string.Empty };
            data.IsInsert = true;
            data.ccprog = (_dataContext.CounterpartsRowsCustomer ?? new ObservableCollection<CUSTOMER_GROUPS>()).Any() ? _dataContext.CounterpartsRowsCustomer!.Max(max => max.ccprog) + 1 : 1;
            data.cccsoc = _dataContext.CompanyID;
            data.cccode = _dataContext.Data.abecod;
            data.ccsegn = "D";
            data.AccountCache = _dataContext.AccountCache;
            data.SubaccountCache = _dataContext.SubaccountCache;
            data.GroupsList = _dataContext.GroupsList;
            data.CausalsList = _dataContext.CausalsList;
            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void rgvCounterpartsRowsCustomer_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var item = e.Row.Item as CUSTOMER_GROUPS;

            var validated = _dataContext.CUSTOMER_GROUPValidate(item!, item!.IsInsert);
            if (validated != null)
            {
                Dispatcher.BeginInvoke(() => { ErrorHandler.Show(validated); });
            }
            e.IsValid = string.IsNullOrWhiteSpace(validated);
        }

        private void rgvCounterpartsRowsCustomer_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            var data = e.Row.Item as CUSTOMER_GROUPS;

            if (data != null)
            {
                data.ccgrup = data.SelectedGroup!.P1GRUP;
                data.cccont = data.SelectedAccount!.P2CONT;
                data.ccsott = data.SelectedSubaccount!.P3SOTC;
                data.cccaus = data.SelectedCausal?.caucod;
            }
        }
        private void rgvCounterpartsRowsCustomer_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvCounterpartsRowsCustomer.ScrollIntoView(e.Row.Item, rgvCounterpartsRowsCustomer.Columns[0]);
        }
        #endregion

        #region Counterparts supplier grid
        private void rgvCounterpartsRows_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {

            var data = new SUPPLIER_GROUPS { ccfsoc = _dataContext.CompanyID, cfcont = string.Empty, cfgrup = string.Empty, cfsott = string.Empty };
            data.IsInsert = true;
            data.cfprog = (_dataContext.CounterpartsRows ?? new ObservableCollection<SUPPLIER_GROUPS>()).Any() ? _dataContext.CounterpartsRows!.Max(max => max.cfprog) + 1 : 1;
            data.ccfsoc = _dataContext.CompanyID;
            data.cfcode = _dataContext.Data.abecod;
            data.cfsegn = "D";
            data.AccountCache = _dataContext.AccountCache;
            data.SubaccountCache = _dataContext.SubaccountCache;
            data.GroupsList = _dataContext.GroupsList;
            data.CausalsList = _dataContext.CausalsList;
            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void rgvCounterpartsRows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var item = e.Row.Item as SUPPLIER_GROUPS;

            var validated = _dataContext.SUPPLIER_GROUPValidate(item!, item!.IsInsert);
            if (validated != null)
            {
                Dispatcher.BeginInvoke(() => { ErrorHandler.Show(validated); });
            }
            e.IsValid = string.IsNullOrWhiteSpace(validated);
        }

        private void rgvCounterpartsRows_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            var data = e.Row.Item as SUPPLIER_GROUPS;

            if (data != null)
            {
                data.cfgrup = data.SelectedGroup!.P1GRUP;
                data.cfcont = data.SelectedAccount!.P2CONT;
                data.cfsott = data.SelectedSubaccount!.P3SOTC;
                data.cfcaus = data.SelectedCausal?.caucod;
            }
        }

        private void rgvCounterpartsRows_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvCounterpartsRows.ScrollIntoView(e.Row.Item, rgvCounterpartsRows.Columns[0]);
        }
        #endregion

        private void cmbEntityUserType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems[0] as GenericIDDescription;
            if (!_dataContext.IsInsert && newValue != null)
            {
                // create needed class
                // customer
                if (_dataContext.CustomerData == null)
                    _dataContext.CustomerData = new CLIAMMI()
                    {
                        Cliasoc = _dataContext.CompanyID,
                        Cliacod = _dataContext.Data.abecod,
                        AccountCache = _dataContext.AccountCache,
                        SubaccountCache = _dataContext.SubaccountCache,
                        GroupsList = _dataContext.GroupsList
                    };
                if (_dataContext.CustomerCommercialData == null)
                    _dataContext.CustomerCommercialData = new CLIENTI() { CLIENT = _dataContext.Data.abecod };
                if (_dataContext.CustomerReferences == null)
                    _dataContext.CustomerReferences = new ObservableCollection<ANDEFRES>();
                if (_dataContext.CounterpartsRowsCustomer == null)
                    _dataContext.CounterpartsRowsCustomer = new ObservableCollection<CUSTOMER_GROUPS>();
                if (_dataContext.CustomerNotes == null)
                    _dataContext.CustomerNotes = new ObservableCollection<NOTECLI1>();
                if (_dataContext.Recipients == null)
                    _dataContext.Recipients = new ObservableCollection<DESTINATARI>();
                // supplier
                if (_dataContext.SupplierData == null)
                    _dataContext.SupplierData = new FORNAMMI()
                    {
                        Foraso = _dataContext.CompanyID,
                        Foraco = _dataContext.Data.abecod,
                        AccountCache = _dataContext.AccountCache,
                        SubaccountCache = _dataContext.SubaccountCache,
                        GroupsList = _dataContext.GroupsList
                    };
                if (_dataContext.SupplierCommercialData == null)
                    _dataContext.SupplierCommercialData = new FORNITORI() { FOCLIF = _dataContext.Data.abecod };
                if (_dataContext.SupplierReferences == null)
                    _dataContext.SupplierReferences = new ObservableCollection<RFFTB00F>();
                if (_dataContext.CounterpartsRows == null)
                    _dataContext.CounterpartsRows = new ObservableCollection<SUPPLIER_GROUPS>();
                if (_dataContext.SupplierNotes == null)
                    _dataContext.SupplierNotes = new ObservableCollection<NOTEFOR>();
                if (_dataContext.SupplierRecipients == null)
                    _dataContext.SupplierRecipients = new ObservableCollection<DESFOR>();
                if (e.RemovedItems != null && e.RemovedItems.Count > 0 && newValue.ID != "E")
                {
                    var removed = (e.RemovedItems[0] as GenericIDDescription);

                    if (removed != null)
                    {
                        switch (removed.ID)
                        {
                            case "E":
                                if (newValue.ID == "C")
                                    InfoHandler.Show("ATTENZIONE, passando da [Entrambi] a solo [Cliente] se presenti delle informazioni come fornitore verranno perse irrimediabilmente");
                                else
                                    InfoHandler.Show("ATTENZIONE, passando da [Entrambi] a solo [Fornitore] se presenti delle informazioni come cliente verranno perse irrimediabilmente");
                                break;
                            case "F":
                                InfoHandler.Show("ATTENZIONE, passando da [Fornitore] a [Cliente] se presenti delle informazioni come fornitore verranno perse irrimediabilmente");
                                break;
                            case "C":
                                InfoHandler.Show("ATTENZIONE, passando da [Cliente] a [Fornitore] se presenti delle informazioni come cliente verranno perse irrimediabilmente");
                                break;
                        }
                    }
                }
            }
        }

        #region Extern codes grid
        private void rgvExtern_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var model = this.DataContext as ABEViewModel;
            var data = new ABE_EXTERN { abeextcode = string.Empty, abeextid = string.Empty, };
            data.abecod = _dataContext.Data.abecod;
            data.Destinations = new ObservableCollection<ABE_EXTERN_DESTS>();
            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void rgvExtern_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.OldValues != null)
            {
                var item = e.Row.Item as ABE_EXTERN;

                var validated = _dataContext.ABE_EXTERNValidate(item!, true);

                if (!string.IsNullOrWhiteSpace(validated))
                {
                    Dispatcher.BeginInvoke(() => { ErrorHandler.Show(validated); });
                    e.IsValid = false;
                }
                else
                {
                    var exts = rgvExtern.ItemsSource as ObservableCollection<ABE_EXTERN>;

                    if (exts?.Where(w => w.abeextcode.ToLower() == item!.abeextcode.ToLower()).Count() > 1)
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Show("Il codice inserito è già in uso o non è valido"); });
                        e.IsValid = false;
                    }
                    else
                    {
                        e.IsValid = true;
                    }
                }
            }
            else
            {
                e.IsValid = true;
            }
        }

        private void rgvExtern_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvExtern.ScrollIntoView(e.Row.Item, rgvExtern.Columns[0]);
        }
        #endregion

        #region Extern destinations codes grid
        private void rgvExternDest_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            if (rgvExtern.SelectedItem != null)
            {

                var data = new ABE_EXTERN_DESTS { abeextid = string.Empty, abeextcode = string.Empty, abeextdid = string.Empty };
                var ext = (rgvExtern.SelectedItem as ABE_EXTERN);

                if (ext != null)
                {
                    data.abecod = ext.abecod;
                    data.abeextcode = ext.abeextcode;
                    data.abeextid = ext.abeextid;
                    data.DestinationsList = _dataContext.Recipients;
                    e.NewObject = data;
                    var grid = e.OwnerGridViewItemsControl;
                    grid.CurrentColumn = grid.Columns[0];
                }
            }
            else
            {
                e.Cancel = true;
            }
        }
        private void rgvExternDest_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.OldValues != null)
            {
                var item = e.Row.Item as ABE_EXTERN_DESTS;

                var validated = _dataContext.ABE_EXTERN_DESTValidate(item!, true);

                if (!string.IsNullOrWhiteSpace(validated))
                {
                    Dispatcher.BeginInvoke(() => { ErrorHandler.Show(validated); });
                    e.IsValid = false;
                }
                else
                {
                    if ((rgvExternDest.ItemsSource as ObservableCollection<ABE_EXTERN_DESTS>)?.Where(w => w.abeextdid.ToLower() == item!.abeextdid.ToLower()).Count() > 1 ||
                        (rgvExternDest.ItemsSource as ObservableCollection<ABE_EXTERN_DESTS>)?.Where(w => w.abeextcode.ToLower() == item!.abeextcode.ToLower() && w.abeextid == item.abeextid && w.abedestid == item.abedestid).Count() > 1)
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Show("Il codice inserito è già in uso o non è valido"); });
                        e.IsValid = false;
                    }
                    else
                    {
                        e.IsValid = true;
                    }
                }
            }
            else
            {
                e.IsValid = true;
            }
        }
        private void rgvExternDest_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvExternDest.ScrollIntoView(e.Row.Item, rgvExternDest.Columns[0]);
        }
        #endregion

        private void togGroupInvoice_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_dataContext.CustomerData != null)
                _dataContext.CustomerData.CLRAGFD = false;
        }

        private void acCity_TextChanged(object sender, Telerik.Windows.Controls.AutoSuggestBox.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(acCity.Text))
            {
                acCity.ItemsSource = _dataContext.Data.Cities;
            }
            else
            {
                if (e.Reason == Telerik.Windows.Controls.AutoSuggestBox.TextChangeReason.UserInput)
                {
                    acCity.ItemsSource = _dataContext.Data.Cities?.Where(o => o.FullDescriptionSearchable.ToLower().Contains(acCity.Text.ToLower()));
                }
            }
        }


        private void acDestCity_TextChanged(object sender, Telerik.Windows.Controls.AutoSuggestBox.TextChangedEventArgs e)
        {
            var autoSuggest = (sender as RadAutoSuggestBox);

            if (autoSuggest != null)
            {
                if (string.IsNullOrEmpty(autoSuggest.Text))
                {
                    autoSuggest.ItemsSource = _dataContext.Data.Cities;
                }
                else
                {
                    if (e.Reason == Telerik.Windows.Controls.AutoSuggestBox.TextChangeReason.UserInput)
                    {
                        autoSuggest.ItemsSource = _dataContext.Data.Cities?.Where(o => o.FullDescriptionSearchable.ToLower().Contains(autoSuggest.Text.ToLower()));
                    }
                }
            }
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            if (e.UndoAction != UndoAction.Clear && _dataContext.SupplierData != null)
            {
                if (_dataContext.SupplierData.FOABIF.HasValue && _dataContext.SupplierData.FOCABF.HasValue && !string.IsNullOrWhiteSpace((e.Source as TextBox)!.Text))
                {
                    var bankInfoComputed = BankHelper.ComputeCINBBANIBAN(_dataContext.SupplierData.FOABIF.Value, _dataContext.SupplierData.FOCABF.Value, (e.Source as TextBox)!.Text.ToUpper(), _dataContext.Data.isocod ?? string.Empty);
                    _dataContext.SupplierData.FOCIN = bankInfoComputed.CIN;
                    _dataContext.SupplierData.FOBBAN = bankInfoComputed.BBAN;
                    _dataContext.SupplierData.FOIBAN = bankInfoComputed.IBAN;
                }
            }
        }
    }
}
