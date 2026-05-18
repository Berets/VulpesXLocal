namespace VulpesX.Models.Default;
 
public partial class pro_ordine_composizione_lotto : Base 
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
	private long _ID;
	public long ID { get => _ID; set { if (_ID != value) { _ID = value; NotifyPropertyChanged();} } }
	private string? _Lotto;
	public string? Lotto { get => _Lotto; set { if (_Lotto != value) { _Lotto = value; NotifyPropertyChanged();} } }
	private decimal? _Quantita;
	public decimal? Quantita { get => _Quantita; set { if (_Quantita != value) { _Quantita = value; NotifyPropertyChanged();} } }
	private DateTime? _DataScadenza;
	public DateTime? DataScadenza { get => _DataScadenza; set { if (_DataScadenza != value) { _DataScadenza = value; NotifyPropertyChanged();} } }
	private DateTime? _LogAdded;
	public DateTime? LogAdded { get => _LogAdded; set { if (_LogAdded != value) { _LogAdded = value; NotifyPropertyChanged();} } }
	private string? _LogAddedUserID;
	public string? LogAddedUserID { get => _LogAddedUserID; set { if (_LogAddedUserID != value) { _LogAddedUserID = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string _product_id = null!;
	public required string product_id { get => _product_id; set { if (_product_id != value) { _product_id = value; NotifyPropertyChanged();} } }
}