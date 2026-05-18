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
    /// Interaction logic for SPEDIZIONIWindow.xaml
    /// </summary>
    public partial class SPEDIZIONEWindow : FluentDefaultWindow
    {
        private SPEDIZIONEWindowViewModel _dataContext;
        public SPEDIZIONEWindow(SPEDIZIONEWindowViewModel dataContext)
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
                if (_dataContext.SelectedTraduzione != null)
                {
                    _dataContext.SelectedTraduzione.specod = _dataContext.Data.specod;
                    _dataContext.InsertOrUpdateLanguage(_dataContext.SelectedTraduzione);
                }

                this.DialogResult = _dataContext.Save();
            }
            else
            {
                ErrorHandler.Show(validated);
            }
        }

        private void cmbLingua_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                if (_dataContext.SelectedTraduzione != null)
                {
                    _dataContext.SelectedTraduzione.specod = _dataContext.Data.specod;

                    _dataContext.InsertOrUpdateLanguage(_dataContext.SelectedTraduzione);
                }
            }

            if (e.AddedItems.Count > 0)
            {
                var added = e.AddedItems[0] as LINGUA;

                if (added != null)
                    _dataContext.SelectedTraduzione = _dataContext.GetSPEDIZIONE_LINGUA(_dataContext.Data.specod, added.lincod);
            }
        }
    }
}
