using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Default;

namespace VulpesX.DAL.Accounting
{
    public interface IMPTESTATARepository
    {
        ObservableCollection<MPTESTATA>? GetList(string CompanyID, int Year);

        MPTESTATA? Get(string CompanyID, int Year, int ID);

        bool Exists(string CompanyID, int Year, int ID);

        string? GenerateXML(string CompanyID, int Year, int ID);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }

        string? Insert(MPTESTATA Model, ObservableCollection<MPDETTAGLIO> Rows);

        bool Update(MPTESTATA Model);

        string? Update(MPTESTATA Model, ObservableCollection<MPDETTAGLIO> Rows);

        bool CanDelete(string CompanyID, int Year, int ID);

        bool Delete(MPTESTATA Model);

        string? Validate(MPTESTATA Model, bool IsInsert);
        #endregion
    }

    public class MPTESTATARepository : RepositoryBase, IMPTESTATARepository
    {
        public MPTESTATARepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<MPTESTATA>? GetList(string CompanyID, int Year)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<MPTESTATA>(
                    @"SELECT t.* ,
CONCAT(TRIM(bb.abiban),' ' , TRIM(bb.abiage)) AS BankDescription,
mm.mandes as TypeDescription
FROM MPTESTATA as t
LEFT OUTER JOIN ABICAB AS bb ON bb.abiabi = t.MPABI AND bb.abicab = t.MPCAB
LEFT OUTER JOIN MANDATO AS mm ON mm.mancod = t.MPTIPO
WHERE t.MPSOCI = @cid AND t.MPANNO = @year ORDER BY t.MPNUME DESC",
                    new { cid = CompanyID, year = Year });

                return new ObservableCollection<MPTESTATA>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public MPTESTATA? Get(string CompanyID, int Year, int ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<MPTESTATA>(
                    @"SELECT * FROM MPTESTATA WHERE MPSOCI = @cid AND MPANNO = @year AND MPNUME = @id",
                    new { cid = CompanyID, year = Year, id = ID })
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string CompanyID, int Year, int ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM MPTESTATA WHERE MPSOCI=@cid AND MPANNO = @year AND MPNUME = @id",
                    new { cid = CompanyID, year = Year, id = ID }) > 0;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public string? GenerateXML(string CompanyID, int Year, int ID)
        {
            try
            {
                CultureInfo culture = new CultureInfo("en-US");

                string fileName = $"{Year}{ID}";

                var azienda = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);


                if (azienda == null)
                {
                    ErrorHandler.Validation("Azienda non trovata");
                    return null;
                }

                var head = Get(CompanyID, Year, ID);
                var rows = VulpesServiceProvider.Provider.GetRequiredService<IMPDETTAGLIORepository>().GetList(CompanyID, Year, ID);

                if (head == null || rows == null)
                {
                    ErrorHandler.Validation("Mandato di pagamento non trovato");
                    return null;
                }

                var banca = VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Get(CompanyID, head.MPABI!.Value, head.MPCAB!.Value, head.MPCCOR!);

                if (banca == null)
                {
                    ErrorHandler.Validation("Bancan non trovata");
                    return null;
                }

                string fullPath = $"{Path.GetTempPath()}{fileName}.xml";

                XNamespace ns = "urn:CBI:xsd:CBIPaymentRequest.00.04.01";

                // Totale e numero transazioni
                decimal totale = (rows.Where(o => o.M3SEGN == "A").Sum(s => s.M3IMEU) ?? 0) - (rows.Where(o => o.M3SEGN == "D").Sum(s => s.M3IMEU) ?? 0);
                int numeroTx = rows.GroupBy(g => g.M3SOTT).Count();

                // Creazione dei bonifici
                var cdtTrfList = new List<XElement>();
                int index = 0;
                foreach (var m in rows.GroupBy(g => g.M3SOTT))
                {
                    ++index;

                    var supplier = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(m.Key!.Value);
                    var supplierData = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(CompanyID, m.Key!.Value);

                    if (supplier == null || supplierData == null)
                    {
                        ErrorHandler.Validation($"Non trovati dati del fornitore - {m.Key}");
                        return null;
                    }

                    var import = m.Where(o => o.M3SEGN == "A").Sum(s => s.M3IMEU) - m.Where(o => o.M3SEGN == "D").Sum(s => s.M3IMEU);
                    var invoices = string.Join(",", m.Select(s => s.M3RIFE?.Trim()));

                    cdtTrfList.Add(new XElement(ns + "CdtTrfTxInf",
                        new XElement(ns + "PmtId",
                            new XElement(ns + "InstrId", index),
                            new XElement(ns + "EndToEndId", $"{fileName}-{index}")
                        ),
                        new XElement(ns + "PmtTpInf",
                            new XElement(ns + "CtgyPurp",
                               new XElement(ns + "Cd", "SUPP")
                            )
                        ),
                        new XElement(ns + "Amt",
                            new XElement(ns + "InstdAmt",
                                new XAttribute("Ccy", "EUR"),
                                import?.ToString("0.00", CultureInfo.InvariantCulture)
                            )
                        ),
                        new XElement(ns + "Cdtr",
                            new XElement(ns + "Nm", supplier.abers1?.TrimEnd()),
                            !string.IsNullOrEmpty(supplierData.FOBIC?.TrimEnd())
                            ? new XElement(ns + "Id",
                                new XElement(ns + "OrgId",
                                    new XElement(ns + "AnyBIC", supplierData.FOBIC.TrimEnd())
                                )
                              )
                            : null
                        ),
                        new XElement(ns + "CdtrAcct",
                            new XElement(ns + "Id",
                                new XElement(ns + "IBAN", supplierData.FOIBAN?.TrimEnd())
                            )
                        ),
                        new XElement(ns + "RmtInf",
                            new XElement(ns + "Ustrd", invoices.Length > 140 ? invoices.Substring(0, 140) : invoices)
                        )

                        ));
                }

                // Creazione del PmtInf
                var pmtInf = new XElement(ns + "PmtInf",
                    new XElement(ns + "PmtInfId", fileName),
                    new XElement(ns + "PmtMtd", "TRF"),
                    new XElement(ns + "BtchBookg", true),
                    new XElement(ns + "PmtTpInf",
                        new XElement(ns + "InstrPrty", "NORM"),
                        new XElement(ns + "SvcLvl",
                            new XElement(ns + "Cd", "SEPA")
                        )
                    ),
                    new XElement(ns + "ReqdExctnDt",
                        new XElement(ns + "Dt", DateTime.Now.ToString("yyyy-MM-dd"))
                    ),
                    // Dati debitore (esempio)
                    new XElement(ns + "Dbtr",
                        new XElement(ns + "Nm", azienda.azrssl?.TrimEnd()),
                        new XElement(ns + "Id",
                            new XElement(ns + "OrgId",
                                new XElement(ns + "AnyBIC", head.MPBIC?.TrimEnd())
                            )
                        )
                    ),
                    new XElement(ns + "DbtrAcct",
                        new XElement(ns + "Id",
                            new XElement(ns + "IBAN", head.MPIBAN?.TrimEnd())
                        )
                    ),
                    new XElement(ns + "DbtrAgt",
                        new XElement(ns + "FinInstnId",
                            new XElement(ns + "ClrSysMmbId",
                                new XElement(ns + "MmbId", head.MPABI?.ToString("D5"))
                            )
                        )
                    ),
                    new XElement(ns + "ChrgBr", "SLEV"),
                    cdtTrfList
                );

                // Creazione del GrpHdr
                var grpHdr = new XElement(ns + "GrpHdr",
                    new XElement(ns + "MsgId", $"{fileName}-{DateTime.Now.ToString("yyyyMMdd-HH.ss")}"),
                    new XElement(ns + "CreDtTm", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                    new XElement(ns + "NbOfTxs", numeroTx),
                    new XElement(ns + "CtrlSum", totale.ToString("0.00", CultureInfo.InvariantCulture)),
                    new XElement(ns + "InitgPty",
                        new XElement(ns + "Nm", azienda.azrssl?.TrimEnd()),
                        new XElement(ns + "Id",
                            new XElement(ns + "OrgId",
                                new XElement(ns + "Othr",
                                    new XElement(ns + "Id", banca.abicuc),
                                    new XElement(ns + "Issr", "CBI")
                                )
                            )
                        )
                    )
                );

                // Creazione del documento
                var doc = new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    new XElement(ns + "CBIPaymentRequest",
                        grpHdr,
                        pmtInf
                    )
                );

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Encoding = new System.Text.UTF8Encoding(false), // false per evitare BOM
                    Indent = true
                };

                // Usa XmlWriter per salvare il documento senza BOM
                using (XmlWriter writer = XmlWriter.Create(fullPath, settings))
                {
                    doc.Save(writer);
                }

                return fullPath;
            }
            catch (Exception ex)
            {
                ErrorHandler.Validation(ex.Message.ToString());
                return null;
            }
        }

        #region CRUD
        public string INSERT_QUERY => @"INSERT INTO MPTESTATA (MPSOCI, MPANNO, MPNUME, MPTIPO, MPDATA, MPGRUP, MPCONT, MPSOTT, MPSBAN, MPABI, MPCAB, MPCCOR, MPASSE, MPNRFO, MPIMPO, MPEUIM, MPIMAB, MPEUAB, MPCOVA, MPVALU,  MPVAIM, MPVAAB,      
            MPFLST, MPFLCO, MPFLES, MPBIC, MPCIN, MPIBAN, MPBBAN, MPDVAL, mpdesspese, mpdesc, mpdatfir, mpfirma, mpcamb, mpspese, regianno,reginumero)  OUTPUT INSERTED.rv VALUES ( @MPSOCI, @MPANNO, @MPNUME, @MPTIPO, @MPDATA, @MPGRUP, @MPCONT, @MPSOTT, @MPSBAN,@MPABI,@MPCAB, @MPCCOR, @MPASSE, @MPNRFO, @MPIMPO, @MPEUIM, @MPIMAB, @MPEUAB, @MPCOVA, @MPVALU, @MPVAIM, @MPVAAB, @MPFLST, @MPFLCO, @MPFLES, @MPBIC, @MPCIN, @MPIBAN, @MPBBAN, @MPDVAL, @mpdesspese, @mpdesc, @mpdatfir, @mpfirma, @mpcamb, @mpspese,@regianno,@reginumero)";
        public string UPDATE_QUERY => "UPDATE MPTESTATA SET MPTIPO=@MPTIPO, MPDATA=@MPDATA, MPGRUP=@MPGRUP, MPCONT=@MPCONT, MPSOTT=@MPSOTT, MPSBAN=@MPSBAN, MPABI=@MPABI, MPCAB=@MPCAB, MPCCOR=@MPCCOR, MPASSE=@MPASSE, MPNRFO=@MPNRFO, MPIMPO=@MPIMPO, MPEUIM=@MPEUIM, MPIMAB=@MPIMAB, MPEUAB=@MPEUAB, MPCOVA=@MPCOVA, MPVALU=@MPVALU, MPVAIM=@MPVAIM, MPVAAB=@MPVAAB, MPFLST=@MPFLST, MPFLCO=@MPFLCO, MPFLES=@MPFLES, MPBIC=@MPBIC, MPCIN=@MPCIN, MPIBAN=@MPIBAN, MPBBAN=@MPBBAN, MPDVAL=@MPDVAL, mpdesspese=@mpdesspese, mpdesc=@mpdesc, mpdatfir=@mpdatfir, mpfirma=@mpfirma, mpcamb=@mpcamb, mpspese=@mpspese, regianno=@regianno,reginumero=@reginumero OUTPUT INSERTED.rv  WHERE MPSOCI=@MPSOCI AND MPANNO=@MPANNO AND MPNUME=@MPNUME";
        public string DELETE_QUERY => "DELETE FROM MPTESTATA OUTPUT DELETED.rv WHERE MPSOCI = @MPSOCI AND MPANNO = @MPANNO AND MPNUME = @MPNUME AND rv = @rv";

        public string? Insert(MPTESTATA Model, ObservableCollection<MPDETTAGLIO> Rows)
        {
            try
            {
                using var connection = GetOpenConnection();

                using (var transaction = connection.BeginTransaction())
                {
                    int newID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(Model.MPSOCI, Model.MPANNO, Constants.PAYMENT_SENDER, true);

                    if (newID > 0)
                    {
                        Model.MPNUME = newID;
                        Model.MPCOVA = "UIC";
                        Model.MPVALU = "EUR";
                        Model.MPEUIM = Rows.Where(o => o.M3SEGN == "A").Sum(s => s.M3IMEU) - Rows.Where(o => o.M3SEGN == "D").Sum(s => s.M3IMEU);
                        Model.MPNRFO = Rows.Count;


                        var result = connection.Execute(INSERT_QUERY, Model, transaction);
                        if (result > 0)
                        {
                            foreach (var row in Rows)
                            {
                                row.MPNUME = newID;

                                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IMPDETTAGLIORepository>().INSERT_QUERY, row, transaction);
                            }

                            transaction.Commit();

                            return $"{Model.MPANNO}/{Model.MPNUME}";
                        }
                        else
                        {
                            ErrorHandler.Show(Constants.INSERT_VIOLATION);
                            return null;
                        }
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Update(MPTESTATA Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.ExecuteScalar(UPDATE_QUERY, Model);

                if (result != null)
                {
                    return true;
                }
                else
                {
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public string? Update(MPTESTATA Model, ObservableCollection<MPDETTAGLIO> Rows)
        {
            try
            {
                using var connection = GetOpenConnection();

                using (var transaction = connection.BeginTransaction())
                {
                    connection.Execute("DELETE MPDETTAGLIO WHERE MPSOCI = @cid AND MPANNO = @year AND MPNUME = @id", new { cid = Model.MPSOCI, year = Model.MPANNO, id = Model.MPNUME }, transaction);

                    Model.MPEUIM = Rows.Where(o => o.M3SEGN == "A").Sum(s => s.M3IMEU) - Rows.Where(o => o.M3SEGN == "D").Sum(s => s.M3IMEU);

                    var result = connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);

                    if (result != null)
                    {
                        foreach (var row in Rows)
                        {
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IMPDETTAGLIORepository>().INSERT_QUERY, row, transaction);
                        }

                        transaction.Commit();

                        return $"{Model.MPANNO}/{Model.MPNUME}";
                    }
                    else
                    {
                        ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool CanDelete(string CompanyID, int Year, int ID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var item = connection.Query<MPTESTATA>(
                    @"SELECT * FROM MPTESTATA WHERE MPSOCI = @cid AND MPANNO = @year AND MPNUME = @id",
                    new { cid = CompanyID, year = Year, id = ID })
                    .FirstOrDefault();

                return item != null && (item.MPFLST == "N" || string.IsNullOrEmpty(item.MPFLST));
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool Delete(MPTESTATA Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                using (var transaction = connection.BeginTransaction())
                {
                    var result = connection.Execute(DELETE_QUERY, Model, transaction);
                    if (result > 0)
                    {
                        result = connection.Execute("DELETE MPDETTAGLIO WHERE MPSOCI = @cid AND MPANNO = @year AND MPNUME = @id", new { cid = Model.MPSOCI, year = Model.MPANNO, id = Model.MPNUME }, transaction);

                        transaction.Commit();

                        return true;
                    }
                    else
                    {
                        ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public string? Validate(MPTESTATA Model, bool IsInsert)
        {
            try
            {
                if (Model.MPANNO > 0)
                {
                    if (!IsInsert || (IsInsert && !Exists(Model.MPSOCI, Model.MPANNO, Model.MPNUME)))
                    {
                        return null;
                    }
                    else
                    { return "L'anno inserito è già in uso"; }
                }
                else
                { return "L'anno è obbligatorio"; }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }

    public class MPTESTATAUfpRepository : RepositoryBase, IMPTESTATARepository
    {
        public MPTESTATAUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<MPTESTATA>? GetList(string CompanyID, int Year)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<MPTESTATA>(
                    @"SELECT t.* ,
CONCAT(TRIM(bb.abiban),' ' , TRIM(bb.abiage)) AS BankDescription,
mm.mandes as TypeDescription
FROM MPTESTATA as t
LEFT OUTER JOIN TAB_ABICAB AS bb ON bb.abiabi = t.MPABI AND bb.abicab = t.MPCAB
LEFT OUTER JOIN MANDATO AS mm ON mm.mancod = t.MPTIPO
WHERE t.MPSOCI = @cid AND t.MPANNO = @year ORDER BY t.MPNUME DESC",
                    new { cid = CompanyID, year = Year });

                return new ObservableCollection<MPTESTATA>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public MPTESTATA? Get(string CompanyID, int Year, int ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<MPTESTATA>(
                    @"SELECT * FROM MPTESTATA WHERE MPSOCI = @cid AND MPANNO = @year AND MPNUME = @id",
                    new { cid = CompanyID, year = Year, id = ID })
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string CompanyID, int Year, int ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM MPTESTATA WHERE MPSOCI=@cid AND MPANNO = @year AND MPNUME = @id",
                    new { cid = CompanyID, year = Year, id = ID }) > 0;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public string? GenerateXML(string CompanyID, int Year, int ID)
        {
            try
            {
                CultureInfo culture = new CultureInfo("en-US");

                string fileName = $"{Year}{ID}";

                var azienda = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);


                if (azienda == null)
                {
                    ErrorHandler.Validation("Azienda non trovata");
                    return null;
                }

                var head = Get(CompanyID, Year, ID);
                var rows = VulpesServiceProvider.Provider.GetRequiredService<IMPDETTAGLIORepository>().GetList(CompanyID, Year, ID);

                if (head == null || rows == null)
                {
                    ErrorHandler.Validation("Mandato di pagamento non trovato");
                    return null;
                }

                var banca = VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Get(CompanyID, head.MPABI!.Value, head.MPCAB!.Value, head.MPCCOR!);

                if (banca == null)
                {
                    ErrorHandler.Validation("Bancan non trovata");
                    return null;
                }

                string fullPath = $"{Path.GetTempPath()}{fileName}.xml";

                XNamespace ns = "urn:CBI:xsd:CBIPaymentRequest.00.04.01";

                // Totale e numero transazioni
                decimal totale = (rows.Where(o => o.M3SEGN == "A").Sum(s => s.M3IMEU) ?? 0) - (rows.Where(o => o.M3SEGN == "D").Sum(s => s.M3IMEU) ?? 0);
                int numeroTx = rows.GroupBy(g => g.M3SOTT).Count();

                // Creazione dei bonifici
                var cdtTrfList = new List<XElement>();
                int index = 0;
                foreach (var m in rows.GroupBy(g => g.M3SOTT))
                {
                    ++index;

                    var supplier = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(m.Key!.Value);
                    var supplierData = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(CompanyID, m.Key!.Value);

                    if (supplier == null || supplierData == null)
                    {
                        ErrorHandler.Validation($"Non trovati dati del fornitore - {m.Key}");
                        return null;
                    }

                    var import = m.Where(o => o.M3SEGN == "A").Sum(s => s.M3IMEU) - m.Where(o => o.M3SEGN == "D").Sum(s => s.M3IMEU);
                    var invoices = string.Join(",", m.Select(s => s.M3RIFE?.Trim()));

                    cdtTrfList.Add(new XElement(ns + "CdtTrfTxInf",
                        new XElement(ns + "PmtId",
                            new XElement(ns + "InstrId", index),
                            new XElement(ns + "EndToEndId", $"{fileName}-{index}")
                        ),
                        new XElement(ns + "PmtTpInf",
                            new XElement(ns + "CtgyPurp",
                               new XElement(ns + "Cd", "SUPP")
                            )
                        ),
                        new XElement(ns + "Amt",
                            new XElement(ns + "InstdAmt",
                                new XAttribute("Ccy", "EUR"),
                                import?.ToString("0.00", CultureInfo.InvariantCulture)
                            )
                        ),
                        new XElement(ns + "Cdtr",
                            new XElement(ns + "Nm", supplier.abers1?.TrimEnd()),
                            new XElement(ns + "PstlAdr",
                                new XElement(ns + "Ctry", supplier.nazcod?.TrimEnd()),
                                new XElement(ns + "AdrLine", $"{supplier.abeloc?.TrimEnd()} {supplier.abeind?.TrimEnd()}".Length > 35 ? $"{supplier.abeloc?.TrimEnd()} {supplier.abeind?.TrimEnd()}".Substring(0, 35) : $"{supplier.abeloc?.TrimEnd()} {supplier.abeind?.TrimEnd()}")
                            ),
                            !string.IsNullOrEmpty(supplierData.FOBIC?.TrimEnd())
                            ? new XElement(ns + "Id",
                                new XElement(ns + "OrgId",
                                    new XElement(ns + "AnyBIC", supplierData.FOBIC.TrimEnd())
                                )
                              )
                            : null
                        ),
                        new XElement(ns + "CdtrAcct",
                            new XElement(ns + "Id",
                                new XElement(ns + "IBAN", supplierData.FOIBAN?.TrimEnd())
                            )
                        ),
                        new XElement(ns + "RmtInf",
                            new XElement(ns + "Ustrd", invoices.Length > 140 ? invoices.Substring(0, 140) : invoices)
                        )

                        ));
                }

                // Creazione del PmtInf
                var pmtInf = new XElement(ns + "PmtInf",
                    new XElement(ns + "PmtInfId", fileName),
                    new XElement(ns + "PmtMtd", "TRF"),
                    new XElement(ns + "BtchBookg", true),
                    new XElement(ns + "PmtTpInf",
                        new XElement(ns + "InstrPrty", "NORM"),
                        new XElement(ns + "SvcLvl",
                            new XElement(ns + "Cd", "SEPA")
                        )
                    ),
                    new XElement(ns + "ReqdExctnDt",
                        new XElement(ns + "Dt", DateTime.Now.ToString("yyyy-MM-dd"))
                    ),

                    // Dati debitore 
                    new XElement(ns + "Dbtr",
                        new XElement(ns + "Nm", azienda.azrssl?.TrimEnd()),
                        new XElement(ns + "Id",
                            new XElement(ns + "OrgId",
                                new XElement(ns + "AnyBIC", head.MPBIC?.TrimEnd())
                            )
                        )
                    ),
                    new XElement(ns + "DbtrAcct",
                        new XElement(ns + "Id",
                            new XElement(ns + "IBAN", head.MPIBAN?.TrimEnd())
                        )
                    ),
                    new XElement(ns + "DbtrAgt",
                        new XElement(ns + "FinInstnId",
                            new XElement(ns + "ClrSysMmbId",
                                new XElement(ns + "MmbId", head.MPABI?.ToString("D5"))
                            )
                        )
                    ),
                    new XElement(ns + "ChrgBr", "SLEV"),
                    cdtTrfList
                );

                // Creazione del GrpHdr
                var grpHdr = new XElement(ns + "GrpHdr",
                    new XElement(ns + "MsgId", $"{fileName}-{DateTime.Now.ToString("yyyyMMdd-HH.ss")}"),
                    new XElement(ns + "CreDtTm", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                    new XElement(ns + "NbOfTxs", numeroTx),
                    new XElement(ns + "CtrlSum", totale.ToString("0.00", CultureInfo.InvariantCulture)),
                    new XElement(ns + "InitgPty",
                        new XElement(ns + "Nm", azienda.azrssl?.TrimEnd()),
                        new XElement(ns + "Id",
                            new XElement(ns + "OrgId",
                                new XElement(ns + "Othr",
                                    new XElement(ns + "Id", banca.abicuc),
                                    new XElement(ns + "Issr", "CBI")
                                )
                            )
                        )
                    )
                );

                // Creazione del documento
                var doc = new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    new XElement(ns + "CBIPaymentRequest",
                        grpHdr,
                        pmtInf
                    )
                );

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Encoding = new System.Text.UTF8Encoding(false), // false per evitare BOM
                    Indent = true
                };

                // Usa XmlWriter per salvare il documento senza BOM
                using (XmlWriter writer = XmlWriter.Create(fullPath, settings))
                {
                    doc.Save(writer);
                }

                return fullPath;
            }
            catch (Exception ex)
            {
                ErrorHandler.Validation(ex.Message.ToString());
                return null;
            }
        }

        #region CRUD
        public string INSERT_QUERY => @"INSERT INTO MPTESTATA (MPSOCI, MPANNO, MPNUME, MPTIPO, MPDATA, MPGRUP, MPCONT, MPSOTT, MPSBAN, MPABI, MPCAB, MPCCOR, MPASSE, MPNRFO, MPIMPO, MPEUIM, MPIMAB, MPEUAB, MPCOVA, MPVALU,  MPVAIM, MPVAAB,      
            MPFLST, MPFLCO, MPFLES, MPBIC, MPCIN, MPIBAN, MPBBAN, MPDVAL, mpdesspese, mpdesc, mpdatfir, mpfirma, mpcamb, mpspese, regianno,reginumero)  OUTPUT INSERTED.rv VALUES ( @MPSOCI, @MPANNO, @MPNUME, @MPTIPO, @MPDATA, @MPGRUP, @MPCONT, @MPSOTT, @MPSBAN,@MPABI,@MPCAB, @MPCCOR, @MPASSE, @MPNRFO, @MPIMPO, @MPEUIM, @MPIMAB, @MPEUAB, @MPCOVA, @MPVALU, @MPVAIM, @MPVAAB, @MPFLST, @MPFLCO, @MPFLES, @MPBIC, @MPCIN, @MPIBAN, @MPBBAN, @MPDVAL, @mpdesspese, @mpdesc, @mpdatfir, @mpfirma, @mpcamb, @mpspese,@regianno,@reginumero)";
        public string UPDATE_QUERY => "UPDATE MPTESTATA SET MPTIPO=@MPTIPO, MPDATA=@MPDATA, MPGRUP=@MPGRUP, MPCONT=@MPCONT, MPSOTT=@MPSOTT, MPSBAN=@MPSBAN, MPABI=@MPABI, MPCAB=@MPCAB, MPCCOR=@MPCCOR, MPASSE=@MPASSE, MPNRFO=@MPNRFO, MPIMPO=@MPIMPO, MPEUIM=@MPEUIM, MPIMAB=@MPIMAB, MPEUAB=@MPEUAB, MPCOVA=@MPCOVA, MPVALU=@MPVALU, MPVAIM=@MPVAIM, MPVAAB=@MPVAAB, MPFLST=@MPFLST, MPFLCO=@MPFLCO, MPFLES=@MPFLES, MPBIC=@MPBIC, MPCIN=@MPCIN, MPIBAN=@MPIBAN, MPBBAN=@MPBBAN, MPDVAL=@MPDVAL, mpdesspese=@mpdesspese, mpdesc=@mpdesc, mpdatfir=@mpdatfir, mpfirma=@mpfirma, mpcamb=@mpcamb, mpspese=@mpspese, regianno=@regianno,reginumero=@reginumero OUTPUT INSERTED.rv  WHERE MPSOCI=@MPSOCI AND MPANNO=@MPANNO AND MPNUME=@MPNUME";
        public string DELETE_QUERY => "DELETE FROM MPTESTATA OUTPUT DELETED.rv WHERE MPSOCI = @MPSOCI AND MPANNO = @MPANNO AND MPNUME = @MPNUME AND rv = @rv";

        public string? Insert(MPTESTATA Model, ObservableCollection<MPDETTAGLIO> Rows)
        {
            try
            {
                using var connection = GetOpenConnection();

                using (var transaction = connection.BeginTransaction())
                {
                    int newID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(Model.MPSOCI, Model.MPANNO, Constants.PAYMENT_SENDER, true);

                    if (newID > 0)
                    {
                        Model.MPNUME = newID;
                        Model.MPCOVA = "UIC";
                        Model.MPVALU = "EUR";
                        Model.MPEUIM = Rows.Where(o => o.M3SEGN == "A").Sum(s => s.M3IMEU) - Rows.Where(o => o.M3SEGN == "D").Sum(s => s.M3IMEU);
                        Model.MPNRFO = Rows.Count;


                        var result = connection.Execute(INSERT_QUERY, Model, transaction);
                        if (result > 0)
                        {
                            foreach (var row in Rows)
                            {
                                row.MPNUME = newID;

                                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IMPDETTAGLIORepository>().INSERT_QUERY, row, transaction);
                            }

                            transaction.Commit();

                            return $"{Model.MPANNO}/{Model.MPNUME}";
                        }
                        else
                        {
                            ErrorHandler.Show(Constants.INSERT_VIOLATION);
                            return null;
                        }
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Update(MPTESTATA Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.ExecuteScalar(UPDATE_QUERY, Model);

                if (result != null)
                {
                    return true;
                }
                else
                {
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public string? Update(MPTESTATA Model, ObservableCollection<MPDETTAGLIO> Rows)
        {
            try
            {
                using var connection = GetOpenConnection();

                using (var transaction = connection.BeginTransaction())
                {
                    connection.Execute("DELETE MPDETTAGLIO WHERE MPSOCI = @cid AND MPANNO = @year AND MPNUME = @id", new { cid = Model.MPSOCI, year = Model.MPANNO, id = Model.MPNUME }, transaction);

                    Model.MPEUIM = Rows.Where(o => o.M3SEGN == "A").Sum(s => s.M3IMEU) - Rows.Where(o => o.M3SEGN == "D").Sum(s => s.M3IMEU);

                    var result = connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);

                    if (result != null)
                    {
                        foreach (var row in Rows)
                        {
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IMPDETTAGLIORepository>().INSERT_QUERY, row, transaction);
                        }

                        transaction.Commit();

                        return $"{Model.MPANNO}/{Model.MPNUME}";
                    }
                    else
                    {
                        ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool CanDelete(string CompanyID, int Year, int ID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var item = connection.Query<MPTESTATA>(
                    @"SELECT * FROM MPTESTATA WHERE MPSOCI = @cid AND MPANNO = @year AND MPNUME = @id",
                    new { cid = CompanyID, year = Year, id = ID })
                    .FirstOrDefault();

                return item != null && (item.MPFLST == "N" || string.IsNullOrEmpty(item.MPFLST));
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool Delete(MPTESTATA Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                using (var transaction = connection.BeginTransaction())
                {
                    var result = connection.Execute(DELETE_QUERY, Model, transaction);
                    if (result > 0)
                    {
                        result = connection.Execute("DELETE MPDETTAGLIO WHERE MPSOCI = @cid AND MPANNO = @year AND MPNUME = @id", new { cid = Model.MPSOCI, year = Model.MPANNO, id = Model.MPNUME }, transaction);

                        transaction.Commit();

                        return true;
                    }
                    else
                    {
                        ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public string? Validate(MPTESTATA Model, bool IsInsert)
        {
            try
            {
                if (Model.MPANNO > 0)
                {
                    if (!IsInsert || (IsInsert && !Exists(Model.MPSOCI, Model.MPANNO, Model.MPNUME)))
                    {
                        return null;
                    }
                    else
                    { return "L'anno inserito è già in uso"; }
                }
                else
                { return "L'anno è obbligatorio"; }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }
}
