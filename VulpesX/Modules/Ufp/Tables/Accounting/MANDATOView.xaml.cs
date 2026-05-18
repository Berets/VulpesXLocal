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
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Ufp.Tables.Accounting;

namespace VulpesX.Modules.Ufp.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for AGENTIView.xaml
    /// </summary>
    public partial class MANDATOView : UserControl
    {
        private MANDATOViewModel _dataContext;
        private int _selectedPage = 0;
        public MANDATOView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<MANDATOViewModel>();

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
            var item = (sender as Button)!.DataContext as MANDATO;

            if (item != null)
            {
                var agentiWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<MANDATOWindowViewModel>();
                agentiWindowViewModel.Data = item;
                agentiWindowViewModel.IsInsert = false;

                var wAGENTI = new MANDATOWindow(agentiWindowViewModel);
                if (wAGENTI.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as MANDATO;

            MessageBox.Show(item?.mandes);
        }


        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var agentiWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<MANDATOWindowViewModel>();
            agentiWindowViewModel.Data = new MANDATO { mancod = string.Empty };
            agentiWindowViewModel.IsInsert = true;

            var wAGENTI = new MANDATOWindow(agentiWindowViewModel);
            wAGENTI.ShowDialog();
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
