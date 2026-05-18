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
    /// Interaction logic for AssetHistoryWindow.xaml
    /// </summary>
    public partial class AssetHistoryWindow : FluentDefaultWindow
    {
        private AssetHistoryWindowViewModel _dataContext;
        public AssetHistoryWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<AssetHistoryWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            rdpDate.Culture = new System.Globalization.CultureInfo("it-IT");
            rdpDate.Culture.DateTimeFormat.ShortDatePattern = "yyyy";
            rdpDate.SelectedValue = now;

            _dataContext.SelectedComputeType = "F";
            _dataContext.DateFrom = now.Date;
            _dataContext.DateTo = now.Date;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmHandler.Confirm("Confermate i parametri per eseguire l'aggiornamento dei cespiti ?"))
            {
                Mouse.OverrideCursor = Cursors.Wait;
                cmdCancel.IsEnabled = false;
                cmdSave.IsEnabled = false;
                if (rdpDate.SelectedValue.HasValue)
                {
                    if (!string.IsNullOrWhiteSpace(_dataContext.SelectedComputeType))
                    {
                        if (_dataContext.DateFrom.HasValue && _dataContext.DateTo.HasValue && _dataContext.DateTo.Value.Date >= _dataContext.DateFrom.Value.Date)
                        {
                            if (_dataContext.UpdateHistory(rdpDate.SelectedValue.Value.Year))
                            {
                                Mouse.OverrideCursor = null;
                                InfoHandler.Show("Aggiornamento dei cespiti eseguito correttamente");
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
                            ErrorHandler.Validation("Le date di inizio e fine calcolo sono obbligatorie e la data di fine deve essere uguale o superiore alla data di inizio");
                            cmdCancel.IsEnabled = true;
                            cmdSave.IsEnabled = true;
                        }
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation("Il tipo deve avere un valore valido");
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
