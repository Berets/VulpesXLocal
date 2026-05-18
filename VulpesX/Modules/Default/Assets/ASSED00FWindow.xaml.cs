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
using VulpesX.Shared.Controls.Helpers;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Assets;

namespace VulpesX.Modules.Default.Assets
{
    /// <summary>
    /// Interaction logic for ASSED00FWindow.xaml
    /// </summary>
    public partial class ASSED00FWindow : FluentDefaultWindow
    {
        private ASSED00FWindowViewModel _dataContext;
        public ASSED00FWindow(ASSED00FWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
            //var customizations = new AZIENDAService().Get(ctx.CurrentCompanyID);
            //ShowStep = customizations.azaststep ? Visibility.Visible : Visibility.Collapsed

            _dataContext.LoadProducts();
            _dataContext.LoadRows();

            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli dell'asset {_dataContext.Head.id}";
                MoveFocus(new TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
            };
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            rgvRows.CommitEdit();

            string? validated = _dataContext.ValidateAll();
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
            var items = rgvRows.Items.Cast<ASSED00F>();

            var data = new ASSED00F
            {
                company_id = _dataContext.CompanyID,
                id = string.Empty,
                position = items.Any() ? items.Max(max => max.position) + 1 : 1,
                Products = _dataContext.Products
            };
            data.ProductChanged += ProductChanged;

            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[1];
        }
        private void rgvRows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as ASSED00F;
                if (item != null)
                {
                    var validated = _dataContext.Validate(item);
                    if (validated != null)
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(validated); });
                    }
                    e.IsValid = string.IsNullOrWhiteSpace(validated);
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
            int position = 1;
            foreach (var row in (_dataContext.Rows ?? new ObservableCollection<ASSED00F>()).OrderBy(o => o.position))
            {
                row.position = position++;
            }
        }
        private void rgvRows_RowDetailsVisibilityChanged(object sender, Telerik.Windows.Controls.GridView.GridViewRowDetailsEventArgs e)
        {
            if (e.DetailsElement != null && e.DetailsElement.DataContext != null)
            {
                rgvRows.SelectedItem = e.DetailsElement.DataContext as ASSED00F;
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

        #region Sync details row with grid validating
        private void grdDetails_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var inputElement = (FrameworkElement)e.Source;
                var bindingExpression = BindingHelper.GetInputElementBindingExpression(inputElement);
                if (bindingExpression != null)
                {
                    rgvRows.BeginEdit();
                    bindingExpression.UpdateSource();
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var row = inputElement.ParentOfType<GridViewRow>();
                        rgvRows.CommitEdit();
                    }), (DispatcherPriority)5);
                }
            }
        }
        #endregion

        public void ProductChanged(object? sender, EventArgs e)
        {
            var item = sender as ASSED00F;

            if (item != null)
            {
                item.QuantityAvailable = !string.IsNullOrWhiteSpace(item.product_id) ? _dataContext.GetQuantityStock(item.product_id) : null;
            }
        }
    }
}
