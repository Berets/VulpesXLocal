using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
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

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for PortafoglioView.xaml
    /// </summary>
    public partial class PortafoglioView : UserControl
    {
        private PortafoglioViewModel _dataContext;
        private int _selectedPage = 0;
        private bool _isFirstLoad = true;
        public PortafoglioView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<PortafoglioViewModel>();

            InitializeComponent();

            _dataContext.From = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime().AddYears(-1);
            _dataContext.To = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

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

            GridView.DataLoaded += (s, e) =>
            {
                txtSearch_TextChanged(txtSearch, null);
            };
            GridView.SelectionChanged += (s, e) =>
            {
                _dataContext.TotalAmountSelected = GridView.SelectedItems.Cast<PNPORTAFOGLIO>().Sum(sum => sum.N6IMEU) ?? 0;

                if (GridView.SelectedItems != null && GridView.SelectedItems.Count > 0)
                {
                    cmdInsert.IsEnabled = GridView.SelectedItems.Cast<PNPORTAFOGLIO>().Where(w => w.N6STATO == "N").Count() == GridView.SelectedItems.Count;
                }
                else
                {
                    cmdInsert.IsEnabled = false;
                }
            };
            GridViewDist.DataLoaded += (s, e) =>
            {
                rdpGridDist.MoveToPage(_selectedPage);
                txtSearch_TextChanged(txtSearch, null);
            };
            rdpGridDist.PageIndexChanged += (s, e) =>
            {
                _selectedPage = e.NewPageIndex;
            };

            cmbYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbYear.SelectedItem = cmbYear.Items[0];
            cmbStatus.ItemsSource = CommonsService.WalletStatus;
            cmbStatus.SelectedValue = "N";
        }

        private async Task LoadData()
        {
            string? selectedStatusID = cmbStatus.SelectedValue?.ToString();

            if (string.IsNullOrWhiteSpace(selectedStatusID))
                selectedStatusID = "N";

            await _dataContext.Load(selectedStatusID);

            _isFirstLoad = false;
        }

        private async Task LoadDataHistory()
        {
            int year = (cmbYear.SelectedItem as ESERCIZIO)?.eseann ?? DateTime.Now.Year;

            await _dataContext.LoadHistory(year);
        }


        #region Buttons
        private async void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            if (GridView.SelectedItems != null && GridView.SelectedItems.Count > 0)
            {
                var windowYearViewModel = VulpesServiceProvider.Provider.GetRequiredService<AccountingYearWindowViewModel>();

                var windowYear = new AccountingYearWindow(windowYearViewModel);
                if (windowYear.ShowDialog() ?? false)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PortafoglioDistWindowViewModel>();
                    windowViewModel.Data = new PNPORTAFOGLIO_DIST()
                    {
                        company_id = _dataContext.CompanyID,
                        id = _dataContext.GetDistinctLongID(windowYearViewModel.AccountingYear!.eseann),
                        amount = GridView.SelectedItems.Cast<PNPORTAFOGLIO>().Sum(sum => sum.N6IMEU) ?? 0,
                        addedUserID = _dataContext.UserID,
                        extraction_date = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime(),
                        Items = new ObservableCollection<PNPORTAFOGLIO>(GridView.SelectedItems.Cast<PNPORTAFOGLIO>())
                    };
                    windowViewModel.IsInsert = true;

                    var wDist = new PortafoglioDistWindow(windowViewModel);
                    if (wDist.ShowDialog() == true)
                    {
                        await LoadData();
                        await LoadDataHistory();
                    }
                }
            }
        }

        private async void cmdEditDist_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selected = (sender as Button)?.DataContext as PNPORTAFOGLIO_DIST;

            if (selected != null)
            {
                selected = _dataContext.GetFull(selected.id);

                if (selected != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PortafoglioDistWindowViewModel>();
                    windowViewModel.Data = selected;
                    windowViewModel.IsInsert = false;
                    var wDist = new PortafoglioDistWindow(windowViewModel);
                    if (wDist.ShowDialog() == true)
                    {
                        await LoadData();
                        await LoadDataHistory();
                    }
                }
            }
        }
        #endregion

        #region Icons tasks
        private async void rgDelete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as PNPORTAFOGLIO_DIST;

            if (item != null)
            {
                item = _dataContext.GetFull(item.id);

                if (item != null)
                {
                    if (!item.accounting_date.HasValue)
                    {
                        if (ConfirmHandler.Confirm($"Confermate l'eliminazione della distinta n.{item.id} per un totale di {item.amount.ToString("N2")} ?"))
                        {
                            if (_dataContext.DeleteAll(item))
                            {
                                await LoadData();
                                await LoadDataHistory();
                            }
                        }
                    }
                }
            }
        }

        private async void rgAccounting_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as PNPORTAFOGLIO_DIST;
            if (item != null)
            {
                item = _dataContext.GetFull(item.id);

                if (item != null)
                {
                    if (!item.accounting_date.HasValue)
                    {
                        if (ConfirmHandler.Confirm($"Confermate la contabilizzazione della distinta n.{item.id} per un totale di {item.amount.ToString("N2")} ?"))
                        {
                            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<AskWalletAccountWindowViewModel>();
                            var wInfo = new AskWalletAccountWindow(windowViewModel);
                            if (wInfo.ShowDialog() == true)
                            {
                                if (windowViewModel.SelectedDate != null && windowViewModel.AccountingYear != null)
                                {
                                    if (_dataContext.Accounting(item, windowViewModel.SelectedDate.Value, windowViewModel.AccountingYear))
                                    {
                                        await LoadData();
                                        await LoadDataHistory();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private async void rgGenerate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as PNPORTAFOGLIO_DIST;
            if (item != null)
            {
                item = _dataContext.GetFull(item.id);

                if (item != null)
                {
                    if (ConfirmHandler.Confirm($"Confermate la creazione del file CBI per la distinta n.{item.id} per un totale di {item.amount.ToString("N2")} ?"))
                    {
                        var folderDialog = new OpenFolderDialog
                        {
                            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                            Title = "Seleziona la cartella di destinazione"
                        };

                        if (folderDialog.ShowDialog() == true)
                        {
                            bool openFile = ConfirmHandler.Confirm("Aprire il file generato al termine dell'operazione ?");
                            Mouse.OverrideCursor = Cursors.Wait;
                            if (_dataContext.GenerateFileCBI(item, folderDialog.FolderName, openFile))
                            {
                                await LoadData();
                                await LoadDataHistory();
                            }
                        }
                    }
                }
            }
            Mouse.OverrideCursor = null;
        }
        #endregion

        #region Autocompletes events
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
        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }
        #endregion

        #region Cell edit
        private async void GridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.EditAction == Telerik.Windows.Controls.GridView.GridViewEditAction.Commit)
            {
                var model = e.Cell.DataContext as PNPORTAFOGLIO;

                if (model != null)
                {
                    if (e.Cell.Column.UniqueName == "colCustomerBank")
                    {
                        if (model.SelectedCustomerBank != null)
                        {
                            model.N6CABI = model.SelectedCustomerBank.ABI;
                            model.N6CCAB = model.SelectedCustomerBank.CAB;

                            _dataContext.Update(model);
                            await LoadData();
                        }
                        else
                        {
                            ErrorHandler.Validation("Selezionare una banca valida");
                            e.Handled = false;
                        }
                    }

                    if (e.EditingElement is RadDateTimePicker)
                    {
                        if (ConfirmHandler.Confirm($"Confermate {(model.N6SCAD.HasValue ? ($@"{model.N6SCAD.Value.ToString("dd/MM/yyyy")} come data scadenza ?") : " lo svuotamento della data scadenza ?")}"))
                        {
                            _dataContext.Update(model);
                            await LoadData();
                        }
                        else
                        {
                            model.N6SCAD = e.OldData != null ? DateTime.Parse(e.OldData?.ToString() ?? DateTime.MinValue.ToString()) : null;
                        }
                    }
                    else if (e.EditingElement is RadNumericUpDown)
                    {
                        if (ConfirmHandler.Confirm($"Confermate {(model.N6IMEU.HasValue ? ($@"{model.N6IMEU.Value.ToString("N2")} come importo ?") : " lo svuotamento dell'importo ?")}"))
                        {
                            _dataContext.Update(model);
                            await LoadData();
                        }
                        else
                        {
                            model.N6IMEU = e.OldData != null ? decimal.Parse(e.OldData?.ToString() ?? "0") : null;
                        }
                    }
                }
            }
            e.Handled = true;
        }

        private void GridView_BeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
        {
            if ((e.Cell.DataContext as PNPORTAFOGLIO)?.N6STATO != "N")
                e.Cancel = true;

            if (e.Cell.Column.UniqueName == "colCustomerBank")
            {
                var model = (e.Cell.DataContext as PNPORTAFOGLIO);

                model!.Banks = _dataContext.BanksCache;
                model!.SelectedCustomerBank = model!.Banks?.Where(o => o.ABI == model.N6CABI && o.CAB == model.N6CCAB).FirstOrDefault();
            }
        }
        #endregion

        #region Context menu
        private void cmGrid_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            rmiDiscard.IsEnabled = GridView.SelectedItems != null && GridView.SelectedItems.Count > 0;
            foreach (var item in GridView.SelectedItems?.Cast<PNPORTAFOGLIO>().ToList() ?? new List<PNPORTAFOGLIO>())
            {
                if (item.N6STATO != "N")
                {
                    rmiDiscard.IsEnabled = false;
                    break;
                }
            }
        }

        private async void rmiDiscard_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridView.SelectedItems != null && GridView.SelectedItems.Count > 0)
            {
                if (ConfirmHandler.Confirm($"Scartare {(GridView.SelectedItems.Count == 1 ? "la disposizione selezionata ?" : $"le {GridView.SelectedItems.Count.ToString("N0")} disposizioni selezionate ?")}\n\nATTENZIONE: non sarà più possibile procedere alla {(GridView.SelectedItems.Count == 1 ? "sua" : "loro")} estrazione"))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    foreach (var item in GridView.SelectedItems.Cast<PNPORTAFOGLIO>())
                    {
                        item.N6STATO = "S";
                        _dataContext.Update(item);
                    }
                    await LoadData();
                    await LoadDataHistory();
                    Mouse.OverrideCursor = null;
                }
            }
        }
        #endregion

        #region UC standard functions
        private async void rdtFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isFirstLoad)
            {
                await LoadData();
            }
        }

        private async void rdtTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isFirstLoad)
            {
                await LoadData();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        private async void cmbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadData();
            await LoadDataHistory();
        }

        private async void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadData();
        }
        #endregion

        private async void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as PNPORTAFOGLIO;
            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminazione del seguente portafoglio ?\n{item.CustomerDescription} Scadenza {item.N6SCAD?.ToString("dd/MM/yyyy")} Importo {item.N6IMEU?.ToString("N2")}"))
                {
                    if (_dataContext.Delete(item))
                    {
                        InfoHandler.Show("Eliminazione eseguita correttamente");
                    }

                    await LoadData();
                    await LoadDataHistory();
                }
            }
        }

        private async void rgDuplicate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)!.DataContext as PNPORTAFOGLIO;
            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate la duplicazione della seguente disposizione ?\n{item.CustomerDescription} Scadenza {item.N6SCAD?.ToString("dd/MM/yyyy")} Importo {item.N6IMEU?.ToString("N2")}"))
                {
                    if (_dataContext.Duplicate(item))
                    {
                        InfoHandler.Show("Duplicazione della disposizione eseguita correttamente");
                    }

                    await LoadData();
                    await LoadDataHistory();
                }
            }
        }

    }
}
