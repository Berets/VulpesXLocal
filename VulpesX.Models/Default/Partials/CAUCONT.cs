namespace VulpesX.Models.Default
{
    public partial class CAUCONT
    {
        public string FullDescriptionSearchable => $"{caucod} {caudes?.Trim()}";
        public string FullDescriptionNotSearchable => $"[{caucod}] {caudes?.Trim()}";
        public string PrimaNotaBackground
        {
            get
            {
                switch (caucol)
                {
                    case "MGN": return "V";
                    case "RED": return "O";
                    case "GRN": return "G";
                    case "CYN": return "B";
                    case "YLW": return "Y";
                    default: return "X";
                }
            }
        }

        public LIBRIIVA? IVABook { get; set; }

        public bool caugenBool
        {
            get
            {
                return caugen == "S";
            }
            set
            {
                if (value)
                    caugen = "S";
                else
                    caugen = "N";
            }
        }

        public bool cauivaBool
        {
            get
            {
                return cauiva == "S";
            }
            set
            {
                if (value)
                    cauiva = "S";
                else
                    cauiva = "N";
            }
        }

        public bool caucliBool
        {
            get
            {
                return caucli == "S";
            }
            set
            {
                if (value)
                    caucli = "S";
                else
                    caucli = "N";
            }
        }

        public bool cauforBool
        {
            get
            {
                return caufor == "S";
            }
            set
            {
                if (value)
                    caufor = "S";
                else
                    caufor = "N";
            }
        }

        public bool causolBool
        {
            get
            {
                return causol == "S";
            }
            set
            {
                if (value)
                    causol = "S";
                else
                    causol = "N";
            }
        }

        public bool caucecoBool
        {
            get
            {
                return cauceco == "S";
            }
            set
            {
                if (value)
                    cauceco = "S";
                else
                    cauceco = "N";
            }
        }


        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
        public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
        #endregion

        public string? cauter { get; set; }
        public string? cauint { get; set; }

        public string? causos { get; set; }
        public string? caude2 { get; set; }
        public string? cauaut { get; set; }
    }
}