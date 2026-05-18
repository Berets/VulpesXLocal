using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public static class TimeSanitizationHelper
    {
        public static decimal GetMedian(List<decimal> values)
        {
            if (values == null || values.Count == 0)
                return 0;

            var sorted = values.OrderBy(v => v).ToList();
            int mid = sorted.Count / 2;

            if (sorted.Count % 2 != 0)
                // N dispari → elemento centrale
                return sorted[mid];
            else
                // N pari → media dei due elementi centrali
                return (sorted[mid - 1] + sorted[mid]) / 2;
        }

        public static List<decimal> FilterByPercentile(List<decimal> values, decimal lowerPercentile = 5, decimal upperPercentile = 95)
        {
            var sorted = values.OrderBy(v => v).ToList();

            decimal lowerValue = GetPercentile(sorted, lowerPercentile);
            decimal upperValue = GetPercentile(sorted, upperPercentile);

            return values.Where(v => v >= lowerValue && v <= upperValue).ToList();
        }

        private static decimal GetPercentile(List<decimal> sortedValues, decimal percentile)
        {
            if (sortedValues.Count == 0) return 0;

            decimal index = (percentile / 100) * (sortedValues.Count - 1);
            int lower = (int)Math.Floor(index);
            int upper = (int)Math.Ceiling(index);

            if (lower == upper)
                return sortedValues[lower];

            // Interpolazione lineare tra i due valori adiacenti
            decimal fraction = index - lower;
            return sortedValues[lower] + fraction * (sortedValues[upper] - sortedValues[lower]);
        }
    }
}
