using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
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
using System.Windows.Shapes;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.ViewModels.Modules.Default.Treasury;

namespace VulpesX.Modules.Default.Treasury
{
    /// <summary>
    /// Interaction logic for BankFluxesWindow.xaml
    /// </summary>
    public partial class BankFluxesWindow : FluentDefaultWindow
    {
        private BankFluxesWindowViewModel _dataContext;
        public BankFluxesWindow(BankFluxesWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

           this.DataContext = _dataContext;

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
            await _dataContext.Load();
        }
        

        private void cmdDetailsFirst_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as BankFluxMonthItem;

            if (item != null)
            {
                DateTime from = new DateTime(_dataContext.Year, item.Month, 1);
                DateTime to = new DateTime(_dataContext.Year, item.Month, 10);

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<BankFluxesDetailsWindowViewModel>();
                windowViewModel.Year = _dataContext.Year;
                windowViewModel.GroupID = _dataContext.GroupID;
                windowViewModel.AccountID = _dataContext.AccountID;
                windowViewModel.SubaccountID = _dataContext.SubaccountID;
                windowViewModel.Bank = _dataContext.Bank;
                windowViewModel.Agency = _dataContext.Agency;
                windowViewModel.StartBalance = 0;

                var wBankFluxesDetails = new BankFluxesDetailsWindow(windowViewModel);
                wBankFluxesDetails.Owner = Window.GetWindow(this);
                wBankFluxesDetails.ShowDialog();
            }
        }

        private void cmdDetailsSecond_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as BankFluxMonthItem;

            if (item != null)
            {
                DateTime from = new DateTime(_dataContext.Year, item.Month, 11);
                DateTime to = new DateTime(_dataContext.Year, item.Month, 20);

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<BankFluxesDetailsWindowViewModel>();
                windowViewModel.Year = _dataContext.Year;
                windowViewModel.GroupID = _dataContext.GroupID;
                windowViewModel.AccountID = _dataContext.AccountID;
                windowViewModel.SubaccountID = _dataContext.SubaccountID;
                windowViewModel.Bank = _dataContext.Bank;
                windowViewModel.Agency = _dataContext.Agency;
                windowViewModel.StartBalance = 0;

                var wBankFluxesDetails = new BankFluxesDetailsWindow(windowViewModel);
                wBankFluxesDetails.Owner = Window.GetWindow(this);
                wBankFluxesDetails.ShowDialog();
            }
        }

        private void cmdDetailsThird_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as BankFluxMonthItem;

            if (item != null)
            {
                DateTime from = new DateTime(_dataContext.Year, item.Month, 21);
                DateTime to = new DateTime(_dataContext.Year, item.Month, DateTime.DaysInMonth(_dataContext.Year, item.Month));

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<BankFluxesDetailsWindowViewModel>();
                windowViewModel.Year = _dataContext.Year;
                windowViewModel.GroupID = _dataContext.GroupID;
                windowViewModel.AccountID = _dataContext.AccountID;
                windowViewModel.SubaccountID = _dataContext.SubaccountID;
                windowViewModel.Bank = _dataContext.Bank;
                windowViewModel.Agency = _dataContext.Agency;
                windowViewModel.StartBalance = 0;

                var wBankFluxesDetails = new BankFluxesDetailsWindow(windowViewModel);
                wBankFluxesDetails.Owner = Window.GetWindow(this);
                wBankFluxesDetails.ShowDialog();
            }
        }
    }
}
