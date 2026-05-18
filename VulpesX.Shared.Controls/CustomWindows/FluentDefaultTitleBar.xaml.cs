using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VulpesX.Shared.Controls.CustomWindows
{
    /// <summary>
    /// Interaction logic for FluentDefaultTitleBar.xaml
    /// </summary>
    public partial class FluentDefaultTitleBar : UserControl
    {
        private bool isSimulatedMaximized = false;
        private Rect restoreBounds;
        public FluentDefaultTitleBar()
        {
            InitializeComponent();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
                SystemCommands.MinimizeWindow(window);
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null) return;

            const double margin = 25;
            var workArea = SystemParameters.WorkArea;

            if (!isSimulatedMaximized)
            {
                // Save current bounds
                restoreBounds = new Rect(window.Left, window.Top, window.Width, window.Height);

                // Simulate maximize
                window.Left = workArea.Left + margin;
                window.Top = workArea.Top + margin;
                window.Width = workArea.Width - margin * 2;
                window.Height = workArea.Height - margin * 2;

                isSimulatedMaximized = true;
            }
            else
            {
                // Restore original bounds
                window.Left = restoreBounds.Left;
                window.Top = restoreBounds.Top;
                window.Width = restoreBounds.Width;
                window.Height = restoreBounds.Height;

                isSimulatedMaximized = false;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
                SystemCommands.CloseWindow(window);
        }
    }
}
