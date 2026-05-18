using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VulpesX.DAL;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Treasury;

namespace VulpesX.Modules.Default.Treasury
{
    /// <summary>
    /// Interaction logic for MovementsView.xaml
    /// </summary>
    public partial class MovementsView : UserControl
    {
        private MovementsViewModel _dataContext;
        public MovementsView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<MovementsViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            rdtLimitDate.SelectedDate = _dataContext.Today.AddMonths(1);

            cmbYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbYear.SelectedItem = cmbYear.Items[0];

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    LoadData();
                }
            };

            LoadData();
        }

        private async void LoadData()
        {
            if (rdtLimitDate.SelectedValue != null && cmbYear.SelectedItem != null)
            {
                var year = (cmbYear.SelectedItem as ESERCIZIO)!.eseann;
                var onlyAtBank = rtsOnlyAtBank.IsChecked ?? false;

                await _dataContext.Load(year, onlyAtBank);
            }
        }

        private void RadComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void rdtLimitDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void cmGrid_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            rmiBankFluxes.IsEnabled = GridView.SelectedItem != null;
        }

        private void rmiBankFluxes_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = GridView.SelectedItem as RBCC01F0;
            if (selected != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<BankFluxesWindowViewModel>();
                windowViewModel.Year = (cmbYear.SelectedItem as ESERCIZIO)!.eseann;
                windowViewModel.GroupID = selected.cnsl30 ?? string.Empty;
                windowViewModel.AccountID = selected.cnsl31 ?? string.Empty;
                windowViewModel.SubaccountID = selected.cnsl32 ?? string.Empty;
                windowViewModel.Bank = selected.Bank?.Bank?.abiban ?? string.Empty;
                windowViewModel.Agency = selected.Bank?.Bank?.abiage ?? string.Empty;

                var wBankFluxes = new BankFluxesWindow(windowViewModel);
                wBankFluxes.Owner = Window.GetWindow(this);
                wBankFluxes.ShowDialog();
            }
        }

        private void cmdDetails_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as RBCC01F0;
            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<BankFluxesDetailsWindowViewModel>();
                windowViewModel.Year = (cmbYear.SelectedItem as ESERCIZIO)!.eseann;
                windowViewModel.MonthFlag = null;

                windowViewModel.GroupID = item.cnsl30 ?? string.Empty;
                windowViewModel.AccountID = item.cnsl31 ?? string.Empty;
                windowViewModel.SubaccountID = item.cnsl32 ?? string.Empty;
                windowViewModel.Bank = item.Bank?.Bank?.abiban ?? string.Empty;
                windowViewModel.Agency = item.Bank?.Bank?.abiage ?? string.Empty;
                windowViewModel.From = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                windowViewModel.To = null;
                windowViewModel.StartBalance = item.DisponibilitaFutura;

                var wBankFluxesDetails = new BankFluxesDetailsWindow(windowViewModel); wBankFluxesDetails.Owner = Window.GetWindow(this);
                wBankFluxesDetails.ShowDialog();
                LoadData();
            }
        }

        private void cmdDetailsExpire_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as RBCC01F0;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<BankFluxesDetailsWindowViewModel>();
                windowViewModel.Year = (cmbYear.SelectedItem as ESERCIZIO)!.eseann;
                windowViewModel.MonthFlag = "=";

                windowViewModel.GroupID = item.cnsl30 ?? string.Empty;
                windowViewModel.AccountID = item.cnsl31 ?? string.Empty;
                windowViewModel.SubaccountID = item.cnsl32 ?? string.Empty;
                windowViewModel.Bank = item.Bank?.Bank?.abiban ?? string.Empty;
                windowViewModel.Agency = item.Bank?.Bank?.abiage ?? string.Empty;
                windowViewModel.From = null;
                windowViewModel.To = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                windowViewModel.StartBalance = item.DisponibilitaFutura;

                var wBankFluxesDetails = new BankFluxesDetailsWindow(windowViewModel);
                wBankFluxesDetails.Owner = Window.GetWindow(this);
                wBankFluxesDetails.ShowDialog();
                LoadData();
            }
        }

        private void cmdDetailsExpireNext_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as RBCC01F0;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<BankFluxesDetailsWindowViewModel>();
                windowViewModel.Year = (cmbYear.SelectedItem as ESERCIZIO)!.eseann;
                windowViewModel.MonthFlag = ">";

                windowViewModel.GroupID = item.cnsl30 ?? string.Empty;
                windowViewModel.AccountID = item.cnsl31 ?? string.Empty;
                windowViewModel.SubaccountID = item.cnsl32 ?? string.Empty;
                windowViewModel.Bank = item.Bank?.Bank?.abiban ?? string.Empty;
                windowViewModel.Agency = item.Bank?.Bank?.abiage ?? string.Empty;
                windowViewModel.From = null;
                windowViewModel.To = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                windowViewModel.StartBalance = item.DisponibilitaFutura;

                var wBankFluxesDetails = new BankFluxesDetailsWindow(windowViewModel);
                wBankFluxesDetails.Owner = Window.GetWindow(this);
                wBankFluxesDetails.ShowDialog();
                LoadData();
            }
        }

        private void cmdCastellettoDetails_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as RBCC01F0;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<BankCastellettoWindowViewModel>();
                windowViewModel.ABI = item.cnsl01;
                windowViewModel.CAB = item.cnsl02;
                windowViewModel.ABINew = (item.cnslnewabi ?? 0);
                windowViewModel.CABNew = (item.cnslnewcab ?? 0);
                windowViewModel.Bank = item.Bank?.Bank?.abiban ?? string.Empty;
                windowViewModel.Agency = item.Bank?.Bank?.abiage ?? string.Empty;

                var wCD = new BankCastellettoWindow(windowViewModel);
                wCD.Owner = Window.GetWindow(this);
                wCD.ShowDialog();
            }
        }

        #region Change grid data
        private void GridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.EditAction == Telerik.Windows.Controls.GridView.GridViewEditAction.Commit)
            {
                var model = e.Cell.DataContext as RBCC01F0;

                if (model != null)
                {
                    if (e.EditingElement is RadNumericUpDown && (e.EditingElement as RadNumericUpDown)?.Tag.ToString() == "cnsl17")
                    {
                        if (ConfirmHandler.Confirm($"Confermate [{model.cnsl17?.ToString("N2")}] come saldo banca ?"))
                        {
                            _dataContext.Update(model);
                            LoadData();
                        }
                        else
                        {
                            model.cnsl17 = (decimal?)e.OldData;
                            Mouse.OverrideCursor = null;
                        }
                    }
                }
            }

            e.Handled = true;
        }
        #endregion

        private void rtsOnlyAtBank_Checked(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void rtsOnlyAtBank_Unchecked(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
