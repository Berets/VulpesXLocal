using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public class ArticleDropHandler
    {
        public static Func<object, object, int, bool>? SyncDropAction { get; set; }

        public static bool SyncDrop(object Data, object DestinationItems, int DropIndex)
            => SyncDropAction?.Invoke(Data, DestinationItems, DropIndex) ?? false;
    }
}
