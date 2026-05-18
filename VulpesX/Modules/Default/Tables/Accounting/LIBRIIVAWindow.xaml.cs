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
    /// Interaction logic for LIBRIIVAWindow.xaml
    /// </summary>
    public partial class LIBRIIVAWindow : FluentDefaultWindow
    {
        private LIBRIIVAWindowViewModel _dataContext;
        public LIBRIIVAWindow(LIBRIIVAWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadData();

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedGiroCausal = _dataContext.Causals?.Where(w => w.caucod == _dataContext.Data.livcgi).FirstOrDefault();
                _dataContext.SelectedStornoCausal = _dataContext.Causals?.Where(w => w.caucod == _dataContext.Data.livcii).FirstOrDefault();
            }
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {

                if (_dataContext.Save())
                {
                    Mouse.OverrideCursor = null;
                    this.DialogResult = true;
                }

            }
            else
            { Mouse.OverrideCursor = null; ErrorHandler.Validation(validated); }
        }
        #endregion

        #region Autocompletes
        private void acGiroCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedGiroCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedGiroCausal.caucod))
            {
                _dataContext.Data.livcgi = _dataContext.SelectedGiroCausal.caucod;
            }
            else
            { _dataContext.Data.livcgi = null; }
        }

        private void acStornoCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedStornoCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedStornoCausal.caucod))
            {
                _dataContext.Data.livcii = _dataContext.SelectedStornoCausal.caucod;
            }
            else
            { _dataContext.Data.livcii = null; }
        }

        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
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

        #endregion
    }
}
