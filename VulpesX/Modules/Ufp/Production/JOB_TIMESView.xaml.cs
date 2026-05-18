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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VulpesX.ViewModels.Modules.Ufp.Production;

namespace VulpesX.Modules.Ufp.Production
{
    /// <summary>
    /// Interaction logic for JOB_TIMESView.xaml
    /// </summary>
    public partial class JOB_TIMESView : UserControl
    {
        private JOB_TIMESViewModel _dataContext;
        public JOB_TIMESView(JOB_TIMESViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            this.Loaded += async (s, e) =>
            {
                await dataContext.LoadProductionTimes();
            };

        }
    }
}
