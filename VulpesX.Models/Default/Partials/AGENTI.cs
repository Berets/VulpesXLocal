using System.Collections.ObjectModel;


namespace VulpesX.Models.Default
{
    public partial class AGENTI
    {
        public string FullDescriptionSearchable => $"{agecod} {agedes?.Trim()}";
        public string? agecalDescription => ComputeTypes.Where(w => w.ID == agecal).FirstOrDefault()?.Description;
        public string? ageliqDescription => LiquidationTypes.Where(w => w.ID == ageliq).FirstOrDefault()?.Description;
        public string? ageflagDescription => AgentTypes.Where(w => w.ID == ageflag).FirstOrDefault()?.Description;
        public string? agepvgtDescription => CommonsService.StandardValueTypes.Where(w => w.ID == agepvgt).FirstOrDefault()?.Description;

        public ObservableCollection<GenericIDDescription> ComputeTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "1", Description = "Pagato" },
            new GenericIDDescription(){ ID = "2", Description = "Fatturato" }
        };

        public ObservableCollection<GenericIDDescription> LiquidationTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "1", Description = "Mensile" },
            new GenericIDDescription(){ ID = "2", Description = "Trimestrale" }
        };

        public ObservableCollection<GenericIDDescription> AgentTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "M", Description = "Monomandatario" },
            new GenericIDDescription(){ ID = "P", Description = "Plurimandatario" }
        };

        public ObservableCollection<GenericIDDescription> AgentTypesUfp => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "I", Description = "Interno" },
            new GenericIDDescription(){ ID = "E", Description = "Esterno" },
            new GenericIDDescription(){ ID = "D", Description = "Dismesso" },
        };

        public bool CancelEnable { get { return !LogCanceled.HasValue; } }
        public bool ReactivateEnable { get { return LogCanceled.HasValue; } }

        public string? ageflg { get; set; }
        public bool ageatt { get { return ageflg != "D"; } }
        public string? agetel { get; set; }
        public string? agefax { get; set; }
        public string? agemail { get; set; }

        public string? ageinvmail { get; set; }
        public bool ageinvmailBool
        {
            get
            {
                return ageinvmail == "S";
            }
            set
            {
                if (value)
                    ageinvmail = "S";
                else
                    ageinvmail = "N";
            }
        }
        public string? ageflg2 { get; set; }
        public bool ageflg2Bool
        {
            get
            {
                return ageflg2 == "S";
            }
            set
            {
                if (value)
                    ageflg2 = "S";
                else
                    ageflg2 = "N";
            }
        }

        public bool ageflgBool
        {
            get
            {
                return ageflg != "D";
            }
            set
            {
                if (value)
                    ageflg = "I";
                else
                    ageflg = "D";
            }
        }

    }
}

