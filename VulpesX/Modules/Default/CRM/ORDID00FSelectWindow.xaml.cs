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
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for ORDID00FSelectWindow.xaml
    /// </summary>
    public partial class ORDID00FSelectWindow : FluentDefaultWindow
    {
        private ORDID00FSelectWindowViewModel _dataContext;
        public ORDID00FSelectWindow(ORDID00FSelectWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            // default quantity to send
            foreach (var row in _dataContext.AvailableRows ?? new System.Collections.ObjectModel.ObservableCollection<ORDID00F>())
            {
                row.QuantityToSend = row.ODQTAV - (row.ODQTAEV ?? 0);
            }

            this.Title = $"Seleziona le righe da aggiungere {(_dataContext.FlagTarget == "D" ? "al DDT" : "alla fattura")}";
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (rgvRows.SelectedItems != null && rgvRows.SelectedItems.Count > 0)
            {
                if (CheckRowsState())
                {
                    if (ConfirmHandler.Confirm($"Confermate la creazione di {(_dataContext.FlagTarget == "D" ? "un DDT" : "una fattura")} con le righe selezionate ?"))
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        var headNotes = string.Join("\n", _dataContext.OrdersHeads.Select(s => s.OTNOTET));
                        var footNotes = string.Join("\n", _dataContext.OrdersHeads.Select(s => s.OTNOTEP));

                        bool generationResult = _dataContext.GenerateByOrder(rgvRows.SelectedItems.Cast<ORDID00F>().ToList(), headNotes, footNotes);


                        if (generationResult)
                        {
                            // check if completed [E]
                            _dataContext.FlagFulfillment();
                        }
                    }
                    Mouse.OverrideCursor = null;
                    this.DialogResult = true;
                }
                else
                {
                    ErrorHandler.Validation("Se le quantita' sono totali occorre specificare 'SALDO' e le quantita' da evadere non possono eccedere la quantita' disponibile");
                }
            }
            else
            {
                ErrorHandler.Validation("Occorre selezionare almeno una riga da evadere");
            }
        }

        private bool CheckRowsState()
        {
            bool done = true;
            foreach (var row in rgvRows.SelectedItems.Cast<ORDID00F>())
            {
                if ((row.QuantityToSend == row.ODQTAV - row.ODQTAEV && row.FulfillmentID != "S") ||
                    row.QuantityToSend > row.ODQTAV - row.ODQTAEV)
                {
                    done = false;
                    break;
                }
            }
            return done;
        }
        #endregion
    }
}
