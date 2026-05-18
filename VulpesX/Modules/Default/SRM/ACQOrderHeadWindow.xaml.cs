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
using VulpesX.Models;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.SRM;

namespace VulpesX.Modules.Default.SRM
{
    /// <summary>
    /// Interaction logic for ACQOrderHeadWindow.xaml
    /// </summary>
    public partial class ACQOrderHeadWindow : FluentDefaultWindow
    {
        private ACQOrderHeadWindowViewModel _dataContext;
        public ACQOrderHeadWindow(ACQOrderHeadWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            _dataContext.Data.SupplierChanged += ChangedSupplier;
            _dataContext.Data.PaymentChanged += ChangedPayment;
          
            #region Check is readonly
            bool isenabled = true;
            if (_dataContext.Data.commercial_signed.HasValue && !_dataContext.Data.management_signed.HasValue)
            {
                if (UserContext.Instance!.ACCESS!.Roles?.canPOSignCommercial ?? false)
                    isenabled = true;
                else
                    isenabled = false;
            }
            else
            {
                if (_dataContext.Data.commercial_signed.HasValue && _dataContext.Data.management_signed.HasValue)
                {
                    if (UserContext.Instance!.ACCESS!.Roles?.canPOSignManagement ?? false)
                        isenabled = true;
                    else
                        isenabled = false;
                }
            }
            _dataContext.IsReadonly = _dataContext.Data.canceled.HasValue || _dataContext.Data.closed.HasValue ? false : isenabled;
            #endregion
            
            // set info to display
            if (_dataContext.IsInsert)
            {
                spInsertInfo.Visibility = Visibility.Visible;
                spEditInfo.Visibility = Visibility.Collapsed;
            }
            else
            {
                spInsertInfo.Visibility = Visibility.Collapsed;
                spEditInfo.Visibility = Visibility.Visible;
            }

            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli generali ordine di acquisto {(_dataContext.IsInsert ? "nuovo" : _dataContext.Data.id)}";
                if (!_dataContext.IsReadonly)
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
                if (_dataContext.Save())
                {
                    Mouse.OverrideCursor = null;
                    this.DialogResult = true;
                }
                Mouse.OverrideCursor = null;
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation(validated);
            }
        }
        #endregion

        #region Autocompletes
        private void acText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedHeadText != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedHeadText.TTxcod))
            {
                if (ConfirmHandler.Confirm($"Importando il testo\n{_dataContext.SelectedHeadText.FullDescriptionSearchable}\nquello corrente verrà completamente sostituito, proseguire ?"))
                {
                    _dataContext.Data.note = _dataContext.SelectedHeadText.TTXNote;
                }
                else
                {
                    _dataContext.SelectedHeadText = null;
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

        public void ChangedSupplier(object? sender, EventArgs e)
        {
            var item = sender as acq_orders_heads;

            if (item != null)
            {
                if (item.Supplier != null || string.IsNullOrWhiteSpace(item.payment_id))
                {
                    var supplierInfo = _dataContext.GetSupplier(item.supplier_id);
                    if (item.Payments != null)
                    {
                        item.Payment = item.Payments.Where(w => w.pfocod == supplierInfo?.pfocod).FirstOrDefault();
                        if (item.Payment?.PaymentType?.incsup != "R")
                        {
                            item.bank_account = supplierInfo?.FOCOCO;
                            item.Bank = item.Banks?.Where(w => w.ABI == supplierInfo?.FOABIF && w.CAB == supplierInfo?.FOCABF).FirstOrDefault();
                        }
                        else
                        {
                            item.Bank = item.Banks?.Where(w => w.ABI == supplierInfo?.FOAABI && w.CAB == supplierInfo?.FOACAB && w.Account == supplierInfo.FOACCN).FirstOrDefault();
                        }
                    }
                    else
                    {
                        item.Banks = _dataContext.GetSimpleList();
                        item.Bank = null;
                    }
                    if (item.Shipments != null)
                        item.Shipment = item.Shipments.Where(w => w.specod == supplierInfo?.specod).FirstOrDefault();
                    if (item.Deliveries != null)
                        item.Delivery = item.Deliveries.Where(w => w.concod == supplierInfo?.concod).FirstOrDefault();
                }
            }
        }

        public void ChangedPayment(object? sender, EventArgs e)
        {
            var item = sender as acq_orders_heads;
            if (item != null)
            {
                if (item.Payment != null && item.Payment.PaymentType != null && !string.IsNullOrEmpty(item.Payment.PaymentType.incsup))
                {
                    item.Banks = _dataContext.GetSimpleListSuppliers(item.Payment.PaymentType.incsup);
                    // get default bank
                    if (item.Payment != null || (!item.bank_abi.HasValue && !item.bank_cab.HasValue))
                    {
                        if (item.Payment?.PaymentType.incsup != "R")
                        {
                            // load default supplier bank
                            var supplierData = _dataContext.GetSupplier(item.supplier_id);
                            if (supplierData != null)
                            {
                                item.Bank = item.Banks?.Where(w => w.ABI == supplierData.FOABIF && w.CAB == supplierData.FOCABF).FirstOrDefault();
                                item.bank_account = supplierData.FOCOCO;
                            }
                        }
                    }
                }
                else
                {
                    item.Banks = null;
                }
            }
        }
    }
}
