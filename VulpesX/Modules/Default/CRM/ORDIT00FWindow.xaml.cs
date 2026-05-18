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
using VulpesX.Modules.Default.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for ORDIT00FWindow.xaml
    /// </summary>
    public partial class ORDIT00FWindow : FluentDefaultWindow
    {
        private ORDIT00FWindowViewModel _dataContext;

        public ORDIT00FWindow(ORDIT00FWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedCausal = _dataContext.Causals?.Where(w => w.cauacq == _dataContext.Data.OTCAUS).FirstOrDefault();
                _dataContext.SelectedCustomer = _dataContext.Customers?.Where(w => w.abecod == _dataContext.Data.OTCLIE).FirstOrDefault();

                if (_dataContext.SelectedCustomer != null)
                    _dataContext.Recipients = _dataContext.GetDESTINATARIs(_dataContext.Data.OTCLIE!.Value);
                if (_dataContext.SelectedCustomer != null && _dataContext.Recipients != null)
                    _dataContext.SelectedRecipient = _dataContext.Recipients.Where(w => w.cliecod == _dataContext.SelectedCustomer.abecod && w.codesti == _dataContext.Data.DESTIN).FirstOrDefault();

                _dataContext.SelectedPayment = _dataContext.Payments?.Where(w => w.pclcod == _dataContext.Data.OTPAGA).FirstOrDefault();

                _dataContext.Banks = _dataContext.GetABICABs(_dataContext.SelectedPayment?.Incasso?.icssup);
                _dataContext.SelectedCustomerBank = _dataContext.Banks?.Where(w => w.ABI == _dataContext.Data.abiabi && w.CAB == _dataContext.Data.abicab).FirstOrDefault();

                _dataContext.SelectedDelivery = _dataContext.Deliveries?.Where(w => w.concod == _dataContext.Data.OTCONS).FirstOrDefault();
                _dataContext.SelectedShipment = _dataContext.Shipments?.Where(w => w.specod == _dataContext.Data.OTSPED).FirstOrDefault();
                _dataContext.SelectedPacking = _dataContext.Packings?.Where(w => w.imbcod == _dataContext.Data.OTIMBA).FirstOrDefault();
                _dataContext.SelectedFirstCarrier = _dataContext.Carriers?.Where(w => w.vetcod == _dataContext.Data.OTCORR).FirstOrDefault();
                _dataContext.SelectedSecondCarrier = _dataContext.Carriers?.Where(w => w.vetcod == _dataContext.Data.OTCORR2).FirstOrDefault();
                _dataContext.SelectedLanguage = _dataContext.Languages?.Where(w => w.lincod == _dataContext.Data.otling).FirstOrDefault();
                _dataContext.SelectedArea = _dataContext.Areas?.Where(w => w.arecod == _dataContext.Data.OTAREA).FirstOrDefault();
                _dataContext.SelectedRegion = _dataContext.Regions?.Where(w => w.regcod == _dataContext.Data.OTREGI).FirstOrDefault();
                _dataContext.SelectedZone = _dataContext.Zones?.Where(w => w.zoncod == _dataContext.Data.OTZONA).FirstOrDefault();
                _dataContext.SelectedSector = _dataContext.Sectors?.Where(w => w.smecod == _dataContext.Data.OTSETM).FirstOrDefault();
                _dataContext.SelectedBranch = _dataContext.Branches?.Where(w => w.filcod == _dataContext.Data.OTFILI).FirstOrDefault();
                _dataContext.SelectedDealer = _dataContext.Dealers?.Where(w => w.rivcod == _dataContext.Data.OTRIVE).FirstOrDefault();

                spInsertInfo.Visibility = Visibility.Collapsed;
                spEditInfo.Visibility = Visibility.Visible;
            }
            else
            {
                spInsertInfo.Visibility = Visibility.Visible;
                spEditInfo.Visibility = Visibility.Collapsed;
            }

            _dataContext.HasMergedSigns = (UserContext.Instance!.ACCESS!.Roles?.canOrdersSignTech ?? false) && (UserContext.Instance!.ACCESS!.Roles?.canOrdersSignCommercial ?? false);

            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli generali ordine cliente {(_dataContext.IsInsert ? "nuovo" : _dataContext.Data.PrintFullID)}";
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
                if (!_dataContext.IsInsert)
                {
                    // check if already signed
                    bool proceed = true;
                    if ((_dataContext.Data.OTFICO.HasValue || _dataContext.Data.OTFITE.HasValue) && !_dataContext.HasMergedSigns)
                    {
                        if (ConfirmHandler.Confirm("Questo ordine ha già delle firme apposte, proseguendo verranno annullate e bisognerà richiederle nuovamente, procedere ?"))
                        {
                            // clear signs
                            _dataContext.Data.OTFITE = null;
                            _dataContext.Data.OTFITEUSR = null;
                            _dataContext.Data.OTFICO = null;
                            _dataContext.Data.OTFICOUSR = null;
                            _dataContext.Data.OTINFI = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                            _dataContext.Data.OTINFIUSR = _dataContext.UserID;
                        }
                        else
                        {
                            proceed = false;
                        }
                    }
                    if (proceed)
                    {

                        if (_dataContext.Update())
                        {
                            Mouse.OverrideCursor = null;
                            this.DialogResult = true;
                        }
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        this.DialogResult = false;
                    }
                }
                else
                {
                    if (_dataContext.Insert())
                    {
                        Mouse.OverrideCursor = null;
                        this.DialogResult = true;
                    }
                }
            }
            else
            {
                Mouse.OverrideCursor = null; ErrorHandler.Validation(validated);
            }
        }
        #endregion

        #region Autocompletes
        private void acCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCausal.cauacq))
            {
                _dataContext.Data.OTCAUS = _dataContext.SelectedCausal.cauacq;
            }
        }

        private void acCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCustomer != null && _dataContext.SelectedCustomer.abecod > 0)
            {
                if (_dataContext.Data.OTCLIE != _dataContext.SelectedCustomer.abecod)
                {
                    _dataContext.Data.OTCLIE = _dataContext.SelectedCustomer.abecod;
                    _dataContext.SelectedRecipient = null;
                    _dataContext.Recipients = _dataContext.GetDESTINATARIs(_dataContext.Data.OTCLIE!.Value);

                    // set default payment, bank
                    var customerData = _dataContext.GetCLIAMMI(_dataContext.SelectedCustomer.abecod);
                    _dataContext.SelectedCustomerContacts = _dataContext.GetCLIENTI(_dataContext.SelectedCustomer.abecod);
                    _dataContext.Data.OTDE25 = _dataContext.SelectedCustomerContacts?.CLREAC?.Trim();
                    _dataContext.Data.OTSCCL = customerData?.CLSCON;
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

                    _dataContext.SelectedShipment = _dataContext.Shipments?.Where(w => w.specod == customerData?.specod).FirstOrDefault();
                    _dataContext.SelectedDelivery = _dataContext.Deliveries?.Where(w => w.concod == customerData?.concod).FirstOrDefault();
                    // default agents from CLIAMMI to default on rows
                    _dataContext.Data.DefaultFirstAgent = new AGENTI() { agecod = customerData?.CLAGEN ?? string.Empty, agepvg = customerData?.CLAGENP, agepvgt = customerData?.CLAGENPT, agedes = string.Empty, ageflag = string.Empty, };
                    _dataContext.Data.DefaultSecondAgent = new AGENTI() { agecod = customerData?.CLAGEN2 ?? string.Empty, agepvg = customerData?.CLAGEN2P, agepvgt = customerData?.CLAGEN2PT, agedes = string.Empty, ageflag = string.Empty };
                    _dataContext.SelectedFirstCarrier = _dataContext.Carriers?.Where(w => w.vetcod == customerData?.vetcod).FirstOrDefault();
                    _dataContext.SelectedArea = _dataContext.Areas?.Where(w => w.arecod == customerData?.arecod).FirstOrDefault();
                    _dataContext.Data.OTCONZ = customerData?.CLCCON;
                    _dataContext.SelectedBranch = _dataContext.Branches?.Where(w => w.filcod == customerData?.filcod).FirstOrDefault();
                    _dataContext.SelectedDealer = _dataContext.Dealers?.Where(w => w.rivcod == customerData?.rivcod).FirstOrDefault();
                    _dataContext.SelectedRegion = _dataContext.Regions?.Where(w => w.regcod == customerData?.CLREGI).FirstOrDefault();
                    _dataContext.SelectedZone = _dataContext.Zones?.Where(w => w.zoncod == customerData?.CLZONE).FirstOrDefault();
                    _dataContext.SelectedSector = _dataContext.Sectors?.Where(w => w.smecod == customerData?.CLSETM).FirstOrDefault();
                    _dataContext.SelectedPacking = _dataContext.Packings?.Where(w => w.imbcod == customerData?.CLIMBA).FirstOrDefault();


                }
                _dataContext.Expired = _dataContext.GetExpired(_dataContext.Data.OTCLIE.Value) ?? 0;
                _dataContext.ExpiredVisibility = _dataContext.Expired > 0 ? true : false;
            }
            else
            {
                _dataContext.Data.OTCLIE = null;
                _dataContext.Recipients = null;
                _dataContext.SelectedCustomerContacts = null;
            }
        }

        private void acRecipient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedRecipient != null && _dataContext.SelectedRecipient.codesti > 0)
            {
                _dataContext.Data.DESTIN = _dataContext.SelectedRecipient.codesti;
            }
            else
            {
                _dataContext.Data.DESTIN = null;
            }
        }

        private void acPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedPayment != null &&
                !string.IsNullOrWhiteSpace(_dataContext.SelectedPayment.pclcod))
            {
                if (_dataContext.Data.OTPAGA != _dataContext.SelectedPayment.pclcod)
                {
                    _dataContext.Data.OTPAGA = _dataContext.SelectedPayment.pclcod;
                    // refresh bank
                    _dataContext.SelectedCustomerBank = null;
                    _dataContext.Banks = _dataContext.GetABICABs(_dataContext.SelectedPayment.Incasso?.icssup);
                }
            }
            else
            {
                _dataContext.Data.OTPAGA = null;
                _dataContext.Banks = null;
                _dataContext.SelectedCustomerBank = null;
            }
        }

        private void acCustomerBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCustomerBank != null && _dataContext.SelectedCustomerBank.ABI > 0)
            {
                _dataContext.Data.abiabi = _dataContext.SelectedCustomerBank.ABI;
                _dataContext.Data.abicab = _dataContext.SelectedCustomerBank.CAB;
                _dataContext.Data.OTBCON = _dataContext.SelectedCustomerBank.Account;
            }
        }

        private void acDelivery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedDelivery != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedDelivery.concod))
            {
                _dataContext.Data.OTCONS = _dataContext.SelectedDelivery.concod;
            }
        }

        private void acShipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedShipment != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedShipment.specod))
            {
                _dataContext.Data.OTSPED = _dataContext.SelectedShipment.specod;
            }
        }

        private void acPacking_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedPacking != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedPacking.imbcod))
            {
                _dataContext.Data.OTIMBA = _dataContext.SelectedPacking.imbcod;
            }
        }

        private void acFirstCarrier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedFirstCarrier != null && _dataContext.SelectedFirstCarrier.vetcod > 0)
            {
                _dataContext.Data.OTCORR = _dataContext.SelectedFirstCarrier.vetcod;
            }
            else
            {
                _dataContext.Data.OTCORR = null;
            }
        }

        private void acSecondCarrier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedSecondCarrier != null && _dataContext.SelectedSecondCarrier.vetcod > 0)
            {
                _dataContext.Data.OTCORR2 = _dataContext.SelectedSecondCarrier.vetcod;
            }
            else
            {
                _dataContext.Data.OTCORR2 = null;
            }
        }

        private void acLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedLanguage != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedLanguage.lincod))
            {
                _dataContext.Data.otling = _dataContext.SelectedLanguage.lincod;
            }
        }

        private void acArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedArea != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedArea.arecod))
            {
                _dataContext.Data.OTAREA = _dataContext.SelectedArea.arecod;
            }
            else
            {
                _dataContext.Data.OTAREA = null;
            }
        }

        private void acRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedRegion != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedRegion.regcod))
            {
                _dataContext.Data.OTREGI = _dataContext.SelectedRegion.regcod;
            }
            else
            {
                _dataContext.Data.OTREGI = null;
            }
        }

        private void acZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedZone != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedZone.zoncod))
            {
                _dataContext.Data.OTZONA = _dataContext.SelectedZone.zoncod;
            }
            else
            {
                _dataContext.Data.OTZONA = null;
            }
        }

        private void acSector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedSector != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSector.smecod))
            {
                _dataContext.Data.OTSETM = _dataContext.SelectedSector.smecod;
            }
            else
            {
                _dataContext.Data.OTSETM = null;
            }
        }

        private void acBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedBranch != null)
            {
                _dataContext.Data.OTFILI = _dataContext.SelectedBranch.filcod;
            }
            else
            {
                _dataContext.Data.OTFILI = null;
            }
        }

        private void acDealer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedDealer != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedDealer.rivcod))
            {
                _dataContext.Data.OTRIVE = _dataContext.SelectedDealer.rivcod;
            }
            else
            {
                _dataContext.Data.OTRIVE = null;
            }
        }

        private void acHeadText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedHeadText != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedHeadText.TTxcod))
            {
                if (ConfirmHandler.Confirm($"Importando il testo\n{_dataContext.SelectedHeadText.FullDescriptionSearchable}\nquello corrente verrà completamente sostituito, proseguire ?"))
                {
                    _dataContext.Data.OTNOTET = _dataContext.SelectedHeadText.TTXNote;
                }
                else
                {
                    _dataContext.SelectedHeadText = null;
                }
            }
        }

        private void acFootText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedFootText != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedFootText.TTxcod))
            {
                if (ConfirmHandler.Confirm($"Importando il testo\n{_dataContext.SelectedFootText.FullDescriptionSearchable}\nquello corrente verrà completamente sostituito, proseguire ?"))
                {
                    _dataContext.Data.OTNOTEP = _dataContext.SelectedFootText.TTXNote;
                }
                else
                {
                    _dataContext.SelectedFootText = null;
                }
            }
        }

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

        private void grdExpired_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_dataContext.SelectedCustomer != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<SelectECWindowViewModel>();
                windowViewModel.EntityType = "C";
                windowViewModel.EntityID = _dataContext.SelectedCustomer.abecod;

                var wSelect = new SelectECWindow(windowViewModel);
                wSelect.Owner = Window.GetWindow(this);
                wSelect.ShowDialog();
            }
        }
    }
}
