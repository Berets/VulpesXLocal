using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VulpesX.Shared.Controls.CustomWindows
{
    public static class WindowDimmer
    {
        private static readonly Dictionary<Window, Border> Overlays = new();

        private static readonly Dictionary<Window, int> RefCounts = new();

        public static void Dim(Window owner)
        {
            if (owner == null)
                return;

            if (!RefCounts.ContainsKey(owner))
                RefCounts[owner] = 0;

            RefCounts[owner]++;

            if (Overlays.ContainsKey(owner))
                return;

            var root = EnsureGridRoot(owner);

            var overlay = new Border
            {
                Background = new SolidColorBrush(Colors.Black),
                Opacity = 0.7,
                IsHitTestVisible = false
            };

            Grid.SetRow(overlay, 0);
            Grid.SetColumn(overlay, 0);
            Grid.SetRowSpan(overlay, root.RowDefinitions.Count > 0 ? root.RowDefinitions.Count : 1);
            Grid.SetColumnSpan(overlay, root.ColumnDefinitions.Count > 0 ? root.ColumnDefinitions.Count : 1);


            root.Children.Add(overlay);
            Overlays[owner] = overlay;
        }

        public static void Undim(Window owner)
        {
            if (owner == null || !RefCounts.ContainsKey(owner))
                return;

            RefCounts[owner]--;

            if (RefCounts[owner] > 0)
                return;

            RefCounts.Remove(owner);

            if (Overlays.TryGetValue(owner, out var overlay) &&
                owner.Content is Grid grid)
            {
                grid.Children.Remove(overlay);
                Overlays.Remove(owner);
            }
        }

        private static Grid EnsureGridRoot(Window window)
        {
            if (window.Content is Grid grid)
                return grid;

            // Wrap existing content
            var originalContent = window.Content as UIElement;

            var newGrid = new Grid();
            window.Content = null;

            if (originalContent != null)
                newGrid.Children.Add(originalContent);

            window.Content = newGrid;

            return newGrid;
        }
    }


}
