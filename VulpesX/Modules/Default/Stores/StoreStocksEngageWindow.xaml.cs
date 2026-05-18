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
using VulpesX.ViewModels.Modules.Default.Stores;

namespace VulpesX.Modules.Default.Stores
{
    /// <summary>
    /// Interaction logic for StoreStocksEngageWindow.xaml
    /// </summary>
    public partial class StoreStocksEngageWindow : FluentDefaultWindow
    {
        private StoreStocksEngageWindowViewModel _dataContext;
        private decimal olderQuantity;
        public StoreStocksEngageWindow(StoreStocksEngageWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            dtpEngaged.Culture = new System.Globalization.CultureInfo("it-IT");
            dtpEngaged.Culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            dtpEngaged.Culture.DateTimeFormat.ShortTimePattern = "HH:mm:ss";
            dtpUnloaded.Culture = new System.Globalization.CultureInfo("it-IT");
            dtpUnloaded.Culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            dtpUnloaded.Culture.DateTimeFormat.ShortTimePattern = "HH:mm:ss";

            this.DataContext = _dataContext;

            if (!_dataContext.IsInsert)
                olderQuantity = _dataContext.Data.quantity ?? 0;

            _dataContext.Product = _dataContext.GetTab_Articolo();

            if (_dataContext.Product?.HasLots ?? false)
            {
                _dataContext.Lots = _dataContext.GetStore_Stocks_Lots();
                grdLots.Visibility = Visibility.Visible;
            }
            else
            {
                _dataContext.Lots = null;
                grdLots.Visibility = Visibility.Collapsed;
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate(olderQuantity);

            if (((_dataContext.Product?.HasLots ?? false) && !string.IsNullOrWhiteSpace(_dataContext.Data.lot)) || !(_dataContext.Product?.HasLots ?? false))
            {
                if (string.IsNullOrWhiteSpace(validated))
                {
                    if (!_dataContext.IsInsert)
                    {
                        if (!_dataContext.IsUnloading)
                        {
                            _dataContext.Data.update_user = _dataContext.UserID;
                            if (_dataContext.Update())
                                this.DialogResult = true;
                        }
                        else
                        {
                            // unload
                            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<StoreStocksEngageCausalWindowViewModel>();
                            windowViewModel.Engage = _dataContext.Data;

                            var wAskEngageCausal = new StoreStocksEngageCausalWindow(windowViewModel);
                            wAskEngageCausal.Owner = GetWindow(this);
                            if (wAskEngageCausal.ShowDialog() == true)
                                this.DialogResult = true;
                        }
                    }
                    else
                    {
                        if (_dataContext.Insert())
                            this.DialogResult = true;
                    }
                }
                else
                {
                    ErrorHandler.Validation(validated);
                }
            }
            else
            {
                ErrorHandler.Validation("Il lotto e' obbligatorio per questo articolo");
            }
        }
    }
}
