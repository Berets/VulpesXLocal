namespace VulpesX.Models.Default;
 
public partial class ANACOMMESSE : Base 
{
	private string _SOMCOD = null!;
	public required string SOMCOD { get => _SOMCOD; set { if (_SOMCOD != value) { _SOMCOD = value; NotifyPropertyChanged();} } }
	private int _DWAnno;
	public int DWAnno { get => _DWAnno; set { if (_DWAnno != value) { _DWAnno = value; NotifyPropertyChanged();} } }
	private int _DWMese;
	public int DWMese { get => _DWMese; set { if (_DWMese != value) { _DWMese = value; NotifyPropertyChanged();} } }
	private string _dwcoma = null!;
	public required string dwcoma { get => _dwcoma; set { if (_dwcoma != value) { _dwcoma = value; NotifyPropertyChanged();} } }
	private string _linepr = null!;
	public required string linepr { get => _linepr; set { if (_linepr != value) { _linepr = value; NotifyPropertyChanged();} } }
	private string _kommess = null!;
	public required string kommess { get => _kommess; set { if (_kommess != value) { _kommess = value; NotifyPropertyChanged();} } }
	private string _CauComme = null!;
	public required string CauComme { get => _CauComme; set { if (_CauComme != value) { _CauComme = value; NotifyPropertyChanged();} } }
	private string _Moviment = null!;
	public required string Moviment { get => _Moviment; set { if (_Moviment != value) { _Moviment = value; NotifyPropertyChanged();} } }
	private string _DocuNume = null!;
	public required string DocuNume { get => _DocuNume; set { if (_DocuNume != value) { _DocuNume = value; NotifyPropertyChanged();} } }
	private DateTime _DocumeDa;
	public DateTime DocumeDa { get => _DocumeDa; set { if (_DocumeDa != value) { _DocumeDa = value; NotifyPropertyChanged();} } }
	private string _DwsCod = null!;
	public required string DwsCod { get => _DwsCod; set { if (_DwsCod != value) { _DwsCod = value; NotifyPropertyChanged();} } }
	private string _DwaCod = null!;
	public required string DwaCod { get => _DwaCod; set { if (_DwaCod != value) { _DwaCod = value; NotifyPropertyChanged();} } }
	private decimal? _TotRicMe;
	public decimal? TotRicMe { get => _TotRicMe; set { if (_TotRicMe != value) { _TotRicMe = value; NotifyPropertyChanged();} } }
	private decimal? _TotCosMe;
	public decimal? TotCosMe { get => _TotCosMe; set { if (_TotCosMe != value) { _TotCosMe = value; NotifyPropertyChanged();} } }
}