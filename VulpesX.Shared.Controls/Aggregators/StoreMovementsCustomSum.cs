using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Aggregators
{
    public class StoreMovementsCustomSum : EnumerableAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get { return "SumByMovementSign"; }
        }

        protected override Type ExtensionMethodsType
        {
            get
            {
                return typeof(Aggregates);
            }
        }

        public class SumResult
        {
            public string? UM { get; set; }
            public decimal Total { get; set; }

            public override bool Equals(object? obj)
            {
                if (obj == null)
                    return false;
                if (!(obj is SumResult))
                    return false;

                return (obj as SumResult)!.UM == UM;
            }

            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Add(UM);
                hash.Add(Total);
                return hash.ToHashCode();
            }
        }
    }
}
