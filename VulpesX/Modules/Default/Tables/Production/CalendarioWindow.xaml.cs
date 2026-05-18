using Itenso.TimePeriod;
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
using VulpesX.Models.Models.Tables.Production;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.Production;

namespace VulpesX.Modules.Default.Tables.Production
{
    /// <summary>
    /// Interaction logic for CalendarioWindow.xaml
    /// </summary>
    public partial class CalendarioWindow : FluentDefaultWindow
    {
        private CalendarioWindowViewModel _dataContext;
        public CalendarioWindow(CalendarioWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "ELunediAperto")
                {
                    rgvLunedi.IsEnabled = _dataContext.ELunediAperto;
                    if (!_dataContext.ELunediAperto)
                        _dataContext.Lunedi = new ObservableCollection<GeneraCalendarioDayModel>();
                }
                if (e.PropertyName == "EMartediAperto")
                {
                    rgvMartedi.IsEnabled = _dataContext.EMartediAperto;
                    if (!_dataContext.EMartediAperto)
                        _dataContext.Martedi = new ObservableCollection<GeneraCalendarioDayModel>();
                }
                if (e.PropertyName == "EMercolediAperto")
                {
                    rgvMercoledi.IsEnabled = _dataContext.EMercolediAperto;
                    if (!_dataContext.EMercolediAperto)
                        _dataContext.Mercoledi = new ObservableCollection<GeneraCalendarioDayModel>();
                }
                if (e.PropertyName == "EGiovediAperto")
                {
                    rgvGiovedi.IsEnabled = _dataContext.EGiovediAperto;
                    if (!_dataContext.EGiovediAperto)
                        _dataContext.Giovedi = new ObservableCollection<GeneraCalendarioDayModel>();
                }
                if (e.PropertyName == "EVenerdiAperto")
                {
                    rgvVenerdi.IsEnabled = _dataContext.EVenerdiAperto;
                    if (!_dataContext.EVenerdiAperto)
                        _dataContext.Venerdi = new ObservableCollection<GeneraCalendarioDayModel>();
                }
                if (e.PropertyName == "ESabatoAperto")
                {
                    rgvSabato.IsEnabled = _dataContext.ESabatoAperto;
                    if (!_dataContext.ESabatoAperto)
                        _dataContext.Sabato = new ObservableCollection<GeneraCalendarioDayModel>();
                }
                if (e.PropertyName == "EDomenicaAperto")
                {
                    rgvDomenica.IsEnabled = _dataContext.EDomenicaAperto;
                    if (!_dataContext.EDomenicaAperto)
                        _dataContext.Domenica = new ObservableCollection<GeneraCalendarioDayModel>();
                }
            };

        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            bool proceed = true;

            if (!_dataContext.Dal.HasValue || !_dataContext.Al.HasValue)
            {
                proceed = false;
                ErrorHandler.Validation("ATTENZIONE: Dal e Al obbligatori");
            }
            if (_dataContext.Dal > _dataContext.Al)
            {
                proceed = false;
                ErrorHandler.Validation("ATTENZIONE: Dal deve essere minore o uguale a Al");
            }
            if (!(_dataContext.SelectedRisorse ?? new ObservableCollection<tab_produzione_risorsa>()).Any())
            {
                proceed = false;
                ErrorHandler.Validation("ATTENZIONE: Selezionare almeno una risorsa per cui generare il calendario");
            }


            if (proceed)
            {
                _dataContext.IsBusy = true;

                using (BackgroundWorker bgwOrders = new BackgroundWorker())
                {
                    bgwOrders.DoWork += delegate (object? s, DoWorkEventArgs args)
                    {
                        DateTime from = _dataContext.Dal!.Value;
                        DateTime to = _dataContext.Al!.Value;

                        var toSave = new ObservableCollection<tab_produzione_risorsa_calendario>();
                        var toSaveStops = new ObservableCollection<tab_produzione_calendario_chiusura>();

                        while (from <= to)
                        {
                            var turns = new ObservableCollection<GeneraCalendarioDayModel>();
                            string tipo = Constants._CALENDARIO_CLOSE;
                            switch (from.DayOfWeek)
                            {
                                case DayOfWeek.Sunday:
                                    turns = _dataContext.Domenica;
                                    tipo = _dataContext.EDomenicaAperto ? Constants._CALENDARIO_OPEN : Constants._CALENDARIO_CLOSE;
                                    break;
                                case DayOfWeek.Monday:
                                    turns = _dataContext.Lunedi;
                                    tipo = _dataContext.ELunediAperto ? Constants._CALENDARIO_OPEN : Constants._CALENDARIO_CLOSE;
                                    break;
                                case DayOfWeek.Tuesday:
                                    turns = _dataContext.Martedi;
                                    tipo = _dataContext.EMartediAperto ? Constants._CALENDARIO_OPEN : Constants._CALENDARIO_CLOSE;
                                    break;
                                case DayOfWeek.Wednesday:
                                    turns = _dataContext.Mercoledi;
                                    tipo = _dataContext.EMercolediAperto ? Constants._CALENDARIO_OPEN : Constants._CALENDARIO_CLOSE;
                                    break;
                                case DayOfWeek.Thursday:
                                    turns = _dataContext.Giovedi;
                                    tipo = _dataContext.EGiovediAperto ? Constants._CALENDARIO_OPEN : Constants._CALENDARIO_CLOSE;
                                    break;
                                case DayOfWeek.Friday:
                                    turns = _dataContext.Venerdi;
                                    tipo = _dataContext.EVenerdiAperto ? Constants._CALENDARIO_OPEN : Constants._CALENDARIO_CLOSE;
                                    break;
                                case DayOfWeek.Saturday:
                                    turns = _dataContext.Sabato;
                                    tipo = _dataContext.ESabatoAperto ? Constants._CALENDARIO_OPEN : Constants._CALENDARIO_CLOSE;
                                    break;
                                default:
                                    turns = new ObservableCollection<GeneraCalendarioDayModel>();
                                    tipo = Constants._CALENDARIO_CLOSE;
                                    break;
                            }

                            //CALCOLO STOPS
                            TimePeriodCollection sourcePeriods = new TimePeriodCollection { new TimeRange(from.Date, from.Date.AddDays(1)) };
                            TimePeriodCollection subtractingPeriods = new TimePeriodCollection(turns?.Select(s => new TimeRange { Start = from.Date.Add(s.Dalle), End = from.Date.Add(s.Alle) }));
                            TimePeriodSubtractor<TimeRange> subtractor = new TimePeriodSubtractor<TimeRange>();
                            ITimePeriodCollection stops = subtractor.SubtractPeriods(sourcePeriods, subtractingPeriods);

                            foreach (var ris in _dataContext.SelectedRisorse ?? new ObservableCollection<tab_produzione_risorsa>())
                            {
                                if (tipo == Constants._CALENDARIO_OPEN)
                                {
                                    toSave.AddRange(turns?.Select(s => new tab_produzione_risorsa_calendario
                                    {
                                        SocietaID = _dataContext.CompanyID,
                                        RisorsaID = ris.ID,
                                        Giorno = from.Date,
                                        Dalle = from.Date.Add(s.Dalle),
                                        Alle = from.Date.Add(s.Alle),
                                        Tipo = tipo,
                                        LogAdded = DateTime.Now,
                                        LogAddedUserID = _dataContext.UserID,
                                    }));
                                    toSaveStops.AddRange(stops.Select(s => new tab_produzione_calendario_chiusura
                                    {
                                        SocietaID = _dataContext.CompanyID,
                                        RisorsaID = ris.ID,
                                        Dalle = s.Start,
                                        Alle = s.End,
                                        LogAdded = DateTime.Now,
                                        LogAddedUserID = _dataContext.UserID,
                                    }));
                                }
                                else
                                {
                                    toSave.Add(new tab_produzione_risorsa_calendario
                                    {
                                        SocietaID = _dataContext.CompanyID,
                                        RisorsaID = ris.ID,
                                        Giorno = from.Date,
                                        Dalle = from.Date,
                                        Alle = from.Date,
                                        Tipo = tipo,
                                        LogAdded = DateTime.Now,
                                        LogAddedUserID = _dataContext.UserID,
                                    });
                                    toSaveStops.Add(new tab_produzione_calendario_chiusura
                                    {
                                        SocietaID = _dataContext.CompanyID,
                                        RisorsaID = ris.ID,
                                        Dalle = from.Date,
                                        Alle = from.Date.AddDays(1),
                                        LogAdded = DateTime.Now,
                                        LogAddedUserID = _dataContext.UserID,
                                    });
                                }
                            }

                            from = from.AddDays(1);
                        }

                        _dataContext.Save(toSave, toSaveStops);
                    };
                    bgwOrders.RunWorkerAsync();
                    bgwOrders.RunWorkerCompleted += (s, e) =>
                    {
                        _dataContext.IsBusy = false;

                        this.Close();
                    };
                }
            }
        }
        #endregion

        #region Grid events
        private void rgvGiorni_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            e.NewObject = new GeneraCalendarioDayModel
            {
                ID = (sender as RadGridView)!.Items.Count + 1,
                Giorno = (DayOfWeek)(sender as RadGridView)!.Tag
            };
        }

        private void rgvGiorni_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var row = e.Row.DataContext as GeneraCalendarioDayModel;

            if (row != null)
            {
                var rows = (sender as RadGridView)!.Items.Cast<GeneraCalendarioDayModel>().Where(o => o.ID != row.ID).ToList();

                if (row.Alle <= row.Dalle)
                {
                    e.IsValid = false;
                }

                foreach (var rw in rows)
                {
                    bool noOverlap = row.Alle < rw.Dalle || row.Dalle > rw.Alle;

                    if (!noOverlap)
                    {
                        e.IsValid = false;
                        break;
                    }
                }
            }
        }

        private void rgvLunedi_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            if (_dataContext.EUsaComeDefault)
            {
                if (e.Row.DataContext is GeneraCalendarioDayModel)
                {
                    var day = (e.Row.DataContext as GeneraCalendarioDayModel);

                    if (day != null)
                    {
                        if (_dataContext.EMartediAperto)
                            _dataContext.Martedi?.Add(new GeneraCalendarioDayModel { ID = rgvMartedi.Items.Count + 1, Giorno = DayOfWeek.Tuesday, Dalle = day.Dalle, Alle = day.Alle });

                        if (_dataContext.EMercolediAperto)
                            _dataContext.Mercoledi?.Add(new GeneraCalendarioDayModel { ID = rgvMercoledi.Items.Count + 1, Giorno = DayOfWeek.Wednesday, Dalle = day.Dalle, Alle = day.Alle });

                        if (_dataContext.EGiovediAperto)
                            _dataContext.Giovedi?.Add(new GeneraCalendarioDayModel { ID = rgvGiovedi.Items.Count + 1, Giorno = DayOfWeek.Thursday, Dalle = day.Dalle, Alle = day.Alle });

                        if (_dataContext.EVenerdiAperto)
                            _dataContext.Venerdi?.Add(new GeneraCalendarioDayModel { ID = rgvVenerdi.Items.Count + 1, Giorno = DayOfWeek.Friday, Dalle = day.Dalle, Alle = day.Alle });

                        if (_dataContext.ESabatoAperto)
                            _dataContext.Sabato?.Add(new GeneraCalendarioDayModel { ID = rgvSabato.Items.Count + 1, Giorno = DayOfWeek.Saturday, Dalle = day.Dalle, Alle = day.Alle });

                        if (_dataContext.EDomenicaAperto)
                            _dataContext.Domenica?.Add(new GeneraCalendarioDayModel { ID = rgvDomenica.Items.Count + 1, Giorno = DayOfWeek.Sunday, Dalle = day.Dalle, Alle = day.Alle });
                    }
                }
            }
        }
        #endregion
    }
}
