using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls.Diagrams.Extensions.ViewModels;

namespace VulpesX.Shared.Controls.Graph
{
    public class LotTrackingModel
    {
        public class NodeModel : HierarchicalNodeViewModel
        {
            public string? TypeID { get; set; }
            public string? TypeDescription { get; set; }
            public string? Lot { get; set; }
            public string? ID { get; set; }
            public string? CreatedText { get; set; }
            public string? ResourceDescription { get; set; }
            public string? OperatorDescription { get; set; }
            public string? DurationText { get; set; }
            public string? MainBrush { get; set; }
            public ObservableCollection<NodeModel> Nodes { get; set; } = new();
        }
        public class ProductionOrderNodeModel : HierarchicalNodeViewModel
        {
            public string? TypeID { get; set; }
            public string? TypeDescription { get; set; }
            public string? ID { get; set; }
            public string? UserID { get; set; }
            public string? CreatedText { get; set; }
            public ObservableCollection<NodeModel> Nodes { get; set; } = new();
            public string? MainBrush { get; set; }
        }
        public class HistoryNodeModel : HierarchicalNodeViewModel
        {
            public string? TypeID { get; set; }
            public string? TypeDescription { get; set; }
            public string? CreatedText { get; set; }
            public string? UserID { get; set; }
            public string? DurationText { get; set; }
            public string? MainBrush { get; set; } 
            public ObservableCollection<NodeModel> Nodes { get; set; } = new();
        }
        public class LinkModel : LinkViewModelBase<NodeModel>
        {
            public LinkModel(NodeModel source, NodeModel target) : base(source, target)
            {
            }
        }
        public class TrackingGraphSource : ObservableGraphSourceBase<NodeModel, LinkModel>
        {
            public void PopulateGraphSource(NodeModel node)
            {
                this.AddNode(node);

                node.Children.Clear();

                if (node.Nodes != null)
                {
                    foreach (var subNode in node.Nodes)
                    {
                        LinkModel link = new LinkModel(node, subNode);
                        this.AddLink(link);
                        this.PopulateGraphSource(subNode);
                    }
                }
            }
        }
    }
}
