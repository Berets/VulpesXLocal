namespace VulpesX.Models.Default;
 
public partial class ENASARCO : Base 
{
	private int _enAnno;
	public int enAnno { get => _enAnno; set { if (_enAnno != value) { _enAnno = value; NotifyPropertyChanged();} } }
	private decimal _enMaxmono;
	public decimal enMaxmono { get => _enMaxmono; set { if (_enMaxmono != value) { _enMaxmono = value; NotifyPropertyChanged();} } }
	private decimal _enMaxplu;
	public decimal enMaxplu { get => _enMaxplu; set { if (_enMaxplu != value) { _enMaxplu = value; NotifyPropertyChanged();} } }
	private decimal _enQmaxmono;
	public decimal enQmaxmono { get => _enQmaxmono; set { if (_enQmaxmono != value) { _enQmaxmono = value; NotifyPropertyChanged();} } }
	private decimal _enQmaxplu;
	public decimal enQmaxplu { get => _enQmaxplu; set { if (_enQmaxplu != value) { _enQmaxplu = value; NotifyPropertyChanged();} } }
	private decimal _enQminmono;
	public decimal enQminmono { get => _enQminmono; set { if (_enQminmono != value) { _enQminmono = value; NotifyPropertyChanged();} } }
	private decimal _enQminplu;
	public decimal enQminplu { get => _enQminplu; set { if (_enQminplu != value) { _enQminplu = value; NotifyPropertyChanged();} } }
	private decimal _enPena;
	public decimal enPena { get => _enPena; set { if (_enPena != value) { _enPena = value; NotifyPropertyChanged();} } }
	private decimal _enPquo;
	public decimal enPquo { get => _enPquo; set { if (_enPquo != value) { _enPquo = value; NotifyPropertyChanged();} } }
}