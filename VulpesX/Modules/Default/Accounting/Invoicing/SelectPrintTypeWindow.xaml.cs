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
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Invoicing;

namespace VulpesX.Modules.Default.Accounting.Invoicing
{
    /// <summary>
    /// Interaction logic for SelectPrintTypeWindow.xaml
    /// </summary>
    public partial class SelectPrintTypeWindow : FluentDefaultWindow
    {
        private SelectPrintTypeWindowViewModel _dataContext;
        public SelectPrintTypeWindow(SelectPrintTypeWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
        }

        private void cmdPrintAssoXML_Click(object sender, RoutedEventArgs e)
        {
            ReportingHandler.PrintInvoiceXML(_dataContext.Filename, _dataContext.Data, Constants.XSL_TYPE_INVOICE_ASSO, "XML");
            this.DialogResult = true;
        }

        private void cmdPrintAssoPDF_Click(object sender, RoutedEventArgs e)
        {
            ReportingHandler.PrintInvoiceXML(_dataContext.Filename, _dataContext.Data, Constants.XSL_TYPE_INVOICE_ASSO, "PDF");
            this.DialogResult = true;
        }

        private void cmdPrintAEXML_Click(object sender, RoutedEventArgs e)
        {
            ReportingHandler.PrintInvoiceXML(_dataContext.Filename, _dataContext.Data, _dataContext.SendFormat == "FPR12" ? Constants.XSL_TYPE_INVOICE_ADE_PR : Constants.XSL_TYPE_INVOICE_ADE_PA, "XML");
            this.DialogResult = true;
        }

        private void cmdPrintAEPDF_Click(object sender, RoutedEventArgs e)
        {
            ReportingHandler.PrintInvoiceXML(_dataContext.Filename, _dataContext.Data, _dataContext.SendFormat == "FPR12" ? Constants.XSL_TYPE_INVOICE_ADE_PR : Constants.XSL_TYPE_INVOICE_ADE_PA, "PDF");
            this.DialogResult = true;
        }
    }
}
