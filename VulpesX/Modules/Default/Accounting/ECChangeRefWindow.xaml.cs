using DocumentFormat.OpenXml.Bibliography;
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
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for ECChangeRefWindow.xaml
    /// </summary>
    public partial class ECChangeRefWindow : FluentDefaultWindow
    {
        private ECChangeRefWindowViewModel _dataContext;
        public ECChangeRefWindow(ECChangeRefWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Title = $"Cambio riferimenti registrazione n.{_dataContext.EC.Number}";
            this.DataContext = _dataContext;

            var payments =_dataContext.GetPAGCLIs();
            var paymentTypes = _dataContext.GetTAB_ACC_TIPPAGs();

            _dataContext.ExpireDate = _dataContext.EC.ExpireDate;
            _dataContext.ReferenceDate = _dataContext.EC.ReferenceDate;
            _dataContext.ReferenceID = _dataContext.EC.ReferenceID;
            _dataContext.Payments = payments;
            _dataContext.PaymentTypes = paymentTypes;
            _dataContext.Payment = payments?.Where(w => w.ID == _dataContext.EC.PaymentID).FirstOrDefault();
            _dataContext.PaymentType = paymentTypes?.Where(w => w.inccod == _dataContext.EC.PaymentTypeID).FirstOrDefault();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.ExpireDate.HasValue && _dataContext.ReferenceDate.HasValue && !string.IsNullOrWhiteSpace(_dataContext.ReferenceID) &&
                _dataContext.Payment != null && _dataContext.PaymentType != null)
            {
                if (ConfirmHandler.Confirm("Confermate le modifiche alla registrazione contabile ?"))
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    if(_dataContext.Save())
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation("Registrazione aggiornata correttamente!");
                        this.DialogResult = true;
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation("Errore durante l'aggiornamento");
                        this.DialogResult = false;
                    }
                }
            }
            else
            {
                ErrorHandler.Validation("Tutti le informazioni richieste sono obbligatorie");
            }
        }

        #region Autocompletes
        private void acPayment_LostFocus(object sender, RoutedEventArgs e)
        {
            var autoCompleteBox = sender as RadAutoCompleteBox;
            if (autoCompleteBox != null)
            {
                if (autoCompleteBox.SelectedItem == null)
                {
                    autoCompleteBox.SearchText = null;
                }
            }
        }

        private void acPaymentType_LostFocus(object sender, RoutedEventArgs e)
        {
            var autoCompleteBox = sender as RadAutoCompleteBox;

            if (autoCompleteBox != null)
            {
                if (autoCompleteBox.SelectedItem == null)
                {
                    autoCompleteBox.SearchText = null;
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
    }
}
