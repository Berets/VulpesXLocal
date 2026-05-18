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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Stores;

namespace VulpesX.Modules.Default.Stores
{
    /// <summary>
    /// Interaction logic for StoreStocksEngageCausalWindow.xaml
    /// </summary>
    public partial class StoreStocksEngageCausalWindow : FluentDefaultWindow
    {
        private StoreStocksEngageCausalWindowViewModel _dataContext;
        public StoreStocksEngageCausalWindow(StoreStocksEngageCausalWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.UnloadCausals = _dataContext.GetStore_Causals();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var causal = (cmbCausal.SelectedItem as store_causals);

            if (causal != null)
            {
                if (_dataContext.Unload(causal))
                {
                    this.DialogResult = true;
                }
            }
            else
            {
                ErrorHandler.Validation("La causale e' obbligatoria");
            }
        }
    }
}
