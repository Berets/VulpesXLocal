using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Shared;
using static VulpesX.Models.Models.Accounting.AccountingSituationViewModel;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class AccountingDetailWindowViewModel : Base
    {
        public string? Title { get; set; }
        public ObservableCollection<ASItem>? Data { get; set; }
    }
}
