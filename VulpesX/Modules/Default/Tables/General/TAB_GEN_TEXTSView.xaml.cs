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
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.General;

namespace VulpesX.Modules.Default.Tables.General
{
    /// <summary>
    /// Interaction logic for TAB_GEN_TEXTSView.xaml
    /// </summary>
    public partial class TAB_GEN_TEXTSView : UserControl
    {
        private TAB_GEN_TEXTSViewModel _dataContext;
        private int _selectedPage = 0;

        public TAB_GEN_TEXTSView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<TAB_GEN_TEXTSViewModel>();

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

            cmbTypes.ItemsSource = TAB_GEN_TEXTS.TextTypes;
            cmbTypes.SelectedValue = TAB_GEN_TEXTS.OFFERS;

            LoadData();
        }

        private async void LoadData()
        {
            if (cmbTypes.SelectedValue != null)
            {
                await _dataContext.Load(cmbTypes.SelectedValue.ToString()!);
            }
        }

        private void cmbTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            var item = (sender as Button)!.DataContext as TAB_GEN_TEXTS;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TAB_GEN_TEXTSWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wTAB_GEN_TEXTS = new TAB_GEN_TEXTSWindow(windowViewModel);
                wTAB_GEN_TEXTS.Owner = Window.GetWindow(this);
                if (wTAB_GEN_TEXTS.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as TAB_GEN_TEXTS;
            if (item != null)
            {

                if (ConfirmHandler.Confirm($"Confermate l'eliminazione del testo\n{item.FullDescriptionSearchable}?"))
                {
                    _dataContext.Delete(item);
                    LoadData();
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TAB_GEN_TEXTSWindowViewModel>();
            windowViewModel.Data = new TAB_GEN_TEXTS { TTxsoc = _dataContext.CompanyID, TTxcod = string.Empty, TTxdes = string.Empty, TTXNote = string.Empty, TTXtip = cmbTypes.SelectedValue.ToString()! };
            windowViewModel.IsInsert = true;

            var wTAB_GEN_TEXTS = new TAB_GEN_TEXTSWindow(windowViewModel);
            wTAB_GEN_TEXTS.Owner = Window.GetWindow(this);
            if (wTAB_GEN_TEXTS.ShowDialog() == true)
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
