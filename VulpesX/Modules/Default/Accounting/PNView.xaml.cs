using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Telerik.Windows.Controls.Filtering.Editors;
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Controls.Utilities;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;
using static VulpesX.Models.Models.Accounting.AccountingSituationViewModel;
using static VulpesX.Shared.Utilities.TelerikGridService;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for PNView.xaml
    /// </summary>
    public partial class PNView : UserControl
    {
        private PNViewModel _dataContext;
        private List<GenericIDDescription> _currentSort = new List<GenericIDDescription>();
        private List<FilterEntry> _currentWhere = new List<FilterEntry>();
        private bool _isFirstLoad = true;

        public PNView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<PNViewModel>();
            _dataContext.Year = DateTime.Now;
            _dataContext.PageSize = 20;
            _dataContext.PageRequested = 0;

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

            LoadData();
            LoadSituation();
        }

        private async void LoadData()
        {
            await _dataContext.Load(_currentSort, _currentWhere);

            rddbInsert.IsEnabled = _dataContext.Esercizio != null && _dataContext.Esercizio.eseest != "C" && _dataContext.Esercizio.eseist != "C";
        }

        private async void LoadSituation()
        {
            await _dataContext.LoadSituation();

            var converter = (IValueConverter)this.Resources["stringToColorConverter"];
            // ATTIVITA
            spAttivitaText.Children.Clear();
            spAttivitaSaldo.Children.Clear();
            foreach (var item in _dataContext.AccountingSituation?.AttivitaGruppi ?? new System.Collections.ObjectModel.ObservableCollection<ASItem>())
            {
                spAttivitaText.Children.Add(new TextBlock()
                {
                    Text = item.Description,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    TextAlignment = TextAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.DeepSkyBlue),
                    FontSize = 14
                });
                var newValue = new TextBlock()
                {
                    Text = item.Saldo.ToString("N2"),
                    TextAlignment = TextAlignment.Right,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center
                };
                BindingOperations.SetBinding(newValue, TextBlock.ForegroundProperty, new Binding() { Source = item, Path = new PropertyPath("SaldoColore"), Converter = converter });

                spAttivitaSaldo.Children.Add(newValue);
                var newSPText = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                var newSPSaldo = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                foreach (var acc in item.Accounts ?? new System.Collections.ObjectModel.ObservableCollection<ASItem>())
                {
                    newSPText.Children.Add(new TextBlock()
                    {
                        Text = acc.Description,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        TextAlignment = TextAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = new SolidColorBrush(Colors.White),
                        FontSize = 14,
                        Margin = new Thickness(20, 0, 0, 0),
                    });
                    var newValueAcc = new TextBlock()
                    {
                        Text = acc.Saldo.ToString("N2"),
                        TextAlignment = TextAlignment.Right,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tag = acc,
                        TextDecorations = TextDecorations.Underline,
                        ToolTip = "Clicca per vedere il dettaglio dei sottoconti",
                        Cursor = Cursors.Hand
                    };
                    newValueAcc.MouseDown += Account_MouseDown;
                    BindingOperations.SetBinding(newValueAcc, TextBlock.ForegroundProperty, new Binding() { Source = acc, Path = new PropertyPath("SaldoColore"), Converter = converter });
                    newSPSaldo.Children.Add(newValueAcc);
                }

                spAttivitaText.Children.Add(newSPText);
                spAttivitaSaldo.Children.Add(newSPSaldo);
            }
            // PASSIVITA
            spPassivitaText.Children.Clear();
            spPassivitaSaldo.Children.Clear();
            foreach (var item in _dataContext.AccountingSituation?.PassivitaGruppi ?? new System.Collections.ObjectModel.ObservableCollection<ASItem>())
            {
                spPassivitaText.Children.Add(new TextBlock()
                {
                    Text = item.Description,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    TextAlignment = TextAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.DeepSkyBlue),
                    FontSize = 14
                });
                var newValue = new TextBlock()
                {
                    Text = item.Saldo.ToString("N2"),
                    TextAlignment = TextAlignment.Right,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14
                };
                BindingOperations.SetBinding(newValue, TextBlock.ForegroundProperty, new Binding() { Source = item, Path = new PropertyPath("SaldoColore"), Converter = converter });

                spPassivitaSaldo.Children.Add(newValue);
                var newSPText = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                var newSPSaldo = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                foreach (var acc in item.Accounts ?? new System.Collections.ObjectModel.ObservableCollection<ASItem>())
                {
                    newSPText.Children.Add(new TextBlock()
                    {
                        Text = acc.Description,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        TextAlignment = TextAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = new SolidColorBrush(Colors.White),
                        FontSize = 14,
                        Margin = new Thickness(20, 0, 0, 0),
                    });
                    var newValueAcc = new TextBlock()
                    {
                        Text = acc.Saldo.ToString("N2"),
                        TextAlignment = TextAlignment.Right,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tag = acc,
                        TextDecorations = TextDecorations.Underline,
                        ToolTip = "Clicca per vedere il dettaglio dei sottoconti",
                        Cursor = Cursors.Hand,
                        FontSize = 14
                    };
                    newValueAcc.MouseDown += Account_MouseDown;
                    BindingOperations.SetBinding(newValueAcc, TextBlock.ForegroundProperty, new Binding() { Source = acc, Path = new PropertyPath("SaldoColore"), Converter = converter });
                    newSPSaldo.Children.Add(newValueAcc);
                }

                spPassivitaText.Children.Add(newSPText);
                spPassivitaSaldo.Children.Add(newSPSaldo);
            }
            // COSTI
            spCostiText.Children.Clear();
            spCostiSaldo.Children.Clear();

            foreach (var item in _dataContext.AccountingSituation?.CostiGruppi ?? new System.Collections.ObjectModel.ObservableCollection<ASItem>())
            {
                spCostiText.Children.Add(new TextBlock()
                {
                    Text = item.Description,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    TextAlignment = TextAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.DeepSkyBlue),
                    FontSize = 14
                });
                var newValue = new TextBlock()
                {
                    Text = item.Saldo.ToString("N2"),
                    TextAlignment = TextAlignment.Right,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14
                };
                BindingOperations.SetBinding(newValue, TextBlock.ForegroundProperty, new Binding() { Source = item, Path = new PropertyPath("SaldoColore"), Converter = converter });

                spCostiSaldo.Children.Add(newValue);
                var newSPText = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                var newSPSaldo = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                foreach (var acc in item.Accounts ?? new System.Collections.ObjectModel.ObservableCollection<ASItem>())
                {
                    newSPText.Children.Add(new TextBlock()
                    {
                        Text = acc.Description,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        TextAlignment = TextAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = new SolidColorBrush(Colors.White),
                        FontSize = 14,
                        Margin = new Thickness(20, 0, 0, 0),
                    });
                    var newValueAcc = new TextBlock()
                    {
                        Text = acc.Saldo.ToString("N2"),
                        TextAlignment = TextAlignment.Right,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tag = acc,
                        TextDecorations = TextDecorations.Underline,
                        ToolTip = "Clicca per vedere il dettaglio dei sottoconti",
                        Cursor = Cursors.Hand,
                        FontSize = 14
                    };
                    newValueAcc.MouseDown += Account_MouseDown;
                    BindingOperations.SetBinding(newValueAcc, TextBlock.ForegroundProperty, new Binding() { Source = acc, Path = new PropertyPath("SaldoColore"), Converter = converter });
                    newSPSaldo.Children.Add(newValueAcc);
                }

                spCostiText.Children.Add(newSPText);
                spCostiSaldo.Children.Add(newSPSaldo);
            }
            // RICAVI
            spRicaviText.Children.Clear();
            spRicaviSaldo.Children.Clear();
            foreach (var item in _dataContext.AccountingSituation?.RicaviGruppi ?? new System.Collections.ObjectModel.ObservableCollection<ASItem>())
            {
                spRicaviText.Children.Add(new TextBlock()
                {
                    Text = item.Description,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    TextAlignment = TextAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.DeepSkyBlue),
                    FontSize = 14
                });
                var newValue = new TextBlock()
                {
                    Text = item.Saldo.ToString("N2"),
                    TextAlignment = TextAlignment.Right,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 14
                };
                BindingOperations.SetBinding(newValue, TextBlock.ForegroundProperty, new Binding() { Source = item, Path = new PropertyPath("SaldoColore"), Converter = converter });

                spRicaviSaldo.Children.Add(newValue);
                var newSPText = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                var newSPSaldo = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                foreach (var acc in item.Accounts ?? new System.Collections.ObjectModel.ObservableCollection<ASItem>())
                {
                    newSPText.Children.Add(new TextBlock()
                    {
                        Text = acc.Description,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        TextAlignment = TextAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = new SolidColorBrush(Colors.White),
                        FontSize = 14,
                        Margin = new Thickness(20, 0, 0, 0),
                    });
                    var newValueAcc = new TextBlock()
                    {
                        Text = acc.Saldo.ToString("N2"),
                        TextAlignment = TextAlignment.Right,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        Tag = acc,
                        TextDecorations = TextDecorations.Underline,
                        ToolTip = "Clicca per vedere il dettaglio dei sottoconti",
                        Cursor = Cursors.Hand,
                        FontSize = 14
                    };
                    newValueAcc.MouseDown += Account_MouseDown;
                    BindingOperations.SetBinding(newValueAcc, TextBlock.ForegroundProperty, new Binding() { Source = acc, Path = new PropertyPath("SaldoColore"), Converter = converter });
                    newSPSaldo.Children.Add(newValueAcc);
                }

                spRicaviText.Children.Add(newSPText);
                spRicaviSaldo.Children.Add(newSPSaldo);
            }
        }

        #region Buttons
        private void rmiInsertStandard_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (_dataContext.Esercizio != null && _dataContext.Esercizio.eseest != "C" && _dataContext.Esercizio.eseist != "C")
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNTESTATAWindowViewModel>();
                windowViewModel.CompanyID = _dataContext.CompanyID;
                windowViewModel.Populate = true;
                windowViewModel.IsInsert = true;
                windowViewModel.IsReadonly = false;
                windowViewModel.Causals = _dataContext.GetCAUCONT("N");
                windowViewModel.Data = new PNTESTATA
                {
                    N1SOCI = _dataContext.CompanyID,
                    N1ANNO = dtpYear.SelectedValue?.Year ?? 0,
                    pnvcod = "UIC",
                    N1TmpPNBool = false,
                    N1DARE = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime().Date,
                    pnvdiv = "EUR",
                    N1FLCF = null
                };

                var wPNTESTATA = new PNTESTATAWindow(windowViewModel);
                wPNTESTATA.Owner = Window.GetWindow(this);
                if (wPNTESTATA.ShowDialog() == true)
                    LoadData();
            }
        }

        private void rmiInsertRepeat_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (_dataContext.Esercizio != null && _dataContext.Esercizio.eseest != "C" && _dataContext.Esercizio.eseist != "C")
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNTESTATAWindowViewModel>();
                windowViewModel.CompanyID = _dataContext.CompanyID;
                windowViewModel.Populate = true;
                windowViewModel.IsInsert = true;
                windowViewModel.IsReadonly = false;
                windowViewModel.Causals = _dataContext.GetCAUCONT("N");
                windowViewModel.Data = new PNTESTATA
                {
                    N1SOCI = _dataContext.CompanyID,
                    N1ANNO = dtpYear.SelectedValue?.Year ?? 0,
                    pnvcod = "UIC",
                    N1TmpPNBool = false,
                    N1DARE = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime().Date,
                    pnvdiv = "EUR",
                    N1FLCF = null
                };

                var wPNTESTATA = new PNTESTATAWindow(windowViewModel);
                wPNTESTATA.Owner = Window.GetWindow(this);
                while (wPNTESTATA.ShowDialog() == true)
                {
                    var model = wPNTESTATA.DataContext as PNTESTATAWindowViewModel;

                    if (model != null)
                    {
                        var newData = new PNTESTATA()
                        {
                            N1SOCI = _dataContext.CompanyID,
                            N1ANNO = dtpYear.SelectedValue?.Year ?? 0,
                            pnvcod = "UIC",
                            N1DARE = model.Data.N1DARE,
                            pnvdiv = "EUR",
                            N1FLCF = model.Data.N1FLCF,
                            N1CLFO = model.Data.N1CLFO,
                            N1TmpPNBool = model.Data.N1TmpPNBool,
                            Amount = model.Data.Amount,
                            N1docn = model.Data.N1docn,
                            N1docd = model.Data.N1docd,
                            N1rifn = model.Data.N1rifn,
                            N1rifd = model.Data.N1rifd
                        };
                        model.Data = newData;
                        wPNTESTATA = new PNTESTATAWindow(model);
                        wPNTESTATA.Owner = Window.GetWindow(this);
                    }
                }
                if (wPNTESTATA.ShowDialog() == true)
                    LoadData();
            }

        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNImportWindowViewModel>();

            var window = new PNImportWindow(windowViewModel);
            window.ShowDialog();

            LoadData();
        }

        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var selected = (sender as Button)!.DataContext as PNTESTATA;

            if (selected != null)
            {
                var model = _dataContext.GetPNTESTATA(selected.N1ANNO, selected.N1REGI);

                if (model != null)
                {
                    var causals = _dataContext.GetCAUCONT("*");
                    var codes = !string.IsNullOrWhiteSpace(model.N1FLCF) ? _dataContext.GetABE(model.N1FLCF) : null;

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNTESTATAWindowViewModel>();
                    windowViewModel.CompanyID = _dataContext.CompanyID;
                    windowViewModel.Populate = true;
                    windowViewModel.Data = model;
                    windowViewModel.IsInsert = false;
                    windowViewModel.IsReadonly = _dataContext.IsReadOnly(model.N1ANNO, model.N1REGI);
                    windowViewModel.Causals = causals;
                    windowViewModel.SelectedCausal = causals?.Where(w => w.caucod == model.pncaus).FirstOrDefault();
                    windowViewModel.Codes = !string.IsNullOrWhiteSpace(model.N1FLCF) ? codes : null;
                    windowViewModel.SelectedEntity = !string.IsNullOrWhiteSpace(model.N1FLCF) ? codes?.Where(w => w.abecod == model.N1CLFO).FirstOrDefault() : null;

                    var wPNTESTATA = new PNTESTATAWindow(windowViewModel);
                    wPNTESTATA.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wPNTESTATA.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdDetails_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var selected = (sender as Button)!.DataContext as PNTESTATA;

            if (selected != null)
            {
                selected = _dataContext.GetPNTESTATA(selected.N1ANNO, selected.N1REGI);

                if (selected != null)
                {
                    var causals = _dataContext.GetCAUCONT("*");
                    var codes = !string.IsNullOrWhiteSpace(selected.N1FLCF) ? _dataContext.GetABE(selected.N1FLCF) : null;

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNRIGHEWindowViewModel>();
                    windowViewModel.Head = selected;
                    windowViewModel.HeadSelectedCausal = causals?.Where(w => w.caucod == selected.pncaus).FirstOrDefault();
                    windowViewModel.IsInsert = false;

                    var wPNRIGHE = new PNRIGHEWindow(windowViewModel);
                    wPNRIGHE.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wPNRIGHE.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as PNTESTATA;

            if (item != null && !(_dataContext.CheckPrinted(item.N1SOCI, item.N1ANNO, item.N1REGI)))
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminazione della registrazione {item.N1ANNO}/{item.N1REGI} ?"))
                {
                    if (_dataContext.Delete(item))
                        LoadData();
                    else
                        ErrorHandler.Validation("Errore imprevisto durante l'eliminazione");
                }
            }
        }
        #endregion

        #region Grid things
        private void rgPrint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as RadGlyph)!.DataContext as PNTESTATA;
            if (item != null)
            {
                item = _dataContext.GetPNTESTATA(item.N1ANNO, item.N1REGI);

                if (item != null)
                {
                    var reportData = _dataContext.GetPrintAccountingRecord(item);
                    if (reportData != null)
                        ReportingHandler.PrintPDF(UserContext.Instance.Domain!, Constants.MODULE_ACCOUNTING, Constants.REPORT_TYPE_ACCOUNTING_RECORD, _dataContext.CompanyID, reportData, $"Registrazione n.{item.PrintFullID}", item.PrintFilename, true);
                    else
                        ErrorHandler.Show($"Impossibile trovare la registrazione {item.N1ANNO}/{item.N1REGI}");
                }
            }
        }

        private void rgDuplicate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)!.DataContext as PNTESTATA;
            if (item != null)
            {
                item = _dataContext.GetPNTESTATA(item.N1ANNO, item.N1REGI);

                if (item != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNDuplicateWindowViewModel>();
                    windowViewModel.HeadYear = item.N1ANNO;
                    windowViewModel.HeadNumber = item.N1REGI;
                    windowViewModel.Date = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                    var wPnDuplicate = new PNDuplicateWindow(windowViewModel);
                    wPnDuplicate.Owner = Window.GetWindow(this);

                    if (wPnDuplicate.ShowDialog() == true && wPnDuplicate.Tag != null)
                    {
                        var tag = wPnDuplicate.Tag as Tuple<string,int,int>;

                        if (tag != null)
                        {
                            var selected = _dataContext.GetPNTESTATA(tag.Item2,tag.Item3);

                            if (selected != null)
                            {
                                var causals = _dataContext.GetCAUCONT("*");
                                var codes = !string.IsNullOrWhiteSpace(selected.N1FLCF) ? _dataContext.GetABE(selected.N1FLCF) : null;

                                var rowWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PNRIGHEWindowViewModel>();
                                rowWindowViewModel.Head = selected;
                                rowWindowViewModel.HeadSelectedCausal = causals?.Where(w => w.caucod == selected.pncaus).FirstOrDefault();
                                rowWindowViewModel.IsInsert = false;

                                var wPNRIGHE = new PNRIGHEWindow(rowWindowViewModel);
                                wPNRIGHE.Owner = Window.GetWindow(this);
                                Mouse.OverrideCursor = null;
                                if (wPNRIGHE.ShowDialog() == true)
                                    LoadData();
                            }
                            LoadData();
                        }
                    }
                }
            }
        }

        private void dtpYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isFirstLoad)
            {
                _isFirstLoad = false;
                return;
            }

            LoadData();
            LoadSituation();
        }
        #endregion

        #region Situation
        private void Account_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is TextBlock)
            {
                var data = (e.Source as TextBlock)!.Tag as ASItem;

                if (data != null)
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    var result = _dataContext.GetAccountingSituationDetail(data.GroupID, data.IsDare, data.AccountID);

                    if (result != null)
                    {
                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<AccountingDetailWindowViewModel>();
                        windowViewModel.Data = result;
                        windowViewModel.Title = $"Dettaglio sottoconti per {data.GroupID} {data.AccountID}";

                        var wDetails = new AccountingDetailWindow(windowViewModel);
                        wDetails.Owner = Window.GetWindow(this);
                        Mouse.OverrideCursor = null;
                        wDetails.ShowDialog();
                    }
                }
            }
        }

        private void cmdExportBalance_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Title = "Seleziona la cartella di destinazione"
            };

            if (folderDialog.ShowDialog() == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var year = dtpYear.SelectedValue?.Year ?? 0;
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                var filename = $"{folderDialog.FolderName}\\{_dataContext.CompanyID}_{year}_{now.Year}{now.Month}{now.Day}_{now.Hour}{now.Minute}{now.Second}.txt";
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    string? file = _dataContext.ExportCEEBalance(year);

                    if (!string.IsNullOrEmpty(file))
                    {
                        File.WriteAllBytes(filename, Encoding.UTF8.GetBytes(file));
                        if (ConfirmHandler.Confirm($"Esportazione avvenuta correttamente nel file\n{filename}\n\nSi desidera aprire il file generato ?"))
                        {
                            var proc = new ProcessStartInfo(filename);
                            proc.UseShellExecute = true;
                            Process.Start(proc);
                        }
                    }
                }
                Mouse.OverrideCursor = null;
            }
        }

        private void acCC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadSituation();
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

        #region Custom loading
        private void rdpGrid_PageIndexChanged(object sender, PageIndexChangedEventArgs e)
        {
            if (e.OldPageIndex != -1)
                LoadData();
        }

        private void GridView_Filtering(object sender, Telerik.Windows.Controls.GridView.GridViewFilteringEventArgs e)
        {
            TelerikControls.FilterManager(_currentWhere, e);
            LoadData();
        }

        private void GridView_Sorting(object sender, GridViewSortingEventArgs e)
        {
            if (e.OldSortingState == SortingState.None)
            {
                e.NewSortingState = SortingState.Ascending;
            }
            else if (e.OldSortingState == SortingState.Ascending)
            {
                e.NewSortingState = SortingState.Descending;
            }
            else
            {
                e.NewSortingState = SortingState.None;
            }
            TelerikControls.SortManager(_currentSort, e);

            LoadData();

            e.Cancel = true;
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void GridView_FieldFilterEditorCreated(object sender, Telerik.Windows.Controls.GridView.EditorCreatedEventArgs e)
        {
            var stringFilterEditor = e.Editor as StringFilterEditor;
            if (stringFilterEditor != null)
            {
                stringFilterEditor.MatchCaseVisibility = Visibility.Collapsed;
            }
        }

        private void OnRadGridViewFilterOperatorsLoading(object sender, Telerik.Windows.Controls.GridView.FilterOperatorsLoadingEventArgs e)
        {
            TelerikControls.CleanFilters(e);
        }
        #endregion

        #region Grid context menu
        private void cmPN_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridView.SelectedItem != null)
            {
                var item = GridView.SelectedItem as PNTESTATA;

                if (item != null)
                {
                    item = _dataContext.GetPNTESTATA(item.N1ANNO, item.N1REGI);

                    if (item != null)
                    {
                        rmiSelfInvoice.IsEnabled = !item.N1AUGE.HasValue && (item.AccountingCausal?.cauivaBool ?? false);

                        if (rmiSelfInvoice.IsEnabled)
                        {
                            rmiSelfInvoice.Header = $"Crea l'autofattura per la registrazione n. {item.PrintFullID}";
                        }
                        else
                        {
                            if (item.N1AUGE.HasValue)
                                rmiSelfInvoice.Header = $"Creata autofattura n. {item.N1AUAN}/{item.N1AUNU} il {item.N1AUGE.Value.ToString("dd/MM/yyyy HH:mm:ss")}";
                            else
                                rmiSelfInvoice.Header = $"Impossibile creare un'autofattura per questo tipo di registrazione (causale non valida)";
                        }
                    }
                }
            }
            else
            {
                rmiSelfInvoice.IsEnabled = false;
            }
        }

        private void rmiSelfInvoice_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridView.SelectedItem != null)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                var item = GridView.SelectedItem as PNTESTATA;

                if (item != null)
                {
                    item = _dataContext.GetPNTESTATA(item.N1ANNO, item.N1REGI);

                    if (item != null)
                    {
                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<AskSelfInvoiceWindowViewModel>();
                        windowViewModel.Head = item;

                        var wAsk = new AskSelfInvoiceWindow(windowViewModel);
                        wAsk.Owner = Window.GetWindow(this);
                        Mouse.OverrideCursor = null;

                        if (wAsk.ShowDialog() == true)
                            LoadData();
                    }
                }
            }
        }
        #endregion
    }
}
