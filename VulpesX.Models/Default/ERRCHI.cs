namespace VulpesX.Models.Default;
 
public partial class ERRCHI : Base 
{
	private string _errchisoc = null!;
	public required string errchisoc { get => _errchisoc; set { if (_errchisoc != value) { _errchisoc = value; NotifyPropertyChanged();} } }
	private int _errchilin;
	public int errchilin { get => _errchilin; set { if (_errchilin != value) { _errchilin = value; NotifyPropertyChanged();} } }
	private string? _errchides;
	public string? errchides { get => _errchides; set { if (_errchides != value) { _errchides = value; NotifyPropertyChanged();} } }
	private string? _errchigrp;
	public string? errchigrp { get => _errchigrp; set { if (_errchigrp != value) { _errchigrp = value; NotifyPropertyChanged();} } }
	private string? _errchicnt;
	public string? errchicnt { get => _errchicnt; set { if (_errchicnt != value) { _errchicnt = value; NotifyPropertyChanged();} } }
	private string? _errchistc;
	public string? errchistc { get => _errchistc; set { if (_errchistc != value) { _errchistc = value; NotifyPropertyChanged();} } }
	private int? _errchiann;
	public int? errchiann { get => _errchiann; set { if (_errchiann != value) { _errchiann = value; NotifyPropertyChanged();} } }
	private decimal? _errchidar;
	public decimal? errchidar { get => _errchidar; set { if (_errchidar != value) { _errchidar = value; NotifyPropertyChanged();} } }
	private decimal? _errchiave;
	public decimal? errchiave { get => _errchiave; set { if (_errchiave != value) { _errchiave = value; NotifyPropertyChanged();} } }
}