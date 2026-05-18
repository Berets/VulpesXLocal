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
using VulpesX.ViewModels.Modules.Default.Accounting.Assets;

namespace VulpesX.Modules.Default.Accounting.Assets
{
    /// <summary>
    /// Interaction logic for ACC_ASSETS_CARDSHistoryWindow.xaml
    /// </summary>
    public partial class ACC_ASSETS_CARDSHistoryWindow : FluentDefaultWindow
    {
        private ACC_ASSETS_CARDSHistoryWindowViewModel _dataContext;
        public ACC_ASSETS_CARDSHistoryWindow(ACC_ASSETS_CARDSHistoryWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
            this.Title = _dataContext.Title;
        }
    }
}
