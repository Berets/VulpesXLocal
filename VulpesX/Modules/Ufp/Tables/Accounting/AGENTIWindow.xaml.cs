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
using VulpesX.Models.Ufp;
using VulpesX.Modules.Default.Accounting;
using VulpesX.Modules.Default.Accounting.Invoicing;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;
using VulpesX.ViewModels.Modules.Default.Accounting.Invoicing;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Ufp.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for AGENTIWindow.xaml
    /// </summary>
    public partial class AGENTIWindow : FluentDefaultWindow
    {
        private AGENTIWindowViewModel _dataContext;
        public AGENTIWindow(AGENTIWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);
            this.DataContext = _dataContext;

            _dataContext.SelectedSupplier = _dataContext.Suppliers?.Where(w => w.abecod == _dataContext.Data.agefor).FirstOrDefault();

            if (!_dataContext.IsInsert)
            {
                _dataContext.ExceptionsCausal = _dataContext.GetAGEPROVPERs();
                _dataContext.ExceptionsArticle = _dataContext.GetAGENTI_SOTTOLIVELLOs();
                _dataContext.Enasarcos = _dataContext.GetTAB_AGENTI_ENASARCOs();
            }

            foreach (var item in _dataContext.ExceptionsCausal ?? new System.Collections.ObjectModel.ObservableCollection<AGEPROVPER>())
            {
                item.Customers = _dataContext.CustomersCache;
                item.Suppliers = _dataContext.Suppliers;
                item.OrderCausals = _dataContext.OrderCausalsCache;
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                DialogResult = _dataContext.Save();
            }
            else
            {
                ErrorHandler.Show(validated);
            }
        }

        #region Autocompletes
        private void acSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedSupplier != null && _dataContext.SelectedSupplier.abecod > 0)
            {
                _dataContext.Data.agefor = _dataContext.SelectedSupplier.abecod;
            }
        }

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

        #endregion

        #region Causals
        private void rgvCausals_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            var data = new AGEPROVPER { appcod = _dataContext.Data.agecod };
            data.Customers = _dataContext.CustomersCache;
            data.Suppliers = _dataContext.SuppliersCache;
            data.OrderCausals = _dataContext.OrderCausalsCache;

            data.IsInsert = true;

            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void rgvCausals_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != GridViewEditOperationType.None)
            {
                var item = e.Row.Item as AGEPROVPER;

                if (item != null)
                {
                    var validated = _dataContext.ValidateCausals(item);

                    if (validated != null)
                    {
                        ErrorHandler.Validation(validated);
                    }

                    e.IsValid = string.IsNullOrWhiteSpace(validated);
                }
            }
        }

        private void rgvCausals_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            var data = e.Row.Item as AGEPROVPER;

            if (data != null)
            {
                data.appclie = data.SelectedCustomer!.abecod;
                data.appfor = data.SelectedSupplier!.abecod;
                data.appcau = data.SelectedOrderCausal!.cauacq;

                data.IsInsert = false;
            }
        }

        private void rgvCausals_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvCausals.ScrollIntoView(e.Row.Item, rgvCausals.Columns[0]);
        }
        #endregion

        #region Articles
        private void rgvArticles_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            var data = new AGENTI_SOTTOLIVELLO { agecod = _dataContext.Data.agecod };

            data.Customers = _dataContext.CustomersCache;
            data.Articles = _dataContext.ArticlesCache;

            data.IsInsert = true;

            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void rgvArticles_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != GridViewEditOperationType.None)
            {
                var item = e.Row.Item as AGENTI_SOTTOLIVELLO;

                if (item != null)
                {
                    var validated = _dataContext.ValidateArticles(item);

                    if (validated != null)
                    {
                        ErrorHandler.Validation(validated);
                    }

                    e.IsValid = string.IsNullOrWhiteSpace(validated);
                }
            }
        }

        private void rgvArticles_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            var data = e.Row.Item as AGENTI_SOTTOLIVELLO;

            if (data != null)
            {
                data.agecli = data.SelectedCustomer!.abecod;
                data.ageart = data.SelectedArticle!.ID;
                data.ageclid = data.SelectedCustomer!.abers1;
                data.ageartd = data.SelectedArticle!.artdise;

                data.IsInsert = false;
            }
        }

        private void rgvArticles_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvArticles.ScrollIntoView(e.Row.Item, rgvArticles.Columns[0]);
        }
        #endregion

        private async void tabMain_SelectionChanged(object sender, RadSelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var tab = e.AddedItems[0] as RadTabItem;

                if (tab != null && tab.Name == "tabArticle")
                {
                    if (_dataContext.ArticlesCache == null)
                    {
                        await _dataContext.LoadArticles();
                    }

                    foreach (var item in _dataContext.ExceptionsArticle ?? new System.Collections.ObjectModel.ObservableCollection<AGENTI_SOTTOLIVELLO>())
                    {
                        item.Customers = _dataContext.CustomersCache;
                        item.Articles = _dataContext.ArticlesCache;
                    }
                }
            }
        }

        #region Enasarco
        private void rgvEnasarco_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            var data = new TAB_AGENTI_ENASARCO { agesoc = _dataContext.CompanyID, agecod = _dataContext.Data.agecod };

            data.IsInsert = true;

            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void rgvEnasarco_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvEnasarco.ScrollIntoView(e.Row.Item, rgvEnasarco.Columns[0]);
        }

        private void rgvEnasarco_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != GridViewEditOperationType.None)
            {
                var item = e.Row.Item as TAB_AGENTI_ENASARCO;

                if (item != null)
                {
                    var validated = _dataContext.ValidateEnasarco(item);

                    if (validated != null)
                    {
                        ErrorHandler.Validation(validated);
                    }

                    e.IsValid = string.IsNullOrWhiteSpace(validated);
                }
            }
        }

        private void rgvEnasarco_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {

        }

        private void rgvEnasarco_RowIsExpandedChanging(object sender, RowCancelEventArgs e)
        {
            var row = e.Row as GridViewRow;

            if (row != null)
            {
                if (row.IsExpanded)
                {
                    var head = row.DataContext as TAB_AGENTI_ENASARCO;

                    if (head != null && head.SupplierID.HasValue)
                    {
                        var cnst = _dataContext.GetACC_EINVOICE_HEADs(head.enaann, head.SupplierID.Value);

                        if (cnst != null)
                        {
                            head.Items = cnst;
                        }
                    }
                }
            }
        }

        private void cmdViewAccounting_Click(object sender, RoutedEventArgs e)
        {
            var selected = (sender as Button)!.DataContext as ACC_EINVOICE_HEADS;

            if (selected != null && selected.fattannoreg.HasValue && selected.fattnumreg.HasValue)
            {
                var pnHead = _dataContext.GetPNTESTATA(selected.fattannoreg!.Value, selected.fattnumreg!.Value);

                if (pnHead != null)
                {
                    var causals = _dataContext.GetCAUCONT("*");
                    var codes = !string.IsNullOrWhiteSpace(pnHead.N1FLCF) ? _dataContext.GetABE(pnHead.N1FLCF) : null;

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNRIGHEWindowViewModel>();
                    windowViewModel.Head = pnHead;
                    windowViewModel.HeadSelectedCausal = causals?.Where(w => w.caucod == pnHead.pncaus).FirstOrDefault();
                    windowViewModel.IsInsert = false;

                    var wPNRIGHE = new PNRIGHEWindow(windowViewModel);
                    wPNRIGHE.Owner = Window.GetWindow(this);
                    wPNRIGHE.ShowDialog();
                }
            }
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as ACC_EINVOICE_HEADS;
            if (item != null)
            {
                item = _dataContext.GetFull(item.id);

                if (item != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_EINVOICE_HEADSWindowViewModel>();
                    windowViewModel.Data = item;
                    windowViewModel.IsInsert = false;

                    var wDetails = new ACC_EINVOICE_HEADSWindow(windowViewModel);
                    wDetails.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    wDetails.ShowDialog();
                }
            }
        }
        #endregion
    }
}
