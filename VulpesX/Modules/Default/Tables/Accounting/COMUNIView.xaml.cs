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
    public partial class COMUNIView : UserControl
    {
        private COMUNIViewModel _dataContext;
        private int _selectedPage = 0;

        public COMUNIView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<COMUNIViewModel>();

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
            var item = (sender as Button)!.DataContext as COMUNI;

            if (item != null)
            {
                if (!string.IsNullOrWhiteSpace(item.comdes))
                {
                    var comuniWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<COMUNIWindowViewModel>();
                    comuniWindowViewModel.Data = item;
                    comuniWindowViewModel.IsInsert = false;

                    var wCOMUNI = new COMUNIWindow(comuniWindowViewModel);
                    wCOMUNI.ShowDialog();
                    LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as COMUNI;
            MessageBox.Show(item!.comdes);
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var comuniWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<COMUNIWindowViewModel>();
            comuniWindowViewModel.Data = new COMUNI { comdes = string.Empty };
            comuniWindowViewModel.IsInsert = true;

            var wCOMUNI = new COMUNIWindow(comuniWindowViewModel);
            wCOMUNI.ShowDialog();
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
