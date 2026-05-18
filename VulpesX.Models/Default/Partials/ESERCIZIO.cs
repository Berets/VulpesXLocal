namespace VulpesX.Models.Default
{
    public partial class ESERCIZIO
    {
        public bool eseaggBool
        {
            get
            {
                return eseagg == "S";
            }
            set
            {
                if (value)
                    eseagg = "S";
                else
                    eseagg = "N";
            }
        }

        public bool esecomBool
        {
            get
            {
                return esecom == "S";
            }
            set
            {
                if (value)
                    esecom = "S";
                else
                    esecom = "N";
            }
        }

        public bool eseivavenBool
        {
            get
            {
                return eseivaven == "S";
            }
            set
            {
                if (value)
                    eseivaven = "S";
                else
                    eseivaven = "N";
            }
        }

        public DateTime Start { get { return new DateTime(eseann, eseini ?? 1, 1); } }
        public DateTime End { get { return Start.AddYears(1).AddDays(-1); } }
        #region Computed
        public DateTime GetIVAStart(DateTime Now)
        {
            DateTime Start = Now;
            if (eseliq == "M")
            {
                return new DateTime(Now.Year, Now.Month, 1);
            }
            else
            {
                switch (Now.Month)
                {
                    case 1:
                    case 2:
                    case 3:
                        return new DateTime(Now.Year, 1, 1);
                    case 4:
                    case 5:
                    case 6:
                        return new DateTime(Now.Year, 4, 1);
                    case 7:
                    case 8:
                    case 9:
                        return new DateTime(Now.Year, 7, 1);
                    case 10:
                    case 11:
                    case 12:
                        return new DateTime(Now.Year, 10, 1);
                }
            }

            return Now;
        }

        public DateTime GetIVAEnd(DateTime Now)
        {
            DateTime Start = Now;
            if (eseliq == "M")
            {
                return new DateTime(Now.Year, Now.Month, DateTime.DaysInMonth(Now.Year, Now.Month));
            }
            else
            {
                switch (Now.Month)
                {
                    case 1:
                    case 2:
                    case 3:
                        return new DateTime(Now.Year, 3, DateTime.DaysInMonth(Now.Year, 3));
                    case 4:
                    case 5:
                    case 6:
                        return new DateTime(Now.Year, 6, DateTime.DaysInMonth(Now.Year, 6));
                    case 7:
                    case 8:
                    case 9:
                        return new DateTime(Now.Year, 9, DateTime.DaysInMonth(Now.Year, 9));
                    case 10:
                    case 11:
                    case 12:
                        return new DateTime(Now.Year, 12, DateTime.DaysInMonth(Now.Year, 12));
                }
            }

            return Now;
        }
        #endregion

        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
        public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
        #endregion
    }
}
