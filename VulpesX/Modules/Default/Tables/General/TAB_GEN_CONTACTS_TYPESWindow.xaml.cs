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
using VulpesX.ViewModels.Modules.Default.Tables.General;

namespace VulpesX.Modules.Default.Tables.General
{
    /// <summary>
    /// Interaction logic for TAB_GEN_CONTACTS_TYPESWindow.xaml
    /// </summary>
    public partial class TAB_GEN_CONTACTS_TYPESWindow : FluentDefaultWindow
    {
        private TAB_GEN_CONTACTS_TYPESWindowViewModel _dataContext;
        public TAB_GEN_CONTACTS_TYPESWindow(TAB_GEN_CONTACTS_TYPESWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                this.DialogResult = _dataContext.Save();
            }
            else
            {
                ErrorHandler.Show(validated);
            }
        }
    }
}
