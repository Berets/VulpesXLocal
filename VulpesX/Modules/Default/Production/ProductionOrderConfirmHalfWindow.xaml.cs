using DocumentFormat.OpenXml.Drawing.Charts;
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
using VulpesX.Models.Default.Partials;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Production;

namespace VulpesX.Modules.Default.Production
{
    /// <summary>
    /// Interaction logic for ProductionOrderConfirmHalfWindow.xaml
    /// </summary>
    public partial class ProductionOrderConfirmHalfWindow : FluentDefaultWindow
    {
        private ProductionOrderConfirmHalfWindowViewModel _dataContext;
        public ProductionOrderConfirmHalfWindow(ProductionOrderConfirmHalfWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            RefreshQuantities();
            Mouse.OverrideCursor = null;
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            cmdCancel.IsEnabled = false;
            cmdSave.IsEnabled = false;
            bool isChecked = true;
            foreach (var item in _dataContext.HalfworkedList ?? new())
            {
                if (item.QuantitaToDo + item.QuantitaImpegnata + item.QuantitaGiaImpegnata < item.Quantita)
                {
                    isChecked = false;
                    break;
                }
            }
            if (isChecked)
            {
                // check for changed quantities
                foreach (var item in _dataContext.HalfworkedList ?? new())
                {
                    var distinta = _dataContext.GetComponentsByProduct(item.ComponenteArticoloID ?? string.Empty, item.ComponenteRevisioneID ?? string.Empty);

                    foreach (var comp in _dataContext.MaterialList.Where(w => w.ArticoloID == item.ComponenteArticoloID && w.RevisioneID == item.ComponenteRevisioneID))
                    {
                        comp.Quantita = Math.Round(item.QuantitaToDo * (distinta?.Where(w => w.ComponenteArticoloID == comp.ComponenteArticoloID).FirstOrDefault()?.Quantita ?? 0), 6);
                    }

                    // update halfworked quantity
                    item.Quantita = item.QuantitaToDo;
                    item.QuantitaImpegnata = item.QuantitaImpegnata;

                    // engage halfworked if choosed
                    if ((item.Availabilities?.Any(any => any.QuantityToEngage > 0)) ?? false)
                    {
                        foreach (var lot in item.Availabilities.Where(w => w.QuantityToEngage > 0))
                        {
                            _dataContext.Insertstore_stocks_engage(item, lot);
                        }
                    }
                }
                this.DialogResult = true;
            }
            else
            {
                Mouse.OverrideCursor = null;
                cmdCancel.IsEnabled = true;
                cmdSave.IsEnabled = true;
                ErrorHandler.Validation("La quantita' da produrre deve coprire almeno la quantita' necessaria");
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        #endregion

        #region Grid methods
        private void RadGridView_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            RefreshQuantities();
        }
        private void RefreshQuantities()
        {
            foreach (var item in (_dataContext.HalfworkedList ?? new()).OrderBy(o => o.Posizione))
            {
                item.QuantitaImpegnata = (item.Availabilities ?? new()).Sum(sum => sum.QuantityToEngage);
                item.QuantitaToDo = (item.QuantitaOriginale ?? 0) - item.QuantitaImpegnata - item.QuantitaGiaImpegnata - item.QuantitaModificataDaImpegni;
                // search for children
                var distinta = _dataContext.GetComponentsByProduct(item.ComponenteArticoloID ?? string.Empty, item.ComponenteRevisioneID ?? string.Empty);

                foreach (var child in (_dataContext.HalfworkedList ?? new()).Where(w => w.ArticoloID == item.ComponenteArticoloID))
                {
                    var fir = distinta?.Where(w => w.ComponenteArticoloID == child.ComponenteArticoloID && w.ComponenteRevisioneID == child.ComponenteRevisioneID).FirstOrDefault();
                    
                    child.Quantita = (fir?.Quantita ?? 0) * item.QuantitaToDo;
                    child.QuantitaModificataDaImpegni = ((fir?.Quantita ?? 0) * (item.QuantitaOriginale ?? 0)) - (child.Quantita ?? 0);
                    child.QuantitaToDo = child?.QuantitaToDo ?? 0;
                }
            }
        }
        private void RadGridView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as StockInfo;

                if (item != null)
                {
                    if (item.QuantityToEngage <= item.QuantityAvailable)
                    {
                        e.IsValid = true;
                    }
                    else
                    {
                        item.QuantityToEngage = (decimal)e.OldValues["QuantityToEngage"];
                        e.IsValid = false;
                    }
                }
                else
                {
                    e.IsValid = false;
                }
            }
        }

        private void rgvHW_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType == Telerik.Windows.Controls.GridView.GridViewEditOperationType.Edit)
            {
                var item = e.Row.DataContext as pro_ordine_composizione;

                if (item != null)
                {
                    if (item.Quantita <= item.QuantitaToDo + item.QuantitaGiaImpegnata + item.QuantitaImpegnata)
                    {
                        if (item.QuantitaToDo != (decimal)e.OldValues["QuantitaToDo"])
                        {
                            // refresh children recursively
                            r_UpdateQuantities(_dataContext.HalfworkedList, item.ComponenteArticoloID, item.QuantitaToDo);
                        }
                        e.IsValid = true;
                    }
                    else
                    {
                        item.QuantitaToDo = (decimal)e.OldValues["QuantitaToDo"];

                        ErrorHandler.Validation("La quantità da produrre deve coprire interamente la quantità necessaria");

                        e.IsValid = false;
                    }
                }
                else
                {
                    e.IsValid = false;
                }
            }
        }
        private void r_UpdateQuantities(ObservableCollection<pro_ordine_composizione>? HWList, string? IDChanged, decimal NewQuantity)
        {
            if (HWList?.Any(any => any.ArticoloID == IDChanged) ?? false)
            {
                foreach (var toChange in HWList.Where(w => w.ArticoloID == IDChanged))
                {
                    var distint = _dataContext.GetComponentsByProduct(toChange.ArticoloID, toChange.RevisioneID);

                    toChange.QuantitaToDo = Math.Round(NewQuantity * (distint?.Where(w => w.ComponenteArticoloID == toChange.ComponenteArticoloID)?.FirstOrDefault()?.Quantita ?? 0), 6);
                    toChange.Quantita = toChange.QuantitaToDo;

                    r_UpdateQuantities(HWList, toChange.ComponenteArticoloID, toChange.Quantita.Value);
                }
            }
        }
        #endregion
    }
}
