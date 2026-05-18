using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class SPEDIZIONEWindowViewModel : Base
    {
        public required SPEDIZIONE Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<LINGUA>? Lingue { get; set; }
        public LINGUA? SelectedLingua { get; set; }

        private SPEDIZIONE_LINGUA? selectedTraduzione;
        public SPEDIZIONE_LINGUA? SelectedTraduzione { get { return selectedTraduzione; } set { selectedTraduzione = value; NotifyPropertyChanged(); } }

        public bool InsertOrUpdateLanguage(SPEDIZIONE_LINGUA Language)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISPEDIZIONE_LINGUARepository>().InsertOrUpdate(Language);
        }

        public SPEDIZIONE_LINGUA GetSPEDIZIONE_LINGUA(string ID, string LanguageID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISPEDIZIONE_LINGUARepository>().Get(ID, LanguageID) ?? new SPEDIZIONE_LINGUA { specod = ID, lincod = LanguageID };
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISPEDIZIONERepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<ISPEDIZIONERepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<ISPEDIZIONERepository>().Update(Data);
        }
    }
}
