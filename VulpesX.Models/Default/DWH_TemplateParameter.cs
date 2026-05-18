namespace VulpesX.Models.Default;
 
public partial class DWH_TemplateParameter : Base 
{
	private string _SocietaID = null!;
	public required string SocietaID { get => _SocietaID; set { if (_SocietaID != value) { _SocietaID = value; NotifyPropertyChanged();} } }
	private Guid _QueryID;
	public Guid QueryID { get => _QueryID; set { if (_QueryID != value) { _QueryID = value; NotifyPropertyChanged();} } }
	private Guid _ID;
	public Guid ID { get => _ID; set { if (_ID != value) { _ID = value; NotifyPropertyChanged();} } }
	private string _Nome = null!;
	public required string Nome { get => _Nome; set { if (_Nome != value) { _Nome = value; NotifyPropertyChanged();} } }
	private string? _Valore;
	public string? Valore { get => _Valore; set { if (_Valore != value) { _Valore = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}