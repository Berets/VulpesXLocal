using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VulpesX.ViewModels.Modules.Default.CRM;
using VulpesX.ViewModels.Modules.Default.General;
using VulpesX.WindowsFactory.Default.General;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for OFFET00FWindow.xaml
    /// </summary>
    public partial class OFFET00FWindow : FluentDefaultWindow
    {
        private OFFET00FWindowViewModel _dataContext;
        public OFFET00FWindow(OFFET00FWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedCausal = _dataContext.Causals?.Where(w => w.offcod == _dataContext.Data.OFTCAUS).FirstOrDefault();
                _dataContext.SelectedCustomer = _dataContext.Customers?.Where(w => w.abecod == _dataContext.Data.OFTCOCL).FirstOrDefault();

                if (_dataContext.SelectedCustomer != null)
                    _dataContext.Recipients = _dataContext.GetDESTINATARIs(_dataContext.Data.OFTCOCL!.Value);
                if (_dataContext.SelectedCustomer != null && _dataContext.Recipients != null)
                    _dataContext.SelectedRecipient = _dataContext.Recipients.Where(w => w.cliecod == _dataContext.SelectedCustomer.abecod && w.codesti == _dataContext.Data.OFTDEST).FirstOrDefault();
                _dataContext.SelectedPayment = _dataContext.Payments?.Where(w => w.pclcod == _dataContext.Data.OFTPAGA).FirstOrDefault();

                if (_dataContext.SelectedPayment != null)
                    _dataContext.Banks = _dataContext.GetABICABs(_dataContext.SelectedPayment.Incasso?.icssup);
                if (_dataContext.Data.abiabi.HasValue)
                {
                    if (!string.IsNullOrWhiteSpace(_dataContext.Data.OFTBCON))
                        _dataContext.SelectedCustomerBank = _dataContext.Banks?.Where(w => w.ABI == _dataContext.Data.abiabi && w.CAB == _dataContext.Data.abicab && w.Account == _dataContext.Data.OFTBCON).FirstOrDefault();
                    else
                        _dataContext.SelectedCustomerBank = _dataContext.Banks?.Where(w => w.ABI == _dataContext.Data.abiabi && w.CAB == _dataContext.Data.abicab).FirstOrDefault();
                }

                _dataContext.SelectedDelivery = _dataContext.Deliveries?.Where(w => w.concod == _dataContext.Data.OFTCONS).FirstOrDefault();
                _dataContext.SelectedShipment = _dataContext.Shipments?.Where(w => w.specod == _dataContext.Data.OFTSPED).FirstOrDefault();
                _dataContext.SelectedPacking = _dataContext.Packings?.Where(w => w.imbcod == _dataContext.Data.OFTIMBA).FirstOrDefault();
                _dataContext.SelectedFirstCarrier = _dataContext.Carriers?.Where(w => w.vetcod == _dataContext.Data.OFTCORR).FirstOrDefault();
                _dataContext.SelectedSecondCarrier = _dataContext.Carriers?.Where(w => w.vetcod == _dataContext.Data.OFTCORR2).FirstOrDefault();
                _dataContext.SelectedArea = _dataContext.Areas?.Where(w => w.arecod == _dataContext.Data.OFTAREA).FirstOrDefault();
                _dataContext.SelectedRegion = _dataContext.Regions?.Where(w => w.regcod == _dataContext.Data.OFTREGI).FirstOrDefault();
                _dataContext.SelectedZone = _dataContext.Zones?.Where(w => w.zoncod == _dataContext.Data.OFTZONE).FirstOrDefault();
                _dataContext.SelectedSector = _dataContext.Sectors?.Where(w => w.smecod == _dataContext.Data.OFTSETM).FirstOrDefault();
                _dataContext.SelectedBranch = _dataContext.Branches?.Where(w => w.filcod == _dataContext.Data.OFTFILI).FirstOrDefault();
                _dataContext.SelectedDealer = _dataContext.Dealers?.Where(w => w.rivcod == _dataContext.Data.OFTRIVE).FirstOrDefault();
                _dataContext.SelectedLanguage = _dataContext.Languages?.Where(w => w.lincod == _dataContext.Data.OFTLING).FirstOrDefault();

                spInsertInfo.Visibility = Visibility.Collapsed;
                spEditInfo.Visibility = Visibility.Visible;
            }
            else
            {
                spInsertInfo.Visibility = Visibility.Visible;
                spEditInfo.Visibility = Visibility.Collapsed;
            }

            _dataContext.HasMergedSigns = (UserContext.Instance!.ACCESS!.Roles?.canOffersSignTech ?? false) && (UserContext.Instance!.ACCESS!.Roles?.canOffersSignCommercial ?? false);

            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli generali offerta {(_dataContext.IsInsert ? "nuova" : _dataContext.Data.PrintFullID)}";
                if (_dataContext.IsReadonly)
                    this.Title += " - [sola lettura]";
            };
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Data.OFTCONZ.HasValue && _dataContext.Data.OFTCONZ.Value <= 0)
                    _dataContext.Data.OFTCONZ = null;
                if (string.IsNullOrWhiteSpace(_dataContext.Data.OFTDE25))
                    _dataContext.Data.OFTDE25 = null;

                if (!_dataContext.IsInsert)
                {
                    // check if already signed
                    bool proceed = true;
                    if ((_dataContext.Data.OFTFICO.HasValue || _dataContext.Data.OFTFITE.HasValue) && !_dataContext.HasMergedSigns)
                    {
                        if (ConfirmHandler.Confirm("Questa offerta ha già delle firme apposte, proseguendo verranno annullate e bisognerà richiederle nuovamente, procedere ?"))
                        {
                            // clear signs
                            _dataContext.Data.OFTFITE = null;
                            _dataContext.Data.OFTFITEUSR = null;
                            _dataContext.Data.OFTFICO = null;
                            _dataContext.Data.OFTFICOUSR = null;
                            _dataContext.Data.OFTINFI = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                            _dataContext.Data.OFTINFIUSR = _dataContext.UserID;
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
            { Mouse.OverrideCursor = null; ErrorHandler.Validation(validated); }
        }
        #endregion

        #region Autocompletes
        private void acCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCausal.offcod))
            {
                _dataContext.Data.OFTCAUS = _dataContext.SelectedCausal.offcod;
                if (!_dataContext.Data.oftscad.HasValue && _dataContext.SelectedCausal.offsca.HasValue)
                    _dataContext.Data.oftscad = _dataContext.Data.OFTDAOR.HasValue ? _dataContext.Data.OFTDAOR.Value.AddDays(_dataContext.SelectedCausal.offsca.Value) : null;
            }
        }
        private void acCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCustomer != null && _dataContext.SelectedCustomer.abecod > 0)
            {
                if (_dataContext.Data.OFTCOCL != _dataContext.SelectedCustomer.abecod)
                {
                    _dataContext.Data.OFTCOCL = _dataContext.SelectedCustomer.abecod;
                    _dataContext.SelectedRecipient = null;
                    _dataContext.Recipients = _dataContext.GetDESTINATARIs(_dataContext.Data.OFTCOCL.Value);
                    // set default payment, bank
                    var customerData = _dataContext.GetCLIAMMI(_dataContext.SelectedCustomer?.abecod ?? 0);
                    _dataContext.SelectedCustomerContacts = _dataContext.GetCLIENTI(_dataContext.SelectedCustomer?.abecod ?? 0);
                    _dataContext.Data.OFTDE25 = _dataContext.SelectedCustomerContacts?.CLREAC?.Trim();
                    _dataContext.Data.OFTSCCL = customerData?.CLSCON;
                    _dataContext.SelectedPayment = _dataContext.Payments?.Where(w => w.pclcod == customerData?.pclcod).FirstOrDefault();

                    if (_dataContext.SelectedPayment != null)
                    {
                        if (_dataContext.SelectedPayment.Incasso?.icssup == "R")
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
                    // agents from CLIAMMI to default on rows
                    _dataContext.Data.DefaultFirstAgent = new AGENTI() { agecod = customerData?.CLAGEN ?? string.Empty, agepvg = customerData?.CLAGENP, agepvgt = customerData?.CLAGENPT, agedes = string.Empty, ageflag = string.Empty };
                    _dataContext.Data.DefaultSecondAgent = new AGENTI() { agecod = customerData?.CLAGEN2 ?? string.Empty, agepvg = customerData?.CLAGEN2P, agepvgt = customerData?.CLAGEN2PT, agedes = string.Empty, ageflag = string.Empty };
                    _dataContext.SelectedFirstCarrier = _dataContext.Carriers?.Where(w => w.vetcod == customerData?.vetcod).FirstOrDefault();
                    _dataContext.SelectedArea = _dataContext.Areas?.Where(w => w.arecod == customerData?.arecod).FirstOrDefault();
                    _dataContext.Data.OFTCONZ = customerData?.CLCCON;
                    _dataContext.SelectedBranch = _dataContext.Branches?.Where(w => w.filcod == customerData?.filcod).FirstOrDefault();
                    _dataContext.SelectedDealer = _dataContext.Dealers?.Where(w => w.rivcod == customerData?.rivcod).FirstOrDefault();
                    _dataContext.SelectedRegion = _dataContext.Regions?.Where(w => w.regcod == customerData?.CLREGI).FirstOrDefault();
                    _dataContext.SelectedZone = _dataContext.Zones?.Where(w => w.zoncod == customerData?.CLZONE).FirstOrDefault();
                    _dataContext.SelectedSector = _dataContext.Sectors?.Where(w => w.smecod == customerData?.CLSETM).FirstOrDefault();
                    _dataContext.SelectedPacking = _dataContext.Packings?.Where(w => w.imbcod == customerData?.CLIMBA).FirstOrDefault();
                }
                if (_dataContext.SelectedCustomer?.abecfe == "P")
                {
                    tbEntityType.Text = "PROSPECT";
                    tbEntityType.Foreground = new SolidColorBrush(Colors.RoyalBlue);
                    borEntity.BorderBrush = this.FindResource("VulpesXBlueBrush") as SolidColorBrush;
                }
                else
                {
                    tbEntityType.Text = "CLIENTE";
                    tbEntityType.Foreground = new SolidColorBrush(Colors.MediumVioletRed);
                    borEntity.BorderBrush = this.FindResource("VulpesXVioletBrush") as SolidColorBrush;
                }
            }
            else
            {
                tbEntityType.Text = null;
                tbEntityType.Foreground = new SolidColorBrush(Colors.Transparent);
                borEntity.BorderBrush = new SolidColorBrush(Colors.Transparent);
                _dataContext.Data.OFTCOCL = null;
                _dataContext.Recipients = null;
                _dataContext.SelectedCustomerContacts = null;
            }
        }
        private void acRecipient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedRecipient != null && _dataContext.SelectedRecipient.codesti > 0)
            {
                _dataContext.Data.OFTDEST = _dataContext.SelectedRecipient.codesti;
            }
            else
            {
                _dataContext.Data.OFTDEST = null;
            }
        }

        private void acPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedPayment != null &&
                !string.IsNullOrWhiteSpace(_dataContext.SelectedPayment.pclcod))
            {
                if (_dataContext.Data.OFTPAGA != _dataContext.SelectedPayment.pclcod)
                {
                    _dataContext.Data.OFTPAGA = _dataContext.SelectedPayment.pclcod;
                    // refresh bank
                    _dataContext.SelectedCustomerBank = null;
                    _dataContext.Banks = _dataContext.GetABICABs(_dataContext.SelectedPayment?.Incasso?.icssup);
                }
            }
            else
            {
                _dataContext.Data.OFTPAGA = null;
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
                _dataContext.Data.OFTBCON = _dataContext.SelectedCustomerBank.Account;
            }
        }
        private void acDelivery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedDelivery != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedDelivery.concod))
            {
                _dataContext.Data.OFTCONS = _dataContext.SelectedDelivery.concod;
            }
        }

        private void acShipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedShipment != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedShipment.specod))
            {
                _dataContext.Data.OFTSPED = _dataContext.SelectedShipment.specod;
            }
        }

        private void acPacking_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedPacking != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedPacking.imbcod))
            {
                _dataContext.Data.OFTIMBA = _dataContext.SelectedPacking.imbcod;
            }
        }
        private void acFirstCarrier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SelectedFirstCarrier != null && _dataContext.SelectedFirstCarrier.vetcod > 0)
            {
                _dataContext.Data.OFTCORR = _dataContext.SelectedFirstCarrier.vetcod;
            }
        }
        private void acSecondCarrier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext != null && _dataContext.SelectedSecondCarrier != null && _dataContext.SelectedSecondCarrier.vetcod > 0)
            {
                _dataContext.Data.OFTCORR2 = _dataContext.SelectedSecondCarrier.vetcod;
            }
        }
        private void acArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedArea != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedArea.arecod))
            {
                _dataContext.Data.OFTAREA = _dataContext.SelectedArea.arecod;
            }
            else
            {
                _dataContext.Data.OFTAREA = null;
            }
        }
        private void acRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedRegion != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedRegion.regcod))
            {
                _dataContext.Data.OFTREGI = _dataContext.SelectedRegion.regcod;
            }
            else
            {
                _dataContext.Data.OFTREGI = null;
            }
        }
        private void acZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedZone != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedZone.zoncod))
            {
                _dataContext.Data.OFTZONE = _dataContext.SelectedZone.zoncod;
            }
            else
            {
                _dataContext.Data.OFTZONE = null;
            }
        }
        private void acSector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedSector != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSector.smecod))
            {
                _dataContext.Data.OFTSETM = _dataContext.SelectedSector.smecod;
            }
            else
            {
                _dataContext.Data.OFTSETM = null;
            }
        }
        private void acBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedBranch != null)
            {
                _dataContext.Data.OFTFILI = _dataContext.SelectedBranch.filcod;
            }
            else
            {
                _dataContext.Data.OFTFILI = null;
            }
        }
        private void acDealer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedDealer != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedDealer.rivcod))
            {
                _dataContext.Data.OFTRIVE = _dataContext.SelectedDealer.rivcod;
            }
            else
            {
                _dataContext.Data.OFTRIVE = null;
            }
        }
        private void acHeadText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedHeadText != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedHeadText.TTxcod))
            {
                if (ConfirmHandler.Confirm($"Importando il testo\n{_dataContext.SelectedHeadText.FullDescriptionSearchable}\nquello corrente verrà completamente sostituito, proseguire ?"))
                {
                    _dataContext.Data.OFTNOTET = _dataContext.SelectedHeadText.TTXNote;
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
                    _dataContext.Data.OFTNOTEP = _dataContext.SelectedFootText.TTXNote;
                }
                else
                {
                    _dataContext.SelectedFootText = null;
                }
            }
        }

        private void acLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedLanguage != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedLanguage.lincod))
            {
                _dataContext.Data.OFTLING = _dataContext.SelectedLanguage.lincod;
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

        private void cmdCreateRegistry_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var groupsList = _dataContext.GetGruppi();
            var accountCache = _dataContext.GetConti();
            var subaccountCache = _dataContext.GetSotto();

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ABEWindowViewModel>();
            windowViewModel.Data = new ABE()
            {
                isocod = "IT",
                abeind = string.Empty,
            };
            windowViewModel.Recipients = new ObservableCollection<DESTINATARI>();
            windowViewModel.SupplierRecipients = new ObservableCollection<DESFOR>();
            windowViewModel.CustomerNotes = new ObservableCollection<NOTECLI1>();
            windowViewModel.SupplierNotes = new ObservableCollection<NOTEFOR>();
            windowViewModel.SupplierData = new FORNAMMI()
            {
                AccountCache = accountCache,
                SubaccountCache = subaccountCache,
                GroupsList = groupsList,
                Foraso = _dataContext.CompanyID
            };
            windowViewModel.CustomerData = new CLIAMMI()
            {
                AccountCache = accountCache,
                SubaccountCache = subaccountCache,
                GroupsList = groupsList,
                Cliasoc = _dataContext.CompanyID
            };
            windowViewModel.SupplierCommercialData = new FORNITORI()
            { };
            windowViewModel.CustomerCommercialData = new CLIENTI()
            {
            };
            windowViewModel.SupplierReferences = new ObservableCollection<RFFTB00F>();
            windowViewModel.CustomerReferences = new ObservableCollection<ANDEFRES>();
            windowViewModel.CounterpartsRows = new ObservableCollection<SUPPLIER_GROUPS>();
            windowViewModel.CounterpartsRowsCustomer = new ObservableCollection<CUSTOMER_GROUPS>();
            windowViewModel.IsInsert = true;

            var wABE = VulpesServiceProvider.Provider.GetRequiredService<IABEWindowFactory>().Create(windowViewModel);
            Mouse.OverrideCursor = null;
            wABE.Owner = Window.GetWindow(this);
            if (wABE.ShowDialog() == true)
            {
                _dataContext.Customers = _dataContext.GetABEs();
                _dataContext.SelectedCustomer = _dataContext.Customers?.Where(w => w.abecod == windowViewModel.AssignedCustomerID).First();
            }
        }
    }
}
