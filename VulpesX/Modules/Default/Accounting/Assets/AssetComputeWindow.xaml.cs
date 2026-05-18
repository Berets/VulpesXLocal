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
using VulpesX.DAL;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;
using VulpesX.ViewModels.Modules.Default.Accounting.Assets;

namespace VulpesX.Modules.Default.Accounting.Assets
{
    /// <summary>
    /// Interaction logic for AssetComputeWindow.xaml
    /// </summary>
    public partial class AssetComputeWindow : FluentDefaultWindow
    {
        private AssetComputeWindowViewModel _dataContext;
        public AssetComputeWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<AssetComputeWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            rdpDate.Culture = new System.Globalization.CultureInfo("it-IT");
            rdpDate.Culture.DateTimeFormat.ShortDatePattern = "yyyy";
            rdpDate.SelectedValue = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            _dataContext.SelectedComputeType = "F";
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmHandler.Confirm("Confermate i parametri per eseguire il calcolo dell'ammortamento ?"))
            {
                Mouse.OverrideCursor = Cursors.Wait;

                cmdCancel.IsEnabled = false;
                cmdSave.IsEnabled = false;

                if (rdpDate.SelectedValue.HasValue)
                {
                    if (rdpDateTo.SelectedValue.HasValue)
                    {
                        #region Retrieve parms and checks
                        // esercizio
                        var esercizio = _dataContext.GetESERCIZIO(rdpDate.SelectedValue.Value.Year);
                        if (esercizio == null)
                        {
                            Mouse.OverrideCursor = null;
                            ErrorHandler.Validation($"L'esercizio {rdpDate.SelectedValue.Value.Year} non esiste");
                            cmdCancel.IsEnabled = true;
                            cmdSave.IsEnabled = true;
                            e.Handled = true;
                        }
                        // previous esercizio month
                        var prevMonth = (esercizio!.eseini.HasValue && esercizio.eseini.Value > 1 ? esercizio.eseini.Value - 1 : 12);
                        #endregion
                        if ((esercizio.eseini == 1 && rdpDateTo.SelectedValue.Value.Year == rdpDate.SelectedValue.Value.Year) ||
                            (esercizio.eseini != 1 && rdpDateTo.SelectedValue.Value >= new DateTime(rdpDate.SelectedValue.Value.Year, (esercizio.eseini ?? 1), 1) && rdpDateTo.SelectedValue.Value <= new DateTime(rdpDate.SelectedValue.Value.Year + 1, prevMonth, DateTime.DaysInMonth(rdpDate.SelectedValue.Value.Year, prevMonth))))
                        {
                            if (_dataContext.ComputeDepreciation(rdpDate.SelectedValue.Value.Year, rdpDateTo.SelectedValue.Value, esercizio))
                            {
                                Mouse.OverrideCursor = null;
                                InfoHandler.Show("Calcolo ammortamento eseguito correttamente");
                                this.DialogResult = true;
                            }
                            else
                            {
                                Mouse.OverrideCursor = null;
                                cmdCancel.IsEnabled = true;
                                cmdSave.IsEnabled = true;
                            }
                        }
                        else
                        {
                            Mouse.OverrideCursor = null;
                            ErrorHandler.Validation("La data fino alla quale elaborare deve essere inclusa nell'esercizio selezionato");
                            cmdCancel.IsEnabled = true;
                            cmdSave.IsEnabled = true;
                        }
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation("La data fino alla quale elaborare è obbligatoria");
                        cmdCancel.IsEnabled = true;
                        cmdSave.IsEnabled = true;
                    }
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("L'anno deve avere un valore valido");
                    cmdCancel.IsEnabled = true;
                    cmdSave.IsEnabled = true;
                }
            }
        }
    }
}
