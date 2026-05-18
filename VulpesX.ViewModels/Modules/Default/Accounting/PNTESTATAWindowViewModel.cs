using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class PNTESTATAWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required bool Populate { get; set; }
        public PNTESTATAWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public required PNTESTATA Data { get; set; }

        public ObservableCollection<CAUCONT>? Causals { get; set; }

        private ObservableCollection<ABE>? codes;
        public ObservableCollection<ABE>? Codes { get => codes; set { codes = value; NotifyPropertyChanged("Codes"); } }

        public ObservableCollection<GenericIDDescription>? CFFlags { get; set; }
        public string? SelectedCFFlag => CFFlags?.Where(w => w.ID == Data?.N1FLCF).Select(s => s.Description).FirstOrDefault();

        private ABE? selectedEntity;
        public ABE? SelectedEntity { get => selectedEntity; set { selectedEntity = value; NotifyPropertyChanged("SelectedEntity"); } }

        private CAUCONT? selectedCausal;
        public CAUCONT? SelectedCausal { get => selectedCausal; set { selectedCausal = value; NotifyPropertyChanged("SelectedCausal"); } }

        public bool IsInsert { get; set; }
        public bool IsReadonly { get; set; }
        public bool IsEnabled => !IsReadonly;

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().Validate(Data, IsInsert);
        }

        public LIBRIIVA? GetLIBRIIVA(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Get(ID);
        }

        public int GetProtocolNumber(LIBRIIVA IVA)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, Data.N1ANNO, new GenericIDDescription() { ID = IVA.livcod, Description = IVA.livdes }, true);
        }

        public ObservableCollection<ABE>? GetABE()
        {
            if(string.IsNullOrEmpty(Data.N1FLCF))
                return null;

            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList(Data.N1FLCF);
        }
    }
}
