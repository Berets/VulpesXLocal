namespace VulpesX.Models.Default
{
    public partial class acq_orders_rows_jobs
    {
        public decimal QuantityAssigned { get; set; }
        public decimal QuantityFreeForOrders => quantity_needed - quantity_received;
    }
}