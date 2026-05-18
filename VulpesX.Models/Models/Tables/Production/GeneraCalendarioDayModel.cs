using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.Tables.Production
{
    public class GeneraCalendarioDayModel
    {
        public int ID { get; set; }
        public DayOfWeek Giorno { get; set; }
        public TimeSpan Dalle { get; set; }
        public TimeSpan Alle { get; set; }
    }
}
