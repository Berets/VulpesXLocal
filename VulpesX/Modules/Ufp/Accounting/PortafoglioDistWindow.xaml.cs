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

namespace VulpesX.Modules.Ufp.Accounting
{
    /// <summary>
    /// Interaction logic for PortafoglioDistWindow.xaml
    /// </summary>
    public partial class PortafoglioDistWindow : FluentDefaultWindow
    {
        private PortafoglioDistWindowViewModel _dataContext;
        public PortafoglioDistWindow(PortafoglioDistWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
            this.Title = $"Dettagli della distinta di portafoglio n.{_dataContext.Data.id}";

            _dataContext.InternalBanks = _dataContext.GetABICABs();
            _dataContext.SelectedInternalBank = _dataContext.InternalBanks?.Where(w => w.ABI == _dataContext.Data.abi && w.CAB == _dataContext.Data.cab && w.Account == _dataContext.Data.account?.Trim()).FirstOrDefault();

            if (_dataContext.IsReadonly)
                this.Title += " [sola lettura]";
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmHandler.Confirm("Salvare le modifiche apportate alla distinta ?"))
            {
                var validate = _dataContext.Validate();

                if (string.IsNullOrWhiteSpace(validate))
                {
                    if (_dataContext.Save())
                    {
                        this.DialogResult = true;
                    }
                }
                else
                {
                    ErrorHandler.Validation(validate);
                }
            }
        }
        #endregion

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
        private void acInternalBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedInternalBank != null && _dataContext.SelectedInternalBank.ABI > 0)
            {
                _dataContext.Data.abi = _dataContext.SelectedInternalBank.ABI;
                _dataContext.Data.cab = _dataContext.SelectedInternalBank.CAB;
                _dataContext.Data.account = _dataContext.SelectedInternalBank.Account;
            }
        }
        #endregion
    }
}
