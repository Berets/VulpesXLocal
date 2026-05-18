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
using System.Windows.Shapes;
using Telerik.Windows.Documents.Fixed.Model.Editing.Lists;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.ViewModels.Modules.Default.General;

namespace VulpesX.Modules.Default.General
{
    /// <summary>
    /// Interaction logic for ABEFreeWindow.xaml
    /// </summary>
    public partial class ABEFreeWindow : FluentDefaultWindow
    {
        private ABEFreeWindowViewModel _dataContext;
        public ABEFreeWindow(ABEFreeWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            var list = _dataContext.GetFreeIDS();
            foreach (var id in list)
            {
                var newButton = new Button()
                {
                    Tag = id,
                    Content = id.ToString("N0"),
                    Width = 80,
                    Height = 30,
                    Margin = new Thickness(2)
                };
                newButton.Click += cmdID_Click;
                spIDs.Children.Add(newButton);
            }
        }

        private void cmdID_Click(object sender, RoutedEventArgs e)
        {
            this.Tag = (e.Source as Button)!.Tag;
            this.DialogResult = true;
        }
    }
}
