namespace VulpesX.Models.Default;
 
public partial class COMUNI : Base 
{
	private string _comdes = null!;
	public required string comdes { get => _comdes; set { if (_comdes != value) { _comdes = value; NotifyPropertyChanged();} } }
	private int? _comist;
	public int? comist { get => _comist; set { if (_comist != value) { _comist = value; NotifyPropertyChanged();} } }
	private string? _comcod;
	public string? comcod { get => _comcod; set { if (_comcod != value) { _comcod = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}