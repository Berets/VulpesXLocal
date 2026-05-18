using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public static class VersionHelper
    {
        public static string GetVersionText()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            if (version == null)
                return string.Empty;

            return $"Versione {version.Major}.{version.Minor}.{version.Revision} Build {version.Build}";
        }
    }
}
