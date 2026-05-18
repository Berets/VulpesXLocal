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
using VulpesX.ViewModels.Modules.Default.Accounting.IVA;

namespace VulpesX.Modules.Default.Accounting.IVA
{
    /// <summary>
    /// Interaction logic for TCOMLIQIVAWindow.xaml
    /// </summary>
    public partial class TCOMLIQIVAWindow : FluentDefaultWindow
    {
        private TCOMLIQIVAWindowViewModel _dataContext;
        public TCOMLIQIVAWindow(TCOMLIQIVAWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
        }

        #region Buttons

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            // set insert/update info
            foreach (var item in _dataContext.Rows ?? new System.Collections.ObjectModel.ObservableCollection<TCOMLIQIVA>())
            {
                if (!_dataContext.IsInsert)
                    item.updatedUserID = _dataContext.UserID;
            }

            ComputeVP14();

            if (_dataContext.Update())
            {
                Mouse.OverrideCursor = null;
                this.DialogResult = true;
            }
        }
        #endregion

        private void ComputeVP14()
        {
            // compute VP14 =  vp6 + vp7 – vp8 – vp9 – vp11  + vp12 - vp13 
            // se negativo va in credito * -1 altrimenti debito
            var vp14 = _dataContext.Rows?.Where(w => w.CLIVocLiq?.TrimEnd() == "VP14").First();
            var vp6 = _dataContext.Rows?.Where(w => w.CLIVocLiq?.TrimEnd() == "VP6").First();
            var credits = _dataContext.Rows?.Where(w => w.CLIVocLiq?.TrimEnd() == "VP8").First().CLIImpLiqC + _dataContext.Rows?.Where(w => w.CLIVocLiq?.TrimEnd() == "VP9").First().CLIImpLiqC +
                _dataContext.Rows?.Where(w => w.CLIVocLiq?.TrimEnd() == "VP11").First().CLIImpLiqC + _dataContext.Rows?.Where(w => w.CLIVocLiq?.TrimEnd() == "VP13").First().CLIImpLiqC + vp6?.CLIImpLiqC;
            var debits = vp6?.CLIImpLiqD + _dataContext.Rows?.Where(w => w.CLIVocLiq?.TrimEnd() == "VP7").First().CLIImpLiqD +
                _dataContext.Rows?.Where(w => w.CLIVocLiq?.TrimEnd() == "VP12").First().CLIImpLiqD;

            var vp14Value = debits - credits;

            if (vp14 != null && vp14Value.HasValue)
            {
                if (vp14Value > 0)
                {
                    vp14.CLIImpLiqC = 0;
                    vp14.CLIImpLiqD = vp14Value!.Value;
                }
                else
                {
                    vp14.CLIImpLiqC = (vp14Value!.Value) * -1;
                    vp14.CLIImpLiqD = 0;
                }
            }
        }

        #region Grid
        private void rgvRows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as TCOMLIQIVA;

                var validated = _dataContext.Validate();

                if (string.IsNullOrWhiteSpace(validated))
                {
                    ComputeVP14();
                    e.IsValid = true;
                }
                else
                {
                    ErrorHandler.Validation(validated);
                    e.IsValid = false;
                }
            }
            else
            {
                e.IsValid = true;
            }
        }
        private void rgvRows_BeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
        {
            var model = e.Row.Item as TCOMLIQIVA;

            if (model != null)
            {
                if (model.CLIVocLiq != "VP14")
                {
                    e.Handled = true;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        #endregion
    }
}
