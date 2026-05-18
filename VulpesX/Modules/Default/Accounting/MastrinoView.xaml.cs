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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VulpesX.Shared;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for MastrinoView.xaml
    /// </summary>
    public partial class MastrinoView : UserControl
    {
        private MastrinoViewModel _dataContext;
        public MastrinoView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<MastrinoViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.GetCurrency();

            cmbYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbYear.SelectedItem = cmbYear.Items[0];

            this.PreviewKeyDown += async (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    await LoadData();
                }
            };
            this.Loaded += async (s, e) =>
            {
                await LoadData();
            };

            GridView.DataLoaded += (s, e) =>
            {
                txtSearch_TextChanged(txtSearch, null);
            };
        }

        private async Task LoadData()
        {
            int? year = (cmbYear.SelectedItem as ESERCIZIO)?.eseann;

            if (year != null)
            {
                await _dataContext.Load(year.Value);
            }
        }

        private void cmdCompute_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selected = (sender as Button)?.DataContext as PDCANNI;
            int? year = (cmbYear.SelectedItem as ESERCIZIO)?.eseann;

            if (selected != null && year.HasValue)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<MastrinoWindowViewModel>();
                windowViewModel.Year = year.Value;
                windowViewModel.SelectedMastrino = selected;

                var wMastrino = new MastrinoWindow(windowViewModel);

                wMastrino.Owner = Window.GetWindow(this);
                wMastrino.ShowDialog();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        private async void cmbYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadData();
        }
    }
}
