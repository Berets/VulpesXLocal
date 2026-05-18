using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models
{
    public interface ISendEmailLogs
    {
        string company_id { get; set; }
        DateTime istant { get; set; }
        string document_type { get; set; }
        string? client_name { get; set; }
        DateTime? client_time { get; set; }
        string? sent_to { get; set; }
        string? sent_cc { get; set; }
        string? sent_object { get; set; }
        string? sent_from { get; set; }
        string? sent_attachments { get; set; }
        string? sendUserID { get; set; }
        string? result { get; set; }
    }
}
