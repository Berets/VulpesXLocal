namespace VulpesX.Models.Default
{
    public partial class tab_articolo_unita
    {
        public string FullDescriptionSearchable => $"{ID} {Descrizione}";
    }
}
