using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VulpesX.Modules.Default.Commons;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.IVA;
using VulpesX.ViewModels.Modules.Default.Commons;

namespace VulpesX.Modules.Default.Accounting.IVA
{
    /// <summary>
    /// Interaction logic for PNIVAView.xaml
    /// </summary>
    public partial class PNIVAView : UserControl
    {
        private PNIVAViewModel _dataContext;
        private int _selectedPage = 0;

        public PNIVAView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<PNIVAViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    LoadData();
                }
            };

            GridView.DataLoaded += (s, e) =>
            {
                rdpGrid.MoveToPage(_selectedPage);
                txtSearch_TextChanged(txtSearch, null);
            };
            rdpGrid.PageIndexChanged += (s, e) =>
            {
                _selectedPage = e.NewPageIndex;
            };

            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
            _dataContext.IVABooks = _dataContext.GetLIBRIIVAs();
            
            _dataContext.SelectedIVABook = _dataContext.IVABooks?.FirstOrDefault();
            _dataContext.SelectedPrintedStatus = _dataContext.PrintedStatuses.Where(w => w.ID == "N").FirstOrDefault();
            _dataContext.SinceDate = new DateTime(now.Year, 1, 1);
            _dataContext.UntilDate = now.Date;
            _dataContext.ExpireDate = null;
        }

        private async void LoadData()
        {
            await _dataContext.Load();
        }

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
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

        private void acIVABook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }
        #endregion

        #region Filters
        private void cmbPrintStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void RadDateTimePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void chkOnlyUnpaid_Checked(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
        #endregion

        #region Change grid data
        private void GridView_BeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
        {
            if (GridView.SelectedItems.Count > 1)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<SingleDateWindowViewModel>();
                windowViewModel.SelectedDate = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                windowViewModel.AllowNullDate = true;

                var window = new SingleDateWindow(windowViewModel);
                if (window.ShowDialog() == true)
                {
                    if (e.Cell.DataColumn.UniqueName == "N4DTPGEF")
                    {
                        if (ConfirmHandler.Confirm($"Confermate{(windowViewModel.SelectedDate.HasValue ? $" il {windowViewModel.SelectedDate?.ToString("dd/MM/yyyy")} come data del pagamento " : " lo svuotamento della data di pagamento ")} per i {GridView.SelectedItems.Count} elementi selezionati ?"))
                        {
                            Mouse.OverrideCursor = Cursors.Wait;
                            foreach (var item in GridView.SelectedItems.Cast<PNIVA>())
                            {
                                item.N4DTPGEF = windowViewModel.SelectedDate;
                                _dataContext.Update(item);
                            }
                            e.Cancel = true;
                            Mouse.OverrideCursor = null;
                            InfoHandler.Show("Operazione eseguita correttamente");
                        }
                        else
                        {
                            e.Cancel = true;
                            Mouse.OverrideCursor = null;
                        }
                    }
                    if (e.Cell.DataColumn.UniqueName == "N4DTSCPG")
                    {
                        if (ConfirmHandler.Confirm($"Confermate{(windowViewModel.SelectedDate.HasValue ? $" il {windowViewModel.SelectedDate?.ToString("dd/MM/yyyy")} come data scadenza " : " lo svuotamento della data scadenza ")} per i {GridView.SelectedItems.Count} elementi selezionati ?"))
                        {
                            Mouse.OverrideCursor = Cursors.Wait;
                            foreach (var item in GridView.SelectedItems.Cast<PNIVA>())
                            {
                                item.N4DTSCPG = windowViewModel.SelectedDate;
                                _dataContext.Update(item);
                            }
                            e.Cancel = true;
                            Mouse.OverrideCursor = null;
                            InfoHandler.Show("Operazione eseguita correttamente");
                        }
                        else
                        {
                            e.Cancel = true;
                            Mouse.OverrideCursor = null;
                        }
                    }
                }
                else
                {
                    e.Cancel = true;
                    Mouse.OverrideCursor = null;
                }
                e.Handled = true;
                LoadData();
            }
        }

        private void GridView_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.EditAction == Telerik.Windows.Controls.GridView.GridViewEditAction.Commit)
            {
                var model = e.Cell.DataContext as PNIVA;
                if (model != null)
                {
                    if (e.EditingElement is RadDateTimePicker && (e.EditingElement as RadDateTimePicker)?.Tag.ToString() == "PAY")
                    {
                        if (ConfirmHandler.Confirm($"Confermate {(model.N4DTPGEF.HasValue ? ($@"{model.N4DTPGEF.Value.ToString("dd/MM/yyyy")} come data di pagamento ?") : " lo svuotamento della data di pagamento ?")}"))
                        {
                            _dataContext.Update(model);
                            LoadData();
                        }
                        else
                        {
                            model.N4DTPGEF = (DateTime?)e.OldData;
                            Mouse.OverrideCursor = null;
                        }
                    }
                    if (e.EditingElement is RadDateTimePicker && (e.EditingElement as RadDateTimePicker)?.Tag.ToString() == "EXP")
                    {
                        if (ConfirmHandler.Confirm($"Confermate {(model.N4DTSCPG.HasValue ? ($@"{model.N4DTSCPG.Value.ToString("dd/MM/yyyy")} come data scadenza ?") : " lo svuotamento della data scadenza ?")}"))
                        {
                            _dataContext.Update(model);
                            LoadData();
                        }
                        else
                        {
                            model.N4DTSCPG = (DateTime?)e.OldData;
                            Mouse.OverrideCursor = null;
                        }
                    }
                }
            }

            e.Handled = true;
        }
        #endregion
    }
}
