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
using System.Windows.Shapes;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.EnergyMonitor;

namespace VulpesX.Modules.Default.Tables.EnergyMonitor
{
    /// <summary>
    /// Interaction logic for DeviceWindow.xaml
    /// </summary>
    public partial class DeviceWindow : FluentDefaultWindow
    {
        private DeviceWindowViewModel _dataContext;
        public DeviceWindow(DeviceWindowViewModel dataContext)
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

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save())
                {
                    this.DialogResult = true;
                }
            }
            else
            {
                ErrorHandler.Validation(validated);
            }
        }

        #region Costs grid events
        private void rgvCosts_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var data = new EM_DEVICE_PERIOD { SocietaID = _dataContext.CompanyID, DeviceID = _dataContext.Data.ID, Mese = DateTime.Now.Date };

            var items = rgvCosts.Items.Cast<EM_DEVICE_PERIOD>();

            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;

            grid.CurrentColumn = grid.Columns[0];
        }

        private void rgvCosts_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.OldValues != null)
            {
                var item = e.Row.Item as EM_DEVICE_PERIOD;

                if (item != null)
                {
                    var validated = "";
                    if (!string.IsNullOrWhiteSpace(validated))
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(validated); });
                        e.IsValid = false;
                    }
                    else
                    {
                        if ((rgvCosts.ItemsSource as ObservableCollection<EM_DEVICE_PERIOD>)?.Where(w => w.Mese == item.Mese).Count() > 1)
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
