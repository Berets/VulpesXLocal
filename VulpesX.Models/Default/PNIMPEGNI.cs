namespace VulpesX.Models.Default;
 
public partial class PNIMPEGNI : Base 
{
	private string _PIMSOCI = null!;
	public required string PIMSOCI { get => _PIMSOCI; set { if (_PIMSOCI != value) { _PIMSOCI = value; NotifyPropertyChanged();} } }
	private string _PIMGRUPPO = null!;
	public required string PIMGRUPPO { get => _PIMGRUPPO; set { if (_PIMGRUPPO != value) { _PIMGRUPPO = value; NotifyPropertyChanged();} } }
	private string _PIMCONTO = null!;
	public required string PIMCONTO { get => _PIMCONTO; set { if (_PIMCONTO != value) { _PIMCONTO = value; NotifyPropertyChanged();} } }
	private string _PIMSOTTO = null!;
	public required string PIMSOTTO { get => _PIMSOTTO; set { if (_PIMSOTTO != value) { _PIMSOTTO = value; NotifyPropertyChanged();} } }
	private string _PIMTIPO = null!;
	public required string PIMTIPO { get => _PIMTIPO; set { if (_PIMTIPO != value) { _PIMTIPO = value; NotifyPropertyChanged();} } }
	private int _PIMPROG;
	public int PIMPROG { get => _PIMPROG; set { if (_PIMPROG != value) { _PIMPROG = value; NotifyPropertyChanged();} } }
	private string? _PIMNOMINATIVO;
	public string? PIMNOMINATIVO { get => _PIMNOMINATIVO; set { if (_PIMNOMINATIVO != value) { _PIMNOMINATIVO = value; NotifyPropertyChanged();} } }
	private string? _PIMNUDOC;
	public string? PIMNUDOC { get => _PIMNUDOC; set { if (_PIMNUDOC != value) { _PIMNUDOC = value; NotifyPropertyChanged();} } }
	private DateTime? _PIMDADOC;
	public DateTime? PIMDADOC { get => _PIMDADOC; set { if (_PIMDADOC != value) { _PIMDADOC = value; NotifyPropertyChanged();} } }
	private decimal? _PIMIMPORTO;
	public decimal? PIMIMPORTO { get => _PIMIMPORTO; set { if (_PIMIMPORTO != value) { _PIMIMPORTO = value; NotifyPropertyChanged();} } }
	private decimal? _PIMIMPEFF;
	public decimal? PIMIMPEFF { get => _PIMIMPEFF; set { if (_PIMIMPEFF != value) { _PIMIMPEFF = value; NotifyPropertyChanged();} } }
	private DateTime? _PIMSCADE;
	public DateTime? PIMSCADE { get => _PIMSCADE; set { if (_PIMSCADE != value) { _PIMSCADE = value; NotifyPropertyChanged();} } }
	private string? _PIMNOTE;
	public string? PIMNOTE { get => _PIMNOTE; set { if (_PIMNOTE != value) { _PIMNOTE = value; NotifyPropertyChanged();} } }
	private string? _PIMTIPAG;
	public string? PIMTIPAG { get => _PIMTIPAG; set { if (_PIMTIPAG != value) { _PIMTIPAG = value; NotifyPropertyChanged();} } }
	private int? _PIMANNO;
	public int? PIMANNO { get => _PIMANNO; set { if (_PIMANNO != value) { _PIMANNO = value; NotifyPropertyChanged();} } }
	private int? _PIMNURE;
	public int? PIMNURE { get => _PIMNURE; set { if (_PIMNURE != value) { _PIMNURE = value; NotifyPropertyChanged();} } }
	private string? _PIMDOCON;
	public string? PIMDOCON { get => _PIMDOCON; set { if (_PIMDOCON != value) { _PIMDOCON = value; NotifyPropertyChanged();} } }
	private int? _PIMABI;
	public int? PIMABI { get => _PIMABI; set { if (_PIMABI != value) { _PIMABI = value; NotifyPropertyChanged();} } }
	private int? _PIMCAB;
	public int? PIMCAB { get => _PIMCAB; set { if (_PIMCAB != value) { _PIMCAB = value; NotifyPropertyChanged();} } }
	private string? _PIMNUCO;
	public string? PIMNUCO { get => _PIMNUCO; set { if (_PIMNUCO != value) { _PIMNUCO = value; NotifyPropertyChanged();} } }
	private string? _PIMSOC;
	public string? PIMSOC { get => _PIMSOC; set { if (_PIMSOC != value) { _PIMSOC = value; NotifyPropertyChanged();} } }
	private int? _PIMCFOR;
	public int? PIMCFOR { get => _PIMCFOR; set { if (_PIMCFOR != value) { _PIMCFOR = value; NotifyPropertyChanged();} } }
}