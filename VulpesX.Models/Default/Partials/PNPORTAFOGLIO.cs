

using Azure.Storage.Blobs.Models;
using System.Collections.ObjectModel;
using VulpesX.Models.Default.Partials;

namespace VulpesX.Models.Default
{
    public partial class PNPORTAFOGLIO : Base
    {
        private ObservableCollection<BankItem>? banks;
        public ObservableCollection<BankItem>? Banks
        {
            get { return banks; }
            set
            {
                banks = value;
                NotifyPropertyChanged("Banks");
            }
        }

        private BankItem? selectedCustomerBank;
        public BankItem? SelectedCustomerBank
        {
            get { return selectedCustomerBank; }
            set
            {
                selectedCustomerBank = value;
                NotifyPropertyChanged("SelectedCustomerBank");
            }
        }

        public string? CustomerDescription { get; set; }
        public string? CustomerBankDescription { get; set; }
        public string? StatusDescription => CommonsService.WalletStatus.Where(w => w.ID == N6STATO).FirstOrDefault()?.Description;
        public string? StatusDescriptionUfp
        {
            get
            {
                string? retValue = null;

                if (N6ESTR == "N")
                    retValue = "Non estratto";
                if (N6ESTR == "S" && N6COLL == "N")
                    retValue = "Estratto non contabilizzato";
                if (N6ESTR == "S" && N6COLL == "S")
                    retValue = "Contabilizzato";

                return retValue;
            }
        }
        public string? BankDescription { get; set; }
        public DateTime? DistinctDate { get; set; }

        public long? N6NUDI { get; set; }
        public DateTime? N6DADI { get; set; }
        public string? N6ESTR { get; set; }
        public string? N6COLL { get; set; }

        public int? N6AABI { get; set; }
        public int? N6ACAB { get; set; }
    }
}
