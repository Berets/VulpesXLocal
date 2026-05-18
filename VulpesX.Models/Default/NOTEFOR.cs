namespace VulpesX.Models.Default;
 
public partial class NOTEFOR : Base 
{
	private int _Nofcod;
	public int Nofcod { get => _Nofcod; set { if (_Nofcod != value) { _Nofcod = value; NotifyPropertyChanged();} } }
	private int _nofrig;
	public int nofrig { get => _nofrig; set { if (_nofrig != value) { _nofrig = value; NotifyPropertyChanged();} } }
	private string? _Nofnot;
	public string? Nofnot { get => _Nofnot; set { if (_Nofnot != value) { _Nofnot = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}