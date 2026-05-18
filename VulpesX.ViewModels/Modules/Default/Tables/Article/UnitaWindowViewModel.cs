using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Article
{
    public class UnitaWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public UnitaWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required tab_articolo_unita Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_unitaRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_unitaRepository>().Insert(Data);

            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_unitaRepository>().Update(Data);
            }
        }
    }
}
