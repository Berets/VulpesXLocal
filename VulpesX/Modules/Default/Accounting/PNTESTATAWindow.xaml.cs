using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for PNTESTATAWindow.xaml
    /// </summary>
    public partial class PNTESTATAWindow : FluentDefaultWindow
    {
        private PNTESTATAWindowViewModel _dataContext;
        public PNTESTATAWindow(PNTESTATAWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
            this.Loaded += (sender, e) =>
            {
                acCausal.IsEnabled = _dataContext.IsInsert;
                this.Title = $"Dettagli della testata registrazione di prima nota {(_dataContext.IsInsert ? "nuova" : _dataContext.Data.PrintFullID)}";
                if (_dataContext.IsReadonly)
                    this.Title += " - [sola lettura]";

            };

            if (_dataContext.Populate)
            {
                _dataContext.CFFlags = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = null, Description = "Nessuno" },
                    new GenericIDDescription { ID = "C", Description = "Cliente" },
                    new GenericIDDescription { ID = "F", Description = "Fornitore" }};
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            string? validated = _dataContext.Validate();

            if (validated == null)
            {
                _dataContext.Data.AccountingCausal = _dataContext.SelectedCausal;

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNRIGHEWindowViewModel>();
                windowViewModel.Head = _dataContext.Data;
                windowViewModel.HeadSelectedCausal = _dataContext.Data.AccountingCausal;
                windowViewModel.IsInsert = _dataContext.IsInsert;

                var wPNRIGHE = new PNRIGHEWindow(windowViewModel);
                wPNRIGHE.Owner = this.Owner;
                if (wPNRIGHE.ShowDialog() == true)
                    this.DialogResult = true;
            }
            else
            {
                ErrorHandler.Show(validated);
            }
        }

        private void cmbEntityType_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
        {
            if (string.IsNullOrWhiteSpace(_dataContext.Data.N1FLCF))
            {
                _dataContext.Data.N1CLFO = null;
                _dataContext.Codes = null;
                _dataContext.SelectedEntity = null;
            }
            else
            {
                var newCodes = _dataContext.GetABE();
                _dataContext.Codes = newCodes;
                _dataContext.SelectedEntity = newCodes?.Where(w => w.abecod == _dataContext.Data.N1CLFO).FirstOrDefault();
            }
        }

        private void acCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedEntity != null)
                _dataContext.Data.N1CLFO = _dataContext.SelectedEntity.abecod;
            else
                _dataContext.Data.N1CLFO = null;
        }

        private void acCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCausal.caucod))
            {
                _dataContext.Data.pncaus = _dataContext.SelectedCausal.caucod;
                // causal default CF flag
                _dataContext.Data.N1FLCF = _dataContext.SelectedCausal == null ? null : _dataContext.SelectedCausal.caucliBool ? "C" : (_dataContext.SelectedCausal.cauforBool ? "F" : null);

                if(!string.IsNullOrEmpty(_dataContext.SelectedCausal!.cauliv) && _dataContext.IsInsert)
                {
                    var libroIVA = _dataContext.GetLIBRIIVA(_dataContext.SelectedCausal!.cauliv);

                    if (libroIVA != null)
                    {
                        _dataContext.Data.N1docn = _dataContext.GetProtocolNumber(libroIVA).ToString();
                    }
                }

                cmbEntityType_SelectionChanged(null, null);
            }
        }

        private void acCausal_LostFocus(object sender, RoutedEventArgs e)
        {
            var autoCompleteBox = sender as RadAutoCompleteBox;

            if (autoCompleteBox != null)
            {
                if (autoCompleteBox.SelectedItem == null)
                {
                    autoCompleteBox.SearchText = null;
                    _dataContext.Data.pncaus = null;
                    _dataContext.SelectedCausal = null;
                }
            }
        }

        private void acCode_LostFocus(object sender, RoutedEventArgs e)
        {
            var autoCompleteBox = sender as RadAutoCompleteBox;
            if (autoCompleteBox != null)
            {
                if (autoCompleteBox.SelectedItem == null)
                {
                    autoCompleteBox.SearchText = null;
                    _dataContext.Data.N1CLFO = null;
                }
            }
        }

        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }
    }
}
