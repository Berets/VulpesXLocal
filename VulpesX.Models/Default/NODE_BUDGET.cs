namespace VulpesX.Models.Default;
 
public partial class NODE_BUDGET : Base 
{
	private string _nodesoc = null!;
	public required string nodesoc { get => _nodesoc; set { if (_nodesoc != value) { _nodesoc = value; NotifyPropertyChanged();} } }
	private int _nodeann;
	public int nodeann { get => _nodeann; set { if (_nodeann != value) { _nodeann = value; NotifyPropertyChanged();} } }
	private string _nodetip = null!;
	public required string nodetip { get => _nodetip; set { if (_nodetip != value) { _nodetip = value; NotifyPropertyChanged();} } }
	private int _nodefil;
	public int nodefil { get => _nodefil; set { if (_nodefil != value) { _nodefil = value; NotifyPropertyChanged();} } }
	private int _nodenum1;
	public int nodenum1 { get => _nodenum1; set { if (_nodenum1 != value) { _nodenum1 = value; NotifyPropertyChanged();} } }
	private int _nodenum2;
	public int nodenum2 { get => _nodenum2; set { if (_nodenum2 != value) { _nodenum2 = value; NotifyPropertyChanged();} } }
	private int _nodenum3;
	public int nodenum3 { get => _nodenum3; set { if (_nodenum3 != value) { _nodenum3 = value; NotifyPropertyChanged();} } }
	private decimal? _bud1;
	public decimal? bud1 { get => _bud1; set { if (_bud1 != value) { _bud1 = value; NotifyPropertyChanged();} } }
	private decimal? _bud2;
	public decimal? bud2 { get => _bud2; set { if (_bud2 != value) { _bud2 = value; NotifyPropertyChanged();} } }
	private decimal? _bud3;
	public decimal? bud3 { get => _bud3; set { if (_bud3 != value) { _bud3 = value; NotifyPropertyChanged();} } }
	private decimal? _bud4;
	public decimal? bud4 { get => _bud4; set { if (_bud4 != value) { _bud4 = value; NotifyPropertyChanged();} } }
	private decimal? _bud5;
	public decimal? bud5 { get => _bud5; set { if (_bud5 != value) { _bud5 = value; NotifyPropertyChanged();} } }
	private decimal? _bud6;
	public decimal? bud6 { get => _bud6; set { if (_bud6 != value) { _bud6 = value; NotifyPropertyChanged();} } }
	private decimal? _bud7;
	public decimal? bud7 { get => _bud7; set { if (_bud7 != value) { _bud7 = value; NotifyPropertyChanged();} } }
	private decimal? _bud8;
	public decimal? bud8 { get => _bud8; set { if (_bud8 != value) { _bud8 = value; NotifyPropertyChanged();} } }
	private decimal? _bud9;
	public decimal? bud9 { get => _bud9; set { if (_bud9 != value) { _bud9 = value; NotifyPropertyChanged();} } }
	private decimal? _bud10;
	public decimal? bud10 { get => _bud10; set { if (_bud10 != value) { _bud10 = value; NotifyPropertyChanged();} } }
	private decimal? _bud11;
	public decimal? bud11 { get => _bud11; set { if (_bud11 != value) { _bud11 = value; NotifyPropertyChanged();} } }
	private decimal? _bud12;
	public decimal? bud12 { get => _bud12; set { if (_bud12 != value) { _bud12 = value; NotifyPropertyChanged();} } }
	private decimal? _budtot;
	public decimal? budtot { get => _budtot; set { if (_budtot != value) { _budtot = value; NotifyPropertyChanged();} } }
}