namespace VulpesX.Models.Default;
 
public partial class AFFIDABILITA : Base 
{
	private string _affcod = null!;
	public required string affcod { get => _affcod; set { if (_affcod != value) { _affcod = value; NotifyPropertyChanged();} } }
	private string _affdes = null!;
	public required string affdes { get => _affdes; set { if (_affdes != value) { _affdes = value; NotifyPropertyChanged();} } }
	private string? _afford;
	public string? afford { get => _afford; set { if (_afford != value) { _afford = value; NotifyPropertyChanged();} } }
	private string? _afffat;
	public string? afffat { get => _afffat; set { if (_afffat != value) { _afffat = value; NotifyPropertyChanged();} } }
	private string? _affrib;
	public string? affrib { get => _affrib; set { if (_affrib != value) { _affrib = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}