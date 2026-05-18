namespace VulpesX.Models.Default;
 
public partial class TABDIPEDETT : Base 
{
	private string _tdicod = null!;
	public required string tdicod { get => _tdicod; set { if (_tdicod != value) { _tdicod = value; NotifyPropertyChanged();} } }
	private int _tdianno;
	public int tdianno { get => _tdianno; set { if (_tdianno != value) { _tdianno = value; NotifyPropertyChanged();} } }
	private int _tdimese;
	public int tdimese { get => _tdimese; set { if (_tdimese != value) { _tdimese = value; NotifyPropertyChanged();} } }
	private string? _tdidesmese;
	public string? tdidesmese { get => _tdidesmese; set { if (_tdidesmese != value) { _tdidesmese = value; NotifyPropertyChanged();} } }
	private decimal? _tdiacc;
	public decimal? tdiacc { get => _tdiacc; set { if (_tdiacc != value) { _tdiacc = value; NotifyPropertyChanged();} } }
	private decimal? _tdistip;
	public decimal? tdistip { get => _tdistip; set { if (_tdistip != value) { _tdistip = value; NotifyPropertyChanged();} } }
	private decimal? _tditfr;
	public decimal? tditfr { get => _tditfr; set { if (_tditfr != value) { _tditfr = value; NotifyPropertyChanged();} } }
	private int? _tdinumreg;
	public int? tdinumreg { get => _tdinumreg; set { if (_tdinumreg != value) { _tdinumreg = value; NotifyPropertyChanged();} } }
	private int? _tdiannoreg;
	public int? tdiannoreg { get => _tdiannoreg; set { if (_tdiannoreg != value) { _tdiannoreg = value; NotifyPropertyChanged();} } }
}