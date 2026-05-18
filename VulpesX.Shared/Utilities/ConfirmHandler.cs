using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public static class ConfirmHandler
    {
        public static Func<string, bool>? ConfirmAction { get; set; }
        public static bool Confirm(string message) => ConfirmAction?.Invoke(message) ?? false;
    }
}
