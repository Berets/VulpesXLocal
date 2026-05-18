using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;
using VulpesX.Shared.Generics;
using static VulpesX.Shared.Utilities.TelerikGridService;

namespace VulpesX.Shared.Controls.Utilities
{
    public static class TelerikControls
    {
        public static void SortManager(List<GenericIDDescription> CurrentSort, GridViewSortingEventArgs e)
        {
            if (e.Column.SortingIndex == -1 && e.OldSortingState == SortingState.None)
            {
                if (e.DataControl.SortDescriptors.Count == 1)
                    CurrentSort.Clear();
                CurrentSort.Add(new GenericIDDescription() { ID = e.Column.UniqueName, Description = e.NewSortingState == SortingState.Ascending ? "ASC" : "DESC" });
            }
            else
            {
                var curr = CurrentSort.Where(w => w.ID == e.Column.UniqueName).First();
                if (e.NewSortingState == SortingState.None)
                {
                    CurrentSort.Remove(curr);
                }
                else
                {
                    curr.Description = e.NewSortingState == SortingState.Ascending ? "ASC" : "DESC";
                }
            }
        }

        public static void FilterManager(List<FilterEntry> CurrentFilter, GridViewFilteringEventArgs e)
        {
            if (e.Added.Count() > 0)
            {
                foreach (var item in e.Added.Cast<FilterDescriptor>())
                {
                    CurrentFilter.Add(new FilterEntry(item.Member, item.Member, DecodeFilterOperator(item.Operator), item.Value?.ToString() ?? string.Empty));
                }
            }
            if (e.Removed.Count() > 0)
            {
                foreach (var item in e.Removed.Cast<FilterDescriptor>())
                {
                    CurrentFilter.Remove(CurrentFilter.Where(w => w.AliasId == item.Member).First());
                }
            }
        }

        public static string DecodeFilterOperator(FilterOperator FilterOperator)
        {
            switch (FilterOperator)
            {
                case FilterOperator.IsEqualTo: return "=";
                case FilterOperator.IsNotEqualTo: return "<>";
                case FilterOperator.IsNull: return "IS NULL";
                case FilterOperator.IsNotNull: return "IS NOT NULL";
                case FilterOperator.IsGreaterThan: return ">";
                case FilterOperator.IsLessThan: return "<";
                case FilterOperator.IsGreaterThanOrEqualTo: return ">=";
                case FilterOperator.IsLessThanOrEqualTo: return "<=";
                case FilterOperator.Contains: return "LIKE";
                case FilterOperator.DoesNotContain: return "NOT LIKE";
            }
            return "=";
        }

        public static void CleanFilters(FilterOperatorsLoadingEventArgs e)
        {
            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsEmpty);
            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsNotEmpty);

            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.EndsWith);
            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.StartsWith);

            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsContainedIn);
            e.AvailableOperators.Remove(Telerik.Windows.Data.FilterOperator.IsNotContainedIn);

            e.DefaultOperator1 = Telerik.Windows.Data.FilterOperator.Contains;
            e.DefaultOperator2 = Telerik.Windows.Data.FilterOperator.Contains;
        }

        public static void CleanFiltersOnlyContains(FilterOperatorsLoadingEventArgs e)
        {
            foreach (var e2 in e.AvailableOperators.ToList())
            {
                if (e2 != FilterOperator.Contains)
                    e.AvailableOperators.Remove(e2);
            }

            e.DefaultOperator1 = Telerik.Windows.Data.FilterOperator.Contains;
            e.DefaultOperator2 = Telerik.Windows.Data.FilterOperator.Contains;
        }
    }
}
