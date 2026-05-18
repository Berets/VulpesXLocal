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
    /// Interaction logic for BOLLT00FWindow.xaml
    /// </summary>
    public partial class BOLLT00FWindow : FluentDefaultWindow
    {
        private BOLLT00FWindowViewModel _dataContext;
        private BOLLT00F_history? currentVersion;

        public BOLLT00FWindow(BOLLT00FWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedCustomer = _dataContext.Customers?.Where(w => w.abecod == _dataContext.Data.BTCODC).FirstOrDefault();

                if (_dataContext.SelectedCustomer != null)
                {
                    if (_dataContext.Data.BTFLCF == "C")
                        _dataContext.References = _dataContext.GetDESTINATARIs(_dataContext.Data.BTCODC ?? 0);
                    else
                        _dataContext.References = _dataContext.GetDESFORs(_dataContext.Data.BTCODC ?? 0);
                }

                if (_dataContext.SelectedCustomer != null && _dataContext.References != null)
                    _dataContext.SelectedReference = _dataContext.References?.Where(w => w.ID == _dataContext.Data.BTCODD).FirstOrDefault();

                _dataContext.SelectedCausal = _dataContext.Causals?.Where(w => w.bolcod == _dataContext.Data.BTCAUS).FirstOrDefault();
                _dataContext.SelectedPayment = _dataContext.Payments?.Where(w => w.pclcod == _dataContext.Data.BTPAGA).FirstOrDefault();

                if (_dataContext.SelectedPayment != null)
                    _dataContext.Banks = _dataContext.GetABICABs(_dataContext.SelectedPayment?.Incasso?.icssup);

                if (_dataContext.Data.abiabi.HasValue)
                {
                    if (!string.IsNullOrWhiteSpace(_dataContext.Data.BTBCON))
                        _dataContext.SelectedCustomerBank = _dataContext.Banks?.Where(w => w.ABI == _dataContext.Data.abiabi && w.CAB == _dataContext.Data.abicab && w.Account == _dataContext.Data.BTBCON).FirstOrDefault();
                    else
                        _dataContext.SelectedCustomerBank = _dataContext.Banks?.Where(w => w.ABI == _dataContext.Data.abiabi && w.CAB == _dataContext.Data.abicab).FirstOrDefault();
                }

                _dataContext.SelectedDelivery = _dataContext.Deliveries?.Where(w => w.concod == _dataContext.Data.BTCONS).FirstOrDefault();
                _dataContext.SelectedShipment = _dataContext.Shipments?.Where(w => w.specod == _dataContext.Data.BTSPED).FirstOrDefault();
                _dataContext.SelectedPacking = _dataContext.Packings?.Where(w => w.imbcod == _dataContext.Data.BTIMBA).FirstOrDefault();
                _dataContext.SelectedFirstCarrier = _dataContext.Carriers?.Where(w => w.vetcod == _dataContext.Data.BTCORR).FirstOrDefault();
                _dataContext.SelectedSecondCarrier = _dataContext.Carriers?.Where(w => w.vetcod == _dataContext.Data.BTCORR2).FirstOrDefault();
                _dataContext.SelectedLanguage = _dataContext.Languages?.Where(w => w.lincod == _dataContext.Data.BTLING).FirstOrDefault();
                _dataContext.SelectedArea = _dataContext.Areas?.Where(w => w.arecod == _dataContext.Data.BTAREA).FirstOrDefault();
                _dataContext.SelectedRegion = _dataContext.Regions?.Where(w => w.regcod == _dataContext.Data.BTREGI).FirstOrDefault();
                _dataContext.SelectedZone = _dataContext.Zones?.Where(w => w.zoncod == _dataContext.Data.BTZONA).FirstOrDefault();
                _dataContext.SelectedSector = _dataContext.Sectors?.Where(w => w.smecod == _dataContext.Data.BTSETM).FirstOrDefault();
                _dataContext.SelectedBranch = _dataContext.Branches?.Where(w => w.filcod == _dataContext.Data.BTFILI).FirstOrDefault();
                _dataContext.SelectedDealer = _dataContext.Dealers?.Where(w => w.rivcod == _dataContext.Data.BTRIVE).FirstOrDefault();
            }

            var customization = _dataContext.GetAZIENDA();
            _dataContext.IsReadonly = _dataContext.Data.BTSTATO == "F" || _dataContext.Data.canceled.HasValue || (_dataContext.Data.BTNUBD > 0 && !(customization?.azddtdefic ?? true));

            // set info to display
            if (_dataContext.IsInsert)
            {
                spInsertInfo.Visibility = Visibility.Visible;
                spEditInfo.Visibility = Visibility.Collapsed;
                spEditDefinitiveInfo.Visibility = Visibility.Collapsed;
            }
            else
            {
                spInsertInfo.Visibility = Visibility.Collapsed;
                spEditInfo.Visibility = Visibility.Visible;
                spEditDefinitiveInfo.Visibility = Visibility.Visible;
            }
            rtiPayment.Visibility = _dataContext.Data.BTFLCF == "C" ? Visibility.Visible : Visibility.Collapsed;
            rtiCommercial.Visibility = _dataContext.Data.BTFLCF == "C" ? Visibility.Visible : Visibility.Collapsed;

            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli generali DDT {(_dataContext.IsInsert ? "nuovo" : _dataContext.Data.PrintFullID)}";
                if (_dataContext.IsReadonly)
                    Title += " - [sola lettura]";
                if (!_dataContext.IsInsert)
                {
                    #region Save current version
                    currentVersion = new BOLLT00F_history()
                    {
                        bolsoc = _dataContext.Data.bolsoc,
                        BTANNO = _dataContext.Data.BTANNO,
                        BTBOLL = _dataContext.Data.BTBOLL,
                        BTNUBD = _dataContext.Data.BTNUBD,
                        BTDATP = _dataContext.Data.BTDATP,
                        BTDATA = _dataContext.Data.BTDATA,
                        BTCAUS = _dataContext.Data.BTCAUS,
                        BTCODC = _dataContext.Data.BTCODC,
                        BTCODD = _dataContext.Data.BTCODD,
                        BTPAGA = _dataContext.Data.BTPAGA,
                        BTCONS = _dataContext.Data.BTCONS,
                        BTSPED = _dataContext.Data.BTSPED,
                        BTCORR = _dataContext.Data.BTCORR,
                        BTCORR2 = _dataContext.Data.BTCORR2,
                        BTDE25 = _dataContext.Data.BTDE25,
                        BTPESO = _dataContext.Data.BTPESO,
                        BTPES2 = _dataContext.Data.BTPES2,
                        BTDASP = _dataContext.Data.BTDASP,
                        BTCOLL = _dataContext.Data.BTCOLL,
                        BTAREA = _dataContext.Data.BTAREA,
                        BTDEBE = _dataContext.Data.BTDEBE,
                        BTIMBA = _dataContext.Data.BTIMBA,
                        abiabi = _dataContext.Data.abiabi,
                        abicab = _dataContext.Data.abicab,
                        BTLING = _dataContext.Data.BTLING,
                        BTNOTET = _dataContext.Data.BTNOTET,
                        BTNOTEP = _dataContext.Data.BTNOTEP,
                        BTSHOWT = _dataContext.Data.BTSHOWT,
                        BTSHOWP = _dataContext.Data.BTSHOWP,
                        added = _dataContext.Data.added,
                        addedUserID = _dataContext.Data.addedUserID,
                        updated = _dataContext.Data.updated,
                        updatedUserID = _dataContext.Data.updatedUserID,
                        BTBCON = _dataContext.Data.BTBCON,
                        BTCONZ = _dataContext.Data.BTCONZ,
                        BTSTATO = _dataContext.Data.BTSTATO,
                        BTDAFA = _dataContext.Data.BTDAFA,
                        BTFILI = _dataContext.Data.BTFILI,
                        BTSCCL = _dataContext.Data.BTSCCL,
                        BTZONA = _dataContext.Data.BTZONA,
                        BTSETM = _dataContext.Data.BTSETM,
                        BTREGI = _dataContext.Data.BTREGI,
                        BTRIVE = _dataContext.Data.BTRIVE
                    };
                    currentVersion.Rows = new List<BOLLD00F_history>();

                    if (_dataContext.Data.Rows != null)
                    {
                        foreach (var row in _dataContext.Data.Rows ?? new())
                        {
                            var newRow = new BOLLD00F_history()
                            {
                                bolsoc = row.bolsoc,
                                BTANNO = row.BTANNO,
                                BTBOLL = row.BTBOLL,
                                BORIGB = row.BORIGB,
                                BOANNO = row.BOANNO,
                                BONUOR = row.BONUOR,
                                BORIGA = row.BORIGA,
                                BOMAGA = row.BOMAGA,
                                BOCODA = row.BOCODA,
                                BOQTAV = row.BOQTAV,
                                BOTQTA = row.BOTQTA,
                                BOUNIM = row.BOUNIM,
                                BODACO = row.BODACO,
                                BOSERI = row.BOSERI,
                                BORIFC = row.BORIFC,
                                boprez = row.boprez,
                                botpre = row.botpre,
                                boaliq = row.boaliq,
                                boasso = row.boasso,
                                bogrup = row.bogrup,
                                bocont = row.bocont,
                                bosotc = row.bosotc,
                                bosco1 = row.bosco1,
                                bosco2 = row.bosco2,
                                bosco3 = row.bosco3,
                                bomagg = row.bomagg,
                                botsc1 = row.botsc1,
                                botsc2 = row.botsc2,
                                botsc3 = row.botsc3,
                                botmag = row.botmag,
                                BONOTE = row.BONOTE,
                                BOSHOW = row.BOSHOW,
                                BOCOA1 = row.BOCOA1,
                                BOCOA2 = row.BOCOA2,
                                BOCOA1P = row.BOCOA1P,
                                BOCOA2P = row.BOCOA2P,
                                BOCOA1PT = row.BOCOA1PT,
                                BOCOA2PT = row.BOCOA2PT
                            };
                            newRow.Engages = new List<BOLLD00F1_history>();
                            foreach (var eng in row.EngagesRows ?? new System.Collections.ObjectModel.ObservableCollection<BOLLD00F1>())
                            {
                                newRow.Engages.Add(new BOLLD00F1_history()
                                {
                                    bolsoc = eng.bolsoc,
                                    BTANNO = eng.BTANNO,
                                    BTBOLL = eng.BTBOLL,
                                    BORIGB = eng.BORIGB,
                                    boposc = eng.boposc,
                                    bolott = eng.bolott,
                                    boqtlo = eng.boqtlo,
                                    store_id = eng.store_id,
                                    product_id = eng.product_id
                                });
                            }
                            currentVersion.Rows.Add(newRow);
                        }
                    }
                    #endregion
                }
            };
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save(currentVersion))
                {
                    Mouse.OverrideCursor = null;
                    this.DialogResult = true;
                }
            }
            else
            {
                Mouse.OverrideCursor = null; ErrorHandler.Validation(validated);
            }
        }
        #endregion

        #region Autocompletes
        private void acCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCustomer != null && _dataContext.SelectedCustomer.abecod > 0)
            {
                if (_dataContext.Data.BTCODC != _dataContext.SelectedCustomer.abecod)
                {
                    _dataContext.Data.BTCODC = _dataContext.SelectedCustomer.abecod;
                    _dataContext.SelectedReference = null;
                    if (_dataContext.Data.BTFLCF == "C")
                        _dataContext.References = _dataContext.GetDESTINATARIs(_dataContext.Data.BTCODC.Value);
                    else
                        _dataContext.References = _dataContext.GetDESFORs(_dataContext.Data.BTCODC.Value);

                    // if customer set default payment, bank
                    if (_dataContext.Data.BTFLCF == "C")
                    {
                        var customerData = _dataContext.GetCLIAMMI(_dataContext.SelectedCustomer.abecod);
                        _dataContext.SelectedCustomerContacts = _dataContext.GetCLIENTI(_dataContext.SelectedCustomer.abecod);
                        _dataContext.Data.BTDE25 = _dataContext.SelectedCustomerContacts?.CLREAC?.Trim();
                        _dataContext.Data.BTSCCL = customerData?.CLSCON;
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
                        _dataContext.Data.DefaultFirstAgent = (string.IsNullOrWhiteSpace(customerData?.CLAGEN) ? null : new AGENTI() { agecod = customerData?.CLAGEN ?? string.Empty, agepvg = customerData?.CLAGENP, agepvgt = customerData?.CLAGENPT, agedes = string.Empty, ageflag = string.Empty });
                        _dataContext.Data.DefaultSecondAgent = (string.IsNullOrWhiteSpace(customerData?.CLAGEN2) ? null : new AGENTI() { agecod = customerData?.CLAGEN2 ?? string.Empty, agepvg = customerData?.CLAGEN2P, agepvgt = customerData?.CLAGEN2PT, agedes = string.Empty, ageflag = string.Empty });
                        _dataContext.SelectedFirstCarrier = _dataContext.Carriers?.Where(w => w.vetcod == customerData?.vetcod).FirstOrDefault();
                        _dataContext.SelectedPacking = _dataContext.Packings?.Where(w => w.imbcod == customerData?.CLIMBA).FirstOrDefault();
                        _dataContext.SelectedArea = _dataContext.Areas?.Where(w => w.arecod == customerData?.arecod).FirstOrDefault();
                        _dataContext.Data.BTCONZ = customerData?.CLCCON;
                        _dataContext.SelectedBranch = _dataContext.Branches?.Where(w => w.filcod == customerData?.filcod).FirstOrDefault();
                        _dataContext.SelectedDealer = _dataContext.Dealers?.Where(w => w.rivcod == customerData?.rivcod).FirstOrDefault();
                        _dataContext.SelectedRegion = _dataContext.Regions?.Where(w => w.regcod == customerData?.CLREGI).FirstOrDefault();
                        _dataContext.SelectedZone = _dataContext.Zones?.Where(w => w.zoncod == customerData?.CLZONE).FirstOrDefault();
                        _dataContext.SelectedSector = _dataContext.Sectors?.Where(w => w.smecod == customerData?.CLSETM).FirstOrDefault();
                        _dataContext.SelectedPacking = _dataContext.Packings?.Where(w => w.imbcod == customerData?.CLIMBA).FirstOrDefault();
                    }
                }
            }
            else
            {
                _dataContext.Data.BTCODC = null;
                _dataContext.References = null;
                _dataContext.SelectedCustomerContacts = null;
            }
        }

        private void acReference_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedReference != null && _dataContext.SelectedReference.ID > 0)
            {
                _dataContext.Data.BTCODD = _dataContext.SelectedReference.ID;
                if (_dataContext.Data.BTFLCF == "C")
                {
                    var customerData = _dataContext.GetCLIAMMI(_dataContext.SelectedCustomer?.abecod ?? 0);

                    var recipient = _dataContext.GetDESTINATARI(_dataContext.Data.BTCODC ?? 0, _dataContext.Data.BTCODD.Value);

                    _dataContext.Data.DefaultFirstAgent = !string.IsNullOrWhiteSpace(recipient?.decoag1) ?
                        new AGENTI() { agecod = recipient?.decoag1 ?? string.Empty, agepvg = recipient?.deage1p, agepvgt = recipient?.deage1pt, agedes = string.Empty, ageflag = string.Empty } :
                    (string.IsNullOrWhiteSpace(customerData?.CLAGEN) ? null : new AGENTI() { agecod = customerData?.CLAGEN ?? string.Empty, agepvg = customerData?.CLAGENP, agepvgt = customerData?.CLAGENPT, agedes = string.Empty, ageflag = string.Empty });
                    _dataContext.Data.DefaultSecondAgent = !string.IsNullOrWhiteSpace(recipient?.decoag2) ?
                        new AGENTI() { agecod = recipient?.decoag2 ?? string.Empty, agepvg = recipient?.deage2p, agepvgt = recipient?.deage2pt, agedes = string.Empty, ageflag = string.Empty } :
                    (string.IsNullOrWhiteSpace(customerData?.CLAGEN2) ? null : new AGENTI() { agecod = customerData?.CLAGEN2 ?? string.Empty, agepvg = customerData?.CLAGEN2P, agepvgt = customerData?.CLAGEN2PT, agedes = string.Empty, ageflag = string.Empty });
                }
            }
            else
            {
                _dataContext.Data.BTCODD = null;
                if (_dataContext.Data.BTFLCF == "C")
                {
                    // default agents from CLIAMMI to default on rows
                    var customerData = _dataContext.GetCLIAMMI(_dataContext.SelectedCustomer?.abecod ?? 0);
                    _dataContext.Data.DefaultFirstAgent = (string.IsNullOrWhiteSpace(customerData?.CLAGEN) ? null : new AGENTI() { agecod = customerData?.CLAGEN ?? string.Empty, agepvg = customerData?.CLAGENP, agepvgt = customerData?.CLAGENPT, agedes = string.Empty, ageflag = string.Empty });
                    _dataContext.Data.DefaultSecondAgent = (string.IsNullOrWhiteSpace(customerData?.CLAGEN2) ? null : new AGENTI() { agecod = customerData?.CLAGEN2 ?? string.Empty, agepvg = customerData?.CLAGEN2P, agepvgt = customerData?.CLAGEN2PT, agedes = string.Empty, ageflag = string.Empty });
                }
            }
        }
        private void acCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCausal.bolcod))
            {
                _dataContext.Data.BTCAUS = _dataContext.SelectedCausal.bolcod;
            }
            else
            { _dataContext.Data.BTCAUS = null; }
        }
        private void acPayment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedPayment != null &&
                !string.IsNullOrWhiteSpace(_dataContext.SelectedPayment.pclcod))
            {
                if (_dataContext.Data.BTPAGA != _dataContext.SelectedPayment.pclcod)
                {
                    _dataContext.Data.BTPAGA = _dataContext.SelectedPayment.pclcod;
                    // refresh bank
                    _dataContext.SelectedCustomerBank = null;
                    _dataContext.Banks = _dataContext.GetABICABs(_dataContext.SelectedPayment?.Incasso?.icssup);
                }
            }
            else
            {
                _dataContext.Data.BTPAGA = null;
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
                _dataContext.Data.BTBCON = _dataContext.SelectedCustomerBank.Account;
            }
            else
            {
                _dataContext.Data.abiabi = null;
                _dataContext.Data.abicab = null;
                _dataContext.Data.BTBCON = null;
            }
        }
        private void acDelivery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedDelivery != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedDelivery.concod))
            {
                _dataContext.Data.BTCONS = _dataContext.SelectedDelivery.concod;
            }
            else
            { _dataContext.Data.BTCONS = null; }
        }
        private void acShipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedShipment != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedShipment.specod))
            {
                _dataContext.Data.BTSPED = _dataContext.SelectedShipment.specod;
            }
            else
            { _dataContext.Data.BTSPED = null; }
        }
        private void acPacking_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedPacking != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedPacking.imbcod))
            {
                _dataContext.Data.BTIMBA = _dataContext.SelectedPacking.imbcod;
                if (string.IsNullOrWhiteSpace(_dataContext.Data.BTDEBE))
                    _dataContext.Data.BTDEBE = _dataContext.SelectedPacking.imbdes.Trim();
            }
            else
            { _dataContext.Data.BTIMBA = null; }
        }
        private void acFirstCarrier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedFirstCarrier != null && _dataContext.SelectedFirstCarrier.vetcod > 0)
            {
                _dataContext.Data.BTCORR = _dataContext.SelectedFirstCarrier.vetcod;
            }
            else
            { _dataContext.Data.BTCORR = null; }
        }
        private void acSecondCarrier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedSecondCarrier != null && _dataContext.SelectedSecondCarrier.vetcod > 0)
            {
                _dataContext.Data.BTCORR2 = _dataContext.SelectedSecondCarrier.vetcod;
            }
            else
            { _dataContext.Data.BTCORR2 = null; }
        }
        private void acArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedArea != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedArea.arecod))
            {
                _dataContext.Data.BTAREA = _dataContext.SelectedArea.arecod;
            }
            else
            {
                _dataContext.Data.BTAREA = null;
            }
        }
        private void acRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedRegion != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedRegion.regcod))
            {
                _dataContext.Data.BTREGI = _dataContext.SelectedRegion.regcod;
            }
            else
            {
                _dataContext.Data.BTREGI = null;
            }
        }
        private void acZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedZone != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedZone.zoncod))
            {
                _dataContext.Data.BTZONA = _dataContext.SelectedZone.zoncod;
            }
            else
            {
                _dataContext.Data.BTZONA = null;
            }
        }
        private void acSector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedSector != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSector.smecod))
            {
                _dataContext.Data.BTSETM = _dataContext.SelectedSector.smecod;
            }
            else
            {
                _dataContext.Data.BTSETM = null;
            }
        }
        private void acBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedBranch != null)
            {
                _dataContext.Data.BTFILI = _dataContext.SelectedBranch.filcod;
            }
            else
            {
                _dataContext.Data.BTFILI = null;
            }
        }
        private void acDealer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedDealer != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedDealer.rivcod))
            {
                _dataContext.Data.BTRIVE = _dataContext.SelectedDealer.rivcod;
            }
            else
            {
                _dataContext.Data.BTRIVE = null;
            }
        }
        private void acLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedLanguage != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedLanguage.lincod))
            {
                _dataContext.Data.BTLING = _dataContext.SelectedLanguage.lincod;
            }
            else
            { _dataContext.Data.BTLING = null; }
        }
        private void acHeadText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedHeadText != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedHeadText.TTxcod))
            {
                if (ConfirmHandler.Confirm($"Importando il testo\n{_dataContext.SelectedHeadText.FullDescriptionSearchable}\nquello corrente verrà completamente sostituito, proseguire ?"))
                {
                    _dataContext.Data.BTNOTET = _dataContext.SelectedHeadText.TTXNote;
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
                    _dataContext.Data.BTNOTEP = _dataContext.SelectedFootText.TTXNote;
                }
                else
                {
                    _dataContext.SelectedFootText = null;
                }
            }
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
