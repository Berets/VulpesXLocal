using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.General;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class ISOWindowViewModel : Base
    {
        public ISOWindowViewModel()
        {
            Languages = VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetList();
        }
        public required ISO Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<LINGUA>? Languages { get; set; }
        public LINGUA? SelectedLanguage { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IISORepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IISORepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IISORepository>().Update(Data);
        }
    }
}
