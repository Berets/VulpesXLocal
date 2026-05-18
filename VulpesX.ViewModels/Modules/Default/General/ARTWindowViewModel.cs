using DocumentFormat.OpenXml.EMMA;
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
using VulpesX.DAL.Tables.Article;
using VulpesX.DAL.Tables.General;
using VulpesX.DAL.Tables.Productions;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.General
{
    public class ARTWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ARTWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required tab_articolo Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<tab_articolo_categoria>? Categorie { get; set; }
        public ObservableCollection<tab_articolo_unita>? Unita { get; set; }
        public ObservableCollection<tab_articolo_tipo>? Tipi { get; set; }
        public ObservableCollection<ABE>? Fornitori { get; set; }
        public ObservableCollection<ASSOGGETAMENTI>? Aliquote { get; set; }
        public ASSOGGETAMENTI? Aliquota { get; set; }

        public ObservableCollection<tab_produzione_risorsa>? RisorseList { get; set; }
        public ObservableCollection<tab_produzione_risorsa_sorgenti>? Allsources { get; set; }

        private ObservableCollection<tab_articolo_produzione_sorgenti>? plantSources;
        public ObservableCollection<tab_articolo_produzione_sorgenti>? PlantSources
        {
            get
            {
                return plantSources;
            }
            set
            {
                plantSources = value;
                foreach (var item in plantSources ?? new ObservableCollection<tab_articolo_produzione_sorgenti>())
                {
                    item.RisorseList = RisorseList;
                    item.AllSources = Allsources;
                }
            }
        }

        public ObservableCollection<string>? Revisions { get; set; }
        public ObservableCollection<tab_articolo_extern>? ExternalCodes { get; set; }

        public ObservableCollection<LINGUA>? Lingue { get; set; }
        public LINGUA? SelectedLingua { get; set; }

        private tab_articolo_lingua? selectedTraduzione;
        public tab_articolo_lingua? SelectedTraduzione { get { return selectedTraduzione; } set { selectedTraduzione = value; NotifyPropertyChanged("SelectedTraduzione"); } }

        public void LoadDetails()
        {
            Categorie = VulpesServiceProvider.Provider.GetRequiredService<ICategoriaRepository>().GetList(CompanyID);
            Unita = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetList(CompanyID);
            Tipi = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_tipoRepository>().GetList(CompanyID);
            Fornitori = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
            Aliquote = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetList();
            RisorseList = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>().GetList(CompanyID);
            Allsources = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsa_sorgentiRepository>().GetFullList(CompanyID);

            Data.AccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            Data.SubaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
            Data.CostGroupsList = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
            Data.RevenueGroupsList = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
            Data.CostCentersList = VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().GetList(CompanyID, false);
            Revisions = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().GetRevisioniSimpleList(CompanyID, Data.ID);
            Lingue = VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetList(CompanyID);

        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (SelectedTraduzione != null)
            {
                SelectedTraduzione.ArticoloID = Data.ID;

                VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_linguaRepository>().InsertOrUpdate(SelectedTraduzione);
            }

            if (IsInsert)
            {
                Data.LogAddedUserID = UserID;

                return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Insert(Data, PlantSources ?? new ObservableCollection<tab_articolo_produzione_sorgenti>(), ExternalCodes ?? new ObservableCollection<tab_articolo_extern>());
            }
            else
            {
                Data.LogUpdatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Update(Data, PlantSources ?? new ObservableCollection<tab_articolo_produzione_sorgenti>(), ExternalCodes ?? new ObservableCollection<tab_articolo_extern>());
            }
        }

        public ObservableCollection<tab_articolo_produzione_sorgenti>? GetSources()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_produzione_sorgentiRepository>().GetList(CompanyID, Data.ID);
        }

        public string? ValidateSource(tab_articolo_produzione_sorgenti Source)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_produzione_sorgentiRepository>().Validate(Source, true);
        }

        public ObservableCollection<tab_articolo_extern>? GetExternals()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_externRepository>().GetList(CompanyID, Data.ID);
        }

        public string? ValidateExternal(tab_articolo_extern External)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_externRepository>().Validate(External, true);
        }

        public tab_articolo_lingua? GetLanguage(string LanguageID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_linguaRepository>().Get(CompanyID, Data.ID, LanguageID);
        }

        public bool InsertOrUpdateLanguage()
        {
            if (SelectedTraduzione != null)
            {
                SelectedTraduzione.ArticoloID = Data.ID;

                return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_linguaRepository>().InsertOrUpdate(SelectedTraduzione);
            }

            return false;
        }
    }
}
