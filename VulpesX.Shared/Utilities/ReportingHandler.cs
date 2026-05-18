using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public static class ReportingHandler
    {
        public static Func<string, string, string[], string, object, string, string, bool, bool, string?>? PrintBookPDFAction { get; set; }

        public static string? PrintBookPDF(string Domain, string Module, string[] ReportTypes, string CompanyID, object ReportDataSource, string DocumentTitle, string Filename, bool OpenReport = false, bool PrintTempWatermark = true)
            => PrintBookPDFAction?.Invoke(Domain, Module, ReportTypes, CompanyID, ReportDataSource, DocumentTitle, Filename, OpenReport, PrintTempWatermark) ?? null;

        public static Func<string, byte[], string, string, bool>? PrintInvoiceXMLAction { get; set; }

        public static bool PrintInvoiceXML(string Filename, byte[] XMLData, string XSLType, string Output)
            => PrintInvoiceXMLAction?.Invoke(Filename, XMLData, XSLType, Output) ?? false;

        public static Func<string, string, string, string, object, string, string, bool, List<SubReportInfo>?, ReportResult?>? PrintPDFAction { get; set; }

        public static ReportResult? PrintPDF(string Domain, string Module, string ReportType, string CompanyID, object ReportDataSource, string DocumentTitle, string Filename, bool OpenReport = false, List<SubReportInfo>? SubreportTypes = null)
            => PrintPDFAction?.Invoke(Domain, Module, ReportType, CompanyID, ReportDataSource, DocumentTitle, Filename, OpenReport, SubreportTypes) ?? null;

        public class ReportResult
        {
            public string? FullPath { get; set; }
            public int PrintedPages { get; set; }
        }

        public class SubReportInfo
        {
            public string? Name { get; set; }
            public string? InternalName { get; set; }
            public object? Datasource { get; set; }
        }
    }
}
