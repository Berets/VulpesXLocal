using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.CRM.AF
{
    public class ArticleFilterStateModel
    {
        // String Filters
        public string? Filter_ID { get; set; }
        public string? Filter_artdise { get; set; }

        // Auto Filters
        public string? SelectedTIPTA00F_Key { get; set; }
        public string? SelectedANALOGIE_Key { get; set; }
        public string? SelectedRIVESTIMENTI_Key { get; set; }
        public string? SelectedTIPMATPRI_Key { get; set; }
        public string? SelectedMATERIEPRIME_Key { get; set; }
        public string? SelectedDENTI_Key { get; set; }
        public string? SelectedDIAMETRO_Key { get; set; }
        public string? SelectedLD_Key { get; set; }
        public string? SelectedFORILUBRIFICATI_Key { get; set; }
        public string? SelectedATTACCO_Key { get; set; }

        // Decimal Ranges
        public decimal? Filter_artdi3_From { get; set; }
        public decimal? Filter_artdi3_To { get; set; }
        public decimal? Filter_artdi1_From { get; set; }
        public decimal? Filter_artdi1_To { get; set; }
        public decimal? Filter_artlun_From { get; set; }
        public decimal? Filter_artlun_To { get; set; }
        public decimal? Filter_artlar_From { get; set; }
        public decimal? Filter_artlar_To { get; set; }
        public decimal? Filter_artluncod_From { get; set; }
        public decimal? Filter_artluncod_To { get; set; }
        public decimal? Filter_artlinuta_From { get; set; }
        public decimal? Filter_artlinuta_To { get; set; }
        public decimal? Filter_artalt_From { get; set; }
        public decimal? Filter_artalt_To { get; set; }
    }
}
