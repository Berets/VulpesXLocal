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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.General;

namespace VulpesX.Modules.Default.General
{
    /// <summary>
    /// Interaction logic for ARTWindow.xaml
    /// </summary>
    public partial class ARTWindow : FluentDefaultWindow
    {
        private ARTWindowViewModel _dataContext;
        private bool? isInfiniteOriginal;

        public ARTWindow(ARTWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            _dataContext.SelectedLingua = _dataContext.Lingue?.FirstOrDefault();

            if (!_dataContext.IsInsert)
            {
                this.isInfiniteOriginal = _dataContext.Data.IsInfinite;
                _dataContext.ExternalCodes = _dataContext.GetExternals();
                _dataContext.PlantSources = _dataContext.GetSources();
                _dataContext.Aliquota = _dataContext.Aliquote?.Where(o => o.asscod == _dataContext.Data.asscod && o.assali == _dataContext.Data.assali).FirstOrDefault();
            }
            else
            {
                this.isInfiniteOriginal = null;
                _dataContext.ExternalCodes = new ObservableCollection<tab_articolo_extern>();
                _dataContext.PlantSources = new ObservableCollection<tab_articolo_produzione_sorgenti>();
            }

            this.Title = $"Dettagli articolo {(_dataContext.IsInsert ? "nuovo" : _dataContext.Data.ID)}";
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                var result = _dataContext.Save();

                if (result)
                {
                    this.DialogResult = true;
                }
            }
            else
            {
                ErrorHandler.Validation(validated);
            }
        }

        #region Sources grid events
        private void rgvPlantSources_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var data = new tab_articolo_produzione_sorgenti
            {
                SocietaID = _dataContext.CompanyID,
                ArticoloID = _dataContext.Data.ID,
                RisorsaID = string.Empty,
                SorgenteID = string.Empty,
            };
            data.RisorseList = _dataContext.RisorseList;
            data.AllSources = _dataContext.Allsources;

            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void rgvPlantSources_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.OldValues != null)
            {
                var item = e.Row.Item as tab_articolo_produzione_sorgenti;

                if (item != null)
                {
                    var validated = _dataContext.ValidateSource(item);
                    if (!string.IsNullOrWhiteSpace(validated))
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(validated); });
                        e.IsValid = false;
                    }
                    else
                    {
                        if ((rgvPlantSources.ItemsSource as ObservableCollection<tab_articolo_produzione_sorgenti>)?.Where(w => w.RisorsaID == item.SelectedRisorsa?.ID && w.SorgenteID == item.SelectedSorgente?.ID).Count() >= 1)
                        {
                            Dispatcher.BeginInvoke(() => { ErrorHandler.Validation("Il codice inserito è già in uso o non è valido"); });
                            e.IsValid = false;
                        }
                        else
                        {
                            e.IsValid = true;
                        }
                    }
                }
            }
            else
            {
                e.IsValid = true;
            }
        }

        private void rgvPlantSources_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            var data = e.Row.Item as tab_articolo_produzione_sorgenti;

            if (data != null)
            {
                data.RisorsaID = data.SelectedRisorsa?.ID ?? string.Empty;
                data.SorgenteID = data.SelectedSorgente?.ID ?? string.Empty;
            }
        }

        private void rgvPlantSources_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvPlantSources.ScrollIntoView(e.Row.Item, rgvPlantSources.Columns[0]);
        }
        #endregion

        #region Extern grid events
        private void rgvExtern_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var data = new tab_articolo_extern
            {
                extsoc = _dataContext.CompanyID,
                extpid = _dataContext.Data.ID,
                extcode = string.Empty,
                extid = string.Empty,
            };
            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void rgvExtern_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.OldValues != null)
            {
                var item = e.Row.Item as tab_articolo_extern;

                if (item != null)
                {
                    var validated = _dataContext.ValidateExternal(item);
                    if (!string.IsNullOrWhiteSpace(validated))
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(validated); });
                        e.IsValid = false;
                    }
                    else
                    {
                        if ((rgvExtern.ItemsSource as ObservableCollection<tab_articolo_extern>)?.Where(w => w.extcode.ToLower() == item.extcode.ToLower()).Count() > 1)
                        {
                            Dispatcher.BeginInvoke(() => { ErrorHandler.Validation("Il codice inserito è già in uso o non è valido"); });
                            e.IsValid = false;
                        }
                        else
                        {
                            e.IsValid = true;
                        }
                    }
                }
            }
            else
            {
                e.IsValid = true;
            }
        }

        private void rgvExtern_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvExtern.ScrollIntoView(e.Row.Item, rgvExtern.Columns[0]);
        }
        #endregion

        #region Autocompletes
        private void acAliquota_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.Aliquota != null && !string.IsNullOrWhiteSpace(_dataContext.Aliquota.asscod))
            {
                _dataContext.Data.asscod = _dataContext.Aliquota.asscod;
                _dataContext.Data.assali = _dataContext.Aliquota.assali;
            }
            else
            {
                _dataContext.Data.asscod = null;
                _dataContext.Data.assali = null;
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
        #endregion

        private void togInfinite_Checked(object sender, RoutedEventArgs e)
        {
            if (this.isInfiniteOriginal.HasValue && this.isInfiniteOriginal == false)
                InfoHandler.Show("ATTENZIONE, impostando l'articolo come sempre disponibile verranno eliminati tutti i lotti eventualmente in giacenza su tutti i magazzini!");
        }

        private void cmbLingua_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                _dataContext.InsertOrUpdateLanguage();
            }

            if (e.AddedItems.Count > 0)
            {
                var added = e.AddedItems[0] as LINGUA;

                if (added != null)
                {
                    _dataContext.SelectedTraduzione = _dataContext.GetLanguage(added.lincod) ?? new tab_articolo_lingua { SocietaID = _dataContext.CompanyID, ArticoloID = _dataContext.Data.ID, lincod = added.lincod };
                }
            }
        }
    }
}
