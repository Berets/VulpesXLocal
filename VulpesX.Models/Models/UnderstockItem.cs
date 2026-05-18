using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models
{
    public class UnderstockItem
    {
        public required string ID { get; set; }
        public string? Description { get; set; }
        public string? UM { get; set; }
        public string? SupplierPID { get; set; }
        public string? SupplierPDescription { get; set; }
        public int? DefaultSupplierID { get; set; }
        public string? DefaultSupplierDescription { get; set; }
        public decimal QuantityAvailable { get; set; }
        public decimal QuantityMinimum { get; set; }
        public decimal QuantityRestock { get; set; }
        public decimal QuantityOrdered { get; set; }
    }
}
