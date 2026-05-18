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

namespace VulpesX.Modules.Ufp.Accounting
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
