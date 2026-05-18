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
using VulpesX.Modules.Default.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.ViewModels.Modules.Default.Tables.CRM;

namespace VulpesX.Modules.Default.Tables.CRM
{
    /// <summary>
    /// Interaction logic for TAB_CRM_FEASABILITYView.xaml
    /// </summary>
    public partial class TAB_CRM_FEASABILITYView : UserControl
    {
        private TAB_CRM_FEASABILITYViewModel _dataContext;
        private int _selectedPage = 0;

        public TAB_CRM_FEASABILITYView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<TAB_CRM_FEASABILITYViewModel>();

            InitializeComponent();
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

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as TAB_CRM_FEASABILITY;

            if (item != null)
            {

                var banazienWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TAB_CRM_FEASABILITYWindowViewModel>();
                banazienWindowViewModel.Data = item;
                banazienWindowViewModel.IsInsert = false;

                var wBANAZIEN = new TAB_CRM_FEASABILITYWindow(banazienWindowViewModel);
                if (wBANAZIEN.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as TAB_CRM_FEASABILITY;
            MessageBox.Show(item!.tdedes);
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var banazienWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TAB_CRM_FEASABILITYWindowViewModel>();
            banazienWindowViewModel.Data = new TAB_CRM_FEASABILITY { tdecod = string.Empty, tdedes = string.Empty };
            banazienWindowViewModel.IsInsert = true;

            var wBANAZIEN = new TAB_CRM_FEASABILITYWindow(banazienWindowViewModel);
            wBANAZIEN.ShowDialog();
            LoadData();
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        #endregion
    }
}
