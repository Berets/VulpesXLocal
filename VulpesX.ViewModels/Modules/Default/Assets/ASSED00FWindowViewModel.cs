using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Assets;
using VulpesX.DAL.General;
using VulpesX.DAL.Store;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Assets
{
    public class ASSED00FWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ASSED00FWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required ASSET00F Head { get; set; }

        private ObservableCollection<ASSED00F>? rows;
        public ObservableCollection<ASSED00F>? Rows
        {
            get { return rows; }
            set
            {
                rows = value;
                foreach (var item in rows ?? new ObservableCollection<ASSED00F>())
                {
                    item.Products = Products;
                }
            }
        }

        public ObservableCollection<tab_articolo>? Products { get; set; }

        public void LoadProducts()
        {
            Products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);
        }

        public void LoadRows()
        {
            Rows = VulpesServiceProvider.Provider.GetRequiredService<IASSED00FRepository>().GetList(CompanyID, Head.id);
        }

        public decimal GetQuantityStock(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocksRepository>().GetListByProduct(CompanyID, ProductID)?.Sum(s => s.QuantityAvailable) ?? 0;
        }

        public string? Validate(ASSED00F Data)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IASSED00FRepository>().Validate(Data, true);
        }

        public string? ValidateAll()
        {
            if (Rows != null && Rows.Count > 0)
            {
                string? validation = null;

                foreach (var row in Rows)
                {
                    validation = VulpesServiceProvider.Provider.GetRequiredService<IASSED00FRepository>().Validate(row, false);

                    if (!string.IsNullOrWhiteSpace(validation))
                        break;
                }
                if (string.IsNullOrWhiteSpace(validation))
                {
                    return null;
                }
                else
                {
                    return validation;
                }
            }
            else
            {
                return "E' necessario che siano presenti delle righe per confermare l'asset";
            }
        }

        public bool Save()
        {
            Head.updatedUserID = UserID;

            return VulpesServiceProvider.Provider.GetRequiredService<IASSED00FRepository>().UpdateAll(Head, (Rows ?? new ObservableCollection<ASSED00F>()).ToList());
        }
    }
}
