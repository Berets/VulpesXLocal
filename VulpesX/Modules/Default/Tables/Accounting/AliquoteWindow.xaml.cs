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
    /// Interaction logic for AliquoteWindow.xaml
    /// </summary>
    public partial class AliquoteWindow : FluentDefaultWindow
    {
        private AliquoteWindowViewModel _dataContext;
        public AliquoteWindow(AliquoteWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedIVANature = _dataContext.IVANatures?.Where(w => w.FETICod == _dataContext.Data.assnatufe).FirstOrDefault();
            }
            _dataContext.Data.asspin = _dataContext.Data.asspin ?? 0;
            _dataContext.Data.RatesList = _dataContext.GetRatesList();
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

        #region Autocompletes
        private void acIVANature_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedIVANature != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedIVANature.FETICod))
            {
                _dataContext.Data.assnatufe = _dataContext.SelectedIVANature.FETICod;
            }
            else
            {
                _dataContext.Data.assnatufe = null;
            }
        }
        private void ac_LostFocus(object sender, RoutedEventArgs e)
        {
            var ac = sender as RadAutoCompleteBox;
            if (ac != null)
            {
                if (ac.SelectedItem == null)
                {
                    ac.SearchText = null;
                }
            }
        }
        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }
        #endregion
    }
}
