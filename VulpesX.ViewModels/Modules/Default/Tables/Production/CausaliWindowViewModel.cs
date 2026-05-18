using DocumentFormat.OpenXml.EMMA;
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
    public class CausaliWindowViewModel
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public CausaliWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            Risorse = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().GetList(CompanyID);
        }

        public required tab_produzione_causale Data { get; set; }
        public ObservableCollection<tab_produzione_risorsa>? Risorse { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_causaleRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_causaleRepository>().Insert(Data);

            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_causaleRepository>().Update(Data);
            }
        }

        public ObservableCollection<tab_produzione_risorsa>? GetListFromCausale(string CausalID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().GetListFromCausale(CompanyID, CausalID);
        }
    }
}
