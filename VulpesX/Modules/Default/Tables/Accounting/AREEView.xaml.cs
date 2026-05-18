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
    /// Interaction logic for AREEView.xaml
    /// </summary>
    public partial class AREEView : UserControl
    {
        private AREEViewModel _dataContext;
        private int _selectedPage = 0;

        public AREEView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<AREEViewModel>();

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
            var item = (sender as Button)!.DataContext as AREE;

            if (item != null)
            {
                var areeWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<AREEWindowViewModel>();
                areeWindowViewModel.Data = item;
                areeWindowViewModel.IsInsert = false;

                var wAREE = new AREEWindow(areeWindowViewModel);
                if (wAREE.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as AREE;
            MessageBox.Show($"{item!.arecod} ");
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var areeWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<AREEWindowViewModel>();
            areeWindowViewModel.Data = new AREE { arecod = string.Empty, aredes = string.Empty };
            areeWindowViewModel.IsInsert = true;

            var wAREE = new AREEWindow(areeWindowViewModel);
            wAREE.ShowDialog();
            LoadData();
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)!.Execute(txtSearch.Text, GridView);
        }
        #endregion
    }
}
