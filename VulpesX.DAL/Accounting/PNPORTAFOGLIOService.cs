namespace VulpesX.DAL.Accounting
{
    public interface IPNPORTAFOGLIORepository
    {
        ObservableCollection<PNPORTAFOGLIO>? GetList(string CompanyID, int Year, string Status);

        ObservableCollection<PNPORTAFOGLIO>? GetList(string CompanyID, DateTime From, DateTime To, string Status);

        PNPORTAFOGLIO? Get(string N6SOCI, int N6ANNO, int N6REGI, int N6RIGA);

        decimal GetCastelletto(string CompanyID, DateTime ExpireDate, int ABI, int ABINew, int CAB, int CABNew);

        ObservableCollection<PNPORTAFOGLIO>? GetCastellettoDetails(string CompanyID, int ABI, int CAB, int ABINew, int CABNew);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }

        bool Insert(PNPORTAFOGLIO Model);

        bool Update(PNPORTAFOGLIO Model);

        bool Delete(PNPORTAFOGLIO Model);

        bool Duplicate(PNPORTAFOGLIO Model);

        string? Validate(PNPORTAFOGLIO Model, bool IsInsert);

        #endregion
    }

    public class PNPORTAFOGLIORepository : RepositoryBase, IPNPORTAFOGLIORepository
    {
        public PNPORTAFOGLIORepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<PNPORTAFOGLIO>? GetList(string CompanyID, int Year, string Status)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PNPORTAFOGLIO>(
                    $@"SELECT p.*, CONCAT(a.abecod,' ',a.abers1) AS CustomerDescription, CONCAT(TRIM(b.abiban),' ' , TRIM(b.abiage)) AS CustomerBankDescription,CONCAT(TRIM(bd.abiban),' ' , TRIM(bd.abiage)) AS BankDescription, d.accounting_date DistinctDate  FROM PNPORTAFOGLIO AS p
                            INNER JOIN ABE AS a ON a.abecod = p.N6SOTT
                            INNER JOIN ABICAB AS b ON b.abiabi = p.N6CABI AND b.abicab = p.N6CCAB
							LEFT OUTER JOIN PNPORTAFOGLIO_DIST as d ON p.N6SOCI = d.company_id AND p.N6DISTI = d.id
							LEFT OUTER JOIN ABICAB AS bd ON bd.abiabi = d.abi AND bd.abicab = d.cab
                            WHERE p.N6SOCI=@cid AND p.N6ANNO=@yea {(Status == "*" ? null : "AND p.N6STATO=@sta")}
                            ORDER BY N6SCAD, N6SOTT",
                    new { cid = CompanyID, yea = Year, sta = Status });

                return new ObservableCollection<PNPORTAFOGLIO>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<PNPORTAFOGLIO>? GetList(string CompanyID, DateTime From, DateTime To, string Status)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<PNPORTAFOGLIO>(
                    $@"SELECT p.*, CONCAT(a.abecod,' ',a.abers1) AS CustomerDescription, CONCAT(TRIM(b.abiban),' ' , TRIM(b.abiage)) AS CustomerBankDescription,CONCAT(TRIM(bd.abiban),' ' , TRIM(bd.abiage)) AS BankDescription, d.accounting_date DistinctDate  FROM PNPORTAFOGLIO AS p
                            INNER JOIN ABE AS a ON a.abecod = p.N6SOTT
                            INNER JOIN ABICAB AS b ON b.abiabi = p.N6CABI AND b.abicab = p.N6CCAB
							LEFT OUTER JOIN PNPORTAFOGLIO_DIST as d ON p.N6SOCI = d.company_id AND p.N6DISTI = d.id
							LEFT OUTER JOIN ABICAB AS bd ON bd.abiabi = d.abi AND bd.abicab = d.cab
                            WHERE p.N6SOCI=@cid AND p.N6SCAD >= @From AND p.N6SCAD <= @To {(Status == "*" ? null : "AND p.N6STATO=@sta")}
                            ORDER BY N6SCAD, N6SOTT",
                    new { cid = CompanyID, From = From, To = To, sta = Status });

                return new ObservableCollection<PNPORTAFOGLIO>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public PNPORTAFOGLIO? Get(string N6SOCI, int N6ANNO, int N6REGI, int N6RIGA)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<PNPORTAFOGLIO>(
                    "SELECT * FROM PNPORTAFOGLIO WHERE N6SOCI = @N6SOCI AND N6ANNO = @N6ANNO AND N6REGI = @N6REGI AND N6RIGA = @N6RIGA",
                    new { N6SOCI = N6SOCI, N6ANNO = N6ANNO, N6REGI = N6REGI, N6RIGA = N6RIGA })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public decimal GetCastelletto(string CompanyID, DateTime ExpireDate, int ABI, int ABINew, int CAB, int CABNew)
        {
            try
            {
                using var connection = GetOpenConnection();


                decimal? result = connection.ExecuteScalar<decimal?>(
                    @"SELECT SUM(case when N6COVA ='EUR' then N6IMEU else N6IMPO end) FROM PNPORTAFOGLIO 
                            INNER JOIN PNPORTAFOGLIO_DIST AS d ON d.company_id = N6SOCI AND d.id = N6DISTI
                            WHERE N6SOCI = @cid AND N6STATO='C' AND N6SCAD >= @exp AND 
                            (d.abi = @abi OR d.abi = @abinew) AND (d.cab = @cab OR d.cab = @cabnew)",
                    new { cid = CompanyID, exp = ExpireDate, abi = ABI, abinew = ABINew, cab = CAB, cabnew = CABNew });

                return result ?? 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return -1;
            }
        }

        public ObservableCollection<PNPORTAFOGLIO>? GetCastellettoDetails(string CompanyID, int ABI, int CAB, int ABINew, int CABNew)
        {
            try
            {
                using var connection = GetOpenConnection();


                return new ObservableCollection<PNPORTAFOGLIO>(connection.Query<PNPORTAFOGLIO, PNTESTATA, string, PNPORTAFOGLIO>(
                    @"SELECT p.*, t.N1CLFO, a.abers1 FROM PNPORTAFOGLIO AS p
                            INNER JOIN PNPORTAFOGLIO_DIST AS d ON d.company_id = p.N6SOCI AND d.id = p.N6DISTI
                            INNER JOIN PNTESTATA AS t ON t.N1SOCI = p.N6SOCI AND t.N1ANNO = p.N6ANNO AND t.N1REGI = p.N6REGI
                            INNER JOIN ABE AS a ON a.abecod = t.N1CLFO
                            WHERE p.N6SOCI = @cid AND p.N6SCAD >= SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time' AND 
                                (d.abi = @abi OR d.abi = @abinew) AND (d.cab = @cab OR d.cab = @cabnew)
                            ORDER BY p.N6SCAD, p.N6REGI, p.N6RIGA",
                    (por, hea, abe) => { por.CustomerDescription = abe; return por; },
                    new { cid = CompanyID, abi = ABI, cab = CAB, abinew = ABINew, cabnew = CABNew }, splitOn: "N1CLFO,abers1"));

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO PNPORTAFOGLIO (N6SOCI,N6ANNO,N6REGI,N6RIGA,N6DOCU,N6RIFE,N6DARE,N6DADO,N6DARI,N6CAUS,N6GRUP,N6CONT,N6SOTT,N6IMPO,N6SCAD,N6DESC,N6SEGN,N6RATA,N6EFFE,N6DATE,N6CABI,N6CCAB,N6TIDI,N6IMPF,N6IMPV,N6COVA,N6IMEU,N6IFEU,N6COBA,N6TIPODOC,N6PERCANT,N6IMPANT,N6FIRANT,N6DATFIR,N6STATO,N6DISTI) OUTPUT INSERTED.rv VALUES(@N6SOCI,@N6ANNO,@N6REGI,@N6RIGA,@N6DOCU,@N6RIFE,@N6DARE,@N6DADO,@N6DARI,@N6CAUS,@N6GRUP,@N6CONT,@N6SOTT,@N6IMPO,@N6SCAD,@N6DESC,@N6SEGN,@N6RATA,@N6EFFE,@N6DATE,@N6CABI,@N6CCAB,@N6TIDI,@N6IMPF,@N6IMPV,@N6COVA,@N6IMEU,@N6IFEU,@N6COBA,@N6TIPODOC,@N6PERCANT,@N6IMPANT,@N6FIRANT,@N6DATFIR,@N6STATO,@N6DISTI)";
        public string UPDATE_QUERY => "UPDATE PNPORTAFOGLIO SET N6SOCI = @N6SOCI,N6ANNO = @N6ANNO,N6REGI = @N6REGI,N6RIGA = @N6RIGA,N6DOCU = @N6DOCU,N6RIFE = @N6RIFE,N6DARE = @N6DARE,N6DADO = @N6DADO,N6DARI = @N6DARI,N6CAUS = @N6CAUS,N6GRUP = @N6GRUP,N6CONT = @N6CONT,N6SOTT = @N6SOTT,N6IMPO = @N6IMPO,N6SCAD = @N6SCAD,N6DESC = @N6DESC,N6SEGN = @N6SEGN,N6RATA = @N6RATA,N6EFFE = @N6EFFE,N6DATE = @N6DATE,N6CABI = @N6CABI,N6CCAB = @N6CCAB,N6TIDI = @N6TIDI,N6IMPF = @N6IMPF,N6IMPV = @N6IMPV,N6COVA = @N6COVA,N6IMEU = @N6IMEU,N6IFEU = @N6IFEU,N6COBA = @N6COBA,N6TIPODOC = @N6TIPODOC,N6PERCANT = @N6PERCANT,N6IMPANT = @N6IMPANT,N6FIRANT = @N6FIRANT,N6DATFIR = @N6DATFIR,N6STATO = @N6STATO,N6DISTI = @N6DISTI OUTPUT INSERTED.rv WHERE N6SOCI = @N6SOCI AND N6ANNO = @N6ANNO AND N6REGI = @N6REGI AND N6RIGA = @N6RIGA AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM PNPORTAFOGLIO OUTPUT DELETED.rv WHERE N6SOCI = @N6SOCI AND N6ANNO = @N6ANNO AND N6REGI = @N6REGI AND N6RIGA = @N6RIGA AND rv = @rv";

        public bool Insert(PNPORTAFOGLIO Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.Execute(INSERT_QUERY, Model);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    ErrorHandler.Show(Constants.INSERT_VIOLATION);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool Update(PNPORTAFOGLIO Model)
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

        public bool Delete(PNPORTAFOGLIO Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = connection.Execute(DELETE_QUERY, Model);
                if (result > 0)
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

        public bool Duplicate(PNPORTAFOGLIO Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var lastRow = (int?)connection.ExecuteScalar(@"SELECT MAX(N6RIGA) FROM PNPORTAFOGLIO
                                                        WHERE N6SOCI=@cid AND N6ANNO=@yea AND N6REGI=@reg",
                                                    new { cid = Model.N6SOCI, yea = Model.N6ANNO, reg = Model.N6REGI }) ?? 0;
                var duplicated = new PNPORTAFOGLIO()
                {
                    N6SOCI = Model.N6SOCI,
                    N6ANNO = Model.N6ANNO,
                    N6REGI = Model.N6REGI,
                    N6RIGA = lastRow + 1,
                    N6DOCU = Model.N6DOCU,
                    N6RIFE = Model.N6RIFE,
                    N6DADO = Model.N6DADO,
                    N6DARI = Model.N6DARI,
                    N6DARE = Model.N6DARE,
                    N6CAUS = Model.N6CAUS,
                    N6GRUP = Model.N6GRUP,
                    N6CONT = Model.N6CONT,
                    N6SOTT = Model.N6SOTT,
                    N6IMPO = Model.N6IMPO,
                    N6SCAD = Model.N6SCAD,
                    N6DESC = Model.N6DESC,
                    N6SEGN = Model.N6SEGN,
                    N6DATE = Model.N6DATE,
                    N6RATA = Model.N6RATA,
                    N6EFFE = Model.N6EFFE,
                    N6CABI = Model.N6CABI,
                    N6CCAB = Model.N6CCAB,
                    N6TIDI = Model.N6TIDI,
                    N6IMPF = Model.N6IMPF,
                    N6IMPV = Model.N6IMPV,
                    N6COVA = Model.N6COVA,
                    N6IMEU = Model.N6IMEU,
                    N6IFEU = Model.N6IFEU,
                    N6COBA = Model.N6COBA,
                    N6TIPODOC = Model.N6TIPODOC,
                    N6PERCANT = Model.N6PERCANT,
                    N6IMPANT = Model.N6IMPANT,
                    N6FIRANT = Model.N6FIRANT,
                    N6DATFIR = Model.N6DATFIR,
                    N6STATO = "N",
                    N6DISTI = null
                };
                var result = connection.Execute(INSERT_QUERY, duplicated);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    ErrorHandler.Show(Constants.INSERT_VIOLATION);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public string? Validate(PNPORTAFOGLIO Model, bool IsInsert)
        {
            try
            {
                if (!string.IsNullOrEmpty(Model.N6SOCI))
                {
                    if (!string.IsNullOrWhiteSpace(Model.N6SOCI))
                    {
                        return null;
                    }
                    else
                    { return "La descrizione è obbligatoria"; }
                }
                else
                { return "Il codice inserito è già in uso o non è valido"; }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion
    }

    public class PNPORTAFOGLIOUfpRepository : RepositoryBase, IPNPORTAFOGLIORepository
    {
        public PNPORTAFOGLIOUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<PNPORTAFOGLIO>? GetList(string CompanyID, int Year, string Status)
        {
            try
            {
                using var connection = GetOpenConnection();

                string statusFilter = Status switch
                {
                    "*" => "",
                    "N" => "AND p.N6ESTR = 'N'",
                    "E" => "AND p.N6ESTR = 'S' AND p.N6COLL = 'N'",
                    "C" => "AND p.N6ESTR = 'S' AND p.N6COLL = 'S'",
                    _ => ""
                };

                var list = connection.Query<PNPORTAFOGLIO>(
                    $@"SELECT p.*, CONCAT(a.abecod,' ',a.abers1) AS CustomerDescription, CONCAT(TRIM(b.abiban),' ' , TRIM(b.abiage)) AS CustomerBankDescription, CONCAT(TRIM(bb.abiban),' ' , TRIM(bb.abiage)) AS BankDescription  FROM PN_PORTAFOGLIO AS p
                            INNER JOIN ANAG_BASE AS a ON a.abecod = p.N6SOTT
                            INNER JOIN TAB_ABICAB AS b ON b.abiabi = p.N6CABI AND b.abicab = p.N6CCAB
                            LEFT OUTER JOIN TAB_ABICAB AS bb ON bb.abiabi = p.N6AABI AND bb.abicab = p.N6ACAB
                            WHERE p.N6SOCI=@cid AND p.N6ANNO=@yea {statusFilter}
                            ORDER BY N6SCAD, N6SOTT",
                    new { cid = CompanyID, yea = Year, sta = Status });

                return new ObservableCollection<PNPORTAFOGLIO>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<PNPORTAFOGLIO>? GetList(string CompanyID, DateTime From, DateTime To, string Status)
        {
            try
            {
                using var connection = GetOpenConnection();

                string statusFilter = Status switch
                {
                    "*" => "",
                    "N" => "AND p.N6ESTR = 'N'",
                    "E" => "AND p.N6ESTR = 'S' AND p.N6COLL = 'N'",
                    "C" => "AND p.N6ESTR = 'S' AND p.N6COLL = 'S'",
                    _ => ""
                };

                var list = connection.Query<PNPORTAFOGLIO>(
                    $@"SELECT p.*, CONCAT(a.abecod,' ',a.abers1) AS CustomerDescription, CONCAT(TRIM(b.abiban),' ' , TRIM(b.abiage)) AS CustomerBankDescription, CONCAT(TRIM(bb.abiban),' ' , TRIM(bb.abiage)) AS BankDescription  FROM PN_PORTAFOGLIO AS p
                            INNER JOIN ANAG_BASE AS a ON a.abecod = p.N6SOTT
                            INNER JOIN TAB_ABICAB AS b ON b.abiabi = p.N6CABI AND b.abicab = p.N6CCAB
                            LEFT OUTER JOIN TAB_ABICAB AS bb ON bb.abiabi = p.N6AABI AND bb.abicab = p.N6ACAB
                            WHERE p.N6SOCI=@cid AND p.N6SCAD >= @From AND p.N6SCAD <= @To {statusFilter}
                            ORDER BY N6SCAD, N6SOTT",
                    new { cid = CompanyID, From = From, To = To, sta = Status });

                return new ObservableCollection<PNPORTAFOGLIO>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public PNPORTAFOGLIO? Get(string N6SOCI, int N6ANNO, int N6REGI, int N6RIGA)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<PNPORTAFOGLIO>(
                    "SELECT * FROM PN_PORTAFOGLIO WHERE N6SOCI = @N6SOCI AND N6ANNO = @N6ANNO AND N6REGI = @N6REGI AND N6RIGA = @N6RIGA",
                    new { N6SOCI = N6SOCI, N6ANNO = N6ANNO, N6REGI = N6REGI, N6RIGA = N6RIGA })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public decimal GetCastelletto(string CompanyID, DateTime ExpireDate, int ABI, int ABINew, int CAB, int CABNew)
        {
            try
            {
                using var connection = GetOpenConnection();


                decimal? result = connection.ExecuteScalar<decimal?>(
                    @"SELECT SUM(case when N6COVA ='EUR' then N6IMEU else N6IMPO end) FROM PN_PORTAFOGLIO 
                            INNER JOIN PN_PORTAFOGLIO_DIST AS d ON d.company_id = N6SOCI AND d.id = N6DISTI
                            WHERE N6SOCI = @cid AND N6STATO='C' AND N6SCAD >= @exp AND 
                            (d.abi = @abi OR d.abi = @abinew) AND (d.cab = @cab OR d.cab = @cabnew)",
                    new { cid = CompanyID, exp = ExpireDate, abi = ABI, abinew = ABINew, cab = CAB, cabnew = CABNew });

                return result ?? 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return -1;
            }
        }

        public ObservableCollection<PNPORTAFOGLIO>? GetCastellettoDetails(string CompanyID, int ABI, int CAB, int ABINew, int CABNew)
        {
            try
            {
                using var connection = GetOpenConnection();


                return new ObservableCollection<PNPORTAFOGLIO>(connection.Query<PNPORTAFOGLIO, PNTESTATA, string, PNPORTAFOGLIO>(
                    @"SELECT p.*, t.N1CLFO, a.abers1 FROM PN_PORTAFOGLIO AS p
                            INNER JOIN PN_PORTAFOGLIO_DIST AS d ON d.company_id = p.N6SOCI AND d.id = p.N6DISTI
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI = p.N6SOCI AND t.N1ANNO = p.N6ANNO AND t.N1REGI = p.N6REGI
                            INNER JOIN ANAG_BASE AS a ON a.abecod = t.N1CLFO
                            WHERE p.N6SOCI = @cid AND p.N6SCAD >= SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time' AND 
                                (d.abi = @abi OR d.abi = @abinew) AND (d.cab = @cab OR d.cab = @cabnew)
                            ORDER BY p.N6SCAD, p.N6REGI, p.N6RIGA",
                    (por, hea, abe) => { por.CustomerDescription = abe; return por; },
                    new { cid = CompanyID, abi = ABI, cab = CAB, abinew = ABINew, cabnew = CABNew }, splitOn: "N1CLFO,abers1"));

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO PN_PORTAFOGLIO (N6SOCI,N6ANNO,N6REGI,N6RIGA,N6DOCU,N6RIFE,N6DARE,N6DADO,N6DARI,N6CAUS,N6GRUP,N6CONT,N6SOTT,N6IMPO,N6SCAD,N6DESC,N6SEGN,N6RATA,N6EFFE,N6DATE,N6CABI,N6CCAB,N6TIDI,N6IMPF,N6IMPV,N6COVA,N6IMEU,N6IFEU,N6COBA,N6TIPODOC,N6PERCANT,N6IMPANT,N6FIRANT,N6DATFIR , N6AABI, N6ACAB,N6ESTR,N6NUDI,N6DADI, N6COLL, N6GCT1,N6GCT2,N6GCT3,N6GCT4) OUTPUT INSERTED.rv " +
            "                                               VALUES(@N6SOCI,@N6ANNO,@N6REGI,@N6RIGA,@N6DOCU,@N6RIFE,@N6DARE,@N6DADO,@N6DARI,@N6CAUS,@N6GRUP,@N6CONT,@N6SOTT,@N6IMPO,@N6SCAD,@N6DESC,@N6SEGN,@N6RATA,@N6EFFE,@N6DATE,@N6CABI,@N6CCAB,@N6TIDI,@N6IMPF,@N6IMPV,@N6COVA,@N6IMEU,@N6IFEU,@N6COBA,@N6TIPODOC,@N6PERCANT,@N6IMPANT,@N6FIRANT,@N6DATFIR,@N6AABI, @N6ACAB,@N6ESTR, @N6NUDI, @N6DADI,@N6COLL,'','','','')";
        public string UPDATE_QUERY => "UPDATE PN_PORTAFOGLIO SET N6SOCI = @N6SOCI,N6ANNO = @N6ANNO,N6REGI = @N6REGI,N6RIGA = @N6RIGA,N6DOCU = @N6DOCU,N6RIFE = @N6RIFE,N6DARE = @N6DARE,N6DADO = @N6DADO,N6DARI = @N6DARI,N6CAUS = @N6CAUS,N6GRUP = @N6GRUP,N6CONT = @N6CONT,N6SOTT = @N6SOTT,N6IMPO = @N6IMPO,N6SCAD = @N6SCAD,N6DESC = @N6DESC,N6SEGN = @N6SEGN,N6RATA = @N6RATA,N6EFFE = @N6EFFE,N6DATE = @N6DATE,N6CABI = @N6CABI,N6CCAB = @N6CCAB,N6TIDI = @N6TIDI,N6IMPF = @N6IMPF,N6IMPV = @N6IMPV,N6COVA = @N6COVA,N6IMEU = @N6IMEU,N6IFEU = @N6IFEU,N6COBA = @N6COBA,N6TIPODOC = @N6TIPODOC,N6PERCANT = @N6PERCANT,N6IMPANT = @N6IMPANT,N6FIRANT = @N6FIRANT,N6DATFIR = @N6DATFIR, N6AABI = @N6AABI, N6ACAB = @N6ACAB,N6ESTR =@N6ESTR,N6NUDI =@N6NUDI, N6DADI = @N6DADI,N6COLL = @N6COLL OUTPUT INSERTED.rv WHERE N6SOCI = @N6SOCI AND N6ANNO = @N6ANNO AND N6REGI = @N6REGI AND N6RIGA = @N6RIGA AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM PN_PORTAFOGLIO OUTPUT DELETED.rv WHERE N6SOCI = @N6SOCI AND N6ANNO = @N6ANNO AND N6REGI = @N6REGI AND N6RIGA = @N6RIGA AND rv = @rv";

        public bool Insert(PNPORTAFOGLIO Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.Execute(INSERT_QUERY, Model);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    ErrorHandler.Show(Constants.INSERT_VIOLATION);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool Update(PNPORTAFOGLIO Model)
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

        public bool Delete(PNPORTAFOGLIO Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = connection.Execute(DELETE_QUERY, Model);
                if (result > 0)
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

        public bool Duplicate(PNPORTAFOGLIO Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var lastRow = (int?)connection.ExecuteScalar(@"SELECT MAX(N6RIGA) FROM PN_PORTAFOGLIO
                                                        WHERE N6SOCI=@cid AND N6ANNO=@yea AND N6REGI=@reg",
                                                    new { cid = Model.N6SOCI, yea = Model.N6ANNO, reg = Model.N6REGI }) ?? 0;
                var duplicated = new PNPORTAFOGLIO()
                {
                    N6SOCI = Model.N6SOCI,
                    N6ANNO = Model.N6ANNO,
                    N6REGI = Model.N6REGI,
                    N6RIGA = lastRow + 1,
                    N6DOCU = Model.N6DOCU,
                    N6RIFE = Model.N6RIFE,
                    N6DADO = Model.N6DADO,
                    N6DARI = Model.N6DARI,
                    N6DARE = Model.N6DARE,
                    N6CAUS = Model.N6CAUS,
                    N6GRUP = Model.N6GRUP,
                    N6CONT = Model.N6CONT,
                    N6SOTT = Model.N6SOTT,
                    N6IMPO = Model.N6IMPO,
                    N6SCAD = Model.N6SCAD,
                    N6DESC = Model.N6DESC,
                    N6SEGN = Model.N6SEGN,
                    N6DATE = Model.N6DATE,
                    N6RATA = Model.N6RATA,
                    N6EFFE = Model.N6EFFE,
                    N6CABI = Model.N6CABI,
                    N6CCAB = Model.N6CCAB,
                    N6TIDI = Model.N6TIDI,
                    N6IMPF = Model.N6IMPF,
                    N6IMPV = Model.N6IMPV,
                    N6COVA = Model.N6COVA,
                    N6IMEU = Model.N6IMEU,
                    N6IFEU = Model.N6IFEU,
                    N6COBA = Model.N6COBA,
                    N6TIPODOC = Model.N6TIPODOC,
                    N6PERCANT = Model.N6PERCANT,
                    N6IMPANT = Model.N6IMPANT,
                    N6FIRANT = Model.N6FIRANT,
                    N6DATFIR = Model.N6DATFIR,
                    N6STATO = "N",
                    N6DISTI = null,
                    N6ESTR = Model.N6ESTR,
                };
                var result = connection.Execute(INSERT_QUERY, duplicated);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    ErrorHandler.Show(Constants.INSERT_VIOLATION);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public string? Validate(PNPORTAFOGLIO Model, bool IsInsert)
        {
            try
            {
                if (!string.IsNullOrEmpty(Model.N6SOCI))
                {
                    if (!string.IsNullOrWhiteSpace(Model.N6SOCI))
                    {
                        return null;
                    }
                    else
                    { return "La descrizione è obbligatoria"; }
                }
                else
                { return "Il codice inserito è già in uso o non è valido"; }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion
    }
}
