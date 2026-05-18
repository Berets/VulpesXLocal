using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.General
{
    public abstract class NOTECLI1WindowViewModel : Base
    {
        public NOTECLI1WindowViewModel()
        {
            NoteTypes = new ObservableCollection<GenericIDDescription>();
            MarkTypes = new ObservableCollection<GenericIDDescription>();
        }

        public required NOTECLI1 Data { get; set; }
        public bool IsInsert { get; set; }
        public ObservableCollection<GenericIDDescription> NoteTypes { get; set; }
        public ObservableCollection<GenericIDDescription> MarkTypes { get; set; }

        public abstract string? Validate(NOTECLI1 Model, bool IsInsert);
    }

    public class NOTECLI1WindowViewModelDefault : NOTECLI1WindowViewModel
    {
        public NOTECLI1WindowViewModelDefault()
        {
            NoteTypes = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = "O", Description = "Ordini" },
                    new GenericIDDescription { ID = "B", Description = "Bolle" },
                    new GenericIDDescription { ID = "F", Description = "Fatture" },
                    new GenericIDDescription { ID = "G", Description = "Generale" }};
        }

        public override string? Validate(NOTECLI1 Model, bool IsInsert)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<INOTECLI1Repository>().Validate(Model, IsInsert);
        }
    }

    public class NOTECLI1WindowViewModelUfp : NOTECLI1WindowViewModel
    {
        public NOTECLI1WindowViewModelUfp()
        {
            NoteTypes = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = "O", Description = "Ordini" },
                    new GenericIDDescription { ID = "B", Description = "Bolle" },
                    new GenericIDDescription { ID = "F", Description = "Fatture" },
                    new GenericIDDescription { ID = "G", Description = "Generale" },
                    new GenericIDDescription { ID = "P", Description = "Offerte" },
                    new GenericIDDescription { ID = "L", Description = "Legali" },
                    new GenericIDDescription { ID = "M", Description = "Marcatura" },
                    new GenericIDDescription { ID = "Z", Description = "Produzione" },
                    new GenericIDDescription { ID = "C", Description = "Marcatura C/Lavoro" }};

            MarkTypes = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = "N", Description = "Non marcare" },
                    new GenericIDDescription { ID = "C", Description = "Solo N° commessa" },
                    new GenericIDDescription { ID = "T", Description = "Completa" }
            };
        }

        public override string? Validate(NOTECLI1 Model, bool IsInsert)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<INOTECLI1Repository>().Validate(Model, IsInsert);
        }
    }
}
