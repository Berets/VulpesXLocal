namespace VulpesX.Models.Default;
 
public partial class tab_articolo_extern : Base 
{
	private string _extsoc = null!;
	public required string extsoc { get => _extsoc; set { if (_extsoc != value) { _extsoc = value; NotifyPropertyChanged();} } }
	private string _extpid = null!;
	public required string extpid { get => _extpid; set { if (_extpid != value) { _extpid = value; NotifyPropertyChanged();} } }
	private string _extcode = null!;
	public required string extcode { get => _extcode; set { if (_extcode != value) { _extcode = value; NotifyPropertyChanged();} } }
	private string _extid = null!;
	public required string extid { get => _extid; set { if (_extid != value) { _extid = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}