namespace VulpesX.Models.Default;
 
public partial class F24CONTIINPS : Base 
{
	private string _TF24soc = null!;
	public required string TF24soc { get => _TF24soc; set { if (_TF24soc != value) { _TF24soc = value; NotifyPropertyChanged();} } }
	private string _TF24codice = null!;
	public required string TF24codice { get => _TF24codice; set { if (_TF24codice != value) { _TF24codice = value; NotifyPropertyChanged();} } }
	private string _TF24desc = null!;
	public required string TF24desc { get => _TF24desc; set { if (_TF24desc != value) { _TF24desc = value; NotifyPropertyChanged();} } }
	private string _TF24grp = null!;
	public required string TF24grp { get => _TF24grp; set { if (_TF24grp != value) { _TF24grp = value; NotifyPropertyChanged();} } }
	private string _TF24cto = null!;
	public required string TF24cto { get => _TF24cto; set { if (_TF24cto != value) { _TF24cto = value; NotifyPropertyChanged();} } }
	private string _TF24sotc = null!;
	public required string TF24sotc { get => _TF24sotc; set { if (_TF24sotc != value) { _TF24sotc = value; NotifyPropertyChanged();} } }
}