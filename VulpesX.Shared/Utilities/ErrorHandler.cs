using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public static class ErrorHandler
    {
        public static Action<string>? ShowErrorAction { get; set; }

        public static void Show(string message, [CallerMemberName] string caller = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            var ns = Path.GetFileNameWithoutExtension(file);

            var fullLocation = $"{ns}.{caller} (line {line})";

            ShowErrorAction?.Invoke($"{fullLocation}\n{message}");
        }

        public static void Validation(string message)
        {
            ShowErrorAction?.Invoke(message);
        }
    }
}
