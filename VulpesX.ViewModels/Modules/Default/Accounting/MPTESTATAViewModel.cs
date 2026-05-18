using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class MPTESTATAViewModel : Base
    {
        public required string CompanyID { get; set; }

        public MPTESTATAViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime year;
        public DateTime Year
        {
            get => year; set
            {
                year = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<MPTESTATA>? items;
        public ObservableCollection<MPTESTATA>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        public async Task Load()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    var items = VulpesServiceProvider.Provider.GetRequiredService<IMPTESTATARepository>().GetList(CompanyID, Year.Year);

                    return new { items };
                });

                Items = result.items;
            }
            finally
            {
                IsBusy = false;
            }
        }


        public bool CanDelete(MPTESTATA Model)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IMPTESTATARepository>().CanDelete(Model.MPSOCI, Model.MPANNO, Model.MPNUME);
        }

        public bool Delete(MPTESTATA Model)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IMPTESTATARepository>().Delete(Model);
        }

        public bool Sign(MPTESTATA Model)
        {
            Model.mpfirma = UserContext.Instance!.UserName;
            Model.mpdatfir = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            return VulpesServiceProvider.Provider.GetRequiredService<IMPTESTATARepository>().Update(Model);
        }

        public bool Update(MPTESTATA Model)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IMPTESTATARepository>().Update(Model);
        }

        public string? GenerateXML(MPTESTATA Model)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IMPTESTATARepository>().GenerateXML(Model.MPSOCI, Model.MPANNO, Model.MPNUME);
        }

        public ObservableCollection<ABE>? GetSuppliersCache()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
        }

        public ObservableCollection<MPDETTAGLIO>? GetMPDETTAGLIOs(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IMPDETTAGLIORepository>().GetList(CompanyID, Year, ID);
        }

        public PNFORNITORI? GetPNFORNITORI(int Year, int ID, int Row)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().Get(CompanyID, Year, ID, Row);
        }
    }
}
