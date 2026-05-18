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
using VulpesX.Modules.Ufp.General;
using VulpesX.ViewModels.Modules.Ufp.CRM.AF;

namespace VulpesX.Modules.Ufp.CRM.AF
{
    /// <summary>
    /// Interaction logic for ANAFAT_ROW_ExtraView.xaml
    /// </summary>
    public partial class ANAFAT_ROW_ExtraView : UserControl
    {
        private ANAFAT_ROWWindowViewModel? _dataContext;

        public ANAFAT_ROW_ExtraView()
        {
            InitializeComponent();

            this.DataContextChanged += (s, e) =>
            {
                var newDC = e.NewValue as ANAFAT_ROWWindowViewModel;

                if (newDC != null)
                {
                    _dataContext = newDC;

                    this.DataContext = _dataContext;
                }
            };

        }

        private void numTotal_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (_dataContext == null) return;

            _dataContext.CalculateCost();
        }
    }
}
