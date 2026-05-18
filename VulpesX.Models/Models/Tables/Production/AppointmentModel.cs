using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.Tables.Production
{
    public class AppointmentModel
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public bool Importance { get; set; }
    }
}
