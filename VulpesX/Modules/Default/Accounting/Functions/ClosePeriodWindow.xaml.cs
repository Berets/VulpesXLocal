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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Functions;

namespace VulpesX.Modules.Default.Accounting.Functions
{
    /// <summary>
    /// Interaction logic for ClosePeriodWindow.xaml
    /// </summary>
    public partial class ClosePeriodWindow : FluentDefaultWindow
    {
        private ClosePeriodWindowViewModel _dataContext;
        public ClosePeriodWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ClosePeriodWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;
        }

        private async void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.UntilDate.HasValue)
            {
                if (ConfirmHandler.Confirm($"Confermate la chiusura contabile periodica per l'esercizio {_dataContext.AccountingYear} fino al {_dataContext.UntilDate.Value.ToString("dd/MM/yyyy")} ?"))
                {
                    var result = await _dataContext.PeriodClosing();
                    if (result)
                    {
                        InfoHandler.Show("Chiusura contabile periodica terminata correttamente");
                    }
                }
            }
            else
            {
                ErrorHandler.Validation("Impossibile eseguire la chiusura senza prima aver stampato i mastrini in definitivo");
            }
        }

        private void cmbAccountingYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (cmbAccountingYear.SelectedItem as ESERCIZIO);

            if (item != null)
            {
                _dataContext.AccountingYear = item.eseann;
                _dataContext.UntilDate = item.eseusm;
            }
        }
    }
}
