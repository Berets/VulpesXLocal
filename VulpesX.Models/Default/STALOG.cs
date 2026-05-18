namespace VulpesX.Models.Default;
 
public partial class STALOG : Base 
{
	private int _stacod;
	public int stacod { get => _stacod; set { if (_stacod != value) { _stacod = value; NotifyPropertyChanged();} } }
	private string? _stades;
	public string? stades { get => _stades; set { if (_stades != value) { _stades = value; NotifyPropertyChanged();} } }
	private byte[]? _stadoc;
	public byte[]? stadoc { get => _stadoc; set { if (_stadoc != value) { _stadoc = value; NotifyPropertyChanged();} } }
	private DateTime? _stadat;
	public DateTime? stadat { get => _stadat; set { if (_stadat != value) { _stadat = value; NotifyPropertyChanged();} } }
	private string? _statxt;
	public string? statxt { get => _statxt; set { if (_statxt != value) { _statxt = value; NotifyPropertyChanged();} } }
}