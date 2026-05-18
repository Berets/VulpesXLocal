using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;

namespace VulpesX.Shared.Controls.CustomWindows
{
    public class FluentDefaultWindow : Window
    {
        static FluentDefaultWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(FluentDefaultWindow),
                new FrameworkPropertyMetadata(typeof(FluentDefaultWindow)));
        }

        public FluentDefaultWindow()
        {
            WindowStyle = WindowStyle.None;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.CanResize;

            WindowChrome.SetWindowChrome(this, new WindowChrome
            {
                CaptionHeight = 48,
                ResizeBorderThickness = new Thickness(4),
                GlassFrameThickness = new Thickness(0),
                UseAeroCaptionButtons = false
            });

        }
    }
}
