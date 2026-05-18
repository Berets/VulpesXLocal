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
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Default.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for FE_IVADOCWindow.xaml
    /// </summary>
    public partial class FE_IVADOCWindow : FluentDefaultWindow
    {
        private FE_IVADOCWindowViewModel _dataContext;
        public FE_IVADOCWindow(FE_IVADOCWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
        }


        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {

                Mouse.OverrideCursor = null;

                if (_dataContext.Save())
                    this.DialogResult = true;

            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation(validated);
            }
        }
        #endregion
    }
}
