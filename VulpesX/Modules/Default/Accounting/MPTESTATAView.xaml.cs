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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VulpesX.DAL;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Modules.Default.Accounting.Invoicing;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;
using VulpesX.ViewModels.Modules.Default.Accounting.Invoicing;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for MPTESTATAView.xaml
    /// </summary>
    public partial class MPTESTATAView : UserControl
    {
        private MPTESTATAViewModel _dataContext;
        private int _selectedPage = 0;
        private bool _isFirstLoad = true;

        public MPTESTATAView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<MPTESTATAViewModel>();
            _dataContext.Year = DateTime.Now;

            InitializeComponent();
            dtpYear.Culture = new System.Globalization.CultureInfo("it-IT");
            dtpYear.Culture.DateTimeFormat.ShortDatePattern = "yyyy";

            this.DataContext = _dataContext;
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    LoadData();
                }
            };

            GridView.DataLoaded += (s, e) =>
            {
                rdpGrid.MoveToPage(_selectedPage);
                txtSearch_TextChanged(txtSearch, null);
            };
            rdpGrid.PageIndexChanged += (s, e) =>
            {
                _selectedPage = e.NewPageIndex;
            };

            LoadData();
        }

        private async void LoadData()
        {
            await _dataContext.Load();
        }


        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)!.Execute(txtSearch.Text, GridView);
        }
        #endregion

        private void dtpYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isFirstLoad)
            {
                _isFirstLoad = false;
                return;
            }

            LoadData();
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<MPTESTATAWindowViewModel>();
            windowViewModel.Data = new MPTESTATA
            {
                MPSOCI = _dataContext.CompanyID,
                MPSBAN = _dataContext.CompanyID,
                MPANNO = now.Year,
                MPDATA = now,
                MPDVAL = now,
                MPIMAB = 0,
                MPEUAB = 0,
                MPVAIM = 0,
                MPVAAB = 0,
                MPFLST = "N",
                MPFLCO = "N",
                MPFLES = "N",
                MPASSE = string.Empty,
            };
            windowViewModel.IsInsert = true;

            var window = new MPTESTATAWindow(windowViewModel);
            window.ShowDialog();

            LoadData();
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as MPTESTATA;

            if (item != null && _dataContext.CanDelete(item))
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminazione del mandato di pagamento {item.MPANNO}/{item.MPNUME} ?"))
                {
                    if (_dataContext.Delete(item))
                        LoadData();
                    else
                        ErrorHandler.Validation("Errore imprevisto durante l'eliminazione");
                }
            }
            else
            {
                ErrorHandler.Validation("Mandato di pagamento già firmato, impossibile eliminare");
            }
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = (sender as Button)!.DataContext as MPTESTATA;

            if (selected != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<MPTESTATAWindowViewModel>();
                windowViewModel.Data = selected;
                windowViewModel.IsInsert = false;

                var window = new MPTESTATAWindow(windowViewModel);
                window.ShowDialog();

                LoadData();
            }
        }

        private void rgSign_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)!.DataContext as MPTESTATA;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermi di voler firmare il mandato di pagamento {item.MPANNO}/{item.MPNUME}? Non sarà più modificabile"))
                {
                    var result = _dataContext.Sign(item);

                    if (result)
                    {
                        InfoHandler.Show($"Mandato di pagamento firmato correttamente {item.MPANNO}/{item.MPNUME}");

                        LoadData();
                    }
                }
            }
        }

        private void rgFile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)!.DataContext as MPTESTATA;
            if (item != null)
            {
                var result = _dataContext.GenerateXML(item);

                if (!string.IsNullOrEmpty(result))
                {
                    FileHelper.Open(result);
                }
            }
        }

        #region Context menu
        private void cmGrid_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            rmiAccounting.IsEnabled = false;

            if (GridView.SelectedItem != null)
            {
                var item = GridView.SelectedItem as MPTESTATA;
                if (item != null)
                {
                    if (!item.MPFLCOBool)
                    {
                        rmiAccounting.Header = $"Contabilizza il mandato n. {item.MPNUME} del {item.MPDATA?.ToString("dd/MM/yyyy")}";
                        rmiAccounting.IsEnabled = item.MPFIRMABool;
                    }
                    else
                    {
                        rmiAccounting.Header = $"Mandato n. {item.MPNUME} del {item.MPDATA?.ToString("dd/MM/yyyy")} gia' contabilizzata, registrazione N° {item.regianno}/{item.reginumero}";
                    }
                }
            }
        }

        private void rmiAccounting_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridView.SelectedItem != null)
            {
                var selected = GridView.SelectedItem as MPTESTATA;

                if (selected != null)
                {
                    var rows = _dataContext.GetMPDETTAGLIOs(selected.MPANNO, selected.MPNUME);
                    var suppliers = _dataContext.GetSuppliersCache();

                    if (rows != null && rows.Any())
                    {
                        var supplier = suppliers?.Where(o => o.abecod == rows.First().M3SOTT).FirstOrDefault();

                        if (supplier != null)
                        {
                            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ECAccountingRegistrationWindowViewModel>();
                            windowViewModel.EntityType = "F";
                            windowViewModel.Entity = supplier;
                            windowViewModel.BankABI = selected.MPABI;
                            windowViewModel.BankCAB = selected.MPCAB;
                            windowViewModel.BankCC = selected.MPCCOR;

                            ObservableCollection<MastrinoECReportItem> rowsEC = new ObservableCollection<MastrinoECReportItem>();

                            string? validate = null;
                            foreach (var row in rows)
                            {
                                if (row.M3ANNO.HasValue && row.M3REGI.HasValue && row.M3RIGA.HasValue)
                                {
                                    var pnFornitori = _dataContext.GetPNFORNITORI(row.M3ANNO!.Value, row.M3REGI!.Value, row.M3RIGA!.Value);

                                    if (pnFornitori != null)
                                    {
                                        rowsEC.Add(new MastrinoECReportItem
                                        {
                                            CompanyID = pnFornitori.N3SOCI,
                                            EntityType = "S",
                                            Dare = pnFornitori.N3SEGN == "D" ? pnFornitori.N3IMEU ?? 0 : 0,
                                            Avere = pnFornitori.N3SEGN == "A" ? pnFornitori.N3IMEU ?? 0 : 0,
                                            ReferenceDate = pnFornitori.N3DARI,
                                            ReferenceID = pnFornitori.N3RIFE,
                                            PNFORNITORI = pnFornitori,
                                            Year = pnFornitori.N3ANNO,
                                            Number = pnFornitori.N3REGI,
                                            RowID = pnFornitori.N3RIGA,
                                            EntityID = pnFornitori.N3SOTT ?? 0,
                                        });
                                    }
                                    else
                                    {
                                        validate += $"Non trovata registrazione {row.M3ANNO}-{row.M3REGI}-{row.M3RIGA}\n";
                                    }
                                }
                                else
                                {
                                    validate += $"Errore imprevisto\n";
                                }
                            }

                            if (string.IsNullOrEmpty(validate))
                            {
                                windowViewModel.Items = rowsEC;

                                var window = new ECAccountingRegistrationWindow(windowViewModel);
                                if (window.ShowDialog() ?? false)
                                {

                                    selected.MPFLCO = "S";
                                    selected.regianno = windowViewModel.RegistrationYear;
                                    selected.reginumero = windowViewModel.RegistrationNumber;

                                    _dataContext.Update(selected);

                                    LoadData();
                                }
                            }
                            else
                            {
                                ErrorHandler.Validation(validate);
                            }
                        }
                    }
                }
            }
        }
        #endregion


    }
}
