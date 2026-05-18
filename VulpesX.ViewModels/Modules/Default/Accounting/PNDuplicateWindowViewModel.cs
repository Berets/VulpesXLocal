using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class PNDuplicateWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public int HeadYear { get; set; }
        public int HeadNumber { get; set; }

        public PNDuplicateWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList("N");
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }

        public int? AccountingYear { get; set; }

        public DateTime? Date { get; set; }

        public ObservableCollection<CAUCONT>? Causals { get; set; }

        private CAUCONT? selectedCausal;
        public CAUCONT? SelectedCausal { get => selectedCausal; set { selectedCausal = value; NotifyPropertyChanged("SelectedCausal"); } }

        public bool IsRemove { get; set; }

        public ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public string? Validate()
        {
            if (!AccountingYear.HasValue)
            {
                return "L'esercizio è obbligatorio";
            }

            if (!Date.HasValue)
            {
                return "La data di registrazione è obbligatoria";
            }

            if (SelectedCausal == null)
            {
                return "La causale contabile è obbligatoria";
            }

            var causalEnabled = SelectedCausal.caugen == "S" && SelectedCausal.cauiva == "N" && SelectedCausal.caucli == "N" && SelectedCausal.caufor == "N" && SelectedCausal.cauter == "N" && SelectedCausal.cauint == "N";
            if (!causalEnabled)
            {
                return "La causale contabile deve essere solamente di tipo Generale";
            }

            return null;
        }

        public async Task<Tuple<string, int, int>?> Duplicate()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    var duplicate = VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().Duplicate(CompanyID, HeadYear, HeadNumber, AccountingYear!.Value, Date!.Value, SelectedCausal!, IsRemove);

                    return new { duplicate };
                });

                return result.duplicate;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
