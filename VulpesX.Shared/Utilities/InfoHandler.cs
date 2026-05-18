using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public static class InfoHandler
    {
        public static Action<string>? ShowInfoAction { get; set; }

        public static void Show(string message) => ShowInfoAction?.Invoke(message);
    }
}
