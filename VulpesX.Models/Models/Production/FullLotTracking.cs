using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Production
{
    public class FullLotTracking
    {
        // starting lot info
        public pro_ordine_lotti? Lot { get; set; }
        // linked lots
        public List<pro_ordine_lotti>? LinkedLots { get; set; }

        // PRODUCTION INFO
        // production order
        public pro_ordine? ProductionOrder { get; set; }
        // production order history
        public List<pro_ordine_history>? ProductionHistory { get; set; }
        // produciton times
        public List<pro_ordine_composizione_tempo>? ProductionTimes { get; set; }

        // STORE INFO
        // engages
        public List<store_stocks_engage>? Engages { get; set; }
        // lot
        public List<store_stocks_lots>? LotInfo { get; set; }
    }
}
