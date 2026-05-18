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
using VulpesX.Shared;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Default.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for CATEGORIAView.xaml
    /// </summary>
    public partial class CLAZIONEView : UserControl
    {
        private CLAZIONEViewModel _dataContext;
        private int _selectedPage = 0;

        public CLAZIONEView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<CLAZIONEViewModel>();

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
            var item = (sender as Button)!.DataContext as CLAZIONE;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CLAZIONEWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wCATEGORIA = new CLAZIONEWindow(windowViewModel);
                if (wCATEGORIA.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as CLAZIONE;
            MessageBox.Show($"{item!.csfcod} ");
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CLAZIONEWindowViewModel>();
            windowViewModel.Data = new CLAZIONE {  csfcod = string.Empty,  csfdes = string.Empty };
            windowViewModel.IsInsert = true;

            var wCATEGORIA = new CLAZIONEWindow(windowViewModel);
            wCATEGORIA.ShowDialog();
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
