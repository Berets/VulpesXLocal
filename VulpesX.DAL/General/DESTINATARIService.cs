using VulpesX.Shared.Generics;

namespace VulpesX.DAL.General;

public interface IDESTINATARIRepository
{
    ObservableCollection<DESTINATARI>? GetList(int EntityID);

    ObservableCollection<GenericIDDescription>? GetSimpleList(int EntityID, bool AddAllItem);

    ObservableCollection<GenericIntIDDescription>? GetSimpleIntList(int EntityID, bool AddAllItem);

    ObservableCollection<DESTINATARI>? GetFullList(int EntityID);

    DESTINATARI? Get(int EntityID, int Sequence);

    bool Exists(int EntityID, int Sequence);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(DESTINATARI Model);

    bool Update(DESTINATARI Model);

    string? CanDelete(DESTINATARI Model);

    bool Delete(DESTINATARI Model);

    string? Validate(DESTINATARI Model, bool IsInsert);
    #endregion
}

public class DESTINATARIRepository : RepositoryBase, IDESTINATARIRepository
{
    public DESTINATARIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<DESTINATARI>? GetList(int EntityID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<DESTINATARI>(
                @"SELECT d.* FROM DESTINATARI AS d
                        WHERE d.cliecod = @cliecod
                        ORDER BY d.codesti",
                new { cliecod = EntityID });

            return new ObservableCollection<DESTINATARI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<GenericIDDescription>? GetSimpleList(int EntityID, bool AddAllItem)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<GenericIDDescription>(
                @"SELECT d.codesti AS ID, d.ragisoc AS Description FROM DESTINATARI AS d
                        WHERE d.cliecod = @cliecod
                        ORDER BY d.codesti",
                new { cliecod = EntityID }).ToList();

            if (AddAllItem)
            {
                list.Add(new GenericIDDescription()
                {
                    ID = null,
                    Description = "Tutti i destinatari"
                });
            }

            return new ObservableCollection<GenericIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<GenericIntIDDescription>? GetSimpleIntList(int EntityID, bool AddAllItem)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<GenericIntIDDescription>(
                @"SELECT d.codesti AS ID, d.ragisoc AS Description FROM DESTINATARI AS d
                        WHERE d.cliecod = @cliecod
                        ORDER BY d.codesti",
                new { cliecod = EntityID }).ToList();

            if (AddAllItem)
            {
                list.Add(new GenericIntIDDescription()
                {
                    ID = null,
                    Description = "Tutti i destinatari"
                });
            }

            return new ObservableCollection<GenericIntIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<DESTINATARI>? GetFullList(int EntityID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<DESTINATARI, AGENTI, AGENTI, DESTINATARI>(
                @"SELECT d.*, a1.*, a2.* FROM DESTINATARI AS d
                        LEFT JOIN AGENTI AS a1 ON a1.agecod = d.decoag1
                        LEFT JOIN AGENTI AS a2 ON a2.agecod = d.decoag2
                        WHERE d.cliecod = @cliecod
                        ORDER BY d.codesti",
                (des, ag1, ag2) => { des.FirstAgent = ag1; des.SecondAgent = ag2; return des; },
                new { cliecod = EntityID }, splitOn: "agecod");

            return new ObservableCollection<DESTINATARI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public DESTINATARI? Get(int EntityID, int Sequence)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<DESTINATARI>(
                "SELECT * FROM DESTINATARI WHERE cliecod = @cliecod AND codesti = @codesti",
                new { cliecod = EntityID, codesti = Sequence })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int EntityID, int Sequence)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM DESTINATARI WHERE cliecod = @cliecod AND codesti = @codesti ",
                new { cliecod = EntityID, codesti = Sequence }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO DESTINATARI (cliecod,codesti,ragisoc,DEINDI,DECAP,deloc,depro,person,decoag1,decoag2,deage1p,deage2p,deage1pt,deage2pt,isocod) OUTPUT INSERTED.rv VALUES(@cliecod,@codesti,@ragisoc,@DEINDI,@DECAP,@deloc,@depro,@person,@decoag1,@decoag2,@deage1p,@deage2p,@deage1pt,@deage2pt,@isocod)";
    public string UPDATE_QUERY => "UPDATE DESTINATARI SET cliecod = @cliecod,codesti = @codesti,ragisoc = @ragisoc,DEINDI = @DEINDI,DECAP = @DECAP,deloc = @deloc,depro = @depro,person = @person,decoag1 = @decoag1,decoag2 = @decoag2,deage1p = @deage1p,deage2p = @deage2p,deage1pt = @deage1pt,deage2pt = @deage2pt, isocod = @isocod OUTPUT INSERTED.rv WHERE cliecod = @cliecod AND codesti = @codesti AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM DESTINATARI OUTPUT DELETED.rv WHERE cliecod = @cliecod AND codesti = @codesti AND rv = @rv";
    public bool Insert(DESTINATARI Model)
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

    public bool Update(DESTINATARI Model)
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

    public string? CanDelete(DESTINATARI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            if (connection != null)
            {
                connection.Open();
                // offers
                if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM OFFET00F
                                                    WHERE OFTCOCL=@customer AND OFTDEST=@recipient",
                    new { customer = Model.cliecod, recipient = Model.codesti }) > 0)
                {
                    return "Impossibile eliminare, ci sono delle offerte che utilizzano questa destinazione";
                }
                // orders
                if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM ORDIT00F
                                                    WHERE OTCLIE=@customer AND DESTIN=@recipient",
                    new { customer = Model.cliecod, recipient = Model.codesti }) > 0)
                {
                    return "Impossibile eliminare, ci sono degli ordini che utilizzano questa destinazione";
                }
                // DDT
                if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM BOLLT00F
                                                    WHERE BTCODC=@customer AND BTCODD=@recipient",
                    new { customer = Model.cliecod, recipient = Model.codesti }) > 0)
                {
                    return "Impossibile eliminare, ci sono dei DDT che utilizzano questa destinazione";
                }
                // invoices
                if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM FATTT00F
                                                    WHERE FTCODC=@customer AND FTCODD=@recipient",
                    new { customer = Model.cliecod, recipient = Model.codesti }) > 0)
                {
                    return "Impossibile eliminare, ci sono delle fatture che utilizzano questa destinazione";
                }
                return null;
            }
            else
            {
                return Constants.CONNECTION_CREATION_ERROR;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public bool Delete(DESTINATARI Model)
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

    public string? Validate(DESTINATARI Model, bool IsInsert)
    {
        try
        {
            if (Model.cliecod > 0)
            {
                if (Model.codesti > 0)
                {
                    if (!string.IsNullOrWhiteSpace(Model.ragisoc))
                    {
                        if (!string.IsNullOrWhiteSpace(Model.DEINDI))
                        {
                            if (Model.DECAP.HasValue && Model.DECAP.Value > 0)
                            {
                                if (!string.IsNullOrWhiteSpace(Model.deloc))
                                {
                                    if (!string.IsNullOrWhiteSpace(Model.depro))
                                    {
                                        if (!string.IsNullOrWhiteSpace(Model.person))
                                        {
                                            if ((string.IsNullOrWhiteSpace(Model.decoag1) && !Model.deage1p.HasValue && string.IsNullOrWhiteSpace(Model.deage1pt)) ||
                                                (!string.IsNullOrWhiteSpace(Model.decoag1) && ((Model.deage1p.HasValue && !string.IsNullOrWhiteSpace(Model.deage1pt)) || (!Model.deage1p.HasValue && string.IsNullOrWhiteSpace(Model.deage1pt)))))
                                            {
                                                if ((string.IsNullOrWhiteSpace(Model.decoag1) && !Model.deage1p.HasValue && string.IsNullOrWhiteSpace(Model.deage1pt)) ||
                                                (!string.IsNullOrWhiteSpace(Model.decoag1) && ((Model.deage1p.HasValue && !string.IsNullOrWhiteSpace(Model.deage1pt)) || (!Model.deage1p.HasValue && string.IsNullOrWhiteSpace(Model.deage1pt)))))
                                                {
                                                    return null;
                                                }
                                                else
                                                { return "La provvigione del secondo agente ed il suo tipo devono essere specificati entrambi o nessuno e solo se l'agente è stato selezionato"; }
                                            }
                                            else
                                            { return "La provvigione del primo agente ed il suo tipo devono essere specificati entrambi o nessuno e solo se l'agente è stato selezionato"; }
                                        }
                                        else
                                        { return "Il riferimento è obbligatorio"; }
                                    }
                                    else
                                    { return "La provincia è obbligatoria"; }
                                }
                                else
                                { return "Il comune è obbligatorio"; }
                            }
                            else
                            { return "Il C.A.P. è un dato obbligatorio"; }
                        }
                        else
                        { return "L'indirizzo è obbligatorio"; }
                    }
                    else
                    { return "La ragione sociale è obbligatoria"; }
                }
                else
                { return "Il progressivo è obbligatorio"; }
            }
            else
            { return "Il cliente di riferimento è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class DESTINATARIUfpRepository : RepositoryBase, IDESTINATARIRepository
{
    public DESTINATARIUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<DESTINATARI>? GetList(int EntityID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<DESTINATARI>(
                @"SELECT d.*, d.isoid as isocod, d.depro1 as depro FROM DESTINATARI AS d
                        WHERE d.cliecod = @cliecod
                        ORDER BY d.codesti",
                new { cliecod = EntityID });

            return new ObservableCollection<DESTINATARI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<GenericIDDescription>? GetSimpleList(int EntityID, bool AddAllItem)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<GenericIDDescription>(
                @"SELECT d.codesti AS ID, d.ragisoc AS Description FROM DESTINATARI AS d
                        WHERE d.cliecod = @cliecod
                        ORDER BY d.codesti",
                new { cliecod = EntityID }).ToList();

            if (AddAllItem)
            {
                list.Add(new GenericIDDescription()
                {
                    ID = null,
                    Description = "Tutti i destinatari"
                });
            }

            return new ObservableCollection<GenericIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<GenericIntIDDescription>? GetSimpleIntList(int EntityID, bool AddAllItem)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<GenericIntIDDescription>(
                @"SELECT d.codesti AS ID, d.ragisoc AS Description FROM DESTINATARI AS d
                        WHERE d.cliecod = @cliecod
                        ORDER BY d.codesti",
                new { cliecod = EntityID }).ToList();

            if (AddAllItem)
            {
                list.Add(new GenericIntIDDescription()
                {
                    ID = null,
                    Description = "Tutti i destinatari"
                });
            }

            return new ObservableCollection<GenericIntIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<DESTINATARI>? GetFullList(int EntityID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<DESTINATARI, AGENTI, AGENTI, DESTINATARI>(
                @"SELECT d.*, a1.*, a2.*, d.isoid as isocod  FROM DESTINATARI AS d
                        LEFT JOIN AGENTI AS a1 ON a1.agecod = d.decoag1
                        LEFT JOIN AGENTI AS a2 ON a2.agecod = d.decoag2
                        WHERE d.cliecod = @cliecod
                        ORDER BY d.codesti",
                (des, ag1, ag2) => { des.FirstAgent = ag1; des.SecondAgent = ag2; return des; },
                new { cliecod = EntityID }, splitOn: "agecod");

            return new ObservableCollection<DESTINATARI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public DESTINATARI? Get(int EntityID, int Sequence)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<DESTINATARI>(
                "SELECT *, isoid as isocod  FROM DESTINATARI WHERE cliecod = @cliecod AND codesti = @codesti",
                new { cliecod = EntityID, codesti = Sequence })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int EntityID, int Sequence)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM DESTINATARI WHERE cliecod = @cliecod AND codesti = @codesti ",
                new { cliecod = EntityID, codesti = Sequence }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO DESTINATARI (cliecod,codesti,ragisoc,DEINDI,DECAP,deloc,depro1, depro2,person,climaga,clicaumag,depri,isoid) OUTPUT INSERTED.rv VALUES(@cliecod,@codesti,@ragisoc,@DEINDI,@DECAP,@deloc,@depro,@depro2,@person,@climaga,@clicaumag,@depri,@isocod)";
    public string UPDATE_QUERY => "UPDATE DESTINATARI SET cliecod = @cliecod,codesti = @codesti,ragisoc = @ragisoc,DEINDI = @DEINDI,DECAP = @DECAP,deloc = @deloc,depro1 = @depro,depro2=@depro2,person = @person, climaga=@climaga,clicaumag=@clicaumag,depri=@depri, isoid = @isocod OUTPUT INSERTED.rv WHERE cliecod = @cliecod AND codesti = @codesti AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM DESTINATARI OUTPUT DELETED.rv WHERE cliecod = @cliecod AND codesti = @codesti AND rv = @rv";
    public bool Insert(DESTINATARI Model)
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

    public bool Update(DESTINATARI Model)
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

    public string? CanDelete(DESTINATARI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            if (connection != null)
            {
                connection.Open();
                // offers
                if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM OFFET00F
                                                    WHERE OFTCOCL=@customer AND OFTDEST=@recipient",
                    new { customer = Model.cliecod, recipient = Model.codesti }) > 0)
                {
                    return "Impossibile eliminare, ci sono delle offerte che utilizzano questa destinazione";
                }
                // orders
                if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM ORDIT00F
                                                    WHERE OTCLIE=@customer AND DESTIN=@recipient",
                    new { customer = Model.cliecod, recipient = Model.codesti }) > 0)
                {
                    return "Impossibile eliminare, ci sono degli ordini che utilizzano questa destinazione";
                }
                // DDT
                if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM BOLLT00F
                                                    WHERE BTCODC=@customer AND BTCODD=@recipient",
                    new { customer = Model.cliecod, recipient = Model.codesti }) > 0)
                {
                    return "Impossibile eliminare, ci sono dei DDT che utilizzano questa destinazione";
                }
                // invoices
                if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM FATTT00F
                                                    WHERE FTCODC=@customer AND FTCODD=@recipient",
                    new { customer = Model.cliecod, recipient = Model.codesti }) > 0)
                {
                    return "Impossibile eliminare, ci sono delle fatture che utilizzano questa destinazione";
                }
                return null;
            }
            else
            {
                return Constants.CONNECTION_CREATION_ERROR;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public bool Delete(DESTINATARI Model)
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

    public string? Validate(DESTINATARI Model, bool IsInsert)
    {
        try
        {
            if (Model.cliecod > 0)
            {
                if (Model.codesti > 0)
                {
                    if (!string.IsNullOrWhiteSpace(Model.ragisoc))
                    {
                        if (!string.IsNullOrWhiteSpace(Model.DEINDI))
                        {


                         
                                if (!string.IsNullOrWhiteSpace(Model.person))
                                {
                                    if ((string.IsNullOrWhiteSpace(Model.decoag1) && !Model.deage1p.HasValue && string.IsNullOrWhiteSpace(Model.deage1pt)) ||
                                        (!string.IsNullOrWhiteSpace(Model.decoag1) && ((Model.deage1p.HasValue && !string.IsNullOrWhiteSpace(Model.deage1pt)) || (!Model.deage1p.HasValue && string.IsNullOrWhiteSpace(Model.deage1pt)))))
                                    {
                                        if ((string.IsNullOrWhiteSpace(Model.decoag1) && !Model.deage1p.HasValue && string.IsNullOrWhiteSpace(Model.deage1pt)) ||
                                        (!string.IsNullOrWhiteSpace(Model.decoag1) && ((Model.deage1p.HasValue && !string.IsNullOrWhiteSpace(Model.deage1pt)) || (!Model.deage1p.HasValue && string.IsNullOrWhiteSpace(Model.deage1pt)))))
                                        {
                                            return null;
                                        }
                                        else
                                        { return "La provvigione del secondo agente ed il suo tipo devono essere specificati entrambi o nessuno e solo se l'agente è stato selezionato"; }
                                    }
                                    else
                                    { return "La provvigione del primo agente ed il suo tipo devono essere specificati entrambi o nessuno e solo se l'agente è stato selezionato"; }
                                }
                                else
                                { return "Il riferimento è obbligatorio"; }
                          

                        }
                        else
                        { return "L'indirizzo è obbligatorio"; }
                    }
                    else
                    { return "La ragione sociale è obbligatoria"; }
                }
                else
                { return "Il progressivo è obbligatorio"; }
            }
            else
            { return "Il cliente di riferimento è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}