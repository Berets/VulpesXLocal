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

namespace VulpesX.Modules.Default.Auth
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

            pbSMTPPassword.Password = !string.IsNullOrWhiteSpace(_dataContext.Details?.azpsw) ? CryptoHelper.CSDecrypt(_dataContext.Details.azpsw, UserContext.Instance!.PKKP, UserContext.Instance!.Domain!) : null;
            pbSMTPPasswordCRM.Password = !string.IsNullOrWhiteSpace(_dataContext.Details?.azusrcrmpsw) ? CryptoHelper.CSDecrypt(_dataContext.Details.azusrcrmpsw, UserContext.Instance!.PKKP!, UserContext.Instance!.Domain!) : null;
            pbSMTPPasswordSRM.Password = !string.IsNullOrWhiteSpace(_dataContext.Details?.azusrsrmpsw) ? CryptoHelper.CSDecrypt(_dataContext.Details.azusrsrmpsw, UserContext.Instance!.PKKP!, UserContext.Instance!.Domain!) : null;
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

                        _dataContext.Details!.azpsw = !string.IsNullOrWhiteSpace(pbSMTPPassword.Password) ? CryptoHelper.CSEncrypt(pbSMTPPassword.Password, UserContext.Instance!.PKKP, UserContext.Instance!.Domain!) : null;
                        _dataContext.Details!.azusrcrmpsw = !string.IsNullOrWhiteSpace(pbSMTPPasswordCRM.Password) ? CryptoHelper.CSEncrypt(pbSMTPPasswordCRM.Password, UserContext.Instance!.PKKP, UserContext.Instance!.Domain!) : null;
                        _dataContext.Details!.azusrsrmpsw = !string.IsNullOrWhiteSpace(pbSMTPPasswordSRM.Password) ? CryptoHelper.CSEncrypt(pbSMTPPasswordSRM.Password, UserContext.Instance!.PKKP, UserContext.Instance!.Domain!) : null;

                        if (_dataContext.IsDetailsInsert)
                            _dataContext.Details.addedUserID = _dataContext.UserID;
                        else
                            _dataContext.Details.updatedUserID = _dataContext.UserID;

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

        #region Email buttons
        private void cmdTestSMTP_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (!string.IsNullOrWhiteSpace(_dataContext.Details!.azsmtp) && _dataContext.Details.azsmtpport.HasValue &&
                !string.IsNullOrWhiteSpace(_dataContext.Details!.azuser) && !string.IsNullOrWhiteSpace(pbSMTPPassword.Password) &&
                _dataContext.Details.azusetls.HasValue)
            {
                if (NotifierHelper.CheckSMTPConnection(_dataContext.Details.azsmtp, _dataContext.Details.azsmtpport.Value, _dataContext.Details.azusetls.Value, _dataContext.Details.azuser, pbSMTPPassword.Password))
                {
                    Mouse.OverrideCursor = null;
                    InfoHandler.Show($"Collegamento con {_dataContext.Details.azsmtp}:{_dataContext.Details.azsmtpport} riuscito correttamente");
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation($"Collegamento con {_dataContext.Details.azsmtp}:{_dataContext.Details.azsmtpport} non riuscito");
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("Tutti i parametri sono necessari per effettuare una connessione");
            }
        }

        private void cmdTestEmail_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (!string.IsNullOrWhiteSpace(_dataContext.Details?.azsmtp) && _dataContext.Details.azsmtpport.HasValue &&
                !string.IsNullOrWhiteSpace(_dataContext.Details?.azuser) && !string.IsNullOrWhiteSpace(pbSMTPPassword.Password) &&
                _dataContext.Details.azusetls.HasValue)
            {
                if (NotifierHelper.CheckEmailAddresses(txtEmailAddress.Text))
                {
                    if (NotifierHelper.CheckSMTPConnection(_dataContext.Details.azsmtp, _dataContext.Details.azsmtpport.Value, _dataContext.Details.azusetls.Value, _dataContext.Details.azuser, pbSMTPPassword.Password))
                    {
                        Mouse.OverrideCursor = null;
                        var result = NotifierHelper.SendTestEmail(_dataContext.Details.azsmtp, _dataContext.Details.azsmtpport.Value, _dataContext.Details.azusetls.Value, _dataContext.Details.azuser, pbSMTPPassword.Password, txtEmailAddress.Text);
                        if (string.IsNullOrWhiteSpace(result))
                            InfoHandler.Show($"Email inviata correttamente");
                        else
                            ErrorHandler.Validation(result);
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation($"Collegamento con {_dataContext.Details.azsmtp}:{_dataContext.Details.azsmtpport} non riuscito");
                    }
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("Gli indirizzi email inseriti non sono validi, inserire uno o più indirizzi email validi, separati da , (virgola)");
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("Tutti i parametri sono necessari per effettuare un invio di prova");
            }
        }

        private void cmdTestSMTPCRM_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (!string.IsNullOrWhiteSpace(_dataContext.Details!.azsmtp) && _dataContext.Details.azsmtpport.HasValue &&
                !string.IsNullOrWhiteSpace(_dataContext.Details.azusrcrm) && !string.IsNullOrWhiteSpace(pbSMTPPasswordCRM.Password) &&
                _dataContext.Details.azusetls.HasValue)
            {
                if (NotifierHelper.CheckSMTPConnection(_dataContext.Details.azsmtp, _dataContext.Details.azsmtpport.Value, _dataContext.Details.azusetls.Value, _dataContext.Details.azusrcrm, pbSMTPPasswordCRM.Password))
                {
                    Mouse.OverrideCursor = null;
                    InfoHandler.Show($"Collegamento con {_dataContext.Details.azsmtp}:{_dataContext.Details.azsmtpport} riuscito correttamente");
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation($"Collegamento con {_dataContext.Details.azsmtp}:{_dataContext.Details.azsmtpport} non riuscito");
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("Tutti i parametri sono necessari per effettuare una connessione");
            }
        }

        private void cmdTestEmailCRM_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (!string.IsNullOrWhiteSpace(_dataContext.Details!.azsmtp) && _dataContext.Details.azsmtpport.HasValue &&
                !string.IsNullOrWhiteSpace(_dataContext.Details.azusrcrm) && !string.IsNullOrWhiteSpace(pbSMTPPasswordCRM.Password) &&
                _dataContext.Details.azusetls.HasValue)
            {
                if (NotifierHelper.CheckEmailAddresses(txtEmailAddressCRM.Text))
                {
                    if (NotifierHelper.CheckSMTPConnection(_dataContext.Details.azsmtp, _dataContext.Details.azsmtpport.Value, _dataContext.Details.azusetls.Value, _dataContext.Details.azusrcrm, pbSMTPPasswordCRM.Password))
                    {
                        Mouse.OverrideCursor = null;
                        var result = NotifierHelper.SendTestEmail(_dataContext.Details.azsmtp, _dataContext.Details.azsmtpport.Value, _dataContext.Details.azusetls.Value, _dataContext.Details.azusrcrm, pbSMTPPasswordCRM.Password, txtEmailAddressCRM.Text);
                        if (string.IsNullOrWhiteSpace(result))
                            InfoHandler.Show($"Email inviata correttamente");
                        else
                            ErrorHandler.Validation(result);
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation($"Collegamento con {_dataContext.Details.azsmtp}:{_dataContext.Details.azsmtpport} non riuscito");
                    }
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("Gli indirizzi email inseriti non sono validi, inserire uno o più indirizzi email validi, separati da , (virgola)");
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("Tutti i parametri sono necessari per effettuare un invio di prova");
            }
        }

        private void cmdTestSMTPSRM_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (!string.IsNullOrWhiteSpace(_dataContext.Details!.azsmtp) && _dataContext.Details.azsmtpport.HasValue &&
                !string.IsNullOrWhiteSpace(_dataContext.Details.azusrsrm) && !string.IsNullOrWhiteSpace(pbSMTPPasswordSRM.Password) &&
                _dataContext.Details.azusetls.HasValue)
            {
                if (NotifierHelper.CheckSMTPConnection(_dataContext.Details.azsmtp, _dataContext.Details.azsmtpport.Value, _dataContext.Details.azusetls.Value, _dataContext.Details.azusrsrm, pbSMTPPasswordSRM.Password))
                {
                    Mouse.OverrideCursor = null;
                    InfoHandler.Show($"Collegamento con {_dataContext.Details.azsmtp}:{_dataContext.Details.azsmtpport} riuscito correttamente");
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation($"Collegamento con {_dataContext.Details.azsmtp}:{_dataContext.Details.azsmtpport} non riuscito");
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("Tutti i parametri sono necessari per effettuare una connessione");
            }
        }

        private void cmdTestEmailSRM_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (!string.IsNullOrWhiteSpace(_dataContext.Details!.azsmtp) && _dataContext.Details.azsmtpport.HasValue &&
                !string.IsNullOrWhiteSpace(_dataContext.Details.azusrsrm) && !string.IsNullOrWhiteSpace(pbSMTPPasswordSRM.Password) &&
                _dataContext.Details.azusetls.HasValue)
            {
                if (NotifierHelper.CheckEmailAddresses(txtEmailAddressSRM.Text))
                {
                    if (NotifierHelper.CheckSMTPConnection(_dataContext.Details.azsmtp, _dataContext.Details.azsmtpport.Value, _dataContext.Details.azusetls.Value, _dataContext.Details.azusrsrm, pbSMTPPasswordSRM.Password))
                    {
                        Mouse.OverrideCursor = null;
                        var result = NotifierHelper.SendTestEmail(_dataContext.Details.azsmtp, _dataContext.Details.azsmtpport.Value, _dataContext.Details.azusetls.Value, _dataContext.Details.azusrsrm, pbSMTPPasswordSRM.Password, txtEmailAddressSRM.Text);
                        if (string.IsNullOrWhiteSpace(result))
                            InfoHandler.Show($"Email inviata correttamente");
                        else
                            ErrorHandler.Validation(result);
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation($"Collegamento con {_dataContext.Details.azsmtp}:{_dataContext.Details.azsmtpport} non riuscito");
                    }
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("Gli indirizzi email inseriti non sono validi, inserire uno o più indirizzi email validi, separati da , (virgola)");
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("Tutti i parametri sono necessari per effettuare un invio di prova");
            }
        }
        #endregion

        #region Lot template
        private void txtLotTemplate_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtLotTemplate.Text))
            {
                try
                {
                    _dataContext.LotPreview = _dataContext.GenerateLotID(txtLotTemplate.Text);
                }
                catch
                {
                    _dataContext.LotPreview = null;
                }
            }
        }

        #endregion

        private void cmdSelectEIPath_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog { InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) };
            if (folderDialog.ShowDialog() ?? false)
            {
                _dataContext.Details!.azeipath = folderDialog.FolderName;
            }
        }

        private void ImportGILAT_Checking(object sender, RoutedEventArgs e)
        {
            txtImportGILAT.IsEnabled = _dataContext.Details!.azimpgilat;
        }

        private void ImportBANCOLAT_Checking(object sender, RoutedEventArgs e)
        {
            txtImportBANCOLAT.IsEnabled = _dataContext.Details!.azimpbancolat;
        }

        private void grdUserRole_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            //if (e.EditAction == Telerik.Windows.Controls.GridView.GridViewEditAction.Commit)
            //{
            //    if (e.EditOperationType == Telerik.Windows.Controls.GridView.GridViewEditOperationType.Edit)
            //    {
            //        var role = e.EditedItem as AUTH_ACCESS_ROLES;

            //        if (role != null)
            //        {

            //            if (_dataContext.UpdateRole(role))
            //            {
            //                _dataContext.UsersRoles = new ObservableCollection<AUTH_ACCESS_ROLES>();
            //                _dataContext.UsersRoles = _dataContext.GetAUTH_ACCESS_ROLEs();
            //                InfoHandler.Show("Salvataggio effettuato correttamente");
            //            }
            //            else
            //            {
            //                ErrorHandler.Validation("Qualcosa è andato storto");
            //            }
            //        }
            //    }
            //}
        }

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
