using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
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
using VulpesX.DAL;
using VulpesX.Modules.Default.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;
using VulpesX.ViewModels.Modules.Default.Treasury;

namespace VulpesX.Modules.Default.Treasury
{
    /// <summary>
    /// Interaction logic for BankFluxesDetailsWindow.xaml
    /// </summary>
    public partial class BankFluxesDetailsWindow : FluentDefaultWindow
    {
        private BankFluxesDetailsWindowViewModel _dataContext;
        public BankFluxesDetailsWindow(BankFluxesDetailsWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);

            this.DataContext = _dataContext;

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    LoadData();
                }
            };

            LoadData();
        }

        private async void LoadData()
        {
            await _dataContext.Load();

            decimal last = _dataContext.StartBalance;
            string? lastSign = _dataContext.StartBalanceSign;
            foreach (var item in _dataContext.Items ?? new ObservableCollection<PNRIGHE>())
            {
                #region Customer description <> note
                if (item.Testata != null && (!item.Testata.N1CLFO.HasValue || item.Testata.N1CLFO == 0))
                    item.Testata.BasicRegistry = new ABE() { abers1 = "#", abers2 = item.N1DESC, abeind = string.Empty };
                #endregion
                #region Progressive
                if (lastSign == item.N1SEGN)
                {
                    last += item.N1IMEU ?? 0;
                    lastSign = item.N1SEGN;
                }
                else
                {
                    if (lastSign == "-")
                    {
                        last = item.N1IMEU ?? 0;
                        lastSign = item.N1SEGN;
                    }
                    else
                    {
                        if (lastSign == "D")
                        {
                            if (last > (item.N1IMEU ?? 0))
                            {
                                last -= item.N1IMEU ?? 0;
                                lastSign = "D";
                            }
                            else if (last == (item.N1IMEU ?? 0))
                            {
                                last -= 0;
                                lastSign = "-";
                            }
                            else
                            {
                                last = (item.N1IMEU ?? 0) - last;
                                lastSign = "A";
                            }
                        }
                        else
                        {
                            if (last > (item.N1IMEU ?? 0))
                            {
                                last -= item.N1IMEU ?? 0;
                                lastSign = "A";
                            }
                            else if (last == (item.N1IMEU ?? 0))
                            {
                                last -= 0;
                                lastSign = "-";
                            }
                            else
                            {
                                last = item.N1IMEU ?? 0 - last;
                                lastSign = "D";
                            }
                        }
                    }
                }
                item.TreasuryProgressive = last;
                item.TreasuryProgressiveSign = lastSign;
                #endregion
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
                LoadData();
        }

        private void cmdSflag_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as PNRIGHE;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Rimuovere il flag di temporanea dalla riga {item.N1RIGA} della registrazione {item.N1ANNO}/{item.N1REGI} del {item.Testata?.N1DARE?.ToString("dd/MM/yyyy")}"))
                {
                    if (_dataContext.RemoveTemporaryFlag(item))
                        LoadData();
                }
            }
        }

        private void cmdEditHead_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as PNRIGHE;

            if (item != null)
            {
                var head = _dataContext.Get(item.N1ANNO, item.N1REGI);

                if (head != null)
                {
                    var codes = !string.IsNullOrWhiteSpace(head.N1FLCF) ? _dataContext.GetABEs(head.N1FLCF) : null;
                    var causals = _dataContext.GetCAUCONTs();

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNTESTATAWindowViewModel>();
                    windowViewModel.CompanyID = _dataContext.CompanyID;
                    windowViewModel.Populate = true;
                    windowViewModel.IsInsert = false;
                    windowViewModel.IsReadonly = _dataContext.PrintedOnGeneralJournal(item.N1ANNO, item.N1REGI);
                    windowViewModel.Causals = causals;
                    windowViewModel.SelectedCausal = causals?.Where(w => w.caucod == head.pncaus).FirstOrDefault();
                    windowViewModel.Data = head;
                    windowViewModel.Codes = !string.IsNullOrWhiteSpace(head.N1FLCF) ? codes : null;
                    windowViewModel.SelectedEntity = !string.IsNullOrWhiteSpace(head.N1FLCF) ? codes?.Where(w => w.abecod == head.BasicRegistry?.abecod).FirstOrDefault() : null;

                    var wPNTESTATA = new PNTESTATAWindow(windowViewModel);
                    wPNTESTATA.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wPNTESTATA.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdEditDetails_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as PNRIGHE;

            if (item != null)
            {
                var head = _dataContext.Get(item.N1ANNO, item.N1REGI);

                if (head != null)
                {
                    var codes = !string.IsNullOrWhiteSpace(head.N1FLCF) ? _dataContext.GetABEs(head.N1FLCF) : null;
                    var causals = _dataContext.GetCAUCONTs();

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNRIGHEWindowViewModel>();
                    windowViewModel.Head = head;
                    windowViewModel.HeadSelectedCausal = causals?.Where(w => w.caucod == head.pncaus).FirstOrDefault();
                    windowViewModel.IsInsert = false;

                    var wPNRIGHE = new PNRIGHEWindow(windowViewModel);
                    //var wPNRIGHE = new wPNRIGHE(ctx, new PNTESTATAViewModel(true)
                    //{
                    //    Data = head,
                    //    Codes = !string.IsNullOrWhiteSpace(head.N1FLCF) ? codes : null,
                    //    SelectedEntity = !string.IsNullOrWhiteSpace(head.N1FLCF) ? codes.Where(w => w.abecod == head.BasicRegistry.abecod).FirstOrDefault() : null,
                    //    IsInsert = false,
                    //    Causals = causals,
                    //    SelectedCausal = causals.Where(w => w.caucod == head.pncaus).FirstOrDefault()
                    //});
                    wPNRIGHE.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wPNRIGHE.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void grdData_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.EditAction == Telerik.Windows.Controls.GridView.GridViewEditAction.Commit)
            {
                if (e.NewData != e.OldData)
                {
                    if (grdData.SelectedItem != null)
                    {
                        var selected = grdData.SelectedItem as PNRIGHE;

                        if (selected != null)
                        {
                            if (_dataContext.ChangeBankFlag(selected, (bool)e.NewData))
                                LoadData();
                        }
                    }
                }
            }
        }
    }
}
