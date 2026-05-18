using Microsoft.Extensions.DependencyInjection;
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
using VulpesX.DAL;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Treasury;

namespace VulpesX.Modules.Default.Treasury
{
    /// <summary>
    /// Interaction logic for CommitmentsView.xaml
    /// </summary>
    public partial class CommitmentView : UserControl
    {
        private CommitmentViewModel _dataContext;
        public CommitmentView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<CommitmentViewModel>();

            InitializeComponent();

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
        }

        #region CRUD buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as TES_IMFI;

            if (item != null)
            {
                if (!string.IsNullOrWhiteSpace(item.ifgrup))
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CommitmentWindowViewModel>();
                    windowViewModel.Data = item;
                    windowViewModel.IsInsert = false;

                    var wTES_IMFI = new CommitmentWindow(windowViewModel);
                    if (wTES_IMFI.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as TES_IMFI;

            if (item != null)
            {
                if (ConfirmHandler.Confirm("Confermate l'eliminazione del dato selezionato ?"))
                {
                    _dataContext.Delete(item);
                    LoadData();
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            bool? stay = true;

            CommitmentWindowViewModel? last = null;

            while (stay == true)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CommitmentWindowViewModel>();
                windowViewModel.Data = new TES_IMFI()
                {
                    ifsoci = _dataContext.CompanyID,
                    ifdata = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime(),
                    ifcont = string.Empty,
                    ifgrup = string.Empty,
                    ifsott = string.Empty,
                };
                windowViewModel.IsInsert = true;

                if (last != null)
                {
                    windowViewModel.LastReference = last.LastReference;
                    windowViewModel.LastDate = last.LastDate;
                    windowViewModel.LastAccountID = last.LastAccountID;
                    windowViewModel.LastSubaccountID = last.LastSubaccountID;
                    windowViewModel.LastGroupID = last.LastGroupID;
                }

                var wTES_IMFI = new CommitmentWindow(windowViewModel);
                stay = wTES_IMFI.ShowDialog();

                last = windowViewModel;
            }
            LoadData();
        }
        #endregion

        #region Accounting
        private void cmGrid_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridView.SelectedItem != null)
            {
                var item = GridView.SelectedItem as TES_IMFI;
                if (item != null)
                {
                    item = _dataContext.Get(item.ifgrup, item.ifcont, item.ifsott, item.ifdata);

                    if (item != null)
                    {
                        rmiAccounting.IsEnabled = !item.ifdaca.HasValue;
                        if (rmiAccounting.IsEnabled)
                            rmiAccounting.Header = $"Contabilizzazione impegno da {item.ifimpo.ToString("N2")} del {item.ifdata.Date.ToShortDateString()}";
                        else
                            rmiAccounting.Header = $"Impegno contabilizzato il {item.ifdaca?.Date} reg. n. {item.ifregann}/{item.ifregnum}";
                    }
                }
            }
            else
            {
                rmiAccounting.IsEnabled = false;
            }
        }

        private void cmGridNext_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridViewNear.SelectedItem != null)
            {
                var item = GridViewNear.SelectedItem as TES_IMFI;
                if (item != null)
                {
                    item = _dataContext.Get(item.ifgrup, item.ifcont, item.ifsott, item.ifdata);

                    if (item != null)
                    {
                        rmiAccountingNext.IsEnabled = !item.ifdaca.HasValue;
                        if (rmiAccountingNext.IsEnabled)
                            rmiAccountingNext.Header = $"Contabilizzazione impegno da {item.ifimpo.ToString("N2")} del {item.ifdata.Date.ToShortDateString()}";
                        else
                            rmiAccountingNext.Header = $"Impegno contabilizzato il {item.ifdaca?.Date} reg. n. {item.ifregann}/{item.ifregnum}";
                    }
                }
            }
            else
            {
                rmiAccountingNext.IsEnabled = false;
            }
        }

        private void rmiAccounting_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridView.SelectedItem != null)
            {
                var item = GridView.SelectedItem as TES_IMFI;
                if (item != null)
                {
                    item = _dataContext.Get(item.ifgrup, item.ifcont, item.ifsott, item.ifdata);

                    if (item != null)
                    {
                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<AskAccountingCommitmentWindowViewModel>();
                        windowViewModel.Item = item;

                        var wAskAccountingIMFI = new AskAccountingCommitmentWindow(windowViewModel);
                        wAskAccountingIMFI.Owner = Window.GetWindow(this);
                        if (wAskAccountingIMFI.ShowDialog() == true)
                            LoadData();
                    }
                }
            }
        }

        private void rmiAccountingNext_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridViewNear.SelectedItem != null)
            {
                var item = GridViewNear.SelectedItem as TES_IMFI;
                if (item != null)
                {
                    item = _dataContext.Get(item.ifgrup, item.ifcont, item.ifsott, item.ifdata);

                    if (item != null)
                    {
                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<AskAccountingCommitmentWindowViewModel>();
                        windowViewModel.Item = item;

                        var wAskAccountingIMFI = new AskAccountingCommitmentWindow(windowViewModel);
                        wAskAccountingIMFI.Owner = Window.GetWindow(this);
                        if (wAskAccountingIMFI.ShowDialog() == true)
                            LoadData();
                    }
                }
            }
        }
        #endregion

        #region Grids search
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }
        private void txtSearchNext_TextChanged(object sender, TextChangedEventArgs e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearchNext.Text, GridViewNear);
        }
        #endregion
    }
}
