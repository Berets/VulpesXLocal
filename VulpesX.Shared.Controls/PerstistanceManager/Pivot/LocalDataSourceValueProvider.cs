using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Pivot.Core;

namespace VulpesX.Shared.Controls.PerstistanceManager.Pivot
{
    public class LocalDataSourceValueProvider : DataProviderValueProvider
    {
        public override IEnumerable<Type> KnownTypes
        {
            get
            {
                return PivotSerializationHelper.KnownTypes;
            }
        }
    }
}
