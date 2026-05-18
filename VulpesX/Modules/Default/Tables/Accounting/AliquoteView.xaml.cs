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
    public partial class AliquoteView : UserControl
    {
        private AliquoteViewModel _dataContext;
        private int _selectedPage = 0;

        public AliquoteView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<AliquoteViewModel>();

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
            var item = (sender as Button)!.DataContext as ASSOGGETAMENTI;

            if (item != null)
            {
                if (!string.IsNullOrWhiteSpace(item.asscod) && !string.IsNullOrWhiteSpace(item.assali))
                {
                    var aliquoteWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<AliquoteWindowViewModel>();
                    aliquoteWindowViewModel.Data = item;
                    aliquoteWindowViewModel.IsInsert = false;

                    var wAliquote = new AliquoteWindow(aliquoteWindowViewModel);
                    if (wAliquote.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as ASSOGGETAMENTI;
            MessageBox.Show(item!.assdes);
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var aliquoteWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<AliquoteWindowViewModel>();
            aliquoteWindowViewModel.Data = new ASSOGGETAMENTI { assali = string.Empty, asscod = string.Empty, assdes = string.Empty };
            aliquoteWindowViewModel.IsInsert = true;

            var wAliquote = new AliquoteWindow(aliquoteWindowViewModel);
            wAliquote.ShowDialog();
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
