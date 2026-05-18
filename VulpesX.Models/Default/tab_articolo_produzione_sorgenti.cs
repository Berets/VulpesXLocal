namespace VulpesX.Models.Default;
 
public partial class tab_articolo_produzione_sorgenti : Base 
{
	private string _SocietaID = null!;
	public required string SocietaID { get => _SocietaID; set { if (_SocietaID != value) { _SocietaID = value; NotifyPropertyChanged();} } }
	private string _ArticoloID = null!;
	public required string ArticoloID { get => _ArticoloID; set { if (_ArticoloID != value) { _ArticoloID = value; NotifyPropertyChanged();} } }
	private string _RisorsaID = null!;
	public required string RisorsaID { get => _RisorsaID; set { if (_RisorsaID != value) { _RisorsaID = value; NotifyPropertyChanged();} } }
	private string _SorgenteID = null!;
	public required string SorgenteID { get => _SorgenteID; set { if (_SorgenteID != value) { _SorgenteID = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}