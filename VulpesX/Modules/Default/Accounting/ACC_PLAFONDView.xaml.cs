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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VulpesX.DAL;
using VulpesX.Models.Default;
using VulpesX.Modules.Default.Commons;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;
using VulpesX.ViewModels.Modules.Default.Commons;
using VulpesX.WindowsFactory.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for ACC_PLAFONDView.xaml
    /// </summary>
    public partial class ACC_PLAFONDView : UserControl
    {
        private ACC_PLAFONDViewModel _dataContext;
        public ACC_PLAFONDView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ACC_PLAFONDViewModel>();

            InitializeComponent();

            _dataContext.Year = DateTime.Now;

            dtpYear.Culture = new System.Globalization.CultureInfo("it-IT");
            dtpYear.Culture.DateTimeFormat.ShortDatePattern = "yyyy";

            this.DataContext = _dataContext;
            this.PreviewKeyDown += async (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    await LoadData();
                }
            };
            this.Loaded += async (s, e) =>
            {
                await LoadData();
            };

        }

        private async Task LoadData()
        {
            await _dataContext.Load();
        }

        #region Buttons
        private async void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as ACC_PLAFOND;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_PLAFONDWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wACC_PLAFOND = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDWindowFactory>().Create(windowViewModel);
                wACC_PLAFOND.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wACC_PLAFOND.ShowDialog() == true)
                    await LoadData();
            }
        }

        private async void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as ACC_PLAFOND;

            if (item != null)
            {
                if (!item.clidatchi.HasValue)
                {
                    if (item.cliimpfattprog == 0 && item.cliimpfattprovv == 0)
                    {
                        if (ConfirmHandler.Confirm($"Confermate l'annullamento della dichiarazione d'intento n.{item.FullID} ?"))
                        {
                            // ask for reason
                            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CancelReasonWindowViewModel>();

                            var wAskCR = new CancelReasonWindow(windowViewModel);
                            wAskCR.Owner = Window.GetWindow(this);
                            Mouse.OverrideCursor = null;
                            if (wAskCR.ShowDialog() == true && !string.IsNullOrWhiteSpace(windowViewModel.SelectedReason))
                            {
                                Mouse.OverrideCursor = Cursors.Wait;

                                await _dataContext.Cancel(item, windowViewModel.SelectedReason);

                                Mouse.OverrideCursor = null;
                                InfoHandler.Show($"Dichiarazione d'intento n.{item.FullID} annullata con successo.");
                                await LoadData();
                            }
                        }
                        Mouse.OverrideCursor = null;
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation("Impossibile annullare una dichiarazione d'intento con già del fatturato");
                    }
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("Impossibile annullare una dichiarazione d'intento chiusa");
                }
            }
        }

        private async void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_PLAFONDWindowViewModel>();
            windowViewModel.Data = new ACC_PLAFOND()
            {
                Cliasoc = _dataContext.CompanyID,
                cliannosol = dtpYear.SelectedValue?.Year ?? _dataContext.GetEsercizio() ?? 0,
                clistart = now,
                cliimpesefino = 0,
                cliimpfattprog = 0,
                cliimpfattprovv = 0
            };
            windowViewModel.IsInsert = true;

            var wACC_PLAFOND = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDWindowFactory>().Create(windowViewModel);
            wACC_PLAFOND.Owner = Window.GetWindow(this);
            Mouse.OverrideCursor = null;
            if (wACC_PLAFOND.ShowDialog() == true)
                await LoadData();
        }

        private void cmdSettings_Click(object sender, RoutedEventArgs e)
        {
            var data = _dataContext.GetACC_PLAFOND_PARMS();
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_PLAFOND_PARMSWindowViewModel>();

            if (data == null)
            {
                windowViewModel.Data = new ACC_PLAFOND_PARMS
                {
                    company_id = _dataContext.CompanyID,
                    limit_amount = 0.01m,
                    stamp_amount = 0.01m,
                    addedUserID = _dataContext.UserID,
                    rate_code = string.Empty,
                    rate_value = string.Empty,
                };
                windowViewModel.IsInsert = true;
            }
            else
            {
                windowViewModel.Data = data;
                windowViewModel.IsInsert = false;
            }

            var wSettings = new ACC_PLAFOND_PARMSWindow(windowViewModel);
            wSettings.Owner = Window.GetWindow(this);
            wSettings.ShowDialog();
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        private async void dtpYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadData();
        }

        private void GridView_DataLoaded(object sender, EventArgs e)
        {
            txtSearch_TextChanged(txtSearch, null);
        }

        #endregion

        #region Context menu
        private async void rmiClose_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = GridView.SelectedItems.Cast<ACC_PLAFOND>();

            if (selected != null && selected.Any())
            {
                if (ConfirmHandler.Confirm($"Hai selezionato {selected.Count()} dichiarazioni di intento da chiudere. Confermi?"))
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<SingleDateWindowViewModel>();
                    windowViewModel.SelectedDate = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                    windowViewModel.AllowNullDate = false;

                    var window = new SingleDateWindow(windowViewModel);
                    if (window.ShowDialog() == true && windowViewModel.SelectedDate != null)
                    {
                        var result = _dataContext.Close(selected.ToList(), windowViewModel.SelectedDate!.Value);

                        if (result)
                        {
                            await LoadData();
                        }
                    }
                }
            }
            else
            {
                ErrorHandler.Validation("Selezionare almeno una dichiarazione di intento");
            }
        }
        #endregion
    }
}
