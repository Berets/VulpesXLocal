namespace VulpesX.Models.Default
{
    public partial class cost_data
    {
        public decimal? TempoOreTotali
        {
            get
            {
                if (Tempo.HasValue)
                    return (decimal)new TimeSpan(Tempo.Value).TotalHours;
                else
                    return 0;
            }
        }

        public decimal? CostoUnitario
        {
            get
            {
                return TempoCosto != null ? TempoCosto : QuantitaCosto;
            }
        }
    }
}
