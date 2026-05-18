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
    public class CONSEGNAWindowViewModel : Base
    {
        public required CONSEGNA Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<LINGUA>? Lingue { get; set; }
        public LINGUA? SelectedLingua { get; set; }

        private CONSEGNA_LINGUA? selectedTraduzione;
        public CONSEGNA_LINGUA? SelectedTraduzione { get { return selectedTraduzione; } set { selectedTraduzione = value; NotifyPropertyChanged(); } }

        public bool InsertOrUpdateCONSEGNA_LINGUA(CONSEGNA_LINGUA Language)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICONSEGNA_LINGUARepository>().InsertOrUpdate(Language);
        }

        public CONSEGNA_LINGUA GetCONSEGNA_LINGUA(string ID, string LanguageID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICONSEGNA_LINGUARepository>().Get(ID, LanguageID) ?? new CONSEGNA_LINGUA { concod = ID, lincod = LanguageID };
        }


        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICONSEGNARepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<ICONSEGNARepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<ICONSEGNARepository>().Update(Data);
        }
    }
}
