using DocumentFormat.OpenXml.Math;
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
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for AskInvoiceFinalDateWindow.xaml
    /// </summary>
    public partial class AskInvoiceFinalDateWindow : FluentDefaultWindow
    {
        private AskInvoiceFinalDateWindowViewModel _dataContext;
        public AskInvoiceFinalDateWindow(AskInvoiceFinalDateWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (rdpDate.SelectedValue.HasValue)
            {
                // check is valid
                var last = _dataContext.CheckLastFinalDate();
                if (last <= rdpDate.SelectedValue)
                {
                    _dataContext.SelectedDate = rdpDate.SelectedValue;
                    this.DialogResult = true;
                }
                else
                {
                    ErrorHandler.Validation($"Impossibile utilizzare questa data perche' ci sono fatture definitive con data superiore al {rdpDate.SelectedValue.Value.ToShortDateString()}\n\nL'ultima data di stampa definitiva e' il {last?.ToShortDateString()}");
                    e.Handled = true;
                }
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.SelectedDate = null;
            this.DialogResult = false;
        }
    }
}
