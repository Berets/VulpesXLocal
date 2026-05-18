namespace VulpesX.Models.Default;
 
public partial class TF24CONTIINPS1 : Base 
{
	private string _TF24soc1 = null!;
	public required string TF24soc1 { get => _TF24soc1; set { if (_TF24soc1 != value) { _TF24soc1 = value; NotifyPropertyChanged();} } }
	private string _TF24codice1 = null!;
	public required string TF24codice1 { get => _TF24codice1; set { if (_TF24codice1 != value) { _TF24codice1 = value; NotifyPropertyChanged();} } }
	private string? _TF24desc1;
	public string? TF24desc1 { get => _TF24desc1; set { if (_TF24desc1 != value) { _TF24desc1 = value; NotifyPropertyChanged();} } }
	private string? _TF24grp1;
	public string? TF24grp1 { get => _TF24grp1; set { if (_TF24grp1 != value) { _TF24grp1 = value; NotifyPropertyChanged();} } }
	private string? _tf24cto1;
	public string? tf24cto1 { get => _tf24cto1; set { if (_tf24cto1 != value) { _tf24cto1 = value; NotifyPropertyChanged();} } }
	private string? _tf24sotc1;
	public string? tf24sotc1 { get => _tf24sotc1; set { if (_tf24sotc1 != value) { _tf24sotc1 = value; NotifyPropertyChanged();} } }
	private string? _tf24aggdesc;
	public string? tf24aggdesc { get => _tf24aggdesc; set { if (_tf24aggdesc != value) { _tf24aggdesc = value; NotifyPropertyChanged();} } }
	private string? _tf24sodes1;
	public string? tf24sodes1 { get => _tf24sodes1; set { if (_tf24sodes1 != value) { _tf24sodes1 = value; NotifyPropertyChanged();} } }
}