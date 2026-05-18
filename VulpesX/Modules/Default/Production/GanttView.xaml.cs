using Itenso.TimePeriod;
using Microsoft.Extensions.DependencyInjection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls.Scheduling;
using VulpesX.Shared;
using VulpesX.Shared.Controls.Gantt;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Production;

namespace VulpesX.Modules.Default.Production
{
    /// <summary>
    /// Interaction logic for GanttView.xaml
    /// </summary>
    public partial class GanttView : UserControl
    {
        private GanttViewModel _dataContext;
        private ObservableCollection<CompositionGanttModel>? _tasks;

        public GanttView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<GanttViewModel>();

            InitializeComponent();

            _dataContext.VisibleStartTime = DateTime.Now.AddDays(-3);
            _dataContext.VisibleEndTime = DateTime.Now.AddDays(30);
            _dataContext.PixelLength = new TimeSpan(0, 30, 0);

            this.DataContext = _dataContext;

            rgvOrders.TimeLineVisualizationBehavior = new TimeLineBaselineBehavior();

            this.Loaded += (s, e) =>
            {
                LoadData();
            };
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
                LoadData();
        }

        #region Private methods

        private async void LoadData()
        {
            await _dataContext.Load();

            if (_dataContext.Tasks != null)
            {
                _tasks = new ObservableCollection<CompositionGanttModel>(_dataContext.Tasks.Select(CompositionGanttModel.FromData).ToList());

                rgvOrders.VisibleRange = new DateRange(_dataContext.VisibleStartTime, _dataContext.VisibleEndTime);
                rgvOrders.TasksSource = _tasks;
            }
            //_dataContext.RisorseGantt = new ObservableCollection<ucGanttSingle>();
            _dataContext.Risorse = _dataContext.GetListFullInfo();

            ExpandCollapseExecute();
        }

        private void ExpandCollapseExecute()
        {
            if (_dataContext.ExpandedTasks != null && _dataContext.ExpandedTasks.Any())
            {
                foreach (var task in rgvOrders.TasksSource.Cast<CompositionGanttModel>())
                {
                    if (_dataContext.ExpandedTasks.Where(o => o.UniqueId == task.UniqueId).Any())
                        rgvOrders.ExpandCollapseService.ExpandItem(task);
                    else
                        rgvOrders.ExpandCollapseService.CollapseItem(task);

                    ExpandCollapseExecuteChilds(task.Children.Cast<CompositionGanttModel>().ToList());
                }
            }
            else
            {
                foreach (var task in rgvOrders.TasksSource.Cast<CompositionGanttModel>())
                {
                    rgvOrders.ExpandCollapseService.CollapseItem(task);

                    ExpandCollapseExecuteChilds(task.Children.Cast<CompositionGanttModel>().ToList());
                }
            }
        }

        private void ExpandCollapseExecuteChilds(List<CompositionGanttModel> Tasks)
        {
            if (_dataContext.ExpandedTasks != null && _dataContext.ExpandedTasks.Any())
            {
                foreach (var task in Tasks)
                {
                    if (_dataContext.ExpandedTasks.Where(o => o.UniqueId == task.UniqueId).Any())
                        rgvOrders.ExpandCollapseService.ExpandItem(task);
                    else
                        rgvOrders.ExpandCollapseService.CollapseItem(task);

                    ExpandCollapseExecuteChilds(task.Children.Cast<CompositionGanttModel>().ToList());
                }
            }
            else
            {
                foreach (var task in Tasks)
                {
                    rgvOrders.ExpandCollapseService.CollapseItem(task);

                    ExpandCollapseExecuteChilds(task.Children.Cast<CompositionGanttModel>().ToList());
                }
            }
        }

        private void Expand(List<CompositionGanttModel> Tasks)
        {
            foreach (var task in Tasks)
            {
                rgvOrders.ExpandCollapseService.ExpandItem(task);

                Expand(task.Children.Cast<CompositionGanttModel>().ToList());
            }
        }

        private void Collapse(List<CompositionGanttModel> Tasks)
        {
            foreach (var task in Tasks)
            {
                rgvOrders.ExpandCollapseService.CollapseItem(task);

                Collapse(task.Children.Cast<CompositionGanttModel>().ToList());
            }
        }
        #endregion

        #region Various events
        private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _dataContext.PixelLength = TimeSpan.FromTicks((long)(double)e.NewValue);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void btnExpandAll_Click(object sender, RoutedEventArgs e)
        {
            if (_tasks != null)
            {
                Expand(_tasks.ToList());
            }
        }

        private void btnCollapseAll_Click(object sender, RoutedEventArgs e)
        {
            if (_tasks != null)
            {
                Collapse(_tasks.ToList());
            }
        }

        private void btnSearchUp_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.Tasks != null && !string.IsNullOrEmpty(rwtSearch.Text) && rgvOrders.SelectedItem != null)
            {
                var selected = rgvOrders.TasksSource.Cast<pro_ordine_composizione>().Where(o => o.UniqueId == ((pro_ordine_composizione)rgvOrders.SelectedItem).UniqueId).FirstOrDefault();

                if (selected != null)
                {
                    var founded = rgvOrders.TasksSource.Cast<pro_ordine_composizione>().Where(o => o.PosizioneGantt < selected.PosizioneGantt && (o.Description ?? string.Empty).ToLower().Contains(rwtSearch.Text.ToLower())).OrderByDescending(o => o.PosizioneGantt).FirstOrDefault();

                    if (founded == null)
                    {
                        ErrorHandler.Validation("Nessun elemento trovato");
                    }
                    else
                    {
                        rgvOrders.SelectedItem = founded;
                    }
                }
            }
        }

        private void btnSearchDown_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.Tasks != null && !string.IsNullOrEmpty(rwtSearch.Text) && rgvOrders.SelectedItem != null)
            {
                var selected = rgvOrders.TasksSource.Cast<pro_ordine_composizione>().Where(o => o.UniqueId == ((pro_ordine_composizione)rgvOrders.SelectedItem).UniqueId).FirstOrDefault();

                if (selected != null)
                {
                    var founded = rgvOrders.TasksSource.Cast<pro_ordine_composizione>().Where(o => o.PosizioneGantt > selected.PosizioneGantt && (o.Description ?? string.Empty).ToLower().Contains(rwtSearch.Text.ToLower())).FirstOrDefault();

                    if (founded == null)
                    {
                        ErrorHandler.Validation("Nessun elemento trovato");
                    }
                    else
                    {
                        rgvOrders.SelectedItem = founded;
                    }
                }
            }
        }

        private void rwtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_dataContext.Tasks != null)
            {
                string? search = (e.Source as RadWatermarkTextBox)?.Text;

                var founded = rgvOrders.TasksSource.Cast<pro_ordine_composizione>().Where(o => (o.Description ?? string.Empty).ToLower().Contains(search?.ToLower() ?? string.Empty)).FirstOrDefault();

                if (founded == null)
                {
                    ErrorHandler.Validation("Nessun elemento trovato");
                }
                else
                {
                    rgvOrders.SelectedItem = founded;
                }
            }
        }

        private void rwtSearchResource_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //if (_dataContext.RisorseGantt != null)
            //{
            //    string search = (e.Source as RadWatermarkTextBox).Text;

            //    if (!string.IsNullOrEmpty(search))
            //    {
            //        itmRisorse.ItemsSource = new ObservableCollection<tab_produzione_risorsa>(model.Risorse.Where(o => o.Descrizione.ToLower().Contains(search.ToLower())));
            //    }
            //    else
            //    {
            //        itmRisorse.ItemsSource = model.Risorse;
            //    }
            //}
        }

        private void chkRisorsa_Checked(object sender, RoutedEventArgs e)
        {
            //var dataContext = (sender as RadToggleSwitchButton).DataContext as tab_produzione_risorsa;

            //var ucGanttSingle = new ucGanttSingle(dataContext, new ObservableCollection<pro_ordine_composizione>(model.TasksResources.Where(o => o.ResourceID == dataContext.ID)), model.Stops);
            //ucGanttSingle.Tag = dataContext.ID;

            //model.RisorseGantt.Add(ucGanttSingle);
        }

        private void chkRisorsa_Unchecked(object sender, RoutedEventArgs e)
        {
            //var dataContext = (sender as RadToggleSwitchButton).DataContext as tab_produzione_risorsa;

            //var ucGanttSingle = model.RisorseGantt.Where(o => o.Tag.ToString() == dataContext.ID).FirstOrDefault();

            //if (ucGanttSingle != null)
            //    model.RisorseGantt.Remove(ucGanttSingle);
        }
        #endregion

        #region Gantt events
        private void rgvOrders_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Vulpes.UIX.Core.Utilities.TelerikGrid.RadialMenu(rgvOrders, e);
        }
        #endregion

        private List<Tuple<string, TimePeriodCollection, TimePeriodCollection>> LoadStacks()
        {
            var retValue = new List<Tuple<string, TimePeriodCollection, TimePeriodCollection>>();

            var risorse = _dataContext.GetTab_Produzione_Risorsas();

            foreach (var ris in risorse ?? new ObservableCollection<tab_produzione_risorsa>())
            {
                var stops = _dataContext.GetTab_Produzione_Calendario_Chiusuras(ris.ID);

                if (stops != null && stops.Any())
                {
                    TimePeriodCollection sourcePeriods = new TimePeriodCollection { new TimeRange(DateTime.Now, DateTime.Now.AddYears(1)) };

                    TimePeriodCollection subtractingPeriods = new TimePeriodCollection(stops.Select(s => new TimeRange { Start = s.Dalle, End = s.Alle }));

                    retValue.Add(new Tuple<string, TimePeriodCollection, TimePeriodCollection>(ris.ID, sourcePeriods, subtractingPeriods));
                }
            }

            return retValue;
        }


        private void Recalculate(pro_ordine_composizione Composizione, ref List<pro_ordine_composizione> Planned, ref List<Tuple<string, TimePeriodCollection, TimePeriodCollection>> Stacks)
        {
            foreach (var chl in Composizione.Children.Cast<pro_ordine_composizione>())
            {
                // ARRIVA IN FONDO
                foreach (var sub in chl.Children.Cast<pro_ordine_composizione>())
                {
                    Recalculate(sub, ref Planned, ref Stacks);
                }

                if (chl.EReparto)
                {
                    DateTime start = DateTime.Now;
                    DateTime end = DateTime.Now.Add(chl.Duration);

                    if (chl.Dependencies.Any())
                    {
                        // CAPIRE SE SOLO UNA
                        var dependency = Composizione.Dependencies.FirstOrDefault();

                        if (dependency != null)
                        {
                            var poc = dependency.FromTask as pro_ordine_composizione;
                            var pol = Planned.Where(o => o.UID == poc.UID).FirstOrDefault();

                            if (pol != null)
                            {
                                if (pol.Children.Any())
                                {
                                    var stack = Stacks.Where(o => o.Item1 == chl.RisorsaID).FirstOrDefault();
                                    var ranges = new TimePeriodSubtractor<TimeRange>().SubtractPeriods(stack?.Item2, stack?.Item3);

                                    start = pol.Children.Cast<pro_ordine_composizione>().Max(o => o.End);
                                    end = start.Add(chl.EffectiveDuration);
                                }
                                else
                                {
                                    var stack = Stacks.Where(o => o.Item1 == chl.RisorsaID).FirstOrDefault();
                                    var ranges = new TimePeriodSubtractor<TimeRange>().SubtractPeriods(stack?.Item2, stack?.Item3);

                                    start = chl.End;
                                    end = start.Add(chl.EffectiveDuration);
                                }
                            }
                        }
                    }
                    else
                    {
                        var stack = Stacks.Where(o => o.Item1 == Composizione.RisorsaID).FirstOrDefault();
                        if (stack != null)
                        {
                            var ranges = new TimePeriodSubtractor<TimeRange>().SubtractPeriods(stack?.Item2, stack?.Item3);
                        }
                    }

                    chl.Start = start;
                    chl.End = end;

                    Planned.Add(chl);
                }

                if (chl.Children.Any())
                {
                    chl.Start = chl.Children.Cast<pro_ordine_composizione>().Min(o => o.Start);
                    chl.End = chl.Children.Cast<pro_ordine_composizione>().Max(o => o.End);

                    Planned.Add(chl);
                }
            }

            if (Composizione.EReparto)
            {
                DateTime start = DateTime.Now;
                DateTime end = DateTime.Now.Add(Composizione.Duration);

                if (Composizione.Dependencies.Any())
                {
                    // CAPIRE SE SOLO UNA
                    var dependency = Composizione.Dependencies.FirstOrDefault();

                    if (dependency != null)
                    {
                        var poc = dependency.FromTask as pro_ordine_composizione;
                        var pol = Planned.Where(o => o.UID == poc.UID).FirstOrDefault();

                        if (pol != null)
                        {
                            if (pol.Children.Any())
                            {
                                var stack = Stacks.Where(o => o.Item1 == Composizione.RisorsaID).FirstOrDefault();

                                var ranges = new TimePeriodSubtractor<TimeRange>().SubtractPeriods(stack?.Item2, stack?.Item3);

                                start = pol.Children.Cast<pro_ordine_composizione>().Max(o => o.End);
                                end = start.Add(Composizione.EffectiveDuration);
                            }
                            else
                            {
                                var stack = Stacks.Where(o => o.Item1 == Composizione.RisorsaID).FirstOrDefault();
                                var ranges = new TimePeriodSubtractor<TimeRange>().SubtractPeriods(stack?.Item2, stack?.Item3);

                                start = pol.End;
                                end = start.Add(Composizione.EffectiveDuration);
                            }
                        }
                    }
                }
                else
                {
                    var stack = Stacks.Where(o => o.Item1 == Composizione.RisorsaID).FirstOrDefault();
                    var ranges = new TimePeriodSubtractor<TimeRange>().SubtractPeriods(stack?.Item2, stack?.Item3);
                }

                Composizione.Start = start;
                Composizione.End = end;

                Planned.Add(Composizione);
            }

            if (Composizione.Children.Any())
            {
                Composizione.Start = Composizione.Children.Cast<pro_ordine_composizione>().Min(o => o.Start);
                Composizione.End = Composizione.Children.Cast<pro_ordine_composizione>().Max(o => o.End);

                Planned.Add(Composizione);
            }
        }


        private void btnRecalculate_Click(object sender, RoutedEventArgs e)
        {
            CalendarDateAdd calendarDateAdd = new CalendarDateAdd();
            // holidays
            calendarDateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2024, 9, 28, 18, 0, 0), new DateTime(2024, 9, 29, 0, 0, 0)));
            calendarDateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2024, 9, 29, 0, 0, 0), new DateTime(2024, 9, 30, 0, 0, 0)));
            calendarDateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2024, 9, 30, 0, 0, 0), new DateTime(2024, 10, 1, 8, 0, 0)));
            calendarDateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2024, 10, 1, 16, 0, 0), new DateTime(2024, 10, 2, 0, 0, 0)));

            DateTime start = DateTime.Now;
            TimeSpan offset = new TimeSpan(4, 0, 0); // 22 hours

            DateTime? end = calendarDateAdd.Add(start, offset);

            var orders = rgvOrders.TasksSource.Cast<pro_ordine_composizione>();
            var planned = new List<pro_ordine_composizione>();

            var stacks = LoadStacks();

            foreach (var ord in orders)
            {
                foreach (var roo in ord.Children.Cast<pro_ordine_composizione>())
                {
                    Recalculate(roo, ref planned, ref stacks);
                }

            }

            foreach (var pla in planned)
            {
                pla.Duration = pla.End.Subtract(pla.Start);
            }

            //var list = new List<JsonAI.Order>();

            //foreach (var ord in orders)
            //{
            //    var order = new JsonAI.Order { OrderID = ord.OrdineID, DeliveryDate = ord.DataConsegna ?? DateTime.Now, Tasks = new List<JsonAI.Tasks>() };
            //    order.Tasks = Iterate(ord);
            //    list.Add(order);
            //}

            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
            //{
            //    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            //});


            //File.WriteAllText(@"C:\Users\Matteo.GXITALIA\Desktop\test.json", json);
        }
    }
}
