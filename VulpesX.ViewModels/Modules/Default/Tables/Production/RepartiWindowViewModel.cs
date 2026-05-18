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
    public class RepartiWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public RepartiWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            Risorse = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().GetList(CompanyID);
        }

        public required tab_produzione_reparto Data { get; set; }
        public ObservableCollection<tab_produzione_risorsa>? Risorse { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_repartoRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_repartoRepository>().Insert(Data);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_repartoRepository>().Update(Data);
            }
        }

        public ObservableCollection<tab_produzione_risorsa>? GetRequiredService(string WardID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().GetListFromReparto(CompanyID, WardID);
        }
    }
}
