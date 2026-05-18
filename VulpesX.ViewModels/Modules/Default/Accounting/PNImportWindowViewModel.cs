using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Accounting;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class PNImportWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public PNImportWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
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

        public string? FileName { get; set; }

        public List<ImportPNModel>? ImportPNS { get; set; }

        public async Task<bool> ImportFile()
        {
            IsBusy = true;

            try
            {
                var wbook = new XLWorkbook(FileName);
                var wsheet = wbook.Worksheet(1);

                var result = await Task.Run(() =>
                {
                    var list = new List<ImportPNModel>();

                    foreach (var row in wsheet.RowsUsed().Skip(1))
                    {
                        var import = new ImportPNModel { SocietaID = CompanyID };

                        short anno = 0;
                        bool annoResult = short.TryParse(row.Cell(1).Value.ToString(), out anno);
                        if (annoResult)
                            import.Anno = anno;
                        else
                            import.Anno = 0;

                        int id = 0;
                        bool idResult = int.TryParse(row.Cell(2).Value.ToString(), out id);
                        if (idResult)
                            import.ID = id;
                        else
                            import.ID = 0;

                        import.CausaleID = row.Cell(3).Value.ToString();

                        DateTime data = DateTime.MinValue;
                        bool dataResult = DateTime.TryParse(row.Cell(4).Value.ToString(), out data);
                        if (dataResult)
                            import.Data = data;
                        else
                            import.Data = null;

                        import.DocumentoID = row.Cell(5).Value.ToString();
                        import.RiferimentoID = row.Cell(6).Value.ToString();

                        DateTime documentoData = DateTime.MinValue;
                        bool documentoDataResult = DateTime.TryParse(row.Cell(7).Value.ToString(), out documentoData);
                        if (documentoDataResult)
                            import.DocumentoData = documentoData;
                        else
                            import.DocumentoData = null;

                        DateTime riferimentoData = DateTime.MinValue;
                        bool riferimentoDataResult = DateTime.TryParse(row.Cell(8).Value.ToString(), out riferimentoData);
                        if (riferimentoDataResult)
                            import.RiferimentoData = riferimentoData;
                        else
                            import.RiferimentoData = null;

                        import.GruppoID = row.Cell(9).Value.ToString().Trim();
                        import.ContoID = row.Cell(10).Value.ToString().Trim();
                        import.SottocontoID = row.Cell(11).Value.ToString().Trim();
                        import.Note = row.Cell(12).Value.ToString();
                        import.Segno = row.Cell(13).Value.ToString();

                        decimal importo = 0;
                        bool importoResult = decimal.TryParse(row.Cell(14).Value.ToString(), out importo);
                        if (importoResult)
                            import.Importo = Math.Round(importo, 2);
                        else
                            import.Importo = null;

                        import.Valuta = row.Cell(15).Value.ToString();

                        list.Add(import);
                    }
                    return new { list };
                });

                ImportPNS = result.list;
                return true;
            }
            finally
            {
                IsBusy = false;

            }
        }

        public List<Tuple<bool, string>> Validate()
        {
            var retValue = new List<Tuple<bool, string>>();
            int currentRow = 0;

            currentRow = 0;

            foreach (var pn in ImportPNS ?? new List<ImportPNModel>())
            {
                ++currentRow;

                //ANNO
                if (pn.Anno == 0)
                    retValue.Add(new Tuple<bool, string>(false, $"Riga {currentRow} - Anno non compilato"));

                //NUMERO
                if (pn.ID == 0)
                    retValue.Add(new Tuple<bool, string>(false, $"Riga {currentRow} - Numero non compilato"));

                //DATA
                if (pn.Data == null)
                    retValue.Add(new Tuple<bool, string>(false, $"Riga {currentRow} - Data non compilata"));

                //CAUSALE
                if (string.IsNullOrEmpty(pn.CausaleID))
                {
                    retValue.Add(new Tuple<bool, string>(false, $"Riga {currentRow} - Causale non compilata"));
                }
                else
                {
                    var causal = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().Get(pn.CausaleID);

                    if (causal == null)
                    {
                        retValue.Add(new Tuple<bool, string>(false, $"Riga {currentRow} - Causale non presente"));
                    }
                    else
                    {
                        var causalCorrect = causal.caugen == "S" && causal.cauiva == "N" && causal.caucli == "N" && causal.caufor == "N" && causal.cauter == "N" && causal.cauint == "N";

                        if (!causalCorrect)
                        {
                            retValue.Add(new Tuple<bool, string>(false, $"Riga {currentRow} - Causale non di tipo solo Generale"));
                        }
                    }
                }

                //GRUPPO CONTO SOTTOCONTO
                if (string.IsNullOrEmpty(pn.GruppoID) || string.IsNullOrEmpty(pn.ContoID) || string.IsNullOrEmpty(pn.SottocontoID))
                {
                    retValue.Add(new Tuple<bool, string>(false, $"Riga {currentRow} - Gruppo | Conto | Sottoconto non compilato"));
                }
                else
                {
                    var pdc = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Get(pn.GruppoID, pn.ContoID, pn.SottocontoID, CompanyID);

                    if (pdc == null)
                    {
                        retValue.Add(new Tuple<bool, string>(false, $"Riga {currentRow} - Gruppo | Conto | Sottoconto non presente"));
                    }
                }

                //SEGNO
                if (string.IsNullOrEmpty(pn.Segno))
                    retValue.Add(new Tuple<bool, string>(false, $"Riga {currentRow} - Segno non compilato"));
                else
                {
                    if (pn.Segno != "+" && pn.Segno != "-")
                        retValue.Add(new Tuple<bool, string>(false, $"Riga {currentRow} - Segno non valido - Accettati solo '+' o '-'"));
                }

                //IMPORTO
                if (!pn.Importo.HasValue)
                    retValue.Add(new Tuple<bool, string>(false, $"Riga {currentRow} - Importo non compilato"));

                //VALUTA
                if (string.IsNullOrEmpty(pn.Valuta))
                    retValue.Add(new Tuple<bool, string>(false, $"Riga {currentRow} - Valuta non compilata"));
                else
                {
                    if (pn.Valuta.ToUpper() != "EUR" && pn.Valuta.ToUpper() != "USD" && pn.Valuta.ToUpper() != "EURO" && pn.Valuta.ToUpper() != "DOLLAR")
                        retValue.Add(new Tuple<bool, string>(false, $"Riga {currentRow} - Valuta non valida - Accettati solo 'EUR' o 'USD' o 'euro' o 'dollar'"));
                }

                if (pn.Note?.TrimEnd().Length > 50)
                    retValue.Add(new Tuple<bool, string>(false, $"Riga {currentRow} - Note troppo lunghe MAX(50 caratteri)"));
            }

            foreach (var pn in (ImportPNS ?? new List<ImportPNModel>()).GroupBy(g => new { g.SocietaID, g.Anno }))
            {
                var esercizio = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(pn.Key.SocietaID, pn.Key.Anno);

                var enabled = esercizio != null && esercizio.eseest != "C" && esercizio.eseist != "C";

                if(!enabled)
                {
                    retValue.Add(new Tuple<bool, string>(false, $"Esercizio non abilitato {pn.Key.Anno}|{pn.Key.SocietaID}"));
                }
            }

            foreach (var pn in (ImportPNS ?? new List<ImportPNModel>()).GroupBy(g => new { g.SocietaID, g.Anno, g.ID }))
            {
                var dare = pn.Where(s => s.Segno == "+").Sum(s => s.Importo) ?? 0;
                var avere = pn.Where(s => s.Segno == "-").Sum(s => s.Importo) ?? 0;

                if (dare - avere != 0)
                {
                    retValue.Add(new Tuple<bool, string>(false, $"Registrazione {pn.Key.Anno}|{pn.Key.ID} - Non bilancia - DARE {dare.ToString("N2")} AVERE {avere.ToString()})"));
                }
            }

            return retValue;
        }

        public async Task<string?> Import()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    var import = VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().Import(ImportPNS ?? new List<ImportPNModel>());
                    return new { import };
                });

                return result.import;
            }
            finally
            {
                IsBusy = false;

            }
        }

    }
}
