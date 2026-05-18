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
using Telerik.Windows.Controls.Scheduling;
using VulpesX.Shared.Controls.Gantt;
using VulpesX.ViewModels.Modules.Default.Production;

namespace VulpesX.Modules.Default.Production
{
    /// <summary>
    /// Interaction logic for GanttSingleView.xaml
    /// </summary>
    public partial class GanttSingleView : UserControl
    {
        private GanttSingleViewModel _dataContext;

        public GanttSingleView(GanttSingleViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.VisibleStartTime = DateTime.Now.AddDays(-3);
            _dataContext.VisibleEndTime = DateTime.Now.AddDays(30);
            _dataContext.PixelLength = new TimeSpan(0, 30, 0);

            UpdateOccupazione();
            
            rgvOrders.TimeLineVisualizationBehavior = new TimeLineBaselineBehavior();
        }

        public void UpdateOccupazione()
        {
            var disponibile = new TimeSpan((_dataContext.VisibleEndTime - _dataContext.VisibleStartTime)?.Ticks - (_dataContext.Stops ?? new()).Where(o => o.RisorsaID == _dataContext.Risorsa.ID && o.Dalle >= _dataContext.VisibleStartTime && o.Alle <= _dataContext.VisibleEndTime).Sum(s => (s.Alle - s.Dalle).Ticks) ?? 0);
            var impegni = new TimeSpan(_dataContext.Tasks?.Sum(s => s.Duration.Ticks) ?? 0);
            var impegniPercentuale = (impegni.Ticks * 100) / disponibile.Ticks;
            var residua = new TimeSpan(disponibile.Ticks - impegni.Ticks);

            _dataContext.OccupazioneDisponibile = disponibile.ToString(@"dd\:hh\:mm");
            _dataContext.OccupazioneImpegni = impegni.ToString(@"dd\:hh\:mm");
            _dataContext.OccupazioneImpegniPercentuale = string.Format("{0} %", impegniPercentuale.ToString("N2"));
            _dataContext.OccupazioneResidua = residua.ToString(@"dd\:hh\:mm");
        }

        #region Various events
        private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _dataContext.PixelLength = TimeSpan.FromTicks((long)(double)e.NewValue);
        }
        #endregion

        #region Gantt events
        private void rgvOrders_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        #endregion
    }
}
