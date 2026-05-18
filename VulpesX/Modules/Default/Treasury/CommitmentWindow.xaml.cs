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
using VulpesX.ViewModels.Modules.Default.Treasury;

namespace VulpesX.Modules.Default.Treasury
{
    /// <summary>
    /// Interaction logic for CommitmentWindow.xaml
    /// </summary>
    public partial class CommitmentWindow : FluentDefaultWindow
    {
        private CommitmentWindowViewModel _dataContext;
        public CommitmentWindow(CommitmentWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            if (_dataContext.LastDate.HasValue)
                _dataContext.Data.ifdata = _dataContext.LastDate.Value;

            if (!string.IsNullOrWhiteSpace(_dataContext.LastReference))
                _dataContext.Data.ifrife = _dataContext.LastReference;

            if (!string.IsNullOrWhiteSpace(_dataContext.LastGroupID))
            {
                _dataContext.Data.SelectedGroup = _dataContext.Data.GroupsList?.Where(w => w.P1GRUP == _dataContext.LastGroupID).FirstOrDefault();
                _dataContext.Data.SelectedAccount = _dataContext.Data.AccountsList?.Where(w => w.P1GRUP == _dataContext.LastGroupID && w.P2CONT == _dataContext.LastAccountID).FirstOrDefault();
                _dataContext.Data.SelectedSubaccount = _dataContext.Data.SubaccountsList?.Where(w => w.P1GRUP == _dataContext.LastGroupID && w.P2CONT == _dataContext.LastAccountID && w.P3SOTC == _dataContext.LastSubaccountID).FirstOrDefault();

                dtpDate.Focus();
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
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

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validation = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validation))
            {
                if (!string.IsNullOrWhiteSpace(_dataContext.Data.ifgrup) && !string.IsNullOrWhiteSpace(_dataContext.Data.ifcont) && !string.IsNullOrWhiteSpace(_dataContext.Data.ifsott) &&
                    _dataContext.Data.ifimpo > 0 && !string.IsNullOrWhiteSpace(_dataContext.Data.ifrife))
                {
                    if (_dataContext.Save())
                    {
                        _dataContext.LastGroupID = _dataContext.Data.SelectedGroup?.P1GRUP;
                        _dataContext.LastAccountID = _dataContext.Data.SelectedAccount?.P2CONT;
                        _dataContext.LastSubaccountID = _dataContext.Data.SelectedSubaccount?.P3SOTC;
                        _dataContext.LastDate = _dataContext.Data.ifdata;
                        _dataContext.LastReference = _dataContext.Data.ifrife;

                        this.DialogResult = true;
                    }
                }
                else
                {
                    ErrorHandler.Validation("Tutti i dati sono obbligatori, ad eccezione di note e data chiusura");
                }
            }
            else
            {
                ErrorHandler.Validation(validation);
            }
        }
    }
}
