using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Productions;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Production
{
    public class RisorseWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public RisorseWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required tab_produzione_risorsa Data { get; set; }
        public bool IsInsert { get; set; }
        public ObservableCollection<tab_produzione_risorsa_sorgenti>? Sources { get; set; }

        public ObservableCollection<tab_produzione_risorsa_costo>? Costs { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().Insert(Data, Sources ?? new ObservableCollection<tab_produzione_risorsa_sorgenti>(), Costs ?? new ObservableCollection<tab_produzione_risorsa_costo>());
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().Update(Data, Sources ?? new ObservableCollection<tab_produzione_risorsa_sorgenti>(), Costs ?? new ObservableCollection<tab_produzione_risorsa_costo>());
            }
        }

        public string? ValidateSource(tab_produzione_risorsa_sorgenti Source)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsa_sorgentiRepository>().Validate(Source, true);
        }

        public string? ValidateCost(tab_produzione_risorsa_costo Cost)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsa_costoRepository>().Validate(Cost, true);
        }
    }
}
