using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.ViewModels.Modules.Default.General;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.WindowsFactory.Default.Tables.Accounting
{

    public interface ICAUCONTWindowFactory
    {
        IWindowFactory Create(CAUCONTWindowViewModel dataContext);
    }

    public class CAUCONTWindowFactory : ICAUCONTWindowFactory
    {
        public IWindowFactory Create(CAUCONTWindowViewModel user)
        {
            switch (UserContext.Instance.Domain)
            {
                case Constants.UFP_DOMAIN:
                    return new Modules.Ufp.Tables.Accounting.CAUCONTWindow(user);
                default:
                    return new Modules.Default.Tables.Accounting.CAUCONTWindow(user);
            }
        }
    }
}

