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
using Telerik.Windows.Controls.Data.DataFilter;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.DWH;

namespace VulpesX.Modules.Default.DWH
{
    /// <summary>
    /// Interaction logic for DWHFolderWindow.xaml
    /// </summary>
    public partial class DWHFolderWindow : FluentDefaultWindow
    {
        private DWHFolderWindowViewModel _dataContext;
        public DWHFolderWindow(DWHFolderWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

        }

        #region Buttons
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.IsInsert)
                _dataContext.Data.FolderID = Guid.NewGuid();

            var validated = _dataContext.Validate();
            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save())
                    this.DialogResult = true;
            }
            else
            {
                ErrorHandler.Validation(validated);
            }
        }
        #endregion

    }
}
