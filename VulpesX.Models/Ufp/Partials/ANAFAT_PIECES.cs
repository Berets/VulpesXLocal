using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class ANAFAT_PIECES : Base
    {
        private decimal? productionCostPresidiato;
        public decimal? ProductionCostPresidiato
        {
            get
            {
                return productionCostPresidiato;
            }
            set
            {
                productionCostPresidiato = value; 
                NotifyPropertyChanged();
            }
        }

        private decimal? productionCostAuto;
        public decimal? ProductionCostAuto
        {
            get
            {
                return productionCostAuto;
            }
            set
            {
                productionCostAuto = value;
                NotifyPropertyChanged();
            }
        }

        private decimal? netCostPresidiato;
        public decimal? NetCostPresidiato
        {
            get
            {
                return netCostPresidiato;
            }
            set
            {
                netCostPresidiato = value;
                NotifyPropertyChanged();
            }
        }

        private decimal? netCostAuto;
        public decimal? NetCostAuto
        {
            get
            {
                return netCostAuto;
            }
            set
            {
                netCostAuto = value;
                NotifyPropertyChanged();
            }
        }

        private decimal? complexityPresidiato;
        public decimal? ComplexityPresidiato
        {
            get
            {
                return complexityPresidiato;
            }
            set
            {
                complexityPresidiato = value;
                NotifyPropertyChanged();
            }
        }

        private decimal? complexityAuto;
        public decimal? ComplexityAuto
        {
            get
            {
                return complexityAuto;
            }
            set
            {
                complexityAuto = value;
                NotifyPropertyChanged();
            }
        }

        private decimal? agentPresidiato;
        public decimal? AgentPresidiato
        {
            get
            {
                return agentPresidiato;
            }
            set
            {
                agentPresidiato = value;
                NotifyPropertyChanged();
            }
        }

        private decimal? agentAuto;
        public decimal? AgentAuto
        {
            get
            {
                return agentAuto;
            }
            set
            {
                agentAuto = value;
                NotifyPropertyChanged();
            }
        }

        private decimal? customerPresidiato;
        public decimal? CustomerPresidiato
        {
            get
            {
                return customerPresidiato;
            }
            set
            {
                customerPresidiato = value;
                NotifyPropertyChanged();
            }
        }

        private decimal? customerAuto;
        public decimal? CustomerAuto
        {
            get
            {
                return customerAuto;
            }
            set
            {
                customerAuto = value;
                NotifyPropertyChanged();
            }
        }

        private decimal? totalPresidiato;
        public decimal? TotalPresidiato
        {
            get
            {
                return totalPresidiato;
            }
            set
            {
                totalPresidiato = value;
                NotifyPropertyChanged();
            }
        }

        private decimal? totalAuto;
        public decimal? TotalAuto
        {
            get
            {
                return totalAuto;
            }
            set
            {
                totalAuto = value;
                NotifyPropertyChanged();
            }
        }

        private decimal? totalPricePresidiato;
        public decimal? TotalPricePresidiato
        {
            get
            {
                return totalPricePresidiato;
            }
            set
            {
                totalPricePresidiato = value;
                NotifyPropertyChanged();
            }
        }

        private decimal? totalPriceAuto;
        public decimal? TotalPriceAuto
        {
            get
            {
                return totalPriceAuto;
            }
            set
            {
                totalPriceAuto = value;
                NotifyPropertyChanged();
            }
        }

        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updateUserID) ? updateUserID : "---";
        public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
        #endregion
    }
}
