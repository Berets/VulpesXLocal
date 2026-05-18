namespace VulpesX.Models.Default
{
    public partial class pro_ordine_composizione_tempo
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan Duration
        {
            get
            {
                return End - Start;
            }
        }

        private string? _Description;
        public string? Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        private string? _Title;
        public string? Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
            }
        }

        public string ToolTip
        {
            get
            {
                return Description + "\n" + Title + "\n" + "Inizio: " + Start + "\n" + "Fine: " + End;
            }
        }

        public TimeSpan DurataSpan
        {
            get { return new TimeSpan(Durata ?? 0); }
        }

        public TimeSpan DurataSospensioneSpan
        {
            get { return new TimeSpan(DurataSospensione ?? 0); }
        }

        private string? _RisorsaDescrizione;
        public string? RisorsaDescrizione
        {
            get
            {
                return _RisorsaDescrizione;
            }
            set
            {
                _RisorsaDescrizione = value;
            }
        }

        private string? _OperatoreDescrizione;
        public string? OperatoreDescrizione
        {
            get
            {
                return _OperatoreDescrizione;
            }
            set
            {
                _OperatoreDescrizione = value;
            }
        }

        private string? _CausaleDescrizione;
        public string? CausaleDescrizione
        {
            get
            {
                return _CausaleDescrizione;
            }
            set
            {
                _CausaleDescrizione = value;
            }
        }

        private decimal _QuantitaSparata;
        public decimal QuantitaSparata
        {
            get
            {
                return _QuantitaSparata;
            }
            set
            {
                _QuantitaSparata = value;
            }
        }

        public string? TipoDescrizione { get; set; }

        public override string ToString()
        {
            return ToolTip;
        }

        #region Overrides

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            var tObj = obj as pro_ordine_composizione_tempo;

            if (tObj == null)
                return false;
            return Start == tObj.Start && End == tObj.End && OrdineID == tObj.OrdineID && ArticoloID == tObj.ArticoloID && ComposizioneID == tObj.ComposizioneID && ProgressivoID == tObj.ProgressivoID;
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() + End.GetHashCode();
        }

        #endregion

        #region Ufp
        public string? TESOCI { get; set; }
        public short? TEANNP { get; set; }
        public int? TEORDP { get; set; }
        public short? TENSEQ { get; set; }
        public short? TERIGA { get; set; }
        public string? teprocessed { get; set; }
        #endregion
    }
}
