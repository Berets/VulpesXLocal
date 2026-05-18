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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Production;

namespace VulpesX.Modules.Default.Production
{
    /// <summary>
    /// Interaction logic for ProductionOrderWindow.xaml
    /// </summary>
    public partial class ProductionOrderWindow : FluentDefaultWindow
    {
        private ProductionOrderWindowViewModel _dataContext;
        public ProductionOrderWindow(ProductionOrderWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedCustomer = _dataContext.Customers?.Where(w => w.abecod == _dataContext.Data.ClienteID).FirstOrDefault();
                _dataContext.SelectedProduct = _dataContext.Products?.Where(w => w.ID == _dataContext.Data.ArticoloID).FirstOrDefault();

                if (_dataContext.SelectedProduct != null)
                {
                    _dataContext.Revisions = _dataContext.GetRevisioni(_dataContext.SelectedProduct.ID);
                    _dataContext.SelectedRevision = _dataContext.Revisions?.Where(w => w.ID == _dataContext.Data.ArticoloID && w.RevisioneID == _dataContext.Data.RevisioneID).FirstOrDefault();
                }
            }
        }

        #region Various events

        private void acCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCustomer != null)
            {
                if (_dataContext.Data.ClienteID != _dataContext.SelectedCustomer.abecod)
                {
                    _dataContext.Data.ClienteID = _dataContext.SelectedCustomer.abecod;
                    _dataContext.Data.ClienteDescrizione = _dataContext.SelectedCustomer.abers1;
                }
            }
        }

        private void acProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedProduct != null)
            {
                if (_dataContext.Data.ArticoloID != _dataContext.SelectedProduct.ID)
                {
                    _dataContext.Data.ArticoloID = _dataContext.SelectedProduct.ID;
                    _dataContext.Data.ArticoloDescrizione = _dataContext.SelectedProduct.Descrizione;

                    _dataContext.SelectedRevision = null;
                    _dataContext.Revisions = _dataContext.GetRevisioni(_dataContext.SelectedProduct.ID);

                    if (_dataContext.Revisions?.Any() ?? false)
                        _dataContext.SelectedRevision = _dataContext.Revisions.FirstOrDefault();
                }
            }
        }

        private void cmbRevision_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedRevision != null)
            {
                if (_dataContext.Data.RevisioneID != _dataContext.SelectedRevision.RevisioneID)
                {
                    _dataContext.Data.RevisioneID = _dataContext.SelectedRevision.RevisioneID;
                }
            }
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

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save())
                {
                    Mouse.OverrideCursor = null;
                    this.DialogResult = true;
                }
            }
            else
            { Mouse.OverrideCursor = null; ErrorHandler.Validation(validated); }
        }
        #endregion
    }
}
