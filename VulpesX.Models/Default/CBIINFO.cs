namespace VulpesX.Models.Default;
 
public partial class CBIINFO : Base 
{
	private string _CBISoc = null!;
	public required string CBISoc { get => _CBISoc; set { if (_CBISoc != value) { _CBISoc = value; NotifyPropertyChanged();} } }
	private int _CBIPro;
	public int CBIPro { get => _CBIPro; set { if (_CBIPro != value) { _CBIPro = value; NotifyPropertyChanged();} } }
	private string? _CBIFil;
	public string? CBIFil { get => _CBIFil; set { if (_CBIFil != value) { _CBIFil = value; NotifyPropertyChanged();} } }
	private string? _CBITip;
	public string? CBITip { get => _CBITip; set { if (_CBITip != value) { _CBITip = value; NotifyPropertyChanged();} } }
	private string? _CBIDat;
	public string? CBIDat { get => _CBIDat; set { if (_CBIDat != value) { _CBIDat = value; NotifyPropertyChanged();} } }
}