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
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using VulpesX.Models.Models.CRM.AF;
using VulpesX.Modules.Ufp.General;
using VulpesX.Shared;
using VulpesX.ViewModels.Modules.Ufp.CRM.AF;
using VulpesX.ViewModels.Modules.Ufp.General;

namespace VulpesX.Modules.Ufp.CRM.AF
{
    /// <summary>
    /// Interaction logic for ANAFAT_ROW_MaterialView.xaml
    /// </summary>
    public partial class ANAFAT_ROW_MaterialView : UserControl
    {
        private ANAFAT_ROWWindowViewModel? _dataContext;
        private bool _isUserSelectingMaterial = false;
        public ANAFAT_ROW_MaterialView()
        {
            InitializeComponent();

            this.DataContextChanged += (s, e) =>
            {
                var newDC = e.NewValue as ANAFAT_ROWWindowViewModel;

                if (newDC != null)
                {
                    _dataContext = newDC;

                    this.DataContext = _dataContext;

                    _dataContext.ColumnsReady += (suppliers) =>
                    {
                        BuildColumns(suppliers);
                    };
                    _dataContext.ArtSelectMaterialViewModel.SearchCompleted += async (count, isUserSearch) =>
                    {
                        if (_dataContext.ArtSelectMaterialViewModel.Items != null && _dataContext.ArtSelectMaterialViewModel.Items.Any())
                        {
                            await _dataContext.LoadPriceLists(_dataContext.ArtSelectMaterialViewModel.Items!.ToList());

                            if (!string.IsNullOrEmpty(_dataContext.Item.afmatid))
                            {
                                var article = _dataContext.PriceLists?.Where(o => ((dynamic)o).ID == _dataContext.Item.afmatid).FirstOrDefault();

                                if (article != null)
                                {
                                    var supplier = article.GetAllCells().Where(o => o.SupplierID == _dataContext.Item.afmatforid).FirstOrDefault();

                                    if (supplier != null)
                                    {
                                        supplier.IsSelected = true;
                                    }
                                }
                            }

                            if (_dataContext.PriceLists != null)
                            {
                                var sortedList = _dataContext.PriceLists.OrderBy(row =>
                                {
                                    var cells = row.GetAllCells();

                                    if (cells.Any(c => c.IsSelected)) return 1;

                                    if (cells.Any(c => c.Color == "R")) return 2;

                                    if (cells.Any(c => c.Color == "G")) return 3;

                                    return 4;
                                }).ToList();

                                _dataContext.PriceLists = new ObservableCollection<ArticlePriceListRowModel>(sortedList);
                            }
                        }
                        else
                        {
                            GridView.Columns.Clear();

                            _dataContext.PriceLists = null;
                        }
                    };

                    grdArtSelect.Children.Add(new ARTSelectView(_dataContext.ArtSelectMaterialViewModel));
                }
            };
        }

        private void BuildColumns(List<string> supplierKeys)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GridView.Columns.Clear();
                GridView.AutoGenerateColumns = false;


                var articleCellTemplate = (DataTemplate)FindResource("ArticleCellTemplate");
                var supplierCellTemplate = (DataTemplate)FindResource("SupplierCellTemplate");

                GridView.Columns.Add(new GridViewDataColumn
                {
                    Header = "Disegno",
                    UniqueName = "Disegno",
                    Width = new GridViewLength(110, GridViewLengthUnitType.Star),
                    DataMemberBinding = new Binding($"[Article]"),
                    CellTemplate = articleCellTemplate,
                    Tag = "Disegno",
                });

                foreach (var key in supplierKeys)
                {
                    var parts = key.Split('\n');
                    string supplierNo = parts[0];
                    string supplierName = parts.Length > 1 ? parts[1] : "";

                    GridView.Columns.Add(new GridViewDataColumn
                    {
                        Header = key,
                        UniqueName = key,
                        Width = new GridViewLength(110),
                        DataMemberBinding = new Binding($"[{key}]"),
                        CellTemplate = supplierCellTemplate,
                        Tag = key
                    });
                }
            });
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var currentCell = (sender as CheckBox)!.DataContext as ArticlePriceListCellModel;
            if (currentCell == null || _dataContext == null) return;

            if ((currentCell.Article.artlun ?? 0) > 0 && _isUserSelectingMaterial)
            {
                _dataContext.Item.afmatpre = (currentCell.Price / currentCell.Article.artlun) * _dataContext.Item.Article?.artlun;
                _dataContext.Item.afmatid = currentCell.Article.ID;
                _dataContext.Item.afmatfordata = currentCell.Date;
                _dataContext.Item.afmatforid = currentCell.SupplierID;

            }

            _isUserSelectingMaterial = false;

            foreach (var row in _dataContext!.PriceLists ?? new System.Collections.ObjectModel.ObservableCollection<ArticlePriceListRowModel>())
            {
                foreach (var cell in row.GetAllCells())
                {
                    if (cell != currentCell)
                    {
                        cell.IsSelected = false;
                    }
                }
            }
        }

        private void CheckBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isUserSelectingMaterial = true;
        }

        private void numTotal_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (_dataContext == null) return;

            _dataContext.CalculateCost();
        }
    }
}
