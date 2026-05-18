using MediaFoundation.Misc;
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
using VulpesX.ViewModels.Modules.Default.Commons;

namespace VulpesX.Modules.Default.Commons
{
    /// <summary>
    /// Interaction logic for CancelReasonWindow.xaml
    /// </summary>
    public partial class CancelReasonWindow : FluentDefaultWindow
    {
        private CancelReasonWindowViewModel _dataContext;
        public CancelReasonWindow(CancelReasonWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
            tbLabel.Text = tbLabel.Text.Replace("#minSize#", _dataContext.MinSize.ToString());
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtReason.Text) && txtReason.Text.Trim().Length >= _dataContext.MinSize)
            {
                _dataContext.SelectedReason = txtReason.Text.Trim();
                this.DialogResult = true;
            }
            else
            {
                ErrorHandler.Validation($"Verificare la lunghezza della motivazione, deve essere di almeno {_dataContext.MinSize} caratteri e al massimo di {txtReason.MaxLength}");
                e.Handled = true;
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.SelectedReason = null;
            this.DialogResult = false;
        }

        private void txtReason_TextChanged(object sender, TextChangedEventArgs e)
        {
            _dataContext.CurrentSize = txtReason.Text.Trim().Length;
            e.Handled = true;
        }
    }
}
