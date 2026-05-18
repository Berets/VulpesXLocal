using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using VulpesX.ViewModels.Modules.Default.Treasury;

namespace VulpesX.Modules.Default.Treasury
{
    /// <summary>
    /// Interaction logic for BankCastellettoWindow.xaml
    /// </summary>
    public partial class BankCastellettoWindow : FluentDefaultWindow
    {
        private BankCastellettoWindowViewModel _dataContext;
        public BankCastellettoWindow(BankCastellettoWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);

            this.DataContext = _dataContext;

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    LoadData();
                }
            };

            LoadData();
        }

        private async void LoadData()
        {
            await _dataContext.Load();
        }
    }
}
