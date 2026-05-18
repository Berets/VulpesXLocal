using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Productions;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Default.Partials;
using VulpesX.Models.Models.CRM.AF;
using VulpesX.Models.Ufp;
using VulpesX.Shared;
using VulpesX.Shared.Generics;
using static VulpesX.Shared.Utilities.TelerikGridService;

namespace VulpesX.ViewModels.Modules.Ufp.General
{
    public class ARTSelectViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string Type { get; set; }

        public ARTSelectViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public ObservableCollection<TIPTA00F>? TIPTA00Fs { get; set; }
        public ObservableCollection<ANALOGIE>? ANALOGIEs { get; set; }
        public ObservableCollection<RIVESTIMENTI>? RIVESTIMENTIs { get; set; }
        public ObservableCollection<ATTACCO>? ATTACCOs { get; set; }
        public ObservableCollection<DENTI>? DENTIs { get; set; }
        public ObservableCollection<DIAMETRO>? DIAMETROs { get; set; }
        public ObservableCollection<FORILUBRIFICATI>? FORILUBRIFICATIs { get; set; }
        public ObservableCollection<LD>? LDs { get; set; }
        public ObservableCollection<MATERIEPRIME>? MATERIEPRIMEs { get; set; }
        public ObservableCollection<TIPMATPRI>? TIPMATPRIs { get; set; }

        #region Filters
        private TIPTA00F? tIPTA00F;
        public TIPTA00F? TIPTA00F { get { return tIPTA00F; } set { tIPTA00F = value; NotifyPropertyChanged(); } }

        private string? filter_ID;
        public string? Filter_ID { get { return filter_ID; } set { filter_ID = value; NotifyPropertyChanged(); } }

        private string? filter_artdise;
        public string? Filter_artdise { get { return filter_artdise; } set { filter_artdise = value; NotifyPropertyChanged(); } }

        private ANALOGIE? aNALOGIE;
        public ANALOGIE? ANALOGIE { get { return aNALOGIE; } set { aNALOGIE = value; NotifyPropertyChanged(); } }

        private RIVESTIMENTI? rIVESTIMENTI;
        public RIVESTIMENTI? RIVESTIMENTI { get { return rIVESTIMENTI; } set { rIVESTIMENTI = value; NotifyPropertyChanged(); } }

        private TIPMATPRI? tIPMATPRI;
        public TIPMATPRI? TIPMATPRI { get { return tIPMATPRI; } set { tIPMATPRI = value; NotifyPropertyChanged(); } }

        private MATERIEPRIME? mATERIEPRIME;
        public MATERIEPRIME? MATERIEPRIME { get { return mATERIEPRIME; } set { mATERIEPRIME = value; NotifyPropertyChanged(); } }

        private DENTI? dENTI;
        public DENTI? DENTI { get { return dENTI; } set { dENTI = value; NotifyPropertyChanged(); } }

        private DIAMETRO? dIAMETRO;
        public DIAMETRO? DIAMETRO { get { return dIAMETRO; } set { dIAMETRO = value; NotifyPropertyChanged(); } }

        private LD? lD;
        public LD? LD { get { return lD; } set { lD = value; NotifyPropertyChanged(); } }

        private FORILUBRIFICATI? fORILUBRIFICATI;
        public FORILUBRIFICATI? FORILUBRIFICATI { get { return fORILUBRIFICATI; } set { fORILUBRIFICATI = value; NotifyPropertyChanged(); } }

        private ATTACCO? aTTACCO;
        public ATTACCO? ATTACCO { get { return aTTACCO; } set { aTTACCO = value; NotifyPropertyChanged(); } }


        private decimal? filter_artdi3_From;
        public decimal? Filter_artdi3_From { get { return filter_artdi3_From; } set { filter_artdi3_From = value; NotifyPropertyChanged(); } }

        private decimal? filter_artdi3_To;
        public decimal? Filter_artdi3_To { get { return filter_artdi3_To; } set { filter_artdi3_To = value; NotifyPropertyChanged(); } }

        private decimal? filter_artdi1_From;
        public decimal? Filter_artdi1_From { get { return filter_artdi1_From; } set { filter_artdi1_From = value; NotifyPropertyChanged(); } }

        private decimal? filter_artdi1_To;
        public decimal? Filter_artdi1_To { get { return filter_artdi1_To; } set { filter_artdi1_To = value; NotifyPropertyChanged(); } }

        private decimal? filter_artlun_From;
        public decimal? Filter_artlun_From { get { return filter_artlun_From; } set { filter_artlun_From = value; NotifyPropertyChanged(); } }

        private decimal? filter_artlun_To;
        public decimal? Filter_artlun_To { get { return filter_artlun_To; } set { filter_artlun_To = value; NotifyPropertyChanged(); } }

        private decimal? filter_artlar_From;
        public decimal? Filter_artlar_From { get { return filter_artlar_From; } set { filter_artlar_From = value; NotifyPropertyChanged(); } }

        private decimal? filter_artlar_To;
        public decimal? Filter_artlar_To { get { return filter_artlar_To; } set { filter_artlar_To = value; NotifyPropertyChanged(); } }

        private decimal? filter_artluncod_From;
        public decimal? Filter_artluncod_From { get { return filter_artluncod_From; } set { filter_artluncod_From = value; NotifyPropertyChanged(); } }

        private decimal? filter_artluncod_To;
        public decimal? Filter_artluncod_To { get { return filter_artluncod_To; } set { filter_artluncod_To = value; NotifyPropertyChanged(); } }

        private decimal? filter_artlinuta_From;
        public decimal? Filter_artlinuta_From { get { return filter_artlinuta_From; } set { filter_artlinuta_From = value; NotifyPropertyChanged(); } }

        private decimal? filter_artlinuta_To;
        public decimal? Filter_artlinuta_To { get { return filter_artlinuta_To; } set { filter_artlinuta_To = value; NotifyPropertyChanged(); } }

        private decimal? filter_artalt_From;
        public decimal? Filter_artalt_From { get { return filter_artalt_From; } set { filter_artalt_From = value; NotifyPropertyChanged(); } }

        private decimal? filter_artalt_To;
        public decimal? Filter_artalt_To { get { return filter_artalt_To; } set { filter_artalt_To = value; NotifyPropertyChanged(); } }
        #endregion

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        private int totalCount;
        public int TotalCount
        {
            get => totalCount; set
            {
                totalCount = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<tab_articolo>? items;
        public ObservableCollection<tab_articolo>? Items { get { return items; } set { items = value; NotifyPropertyChanged(); } }

        public event Action<int, bool>? SearchCompleted;

        public void LoadDetails()
        {
            TIPTA00Fs = VulpesServiceProvider.Provider.GetRequiredService<ITIPTA00FRepository>().GetList();
            ANALOGIEs = VulpesServiceProvider.Provider.GetRequiredService<IANALOGIERepository>().GetList();
            RIVESTIMENTIs = VulpesServiceProvider.Provider.GetRequiredService<IRIVESTIMENTIRepository>().GetList();
            ATTACCOs = VulpesServiceProvider.Provider.GetRequiredService<IATTACCORepository>().GetList();
            DENTIs = VulpesServiceProvider.Provider.GetRequiredService<IDENTIRepository>().GetList();
            DIAMETROs = VulpesServiceProvider.Provider.GetRequiredService<IDIAMETRORepository>().GetList();
            FORILUBRIFICATIs = VulpesServiceProvider.Provider.GetRequiredService<IFORILUBRIFICATIRepository>().GetList();
            LDs = VulpesServiceProvider.Provider.GetRequiredService<ILDRepository>().GetList();
            MATERIEPRIMEs = VulpesServiceProvider.Provider.GetRequiredService<IMATERIEPRIMERepository>().GetList();
            TIPMATPRIs = VulpesServiceProvider.Provider.GetRequiredService<ITIPMATPRIRepository>().GetList();
        }

        public async Task Search(bool IsUserSearch)
        {
            IsBusy = true;

            var filterList = new List<FilterEntry>();

            #region Filters
            if (TIPTA00F != null && !string.IsNullOrEmpty(TIPTA00F.FullDescription?.TrimEnd()))
            {
                filterList.Add(new FilterEntry("arttipFull", "arttipFull", "LIKE", TIPTA00F.FullDescription));
            }
            if (!string.IsNullOrEmpty(Filter_ID?.TrimEnd()))
            {
                filterList.Add(new FilterEntry("ID", "ID", "=", Filter_ID));
            }
            if (!string.IsNullOrEmpty(Filter_artdise?.TrimEnd()))
            {
                filterList.Add(new FilterEntry("artdise", "artdise", "LIKE", Filter_artdise));
            }
            if (ANALOGIE != null && !string.IsNullOrEmpty(ANALOGIE.FullDescription?.TrimEnd()))
            {
                filterList.Add(new FilterEntry("artfamFull", "artfamFull", "LIKE", ANALOGIE.FullDescription));
            }
            if (RIVESTIMENTI != null && !string.IsNullOrEmpty(RIVESTIMENTI.FullDescription?.TrimEnd()))
            {
                filterList.Add(new FilterEntry("artcorFull", "artcorFull", "LIKE", RIVESTIMENTI.FullDescription));
            }
            if (TIPMATPRI != null && !string.IsNullOrEmpty(TIPMATPRI.FullDescription?.TrimEnd()))
            {
                filterList.Add(new FilterEntry("arttipmatFull", "arttipmatFull", "LIKE", TIPMATPRI.FullDescription));
            }
            if (MATERIEPRIME != null && !string.IsNullOrEmpty(MATERIEPRIME.FullDescription?.TrimEnd()))
            {
                filterList.Add(new FilterEntry("artmapFull", "artmapFull", "LIKE", MATERIEPRIME.FullDescription));
            }
            if (DENTI != null && !string.IsNullOrEmpty(DENTI.FullDescription?.TrimEnd()))
            {
                filterList.Add(new FilterEntry("artdenFull", "artdenFull", "LIKE", DENTI.FullDescription));
            }
            if (DIAMETRO != null && !string.IsNullOrEmpty(DIAMETRO.FullDescription?.TrimEnd()))
            {
                filterList.Add(new FilterEntry("artdiamFull", "artdiamFull", "LIKE", DIAMETRO.FullDescription));
            }
            if (LD != null && !string.IsNullOrEmpty(LD.FullDescription?.TrimEnd()))
            {
                filterList.Add(new FilterEntry("artldFull", "artldFull", "LIKE", LD.FullDescription));
            }
            if (FORILUBRIFICATI != null && !string.IsNullOrEmpty(FORILUBRIFICATI.FullDescription?.TrimEnd()))
            {
                filterList.Add(new FilterEntry("artforiFull", "artforiFull", "LIKE", FORILUBRIFICATI.FullDescription));
            }
            if (ATTACCO != null && !string.IsNullOrEmpty(ATTACCO.FullDescription?.TrimEnd()))
            {
                filterList.Add(new FilterEntry("artatcoFull", "artatcoFull", "LIKE", ATTACCO.FullDescription));
            }

            if (Filter_artdi3_From.HasValue)
            {
                filterList.Add(new FilterEntry("artdi3#from", "artdi3", ">=", Filter_artdi3_From.Value.ToString()));
            }
            if (Filter_artdi3_To.HasValue)
            {
                filterList.Add(new FilterEntry("artdi3#to", "artdi3", "<=", Filter_artdi3_To.Value.ToString()));
            }
            if (Filter_artdi1_From.HasValue)
            {
                filterList.Add(new FilterEntry("artdi1#from", "artdi1", ">=", Filter_artdi1_From.Value.ToString()));
            }
            if (Filter_artdi1_To.HasValue)
            {
                filterList.Add(new FilterEntry("artdi1#to", "artdi1", "<=", Filter_artdi1_To.Value.ToString()));
            }
            if (Filter_artlun_From.HasValue)
            {
                filterList.Add(new FilterEntry("artlun#from", "artlun", ">=", Filter_artlun_From.Value.ToString()));
            }
            if (Filter_artlun_To.HasValue)
            {
                filterList.Add(new FilterEntry("artlun#to", "artlun", "<=", Filter_artlun_To.Value.ToString()));
            }
            if (Filter_artlar_From.HasValue)
            {
                filterList.Add(new FilterEntry("artlar#from", "artlar", ">=", Filter_artlar_From.Value.ToString()));
            }
            if (Filter_artlar_To.HasValue)
            {
                filterList.Add(new FilterEntry("artlar#to", "artlar", "<=", Filter_artlar_To.Value.ToString()));
            }
            if (Filter_artluncod_From.HasValue)
            {
                filterList.Add(new FilterEntry("artluncod#from", "artluncod", ">=", Filter_artluncod_From.Value.ToString()));
            }
            if (Filter_artluncod_To.HasValue)
            {
                filterList.Add(new FilterEntry("artluncod#to", "artluncod", "<=", Filter_artluncod_To.Value.ToString()));
            }
            if (Filter_artlinuta_From.HasValue)
            {
                filterList.Add(new FilterEntry("artlinuta#from", "artlinuta", ">=", Filter_artlinuta_From.Value.ToString()));
            }
            if (Filter_artlinuta_To.HasValue)
            {
                filterList.Add(new FilterEntry("artlinuta#to", "artlinuta", "<=", Filter_artlinuta_To.Value.ToString()));
            }
            if (Filter_artalt_From.HasValue)
            {
                filterList.Add(new FilterEntry("artalt#from", "artalt", ">=", Filter_artalt_From.Value.ToString()));
            }
            if (Filter_artalt_To.HasValue)
            {
                filterList.Add(new FilterEntry("artalt#to", "artalt", "<=", Filter_artalt_To.Value.ToString()));
            }
            #endregion

            var result = await Task.Run(() =>
            {
                int itemsCount = 0;

                var items = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetList(CompanyID, filterList, out itemsCount);

                return new { items, itemsCount };
            });

            Items = result.items;
            TotalCount = result.itemsCount;

            SearchCompleted?.Invoke(TotalCount, IsUserSearch);

            IsBusy = false;
        }

        public string? GetFilterJson()
        {
            var state = new ArticleFilterStateModel
            {
                // String
                Filter_ID = this.Filter_ID,
                Filter_artdise = this.Filter_artdise,

                // Auto
                SelectedTIPTA00F_Key = this.TIPTA00F?.tipcod,
                SelectedANALOGIE_Key = this.ANALOGIE?.angcod,
                SelectedRIVESTIMENTI_Key = this.RIVESTIMENTI?.rivecod,
                SelectedTIPMATPRI_Key = this.TIPMATPRI?.tmpcod,
                SelectedMATERIEPRIME_Key = this.MATERIEPRIME?.matpcod,
                SelectedDENTI_Key = this.DENTI?.Dencod,
                SelectedDIAMETRO_Key = this.DIAMETRO?.Diamcod,
                SelectedLD_Key = this.LD?.Ldcod,
                SelectedFORILUBRIFICATI_Key = this.FORILUBRIFICATI?.FLcod,
                SelectedATTACCO_Key = this.ATTACCO?.attacod,

                //Decimals
                Filter_artdi3_From = this.Filter_artdi3_From,
                Filter_artdi3_To = this.Filter_artdi3_To,
                Filter_artdi1_From = this.Filter_artdi1_From,
                Filter_artdi1_To = this.Filter_artdi1_To,
                Filter_artlun_From = this.Filter_artlun_From,
                Filter_artlun_To = this.Filter_artlun_To,
                Filter_artlar_From = this.Filter_artlar_From,
                Filter_artlar_To = this.Filter_artlar_To,
                Filter_artluncod_From = this.Filter_artluncod_From,
                Filter_artluncod_To = this.Filter_artluncod_To,
                Filter_artlinuta_From = this.Filter_artlinuta_From,
                Filter_artlinuta_To = this.Filter_artlinuta_To,
                Filter_artalt_From = this.Filter_artalt_From,
                Filter_artalt_To = this.Filter_artalt_To,
            };

            return JsonSerializer.Serialize(state);
        }

        public void SetFilterJson(string? Json)
        {
            if (string.IsNullOrEmpty(Json?.TrimEnd()))
                return;

            var state = JsonSerializer.Deserialize<ArticleFilterStateModel>(Json);

            if (state != null)
            {
                this.Filter_ID = state.Filter_ID;
                this.Filter_artdise = state.Filter_artdise;

                this.TIPTA00F = TIPTA00Fs?.FirstOrDefault(x => x.tipcod == state.SelectedTIPTA00F_Key);
                this.ANALOGIE = ANALOGIEs?.FirstOrDefault(x => x.angcod == state.SelectedANALOGIE_Key);
                this.RIVESTIMENTI = RIVESTIMENTIs?.FirstOrDefault(x => x.rivecod == state.SelectedRIVESTIMENTI_Key);
                this.TIPMATPRI = TIPMATPRIs?.FirstOrDefault(x => x.tmpcod == state.SelectedTIPMATPRI_Key);
                this.MATERIEPRIME = MATERIEPRIMEs?.FirstOrDefault(x => x.matpcod == state.SelectedMATERIEPRIME_Key);
                this.DENTI = DENTIs?.FirstOrDefault(x => x.Dencod == state.SelectedDENTI_Key);
                this.DIAMETRO = DIAMETROs?.FirstOrDefault(x => x.Diamcod == state.SelectedDIAMETRO_Key);
                this.LD = LDs?.FirstOrDefault(x => x.Ldcod == state.SelectedLD_Key);
                this.FORILUBRIFICATI = FORILUBRIFICATIs?.FirstOrDefault(x => x.FLcod == state.SelectedFORILUBRIFICATI_Key);
                this.ATTACCO = ATTACCOs?.FirstOrDefault(x => x.attacod == state.SelectedATTACCO_Key);

                this.Filter_artdi3_From = state.Filter_artdi3_From;
                this.Filter_artdi3_To = state.Filter_artdi3_To;
                this.Filter_artdi1_From = state.Filter_artdi1_From;
                this.Filter_artdi1_To = state.Filter_artdi1_To;
                this.Filter_artlun_From = state.Filter_artlun_From;
                this.Filter_artlun_To = state.Filter_artlun_To;
                this.Filter_artlar_From = state.Filter_artlar_From;
                this.Filter_artlar_To = state.Filter_artlar_To;
                this.Filter_artluncod_From = state.Filter_artluncod_From;
                this.Filter_artluncod_To = state.Filter_artluncod_To;
                this.Filter_artlinuta_From = state.Filter_artlinuta_From;
                this.Filter_artlinuta_To = state.Filter_artlinuta_To;
                this.Filter_artalt_From = state.Filter_artalt_From;
                this.Filter_artalt_To = state.Filter_artalt_To;
            }
        }
    }
}
