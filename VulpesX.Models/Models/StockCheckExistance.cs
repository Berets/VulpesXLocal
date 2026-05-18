using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace VulpesX.Models.Models
{
    public class StockCheckExistance : Base
    {
        public class Lots
        {
            public string? CompanyID { get; set; }
            public string? ProductID { get; set; }
            public string? ProductDescription { get; set; }
            public string? StoreID { get; set; }
            public string? StoreDescription { get; set; }
            public string? Lot { get; set; }
            public decimal QuantityLot { get; set; }
            public decimal QuantityStore { get; set; }
            public decimal QuantityMovementsLot { get; set; }
            public decimal QuantityMovementsStore { get; set; }
            public decimal GapLot { get; set; }
            public decimal GapStore { get; set; }
            public decimal GapLotStore { get; set; }
        }

        public class NoLots
        {
            public string? CompanyID { get; set; }
            public string? ProductID { get; set; }
            public string? ProductDescription { get; set; }
            public string? StoreID { get; set; }
            public string? StoreDescription { get; set; }
            public decimal Quantity { get; set; }
            public decimal QuantityMovements { get; set; }
            public decimal QuantityGap { get { return Quantity - QuantityMovements; } }
        }
    }
}
