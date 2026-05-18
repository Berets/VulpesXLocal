namespace VulpesX.Models.Default;
 
public partial class TABDIPEDETT1 : Base 
{
	private string _dtdisoc = null!;
	public required string dtdisoc { get => _dtdisoc; set { if (_dtdisoc != value) { _dtdisoc = value; NotifyPropertyChanged();} } }
	private string _dtdicod = null!;
	public required string dtdicod { get => _dtdicod; set { if (_dtdicod != value) { _dtdicod = value; NotifyPropertyChanged();} } }
	private int _dtdianno;
	public int dtdianno { get => _dtdianno; set { if (_dtdianno != value) { _dtdianno = value; NotifyPropertyChanged();} } }
	private int _dtdimese;
	public int dtdimese { get => _dtdimese; set { if (_dtdimese != value) { _dtdimese = value; NotifyPropertyChanged();} } }
	private string? _dtdidesmese;
	public string? dtdidesmese { get => _dtdidesmese; set { if (_dtdidesmese != value) { _dtdidesmese = value; NotifyPropertyChanged();} } }
	private decimal? _dtdiacc;
	public decimal? dtdiacc { get => _dtdiacc; set { if (_dtdiacc != value) { _dtdiacc = value; NotifyPropertyChanged();} } }
	private decimal? _dtdistip;
	public decimal? dtdistip { get => _dtdistip; set { if (_dtdistip != value) { _dtdistip = value; NotifyPropertyChanged();} } }
	private decimal? _dtditfr;
	public decimal? dtditfr { get => _dtditfr; set { if (_dtditfr != value) { _dtditfr = value; NotifyPropertyChanged();} } }
	private int? _dtdiannoreg;
	public int? dtdiannoreg { get => _dtdiannoreg; set { if (_dtdiannoreg != value) { _dtdiannoreg = value; NotifyPropertyChanged();} } }
	private int? _dtdinumreg;
	public int? dtdinumreg { get => _dtdinumreg; set { if (_dtdinumreg != value) { _dtdinumreg = value; NotifyPropertyChanged();} } }
}