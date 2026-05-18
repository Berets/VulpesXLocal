namespace VulpesX.Models.Default;
 
public partial class TF24CONTIICI : Base 
{
	private string _tf24isoc = null!;
	public required string tf24isoc { get => _tf24isoc; set { if (_tf24isoc != value) { _tf24isoc = value; NotifyPropertyChanged();} } }
	private string _tf24icod = null!;
	public required string tf24icod { get => _tf24icod; set { if (_tf24icod != value) { _tf24icod = value; NotifyPropertyChanged();} } }
	private string? _tf24idesc;
	public string? tf24idesc { get => _tf24idesc; set { if (_tf24idesc != value) { _tf24idesc = value; NotifyPropertyChanged();} } }
	private string? _tf24igrp;
	public string? tf24igrp { get => _tf24igrp; set { if (_tf24igrp != value) { _tf24igrp = value; NotifyPropertyChanged();} } }
	private string? _tf24icto;
	public string? tf24icto { get => _tf24icto; set { if (_tf24icto != value) { _tf24icto = value; NotifyPropertyChanged();} } }
	private string? _tf24isotc;
	public string? tf24isotc { get => _tf24isotc; set { if (_tf24isotc != value) { _tf24isotc = value; NotifyPropertyChanged();} } }
	private string? _tf24idescagg;
	public string? tf24idescagg { get => _tf24idescagg; set { if (_tf24idescagg != value) { _tf24idescagg = value; NotifyPropertyChanged();} } }
	private string? _tf24isodes;
	public string? tf24isodes { get => _tf24isodes; set { if (_tf24isodes != value) { _tf24isodes = value; NotifyPropertyChanged();} } }
}