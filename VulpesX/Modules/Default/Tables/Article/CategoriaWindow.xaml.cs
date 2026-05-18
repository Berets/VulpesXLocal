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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.Article;

namespace VulpesX.Modules.Default.Tables.Article
{
    /// <summary>
    /// Interaction logic for CategoriaWindow.xaml
    /// </summary>
    public partial class CategoriaWindow : FluentDefaultWindow
    {
        private CategoriaWindowViewModel _dataContext;
        public CategoriaWindow(CategoriaWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save())
                {
                    this.DialogResult = true;
                }
            }
            else
            {
                ErrorHandler.Validation(validated);
            }
        }
    }
}
