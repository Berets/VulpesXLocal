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
using VulpesX.ViewModels.Modules.Default.SRM;

namespace VulpesX.Modules.Default.SRM
{
    /// <summary>
    /// Interaction logic for GoodsReceiptWindow.xaml
    /// </summary>
    public partial class GoodsReceiptWindow : FluentDefaultWindow
    {
        private GoodsReceiptWindowViewModel _dataContext;
        public GoodsReceiptWindow(GoodsReceiptWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli entrata merce n.{_dataContext.Data.id}";
            };
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();
            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save())
                    this.DialogResult = true;
            }
            else
            { ErrorHandler.Validation(validated); }
        }
        #endregion
    }
}
