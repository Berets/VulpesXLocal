namespace VulpesX.Models.Default;
 
public partial class pro_ordine_composizione_tempo : Base 
{
	private string _SocietaID = null!;
	public required string SocietaID { get => _SocietaID; set { if (_SocietaID != value) { _SocietaID = value; NotifyPropertyChanged();} } }
	private string _OrdineID = null!;
	public required string OrdineID { get => _OrdineID; set { if (_OrdineID != value) { _OrdineID = value; NotifyPropertyChanged();} } }
	private string _ArticoloID = null!;
	public required string ArticoloID { get => _ArticoloID; set { if (_ArticoloID != value) { _ArticoloID = value; NotifyPropertyChanged();} } }
	private string _RevisioneID = null!;
	public required string RevisioneID { get => _RevisioneID; set { if (_RevisioneID != value) { _RevisioneID = value; NotifyPropertyChanged();} } }
	private long _ComposizioneID;
	public long ComposizioneID { get => _ComposizioneID; set { if (_ComposizioneID != value) { _ComposizioneID = value; NotifyPropertyChanged();} } }
	private long _ProgressivoID;
	public long ProgressivoID { get => _ProgressivoID; set { if (_ProgressivoID != value) { _ProgressivoID = value; NotifyPropertyChanged();} } }
	private DateTime? _LogAdded;
	public DateTime? LogAdded { get => _LogAdded; set { if (_LogAdded != value) { _LogAdded = value; NotifyPropertyChanged();} } }
	private DateTime? _LogUpdated;
	public DateTime? LogUpdated { get => _LogUpdated; set { if (_LogUpdated != value) { _LogUpdated = value; NotifyPropertyChanged();} } }
	private DateTime? _LogCanceled;
	public DateTime? LogCanceled { get => _LogCanceled; set { if (_LogCanceled != value) { _LogCanceled = value; NotifyPropertyChanged();} } }
	private string? _LogAddedUserID;
	public string? LogAddedUserID { get => _LogAddedUserID; set { if (_LogAddedUserID != value) { _LogAddedUserID = value; NotifyPropertyChanged();} } }
	private string? _LogUpdatedUserID;
	public string? LogUpdatedUserID { get => _LogUpdatedUserID; set { if (_LogUpdatedUserID != value) { _LogUpdatedUserID = value; NotifyPropertyChanged();} } }
	private string? _LogCanceledUserID;
	public string? LogCanceledUserID { get => _LogCanceledUserID; set { if (_LogCanceledUserID != value) { _LogCanceledUserID = value; NotifyPropertyChanged();} } }
	private DateTime _Data;
	public DateTime Data { get => _Data; set { if (_Data != value) { _Data = value; NotifyPropertyChanged();} } }
	private string? _TipoID;
	public string? TipoID { get => _TipoID; set { if (_TipoID != value) { _TipoID = value; NotifyPropertyChanged();} } }
	private string? _RisorsaID;
	public string? RisorsaID { get => _RisorsaID; set { if (_RisorsaID != value) { _RisorsaID = value; NotifyPropertyChanged();} } }
	private string? _OperatoreID;
	public string? OperatoreID { get => _OperatoreID; set { if (_OperatoreID != value) { _OperatoreID = value; NotifyPropertyChanged();} } }
	private string? _CausaleID;
	public string? CausaleID { get => _CausaleID; set { if (_CausaleID != value) { _CausaleID = value; NotifyPropertyChanged();} } }
	private long? _Durata;
	public long? Durata { get => _Durata; set { if (_Durata != value) { _Durata = value; NotifyPropertyChanged();} } }
	private long? _DurataSospensione;
	public long? DurataSospensione { get => _DurataSospensione; set { if (_DurataSospensione != value) { _DurataSospensione = value; NotifyPropertyChanged();} } }
	private bool _EProcessata;
	public bool EProcessata { get => _EProcessata; set { if (_EProcessata != value) { _EProcessata = value; NotifyPropertyChanged();} } }
	private decimal? _QuantitaFase;
	public decimal? QuantitaFase { get => _QuantitaFase; set { if (_QuantitaFase != value) { _QuantitaFase = value; NotifyPropertyChanged();} } }
	private decimal? _QuantitaVersata;
	public decimal? QuantitaVersata { get => _QuantitaVersata; set { if (_QuantitaVersata != value) { _QuantitaVersata = value; NotifyPropertyChanged();} } }
	private decimal? _QuantitaScartata;
	public decimal? QuantitaScartata { get => _QuantitaScartata; set { if (_QuantitaScartata != value) { _QuantitaScartata = value; NotifyPropertyChanged();} } }
	private decimal? _QuantitaConfezione;
	public decimal? QuantitaConfezione { get => _QuantitaConfezione; set { if (_QuantitaConfezione != value) { _QuantitaConfezione = value; NotifyPropertyChanged();} } }
	private decimal? _QuantitaUnitaria;
	public decimal? QuantitaUnitaria { get => _QuantitaUnitaria; set { if (_QuantitaUnitaria != value) { _QuantitaUnitaria = value; NotifyPropertyChanged();} } }
	private decimal? _Confezioni;
	public decimal? Confezioni { get => _Confezioni; set { if (_Confezioni != value) { _Confezioni = value; NotifyPropertyChanged();} } }
	private long? _MovimentoID;
	public long? MovimentoID { get => _MovimentoID; set { if (_MovimentoID != value) { _MovimentoID = value; NotifyPropertyChanged();} } }
	private string? _Note;
	public string? Note { get => _Note; set { if (_Note != value) { _Note = value; NotifyPropertyChanged();} } }
	private string? _Lotto;
	public string? Lotto { get => _Lotto; set { if (_Lotto != value) { _Lotto = value; NotifyPropertyChanged();} } }
	private string? _LottoInterno;
	public string? LottoInterno { get => _LottoInterno; set { if (_LottoInterno != value) { _LottoInterno = value; NotifyPropertyChanged();} } }
	private int? _Box;
	public int? Box { get => _Box; set { if (_Box != value) { _Box = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}