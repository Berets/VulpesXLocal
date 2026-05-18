using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Telerik.Pivot.Core;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Spreadsheet.Model.ConditionalFormattings;
using Telerik.Windows.Persistence.Services;

namespace VulpesX.Shared.Controls.PerstistanceManager.Pivot
{
    public abstract class DataProviderValueProvider : IValueProvider
    {
        public abstract IEnumerable<Type> KnownTypes { get; }

        public string ProvideValue(object context)
        {
            string serialized = string.Empty;

            RadPivotGrid? pivotGrid = (context as RadPivotGrid);

            if (pivotGrid != null)
            {
                IDataProvider? dataProvider = pivotGrid.DataProvider;

                if (dataProvider != null)
                {
                    MemoryStream stream = new MemoryStream();

                    DataProviderSettings settings = new DataProviderSettings()
                    {
                        Aggregates = dataProvider.Settings.AggregateDescriptions.OfType<object>().ToArray(),
                        Filters = dataProvider.Settings.FilterDescriptions.OfType<object>().ToArray(),
                        Rows = dataProvider.Settings.RowGroupDescriptions.OfType<object>().ToArray(),
                        Columns = dataProvider.Settings.ColumnGroupDescriptions.OfType<object>().ToArray(),
                        AggregatesLevel = dataProvider.Settings.AggregatesLevel,
                        AggregatesPosition = dataProvider.Settings.AggregatesPosition,
                        HorizontalLayout = pivotGrid.HorizontalLayout,
                        VerticalLayout = pivotGrid.VerticalLayout,
                        RowTotalsPosition = pivotGrid.RowSubTotalsPosition,
                        RowGrandTotalsPosition = pivotGrid.RowGrandTotalsPosition,
                        ColumnTotalsPosition = pivotGrid.ColumnSubTotalsPosition,
                        ColumnGrandTotalsPosition = pivotGrid.ColumnGrandTotalsPosition,
                        ShowAggregateValuesInline = pivotGrid.ShowAggregateValuesInline,
                    };

                    DataContractSerializer serializer = new DataContractSerializer(typeof(DataProviderSettings), KnownTypes);
                    serializer.WriteObject(stream, settings);

                    stream.Position = 0;
                    var streamReader = new StreamReader(stream);
                    serialized += streamReader.ReadToEnd();
                }
            }

            return serialized;
        }

        public void RestoreValue(object context, string savedValue)
        {
            RadPivotGrid? pivotGrid = context as RadPivotGrid;
            if (pivotGrid != null)
            {
                IDataProvider? dataProvider = pivotGrid.DataProvider as IDataProvider;

                if (dataProvider != null)
                {
                    var stream = new MemoryStream();
                    var tw = new StreamWriter(stream);
                    tw.Write(savedValue);
                    tw.Flush();
                    stream.Position = 0;

                    DataContractSerializer serializer = new DataContractSerializer(typeof(DataProviderSettings), KnownTypes);
                    var result = serializer.ReadObject(stream);
                    var dataProviderSettings = (result as DataProviderSettings);

                    if (dataProviderSettings != null)
                    {

                        dataProvider.Settings.AggregateDescriptions.Clear();
                        foreach (var aggregateDescription in dataProviderSettings.Aggregates ?? new object[0])
                        {
                            dataProvider.Settings.AggregateDescriptions.Add(aggregateDescription);
                        }

                        dataProvider.Settings.FilterDescriptions.Clear();
                        foreach (var filterDescription in dataProviderSettings.Filters ?? new object[0])
                        {
                            dataProvider.Settings.FilterDescriptions.Add(filterDescription);
                        }

                        dataProvider.Settings.RowGroupDescriptions.Clear();
                        foreach (var rowDescription in dataProviderSettings.Rows ?? new object[0])
                        {
                            dataProvider.Settings.RowGroupDescriptions.Add(rowDescription);
                        }

                        dataProvider.Settings.ColumnGroupDescriptions.Clear();
                        foreach (var columnDescription in dataProviderSettings.Columns ?? new object[0])
                        {
                            dataProvider.Settings.ColumnGroupDescriptions.Add(columnDescription);
                        }

                        dataProvider.Settings.AggregatesPosition = dataProviderSettings.AggregatesPosition;
                        dataProvider.Settings.AggregatesLevel = dataProviderSettings.AggregatesLevel;

                        pivotGrid.HorizontalLayout = dataProviderSettings.HorizontalLayout;
                        pivotGrid.VerticalLayout = dataProviderSettings.VerticalLayout;
                        pivotGrid.RowSubTotalsPosition = dataProviderSettings.RowTotalsPosition;
                        pivotGrid.RowGrandTotalsPosition = dataProviderSettings.RowGrandTotalsPosition;
                        pivotGrid.ColumnSubTotalsPosition = dataProviderSettings.ColumnTotalsPosition;
                        pivotGrid.ColumnGrandTotalsPosition = dataProviderSettings.ColumnGrandTotalsPosition;
                        pivotGrid.ShowAggregateValuesInline = dataProviderSettings.ShowAggregateValuesInline;
                    }
                }
            }
        }
    }
}
