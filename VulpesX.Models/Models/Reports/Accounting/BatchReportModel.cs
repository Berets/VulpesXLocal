using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.Reports.Accounting
{
    public class BatchReportModel
    {
        public class EntityModel
        {
            public int EntityID { get; set; }
            public string? EntityDescription { get; set; }
            public decimal? InitialValue { get; set; }

            public List<MovementModel>? Movements { get; set; }
        }

        public class MovementModel
        {
            public int Year { get; set; }
            public int ID { get; set; }
            public int Row { get; set; }
            public DateTime? Date { get; set; }
            public decimal Import { get; set; }
            public string? Sign { get; set; }

            public string? DocumentID { get; set; }
            public DateTime? DocumentDate { get; set; }

            public string? ReferenceID { get; set; }
            public DateTime? ReferenceDate { get; set; }
        }
    }
}
