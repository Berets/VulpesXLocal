namespace VulpesX.Models.Default
{
    public partial class ACC_PLAFOND_ROWS
    {
        public int InvoiceDefinitiveNumber { get; set; }

        public string? InvoiceTypeID { get; set; }
        public string? InvoiceTypeDescription { get { return CommonsService.InvoiceTypesUfp.Where(o => o.ID == InvoiceTypeID).Select(s => s.Description).FirstOrDefault(); } }
    }
}
