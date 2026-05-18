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
using VulpesX.ViewModels.Modules.Default.Commons;

namespace VulpesX.Modules.Default.Commons
{
    /// <summary>
    /// Interaction logic for AccountingYearWindow.xaml
    /// </summary>
    public partial class AccountingYearWindow : FluentDefaultWindow
    {
        private AccountingYearWindowViewModel _dataContext;
        public AccountingYearWindow(AccountingYearWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            CultureInfo noSeparator = new CultureInfo("it-IT");
            noSeparator.NumberFormat.NumberGroupSeparator = "";

            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            cmdCancel.IsEnabled = false;
            cmdSave.IsEnabled = false;

            if (_dataContext.AccountingYear != null)
            {
                this.DialogResult = true;
            }
            else
            {
                ErrorHandler.Validation("Selezionare un esercizio");
                cmdCancel.IsEnabled = true;
                cmdSave.IsEnabled = true;
            }
        }

        private void cmbAccountingYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataContext.AccountingYear = cmbAccountingYear.SelectedItem as ESERCIZIO;
        }
    }
}
