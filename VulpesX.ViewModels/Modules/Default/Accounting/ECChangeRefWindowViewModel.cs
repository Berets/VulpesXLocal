using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;
using VulpesX.Shared.Utilities;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class ECChangeRefWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ECChangeRefWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required MastrinoECReportItem EC { get; set; }

        private GenericIDDescription? payment;
        public GenericIDDescription? Payment { get => payment; set { payment = value; NotifyPropertyChanged("Payment"); } }

        public ObservableCollection<TAB_ACC_TIPPAG>? PaymentTypes { get; set; }
        public ObservableCollection<GenericIDDescription>? Payments { get; set; }

        public DateTime? ReferenceDate { get; set; }
        public string? ReferenceID { get; set; }
        public DateTime? ExpireDate { get; set; }

        public TAB_ACC_TIPPAG? PaymentType { get; set; }

        public ObservableCollection<GenericIDDescription>? GetPAGCLIs()
        {
            if (EC.EntityType == "C")
                return VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().GetGenericList();
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().GetGenericList();
        }

        public ObservableCollection<TAB_ACC_TIPPAG>? GetTAB_ACC_TIPPAGs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPPAGRepository>().GetList();
        }

        public bool Save()
        {
            try
            {
                if (EC.EntityType == "C")
                {
                    var pnClientRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>();

                    var data = pnClientRepo.Get(EC.CompanyID, EC.Year, EC.Number, EC.RowID);

                    if (data != null)
                    {
                        data.N2SCAD = ExpireDate;
                        data.N2RIFE = ReferenceID;
                        data.N2DARI = ReferenceDate;
                        data.N2PAGA = Payment?.ID;
                        data.n2tipi = PaymentType?.inccod;

                        return pnClientRepo.Update(data);
                    }

                    return false;
                }
                else
                {
                    var pnFornitoriRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>();

                    var data = pnFornitoriRepo.Get(EC.CompanyID, EC.Year, EC.Number, EC.RowID);

                    if (data != null)
                    {
                        data.N3SCAD = ExpireDate;
                        data.N3RIFE = ReferenceID;
                        data.N3DARI = ReferenceDate;
                        data.N3PAGA = Payment?.ID;
                        data.n3tipp = PaymentType?.inccod;

                        return pnFornitoriRepo.Update(data);
                    }

                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
