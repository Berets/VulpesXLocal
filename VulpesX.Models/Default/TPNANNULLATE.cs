namespace VulpesX.Models.Default;
 
public partial class TPNANNULLATE : Base 
{
	private string _pnannsoc = null!;
	public required string pnannsoc { get => _pnannsoc; set { if (_pnannsoc != value) { _pnannsoc = value; NotifyPropertyChanged();} } }
	private int _pnannann;
	public int pnannann { get => _pnannann; set { if (_pnannann != value) { _pnannann = value; NotifyPropertyChanged();} } }
	private int _pnannreg;
	public int pnannreg { get => _pnannreg; set { if (_pnannreg != value) { _pnannreg = value; NotifyPropertyChanged();} } }
	private string? _pnannsta;
	public string? pnannsta { get => _pnannsta; set { if (_pnannsta != value) { _pnannsta = value; NotifyPropertyChanged();} } }
	private DateTime? _pnanndat;
	public DateTime? pnanndat { get => _pnanndat; set { if (_pnanndat != value) { _pnanndat = value; NotifyPropertyChanged();} } }
	private string? _pnannute;
	public string? pnannute { get => _pnannute; set { if (_pnannute != value) { _pnannute = value; NotifyPropertyChanged();} } }
}