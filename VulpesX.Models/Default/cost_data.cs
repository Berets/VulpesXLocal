namespace VulpesX.Models.Default;
 
public partial class cost_data : Base 
{
	private string _SocietaID = null!;
	public required string SocietaID { get => _SocietaID; set { if (_SocietaID != value) { _SocietaID = value; NotifyPropertyChanged();} } }
	private int _Anno;
	public int Anno { get => _Anno; set { if (_Anno != value) { _Anno = value; NotifyPropertyChanged();} } }
	private long _ID;
	public long ID { get => _ID; set { if (_ID != value) { _ID = value; NotifyPropertyChanged();} } }
	private string? _OrdineID;
	public string? OrdineID { get => _OrdineID; set { if (_OrdineID != value) { _OrdineID = value; NotifyPropertyChanged();} } }
	private string? _DocumentoNumero;
	public string? DocumentoNumero { get => _DocumentoNumero; set { if (_DocumentoNumero != value) { _DocumentoNumero = value; NotifyPropertyChanged();} } }
	private DateTime? _DocumentoData;
	public DateTime? DocumentoData { get => _DocumentoData; set { if (_DocumentoData != value) { _DocumentoData = value; NotifyPropertyChanged();} } }
	private string? _CausaleID;
	public string? CausaleID { get => _CausaleID; set { if (_CausaleID != value) { _CausaleID = value; NotifyPropertyChanged();} } }
	private string? _CausaleDescrizione;
	public string? CausaleDescrizione { get => _CausaleDescrizione; set { if (_CausaleDescrizione != value) { _CausaleDescrizione = value; NotifyPropertyChanged();} } }
	private long? _Tempo;
	public long? Tempo { get => _Tempo; set { if (_Tempo != value) { _Tempo = value; NotifyPropertyChanged();} } }
	private decimal? _TempoCosto;
	public decimal? TempoCosto { get => _TempoCosto; set { if (_TempoCosto != value) { _TempoCosto = value; NotifyPropertyChanged();} } }
	private decimal? _Quantita;
	public decimal? Quantita { get => _Quantita; set { if (_Quantita != value) { _Quantita = value; NotifyPropertyChanged();} } }
	private decimal? _QuantitaCosto;
	public decimal? QuantitaCosto { get => _QuantitaCosto; set { if (_QuantitaCosto != value) { _QuantitaCosto = value; NotifyPropertyChanged();} } }
	private decimal? _CostoTotale;
	public decimal? CostoTotale { get => _CostoTotale; set { if (_CostoTotale != value) { _CostoTotale = value; NotifyPropertyChanged();} } }
	private string? _Segno;
	public string? Segno { get => _Segno; set { if (_Segno != value) { _Segno = value; NotifyPropertyChanged();} } }
	private string? _ArticoloID;
	public string? ArticoloID { get => _ArticoloID; set { if (_ArticoloID != value) { _ArticoloID = value; NotifyPropertyChanged();} } }
	private string? _RevisioneID;
	public string? RevisioneID { get => _RevisioneID; set { if (_RevisioneID != value) { _RevisioneID = value; NotifyPropertyChanged();} } }
	private string? _ArticoloDescrizione;
	public string? ArticoloDescrizione { get => _ArticoloDescrizione; set { if (_ArticoloDescrizione != value) { _ArticoloDescrizione = value; NotifyPropertyChanged();} } }
	private string? _RisorsaID;
	public string? RisorsaID { get => _RisorsaID; set { if (_RisorsaID != value) { _RisorsaID = value; NotifyPropertyChanged();} } }
	private string? _RisorsaDescrizione;
	public string? RisorsaDescrizione { get => _RisorsaDescrizione; set { if (_RisorsaDescrizione != value) { _RisorsaDescrizione = value; NotifyPropertyChanged();} } }
	private string? _RepartoID;
	public string? RepartoID { get => _RepartoID; set { if (_RepartoID != value) { _RepartoID = value; NotifyPropertyChanged();} } }
	private string? _RepartoDescrizione;
	public string? RepartoDescrizione { get => _RepartoDescrizione; set { if (_RepartoDescrizione != value) { _RepartoDescrizione = value; NotifyPropertyChanged();} } }
	private string? _OperatoreID;
	public string? OperatoreID { get => _OperatoreID; set { if (_OperatoreID != value) { _OperatoreID = value; NotifyPropertyChanged();} } }
	private string? _OperatoreDescrizione;
	public string? OperatoreDescrizione { get => _OperatoreDescrizione; set { if (_OperatoreDescrizione != value) { _OperatoreDescrizione = value; NotifyPropertyChanged();} } }
	private int? _ClienteID;
	public int? ClienteID { get => _ClienteID; set { if (_ClienteID != value) { _ClienteID = value; NotifyPropertyChanged();} } }
	private string? _ClienteDescrizione;
	public string? ClienteDescrizione { get => _ClienteDescrizione; set { if (_ClienteDescrizione != value) { _ClienteDescrizione = value; NotifyPropertyChanged();} } }
	private int? _FornitoreID;
	public int? FornitoreID { get => _FornitoreID; set { if (_FornitoreID != value) { _FornitoreID = value; NotifyPropertyChanged();} } }
	private string? _FornitoreDescrizione;
	public string? FornitoreDescrizione { get => _FornitoreDescrizione; set { if (_FornitoreDescrizione != value) { _FornitoreDescrizione = value; NotifyPropertyChanged();} } }
	private string? _Lotto;
	public string? Lotto { get => _Lotto; set { if (_Lotto != value) { _Lotto = value; NotifyPropertyChanged();} } }
	private DateTime? _LogAdded;
	public DateTime? LogAdded { get => _LogAdded; set { if (_LogAdded != value) { _LogAdded = value; NotifyPropertyChanged();} } }
	private string? _LogAddedUserID;
	public string? LogAddedUserID { get => _LogAddedUserID; set { if (_LogAddedUserID != value) { _LogAddedUserID = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}