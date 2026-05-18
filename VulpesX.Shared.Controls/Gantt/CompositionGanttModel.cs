using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls.GanttView;
using VulpesX.Models.Default;

namespace VulpesX.Shared.Controls.Gantt
{
    public class CompositionGanttModel : GanttTask
    {
        public required pro_ordine_composizione Data { get; set; }


        public static CompositionGanttModel FromData(pro_ordine_composizione data)
        {
            var task = new CompositionGanttModel
            {
                Data = data,
                Title = data.Title, // Example: set task content
                Start = data.Start,
                End = data.End,
            };

            foreach (var child in data.Children)
            {
                var childTask = FromData(child);
                task.Children.Add(childTask);
            }

            foreach (var child in data.Dependencies)
            {
                task.Dependencies.Add(new Dependency { FromTask = task });
            }

            return task;
        }
    }
}
