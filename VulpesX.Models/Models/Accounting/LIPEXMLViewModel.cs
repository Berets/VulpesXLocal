using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.Accounting
{
    public class LIPEXMLViewModel : Base
    {
        public required string CompanyID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string? LIPEType { get; set; }
        public string? VATID { get; set; }
        public string? FISCALID { get; set; }
        public string? FiscalIDSender { get; set; }
        public ObservableCollection<GenericIDDescription>? Titles { get; set; }
        public string? TitleID { get; set; }
        // broker 
        public string? BrokerFiscalID { get; set; }
        public ObservableCollection<GenericIDDescription>? Presentations { get; set; }
        public string? PresentationID { get; set; }
    }
}
