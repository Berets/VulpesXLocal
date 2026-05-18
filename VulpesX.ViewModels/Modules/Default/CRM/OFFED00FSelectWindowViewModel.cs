using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class OFFED00FSelectWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string UserID { get; set; }

        public OFFED00FSelectWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required List<OFFET00F> OffersHeads { get; set; }
        public OFFET00F FirstHead => OffersHeads.First();
        public ObservableCollection<OFFED00F>? AvailableRows { get; set; }
        public ObservableCollection<OFFETAL00F>? AvailableAttachments { get; set; }
        public bool HasAllSigns { get; set; }

        public OFFET00F? GetFull(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IOFFET00FRepository>().GetFull(CompanyID, Year, ID);
        }
        public bool Update(OFFET00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IOFFET00FRepository>().Update(Item);
        }

        public bool GenerateByOffer(List<OFFED00F> Rows,List<OFFETAL00F> Attachments)
        {
            if(CompanyUID.HasValue)
                return VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().GenerateByOffer(OffersHeads.First(), Rows, Attachments,UserID, HasAllSigns, CompanyUID!.Value);

            return false;
        }
    }
}
