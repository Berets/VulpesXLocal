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
using VulpesX.Modules.Default.Commons;
using VulpesX.Shared;
using VulpesX.Shared.Controls.Callouts;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Commons;
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for ORDID00FMagnifierWindow.xaml
    /// </summary>
    public partial class ORDID00FMagnifierWindow : FluentDefaultWindow
    {
        private ORDID00FMagnifierWindowViewModel _dataContext;


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
        public ORDID00FMagnifierWindow(ORDID00FMagnifierWindowViewModel dataContext)
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

        #region Callouts
        private void cmdPrices_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (_dataContext.Row.SelectedProduct != null)
            {
                var listGen = _dataContext.GetCurrentGeneric(_dataContext.Row.SelectedProduct.ID, _dataContext.Row.OrderDate, _dataContext.Row.ODQTAV, _dataContext.Row.odunit ?? string.Empty);
                var listCus = _dataContext.GetCurrentCustomer(_dataContext.Row.SelectedProduct.ID, _dataContext.Row.CustomerID, _dataContext.Row.RecipientID, _dataContext.Row.OrderDate, _dataContext.Row.ODQTAV, _dataContext.Row.odunit ?? string.Empty);
                var lastOrd = _dataContext.GetLastPriceDifferentCustomer(_dataContext.Row.SelectedProduct.ID, _dataContext.Row.CustomerID, 0, 0);
                var lastOrdCli = _dataContext.GetLastPriceSameCustomer(_dataContext.Row.SelectedProduct.ID, _dataContext.Row.CustomerID, 0, 0);

                // TODO valorizzazione distinta base costi
                coPrices.Content = new CalloutPricesModel()
                {
                    LastListGeneralPrice = listGen,
                    LastListEntityPrice = listCus,
                    LastOrderPrice = lastOrd,
                    LastOrderEntityPrice = lastOrdCli,
                    LastCostValuationPrice = null
                };
                coPrices.ContentTemplate = TryFindResource("PricesCalloutContentTemplate") as DataTemplate;
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
                        _dataContext.Row.ODPREZ = model.SelectedPrice.Price;
                        _dataContext.Row.ODSCO1 = model.SelectedPrice.Discount1;
                        _dataContext.Row.ODSCO2 = model.SelectedPrice.Discount2;
                        _dataContext.Row.ODSCO3 = model.SelectedPrice.Discount3;
                        _dataContext.Row.ODTSC1 = model.SelectedPrice.DiscountType1;
                        _dataContext.Row.ODTSC2 = model.SelectedPrice.DiscountType2;
                        _dataContext.Row.ODTSC3 = model.SelectedPrice.DiscountType3;
                        _dataContext.Row.ODMAGG = model.SelectedPrice.Surcharge;
                        _dataContext.Row.ODTMAG = model.SelectedPrice.SurchargeType;
                    }
                }
            }
        }

        private void cmdAvailability_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            coQuantities.Content = new CalloutQuantitiesModel()
            {
                Stocks = _dataContext.GetStockInfos(_dataContext.Row.SelectedProduct?.ID ?? string.Empty)
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

        #region Add to price list
        private void cmdAddToPriceList_Click(object sender, RoutedEventArgs e)
        {
            bool useRecipient = !(_dataContext.Row.RecipientID == 0);
            if (useRecipient)
            {
                useRecipient = ConfirmHandler.Confirm("L'ordine attuale presenta una destinazione specifica, creare la nuova voce per questa destinazione (rispondere [SI]) oppure per tutte (rispondere [NO]) ?");
            }
            Mouse.OverrideCursor = Cursors.Wait;

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<LISCLIWindowViewModel>();
            windowViewModel.Data = new CRM_LISCLI()
            {
                companyID = _dataContext.CompanyID,
                fromDate = _dataContext.Row.OrderDate,
                toDate = _dataContext.Row.OrderDate.AddMonths(1),
                fromQuantity = 1,
                toQuantity = _dataContext.Row.ODQTAV,
                customerID = _dataContext.Row.CustomerID,
                recipientID = useRecipient ? _dataContext.Row.RecipientID : null,
                addedUserID = _dataContext.UserID,
                price = _dataContext.Row.ODPREZ,
                discount1 = _dataContext.Row.ODSCO1,
                discountType1 = _dataContext.Row.ODTSC1,
                discount2 = _dataContext.Row.ODSCO2,
                discountType2 = _dataContext.Row.ODTSC2,
                discount3 = _dataContext.Row.ODSCO3,
                discountType3 = _dataContext.Row.ODTSC3,
                surcharge = _dataContext.Row.ODMAGG,
                surchargeType = _dataContext.Row.ODTMAG,
                productID = _dataContext.Row.SelectedProduct?.ID ?? string.Empty
            };
            windowViewModel.Recipients = useRecipient ? _dataContext.GetDESTINATARIs(_dataContext.Row.CustomerID) : null;
            windowViewModel.IsInsert = true;

            var wCRM_LISCLI = new LISCLIWindow(windowViewModel);
            wCRM_LISCLI.Owner = Window.GetWindow(this);
            Mouse.OverrideCursor = null;
            if (wCRM_LISCLI.ShowDialog() == true)
                InfoHandler.Show("Listino aggiornato correttamente");
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

        #region Note magnifier
        private void txtNote_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void txtNote_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TextMagnifierWindowViewModel>();
            windowViewModel.SourceText = _dataContext.Row.ODNOTE ?? string.Empty;
            windowViewModel.MaxSize = 0;

            var wMagnifier = new TextMagnifierWindow(windowViewModel);
            wMagnifier.Owner = Window.GetWindow(this);
            wMagnifier.ShowDialog();

            _dataContext.Row.ODNOTE = windowViewModel.SelectedText;
        }
        #endregion
    }
}
