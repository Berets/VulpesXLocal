using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.General;
using VulpesX.Models.Default;
using VulpesX.Models.Models;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.General
{
    public class LINGUAWindowViewModel : Base
    {
        public required LINGUA Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<LanguageModel>? LanguageReports { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().Update(Data);
        }
    }
}
