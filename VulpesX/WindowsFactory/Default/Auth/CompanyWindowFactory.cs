using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.WindowsFactory.Default.Auth
{


    public interface ICompanyWindowFactory
    {
        IWindowFactory Create();
    }

    public class CompanyWindowFactory : ICompanyWindowFactory
    {
        public IWindowFactory Create()
        {
            switch (UserContext.Instance.Domain)
            {
                case Constants.UFP_DOMAIN:
                    return new Modules.Ufp.Auth.CompanyWindow();
                default:
                    return new Modules.Default.Auth.CompanyWindow();
            }
        }
    }
}
