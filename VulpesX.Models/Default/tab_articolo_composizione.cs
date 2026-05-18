namespace VulpesX.Models.Default;
 
public partial class tab_articolo_composizione : Base 
{
	private string _SocietaID = null!;
	public required string SocietaID { get => _SocietaID; set { if (_SocietaID != value) { _SocietaID = value; NotifyPropertyChanged();} } }
	private string _ArticoloID = null!;
	public required string ArticoloID { get => _ArticoloID; set { if (_ArticoloID != value) { _ArticoloID = value; NotifyPropertyChanged();} } }
	private string _RevisioneID = null!;
	public required string RevisioneID { get => _RevisioneID; set { if (_RevisioneID != value) { _RevisioneID = value; NotifyPropertyChanged();} } }
	private long _ComposizioneID;
	public long ComposizioneID { get => _ComposizioneID; set { if (_ComposizioneID != value) { _ComposizioneID = value; NotifyPropertyChanged();} } }
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
	private long? _ComposizioneIDPadre;
	public long? ComposizioneIDPadre { get => _ComposizioneIDPadre; set { if (_ComposizioneIDPadre != value) { _ComposizioneIDPadre = value; NotifyPropertyChanged();} } }
	private string? _RepartoID;
	public string? RepartoID { get => _RepartoID; set { if (_RepartoID != value) { _RepartoID = value; NotifyPropertyChanged();} } }
	private string? _ComponenteArticoloID;
	public string? ComponenteArticoloID { get => _ComponenteArticoloID; set { if (_ComponenteArticoloID != value) { _ComponenteArticoloID = value; NotifyPropertyChanged();} } }
	private string? _ComponenteRevisioneID;
	public string? ComponenteRevisioneID { get => _ComponenteRevisioneID; set { if (_ComponenteRevisioneID != value) { _ComponenteRevisioneID = value; NotifyPropertyChanged();} } }
	private long? _Posizione;
	public long? Posizione { get => _Posizione; set { if (_Posizione != value) { _Posizione = value; NotifyPropertyChanged();} } }
	private decimal? _Quantita;
	public decimal? Quantita { get => _Quantita; set { if (_Quantita != value) { _Quantita = value; NotifyPropertyChanged();} } }
	private long? _Tempo;
	public long? Tempo { get => _Tempo; set { if (_Tempo != value) { _Tempo = value; NotifyPropertyChanged();} } }
	private string? _RisorsaID;
	public string? RisorsaID { get => _RisorsaID; set { if (_RisorsaID != value) { _RisorsaID = value; NotifyPropertyChanged();} } }
	private bool _ESummary;
	public bool ESummary { get => _ESummary; set { if (_ESummary != value) { _ESummary = value; NotifyPropertyChanged();} } }
	private bool _EMilestone;
	public bool EMilestone { get => _EMilestone; set { if (_EMilestone != value) { _EMilestone = value; NotifyPropertyChanged();} } }
	private string? _DescrizioneMS;
	public string? DescrizioneMS { get => _DescrizioneMS; set { if (_DescrizioneMS != value) { _DescrizioneMS = value; NotifyPropertyChanged();} } }
	private string? _Note;
	public string? Note { get => _Note; set { if (_Note != value) { _Note = value; NotifyPropertyChanged();} } }
	private long? _Piazzamento;
	public long? Piazzamento { get => _Piazzamento; set { if (_Piazzamento != value) { _Piazzamento = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}