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
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Functions;

namespace VulpesX.Modules.Default.Accounting.Functions
{
    /// <summary>
    /// Interaction logic for CloseYearWindow.xaml
    /// </summary>
    public partial class CloseYearWindow : FluentDefaultWindow
    {
        private CloseYearWindowViewModel _dataContext;
        public CloseYearWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<CloseYearWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;
            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;
        }

        private async void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.UntilDate.HasValue)
            {
                var nextYear = _dataContext.GetESERCIZIO(_dataContext.AccountingYear + 1);

                if (nextYear != null)
                {
                    if (_dataContext.NewDate.HasValue && _dataContext.NewDate.Value.Date.Year == _dataContext.AccountingYear + 1 && _dataContext.NewDate.Value.Date.Month >= (nextYear.eseini ?? 1))
                    {
                        if (ConfirmHandler.Confirm($"Confermate la chiusura contabile annuale per l'esercizio {_dataContext.AccountingYear} fino al {_dataContext.UntilDate.Value.ToString("dd/MM/yyyy")} ?"))
                        {
                            var balance = await _dataContext.ComputeLossProfit();

                            if (ConfirmHandler.Confirm($"Il sistema ha calcolato {(balance > 0 ? $"un utile di {balance.ToString("N2")}" : (balance == 0 ? $"un pareggio" : $"una perdita di {balance.ToString("N2")}"))}, proseguire ?"))
                            {
                                var result = await _dataContext.YearClosing();
                                if (result)
                                {
                                    InfoHandler.Show("Chiusura contabile annuale terminata correttamente");
                                    this.Close();
                                }
                            }
                        }
                    }
                    else
                    {
                        ErrorHandler.Validation("La data di riapertura deve essere nell'esercizio successivo a quello di chiusura");
                    }
                }
                else
                {
                    ErrorHandler.Validation("Il nuovo esercizio deve essere aperto");
                }
            }
            else
            {
                ErrorHandler.Validation("Impossibile eseguire la chiusura senza prima aver effettuato la chiusura periodica");
            }
        }

        private void cmbAccountingYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (cmbAccountingYear.SelectedItem as ESERCIZIO);

            if (item != null)
            {
                _dataContext.AccountingYear = item.eseann;
                _dataContext.UntilDate = item.eseuch;
                _dataContext.NewDate = new DateTime(item.eseann + 1, (item.eseini ?? 1), 1);
            }
        }
    }
}
