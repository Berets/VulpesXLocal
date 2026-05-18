using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;
using VulpesX.ViewModels.Modules.Ufp.Tables.CRM.AF;

namespace VulpesX.Modules.Ufp.Tables.CRM.AF
{
    /// <summary>
    /// Interaction logic for ANAFAT_CONSTWindow.xaml
    /// </summary>
    public partial class ANAFAT_CONSTWindow : FluentDefaultWindow
    {
        private ANAFAT_CONSTWindowViewModel _dataContext;
        public ANAFAT_CONSTWindow(ANAFAT_CONSTWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            rdtPeriod.Culture = new System.Globalization.CultureInfo("it-IT");
            rdtPeriod.Culture.DateTimeFormat.ShortDatePattern = "MMMM yyyy";

            cmdSave.IsEnabled = !_dataContext.Data.IsReadOnly;
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.Data.afdata = new DateTime(_dataContext.Data.afdata.Year, _dataContext.Data.afdata.Month, DateTime.DaysInMonth(_dataContext.Data.afdata.Year, _dataContext.Data.afdata.Month));

            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                DialogResult = _dataContext.Save();
            }
            else
            {
                ErrorHandler.Validation(validated);
            }
        }

        private void GridQuantity_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            var data = new ANAFAT_PIECES { afpsoc = _dataContext.CompanyID, afppiecesfrom = 0, afpproductiontype = "A" };

            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void GridQuantity_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var data = e.Row.Item as ANAFAT_PIECES;

            if (data != null)
            {
                var sbError = new StringBuilder();

                if (data.afppiecesfrom <= 0)
                    sbError.AppendLine("Da pezzi deve essere maggiore di 0");
                if (data.afppiecesfrom > (data.afppiecesto ?? 0) && (data.afppiecesto ?? 0) > 0)
                    sbError.AppendLine("Da pezzi deve essere >= di A pezzi");

                if (!data.afppercentage.HasValue)
                    sbError.AppendLine("% di incremento o decremento deve avere un valore");

                bool exist = false;
                if (e.EditOperationType == GridViewEditOperationType.Insert)
                {
                    exist = _dataContext.Data.Pieces.Where(o => o.afppiecesfrom == data.afppiecesfrom || o.afppiecesto == data.afppiecesfrom || o.afppiecesfrom == data.afppiecesto || o.afppiecesto == data.afppiecesto).Count() >= 2;
                }
                if (e.EditOperationType == GridViewEditOperationType.Edit)
                {
                    exist = _dataContext.Data.Pieces.Where(o => o.afppiecesfrom == data.afppiecesfrom || o.afppiecesto == data.afppiecesfrom || o.afppiecesfrom == data.afppiecesto || o.afppiecesto == data.afppiecesto).Count() >= 2;
                }

                if (exist)
                {
                    sbError.AppendLine("Quantità in sovrapposizione");
                }

                if (!string.IsNullOrWhiteSpace(sbError.ToString()))
                {
                    Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(sbError.ToString()); });
                }

                if (!string.IsNullOrEmpty(sbError.ToString()))
                    e.IsValid = false;
            }
        }
    }
}
