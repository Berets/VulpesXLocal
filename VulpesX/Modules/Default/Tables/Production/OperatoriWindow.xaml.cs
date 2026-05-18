using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using VulpesX.ViewModels.Modules.Default.Tables.Production;

namespace VulpesX.Modules.Default.Tables.Production
{
    /// <summary>
    /// Interaction logic for OperatoriWindow.xaml
    /// </summary>
    public partial class OperatoriWindow : FluentDefaultWindow
    {
        private OperatoriWindowViewModel _dataContext;

        public OperatoriWindow(OperatoriWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();

            // Update operator in costs
            foreach (var cst in _dataContext.Costs ?? new ObservableCollection<tab_produzione_operatore_costo>())
            {
                cst.OperatoreID = _dataContext.Data.ID;
            }

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save())
                {
                    this.DialogResult = true;
                }
            }
            else
            { ErrorHandler.Validation(validated); }
        }

        private void txtBadge_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            System.Text.RegularExpressions.Regex _regex = new Regex("[^0-9]+");
            e.Handled = _regex.IsMatch(e.Text);
        }

        #region Costs grid events
        private void rgvCosts_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var data = new tab_produzione_operatore_costo { SocietaID = _dataContext.CompanyID, OperatoreID = _dataContext.Data.ID };

            var items = rgvCosts.Items.Cast<tab_produzione_operatore_costo>();

            data.Periodo = DateTime.Now.Date;

            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;

            grid.CurrentColumn = grid.Columns[0];
        }

        private void rgvCosts_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.OldValues != null)
            {
                var item = e.Row.Item as tab_produzione_operatore_costo;

                if (item != null)
                {
                    var validated = _dataContext.ValidateCosts(item);

                    if (!string.IsNullOrWhiteSpace(validated))
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(validated); });
                        e.IsValid = false;
                    }
                    else
                    {
                        if ((rgvCosts.ItemsSource as ObservableCollection<tab_produzione_operatore_costo>)?.Where(w => w.Periodo == item.Periodo).Count() > 1)
                        {
                            Dispatcher.BeginInvoke(() => { ErrorHandler.Validation("Il periodo inserito è già in uso o non è valido"); });
                            e.IsValid = false;
                        }
                        else
                        {
                            e.IsValid = true;
                        }
                    }
                }
            }
            else
            {
                e.IsValid = true;
            }
        }

        private void rgvCosts_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvCosts.ScrollIntoView(e.Row.Item, rgvCosts.Columns[0]);
        }
        #endregion
    }
}
