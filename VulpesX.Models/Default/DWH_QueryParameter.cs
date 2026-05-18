namespace VulpesX.Models.Default;
 
public partial class DWH_QueryParameter : Base 
{
	private string _SocietaID = null!;
	public required string SocietaID { get => _SocietaID; set { if (_SocietaID != value) { _SocietaID = value; NotifyPropertyChanged();} } }
	private Guid _ID;
	public Guid ID { get => _ID; set { if (_ID != value) { _ID = value; NotifyPropertyChanged();} } }
	private string _Nome = null!;
	public required string Nome { get => _Nome; set { if (_Nome != value) { _Nome = value; NotifyPropertyChanged();} } }
	private int? _Posizione;
	public int? Posizione { get => _Posizione; set { if (_Posizione != value) { _Posizione = value; NotifyPropertyChanged();} } }
	private int? _Tipo;
	public int? Tipo { get => _Tipo; set { if (_Tipo != value) { _Tipo = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}