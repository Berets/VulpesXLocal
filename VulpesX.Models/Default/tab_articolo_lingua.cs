namespace VulpesX.Models.Default;
 
public partial class tab_articolo_lingua : Base 
{
	private string _SocietaID = null!;
	public required string SocietaID { get => _SocietaID; set { if (_SocietaID != value) { _SocietaID = value; NotifyPropertyChanged();} } }
	private string _ArticoloID = null!;
	public required string ArticoloID { get => _ArticoloID; set { if (_ArticoloID != value) { _ArticoloID = value; NotifyPropertyChanged();} } }
	private string _lincod = null!;
	public required string lincod { get => _lincod; set { if (_lincod != value) { _lincod = value; NotifyPropertyChanged();} } }
	private string? _Descrizione;
	public string? Descrizione { get => _Descrizione; set { if (_Descrizione != value) { _Descrizione = value; NotifyPropertyChanged();} } }
	private string? _Note;
	public string? Note { get => _Note; set { if (_Note != value) { _Note = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}