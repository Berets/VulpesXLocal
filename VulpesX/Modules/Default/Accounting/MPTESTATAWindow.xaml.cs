using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using QRClassLibrary;
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
using VulpesX.DAL;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for MPTESTATAWindow.xaml
    /// </summary>
    public partial class MPTESTATAWindow : FluentDefaultWindow
    {
        private MPTESTATAWindowViewModel _dataContext;
        public MPTESTATAWindow(MPTESTATAWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.Types = _dataContext.GetMANDATOs();
            _dataContext.Banks = _dataContext.GetBANAZIENs();
            _dataContext.SuppliersCache = _dataContext.GetSuppliersCache();

            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIOs();


            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedBank = _dataContext.Banks?.Where(o => o.abiabi == _dataContext.Data.MPABI && o.abicab == _dataContext.Data.MPCAB).FirstOrDefault();
                _dataContext.SelectedType = _dataContext.Types?.Where(o => o.mancod == _dataContext.Data.MPTIPO).FirstOrDefault();

                _dataContext.Rows = _dataContext.GetMPDETTAGLIOs() ?? new System.Collections.ObjectModel.ObservableCollection<MPDETTAGLIO>();

                foreach (var row in _dataContext.Rows)
                {
                    row.EntitiesList = _dataContext.SuppliersCache;
                }
            }
            else
            {
                _dataContext.Rows = new System.Collections.ObjectModel.ObservableCollection<MPDETTAGLIO>();
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validate = _dataContext.Validation();

            if (string.IsNullOrEmpty(validate))
            {
                var result = _dataContext.Save();

                if (!string.IsNullOrEmpty(result))
                {
                    InfoHandler.Show($@"{(_dataContext.IsInsert ? "Creato" : "Aggiornato")} mandato di pagamento N° {result}");

                    this.Close();
                }
                else
                {
                    ErrorHandler.Validation("Errore imprevisto, contattare GxItalia");
                }
            }
            else
            {
                ErrorHandler.Validation(validate);
            }
        }

        private void acType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedType != null)
            {
                _dataContext.Data.MPTIPO = _dataContext.SelectedType.mancod;
            }
            else
            {
                _dataContext.Data.MPTIPO = null;
            }
        }

        private void acInternalBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedBank != null)
            {
                _dataContext.Data.MPABI = _dataContext.SelectedBank.abiabi;
                _dataContext.Data.MPCAB = _dataContext.SelectedBank.abicab;

                _dataContext.Data.MPCCOR = _dataContext.SelectedBank.abicon;

                _dataContext.Data.MPGRUP = _dataContext.SelectedBank.abigba;
                _dataContext.Data.MPCONT = _dataContext.SelectedBank.abicba;
                _dataContext.Data.MPSOTT = _dataContext.SelectedBank.abisba;

                _dataContext.Data.MPBIC = _dataContext.SelectedBank.abibic;
                _dataContext.Data.MPCIN = _dataContext.SelectedBank.abicin;
                _dataContext.Data.MPIBAN = _dataContext.SelectedBank.abibiba;
                _dataContext.Data.MPBBAN = _dataContext.SelectedBank.abibba;
            }
            else
            {
                _dataContext.Data.MPABI = null;
                _dataContext.Data.MPCAB = null;

                _dataContext.Data.MPCCOR = null;
                _dataContext.Data.MPGRUP = null;
                _dataContext.Data.MPCONT = null;
                _dataContext.Data.MPSOTT = null;
                _dataContext.Data.MPBIC = null;
                _dataContext.Data.MPCIN = null;
                _dataContext.Data.MPIBAN = null;
                _dataContext.Data.MPBBAN = null;
            }
        }

        private void acType_LostFocus(object sender, RoutedEventArgs e)
        {
            var autoCompleteBox = sender as RadAutoCompleteBox;

            if (autoCompleteBox != null)
            {
                if (autoCompleteBox.SelectedItem == null)
                {
                    autoCompleteBox.SearchText = null;
                    _dataContext.SelectedType = null;
                }
            }
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


        private void rgvRows_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {

        }

        private void rgvRows_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {

        }

        private void rgvRows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var row = e.Row.DataContext as MPDETTAGLIO;

            if (row != null)
            {
                if (row.SelectedEntity == null)
                    e.IsValid = false;

                if(!row.M3IMEU.HasValue)
                    e.IsValid = false;
            }
        }


        private void rgvRows_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            var items = rgvRows.Items?.Cast<MPDETTAGLIO>();

            var data = new MPDETTAGLIO
            {

                MPSOCI = _dataContext.Data.MPSOCI,
                MPANNO = _dataContext.Data.MPANNO,
                MPNUME = _dataContext.Data.MPNUME,
                MPPOSI = items != null ? (items.Any() ? items.Max(max => max.MPPOSI) + 1 : 1) : 1,

                EntitiesList = _dataContext.SuppliersCache,
            };

            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;

            grid.CurrentColumn = grid.Columns[1];
        }

        private void rgSelectEC_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_dataContext.Data.IsEnabled)
            {
                var item = (sender as RadGlyph)!.DataContext as MPDETTAGLIO;
                if (item != null)
                {
                    if (item.SelectECVisibility)
                    {
                        rgvRows.CommitEdit();

                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<SelectECWindowViewModel>();
                        windowViewModel.EntityType = "F";
                        windowViewModel.EntityID = item.SelectedEntity?.abecod ?? 0;
                        windowViewModel.ExcludeInPaymentExecution = true;

                        var wSelect = new SelectECWindow(windowViewModel);
                        wSelect.Owner = Window.GetWindow(this);

                        if (wSelect.ShowDialog() == true && wSelect.Tag != null)
                        {
                            var selectedItems = wSelect.Tag as List<MastrinoECReportItem>;

                            if (selectedItems != null && selectedItems.Count > 0)
                            {
                                bool first = true;
                                foreach (var selected in selectedItems)
                                {
                                    if (first)
                                    {
                                        item.M3DOCU = selected.DocumentID;
                                        item.M3DADO = selected.DocumentDate;
                                        item.M3RIFE = selected.ReferenceID;
                                        item.M3DARI = selected.ReferenceDate;
                                        item.M3SEGN = selected.PNFORNITORI!.N3SEGN;
                                        item.M3IMEU = selected.Dare > 0 ? selected.Dare : selected.Avere;
                                        item.M3SCAD = selected.ExpireDate;
                                        item.M3SOCI = selected.PNFORNITORI!.N3SOCI;
                                        item.M3ANNO = selected.PNFORNITORI!.N3ANNO;
                                        item.M3REGI = selected.PNFORNITORI!.N3REGI;
                                        item.M3RIGA = selected.PNFORNITORI!.N3RIGA;
                                        item.M3DARI = selected.PNFORNITORI!.N3DARI;
                                        item.M3RIFE = selected.PNFORNITORI!.N3RIFE;
                                        item.M3DOCU = selected.PNFORNITORI!.N3DOCU;
                                        item.M3DARE = selected.PNFORNITORI!.N3DARE;
                                        item.M3DADO = selected.PNFORNITORI!.N3DADO;
                                        item.M3CAUS = selected.PNFORNITORI!.N3CAUS;
                                        item.M3GRUP = selected.PNFORNITORI!.N3GRUP;
                                        item.M3CONT = selected.PNFORNITORI!.N3CONT;
                                        item.M3SSOC = selected.PNFORNITORI!.N3SSOC;
                                        item.M3SOTT = selected.PNFORNITORI!.N3SOTT;
                                        item.M3IMPO = selected.PNFORNITORI!.N3IMPO;
                                        item.M3DESC = selected.PNFORNITORI!.N3DESC;
                                        item.M3RATA = selected.PNFORNITORI!.N3RATA;
                                        item.M3SCAD = selected.PNFORNITORI!.N3SCAD;
                                        item.M3PAGA = selected.PNFORNITORI!.N3PAGA;
                                        item.M3PARE = selected.PNFORNITORI!.N3PARE;
                                        item.M3PRAT = selected.PNFORNITORI!.N3PRAT;
                                        item.M3DEST = selected.PNFORNITORI!.N3DEST;
                                        item.M3PAXI = selected.PNFORNITORI!.N3PAXI;
                                        item.M3INIZ = selected.PNFORNITORI!.N3INIZ;
                                        item.M3CAMB = selected.PNFORNITORI!.N3CAMB;
                                        item.M3VALU = selected.PNFORNITORI!.N3VALU;
                                        item.M3DIVI = selected.PNFORNITORI!.N3DIVI;
                                        item.M3vcod = selected.PNFORNITORI!.n3vcod;
                                        item.M3FLPA = selected.PNFORNITORI!.N3FLPA;
                                        item.M3DVAL = selected.PNFORNITORI!.N3DVAL;
                                        item.M3ABIF = selected.PNFORNITORI!.N3ABIF;
                                        item.M3CABF = selected.PNFORNITORI!.N3CABF;
                                        item.M3IMEU = selected.PNFORNITORI!.N3IMEU;
                                        item.M3TIDO = selected.PNFORNITORI!.N3TIDO;
                                        item.M3RIOR = selected.PNFORNITORI!.n3rior;
                                        item.M3FL01 = selected.PNFORNITORI!.N3FL01;
                                        item.M3FLCO = string.Empty;
                                        item.M3FLES = string.Empty;
                                        item.M3RSOC = string.Empty;
                                        item.M3RANN = null;
                                        item.M3RREG = null;
                                        item.M3RRIG = null;
                                        item.M3IMAB = 0;
                                        item.M3EUAB = 0;
                                        item.M3SEGA = string.Empty;
                                        item.M3VAAB = 0;
                                        item.m3numef = null;

                                        rgvRows.CommitEdit();

                                        first = false;
                                    }
                                    else
                                    {
                                        var newRow = new MPDETTAGLIO
                                        {
                                            MPSOCI = _dataContext.Data.MPSOCI,
                                            MPANNO = _dataContext.Data.MPANNO,
                                            MPNUME = _dataContext.Data.MPNUME,
                                            MPPOSI = _dataContext.Rows.Any() ? _dataContext.Rows.Max(max => max.MPPOSI) + 1 : 1,
                                            M3SOCI = selected.PNFORNITORI!.N3SOCI,
                                            M3ANNO = selected.PNFORNITORI!.N3ANNO,
                                            M3REGI = selected.PNFORNITORI!.N3REGI,
                                            M3RIGA = selected.PNFORNITORI!.N3RIGA,
                                            M3DARI = selected.PNFORNITORI!.N3DARI,
                                            M3RIFE = selected.PNFORNITORI!.N3RIFE,
                                            M3DOCU = selected.PNFORNITORI!.N3DOCU,
                                            M3DARE = selected.PNFORNITORI!.N3DARE,
                                            M3DADO = selected.PNFORNITORI!.N3DADO,
                                            M3CAUS = selected.PNFORNITORI!.N3CAUS,
                                            M3GRUP = selected.PNFORNITORI!.N3GRUP,
                                            M3CONT = selected.PNFORNITORI!.N3CONT,
                                            M3SSOC = selected.PNFORNITORI!.N3SSOC,
                                            M3SOTT = selected.PNFORNITORI!.N3SOTT,
                                            M3IMPO = selected.PNFORNITORI!.N3IMPO,
                                            M3DESC = selected.PNFORNITORI!.N3DESC,
                                            M3SEGN = selected.PNFORNITORI!.N3SEGN,
                                            M3RATA = selected.PNFORNITORI!.N3RATA,
                                            M3SCAD = selected.PNFORNITORI!.N3SCAD,
                                            M3PAGA = selected.PNFORNITORI!.N3PAGA,
                                            M3PARE = selected.PNFORNITORI!.N3PARE,
                                            M3PRAT = selected.PNFORNITORI!.N3PRAT,
                                            M3DEST = selected.PNFORNITORI!.N3DEST,
                                            M3PAXI = selected.PNFORNITORI!.N3PAXI,
                                            M3INIZ = selected.PNFORNITORI!.N3INIZ,
                                            M3CAMB = selected.PNFORNITORI!.N3CAMB,
                                            M3VALU = selected.PNFORNITORI!.N3VALU,
                                            M3DIVI = selected.PNFORNITORI!.N3DIVI,
                                            M3vcod = selected.PNFORNITORI!.n3vcod,
                                            M3FLPA = selected.PNFORNITORI!.N3FLPA,
                                            M3DVAL = selected.PNFORNITORI!.N3DVAL,
                                            M3ABIF = selected.PNFORNITORI!.N3ABIF,
                                            M3CABF = selected.PNFORNITORI!.N3CABF,
                                            M3IMEU = selected.PNFORNITORI!.N3IMEU,
                                            M3TIDO = selected.PNFORNITORI!.N3TIDO,
                                            M3RIOR = selected.PNFORNITORI!.n3rior,
                                            M3FL01 = selected.PNFORNITORI!.N3FL01,
                                            M3FLCO = string.Empty,
                                            M3FLES = string.Empty,
                                            M3RSOC = string.Empty,
                                            M3RANN = null,
                                            M3RREG = null,
                                            M3RRIG = null,
                                            M3IMAB = 0,
                                            M3EUAB = 0,
                                            M3SEGA = string.Empty,
                                            M3VAAB = 0,
                                            m3numef = null,

                                            EntitiesList = _dataContext.SuppliersCache,
                                        };

                                        _dataContext.Rows.Add(newRow);
                                        rgvRows.CommitEdit();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void rgvRows_Deleted(object sender, GridViewDeletedEventArgs e)
        {

        }


    }
}
