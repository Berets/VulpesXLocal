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
using VulpesX.ViewModels.Modules.Default.Tables.CustomerRating;

namespace VulpesX.Modules.Default.Tables.CustomerRating
{
    /// <summary>
    /// Interaction logic for ElementWindow.xaml
    /// </summary>
    public partial class ElementWindow : FluentDefaultWindow
    {
        private ElementWindowViewModel _dataContext;
        public ElementWindow(ElementWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
        }

        #region Buttons
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save())
                {
                    this.DialogResult = true;
                }
            }
            else
            {
                ErrorHandler.Validation(validated);
            }
        }
        #endregion
    }
}
