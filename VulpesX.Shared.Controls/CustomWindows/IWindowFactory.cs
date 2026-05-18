using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VulpesX.Shared.Controls.CustomWindows
{
    public interface IWindowFactory
    {
        bool? ShowDialog();

        Window? Owner { get; set; }
    }
}
