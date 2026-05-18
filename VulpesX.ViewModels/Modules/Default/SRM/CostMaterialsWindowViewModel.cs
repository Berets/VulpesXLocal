using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.DAL.SRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.SRM
{
    public class CostMaterialsWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public CostMaterialsWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public string? OldProductID { get; set; }
        public int? OldYear { get; set; }
        public int? OldMonth { get; set; }

        public required tab_articolo_costi Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<tab_articolo>? Products { get; set; }

        private tab_articolo? selectedProduct;
        public tab_articolo? SelectedProduct { get => selectedProduct; set { selectedProduct = value; NotifyPropertyChanged("SelectedProduct"); } }

        public DateTime? SelectedDate { get; set; }

        public ObservableCollection<tab_articolo>? GetTab_Articolos()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_costiRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (!IsInsert && !string.IsNullOrEmpty(OldProductID) && OldYear.HasValue && OldMonth.HasValue)
            {
                VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_costiRepository>().Delete(new tab_articolo_costi
                {
                    cid = Data.cid,
                    product_id = OldProductID,
                    year = OldYear!.Value,
                    month = OldMonth!.Value,
                    rv = Data.rv,
                });
            }

            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_costiRepository>().Add(Data);
        }
    }
}
