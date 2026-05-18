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
using Telerik.Windows.Diagrams.Core;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.ViewModels.Modules.Default.Production;
using static VulpesX.Shared.Controls.Graph.LotTrackingModel;

namespace VulpesX.Modules.Default.Production
{
    /// <summary>
    /// Interaction logic for LotManagerTrackingWindow.xaml
    /// </summary>
    public partial class LotManagerTrackingWindow : FluentDefaultWindow
    {
        private LotManagerTrackingWindowViewModel _dataContext;
        public LotManagerTrackingWindow(LotManagerTrackingWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Width = System.Windows.SystemParameters.WorkArea.Width;
            this.Height = System.Windows.SystemParameters.WorkArea.Height - 200;

            this.DataContext = _dataContext;

            this.Loaded += async (s, e) =>
            {
                diaTracking.GraphSourceChanged += DiaTracking_GraphSourceChanged;
                await LoadData();
            };
        }

        private void DiaTracking_GraphSourceChanged(object? sender, EventArgs e)
        {
            if (!diaTracking.Items.Any())
                return;

            var firstItem = diaTracking.Items[0];

            var settings = new TreeLayoutSettings
            {
                TreeLayoutType = TreeLayoutType.TreeDown,
                RadialFirstLEvelSeparation = 1,
                HorizontalSeparation = 1,
                VerticalSeparation = 20
            };

            var root = this.diaTracking.ContainerGenerator
                .ContainerFromItem(firstItem) as RadDiagramShape;

            if (root != null)
                settings.Roots.Add(root);

            this.diaTracking.LayoutAsync(LayoutType.Tree, settings);

            this.diaTracking.AutoFitAsync(new Thickness(0), true);

            // Center AFTER layout
            if (root != null)
            {
                this.diaTracking.Dispatcher.InvokeAsync(() =>
                {
                    this.diaTracking.BringIntoView(root, 1, true);
                });
            }

        }

        public async Task LoadData()
        {
            await _dataContext.Load();

            var data = new NodeModel();

            if (_dataContext.LotTracking != null)
            {
                bool first = true;
                NodeModel? lastNode = null;
                foreach (var ll in _dataContext.LotTracking.LinkedLots ?? new())
                {
                    if (first)
                    {
                        data.TypeID = "ALOT";
                        data.TypeDescription = "Lotto automatico";
                        data.Lot = ll.ID;
                        data.CreatedText = ll.added.ToString("dd/MM/yyyy HH:mm:ss");
                        data.ResourceDescription = ll.ProductionResource?.FullDescriptionSearchable;
                        data.OperatorDescription = ll.Operator?.FullDescriptionSearchable;
                        data.DurationText = ll.ProductionTime?.DurataSpan.ToString();
                        data.Nodes = new ObservableCollection<NodeModel>();
                        data.MainBrush = _dataContext.LotTracking.Lot?.ID == ll.ID ? "Y" : "B";
                        first = false;
                        lastNode = data;
                    }
                    else
                    {
                        lastNode = new NodeModel()
                        {
                            TypeID = "ALOT",
                            TypeDescription = "Lotto semilavorato",
                            Lot = ll.ID,
                            CreatedText = ll.added.ToString("dd/MM/yyyy HH:mm:ss"),
                            OperatorDescription = ll.Operator?.FullDescriptionSearchable,
                            ResourceDescription = ll.ProductionResource?.FullDescriptionSearchable,
                            DurationText = ll.ProductionTime?.DurataSpan.ToString(),
                            Nodes = new ObservableCollection<NodeModel>(),
                            MainBrush = _dataContext.LotTracking.Lot?.ID == ll.ID ? "Y" : "B"
                        };

                        data.Nodes.Add(lastNode);
                    }
                }

                var newNode = new ProductionOrderNodeModel()
                {
                    TypeID = "PORD",
                    TypeDescription = "Ordine di produzione",
                    ID = _dataContext.LotTracking.ProductionOrder?.ID,
                    CreatedText = _dataContext.LotTracking.ProductionOrder?.LogAdded?.ToString("dd/MM/yyyy HH:mm:ss"),
                    UserID = _dataContext.LotTracking.ProductionOrder?.LogAddedUserID,
                    Nodes = new ObservableCollection<NodeModel>()
                };
                //lastNode.Nodes.Add(newNode);
                //lastNode = newNode;

                foreach (var his in (_dataContext.LotTracking.ProductionHistory ?? new()).OrderBy(o => o.istant))
                {
                    var newHistoryNode = new HistoryNodeModel()
                    {
                        TypeID = "PORH",
                        TypeDescription = "Storico lanci produzione",
                        CreatedText = _dataContext.LotTracking.ProductionOrder?.LogAdded?.ToString("dd/MM/yyyy HH:mm:ss"),
                        UserID = _dataContext.LotTracking.ProductionOrder?.LogAddedUserID,
                        Nodes = new ObservableCollection<NodeModel>()
                    };
                    //lastNode.Nodes.Add(newHistoryNode);
                    //lastNode = newHistoryNode;
                }
            }


            var graphData = new TrackingGraphSource();
            graphData.PopulateGraphSource(data);

            diaTracking.GraphSource = graphData;


        }
    }
}
