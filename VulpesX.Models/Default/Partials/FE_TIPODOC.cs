namespace VulpesX.Models.Default
{
    public partial class FE_TIPODOC
    {
        public string FullDescriptionSearchable => $"{FETDCod} {FETDDes?.Trim()}";
        public string FullDescriptionNotSearchable => $"[{FETDCod}] {FETDDes?.Trim()}";
    }
}
