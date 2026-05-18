namespace VulpesX.Models.Default;
 
public partial class ANAGCLIE : Base 
{
	private int _cliecodice;
	public int cliecodice { get => _cliecodice; set { if (_cliecodice != value) { _cliecodice = value; NotifyPropertyChanged();} } }
	private string? _clieragion;
	public string? clieragion { get => _clieragion; set { if (_clieragion != value) { _clieragion = value; NotifyPropertyChanged();} } }
	private string? _clieindiri;
	public string? clieindiri { get => _clieindiri; set { if (_clieindiri != value) { _clieindiri = value; NotifyPropertyChanged();} } }
	private string? _clielocali;
	public string? clielocali { get => _clielocali; set { if (_clielocali != value) { _clielocali = value; NotifyPropertyChanged();} } }
	private string? _clieprovin;
	public string? clieprovin { get => _clieprovin; set { if (_clieprovin != value) { _clieprovin = value; NotifyPropertyChanged();} } }
	private int? _cliecap;
	public int? cliecap { get => _cliecap; set { if (_cliecap != value) { _cliecap = value; NotifyPropertyChanged();} } }
}