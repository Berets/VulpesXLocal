using ClosedXML.Excel;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office.Word;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
using Telerik.Documents.Common.Model;
using Telerik.Pivot.Core;
using Telerik.Windows.Controls.Data.DataFilter;
using Telerik.Windows.Controls.Pivot.Export;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Persistence;
using Telerik.Windows.Persistence.Services;
using VulpesX.DAL._ConnectionFactory;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.PerstistanceManager.Pivot;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.DWH;

namespace VulpesX.Modules.Default.DWH
{
    /// <summary>
    /// Interaction logic for DWHView.xaml
    /// </summary>
    public partial class DWHView : UserControl
    {
        private DWHViewModel _dataContext;

        private DWH_Query? currentQuery = null;
        private DWH_Template? currentTemplate = null;
        private DataTable? _dataTable;
        private PersistenceManager? _persistanceManager;

        private List<DWH_Template> _templates = new List<DWH_Template>();

        public DWHView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<DWHViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    _ = LoadStartupDataAsync();
                }
            };

            Telerik.Windows.Persistence.Services.ServiceProvider.RegisterPersistenceProvider<IValueProvider>(typeof(RadPivotGrid), new LocalDataSourceValueProvider());

            _persistanceManager = new PersistenceManager();
            _persistanceManager.AllowCrossVersion = true;
            _persistanceManager.AllowedTypes.Add(typeof(System.Windows.Size));
            _persistanceManager.AllowedTypes.Add(typeof(System.Windows.SizeConverter));


            _ = LoadStartupDataAsync();
        }

        private async Task LoadStartupDataAsync()
        {
            await Task.WhenAll(
                _dataContext.LoadQueries(),
                _dataContext.LoadTemplates()

            );

            _templates = _dataContext.Templates?.ToList() ?? new List<DWH_Template>();
        }

        private async void LoadTemplates()
        {
            await _dataContext.LoadTemplates();

            _templates = _dataContext.Templates?.ToList() ?? new List<DWH_Template>();
        }

        #region Private methods
        private async void RunQuery(Guid QueryID)
        {
            try
            {
                var query = _dataContext.GetQuery(QueryID);

                if (query != null)
                {
                    if (!string.IsNullOrEmpty(query.Query))
                    {
                        //se lanciata da template recupera valori e aggiorna parametri 
                        if (currentTemplate != null)
                        {
                            currentTemplate = _dataContext.GetTemplate(currentTemplate.ID);

                            if (currentTemplate != null)
                            {
                                if (currentTemplate.Parametri == null)
                                    currentTemplate.Parametri = new ObservableCollection<DWH_TemplateParameter>();

                                foreach (var prm in query.Parametri ?? new ObservableCollection<DWH_QueryParameter>())
                                {
                                    var parameter = currentTemplate.Parametri.Where(o => o.Nome == prm.Nome).FirstOrDefault();

                                    if (parameter != null)
                                    {
                                        parameter.Tipo = prm.Tipo ?? 0;

                                        prm.Valore = parameter.Valore;
                                    }
                                    else
                                    {
                                        parameter = new DWH_TemplateParameter
                                        {
                                            SocietaID = prm.SocietaID,
                                            QueryID = query.ID,
                                            ID = prm.ID,
                                            Nome = prm.Nome,
                                            ParameterDate = prm.ParameterDate,
                                            ParameterValue = prm.ParameterValue,
                                            Tipo = prm.Tipo ?? 0,
                                            Valore = prm.Valore,
                                        };

                                        currentTemplate.Parametri.Add(parameter);
                                    }
                                }
                            }
                        }

                        if (query.Query.ToUpper().Contains("DELETE") || query.Query.ToUpper().Contains("INSERT") || query.Query.ToUpper().Contains("UPDATE") || query.Query.ToUpper().Contains("DROP") || query.Query.ToUpper().Contains("TRUNCATE"))
                        {
                            ErrorHandler.Validation("Attenzione: impossibile eseguire questa query");
                        }
                        else
                        {
                            var needCompanyID = query.Parametri?.Where(o => o.Nome.Trim() == "@SocietaID").Any();
                            var needParameters = query.Parametri?.Where(o => o.Nome.Trim() != "@SocietaID").Any();
                            var canContinue = !query.Parametri?.Where(o => o.Nome.Trim() != "@SocietaID").Any();

                            if (needCompanyID ?? false)
                            {
                                var parameterCompany = query.Parametri!.Where(o => o.Nome.Trim() == "@SocietaID").First();
                                parameterCompany.ParameterValue = UserContext.Instance!.ACCESS!.SelectedCompany!.SOMCOD;
                            }

                            if (needParameters ?? false)
                            {
                                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<DWHRunWindowViewModel>();
                                windowViewModel.Parameters = new ObservableCollection<DWH_QueryParameter>(query.Parametri?.Where(o => o.Nome.Trim() != "@SocietaID").ToList() ?? new List<DWH_QueryParameter>());

                                var wViewRun = new DWHRunWindow(windowViewModel);
                                wViewRun.ShowDialog();

                                canContinue = wViewRun.DialogResult ?? false;

                                if (canContinue ?? false)
                                {
                                    //se lanciata da template recupera valori parametri
                                    if (currentTemplate != null)
                                    {
                                        if (currentTemplate.Parametri == null)
                                            currentTemplate.Parametri = new ObservableCollection<DWH_TemplateParameter>();

                                        foreach (var prm in windowViewModel.Parameters.ToList())
                                        {
                                            var parameter = currentTemplate.Parametri.Where(o => o.Nome == prm.Nome).FirstOrDefault();

                                            if (parameter != null)
                                            {
                                                parameter.ParameterDate = prm.ParameterDate;
                                                parameter.ParameterValue = prm.ParameterValue;
                                            }
                                        }
                                    }
                                    currentQuery = query;

                                    LocalDataSourceProvider ds = new LocalDataSourceProvider();
                                    ds.FieldDescriptionsProvider.GetDescriptionsDataAsyncCompleted += (s, e) =>
                                    {
                                        var rootItem = e.DescriptionsData.RootFieldInfo;
                                        var allContainers = rootItem.Children.ToList();
                                        rootItem.Children.Clear();

                                        allContainers = allContainers.OrderBy(c => c.Caption).ToList();

                                        foreach (var item in allContainers)
                                        {
                                            rootItem.Children.Add(item);
                                        }
                                    };

                                    var dataSet = new DataSet();
                                    var dataTable = dataSet.Tables.Add("Data");

                                    await Task.Run(async () =>
                                    {
                                        var reader = await _dataContext.Execute(query);

                                        if (reader != null)
                                            dataTable = reader;
                                    })
                                    .ContinueWith(t =>
                                    {
                                        _dataTable = dataTable;

                                        ds.ItemsSource = dataTable;

                                        pivotFieldList.DataProvider = ds;
                                        pivotGrid.DataProvider = ds;


                                        txtQuery.Text = query.Titolo;
                                        txtTemplate.Text = "N/A";

                                        if (currentTemplate != null && currentTemplate.StreamByte != null)
                                        {
                                            var fixedBytes = FixNamespace(currentTemplate.StreamByte);
                                            using (var stream = new MemoryStream(fixedBytes))
                                            {
                                                _persistanceManager!.Load(pivotGrid, stream);
                                            }
                                            txtTemplate.Text = currentTemplate.StreamName;

                                            btnSaveTemplate.IsEnabled = true;
                                        }
                                        else
                                        {
                                            btnSaveTemplate.IsEnabled = false;
                                        }

                                        var parmBuilder = new StringBuilder();
                                        foreach (var parm in query.Parametri ?? new ObservableCollection<DWH_QueryParameter>())
                                        {
                                            if (!string.IsNullOrEmpty(parmBuilder.ToString()))
                                                parmBuilder.Append(" | ");

                                            parmBuilder.Append(parm.Nome);
                                            parmBuilder.Append(" - ");
                                            parmBuilder.Append(parm.ParameterValue);
                                        }

                                        txtParameters.Text = parmBuilder.ToString();

                                    }, TaskScheduler.FromCurrentSynchronizationContext());
                                }
                            }


                        }
                    }
                    else
                    {
                        ErrorHandler.Validation($"Query vuota - {QueryID}");
                    }
                }
                else
                {
                    ErrorHandler.Validation($"Query non trovata - {QueryID}");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Validation(ex.Message.ToString());
            }
        }

        byte[] FixNamespace(byte[] original)
        {
            var xml = Encoding.UTF8.GetString(original);

            xml = xml.Replace(
                "http://schemas.datacontract.org/2004/07/VulpesERP.DWH.ValueProvider",
                "http://schemas.datacontract.org/2004/07/VulpesX.Shared.Controls.PerstistanceManager.Pivot"
            );

            return Encoding.UTF8.GetBytes(xml);
        }

        private async void ExcelINTEROP(string FileName)
        {
            Microsoft.Office.Interop.Excel.Application? excel = null;
            Microsoft.Office.Interop.Excel.Workbook? workBook = null;
            Microsoft.Office.Interop.Excel.Worksheet? sheetData = null;
            Microsoft.Office.Interop.Excel.Worksheet? sheetPivot = null;

            try
            {
                var provider = pivotGrid.DataProvider as LocalDataSourceProvider;

                if (provider != null && currentQuery != null)
                {
                    var dataTable = (provider.ItemsSource as DataTable);

                    if (dataTable != null)
                    {
                        bool filterEnable = chkExcelFilter.IsChecked ?? false;
                        var filterTitle = new StringBuilder();
                        var filterRows = new StringBuilder("(");
                        var filterColumns = new StringBuilder("(");
                        var filterFilter = new StringBuilder("(");

                        if ((dataTable.Rows.Count * dataTable.Columns.Count) > 3000000)
                        {
                            ErrorHandler.Validation("ATTENZIONE: impossibile estrarre troppi elementi per generare una tabella Pivot in Excel. Filtrare i dati o cliccare su Excel");
                        }
                        else
                        {
                            foreach (var item in provider.RowGroupDescriptions)
                            {
                                if (!string.IsNullOrEmpty(filterRows.ToString()) && filterRows.ToString() != "(")
                                    filterRows.Append(") AND (");

                                if (item.GroupFilter is Telerik.Pivot.Core.Filtering.LabelGroupFilter)
                                {
                                    var condition = (item.GroupFilter as Telerik.Pivot.Core.Filtering.LabelGroupFilter)?.Condition;

                                    if (condition != null)
                                    {
                                        if (((item.GroupFilter as Telerik.Pivot.Core.Filtering.LabelGroupFilter)?.Condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Comparison == Telerik.Pivot.Core.Filtering.SetComparison.Includes)
                                        {
                                            var items = (condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Items;

                                            if ((items ?? new()).Any())
                                                filterTitle.Append(item.PropertyName);

                                            var filterItem = new StringBuilder();

                                            foreach (var dis in items ?? new SetConditionHashCollection())
                                            {
                                                if (!string.IsNullOrEmpty(filterItem.ToString()))
                                                    filterItem.Append(" OR ");

                                                filterItem.Append(item.PropertyName + " = " + string.Format("'{0}'", dis?.ToString()?.Replace("'", "''")));
                                                filterTitle.Append(" = " + dis?.ToString());
                                            }

                                            filterRows.Append(filterItem);
                                        }
                                        else
                                        {
                                            var items = (condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Items;
                                            if ((items ?? new SetConditionHashCollection()).Any())
                                                filterTitle.Append(item.PropertyName);

                                            var filterItem = new StringBuilder();

                                            foreach (var dis in items ?? new SetConditionHashCollection())
                                            {
                                                if (!string.IsNullOrEmpty(filterItem.ToString()))
                                                    filterItem.Append(" AND ");

                                                filterItem.Append(item.PropertyName + " <> " + string.Format("'{0}'", dis?.ToString()?.Replace("'", "''")));
                                                filterTitle.Append(" <> " + dis?.ToString());
                                            }

                                            filterRows.Append(filterItem);
                                        }
                                    }
                                }

                                if (filterRows.ToString().EndsWith(") AND ("))
                                    filterRows.Remove(filterRows.Length - 7, 7);
                            }
                            filterRows.Append(")");

                            foreach (var item in provider.ColumnGroupDescriptions)
                            {
                                if (!string.IsNullOrEmpty(filterColumns.ToString()) && filterColumns.ToString() != "(")
                                    filterColumns.Append(") AND (");

                                if (item.GroupFilter is Telerik.Pivot.Core.Filtering.LabelGroupFilter)
                                {
                                    var condition = (item.GroupFilter as Telerik.Pivot.Core.Filtering.LabelGroupFilter)?.Condition;

                                    if (condition != null)
                                    {
                                        if (((item.GroupFilter as Telerik.Pivot.Core.Filtering.LabelGroupFilter)?.Condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Comparison == Telerik.Pivot.Core.Filtering.SetComparison.Includes)
                                        {
                                            var items = (condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Items;

                                            if ((items ?? new SetConditionHashCollection()).Any())
                                                filterTitle.Append(item.PropertyName);

                                            var filterItem = new StringBuilder();

                                            foreach (var dis in items ?? new SetConditionHashCollection())
                                            {
                                                if (!string.IsNullOrEmpty(filterItem.ToString()))
                                                    filterItem.Append(" OR ");

                                                filterItem.Append(item.PropertyName + " = " + string.Format("'{0}'", dis.ToString()));
                                                filterTitle.Append(" : " + dis.ToString());
                                            }

                                            filterColumns.Append(filterItem);
                                        }
                                        else
                                        {
                                            var items = (condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Items;

                                            if ((items ?? new SetConditionHashCollection()).Any())
                                                filterTitle.Append(item.PropertyName);

                                            var filterItem = new StringBuilder();

                                            foreach (var dis in items ?? new SetConditionHashCollection())
                                            {
                                                if (!string.IsNullOrEmpty(filterItem.ToString()))
                                                    filterItem.Append(" AND ");

                                                filterItem.Append(item.PropertyName + " <> " + string.Format("'{0}'", dis.ToString()));
                                                filterTitle.Append(" <> " + dis.ToString());
                                            }

                                            filterColumns.Append(filterItem);
                                        }
                                    }

                                    if (filterColumns.ToString().EndsWith(") AND ("))
                                        filterColumns.Remove(filterColumns.Length - 7, 7);
                                }

                                if (filterColumns.ToString().EndsWith(") AND ("))
                                    filterColumns.Remove(filterColumns.Length - 7, 7);
                            }
                            filterColumns.Append(")");

                            foreach (var pro in provider.FilterDescriptions.Where(o => o.Condition != null).GroupBy(g => g.PropertyName))
                            {
                                if (!string.IsNullOrEmpty(filterFilter.ToString()) && filterFilter.ToString() != "(")
                                    filterFilter.Append(") AND (");

                                foreach (var item in pro)
                                {
                                    if (item.Condition != null)
                                    {
                                        if ((item.Condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Comparison == Telerik.Pivot.Core.Filtering.SetComparison.Includes)
                                        {
                                            var items = (item.Condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Items;

                                            if ((items ?? new SetConditionHashCollection()).Any())
                                                filterTitle.Append(item.PropertyName);

                                            var filterItem = new StringBuilder();

                                            foreach (var dis in items ?? new SetConditionHashCollection())
                                            {
                                                if (!string.IsNullOrEmpty(filterItem.ToString()))
                                                    filterItem.Append(" OR ");

                                                filterItem.Append(item.PropertyName + " = " + string.Format("'{0}'", dis?.ToString()?.Replace("'", "''")));
                                                filterTitle.Append(" : " + dis?.ToString());
                                            }

                                            filterFilter.Append(filterItem);
                                        }
                                        else
                                        {
                                            var items = (item.Condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Items;

                                            if ((items ?? new SetConditionHashCollection()).Any())
                                                filterTitle.Append(item.PropertyName);

                                            var filterItem = new StringBuilder();

                                            foreach (var dis in items ?? new SetConditionHashCollection())
                                            {
                                                if (!string.IsNullOrEmpty(filterItem.ToString()))
                                                    filterItem.Append(" AND ");

                                                filterItem.Append(item.PropertyName + " <> " + string.Format("'{0}'", dis?.ToString()?.Replace("'", "''")));
                                                filterTitle.Append(" <> " + dis?.ToString());
                                            }

                                            filterFilter.Append(filterItem);
                                        }
                                    }

                                    if (filterFilter.ToString().EndsWith(") AND ("))
                                        filterFilter.Remove(filterFilter.Length - 7, 7);
                                }

                                if (filterFilter.ToString().EndsWith(") AND ("))
                                    filterFilter.Remove(filterFilter.Length - 7, 7);
                            }
                            filterFilter.Append(")");

                            excel = new Microsoft.Office.Interop.Excel.Application();
                            excel.DefaultSaveFormat = Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook;
                            excel.Visible = false;
                            excel.DisplayAlerts = false;

                            workBook = excel.Workbooks.Add();

                            sheetData = (Microsoft.Office.Interop.Excel.Worksheet)workBook.ActiveSheet;
                            sheetData.Name = "Data";

                            sheetPivot = (Microsoft.Office.Interop.Excel.Worksheet?)workBook.Worksheets.Add();
                            if (sheetPivot != null)
                            {
                                sheetPivot.Name = "Pivot";

                                Microsoft.Office.Interop.Excel.Range? range = null;
                                int pivotRow = 1;
                                int pivotCol = 1;


                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        #region Filter
                                        if (filterEnable)
                                        {
                                            if (!string.IsNullOrEmpty(filterRows.ToString()) && filterRows.ToString() != "(" && filterRows.ToString() != "()")
                                                dataTable = dataTable.Select(filterRows.ToString()).CopyToDataTable();

                                            if (!string.IsNullOrEmpty(filterColumns.ToString()) && filterColumns.ToString() != "(" && filterColumns.ToString() != "()")
                                                dataTable = dataTable.Select(filterColumns.ToString()).CopyToDataTable();

                                            if (!string.IsNullOrEmpty(filterFilter.ToString()) && filterFilter.ToString() != "(" && filterFilter.ToString() != "()")
                                                dataTable = dataTable.Select(filterFilter.ToString()).CopyToDataTable();
                                        }
                                        #endregion

                                        #region Loading data
                                        object[,] arr = new object[dataTable.Rows.Count + 1, dataTable.Columns.Count];
                                        for (int c = 0; c < dataTable.Columns.Count; c++)
                                        {
                                            arr[0, c] = dataTable.Columns[c].ColumnName;
                                        }
                                        int arrRow = 1;
                                        for (int r = 0; r < dataTable.Rows.Count; r++)
                                        {
                                            for (int c = 0; c < dataTable.Columns.Count; c++)
                                            {
                                                arr[arrRow, c] = dataTable.Rows[r][c];
                                            }
                                            ++arrRow;
                                        }

                                        range = sheetData.Range["A1", ((Microsoft.Office.Interop.Excel.Range)sheetData.Cells[arrRow, dataTable.Columns.Count])];
                                        range.NumberFormat = "@";
                                        range.Value2 = arr;
                                        #endregion

                                        #region Title
                                        sheetPivot.Cells[pivotRow, pivotCol] = (currentTemplate != null) ? currentTemplate.StreamName : currentQuery.Titolo;
                                        ((Microsoft.Office.Interop.Excel.Range)sheetPivot.Cells[pivotRow, pivotCol]).Font.Size = 16;
                                        ((Microsoft.Office.Interop.Excel.Range)sheetPivot.Cells[pivotRow, pivotCol]).Font.Bold = true;
                                        ((Microsoft.Office.Interop.Excel.Range)sheetPivot.Cells[pivotRow, pivotCol]).Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightBlue);

                                        ++pivotRow;
                                        foreach (var parm in currentQuery.Parametri ?? new ObservableCollection<DWH_QueryParameter>())
                                        {
                                            sheetPivot.Cells[pivotRow, pivotCol] = parm.Nome;

                                            if (parm.Tipo == (int)SqlDbType.Date || parm.Tipo == (int)SqlDbType.DateTime)
                                            {
                                                sheetPivot.Cells[pivotRow, ++pivotCol] = Convert.ToDateTime(parm.ParameterValue, new CultureInfo("it-IT")).Day + " . " + Convert.ToDateTime(parm.ParameterValue, new CultureInfo("it-IT")).Month + " . " + Convert.ToDateTime(parm.ParameterValue, new CultureInfo("it-IT")).Year;
                                                ((Microsoft.Office.Interop.Excel.Range)sheetPivot.Cells[pivotRow, pivotCol]).EntireColumn.NumberFormat = "@";
                                            }
                                            else
                                            {
                                                sheetPivot.Cells[pivotRow, ++pivotCol] = parm.ParameterValue?.ToString();
                                                ((Microsoft.Office.Interop.Excel.Range)sheetPivot.Cells[pivotRow, pivotCol]).EntireColumn.NumberFormat = "@";
                                            }

                                            ++pivotRow;
                                            pivotCol = 1;
                                        }
                                        ++pivotRow;
                                        sheetPivot.Cells[pivotRow, pivotCol] = filterTitle.ToString();
                                        ++pivotRow;
                                        ++pivotRow;
                                        ++pivotRow;
                                        #endregion
                                    }
                                    catch (Exception ex)
                                    {
                                        ErrorHandler.Validation(ex.Message.ToString());
                                    }
                                })
                                .ContinueWith(t =>
                                {
                                    Microsoft.Office.Interop.Excel.PivotCaches pch = workBook.PivotCaches();
                                    sheetData.Activate();
                                    pch.Create(Microsoft.Office.Interop.Excel.XlPivotTableSourceType.xlDatabase, range).CreatePivotTable(sheetPivot.Cells[pivotRow, 1], "PivTbl_1", Type.Missing, Type.Missing);
                                    Microsoft.Office.Interop.Excel.PivotTable? pvt = sheetPivot.PivotTables("PivTbl_1") as Microsoft.Office.Interop.Excel.PivotTable;

                                    if (pvt != null)
                                    {
                                        pvt.ShowTableStyleColumnHeaders = true;
                                        pvt.ShowTableStyleColumnStripes = true;
                                        pvt.ShowTableStyleLastColumn = true;
                                        pvt.ShowTableStyleRowHeaders = true;
                                        pvt.ShowTableStyleRowStripes = true;

                                        pvt.TableStyle2 = "PivotStyleMedium9";
                                        pvt.ColumnGrand = pivotGrid.RowGrandTotalsPosition != RowTotalsPosition.None;
                                        pvt.RowGrand = pivotGrid.ColumnGrandTotalsPosition != ColumnTotalsPosition.None;
                                        pvt.PivotCache().MissingItemsLimit = Microsoft.Office.Interop.Excel.XlPivotTableMissingItems.xlMissingItemsNone;
                                        pvt.RefreshTable();

                                        if (pivotGrid.VerticalLayout == PivotLayoutType.Compact)
                                            pvt.RowAxisLayout(Microsoft.Office.Interop.Excel.XlLayoutRowType.xlCompactRow);

                                        if (pivotGrid.VerticalLayout == PivotLayoutType.Outline)
                                            pvt.RowAxisLayout(Microsoft.Office.Interop.Excel.XlLayoutRowType.xlOutlineRow);

                                        if (pivotGrid.VerticalLayout == PivotLayoutType.Tabular)
                                            pvt.RowAxisLayout(Microsoft.Office.Interop.Excel.XlLayoutRowType.xlTabularRow);

                                        if (pivotGrid.RowSubTotalsPosition != RowTotalsPosition.None)
                                        {
                                            if (pivotGrid.RowSubTotalsPosition == RowTotalsPosition.Bottom)
                                            {
                                                pvt.SubtotalLocation(Microsoft.Office.Interop.Excel.XlSubtototalLocationType.xlAtBottom);
                                            }
                                            else
                                            {
                                                pvt.SubtotalLocation(Microsoft.Office.Interop.Excel.XlSubtototalLocationType.xlAtTop);
                                            }
                                        }

                                        pvt.PivotCache().MissingItemsLimit = Microsoft.Office.Interop.Excel.XlPivotTableMissingItems.xlMissingItemsNone;
                                        pvt.RefreshTable();
                                        foreach (var item in provider.RowGroupDescriptions)
                                        {
                                            var field = ((Microsoft.Office.Interop.Excel.PivotField)pvt.PivotFields(item.PropertyName));
                                            field.Orientation = Microsoft.Office.Interop.Excel.XlPivotFieldOrientation.xlRowField;
                                            field.Subtotals[1] = pivotGrid.RowSubTotalsPosition != RowTotalsPosition.None;
                                            field.EnableMultiplePageItems = true;

                                            if (item.GroupFilter is Telerik.Pivot.Core.Filtering.LabelGroupFilter)
                                            {
                                                var condition = (item.GroupFilter as Telerik.Pivot.Core.Filtering.LabelGroupFilter)?.Condition;

                                                if (condition != null)
                                                {
                                                    var items = (condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Items;

                                                    if ((items ?? new SetConditionHashCollection()).Any())
                                                    {
                                                        Microsoft.Office.Interop.Excel.PivotItems _PivotItems = (Microsoft.Office.Interop.Excel.PivotItems)field.PivotItems();

                                                        if (((item.GroupFilter as Telerik.Pivot.Core.Filtering.LabelGroupFilter)?.Condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Comparison == Telerik.Pivot.Core.Filtering.SetComparison.Includes)
                                                        {
                                                            foreach (Microsoft.Office.Interop.Excel.PivotItem _PivotItem in _PivotItems)
                                                            {
                                                                if ((items ?? new SetConditionHashCollection()).Select(s => s.ToString()).Contains(_PivotItem.Caption))
                                                                    _PivotItem.Visible = true;
                                                                else
                                                                    _PivotItem.Visible = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            foreach (Microsoft.Office.Interop.Excel.PivotItem _PivotItem in _PivotItems)
                                                            {
                                                                if ((items ?? new SetConditionHashCollection()).Select(s => s.ToString()).Contains(_PivotItem.Caption))
                                                                    _PivotItem.Visible = false;
                                                                else
                                                                    _PivotItem.Visible = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (item.GroupFilter is Telerik.Pivot.Core.Filtering.ValueGroupFilter)
                                            {

                                            }
                                        }

                                        pvt.PivotCache().MissingItemsLimit = Microsoft.Office.Interop.Excel.XlPivotTableMissingItems.xlMissingItemsNone;
                                        pvt.RefreshTable();
                                        foreach (var item in provider.AggregateDescriptions)
                                        {
                                            var field = ((Microsoft.Office.Interop.Excel.PivotField)pvt.PivotFields((item as PropertyAggregateDescription)?.PropertyName));
                                            field.Orientation = Microsoft.Office.Interop.Excel.XlPivotFieldOrientation.xlDataField;
                                            field.Function = Microsoft.Office.Interop.Excel.XlConsolidationFunction.xlSum;

                                            if (!string.IsNullOrEmpty(item.StringFormat) && item.StringFormat != "General")
                                                field.NumberFormat = item.StringFormat;

                                            if (item.TotalFormat?.ToString() == "Telerik.Pivot.Core.Totals.DifferenceFrom")
                                            {
                                                field.Calculation = Microsoft.Office.Interop.Excel.XlPivotFieldCalculation.xlDifferenceFrom;
                                                field.BaseItem = ((Telerik.Pivot.Core.Totals.DifferenceFrom)item.TotalFormat).GroupName;
                                            }
                                            if (item.TotalFormat?.ToString() == "Telerik.Pivot.Core.Totals.DifferenceFromPrevious")
                                            {
                                                field.Calculation = Microsoft.Office.Interop.Excel.XlPivotFieldCalculation.xlDifferenceFrom;
                                                field.BaseItem = "(previous)";
                                            }
                                            if (item.TotalFormat?.ToString() == "Telerik.Pivot.Core.Totals.DifferenceFromNext")
                                            {

                                                field.Calculation = Microsoft.Office.Interop.Excel.XlPivotFieldCalculation.xlDifferenceFrom;
                                                field.BaseItem = "(next)";
                                            }

                                            if (item.TotalFormat?.ToString() == "Telerik.Pivot.Core.Totals.PercentDifferenceFrom")
                                            {
                                                field.Calculation = Microsoft.Office.Interop.Excel.XlPivotFieldCalculation.xlPercentDifferenceFrom;
                                                field.BaseItem = ((Telerik.Pivot.Core.Totals.PercentDifferenceFrom)item.TotalFormat).GroupName;
                                            }
                                            if (item.TotalFormat?.ToString() == "Telerik.Pivot.Core.Totals.PercentDifferenceFromPrevious")
                                            {
                                                field.Calculation = Microsoft.Office.Interop.Excel.XlPivotFieldCalculation.xlPercentDifferenceFrom;
                                                field.BaseItem = "(previous)";
                                            }
                                            if (item.TotalFormat?.ToString() == "Telerik.Pivot.Core.Totals.PercentDifferenceFromNext")
                                            {
                                                field.Calculation = Microsoft.Office.Interop.Excel.XlPivotFieldCalculation.xlPercentDifferenceFrom;
                                                field.BaseItem = "(next)";
                                            }
                                        }

                                        pvt.PivotCache().MissingItemsLimit = Microsoft.Office.Interop.Excel.XlPivotTableMissingItems.xlMissingItemsNone;
                                        pvt.RefreshTable();
                                        foreach (var item in provider.ColumnGroupDescriptions)
                                        {
                                            var field = ((Microsoft.Office.Interop.Excel.PivotField)pvt.PivotFields(item.PropertyName));
                                            field.Orientation = Microsoft.Office.Interop.Excel.XlPivotFieldOrientation.xlColumnField;
                                            field.Subtotals[1] = pivotGrid.ColumnSubTotalsPosition != ColumnTotalsPosition.None;

                                            if (item.GroupFilter is Telerik.Pivot.Core.Filtering.LabelGroupFilter)
                                            {
                                                var condition = (item.GroupFilter as Telerik.Pivot.Core.Filtering.LabelGroupFilter)?.Condition;

                                                if (condition != null)
                                                {
                                                    var items = (condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Items;

                                                    if ((items ?? new SetConditionHashCollection()).Any())
                                                    {
                                                        Microsoft.Office.Interop.Excel.PivotItems _PivotItems = (Microsoft.Office.Interop.Excel.PivotItems)field.PivotItems();

                                                        if (((item.GroupFilter as Telerik.Pivot.Core.Filtering.LabelGroupFilter)?.Condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Comparison == Telerik.Pivot.Core.Filtering.SetComparison.Includes)
                                                        {
                                                            foreach (Microsoft.Office.Interop.Excel.PivotItem _PivotItem in _PivotItems)
                                                            {
                                                                if ((items ?? new SetConditionHashCollection()).Select(s => s.ToString()).Contains(_PivotItem.Caption))
                                                                    _PivotItem.Visible = true;
                                                                else
                                                                    _PivotItem.Visible = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            foreach (Microsoft.Office.Interop.Excel.PivotItem _PivotItem in _PivotItems)
                                                            {
                                                                if ((items ?? new SetConditionHashCollection()).Select(s => s.ToString()).Contains(_PivotItem.Caption))
                                                                    _PivotItem.Visible = false;
                                                                else
                                                                    _PivotItem.Visible = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (item.GroupFilter is Telerik.Pivot.Core.Filtering.ValueGroupFilter)
                                            {

                                            }
                                        }

                                        pvt.PivotCache().MissingItemsLimit = Microsoft.Office.Interop.Excel.XlPivotTableMissingItems.xlMissingItemsNone;
                                        pvt.RefreshTable();
                                        foreach (var item in provider.FilterDescriptions)
                                        {
                                            var field = ((Microsoft.Office.Interop.Excel.PivotField)pvt.PivotFields(item.PropertyName));
                                            field.Orientation = Microsoft.Office.Interop.Excel.XlPivotFieldOrientation.xlPageField;
                                            field.CurrentPage = "(All)";

                                            if (item.Condition != null)
                                            {
                                                var items = (item.Condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Items;

                                                if ((items ?? new SetConditionHashCollection()).Any())
                                                {
                                                    Microsoft.Office.Interop.Excel.PivotItems _PivotItems = (Microsoft.Office.Interop.Excel.PivotItems)field.PivotItems();

                                                    if ((item.Condition as Telerik.Pivot.Core.Filtering.ItemsFilterCondition)?.DistinctCondition.Comparison == Telerik.Pivot.Core.Filtering.SetComparison.Includes)
                                                    {
                                                        foreach (Microsoft.Office.Interop.Excel.PivotItem _PivotItem in _PivotItems)
                                                        {
                                                            if ((items ?? new SetConditionHashCollection()).Select(s => s.ToString()).Contains(_PivotItem.Caption))
                                                                _PivotItem.Visible = true;
                                                            else
                                                                _PivotItem.Visible = false;

                                                            pvt.RefreshTable();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        foreach (Microsoft.Office.Interop.Excel.PivotItem _PivotItem in _PivotItems)
                                                        {
                                                            if ((items ?? new SetConditionHashCollection()).Select(s => s.ToString()).Contains(_PivotItem.Caption))
                                                                _PivotItem.Visible = false;
                                                            else
                                                                _PivotItem.Visible = true;

                                                            pvt.RefreshTable();
                                                        }
                                                    }
                                                }
                                            }


                                        }

                                        try
                                        {
                                            var pivot = ((Microsoft.Office.Interop.Excel.PivotTable?)sheetPivot.PivotTables("PivTbl_1"));

                                            if (pivot != null)
                                                pivot.DataPivotField.Orientation = Microsoft.Office.Interop.Excel.XlPivotFieldOrientation.xlColumnField;
                                        }
                                        catch (Exception)
                                        {

                                        }

                                        sheetPivot.Columns.AutoFit();
                                        sheetPivot.Activate();

                                        workBook.SaveAs(FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
                                        workBook.Close();

                                        Process[] excelProcesses = Process.GetProcessesByName("excel");
                                        foreach (Process p in excelProcesses)
                                        {
                                            if (string.IsNullOrEmpty(p.MainWindowTitle))
                                            {
                                                p.Kill();
                                            }
                                        }

                                        if (File.Exists(FileName))
                                        {
                                            try
                                            {
                                                var p = new Process();
                                                p.StartInfo = new ProcessStartInfo(FileName)
                                                {
                                                    UseShellExecute = true
                                                };
                                                p.Start();
                                            }
                                            catch (Exception)
                                            {
                                                MessageBox.Show($"Impossibile aprire il file: {Environment.NewLine}{FileName}");
                                            }
                                        }
                                    }
                                }, TaskScheduler.FromCurrentSynchronizationContext());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Validation(ex.Message.ToString());
            }
            finally
            {
                sheetData = null;
                sheetPivot = null;
                workBook = null;
                excel = null;

                GC.Collect();
            }
        }

        private void ExportToExcel(string FileName)
        {
            try
            {
                using (var stream = File.Create(FileName))
                {
                    var workbook = GenerateWorkbook();

                    XlsxFormatProvider provider = new XlsxFormatProvider();

                    if (provider != null)
                        provider.Export(workbook, stream, new TimeSpan(0, 1, 0));
                }

                if (File.Exists(FileName))
                {
                    try
                    {
                        string converted = ConvertXLSX(FileName);

                        if (System.IO.File.Exists(converted))
                            Process.Start(@"cmd.exe ", @"/c" + converted);
                    }
                    catch (Exception)
                    {
                        ErrorHandler.Validation($"Impossibile aprire il file: {Environment.NewLine}{FileName}");
                    }
                }
            }
            catch (IOException ex)
            {
                ErrorHandler.Validation(ex.Message);
            }
        }

        private Workbook GenerateWorkbook()
        {
            var export = pivotGrid.GenerateExport();

            Workbook workbook = new Workbook();
            workbook.History.IsEnabled = false;

            var worksheet = workbook.Worksheets.Add();

            workbook.SuspendLayoutUpdate();
            int rowCount = export.RowCount;
            int columnCount = export.ColumnCount;

            var allCells = worksheet.Cells[0, 0, rowCount - 1, columnCount - 1];
            //allCells.SetFontFamily(new ThemableFontFamily(pivotGrid.FontFamily));
            //allCells.SetFontSize(12);
            //allCells.SetFill(GenerateFill(pivotGrid.Background));

            foreach (var cellInfo in export.Cells)
            {
                int rowStartIndex = cellInfo.Row;
                if (rowStartIndex < 0)
                    rowStartIndex = 0;
                int rowEndIndex = (rowStartIndex + cellInfo.RowSpan) - 1;
                if (rowEndIndex < 0)
                    rowEndIndex = 0;
                int columnStartIndex = cellInfo.Column;
                if (columnStartIndex < 0)
                    columnStartIndex = 0;
                int columnEndIndex = (columnStartIndex + cellInfo.ColumnSpan) - 1;
                if (columnEndIndex < 0)
                    columnEndIndex = 0;

                CellSelection cellSelection = worksheet.Cells[rowStartIndex, columnStartIndex];

                var value = cellInfo.Value;
                if (value != null)
                {
                    cellSelection.SetValueAsText(Convert.ToString(value));
                    cellSelection.SetVerticalAlignment(RadVerticalAlignment.Center);
                    cellSelection.SetHorizontalAlignment(GetHorizontalAlignment(cellInfo.TextAlignment));
                    int indent = cellInfo.Indent;
                    if (indent > 0)
                    {
                        cellSelection.SetIndent(indent);
                    }
                }

                cellSelection = worksheet.Cells[rowStartIndex, columnStartIndex, rowEndIndex, columnEndIndex];

                SetCellProperties(cellInfo, cellSelection);
            }

            for (int i = 0; i < columnCount; i++)
            {
                var columnSelection = worksheet.Columns[i];
                columnSelection.AutoFitWidth();

                //NOTE: workaround for incorrect autofit. 
                var newWidth = worksheet.Columns[i].GetWidth().Value.Value + 15;
                columnSelection.SetWidth(new ColumnWidth(newWidth, false));
            }

            workbook.ResumeLayoutUpdate();
            return workbook;
        }

        private RadHorizontalAlignment GetHorizontalAlignment(System.Windows.TextAlignment textAlignment)
        {
            switch (textAlignment)
            {
                case System.Windows.TextAlignment.Center:
                    return RadHorizontalAlignment.Center;

                case System.Windows.TextAlignment.Left:
                    return RadHorizontalAlignment.Left;

                case System.Windows.TextAlignment.Right:
                    return RadHorizontalAlignment.Right;

                case System.Windows.TextAlignment.Justify:
                default:
                    return RadHorizontalAlignment.Justify;
            }
        }

        private static void SetCellProperties(PivotExportCellInfo cellInfo, CellSelection cellSelection)
        {
            //var fill = GenerateFill(cellInfo.Background);
            //if (fill != null)
            //{
            //    cellSelection.SetFill(fill);
            //}

            //var solidBrush = cellInfo.Foreground as SolidColorBrush;
            //if (solidBrush != null)
            //{
            //    cellSelection.SetForeColor(new ThemableColor(solidBrush.Color));
            //}

            //if (cellInfo.FontWeight.HasValue && cellInfo.FontWeight.Value != FontWeights.Normal)
            //{
            //    cellSelection.SetIsBold(true);
            //}

            //var solidBorderBrush = cellInfo.BorderBrush as SolidColorBrush;

            //if (solidBorderBrush != null && cellInfo.BorderThickness.HasValue)
            //{
            //    var borderThickness = cellInfo.BorderThickness.Value;
            //    var color = new ThemableColor(solidBorderBrush.Color);
            //    var leftBorder = new CellBorder(GetBorderStyle(borderThickness.Left), color);
            //    var topBorder = new CellBorder(GetBorderStyle(borderThickness.Top), color);
            //    var rightBorder = new CellBorder(GetBorderStyle(borderThickness.Right), color);
            //    var bottomBorder = new CellBorder(GetBorderStyle(borderThickness.Bottom), color);
            //    var insideBorder = cellInfo.Background != null ? new CellBorder(CellBorderStyle.None, color) : null;
            //    cellSelection.SetBorders(new CellBorders(leftBorder, topBorder, rightBorder, bottomBorder, insideBorder, insideBorder, null, null));
            //}
        }

        private static CellBorderStyle GetBorderStyle(double thickness)
        {
            if (thickness < 1)
            {
                return CellBorderStyle.None;
            }
            else if (thickness < 2)
            {
                return CellBorderStyle.Thin;
            }
            else if (thickness < 3)
            {
                return CellBorderStyle.Medium;
            }
            else
            {
                return CellBorderStyle.Thick;
            }
        }

        private static IFill? GenerateFill(Brush brush)
        {
            if (brush != null)
            {
                var solidBrush = brush as SolidColorBrush;
                if (solidBrush != null)
                {
                    return PatternFill.CreateSolidFill(solidBrush.Color);
                }
            }

            return null;
        }

        public string ConvertXLSX(string FileName)
        {
            var info = new FileInfo(FileName);
            var outputPath = Path.Combine(
                info.DirectoryName!,
                Path.GetFileNameWithoutExtension(info.Name) + "_exp.xlsx"
            );

            using (var workbook = new XLWorkbook(FileName))
            {
                workbook.SaveAs(outputPath);
            }

            return outputPath;
        }
        #endregion

        #region Buttons
        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as RadButton)?.DataContext as DWH_Query;

            if (item != null)
            {
                currentQuery = item;
                currentTemplate = null;

                RunQuery(currentQuery.ID);
            }
        }

        private void BtnRunTemplate_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as RadButton)?.DataContext as DWH_Template;

            if (item != null)
            {
                var query = _dataContext.GetQuery(item.QueryID);

                if (query != null)
                {
                    currentQuery = item.Query;
                    currentTemplate = item;

                    RunQuery(item.QueryID);
                }
            }
        }

        private void BtnExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\VulpesXDWH\";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                ExcelINTEROP(folder + DateTime.Now.Ticks.ToString() + ".xlsx");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnExcelStatic_Click(object sender, RoutedEventArgs e)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\VulpesXDWH\";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            ExportToExcel(folder + DateTime.Now.Ticks.ToString() + ".xlsx");
        }

        private void BtnSaveTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuery != null && currentTemplate != null)
            {
                PersistenceManager manager = new PersistenceManager();

                var stream = manager.Save(this.pivotGrid);
                var stream_byte = new byte[stream.Length];
                stream.ReadExactly(stream_byte, 0, (int)stream.Length);

                if (currentTemplate != null)
                {
                    currentTemplate.StreamByte = stream_byte;
                    currentTemplate.LogUpdated = DateTime.UtcNow;
                    currentTemplate.LogUpdatedUserID = _dataContext.UserID;

                    _dataContext.UpdateTemplate(currentTemplate);

                    LoadTemplates();
                }
                else
                {
                    ErrorHandler.Validation("ATTENZIONE: template non trovato");
                }
            }
            else
            {
                ErrorHandler.Validation("ATTENZIONE: qualcosa è andato storto");
            }
        }

        private void BtnSaveAsNewTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuery != null)
            {
                var stream = _persistanceManager!.Save(this.pivotGrid);
                var stream_byte = new byte[stream.Length];

                if (stream_byte != null)
                {
                    stream.ReadExactly(stream_byte, 0, (int)stream.Length);

                    var id = Guid.NewGuid();

                    var model = new DWH_Template
                    {
                        SocietaID = _dataContext.CompanyID,
                        QueryID = currentQuery.ID,
                        ID = id,
                        StreamByte = stream_byte,
                        StreamName = string.Empty,
                        LogAdded = DateTime.UtcNow,
                        LogAddedUserID = _dataContext.UserID,
                        IsShared = true,
                        Parametri = new ObservableCollection<DWH_TemplateParameter>()
                    };
                    foreach (var prm in currentQuery.Parametri ?? new ObservableCollection<DWH_QueryParameter>())
                    {
                        model.Parametri.Add(new DWH_TemplateParameter { SocietaID = prm.SocietaID, QueryID = currentQuery.ID, ID = id, Nome = prm.Nome, Tipo = prm.Tipo ?? 0 });
                    }

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<DWHTemplateWindowViewModel>();
                    windowViewModel.Data = model;
                    windowViewModel.IsInsert = true;

                    var wDWHTemplate = new DWHTemplateWindow(windowViewModel);
                    wDWHTemplate.ShowDialog();

                    LoadTemplates();
                }
            }
        }
        #endregion

        #region Context menu


        private void CtxEditTemplate_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var item = (sender as RadMenuItem)?.DataContext as DWH_Template;

            if (item != null)
            {
                var query = _dataContext.GetQuery(item.QueryID);

                var template = _dataContext.GetTemplate(item.ID);

                if (query != null && template != null)
                {
                    foreach (var prm in template.Parametri ?? new ObservableCollection<DWH_TemplateParameter>())
                    {
                        prm.Tipo = (query.Parametri ?? new ObservableCollection<DWH_QueryParameter>()).Where(o => o.Nome == prm.Nome).FirstOrDefault()?.Tipo ?? 0;
                    }

                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<DWHTemplateWindowViewModel>();
                    windowViewModel.Data = template;
                    windowViewModel.IsInsert = false;

                    var wDWHTemplate = new DWHTemplateWindow(windowViewModel);
                    wDWHTemplate.ShowDialog();

                    LoadTemplates();
                }
            }
        }

        private void CtxRemoveTemplate_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var item = (sender as RadMenuItem)?.DataContext as DWH_Template;
            if (item != null)
            {
                if (ConfirmHandler.Confirm($"ATTENZIONE: sei sicuro di voler eliminare il template - {item.StreamName}?"))
                {
                    _dataContext.DeleteTemplate(item);

                    LoadTemplates();
                }
            }
        }
        #endregion

        private  void txtSearchTemplates_TextChanged(object sender, TextChangedEventArgs e)
        {
            _dataContext.Templates = new ObservableCollection<DWH_Template>(_templates);

            if (!string.IsNullOrWhiteSpace(txtSearchTemplates.Text))
            {
                var filteredMenu = new ObservableCollection<DWH_Template>();

                foreach (var menu in _dataContext.Templates ?? new ObservableCollection<DWH_Template>())
                {
                    if (menu.StreamName?.ToLower().Contains(txtSearchTemplates.Text.ToLower()) ?? false)
                    {
                        filteredMenu.Add(menu);
                    }

                    filteredMenu.AddRange(TemplateSearch(menu));
                }

                _dataContext.Templates = filteredMenu;
            }
        }

        private List<DWH_Template> TemplateSearch(DWH_Template Item)
        {
            var filteredMenu = new List<DWH_Template>();

            foreach (var menu in Item.Childs ?? new List<DWH_Template>())
            {
                if (menu.StreamName?.ToLower().Contains(txtSearchTemplates.Text.ToLower()) ?? false)
                {
                    filteredMenu.Add(menu);
                }

                filteredMenu.AddRange(TemplateSearch(menu));
            }

            return filteredMenu;
        }
    }
}
