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
using VulpesX.ViewModels.Modules.Default.General;

namespace VulpesX.Modules.Default.General
{
    /// <summary>
    /// Interaction logic for NOTEFORWindow.xaml
    /// </summary>
    public partial class NOTEFORWindow : FluentDefaultWindow
    {
        private NOTEFORWindowViewModel _dataContext;
        public NOTEFORWindow(NOTEFORWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate(_dataContext.Data, _dataContext.IsInsert);
            if (string.IsNullOrWhiteSpace(validated))
            {
                _dataContext.Data.Updated = true;
                this.DialogResult = true;
            }
            else
            { ErrorHandler.Show(validated); }
        }
    }
}
