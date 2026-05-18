namespace VulpesX.Models.Default;
using VulpesX.Shared;
public partial class TAB_AGENTI_ENASARCO : Base
{
    private string _agesoc = null!;
    public required string agesoc { get => _agesoc; set { if (_agesoc != value) { _agesoc = value; NotifyPropertyChanged(); } } }
    private string _agecod = null!;
	public required string agecod { get => _agecod; set { if (_agecod != value) { _agecod = value; NotifyPropertyChanged();} } }
	private int _enaann;
	public int enaann { get => _enaann; set { if (_enaann != value) { _enaann = value; NotifyPropertyChanged();} } }
	private decimal? _enaperazi;
	public decimal? enaperazi { get => _enaperazi; set { if (_enaperazi != value) { _enaperazi = value; NotifyPropertyChanged();} } }
	private decimal? _enaannmax;
	public decimal? enaannmax { get => _enaannmax; set { if (_enaannmax != value) { _enaannmax = value; NotifyPropertyChanged();} } }
}