namespace VulpesX.Models.Default;
 
public partial class ANALISIDAT : Base 
{
	private string _Csoc = null!;
	public required string Csoc { get => _Csoc; set { if (_Csoc != value) { _Csoc = value; NotifyPropertyChanged();} } }
	private string _Cdc = null!;
	public required string Cdc { get => _Cdc; set { if (_Cdc != value) { _Cdc = value; NotifyPropertyChanged();} } }
	private int _CAnno;
	public int CAnno { get => _CAnno; set { if (_CAnno != value) { _CAnno = value; NotifyPropertyChanged();} } }
	private int _CMese;
	public int CMese { get => _CMese; set { if (_CMese != value) { _CMese = value; NotifyPropertyChanged();} } }
	private decimal _CTotOre;
	public decimal CTotOre { get => _CTotOre; set { if (_CTotOre != value) { _CTotOre = value; NotifyPropertyChanged();} } }
	private decimal _CTotpez;
	public decimal CTotpez { get => _CTotpez; set { if (_CTotpez != value) { _CTotpez = value; NotifyPropertyChanged();} } }
}