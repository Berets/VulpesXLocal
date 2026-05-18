 

namespace VulpesX.Models.Default
{
    public partial class ACC_EINVOICE_EXPIRES
    {
        public string? PaymentConditionDescription => CommonsService.FEPaymentConditions.Where(w => w.ID == fattcond).FirstOrDefault()?.FullDescriptionNotSearchable;
        public FE_PAGDOC? PaymentType { get; set; }
        public TAB_ACC_TIPPAG? PaymentTypeInternal { get; set; }
    }
}
