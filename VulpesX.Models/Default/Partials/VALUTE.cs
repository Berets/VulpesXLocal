using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Default
{
    public partial class VALUTE
    {
        public string FullDescriptionSearchable => $"{VALCOD} {VALDIV?.Trim()} {VALDES?.TrimEnd()}";


        public ObservableCollection<GenericIDDescription> Types => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "E", Description = "Euro" },
            new GenericIDDescription(){ ID = "L", Description = "Lire" },
            new GenericIDDescription(){ ID = null, Description = "Nessuno" },
        };

        public string? VALTIPDescription => Types.Where(w => w.ID == VALTIP).FirstOrDefault()?.Description;

    }
}
