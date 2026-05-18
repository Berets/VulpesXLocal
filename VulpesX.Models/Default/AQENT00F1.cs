namespace VulpesX.Models.Default;
 
public partial class AQENT00F1 : Base 
{
	private string _emsoci = null!;
	public required string emsoci { get => _emsoci; set { if (_emsoci != value) { _emsoci = value; NotifyPropertyChanged();} } }
	private int _EMCDFO;
	public int EMCDFO { get => _EMCDFO; set { if (_EMCDFO != value) { _EMCDFO = value; NotifyPropertyChanged();} } }
	private DateTime _EMEDBF;
	public DateTime EMEDBF { get => _EMEDBF; set { if (_EMEDBF != value) { _EMEDBF = value; NotifyPropertyChanged();} } }
	private string _EMENBF = null!;
	public required string EMENBF { get => _EMENBF; set { if (_EMENBF != value) { _EMENBF = value; NotifyPropertyChanged();} } }
	private int _EMEPEM;
	public int EMEPEM { get => _EMEPEM; set { if (_EMEPEM != value) { _EMEPEM = value; NotifyPropertyChanged();} } }
	private int _emepro;
	public int emepro { get => _emepro; set { if (_emepro != value) { _emepro = value; NotifyPropertyChanged();} } }
	private string _emecol = null!;
	public required string emecol { get => _emecol; set { if (_emecol != value) { _emecol = value; NotifyPropertyChanged();} } }
	private decimal? _emeqtc;
	public decimal? emeqtc { get => _emeqtc; set { if (_emeqtc != value) { _emeqtc = value; NotifyPropertyChanged();} } }
	private decimal? _emeqts;
	public decimal? emeqts { get => _emeqts; set { if (_emeqts != value) { _emeqts = value; NotifyPropertyChanged();} } }
	private DateTime? _emedsc;
	public DateTime? emedsc { get => _emedsc; set { if (_emedsc != value) { _emedsc = value; NotifyPropertyChanged();} } }
}