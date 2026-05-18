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
using VulpesX.ViewModels.Modules.Ufp.Tables.Accounting;

namespace VulpesX.Modules.Ufp.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for AREEWindow.xaml
    /// </summary>
    public partial class INCASSOWindow : FluentDefaultWindow
    {
        private INCASSOWindowViewModel _dataContext;
        public INCASSOWindow(INCASSOWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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
