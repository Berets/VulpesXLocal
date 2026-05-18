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
using Telerik.Windows.Diagrams.Core;
using VulpesX.DAL;
using VulpesX.DAL.CRM.AF;
using VulpesX.Models;
using VulpesX.Models.Ufp;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Ufp.CRM.AF;

namespace VulpesX.Modules.Ufp.CRM.AF
{
    /// <summary>
    /// Interaction logic for ANAFAT_ROWView.xaml
    /// </summary>
    public partial class ANAFAT_ROWView : UserControl
    {
        private ANAFAT_ROWViewModel _dataContext;
        private int _selectedPage = 0;

        public ANAFAT_ROWView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ANAFAT_ROWViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.Year = DateTime.Now;

            dtpYear.Culture = new System.Globalization.CultureInfo("it-IT");
            dtpYear.Culture.DateTimeFormat.ShortDatePattern = "yyyy";

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                    LoadData();
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

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        #region Grid events
        private void GridView_RowIsExpandedChanging(object sender, RowCancelEventArgs e)
        {
            var row = e.Row as GridViewRow;

            if (row != null)
            {
                if (row.IsExpanded)
                {
                    var head = row.DataContext as ANAFAT_ROW;

                    if (head != null)
                    {
                        var cnst = _dataContext.GetConst(head.afconstdata, head.afconstver);

                        if (cnst != null)
                        {
                            head.Constant = cnst;
                            _dataContext.CalculateCost(head.Constant, head);
                        }
                    }
                }
            }
        }

        private void patArticleInfo_ToolTipOpening(object sender, RoutedEventArgs e)
        {
            var item = (sender as Path)?.DataContext as ANAFAT_ROW;
            if (item != null && !string.IsNullOrEmpty(item.afartid))
            {
                item.Article = _dataContext.GetArticle(item.afartid);
            }
        }

        private void rgDuplicate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)!.DataContext as ANAFAT_ROW;
            if (item != null)
            {
                item = _dataContext.GetFull(item.afyear, item.afid);

                if (item != null)
                {
                    var con = _dataContext.GetConsts(item.afdata);

                    if (con != null)
                    {
                        item.afdata = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ANAFAT_ROWWindowViewModel>();
                        windowViewModel.Consts = con;
                        windowViewModel.IsInsert = true;
                        windowViewModel.IsDuplicate = true;
                        windowViewModel.Item = item;

                        var wANAFAT = new AF.ANAFAT_ROWWindow(windowViewModel);
                        wANAFAT.Owner = Window.GetWindow(this);
                        Mouse.OverrideCursor = null;
                        if (wANAFAT.ShowDialog() == true)
                        {
                            LoadData();
                        }
                    }
                }
            }
        }
        #endregion

        #region Buttons
        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var date = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            var con = _dataContext.GetConsts(date);

            if (con != null && con.Any())
            {
                var windowDetailViewModel = VulpesServiceProvider.Provider.GetRequiredService<ANAFAT_ROWWindowViewModel>();
                windowDetailViewModel.Consts = con;
                windowDetailViewModel.IsInsert = true;
                windowDetailViewModel.Item = new ANAFAT_ROW
                {
                    afsoc = _dataContext.CompanyID,
                    afyear = date.Year,
                    afcomplexitytype = "S",
                    afcustomertype = "D",
                    afdata = date,
                    afconstver = con.First().afver,
                };

                var wANAFAT = new ANAFAT_ROWWindow(windowDetailViewModel);
                wANAFAT.Owner = Window.GetWindow(this);
                wANAFAT.ShowDialog();
                LoadData();
            }
            else
            {
                ErrorHandler.Show("Attenzione non presente tabella range pezzi");
            }
        }

        private void cmdEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as ANAFAT_ROW;
            if (item != null)
            {
                item = _dataContext.GetFull(item.afyear, item.afid);

                if (item != null)
                {
                    var con = _dataContext.GetConsts(item.afdata);

                    if (con != null)
                    {
                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ANAFAT_ROWWindowViewModel>();
                        windowViewModel.Consts = con;
                        windowViewModel.IsInsert = false;
                        windowViewModel.Item = item;

                        var wANAFAT = new AF.ANAFAT_ROWWindow(windowViewModel);
                        wANAFAT.Owner = Window.GetWindow(this);
                        Mouse.OverrideCursor = null;
                        if (wANAFAT.ShowDialog() == true)
                        {
                            LoadData();
                        }
                    }
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as ANAFAT_ROW;
            if (item != null)
            {
                item = _dataContext.GetFull(item.afyear, item.afid);

                if (item != null)
                {
                    if (ConfirmHandler.Confirm($"Sei sicuro di voler eliminare l'analisi di fattibilità - {item.afyear}-{item.afid}"))
                    {
                        _dataContext.Delete(item);

                        LoadData();
                    }
                }
            }
        }
        #endregion


    }
}
