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
using Telerik.Windows.SyntaxEditor.Core.History;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Controls.Helpers;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Assets;

namespace VulpesX.Modules.Default.Assets
{
    /// <summary>
    /// Interaction logic for ASSETCO00FWindow.xaml
    /// </summary>
    public partial class ASSETCO00FWindow : FluentDefaultWindow
    {
        private ASSETCO00FWindowViewModel _dataContext;
        public ASSETCO00FWindow(ASSETCO00FWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            foreach (var cont in _dataContext.Head.Contacts ?? new ObservableCollection<ASSETCO00F>())
            {
                cont.ContactRoles = _dataContext.ContactRoles;
                foreach (var det in cont.ContactDetails ?? new ObservableCollection<ASSETCODET00F>())
                {
                    det.ContactTypes = _dataContext.ContactTypes;
                }
            }

            Loaded += (sender, e) =>
            {
                this.Title = $"Contatti dell'asset {_dataContext.Head.FullDescriptionSearchable}";
                if (!_dataContext.IsReadonly)
                {
                    this.Title += " - [sola lettura]";
                    GridView.CanUserInsertRows = false;
                }
                MoveFocus(new TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
            };
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            if (_dataContext.UpdateAll())
            {
                Mouse.OverrideCursor = null;
                this.DialogResult = true;
            }
            else
            { Mouse.OverrideCursor = null; ErrorHandler.Validation("Errore imprevisto durante il salvataggio dei dati"); }
        }

        #endregion

        #region Grid events
        private void GridView_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            if (GridView.SelectedItem != null)
            {
                rdpGridDetails.Source = (GridView.SelectedItem as ASSETCO00F)?.ContactDetails;
                GridViewDetails.CanUserInsertRows = true;
            }
            else
            {
                rdpGridDetails.Source = null;
                GridViewDetails.CanUserInsertRows = false;
            }
        }
        private void GridView_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var items = GridView.Items.Cast<ASSETCO00F>();

            var data = new ASSETCO00F
            {
                company_id = _dataContext.CompanyID,
                id = _dataContext.Head.id,
                sequence = items.Any() ? items.Max(max => max.sequence) + 1 : 1,
                description = string.Empty,
                addedUserID = _dataContext.UserID,
                ContactRoles = _dataContext.ContactRoles,
                ContactDetails = new ObservableCollection<ASSETCODET00F>(),
            };

            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void GridView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as ASSETCO00F;
                if (item != null)
                {
                    var validated = _dataContext.Validate(item);
                    if (!string.IsNullOrEmpty(validated))
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(validated); });
                        GridView.CancelEdit();
                    }
                    e.IsValid = string.IsNullOrWhiteSpace(validated);
                }
            }
            else
            {
                e.IsValid = true;
            }
            e.Handled = true;
        }

        private void GridView_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            GridView.ScrollIntoView(e.Row.Item, GridView.Columns[1]);
        }
        #endregion

        #region Grid details events
        private void GridViewDetails_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            if (GridView.SelectedItem != null)
            {
                var items = GridViewDetails.Items.Cast<ASSETCODET00F>();
                var sequence = (GridView.SelectedItem as ASSETCO00F)?.sequence;

                if (sequence != null)
                {
                    var data = new ASSETCODET00F
                    {
                        company_id = _dataContext.CompanyID,
                        contact = string.Empty,
                        id = _dataContext.Head.id,
                        sequence = sequence!.Value,
                        progressive = items.Any() ? items.Max(max => max.progressive) + 1 : 1,
                        addedUserID = _dataContext.UserID,
                        ContactTypes = _dataContext.ContactTypes,
                    };

                    e.NewObject = data;
                    var grid = e.OwnerGridViewItemsControl;
                    grid.CurrentColumn = grid.Columns[0];
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void GridViewDetails_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as ASSETCODET00F;

                if (item != null)
                {
                    var validated = _dataContext.Validate(item);
                    if (validated != null)
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(validated); });
                        GridView.CancelEdit();
                    }
                    e.IsValid = string.IsNullOrWhiteSpace(validated);
                }
            }
            else
            {
                e.IsValid = true;
            }
            e.Handled = true;
        }

        private void GridViewDetails_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            GridView.ScrollIntoView(e.Row.Item, GridView.Columns[1]);
        }
        #endregion

        #region Sync details row with grid validating
        private void grdContactNote_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var inputElement = (FrameworkElement)e.Source;
                var bindingExpression = BindingHelper.GetInputElementBindingExpression(inputElement);
                if (bindingExpression != null)
                {
                    GridView.BeginEdit();
                    bindingExpression.UpdateSource();
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var row = inputElement.ParentOfType<GridViewRow>();
                        GridView.CommitEdit();
                    }), (DispatcherPriority)5);
                }
            }
        }
        #endregion

        #region Sync details row with grid details validating
        private void grdContactDetailNote_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var inputElement = (FrameworkElement)e.Source;
                var bindingExpression = BindingHelper.GetInputElementBindingExpression(inputElement);
                if (bindingExpression != null)
                {
                    GridView.BeginEdit();
                    bindingExpression.UpdateSource();
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var row = inputElement.ParentOfType<GridViewRow>();
                        GridView.CommitEdit();
                    }), (DispatcherPriority)5);
                }
            }
        }
        #endregion
    }
}
