using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class SOLLECITI
    {
        public string FullDescriptionSearchable => $"{solcod} {soldes?.Trim()}";

        public bool tplegBool
        {
            get
            {
                return tpleg == "S";
            }
            set
            {
                if (value)
                    tpleg = "S";
                else
                    tpleg = "N";
            }
        }

        public bool tpflgBool
        {
            get
            {
                return tpflg == "S";
            }
            set
            {
                if (value)
                    tpflg = "S";
                else
                    tpflg = "N";
            }
        }

        public string? tpgraDescription => InstanceTypes.Where(w => w.ID == tpgra).FirstOrDefault()?.Description;

        public ObservableCollection<GenericIntIDDescription> InstanceTypes => new ObservableCollection<GenericIntIDDescription>() {
            new GenericIntIDDescription(){ ID = 1, Description = "1° grado" },
            new GenericIntIDDescription(){ ID = 2, Description = "2° grado" },
            new GenericIntIDDescription(){ ID = 3, Description = "3° grado" },
            new GenericIntIDDescription(){ ID = 4, Description = "4° grado" },
            new GenericIntIDDescription(){ ID = 5, Description = "5° grado" },
            new GenericIntIDDescription(){ ID = 6, Description = "6° grado" },
            new GenericIntIDDescription(){ ID = 7, Description = "7° grado" },
            new GenericIntIDDescription(){ ID = 8, Description = "8° grado" },
            new GenericIntIDDescription(){ ID = 9, Description = "9° grado" },
        };
    }
}
