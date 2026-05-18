namespace VulpesX.Models.Default;
 
public partial class TF24CONTIERARIO : Base 
{
	private string _tf24Esoc = null!;
	public required string tf24Esoc { get => _tf24Esoc; set { if (_tf24Esoc != value) { _tf24Esoc = value; NotifyPropertyChanged();} } }
	private string _tf24Ecod = null!;
	public required string tf24Ecod { get => _tf24Ecod; set { if (_tf24Ecod != value) { _tf24Ecod = value; NotifyPropertyChanged();} } }
	private string? _tf24Edesc;
	public string? tf24Edesc { get => _tf24Edesc; set { if (_tf24Edesc != value) { _tf24Edesc = value; NotifyPropertyChanged();} } }
	private string? _tf24Egrp;
	public string? tf24Egrp { get => _tf24Egrp; set { if (_tf24Egrp != value) { _tf24Egrp = value; NotifyPropertyChanged();} } }
	private string? _tf24ecto;
	public string? tf24ecto { get => _tf24ecto; set { if (_tf24ecto != value) { _tf24ecto = value; NotifyPropertyChanged();} } }
	private string? _tf24esotc;
	public string? tf24esotc { get => _tf24esotc; set { if (_tf24esotc != value) { _tf24esotc = value; NotifyPropertyChanged();} } }
	private string? _tf24edescagg;
	public string? tf24edescagg { get => _tf24edescagg; set { if (_tf24edescagg != value) { _tf24edescagg = value; NotifyPropertyChanged();} } }
	private string? _tf24esodes;
	public string? tf24esodes { get => _tf24esodes; set { if (_tf24esodes != value) { _tf24esodes = value; NotifyPropertyChanged();} } }
}