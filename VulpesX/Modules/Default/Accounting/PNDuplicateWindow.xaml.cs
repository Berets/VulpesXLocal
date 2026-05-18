using DocumentFormat.OpenXml.Office.PowerPoint.Y2021.M06.Main;
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
using VulpesX.DAL;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for PNDuplicateWindow.xaml
    /// </summary>
    public partial class PNDuplicateWindow : FluentDefaultWindow
    {
        private PNDuplicateWindowViewModel _dataContext;
        public PNDuplicateWindow(PNDuplicateWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;
        }

        private async void cmdConfirm_Click(object sender, RoutedEventArgs e)
        {
            var validate = _dataContext.Validate();

            if (string.IsNullOrEmpty(validate))
            {
                var result = await _dataContext.Duplicate();

                if (result != null)
                {
                    this.Tag = result;
                    this.DialogResult = true;
                }
                else
                {
                    this.DialogResult = false;
                }
            }
            else
            {
                ErrorHandler.Validation(validate);
            }
        }


        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }
    }
}
