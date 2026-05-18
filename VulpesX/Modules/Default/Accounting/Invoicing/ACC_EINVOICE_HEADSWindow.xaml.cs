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
using VulpesX.ViewModels.Modules.Default.Accounting.Invoicing;

namespace VulpesX.Modules.Default.Accounting.Invoicing
{
    /// <summary>
    /// Interaction logic for ACC_EINVOICE_HEADSWindow.xaml
    /// </summary>
    public partial class ACC_EINVOICE_HEADSWindow : FluentDefaultWindow
    {
        private ACC_EINVOICE_HEADSWindowViewModel _dataContext;
        public ACC_EINVOICE_HEADSWindow(ACC_EINVOICE_HEADSWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();
         
            this.DataContext = _dataContext;
            this.Title = $"Dettagli fattura elettronica n. {_dataContext.Data.fattnum} del {_dataContext.Data.fattdata.ToString("dd/MM/yyyy")}";
        }
    }
}
