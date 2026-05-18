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
    /// Interaction logic for ACC_ASSETS_CATEGORIESWindow.xaml
    /// </summary>
    public partial class ACC_ASSETS_CATEGORIESWindow : FluentDefaultWindow
    {
        private ACC_ASSETS_CATEGORIESWindowViewModel _dataContext;
        public ACC_ASSETS_CATEGORIESWindow(ACC_ASSETS_CATEGORIESWindowViewModel dataContext)
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
                DialogResult = _dataContext.Save();
            }
            else
            {
                ErrorHandler.Show(validated);
            }
        }
    }
}
