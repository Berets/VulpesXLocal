using DocumentFormat.OpenXml.Wordprocessing;
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
using Telerik.Windows.Controls.Data.DataFilter;
using VulpesX.Shared.Controls.GridCustomFilters;
using VulpesX.ViewModels.Modules.Ufp.General;
using static VulpesX.Shared.Utilities.TelerikGridService;

namespace VulpesX.Modules.Ufp.General
{
    /// <summary>
    /// Interaction logic for ARTSelectView.xaml
    /// </summary>
    public partial class ARTSelectView : UserControl
    {
        private ARTSelectViewModel _dataContext;
        private List<FilterEntry> _currentWhere = new List<FilterEntry>();

        public ARTSelectView(ARTSelectViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();
        }


        private async void cmdSearch_Click(object sender, RoutedEventArgs e)
        {
            await _dataContext.Search(true);
        }

        private void RadAutoCompleteBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (sender as TextBox)!;
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }

        private void RadNumericUpDown_GotFocus(object sender, RoutedEventArgs e)
        {
            var numeric = (sender as RadNumericUpDown)!;

            Dispatcher.BeginInvoke(new Action(() => { numeric.SelectAll(); }));
        }

        private void RadAutoCompleteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var ac = sender as RadAutoCompleteBox;
            if (ac != null)
            {
                if (ac.SelectedItem == null)
                {
                    ac.SearchText = null;
                }
            }
        }
    }
}
