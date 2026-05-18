using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.IVA
{
    public class TCOMLIQIVAWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public TCOMLIQIVAWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required TCOMLIQIVA Data { get; set; }
        public ObservableCollection<TCOMLIQIVA>? Rows { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCOMLIQIVARepository>().Validate(Data, IsInsert);
        }

        public bool Update()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCOMLIQIVARepository>().Update(Rows ?? new ObservableCollection<TCOMLIQIVA>());
        }
    }
}
