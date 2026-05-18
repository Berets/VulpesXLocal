namespace VulpesX.Models.Default;

public partial class pro_ordine_lotti
{
    public pro_ordine_composizione_tempo? ProductionTime { get; set; }
    public tab_articolo? Product { get; set; }
    public tab_produzione_risorsa? ProductionResource { get; set; }
    public tab_produzione_operatore? Operator { get; set; }

    public string ExpireDateText => ExpireDate.HasValue ? $"{ExpireDate.Value.ToString("dd/MM/yyyy")}" : "**********";
    public string QuantityText => $"{(ProductionTime?.QuantitaVersata ?? 0).ToString("N6")} {Product?.UnitaID}";
    #region Info
    public string AddedText => added.ToLocalTime().ToString();
    public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
    public string UpdatedText => updated.HasValue ? updated.Value.ToLocalTime().ToString() : "---";
    public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
    public string CanceledText => canceled.HasValue ? canceled.Value.ToLocalTime().ToString() : "---";
    public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
    #endregion
}
