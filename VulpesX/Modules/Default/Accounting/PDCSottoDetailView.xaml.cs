using SharpDX.Direct3D9;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for PDCSottoDetailView.xaml
    /// </summary>
    public partial class PDCSottoDetailView : UserControl
    {
        private PDCSottoDetailViewModel _dataContext;
        public event EventHandler? Saved;
        public PDCSottoDetailView(PDCSottoDetailViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.Banks = !string.IsNullOrWhiteSpace(_dataContext.Data!.p3soci) ? _dataContext.LoadBanks(_dataContext.Data!.p3soci) : null;
            _dataContext.IsBankEnabled = !string.IsNullOrWhiteSpace(_dataContext.Data!.p3soci);

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedCEEItem = _dataContext.CEEs?
                     .Where(w => w.ceevo1 == _dataContext.Data.p3cee1 &&
                                 w.ceevo2 == _dataContext.Data.p3cee2 &&
                                 w.ceevo3 == _dataContext.Data.p3cee3 &&
                                 w.ceevo4 == _dataContext.Data.p3cee4 &&
                                 w.ceevo5 == _dataContext.Data.p3cee5 &&
                                 w.ceevo6 == _dataContext.Data.p3cee6 &&
                                 w.ceevo7 == _dataContext.Data.p3cee7).FirstOrDefault();
                _dataContext.SelectedCEEItemAlt = _dataContext.CEEs?
                    .Where(w => w.ceevo1 == _dataContext.Data.p3cee1a &&
                                w.ceevo2 == _dataContext.Data.p3cee2a &&
                                w.ceevo3 == _dataContext.Data.p3cee3a &&
                                w.ceevo4 == _dataContext.Data.p3cee4a &&
                                w.ceevo5 == _dataContext.Data.p3cee5a &&
                                w.ceevo6 == _dataContext.Data.p3cee6a &&
                                w.ceevo7 == _dataContext.Data.p3cee7a).FirstOrDefault();
                _dataContext.SelectedBranch = _dataContext.Branches?
                    .Where(w => w.filcod == _dataContext.Data.p3filiale).FirstOrDefault();
                if (_dataContext.Banks != null)
                    _dataContext.SelectedBank = _dataContext.Banks
                        .Where(w => w.abiabi == _dataContext.Data.p3abi &&
                                  w.abicab == _dataContext.Data.P3cab).FirstOrDefault();
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmHandler.Confirm("Confermate il salvataggio dei dati ?"))
            {
                var validated = _dataContext.Validate();
                if (string.IsNullOrWhiteSpace(validated))
                {
                    if (_dataContext.IsInsert)
                    {
                        if (_dataContext.Insert())
                        {
                            var lastYear = _dataContext.GetLastExerciseYear();

                            if (lastYear != null)
                            {
                                if (ConfirmHandler.Confirm($"Si desidera creare in automatico l'anno [{lastYear}] per il sottoconto appena inserito ?"))
                                {
                                    var result = _dataContext.InsertPDCANNI(lastYear.Value);

                                    if (result)
                                        InfoHandler.Show($"L'anno [{lastYear}] e' stato creato correttamente");
                                    else
                                        ErrorHandler.Show($"Errore durante la creazione dell'anno [{lastYear}]");
                                }
                            }
                            else
                            {
                                ErrorHandler.Show("Non trovato esercizio");
                            }
                        }
                        else
                        {
                            ErrorHandler.Show("Errore durante il salvataggio del sottoconto");
                        }
                    }
                    else
                    {
                        _dataContext.Update();
                    }

                    if (Saved != null)
                        Saved(this, EventArgs.Empty);
                }
                else
                {
                    ErrorHandler.Show(validated);
                }
            }
        }

        private void acCEE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (acCEE.SelectedItem != null)
            {
                var selected = acCEE.SelectedItem as PNCEEBIL;

                if (selected != null)
                {
                    if (_dataContext.Data != null)
                    {
                        _dataContext.Data.p3cee1 = selected.ceevo1;
                        _dataContext.Data.p3cee2 = selected.ceevo2;
                        _dataContext.Data.p3cee3 = selected.ceevo3;
                        _dataContext.Data.p3cee4 = selected.ceevo4;
                        _dataContext.Data.p3cee5 = selected.ceevo5;
                        _dataContext.Data.p3cee6 = selected.ceevo6;
                        _dataContext.Data.p3cee7 = selected.ceevo7;
                    }
                }
            }
        }

        private void acCEE_LostFocus(object sender, RoutedEventArgs e)
        {
            if (acCEE.SelectedItem == null)
            {
                acCEE.SearchText = null;

                if (_dataContext.Data != null)
                {
                    _dataContext.Data.p3cee1 = null;
                    _dataContext.Data.p3cee2 = null;
                    _dataContext.Data.p3cee3 = null;
                    _dataContext.Data.p3cee4 = null;
                    _dataContext.Data.p3cee5 = null;
                    _dataContext.Data.p3cee6 = null;
                    _dataContext.Data.p3cee7 = null;
                }
            }
        }

        private void acCEEAlt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (acCEEAlt.SelectedItem != null)
            {
                var selected = acCEEAlt.SelectedItem as PNCEEBIL;
                if (selected != null)
                {
                    if (_dataContext.Data != null)
                    {
                        _dataContext.Data.p3cee1a = selected.ceevo1;
                        _dataContext.Data.p3cee2a = selected.ceevo2;
                        _dataContext.Data.p3cee3a = selected.ceevo3;
                        _dataContext.Data.p3cee4a = selected.ceevo4;
                        _dataContext.Data.p3cee5a = selected.ceevo5;
                        _dataContext.Data.p3cee6a = selected.ceevo6;
                        _dataContext.Data.p3cee7a = selected.ceevo7;
                    }
                }
            }
        }

        private void acCEEAlt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (acCEEAlt.SelectedItem == null)
            {
                acCEEAlt.SearchText = null;

                if (_dataContext.Data != null)
                {
                    _dataContext.Data.p3cee1a = null;
                    _dataContext.Data.p3cee2a = null;
                    _dataContext.Data.p3cee3a = null;
                    _dataContext.Data.p3cee4a = null;
                    _dataContext.Data.p3cee5a = null;
                    _dataContext.Data.p3cee6a = null;
                    _dataContext.Data.p3cee7a = null;
                }
            }
        }

        private void acBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (acBranch.SelectedItem != null)
            {
                var selected = acBranch.SelectedItem as FILIALI;
                if (selected != null)
                {
                    if (_dataContext.Data != null)
                    {
                        _dataContext.Data.p3filiale = selected.filcod;
                    }
                }
            }
        }

        private void acBranch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (acBranch.SelectedItem == null)
            {
                acBranch.SearchText = null;
                if (_dataContext.Data != null)
                {
                    _dataContext.Data.p3filiale = null;
                }
            }
        }

        private void acBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (acBank.SelectedItem != null)
            {
                var selected = acBank.SelectedItem as BANAZIEN;
                if (selected != null)
                {
                    if (_dataContext.Data != null)
                    {
                        _dataContext.Data.p3abi = selected.abiabi;
                        _dataContext.Data.P3cab = selected.abicab;
                        _dataContext.Data.P3CONTO = selected.abicon;
                    }
                }
            }
        }

        private void acBank_LostFocus(object sender, RoutedEventArgs e)
        {
            if (acBank.SelectedItem == null)
            {
                acBank.SearchText = null;

                if (_dataContext.Data != null)
                {
                    _dataContext.Data.p3abi = null;
                    _dataContext.Data.P3cab = null;
                    _dataContext.Data.P3CONTO = null;
                }
            }
        }

        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }


        private void RadComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (sender as RadComboBox)!.SelectedValue;

            _dataContext.IsBankEnabled = selected != null;

            if (selected != null)
                _dataContext.Banks = _dataContext.LoadBanks(selected!.ToString()!);

            if (!_dataContext.IsBankEnabled)
            {
                if (_dataContext.Data != null)
                {
                    _dataContext.Data.p3abi = null;
                    _dataContext.Data.P3cab = null;
                    _dataContext.Data.P3CONTO = null;
                    _dataContext.SelectedBank = null;
                }
            }
        }
    }
}
