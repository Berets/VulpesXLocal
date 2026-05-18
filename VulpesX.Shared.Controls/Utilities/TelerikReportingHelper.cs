using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Xsl;
using Telerik.Reporting;
using Telerik.Windows.Documents.Flow.Model;
using VulpesX.Models.Models;
using VulpesX.Models.Models.Reports;
using VulpesX.Shared.Utilities;
using static VulpesX.Shared.Utilities.ReportingHandler;
using Report = Telerik.Reporting.Report;

namespace VulpesX.Shared.Controls.Utilities
{
    public class TelerikReportingHelper
    {
        public static ReportResult? PrintPDF(string Domain, string Module, string ReportType, string CompanyID, object ReportDataSource, string DocumentTitle, string Filename, bool OpenReport = false, List<SubReportInfo>? SubreportTypes = null)
        {
            try
            {
                // device info
                var deviceInfo = new Hashtable();
                deviceInfo.Add("ComplianceLevel", "PDF/A-3b");
                deviceInfo.Add("DocumentTitle", DocumentTitle);
                deviceInfo.Add("DocumentAuthor", Constants.APP_NAME);
                deviceInfo.Add("DocumentProducer", Constants.APP_NAME);
                deviceInfo.Add("DocumentCreator", Constants.APP_NAME);
                // report processor
                var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();

                // main report
                Report? report = null;
                var reportPackager = new ReportPackager();
                report = (Report)reportPackager.UnpackageDocument(RetrieveCorrectReportStream(Domain, Module, ReportType, CompanyID));
                var FullPath = FileHelper.GeneratePDFFilename(Filename.Replace("/", "_").Replace("\\", "_"));
                report.DocumentName = Filename;
                report.DataSource = ReportDataSource;
                var reportSource = new InstanceReportSource();
                reportSource.ReportDocument = report;
                // possible subreports
                if (SubreportTypes != null)
                {
                    // retrieve main report subreport
                    foreach (var rep in SubreportTypes)
                    {
                        SubReport? subReportSource = report.Items.Find(rep.InternalName, true)[0] as SubReport;

                        if (subReportSource != null)
                        {
                            Report? subReport = null;

                            var subReportPackager = new ReportPackager();
                            subReport = (Report?)subReportPackager.UnpackageDocument(RetrieveCorrectReportStream(Domain, Module, rep.Name ?? string.Empty, CompanyID));

                            if (subReport != null)
                            {
                                subReport.DocumentName = Filename;
                                subReport.DataSource = rep.Datasource;

                                var subInstanceReportSource = new InstanceReportSource();
                                subInstanceReportSource.ReportDocument = subReport;
                                subReportSource.ReportSource = subInstanceReportSource;
                            }
                        }
                    }
                }

                var result = reportProcessor.RenderReport("PDF", reportSource, deviceInfo);
                if (!result.HasErrors)
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(FullPath, System.IO.FileMode.Create))
                    {
                        fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
                    }
                    if (OpenReport)
                    {
                        var proc = new ProcessStartInfo(FullPath);
                        proc.UseShellExecute = true;
                        Process.Start(proc);
                    }
                    return new ReportResult() { FullPath = FullPath, PrintedPages = result.PageCount };
                }
                else
                {
                    var sb = new StringBuilder();
                    foreach (var err in result.Errors)
                    {
                        sb.Append(err.Message);
                        sb.Append("\n");
                    }
                    ErrorHandler.Show(sb.ToString());
                }
                return null;
            }
            catch (Exception exc)
            {
                ErrorHandler.Show(exc.Message);
                return null;
            }
            finally
            { Mouse.OverrideCursor = null; }
        }

        public static string? PrintBookPDF(string Domain, string Module, string[] ReportTypes, string CompanyID, object ReportDataSource, string DocumentTitle, string Filename, bool OpenReport = false, bool PrintTempWatermark = true)
        {
            try
            {
                var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
                var reportBook = new ReportBook();
                var FullPath = FileHelper.GeneratePDFFilename(Filename);
                #region Watermark
                Telerik.Reporting.Drawing.TextWatermark tempWatermark = new Telerik.Reporting.Drawing.TextWatermark();
                tempWatermark.Color = System.Drawing.Color.Red;
                tempWatermark.Font.Bold = false;
                tempWatermark.Font.Italic = false;
                tempWatermark.Font.Name = "Arial";
                tempWatermark.Font.Size = Telerik.Reporting.Drawing.Unit.Point(200D);
                tempWatermark.Font.Strikeout = false;
                tempWatermark.Font.Underline = false;
                tempWatermark.Orientation = Telerik.Reporting.Drawing.WatermarkOrientation.Diagonal;
                tempWatermark.Position = Telerik.Reporting.Drawing.WatermarkPosition.Front;
                tempWatermark.PrintOnFirstPage = true;
                tempWatermark.PrintOnLastPage = true;
                tempWatermark.Text = "Bozza";
                tempWatermark.Opacity = 0.5D;
                #endregion

                foreach (var rep in ReportTypes)
                {
                    Report? report = null;
                    var reportPackager = new ReportPackager();
                    report = (Report?)reportPackager.UnpackageDocument(RetrieveCorrectReportStream(Domain, Module, rep, CompanyID));

                    if (report != null)
                    {
                        report.DocumentName = Filename;
                        report.DataSource = ReportDataSource;
                        if (PrintTempWatermark)
                        {
                            report.PageSettings.Watermarks.Add(tempWatermark);
                        }
                        var reportSource = new InstanceReportSource();
                        reportSource.ReportDocument = report;
                        reportBook.ReportSources.Add(reportSource);
                    }
                }

                var deviceInfo = new Hashtable();
                deviceInfo.Add("ComplianceLevel", "PDF/A-3b");
                deviceInfo.Add("DocumentTitle", DocumentTitle);
                deviceInfo.Add("DocumentAuthor", Constants.APP_NAME);
                deviceInfo.Add("DocumentProducer", Constants.APP_NAME);
                deviceInfo.Add("DocumentCreator", Constants.APP_NAME);

                var irs = new InstanceReportSource();
                irs.ReportDocument = reportBook;

                var result = reportProcessor.RenderReport("PDF", irs, deviceInfo);

                if (!result.HasErrors)
                {
                    using (System.IO.FileStream fs = new System.IO.FileStream(FullPath, System.IO.FileMode.Create))
                    {
                        fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
                    }
                    if (OpenReport)
                    {
                        var proc = new ProcessStartInfo(FullPath);
                        proc.UseShellExecute = true;
                        Process.Start(proc);
                    }
                    return FullPath;
                }
                else
                {
                    var sb = new StringBuilder();
                    foreach (var err in result.Errors)
                    {
                        sb.Append(err.Message);
                        sb.Append("\n");
                    }
                    ErrorHandler.Show(sb.ToString());
                }
                return null;
            }
            catch (Exception exc)
            {
                ErrorHandler.Show(exc.Message);
                return null;
            }
            finally
            { Mouse.OverrideCursor = null; }
        }

        public static bool PrintInvoiceXML(string Filename, byte[] XMLData, string XSLType, string Output)
        {
            try
            {
                var xsl = GetResourceNames()?.Where(w => (string)w.Key == $"reports/stylesheets/{XSLType}".ToLower()).First().Value as Stream;
                var myXslTrans = new XslCompiledTransform();

                if (xsl != null)
                    myXslTrans.Load(XmlReader.Create(xsl));

                var sw = new StringWriter();
                var xmlW = XmlWriter.Create(sw);

                var data = XMLHelper.GetDocumentFromBytes(XMLData);
                if (data != null)
                    myXslTrans.Transform(data, xmlW);

                string? FullPath = null;
                var htmlText = sw.ToString();

                if (Output == "XML")
                {
                    FullPath = FileHelper.GeneratePDFFilename($"{Filename}.pdf".Replace("/", "_").Replace("\\", "_")).Replace(".pdf", ".html");
                    var xmlBytes = Encoding.UTF8.GetBytes(htmlText);
                    using (System.IO.FileStream fs = new System.IO.FileStream(FullPath, System.IO.FileMode.Create))
                    {
                        fs.Write(xmlBytes, 0, xmlBytes.Length);
                    }
                }
                else if (Output == "PDF")
                {
                    // sanitize html
                    htmlText = htmlText.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?><html xmlns:a=\"http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2\">", "<!DOCTYPE html>\r\n<html lang=\"it\">")
                                        .Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?><html xmlns:b=\"http://www.fatturapa.gov.it/sdi/fatturapa/v1.1\" xmlns:c=\"http://www.fatturapa.gov.it/sdi/fatturapa/v1.0\" xmlns:a=\"http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2\" xmlns:d=\"http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.0\">", "<!DOCTYPE html>\r\n<html lang=\"it\">")
                                        .Replace("margin-left: auto", "margin-left: 0px !important")
                                        .Replace("margin-right: auto", "margin-right: 0px !important")
                                        .Replace("max-width: 1280px;", null)
                                        .Replace("min-width: 800px;", null);
                    Telerik.Windows.Documents.Flow.FormatProviders.Html.HtmlFormatProvider htmlProvider = new Telerik.Windows.Documents.Flow.FormatProviders.Html.HtmlFormatProvider();
                    // Create a document instance from the content. 
                    RadFlowDocument document = htmlProvider.Import(htmlText, new TimeSpan(0, 30, 0));
                    document.Document.Sections[0].PageMargins = new Telerik.Windows.Documents.Primitives.Padding(5);
                    document.Document.Sections[0].HeaderTopMargin = 5;
                    document.Document.Sections[0].FooterBottomMargin = 5;
                    var x = document.Document.Sections[0].Footers;
                    Telerik.Windows.Documents.Flow.FormatProviders.Pdf.PdfFormatProvider pdfProvider = new Telerik.Windows.Documents.Flow.FormatProviders.Pdf.PdfFormatProvider();
                    pdfProvider.ExportSettings.ComplianceLevel = Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.Export.PdfComplianceLevel.PdfA3B;
                    // Export the document. The different overloads enables you to export to a byte[] or to a Stream. 
                    byte[] pdfBytes = pdfProvider.Export(document, new TimeSpan(0, 30, 0));
                    FullPath = FileHelper.GeneratePDFFilename($"{Filename}.pdf".Replace("/", "_").Replace("\\", "_"));
                    using (System.IO.FileStream fs = new System.IO.FileStream(FullPath, System.IO.FileMode.Create))
                    {
                        fs.Write(pdfBytes, 0, pdfBytes.Length);
                    }
                }

                if (!string.IsNullOrEmpty(FullPath))
                {
                    var proc = new ProcessStartInfo(FullPath);
                    proc.UseShellExecute = true;
                    Process.Start(proc);
                }

                return true;
            }
            catch (Exception exc)
            {
                ErrorHandler.Show(exc.Message);
                return false;
            }
        }

        #region Private methods
        internal static Stream? RetrieveCorrectReportStream(string Domain, string Module, string ReportType, string CompanyID)
        {
            try
            {
                if (Domain.StartsWith("@"))
                    Domain = Domain.Replace("@", "");

                var resources = GetResourceNames();
                string deeper = $"Reports/{Module}/{ReportType}.{Domain}.{CompanyID}.trdp".ToLower();
                string deep = $"Reports/{Module}/{ReportType}.{Domain}.trdp".ToLower();
                string standard = $"Reports/{Module}/{ReportType}.trdp".ToLower();

                if (resources != null)
                {
                    if (resources.Where(w => (string)w.Key == deeper).Any())
                    {
                        return resources.Where(w => (string)w.Key == deeper).First().Value as Stream;
                    }
                    else
                    {
                        if (resources.Where(w => (string)w.Key == deep).Any())
                            return resources.Where(w => (string)w.Key == deep).First().Value as Stream;
                        else
                            return resources.Where(w => (string)w.Key == standard).First().Value as Stream;
                    }
                }

                return null;
            }
            catch
            { return null; }
        }

        private static List<DictionaryEntry>? GetResourceNames()
        {
            var assembly = typeof(TelerikReportingHelper).Assembly;

            using (var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".g.resources"))
            {
                if (stream != null)
                {
                    using (var reader = new System.Resources.ResourceReader(stream))
                    {
                        return reader.Cast<DictionaryEntry>()
                            .ToList();
                    }
                }
                return null;
            }
        }

        private static bool _CheckFileInUse(string FullPath)
        {
            try
            {
                using (Stream stream = new FileStream(FullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }
        #endregion

        #region Report helpers
        public static Tuple<string, string>? GetSingleExpire(List<Tuple<string, string>> Data, int Index)
        {
            if (Data == null)
                return null;

            if (Index < Data.Count)
                return Data[Index];
            else
                return null;
        }
        #endregion




    }
}
