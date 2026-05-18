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
using VulpesX.Models.Default.Partials;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Production;

namespace VulpesX.Modules.Default.Production
{
    /// <summary>
    /// Interaction logic for ProductionOrderConfirmRawWindow.xaml
    /// </summary>
    public partial class ProductionOrderConfirmRawWindow : FluentDefaultWindow
    {
        private ProductionOrderConfirmRawWindowViewModel _dataContext;
        public ProductionOrderConfirmRawWindow(ProductionOrderConfirmRawWindowViewModel dataContext)
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
            foreach (var item in _dataContext.RawsList)
            {
                foreach (var eng in item.Availabilities ?? new System.Collections.ObjectModel.ObservableCollection<Models.Default.Partials.StockInfo>())
                {
                    if (eng.QuantityToEngage > eng.QuantityAvailable || eng.QuantityToEngage + eng.QuantityEngagedForOrder > item.Quantita)
                    {
                        isChecked = false;
                        break;
                    }
                }
                if (!isChecked)
                    break;
            }
            if (isChecked)
            {
                this.DialogResult = true;
            }
            else
            {
                Mouse.OverrideCursor = null;
                cmdCancel.IsEnabled = true;
                cmdSave.IsEnabled = true;

                ErrorHandler.Validation("Le quantita' da impegnare non possono essere superiori alle quantita' disponibili per i vari lotti e non possono essere superiori alle quantita' necessarie. Verificare le materie prime da impegnare");
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        #endregion

        private void RadGridView_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            RefreshQuantities();
        }

        private void RefreshQuantities()
        {
            foreach (var item in _dataContext.RawsList)
            {
                item.QuantitaImpegnata = (item.Availabilities ?? new()).Sum(sum => sum.QuantityToEngage);
                item.QuantitaToDo = (item.QuantitaOriginale ?? 0) - item.QuantitaImpegnata;
            }
        }

        private void RadGridView_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var parent = (sender as RadGridView)?.ParentRow.DataContext as pro_ordine_composizione;

                var item = e.Row.Item as StockInfo;

                if (item != null && parent != null)
                {
                    if (item.QuantityToEngage <= item.QuantityAvailable && item.QuantityToEngage + (item.QuantityEngagedForOrder ?? 0) <= _dataContext.RawsList.Where(w => w.ArticoloID == parent.ArticoloID && w.ComponenteArticoloID == item.ProductID).FirstOrDefault()?.Quantita)
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
    }
}
