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
using System.Windows.Shapes;
using Telerik.Windows.Data;
using VulpesX.DAL;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for SelectECWindow.xaml
    /// </summary>
    public partial class SelectECWindow : FluentDefaultWindow
    {
        private SelectECWindowViewModel _dataContext;
        public SelectECWindow(SelectECWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);

            _dataContext.GetABE();
            _dataContext.ToDate = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
            _dataContext.HasDrawn = false;
            _dataContext.SinceDrawnDate = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            this.DataContext = _dataContext;
            this.Title = $"Seleziona partite dall'estratto conto di {_dataContext.SelectedEntity?.FullDescriptionSearchable}";

            

            this.Loaded += async (s, e) =>
            {
                await LoadData();

                ECDropHandler.SyncECAction = async (entity, companyid, sourceyear, sourceid, sourcerow, targetyear, targetid, targetrow) =>
                {
                    var result = _dataContext.SyncEC(entity, companyid, sourceyear, sourceid, sourcerow, targetyear, targetid, targetrow);

                    if (result)
                        await LoadData();

                    return result;
                };
            };
            this.Closed += (s, e) =>
            {
                ECDropHandler.SyncECAction = null;
            };
        }

        private async Task LoadData()
        {
            await _dataContext.Load();

            GridView.GroupDescriptors.Clear();
            GridView.GroupDescriptors.Add(new GroupDescriptor() { Member = "Partita", SortDirection = null });
        }

        private async void rdtLimitDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadData();
        }

        private async void chkDrawn_Checked(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }

        private async void chkDrawn_Unchecked(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }


        private async void rdtDrawnLimitDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await LoadData();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            this.Tag = GridView.SelectedItems.Cast<MastrinoECReportItem>().ToList();
            this.DialogResult = true;
        }

        private void GridView_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            var dare = GridView.SelectedItems?.Cast<MastrinoECReportItem>().Sum(sum => sum.Dare) ?? 0;
            var avere = GridView.SelectedItems?.Cast<MastrinoECReportItem>().Sum(sum => sum.Avere) ?? 0;

            _dataContext.TotalSelectedSign = dare > avere ? "D" : (dare == avere ? "-" : "A");
            _dataContext.TotalSelectedAmount = dare > avere ? Math.Round(dare - avere, 2, MidpointRounding.AwayFromZero) : (dare == avere ? 0 : Math.Round(avere - dare, 2, MidpointRounding.AwayFromZero));
        }
    }
}
