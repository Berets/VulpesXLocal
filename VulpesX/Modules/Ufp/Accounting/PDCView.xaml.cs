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
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Ufp.Accounting
{
    /// <summary>
    /// Interaction logic for PDCView.xaml
    /// </summary>
    public partial class PDCView : UserControl
    {
        private PDCViewModel _dataContext;

        public PDCView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<PDCViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                    LoadData();
            };

            LoadData();
        }

        private void LoadData()
        {
            dockDetails.Children.Clear();

            txtSearchGroups.Text = null;
            txtSearchAccounts.Text = null;
            txtSearchSubaccounts.Text = null;
            txtSearchYears.Text = null;

            _dataContext.Load();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // filters
            var clonedList = new ObservableCollection<PDCGRUPPI>();
            foreach (var grp in _dataContext.Items?.Where(w => (w.P1DES1 ?? string.Empty).ToLower().Contains(txtSearchGroups.Text.ToLower())).ToList() ?? new List<PDCGRUPPI>())
            {
                var newGrp = grp.Clones();
                newGrp!.Accounts = new ObservableCollection<PDCCONTI>();
                foreach (var acc in grp.Accounts?.Where(w => (w.P2DES1 ?? string.Empty).ToLower().Contains(txtSearchAccounts.Text.ToLower())).ToList() ?? new List<PDCCONTI>())
                {
                    var newAcc = acc.Clones();
                    newAcc!.Subaccounts = new ObservableCollection<PDCSOTTO>();
                    foreach (var sub in acc.Subaccounts?.Where(w => (w.P3DES1 ?? string.Empty).ToLower().Contains(txtSearchSubaccounts.Text.ToLower())).ToList() ?? new List<PDCSOTTO>())
                    {
                        var newSub = sub.Clones();
                        newSub!.Years = new ObservableCollection<PDCANNI>();
                        foreach (var yea in sub.Years?.Where(w => w.P4ANNO.ToString().Contains(txtSearchYears.Text.ToLower())).ToList() ?? new List<PDCANNI>())
                        {
                            var newYear = yea.Clones();
                            newSub.Years.Add(newYear!);
                        }
                        if (newSub.Years.Count > 0 || string.IsNullOrWhiteSpace(txtSearchYears.Text))
                            newAcc.Subaccounts.Add(newSub);
                    }
                    if (newAcc.Subaccounts.Count > 0 || string.IsNullOrWhiteSpace(txtSearchSubaccounts.Text))
                        newGrp.Accounts.Add(newAcc);
                }
                if (newGrp.Accounts.Count > 0 || string.IsNullOrWhiteSpace(txtSearchGroups.Text))
                    clonedList.Add(newGrp);
            }
            _dataContext.Items = clonedList;
        }

        private void treePDC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dockDetails.Children.Clear();
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is PDCGRUPPI)
                {
                    var pdcGruppiDetailViewModel = VulpesServiceProvider.Provider.GetRequiredService<PDCGruppiDetailViewModel>();
                    pdcGruppiDetailViewModel.Data = e.AddedItems[0] as PDCGRUPPI;
                    pdcGruppiDetailViewModel.IsInsert = false;

                    var uc = new PDCGruppiDetailView(pdcGruppiDetailViewModel);
                    uc.Saved += Uc_Saved;
                    dockDetails.Children.Add(uc);
                }
            }
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is PDCCONTI)
                {
                    var pdcContiDetailViewModel = VulpesServiceProvider.Provider.GetRequiredService<PDCContiDetailViewModel>();
                    pdcContiDetailViewModel.Data = e.AddedItems[0] as PDCCONTI;
                    pdcContiDetailViewModel.IsInsert = false;

                    var uc = new PDCContiDetailView(pdcContiDetailViewModel);
                    uc.Saved += Uc_Saved;
                    dockDetails.Children.Add(uc);
                }
            }
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is PDCSOTTO)
                {
                    var pdcSottoDetailViewModel = VulpesServiceProvider.Provider.GetRequiredService<PDCSottoDetailViewModel>();
                    pdcSottoDetailViewModel.Data = e.AddedItems[0] as PDCSOTTO;
                    pdcSottoDetailViewModel.IsInsert = false;

                    var uc = new PDCSottoDetailView(pdcSottoDetailViewModel);
                    uc.Saved += Uc_Saved;
                    dockDetails.Children.Add(uc);
                }
            }
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                if (e.AddedItems[0] is PDCANNI)
                {
                    var pdcAnniDetailViewModel = VulpesServiceProvider.Provider.GetRequiredService<PDCAnniDetailViewModel>();
                    pdcAnniDetailViewModel.Data = e.AddedItems[0] as PDCANNI;
                    pdcAnniDetailViewModel.IsInsert = false;

                    var uc = new PDCAnniDetailView(pdcAnniDetailViewModel);
                    uc.Saved += Uc_Saved;
                    dockDetails.Children.Add(uc);
                }
            }

            e.Handled = true;
        }

        private void Uc_Saved(object? sender, EventArgs e)
        {
            LoadData();
        }

        #region Context menù
        private void cmTree_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (treePDC.SelectedItem != null)
            {
                rmiAddGroup.IsEnabled = treePDC.SelectedItem is PDCGRUPPI;
                rmiDeleteGroup.IsEnabled = treePDC.SelectedItem is PDCGRUPPI;
                rmiAddAccount.IsEnabled = treePDC.SelectedItem is PDCGRUPPI;
                rmiDeleteAccount.IsEnabled = treePDC.SelectedItem is PDCCONTI;
                rmiAddSubaccount.IsEnabled = treePDC.SelectedItem is PDCCONTI;
                rmiDeleteSubaccount.IsEnabled = treePDC.SelectedItem is PDCSOTTO;
                rmiAddYear.IsEnabled = treePDC.SelectedItem is PDCSOTTO;
                rmiDeleteYear.IsEnabled = treePDC.SelectedItem is PDCANNI;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void rmiAddGroup_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            dockDetails.Children.Clear();

            var pdcGruppiDetailViewModel = VulpesServiceProvider.Provider.GetRequiredService<PDCGruppiDetailViewModel>();
            pdcGruppiDetailViewModel.Data = new PDCGRUPPI { P1GRUP = string.Empty, P1OBCP = "N" };
            pdcGruppiDetailViewModel.IsInsert = true;

            var uc = new PDCGruppiDetailView(pdcGruppiDetailViewModel);
            uc.Saved += Uc_Saved;
            dockDetails.Children.Add(uc);
        }

        private void rmiDeleteGroup_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = treePDC.SelectedItem as PDCGRUPPI;

            if (selected != null)
            {
                if (selected.Accounts?.Count == 0)
                {
                    if (ConfirmHandler.Confirm($"Confermate l'eliminazione del gruppo [{selected.FullDescriptionSearchable}] ?"))
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (_dataContext.CanDeleteGroup(selected))
                        {
                            _dataContext.DeleteGroup(selected);
                            LoadData();
                            Mouse.OverrideCursor = null;
                        }
                        else
                        {
                            Mouse.OverrideCursor = null;
                            ErrorHandler.Show("Impossibile eliminare questo gruppo perchè è utilizzato");
                        }
                    }
                }
                else
                {
                    ErrorHandler.Show("Impossibile eliminare un gruppo che contiene dei conti");
                }
            }
        }

        private void rmiAddAccount_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = treePDC.SelectedItem as PDCGRUPPI;

            dockDetails.Children.Clear();

            if (selected != null)
            {
                var pdcContiDetailViewModel = VulpesServiceProvider.Provider.GetRequiredService<PDCContiDetailViewModel>();
                pdcContiDetailViewModel.Data = new PDCCONTI { P1GRUP = selected.P1GRUP, P2CONT = string.Empty, p2flcf = null };
                pdcContiDetailViewModel.IsInsert = true;

                var uc = new PDCContiDetailView(pdcContiDetailViewModel);
                uc.Saved += Uc_Saved;
                dockDetails.Children.Add(uc);
            }
        }

        private void rmiDeleteAccount_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = treePDC.SelectedItem as PDCCONTI;

            if (selected != null)
            {
                if (selected.Subaccounts?.Count == 0)
                {
                    if (ConfirmHandler.Confirm($"Confermate l'eliminazione del conto [{selected.FullDescriptionSearchable}] ?"))
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (_dataContext.CanDeleteAccount(selected))
                        {
                            _dataContext.DeleteAccount(selected);
                            LoadData();
                            Mouse.OverrideCursor = null;
                        }
                        else
                        {
                            Mouse.OverrideCursor = null;
                            ErrorHandler.Show("Impossibile eliminare questo conto perchè è utilizzato");
                        }
                    }
                }
                else
                {
                    ErrorHandler.Show("Impossibile eliminare un conto che contiene dei sottoconti");
                }
            }
        }

        private void rmiAddSubaccount_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = treePDC.SelectedItem as PDCCONTI;
            dockDetails.Children.Clear();

            if (selected != null)
            {
                var pdcSottoDetailViewModel = VulpesServiceProvider.Provider.GetRequiredService<PDCSottoDetailViewModel>();
                pdcSottoDetailViewModel.Data = new PDCSOTTO { P1GRUP = selected.P1GRUP, P2CONT = selected.P2CONT, P3SOTC = string.Empty, P3CLFO = null };
                pdcSottoDetailViewModel.IsInsert = true;

                var uc = new PDCSottoDetailView(pdcSottoDetailViewModel);
                uc.Saved += Uc_Saved;
                dockDetails.Children.Add(uc);
            }
        }

        private void rmiDeleteSubaccount_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = treePDC.SelectedItem as PDCSOTTO;

            if (selected != null)
            {
                if (selected.Years?.Count == 0)
                {
                    if (ConfirmHandler.Confirm($"Confermate l'eliminazione del sottoconto [{selected.FullDescriptionSearchable}] ?"))
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        if (_dataContext.CanDeleteSubaccount(selected))
                        {
                            _dataContext.DeleteSubaccount(selected);
                            LoadData();
                            Mouse.OverrideCursor = null;
                        }
                        else
                        {
                            Mouse.OverrideCursor = null;
                            ErrorHandler.Show("Impossibile eliminare questo sottoconto perchè è utilizzato");
                        }
                    }
                }
                else
                {
                    ErrorHandler.Show("Impossibile eliminare un sottoconto che contiene degli anni");
                }
            }
        }

        private void rmiAddYear_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = treePDC.SelectedItem as PDCSOTTO;
            dockDetails.Children.Clear();

            if (selected != null)
            {
                var group = _dataContext.GetGroup(selected.P1GRUP);

                var pdcAnniDetailViewModel = VulpesServiceProvider.Provider.GetRequiredService<PDCAnniDetailViewModel>();
                pdcAnniDetailViewModel.Data = new PDCANNI { P1SOCI = _dataContext.CompanyID, P1GRUP = selected.P1GRUP, P2CONT = selected.P2CONT, P3SOTC = selected.P3SOTC, P1CCHI = group?.p1chco };
                pdcAnniDetailViewModel.IsInsert = true;

                var uc = new PDCAnniDetailView(pdcAnniDetailViewModel);
                uc.Saved += Uc_Saved;
                dockDetails.Children.Add(uc);
            }
        }

        private void rmiDeleteYear_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = treePDC.SelectedItem as PDCANNI;

            if (selected != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminazione dell'anno [{selected.P4ANNO}] ?"))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    if (_dataContext.CanDeleteYear(selected))
                    {
                        _dataContext.DeleteYear(selected);
                        LoadData();
                        Mouse.OverrideCursor = null;
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Show("Impossibile eliminare questo anno perchè è utilizzato");
                    }
                }
            }
        }
        #endregion
    }
}
