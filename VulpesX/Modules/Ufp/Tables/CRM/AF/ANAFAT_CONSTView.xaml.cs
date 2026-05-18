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
using VulpesX.Models.Ufp;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;
using VulpesX.ViewModels.Modules.Ufp.Tables.CRM.AF;

namespace VulpesX.Modules.Ufp.Tables.CRM.AF
{
    /// <summary>
    /// Interaction logic for ANAFAT_CONSTView.xaml
    /// </summary>
    public partial class ANAFAT_CONSTView : UserControl
    {
        private ANAFAT_CONSTViewModel _dataContext;
        private int _selectedPage = 0;
        public ANAFAT_CONSTView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ANAFAT_CONSTViewModel>();

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

                Telerik.Windows.Controls.GridViewColumn activeColumn = this.GridView.Columns["colAtt"];
                Telerik.Windows.Controls.GridView.IColumnFilterDescriptor activeFilter = activeColumn.ColumnFilterDescriptor;

                activeFilter.SuspendNotifications();
                activeFilter.DistinctFilter.AddDistinctValue(true);
                activeFilter.ResumeNotifications();
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
            var item = (sender as Button)!.DataContext as ANAFAT_CONST;

            if (item != null)
            {
                item = _dataContext.Get(item.afsoc, item.afdata, item.afver);

                if (item != null)
                {
                    var areeWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ANAFAT_CONSTWindowViewModel>();
                    areeWindowViewModel.Data = item;
                    areeWindowViewModel.IsInsert = false;

                    var wAREE = new ANAFAT_CONSTWindow(areeWindowViewModel);
                    if (wAREE.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var areeWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ANAFAT_CONSTWindowViewModel>();
            areeWindowViewModel.Data = new ANAFAT_CONST
            {
                afsoc = _dataContext.CompanyID,
                afdata = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime(),
            };
            areeWindowViewModel.IsInsert = true;

            var wAREE = new ANAFAT_CONSTWindow(areeWindowViewModel);
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

        private void GridView_RowIsExpandedChanging(object sender, RowCancelEventArgs e)
        {
            var row = e.Row as GridViewRow;

            if (row != null)
            {
                if (row.IsExpanded)
                {
                    var head = row.DataContext as ANAFAT_CONST;

                    if (head != null)
                    {
                        var full = _dataContext.Get(head.afsoc, head.afdata, head.afver);

                        if (full != null)
                        {
                            head.Pieces = full.Pieces;
                        }
                    }
                }
            }
        }
    }
}
