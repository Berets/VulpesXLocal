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
    /// Interaction logic for CONSEGNAWindow.xaml
    /// </summary>
    public partial class CONSEGNAWindow : FluentDefaultWindow
    {
        private CONSEGNAWindowViewModel _dataContext;
        public CONSEGNAWindow(CONSEGNAWindowViewModel dataContext)
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
                    _dataContext.SelectedTraduzione.concod = _dataContext.Data.concod;
                    _dataContext.InsertOrUpdateCONSEGNA_LINGUA(_dataContext.SelectedTraduzione);
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
                    _dataContext.SelectedTraduzione.concod = _dataContext.Data.concod;

                    _dataContext.InsertOrUpdateCONSEGNA_LINGUA(_dataContext.SelectedTraduzione);
                }
            }

            if (e.AddedItems.Count > 0)
            {
                var added = e.AddedItems[0] as LINGUA;

                if (added != null)
                    _dataContext.SelectedTraduzione = _dataContext.GetCONSEGNA_LINGUA(_dataContext.Data.concod, added.lincod);
            }
        }
    }
}
