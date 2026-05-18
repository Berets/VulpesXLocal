namespace VulpesX.Models.Default
{
    public partial class tab_produzione_calendario_chiusura
    {
        public TimeSpan Duration
        {
            get
            {
                return Alle - Dalle;
            }
        }

        private string? descrizione;
        public string? Descrizione
        {
            get
            {
                return string.IsNullOrEmpty(RisorsaID) ? descrizione : "#" + RisorsaID + " : " + descrizione;
            }

            set
            {
                descrizione = value;
            }
        }

        public string ToolTip
        {
            get
            {
                return "Dal " + Dalle.ToString() + " al " + Alle.ToString() + "\n" + Descrizione;
            }
        }

        #region Overrides
        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            tab_produzione_calendario_chiusura? tObj = obj as tab_produzione_calendario_chiusura;

            if (tObj == null)
                return false;

            return Dalle == tObj.Dalle && Alle == tObj.Alle;
        }

        public override int GetHashCode()
        {
            return Dalle.GetHashCode() + Alle.GetHashCode();
        }

        #endregion

    }
}
