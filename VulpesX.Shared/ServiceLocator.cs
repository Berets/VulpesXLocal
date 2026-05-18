using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared
{
    public static class VulpesServiceProvider
    {
        public static ServiceProvider Provider { get; set; } = null!;
    }
}
