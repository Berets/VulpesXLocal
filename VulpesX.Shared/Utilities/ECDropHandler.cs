using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public class ECDropHandler
    {
        public static Func<string, string, int, int, int, int, int, int, Task<bool>>? SyncECAction { get; set; }

        public static async Task<bool> SyncEC(string EntityType, string CompanyID, int SourceYear, int SourceID, int SourceRow, int TargetYear, int TargetID, int TargetRow)
        {
            if (SyncECAction == null)
                return false;

            return await SyncECAction.Invoke(
                EntityType,
                CompanyID,
                SourceYear,
                SourceID,
                SourceRow,
                TargetYear,
                TargetID,
                TargetRow);
        }

    }
}
