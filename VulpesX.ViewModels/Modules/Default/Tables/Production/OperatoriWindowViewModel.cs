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
    public class OperatoriWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public OperatoriWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            Risorse = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().GetList(CompanyID);
        }

        public required tab_produzione_operatore Data { get; set; }
        public ObservableCollection<tab_produzione_risorsa>? Risorse { get; set; }
        public ObservableCollection<tab_produzione_operatore_costo>? Costs { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_operatoreRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_operatoreRepository>().Insert(Data, Costs ?? new ObservableCollection<tab_produzione_operatore_costo>());

            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_operatoreRepository>().Update(Data, Costs ?? new ObservableCollection<tab_produzione_operatore_costo>());
            }
        }

        public string? ValidateCosts(tab_produzione_operatore_costo Cost)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_operatore_costoRepository>().Validate(Cost, true);
        }
    }
}
