using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class ANACERT : Base
    {
        private int _anacli;
        public int anacli { get => _anacli; set { if (_anacli != value) { _anacli = value; NotifyPropertyChanged(); } } }
        private short _ananmax;
        public short ananmax { get => _ananmax; set { if (_ananmax != value) { _ananmax = value; NotifyPropertyChanged(); } } }
        private string? _clilogoestensione = null!;
        public string? clilogoestensione { get => _clilogoestensione; set { if (_clilogoestensione != value) { _clilogoestensione = value; NotifyPropertyChanged(); } } }
        private string? _clilogonome;
        public string? clilogonome { get => _clilogonome; set { if (_clilogonome != value) { _clilogonome = value; NotifyPropertyChanged(); } } }
        private byte[]? _clilogo;
        public byte[]? clilogo { get => _clilogo; set { if (_clilogo != value) { _clilogo = value; NotifyPropertyChanged(); } } }
        private string? _anapeobb;
        public string? anapeobb { get => _anapeobb; set { if (_anapeobb != value) { _anapeobb = value; NotifyPropertyChanged(); } } }

        private byte[]? _rv;
        public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged(); } } }
    }
}
