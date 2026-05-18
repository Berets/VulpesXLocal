using Microsoft.Extensions.DependencyInjection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls.ScheduleView;
using VulpesX.Models.Models.Tables.Production;
using VulpesX.Shared;
using VulpesX.ViewModels.Modules.Default.Tables.Production;

namespace VulpesX.Modules.Default.Tables.Production
{
    /// <summary>
    /// Interaction logic for CalendarioView.xaml
    /// </summary>
    public partial class CalendarioView : UserControl
    {
        private CalendarioViewModel _dataContext;
        public CalendarioView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<CalendarioViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "SelectedRisorsa")
                {
                    LoadCalendario();
                }
            };
        }

        private async void LoadCalendario()
        {
            DateTime rangeFrom = schedule.VisibleRange.Start;
            DateTime rangeTo = schedule.VisibleRange.End;

            await _dataContext.GetAppointmentModels(rangeFrom, rangeTo);

            schedule.AppointmentsSource = _dataContext.Calendario?.Select(s => new Appointment
            {
                Start = s.Start,
                End = s.End,
                Subject = s.Subject,
                Body = s.Body,
                Importance = s.Importance ? Importance.High : Importance.Low
            });
        }

        #region Scheduler events
        private void schedule_AppointmentCreating(object sender, AppointmentCreatingEventArgs e)
        {
            e.Cancel = true;
        }

        private void schedule_AppointmentEditing(object sender, AppointmentEditingEventArgs e)
        {
            e.Cancel = true;
        }

        private void schedule_AppointmentDeleting(object sender, AppointmentDeletingEventArgs e)
        {
            e.Cancel = true;
        }

        private void schedule_ShowDialog(object sender, ShowDialogEventArgs e)
        {
            e.Cancel = true;
        }

        private void schedule_VisibleRangeChanged(object sender, EventArgs e)
        {
            LoadCalendario();
        }
        #endregion

        #region Buttons
        private void cmdGenera_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CalendarioWindowViewModel>();
            windowViewModel.Dal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            windowViewModel.Al = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            windowViewModel.SelectedRisorse = new ObservableCollection<tab_produzione_risorsa>();
            windowViewModel.EUsaComeDefault = true;
            windowViewModel.ELunediAperto = true;
            windowViewModel.Lunedi = new ObservableCollection<GeneraCalendarioDayModel>();
            windowViewModel.EMartediAperto = true;
            windowViewModel.Martedi = new ObservableCollection<GeneraCalendarioDayModel>();
            windowViewModel.EMercolediAperto = true;
            windowViewModel.Mercoledi = new ObservableCollection<GeneraCalendarioDayModel>();
            windowViewModel.EGiovediAperto = true;
            windowViewModel.Giovedi = new ObservableCollection<GeneraCalendarioDayModel>();
            windowViewModel.EVenerdiAperto = true;
            windowViewModel.Venerdi = new ObservableCollection<GeneraCalendarioDayModel>();
            windowViewModel.ESabatoAperto = false;
            windowViewModel.Sabato = new ObservableCollection<GeneraCalendarioDayModel>();
            windowViewModel.EDomenicaAperto = false;
            windowViewModel.Domenica = new ObservableCollection<GeneraCalendarioDayModel>();

            var wCalendario = new CalendarioWindow(windowViewModel);
            wCalendario.ShowDialog();

            LoadCalendario();
        }
        #endregion
    }
}
