namespace VulpesX.Models.Default;
 
public partial class CUSTOMER_GROUPS : Base 
{
	private string _cccsoc = null!;
	public required string cccsoc { get => _cccsoc; set { if (_cccsoc != value) { _cccsoc = value; NotifyPropertyChanged();} } }
	private int _cccode;
	public int cccode { get => _cccode; set { if (_cccode != value) { _cccode = value; NotifyPropertyChanged();} } }
	private int _ccprog;
	public int ccprog { get => _ccprog; set { if (_ccprog != value) { _ccprog = value; NotifyPropertyChanged();} } }
	private string _ccgrup = null!;
	public required string ccgrup { get => _ccgrup; set { if (_ccgrup != value) { _ccgrup = value; NotifyPropertyChanged();} } }
	private string _cccont = null!;
	public required string cccont { get => _cccont; set { if (_cccont != value) { _cccont = value; NotifyPropertyChanged();} } }
	private string _ccsott = null!;
	public required string ccsott { get => _ccsott; set { if (_ccsott != value) { _ccsott = value; NotifyPropertyChanged();} } }
	private string? _ccsegn;
	public string? ccsegn { get => _ccsegn; set { if (_ccsegn != value) { _ccsegn = value; NotifyPropertyChanged();} } }
	private string? _cccaus;
	public string? cccaus { get => _cccaus; set { if (_cccaus != value) { _cccaus = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}