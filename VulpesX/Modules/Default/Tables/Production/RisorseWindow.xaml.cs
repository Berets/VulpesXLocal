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
using System.Windows.Threading;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.Production;

namespace VulpesX.Modules.Default.Tables.Production
{
    /// <summary>
    /// Interaction logic for RisorseWindow.xaml
    /// </summary>
    public partial class RisorseWindow : FluentDefaultWindow
    {
        private RisorseWindowViewModel _dataContext;
        public RisorseWindow(RisorseWindowViewModel dataContext)
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
            { ErrorHandler.Validation(validated); }
        }

        #region Sources grid events
        private void rgvSources_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var data = new tab_produzione_risorsa_sorgenti { SocietaID = _dataContext.CompanyID, RisorsaID = _dataContext.Data.ID, ID = string.Empty, Descrizione = string.Empty };

            var items = rgvSources.Items.Cast<tab_produzione_risorsa_sorgenti>();
            
            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;

            grid.CurrentColumn = grid.Columns[0];
        }

        private void rgvSources_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.OldValues != null)
            {
                var item = e.Row.Item as tab_produzione_risorsa_sorgenti;

                if (item != null)
                {
                    var validated = _dataContext.ValidateSource(item);
                    if (!string.IsNullOrWhiteSpace(validated))
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(validated); });
                        e.IsValid = false;
                    }
                    else
                    {
                        if ((rgvSources.ItemsSource as ObservableCollection<tab_produzione_risorsa_sorgenti>)?.Where(w => w.ID == item.ID).Count() > 1)
                        {
                            Dispatcher.BeginInvoke(() => { ErrorHandler.Validation("Il codice inserito è già in uso o non è valido"); });
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

        private void rgvSources_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvSources.ScrollIntoView(e.Row.Item, rgvSources.Columns[0]);
        }
        #endregion

        #region Costs grid events
        private void rgvCosts_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var data = new tab_produzione_risorsa_costo { SocietaID = _dataContext.CompanyID, RisorsaID = _dataContext.Data.ID };

            var items = rgvCosts.Items.Cast<tab_produzione_risorsa_costo>();
            
            data.Periodo = DateTime.Now.Date;

            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;

            grid.CurrentColumn = grid.Columns[0];
        }

        private void rgvCosts_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.OldValues != null)
            {
                var item = e.Row.Item as tab_produzione_risorsa_costo;

                if (item != null)
                {
                    var validated = _dataContext.ValidateCost(item);

                    if (!string.IsNullOrWhiteSpace(validated))
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(validated); });
                        e.IsValid = false;
                    }
                    else
                    {
                        if ((rgvCosts.ItemsSource as ObservableCollection<tab_produzione_risorsa_costo>)?.Where(w => w.Periodo == item.Periodo).Count() > 1)
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
