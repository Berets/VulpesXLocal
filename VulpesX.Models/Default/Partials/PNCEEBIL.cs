namespace VulpesX.Models.Default
{
    public partial class PNCEEBIL
    {
        public string FullDescriptionSearchable => $"{ceevo1?.Trim()} {ceevo2?.Trim()} {ceevo3?.Trim()} {ceevo4?.Trim()} {ceevo5?.Trim()} {ceevo6?.Trim()} {ceevo7?.Trim()}";

        public string Description => $"{ceedes?.Trim()}";
        public string DescriptionExtended => $"{ceedee?.Trim()}";
    }
}
