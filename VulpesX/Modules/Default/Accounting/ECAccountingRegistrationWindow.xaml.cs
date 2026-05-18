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
using Telerik.Windows.Controls.GridView.GridView;
using VulpesX.DAL;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for ECAccountingRegistrationWindow.xaml
    /// </summary>
    public partial class ECAccountingRegistrationWindow : FluentDefaultWindow
    {
        private ECAccountingRegistrationWindowViewModel _dataContext;
        public ECAccountingRegistrationWindow(ECAccountingRegistrationWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            this.Title = (_dataContext.EntityType == "F") ? $"Registrazione pagamento fornitore - {_dataContext.Entity.abecod}-{_dataContext.Entity.abers1?.TrimEnd()}" :
                $"Registrazione incasso cliente - {_dataContext.Entity.abecod}-{_dataContext.Entity.abers1?.TrimEnd()}";

            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;

            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            _dataContext.AccountingYear = (cmbAccountingYear.Items[0] as ESERCIZIO)?.eseann;
            _dataContext.Date = now;
            _dataContext.DocumentDate = now;
            _dataContext.Causals = _dataContext.GetCAUCONTs();
            _dataContext.Banks = _dataContext.GetBANAZIENs();

            if (_dataContext.BankABI.HasValue && _dataContext.BankCAB.HasValue && !string.IsNullOrEmpty(_dataContext.BankCC))
            {
                acInternalBank.IsEnabled = false;
                _dataContext.SelectedBank = _dataContext.Banks?.Where(o => o.abiabi == _dataContext.BankABI && o.abicab == _dataContext.BankCAB && o.abicon == _dataContext.BankCC).FirstOrDefault();
            }

            foreach (var row in _dataContext.Items ?? new System.Collections.ObjectModel.ObservableCollection<Models.Models.Reports.Accounting.MastrinoECReportItem>())
            {
                row.Valore = (row.Segno == "A") ? row.SaldoAVERE : row.SaldoDARE;
                row.TypeID = "S";
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validate = _dataContext.Validation();

            if (string.IsNullOrEmpty(validate))
            {
                var result = _dataContext.Save();

                if (!string.IsNullOrEmpty(result))
                {
                    _dataContext.RegistrationYear = short.Parse(result.Split('/')[0]);
                    _dataContext.RegistrationNumber = int.Parse(result.Split("/")[1]);

                    InfoHandler.Show($"Creata registrazione N° {result}");
                    this.DialogResult = true;
                }
            }
            else
            {
                ErrorHandler.Validation(validate);
            }
        }

        private void cmbAccountingYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataContext.AccountingYear = (cmbAccountingYear.SelectedItem as ESERCIZIO)?.eseann;
        }

        private void acCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCausal.caucod))
            {
            }
        }

        private void acCausal_LostFocus(object sender, RoutedEventArgs e)
        {
            var autoCompleteBox = sender as RadAutoCompleteBox;

            if (autoCompleteBox != null)
            {
                if (autoCompleteBox.SelectedItem == null)
                {
                    autoCompleteBox.SearchText = null;
                    _dataContext.SelectedCausal = null;
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


        private void rgvRows_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {

        }

        private void rgvRows_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {

        }

        private void rgvRows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {

        }
    }
}
