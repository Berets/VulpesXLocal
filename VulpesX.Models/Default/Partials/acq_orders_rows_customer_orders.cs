namespace VulpesX.Models.Default
{
    public partial class acq_orders_rows_customer_orders
    {
        public string FullID => $"{customer_order_year}/{customer_order_number}/{customer_order_row}";
        public decimal QuantityAssigned { get; set; }
        public decimal QuantityFreeForOrders => quantity_needed - quantity_received;
    }
}
