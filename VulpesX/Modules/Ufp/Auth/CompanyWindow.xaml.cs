using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using Telerik.Windows.Controls.TreeListView;
using VulpesX.Models;
using VulpesX.Models.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Auth;

namespace VulpesX.Modules.Ufp.Auth
{
    /// <summary>
    /// Interaction logic for CompanyWindow.xaml
    /// </summary>
    public partial class CompanyWindow : FluentDefaultWindow, IWindowFactory
    {
        private CompanyWindowViewModel _dataContext;

        public CompanyWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<CompanyWindowViewModel>();

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);
            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            _dataContext.SelectedISOExtraCEE = _dataContext.ISOs?.Where(w => w.isocod == _dataContext.Details?.azisoextracee).FirstOrDefault();
            _dataContext.SelectedTaxRegime = _dataContext.TaxRegimeList?.Where(w => w.regicod == _dataContext.Details?.azregifatt).FirstOrDefault();
            _dataContext.SelectedLegalState = _dataContext.States?.Where(w => w.cappro == _dataContext.Details?.azprsl).FirstOrDefault();
            _dataContext.SelectedLegalCity = _dataContext.Cities?.Where(w => w.comdes == _dataContext.Details?.azlosl?.Trim()).FirstOrDefault();
            _dataContext.SelectedWorkingState = _dataContext.States?.Where(w => w.cappro == _dataContext.Details?.azprsa).FirstOrDefault();
            _dataContext.SelectedWorkingCity = _dataContext.Cities?.Where(w => w.comdes == _dataContext.Details?.azlosa?.Trim()).FirstOrDefault();
            _dataContext.LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.CUSTOM_FOLDER}logo.png");
            _dataContext.CertsLogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.CUSTOM_FOLDER}certs.png");
        }

        private List<MenuModel> ExtractMenus(List<MenuModel> Menus)
        {
            var retValue = new List<MenuModel>();

            foreach (var menu in Menus)
            {
                if (!string.IsNullOrEmpty(menu.Uri))
                {
                    retValue.Add(menu);
                }

                if (menu.SubItems != null && menu.SubItems.Any())
                {
                    retValue.AddRange(ExtractMenus(menu.SubItems));
                }
            }

            return retValue;
        }

        private List<MenuModel> SelectMenus(List<MenuModel> Menus, bool IsSelect)
        {
            var retValue = new List<MenuModel>();

            foreach (var menu in Menus)
            {
                if (!string.IsNullOrEmpty(menu.Uri))
                {
                    menu.IsEnabled = IsSelect;
                }

                if (menu.SubItems != null && menu.SubItems.Any())
                {
                    retValue.AddRange(SelectMenus(menu.SubItems, IsSelect));
                }
            }

            return retValue;
        }


        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var validated = _dataContext.Validate();
            var validatedDetails = _dataContext.ValidateDetails();

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (string.IsNullOrWhiteSpace(validatedDetails))
                {
                    if (_dataContext.Update())
                    {
                        //LINGUA
                        _dataContext.SaveLingua();

                        if (_dataContext.Details != null)
                        {
                            if (_dataContext.IsDetailsInsert)
                                _dataContext.Details.addedUserID = _dataContext.UserID;
                            else
                                _dataContext.Details.updatedUserID = _dataContext.UserID;
                        }

                        if (_dataContext.IsDetailsInsert ? _dataContext.InsertAzienda() : _dataContext.UpdateAzienda())
                        {
                            // save logo
                            StorageHelper.Delete(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.CUSTOM_FOLDER}logo.png");
                            if (_dataContext.LogoData != null)
                                StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.CUSTOM_FOLDER}logo.png", _dataContext.LogoData);
                            // save certifications logo
                            StorageHelper.Delete(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.CUSTOM_FOLDER}certs.png");
                            if (_dataContext.CertsLogoData != null)
                                StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.CUSTOM_FOLDER}certs.png", _dataContext.CertsLogoData);
                            Mouse.OverrideCursor = null;
                            this.DialogResult = true;
                        }
                        else
                        {
                            Mouse.OverrideCursor = null;
                            ErrorHandler.Validation("Errore salvando AZIENDA");
                        }
                    }
                }
                else
                { Mouse.OverrideCursor = null; ErrorHandler.Validation(validatedDetails); }
            }
            else
            { Mouse.OverrideCursor = null; ErrorHandler.Validation(validated); }
        }

        private void imgLogo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var fileDialog = new OpenFileDialog() { Multiselect = false, Filter = "File immagine PNG|*.png" };
            if (fileDialog.ShowDialog() ?? false)
                _dataContext.LogoData = File.ReadAllBytes(fileDialog.FileName);
            else
                _dataContext.LogoData = null;
        }

        private void imgLogoCerts_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var fileDialog = new OpenFileDialog() { Multiselect = false, Filter = "File immagine PNG|*.png" };

            if (fileDialog.ShowDialog() ?? false)
                _dataContext.CertsLogoData = File.ReadAllBytes(fileDialog.FileName);
            else
                _dataContext.CertsLogoData = null;
        }
        #endregion

        #region Autocompletes
        private void acLegalState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedLegalState != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedLegalState.cappro))
            {
                _dataContext.Details!.azprsl = _dataContext.SelectedLegalState.cappro;
            }
            else
            { _dataContext.Details!.azprsl = null; }
        }
        private void acLegalCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedLegalCity != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedLegalCity.comdes))
            {
                _dataContext.Details!.azlosl = _dataContext.SelectedLegalCity.comdes;
            }
            else
            { _dataContext.Details!.azlosl = null; }
        }
        private void acWorkingCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedWorkingCity != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedWorkingCity.comdes))
            {
                _dataContext.Details!.azlosa = _dataContext.SelectedWorkingCity.comdes;
            }
            else
            { _dataContext.Details!.azlosa = null; }
        }
        private void acWorkingState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedWorkingState != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedWorkingState.cappro))
            {
                _dataContext.Details!.azprsa = _dataContext.SelectedWorkingState.cappro;
            }
            else
            { _dataContext.Details!.azprsa = null; }
        }
        private void acISO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedISOExtraCEE != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedISOExtraCEE.isocod))
            {
                _dataContext.Details!.azisoextracee = _dataContext.SelectedISOExtraCEE.isocod;
            }
            else
            { _dataContext.Details!.azisoextracee = null; }
        }
        private void acTaxRegime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedTaxRegime != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedTaxRegime.regicod))
            {
                _dataContext.Details!.azregifatt = _dataContext.SelectedTaxRegime.regicod;
            }
            else
            { _dataContext.Details!.azregifatt = null; }
        }
        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<System.Windows.Controls.TextBox>().First();
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

        private void cmbLingue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.RemovedItems.Count > 0)
            {
                _dataContext.SaveLingua();
            }

            if (_dataContext.SelectedLingua != null)
                _dataContext.SelectedAziendaLingua = _dataContext.GetAZIENDA_LINGUA(_dataContext.SelectedLingua.lincod) ?? new AZIENDA_LINGUA { AZCode = _dataContext.CompanyID, lincod = _dataContext.SelectedLingua.lincod };
        }

        private void cmbLingueDefault_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataContext.Lingue = new ObservableCollection<LINGUA>(_dataContext.GetLINGUAs()?.Where(o => o.lincod != _dataContext.Details?.azlinguadefault).ToList() ?? new());
            _dataContext.SelectedLingua = _dataContext.Lingue.FirstOrDefault();
        }

        #region User abilitations
        private void grdUsers_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            _dataContext.SelectedUser = grdUsers.SelectedItem as ACCESS;

            if (_dataContext.SelectedUser != null)
            {
                _dataContext.LoadUserMenu();
            }
        }

        private void rtlMenus_RowIsExpandedChanging(object sender, RowCancelEventArgs e)
        {
            if (!(e.Row as TreeListViewRow)?.IsExpanded ?? true)
                e.Cancel = true;
        }

        private void cmdSaveMenus_Click(object sender, RoutedEventArgs e)
        {
            var menus = ExtractMenus(rtlMenus.Items.Cast<MenuModel>().ToList());

            string menuJSON = JsonConvert.SerializeObject(menus.Select(s => new { s.Name, s.Uri, s.IsEnabled }));

            if (_dataContext.SelectedUser != null)
            {
                _dataContext.UpdateUserMenu(menuJSON);

                _dataContext.LoadUserMenu();
            }
        }

        private void btnMenuSelectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectMenus(rtlMenus.Items.Cast<MenuModel>().ToList(), true);
        }

        private void btnMenuDeselectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectMenus(rtlMenus.Items.Cast<MenuModel>().ToList(), false);
        }
        #endregion
    }
}
