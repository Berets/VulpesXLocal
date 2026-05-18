using Microsoft.Extensions.DependencyInjection;
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
using VulpesX.DAL;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Commons;

namespace VulpesX.Modules.Default.Commons
{
    /// <summary>
    /// Interaction logic for SingleDateWindow.xaml
    /// </summary>
    public partial class SingleDateWindow : FluentDefaultWindow
    {
        private SingleDateWindowViewModel _dataContext;
        public SingleDateWindow(SingleDateWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.SelectedDate = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            cmdCancel.IsEnabled = false;
            cmdSave.IsEnabled = false;

            if (rdpDate.SelectedValue.HasValue || _dataContext.AllowNullDate)
            {
                this.DialogResult = true;
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("La data deve avere un valore valido");
                cmdCancel.IsEnabled = true;
                cmdSave.IsEnabled = true;
            }
        }
    }
}
