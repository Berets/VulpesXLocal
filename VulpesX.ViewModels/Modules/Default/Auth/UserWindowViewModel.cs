using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Auth;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Auth
{
    public class UserWindowViewModel
    {
        public ACCESS? Data { get; set; }
        public bool IsInsert { get; set; }
        public ObservableCollection<SOCBASE>? Companies { get; set; }


        public void LoadDetails()
        {
            Data = VulpesServiceProvider.Provider.GetRequiredService<IAuthRepository>().GetFull(UserContext.Instance!.ACCESS!.USRID);
            Companies = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().GetDescriptionsList(UserContext.Instance!.ACCESS!.EnabledCompanies?.Select(s => s.SOMCOD).ToList() ?? new List<string>());
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAuthRepository>().Validate(Data!, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IAuthRepository>().Insert(Data!);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IAuthRepository>().Update(Data!);
            }
        }

        public string? ValidateChangePassword(string OldPassword, string NewPassword, string ConfirmNewPassword)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAuthRepository>().ValidateChangePassword(Data!.USRID, OldPassword, NewPassword, ConfirmNewPassword);
        }

        public bool Update()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAuthRepository>().Update(Data!);
        }
    }
}
