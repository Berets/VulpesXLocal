using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VulpesX.ViewModels.Modules.Default.Tables.Production;

namespace VulpesX.Modules.Default.Tables.Production
{
    /// <summary>
    /// Interaction logic for CausaliWindow.xaml
    /// </summary>
    public partial class CausaliWindow : FluentDefaultWindow
    {
        private CausaliWindowViewModel _dataContext;
        public CausaliWindow(CausaliWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            if (!_dataContext.IsInsert)
            {
                var list = _dataContext.GetListFromCausale(_dataContext.Data.ID);

                if(list != null && _dataContext.Risorse != null)
                    _dataContext.Data.CausaleRisorse = new ObservableCollection<tab_produzione_risorsa>(_dataContext.Risorse.Where(o => list.Any(oo => oo.ID == o.ID)));
            }
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
            { ErrorHandler.Validation(validated); }
        }
    }
}
