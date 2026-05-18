using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Commons
{
    public class SingleDateWindowViewModel : Base
    {
        public DateTime? SelectedDate { get; set; }
        public bool AllowNullDate { get; set; }
    }
}
