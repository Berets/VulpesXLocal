using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Generics
{
    public class GenericIDDescription : Base
    {
        public GenericIDDescription() { }
        public GenericIDDescription(string? ID, string? Description)
        {
            this.ID = ID;
            this.Description = Description;
        }

        private string? _id;
        public string? ID
        {
            get => _id; set
            {
                _id = value;
                NotifyPropertyChanged("ID");
                NotifyPropertyChanged("FullDescription");
            }
        }

        private string? _description;
        public string? Description
        {
            get => _description; set
            {
                _description = value;
                NotifyPropertyChanged("Description");
                NotifyPropertyChanged("FullDescription");
            }
        }

        public string FullDescription => $"{ID} {Description}";
        public string FullDescriptionSearchable => $"{ID} {Description}";
        public string FullDescriptionNotSearchable => $"[{ID}] {Description}";

        #region Overrides
        public override string ToString()
        {
            return FullDescription;
        }
        #endregion
    }
}
