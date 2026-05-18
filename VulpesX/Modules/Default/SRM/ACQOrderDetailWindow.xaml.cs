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
using System.Windows.Shapes;
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.SRM;

namespace VulpesX.Modules.Default.SRM
{
    /// <summary>
    /// Interaction logic for ACQOrderDetailWindow.xaml
    /// </summary>
    public partial class ACQOrderDetailWindow : FluentDefaultWindow
    {
        private ACQOrderDetailWindowViewModel _dataContext;
        public ACQOrderDetailWindow(ACQOrderDetailWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            #region Check is readonly
            bool isenabled = true;
            if (_dataContext.Head.commercial_signed.HasValue && !_dataContext.Head.management_signed.HasValue)
            {
                if (UserContext.Instance!.ACCESS!.Roles?.canPOSignCommercial ?? false)
                    isenabled = true;
                else
                    isenabled = false;
            }
            else
            {
                if (_dataContext.Head.commercial_signed.HasValue && _dataContext.Head.management_signed.HasValue)
                {
                    if (UserContext.Instance!.ACCESS!.Roles?.canPOSignManagement ?? false)
                        isenabled = true;
                    else
                        isenabled = false;
                }
            }
            _dataContext.IsReadonly = _dataContext.Head.canceled.HasValue || _dataContext.Head.closed.HasValue ? true : !isenabled;
            _dataContext.HasMergedSigns = (UserContext.Instance!.ACCESS!.Roles?.canPOSignCommercial ?? false) && (UserContext.Instance!.ACCESS!.Roles?.canPOSignManagement ?? false);
            #endregion

            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli dell'ordine di acquisto {_dataContext.Head.id}";
                if (_dataContext.IsReadonly)
                    Title += " - [sola lettura]";
            };
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            rgvRows.CommitEdit();

            string? validated = _dataContext.ValidateModel();

            if (string.IsNullOrEmpty(validated))
            {
                if (_dataContext.Save())
                    this.DialogResult = true;
            }
            else
            {
                ErrorHandler.Validation(validated);
            }
        }
        #endregion

        #region Rows grid
        private void rgvRows_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var items = rgvRows.Items.Cast<acq_orders_rows>();

            var data = new acq_orders_rows
            {
                company_id = _dataContext.CompanyID,
                id = _dataContext.Head.id,
                sequence = items.Any() ? items.Max(max => max.sequence) + 1 : 1,
                price_type = "U",
                Products = _dataContext.Products,
                Stores = _dataContext.Stores,
                Rates = _dataContext.Rates,
                UMsCache = _dataContext.UMsCache,
                ProductTypes = _dataContext.ProductTypes,
                SupplierID = _dataContext.Head.supplier_id,
                OrderDate = _dataContext.Head.order_date ?? DateTime.Now,
                tax_code = string.Empty,
                tax_rate = string.Empty,
            };
            data.ProductOrQuantityValueChanged += _dataContext.OnProductOrQuantityChanged;

            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[1];
        }

        private void rgvRows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.DataContext as acq_orders_rows;
                if (item != null)
                {
                    var validated = _dataContext.Validate(item);
                    if (validated != null)
                        ErrorHandler.Validation(validated);
                    e.IsValid = string.IsNullOrWhiteSpace(validated);
                }
                else
                {
                    e.IsValid = false;
                }
            }
            else
            {
                e.IsValid = true;
            }
        }

        private void rgvRows_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvRows.ScrollIntoView(e.Row.Item, rgvRows.Columns[0]);
        }

        private void rgvRows_Deleted(object sender, GridViewDeletedEventArgs e)
        {
            int sequence = 1;
            foreach (var row in (_dataContext.Rows ?? new ObservableCollection<acq_orders_rows>()).OrderBy(o => o.sequence))
            {
                row.sequence = sequence++;
            }
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
        #endregion



        #region Magnifier
        private void rgMagnifier_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as acq_orders_rows;

            if (item != null)
            {
                var clonedItem = new acq_orders_rows()
                {
                    company_id = item.company_id,
                    delivery_requested = item.delivery_requested,
                    product_id = item.product_id,
                    discount = item.discount,
                    discount_type = item.discount_type,
                    id = item.id,
                    is_closed = item.is_closed,
                    note = item.note,
                    price = item.price,
                    price_second_um = item.price_second_um,
                    price_type = item.price_type,
                    quantity = item.quantity,
                    quantity_received = item.quantity_received,
                    sequence = item.sequence,
                    store_id = item.store_id,
                    tax_code = item.tax_code,
                    tax_rate = item.tax_rate,
                    unit_id = item.unit_id,
                    SupplierID = _dataContext.Head.supplier_id,
                    OrderDate = _dataContext.Head.order_date!.Value
                };
                clonedItem.UMsCache = item.UMsCache;
                clonedItem.ProductTypes = item.ProductTypes;
                clonedItem.Products = item.Products;
                clonedItem.Rates = item.Rates;
                clonedItem.Stores = item.Stores;
                clonedItem.Product = item.Product;
                clonedItem.ProductOrQuantityValueChanged += _dataContext.OnProductOrQuantityChanged;

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACQOrderDetailMagnifierWindowViewModel>();
                windowViewModel.Title = "Dettagli della riga d'ordine d'acquisto";
                windowViewModel.Row =clonedItem;
                windowViewModel.IsReadonly = _dataContext.IsReadonly;

                var wMagnifier = new ACQOrderDetailMagnifierWindow(windowViewModel);
                wMagnifier.Owner = Window.GetWindow(this);
                if (wMagnifier.ShowDialog() == true)
                {
                    // update row data
                    item.Product = windowViewModel.Row.Product;
                    item.Store = windowViewModel.Row.Store;
                    item.Rate = windowViewModel.Row.Rate;
                    item.QuantityValue = windowViewModel.Row.QuantityValue;
                    item.delivery_requested = windowViewModel.Row.delivery_requested;
                    item.discount = windowViewModel.Row.discount;
                    item.discount_type = windowViewModel.Row.discount_type;
                    item.is_closed = windowViewModel.Row.is_closed;
                    item.note = windowViewModel.Row.note;
                    item.price = windowViewModel.Row.price;
                    item.price_second_um = windowViewModel.Row.price_second_um;
                    item.price_type = windowViewModel.Row.price_type;
                    item.quantity_received = windowViewModel.Row.quantity_received;
                    item.unit_id = windowViewModel.Row.unit_id;
                }
            }
        }
        #endregion
    }
}
