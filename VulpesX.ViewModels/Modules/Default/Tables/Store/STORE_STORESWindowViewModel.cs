using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Assets;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Tables.Store
{
    public class STORE_STORESWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public STORE_STORESWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required store_stores Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<GenericIDDescription> StoreTypes => CommonsService.StoreTypes;

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().Insert(Data);

            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().Update(Data);
            }
        }
    }
}
