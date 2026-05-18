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
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Default.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for NUMREGView.xaml
    /// </summary>
    public partial class NUMREGView : UserControl
    {
        private NUMREGViewModel _dataContext;
        private int _selectedPage = 0;

        public NUMREGView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<NUMREGViewModel>();

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
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)!.DataContext as NUMREG;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<NUMREGWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wNUMREG = new NUMREGWindow(windowViewModel);
                wNUMREG.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wNUMREG.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as NUMREG;
            MessageBox.Show($"{item!.PERSOC} {item.PERCOD} {item.PERANN} ");
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<NUMREGWindowViewModel>();
            windowViewModel.Data = new NUMREG { PERSOC = _dataContext.CompanyID, PERCOD = string.Empty, PERANN = DateTime.Now.Year };
            windowViewModel.IsInsert = true;

            var wNUMREG = new NUMREGWindow(windowViewModel);
            wNUMREG.Owner = Window.GetWindow(this);
            Mouse.OverrideCursor = null;
            if (wNUMREG.ShowDialog() == true)
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
