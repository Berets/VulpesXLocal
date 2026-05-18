namespace VulpesX.Models.Models.Reports.Accounting
{
    public class VATRecap
    {
        public required string RateFullDescription { get; set; }
        public bool IsSplitPayment { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal TotalVATAmount { get; set; }
        public decimal TotalNoVATAmount { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (!(obj is VATRecap))
                return false;

            return (obj as VATRecap)!.RateFullDescription == RateFullDescription;
        }

        public override int GetHashCode()
        {
            return RateFullDescription.GetHashCode();
        }
    }
}
