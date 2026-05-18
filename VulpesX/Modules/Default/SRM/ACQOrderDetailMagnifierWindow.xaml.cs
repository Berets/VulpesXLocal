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
using VulpesX.Models.Models;
using VulpesX.Modules.Default.Commons;
using VulpesX.Shared;
using VulpesX.Shared.Controls.Callouts;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Commons;
using VulpesX.ViewModels.Modules.Default.SRM;

namespace VulpesX.Modules.Default.SRM
{
    /// <summary>
    /// Interaction logic for ACQOrderDetailMagnifierWindow.xaml
    /// </summary>
    public partial class ACQOrderDetailMagnifierWindow : FluentDefaultWindow
    {
        private ACQOrderDetailMagnifierWindowViewModel _dataContext;

        private RadCallout coPrices = new RadCallout()
        {
            Background = new SolidColorBrush(Colors.Black),
            Padding = new Thickness(0),
            Width = 650,
            Height = 130,
            StrokeThickness = 1,
            BorderBrush = new SolidColorBrush(Colors.MediumVioletRed),
            UseLayoutRounding = true,
            ArrowAnchorPoint = new Point(-0.15, 0.5),
            ArrowBasePoint1 = new Point(1, 0),
            ArrowBasePoint2 = new Point(1, 1)
        };
        private RadCallout coQuantities = new RadCallout()
        {
            Background = new SolidColorBrush(Colors.Black),
            Padding = new Thickness(0),
            Width = 650,
            Height = 350,
            StrokeThickness = 1,
            BorderBrush = new SolidColorBrush(Colors.MediumVioletRed),
            UseLayoutRounding = true,
            ArrowAnchorPoint = new Point(1.15, 0.5),
            ArrowBasePoint1 = new Point(0.7, 0.3),
            ArrowBasePoint2 = new Point(0.7, 0.7)
        };

        public ACQOrderDetailMagnifierWindow(ACQOrderDetailMagnifierWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
        }


        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();
            if (string.IsNullOrWhiteSpace(validated))
            {
                this.DialogResult = true;
            }
            else
            {
                ErrorHandler.Validation(validated);
            }
        }

        #region Add to price list
        private void cmdAddToPriceList_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.Row.price.HasValue && !string.IsNullOrEmpty(_dataContext.Row.product_id))
            {
                Mouse.OverrideCursor = Cursors.Wait;

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<SRM_LISFORWindowViewModel>();
                windowViewModel.Data = new SRM_LISFOR()
                {
                    companyID = _dataContext.CompanyID,
                    fromDate = _dataContext.Row.OrderDate,
                    toDate = _dataContext.Row.OrderDate.AddMonths(1),
                    fromQuantity = 1,
                    toQuantity = _dataContext.Row.quantity ?? 1,
                    supplierID = _dataContext.Row.SupplierID,
                    addedUserID = _dataContext.UserID,
                    price = _dataContext.Row.price.Value,
                    discount1 = _dataContext.Row.discount,
                    discountType1 = _dataContext.Row.discount_type,
                    productID = _dataContext.Row.product_id
                };
                windowViewModel.IsInsert = true;

                var wSRM_LISFOR = new SRM_LISFORWindow(windowViewModel);
                wSRM_LISFOR.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wSRM_LISFOR.ShowDialog() == true)
                    InfoHandler.Show("Listino aggiornato correttamente");
            }
            else
            {
                ErrorHandler.Validation("Controllare articolo e prezzo inseriti");
            }
        }
        #endregion

        #region Callouts
        private void cmdPrices_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (_dataContext.Row.Product != null)
            {
                var listSup = _dataContext.GetCurrenSupplier(_dataContext.Row.Product.ID, _dataContext.Row.SupplierID, _dataContext.Row.OrderDate, (_dataContext.Row.quantity ?? 0), _dataContext.Row.unit_id ?? string.Empty);
                var lastOrd = _dataContext.GetLastPriceDifferentSupplier(_dataContext.Row.Product.ID, _dataContext.Row.SupplierID, _dataContext.Row.id);
                var lastOrdSup = _dataContext.GetLastPriceSameSupplier(_dataContext.Row.Product.ID, _dataContext.Row.SupplierID, _dataContext.Row.id);
                var lastCost = _dataContext.GetLast(_dataContext.Row.Product.ID);

                coPrices.Content = new CalloutPricesModel()
                {
                    LastListEntityPrice = listSup,
                    LastOrderPrice = lastOrd,
                    LastOrderEntityPrice = lastOrdSup,
                    LastCostValuationPrice = new GenericPriceInfo()
                    {
                        Price = lastCost?.AverageCost ?? 0
                    }
                };
                coPrices.ContentTemplate = TryFindResource("SuppliersPricesCalloutContentTemplate") as DataTemplate;
                CalloutPopupService.AddPopupClosedHandler(sender as FrameworkElement, OnPricesPopupClosed);
                Mouse.OverrideCursor = null;
                CalloutPopupService.Show(
                    coPrices,
                    sender as FrameworkElement,
                    new CalloutPopupSettings()
                    {
                        Placement = System.Windows.Controls.Primitives.PlacementMode.Right
                    });
            }
        }

        private void OnPricesPopupClosed(object sender, RoutedEventArgs e)
        {
            if (!_dataContext.IsReadonly)
            {
                var model = coPrices.Content as CalloutPricesModel;

                if (model != null)
                {
                    if (model.SelectedPrice != null && model.SelectedPrice.Price > 0)
                    {
                        _dataContext.Row.price = model.SelectedPrice.Price;
                        _dataContext.Row.discount = model.SelectedPrice.Discount1;
                        _dataContext.Row.discount_type = model.SelectedPrice.DiscountType1;
                    }
                }
            }
        }

        private void cmdAvailability_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            coQuantities.Content = new CalloutQuantitiesModel()
            {
                Stocks = _dataContext.GetStockInfos(_dataContext.Row.Product?.ID ?? string.Empty)
            };
            coQuantities.ContentTemplate = TryFindResource("QuantitiesCalloutContentTemplate") as DataTemplate;
            Mouse.OverrideCursor = null;
            CalloutPopupService.Show(
                coQuantities,
                sender as FrameworkElement,
                new CalloutPopupSettings()
                {
                    Placement = System.Windows.Controls.Primitives.PlacementMode.Left
                });
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

        private void txtNote_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void TextBox_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TextMagnifierWindowViewModel>();
            windowViewModel.SourceText = _dataContext.Row.note ?? string.Empty;
            windowViewModel.MaxSize = 0;

            var wMagnifier = new TextMagnifierWindow(windowViewModel);
            wMagnifier.Owner = Window.GetWindow(this);
            wMagnifier.ShowDialog();
            _dataContext.Row.note = windowViewModel.SelectedText;
        }
    }
}
