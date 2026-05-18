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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VulpesX.Shared;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;
using VulpesX.WindowsFactory.Default.Tables.Accounting;

namespace VulpesX.Modules.Default.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for CAUCONTView.xaml
    /// </summary>
    public partial class CAUCONTView : UserControl
    {
        private CAUCONTViewModel _dataContext;
        private int _selectedPage = 0;

        public CAUCONTView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<CAUCONTViewModel>();

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
            var item = (sender as Button)!.DataContext as CAUCONT;

            if (item != null)
            {
                if (!string.IsNullOrWhiteSpace(item.caucod))
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CAUCONTWindowViewModel>();
                    windowViewModel.Data = item;
                    windowViewModel.IsInsert = false;

                    var wCAUCONT = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTWindowFactory>().Create(windowViewModel);
                    if (wCAUCONT.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as CAUCONT;
            MessageBox.Show(item!.caudes);
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CAUCONTWindowViewModel>();
            windowViewModel.Data = new CAUCONT
            {
                caucod = string.Empty,
                caudes = string.Empty,
                caugenBool = false,
                cauivaBool = false,
                caucliBool = false,
                cauforBool = false,
                causolBool = false
            };
            windowViewModel.IsInsert = true;

            var wCAUCONT = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTWindowFactory>().Create(windowViewModel);
            wCAUCONT.ShowDialog();
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
