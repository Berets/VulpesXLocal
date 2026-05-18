using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Generics
{
    public class GenericIDDescriptionType : Base
    {
        public GenericIDDescriptionType() { }
        public GenericIDDescriptionType(string ID, string Description, string Type)
        {
            this.ID = ID;
            this.Description = Description;
            this.Type = Type;
        }
        private string? id;
        public string? ID
        {
            get => id; set
            {
                id = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("FullDescription");
            }
        }

        private string? description;
        public string? Description
        {
            get => description; set
            {
                description = value;
                NotifyPropertyChanged("Description");
                NotifyPropertyChanged("FullDescription");
            }
        }
        private string? type;
        public string? Type
        {
            get => type; set
            {
                type = value;
                NotifyPropertyChanged("Type");
            }
        }

        #region Overrides
        public override string ToString()
        {
            return $"{ID} {Description} {Type}";
        }
        #endregion
    }
}
