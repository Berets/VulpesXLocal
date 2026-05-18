using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.General
{
    public class NOTEFORWindowViewModel : Base
    {
        private INOTEFORRepository _noteForRepository;

        public NOTEFORWindowViewModel(INOTEFORRepository noteForRepository)
        {
            _noteForRepository = noteForRepository;
        }

        public required NOTEFOR Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate(NOTEFOR Model, bool IsInsert)
        {
            return _noteForRepository.Validate(Model, IsInsert);
        }
    }
}
