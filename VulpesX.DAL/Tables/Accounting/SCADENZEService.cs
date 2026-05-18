namespace VulpesX.DAL.Tables.Accounting;

public interface ISCADENZERepository
{
    ObservableCollection<SCADENZE>? GetList();

    SCADENZE? Get(string ID);

    DateTime ComputeExpireMove(SCADENZE? Mover, DateTime StartingExpire);

    bool Exists(string scacod);

    #region CRUD
    bool Insert(SCADENZE Model);

    bool Update(SCADENZE Model);

    bool Delete(SCADENZE Model);

    string? Validate(SCADENZE Model, bool IsInsert);
    #endregion
}

public class SCADENZERepository : RepositoryBase, ISCADENZERepository
{
    public SCADENZERepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<SCADENZE>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<SCADENZE>(
                "SELECT * FROM SCADENZE");

            return new ObservableCollection<SCADENZE>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public SCADENZE? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<SCADENZE>(
                "SELECT * FROM SCADENZE WHERE scacod = @id",
                new { id = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public DateTime ComputeExpireMove(SCADENZE? Mover, DateTime StartingExpire)
    {
        if (Mover != null)
        {
            if (Mover.scam01.HasValue && Mover.scam01.Value > 0)
            {
                if (Mover.scam01 == StartingExpire.Month)
                {
                    var dm = TextHelper.ExtractDayMonth(Mover.scas01.ToString());
                    if (dm != null)
                    {
                        int year = StartingExpire.Year;
                        if (dm.Item2 < StartingExpire.Month)
                            year += 1;
                        return new DateTime(year, dm.Item2, dm.Item1);
                    }
                }
            }
            if (Mover.scam02.HasValue && Mover.scam02.Value > 0)
            {
                if (Mover.scam02 == StartingExpire.Month)
                {
                    var dm = TextHelper.ExtractDayMonth(Mover.scas02.ToString());
                    if (dm != null)
                    {
                        int year = StartingExpire.Year;
                        if (dm.Item2 < StartingExpire.Month)
                            year += 1;
                        return new DateTime(year, dm.Item2, dm.Item1);
                    }
                }
            }
            if (Mover.scam03.HasValue && Mover.scam03.Value > 0)
            {
                if (Mover.scam03 == StartingExpire.Month)
                {
                    var dm = TextHelper.ExtractDayMonth(Mover.scas03.ToString());
                    if (dm != null)
                    {
                        int year = StartingExpire.Year;
                        if (dm.Item2 < StartingExpire.Month)
                            year += 1;
                        return new DateTime(year, dm.Item2, dm.Item1);
                    }
                }
            }
            if (Mover.scam04.HasValue && Mover.scam04.Value > 0)
            {
                if (Mover.scam04 == StartingExpire.Month)
                {
                    var dm = TextHelper.ExtractDayMonth(Mover.scas04.ToString());
                    if (dm != null)
                    {
                        int year = StartingExpire.Year;
                        if (dm.Item2 < StartingExpire.Month)
                            year += 1;
                        return new DateTime(year, dm.Item2, dm.Item1);
                    }
                }
            }
            if (Mover.scam05.HasValue && Mover.scam05.Value > 0)
            {
                if (Mover.scam05 == StartingExpire.Month)
                {
                    var dm = TextHelper.ExtractDayMonth(Mover.scas05.ToString());
                    if (dm != null)
                    {
                        int year = StartingExpire.Year;
                        if (dm.Item2 < StartingExpire.Month)
                            year += 1;
                        return new DateTime(year, dm.Item2, dm.Item1);
                    }
                }
            }
            if (Mover.scam06.HasValue && Mover.scam06.Value > 0)
            {
                if (Mover.scam06 == StartingExpire.Month)
                {
                    var dm = TextHelper.ExtractDayMonth(Mover.scas06.ToString());
                    if (dm != null)
                    {
                        int year = StartingExpire.Year;
                        if (dm.Item2 < StartingExpire.Month)
                            year += 1;
                        return new DateTime(year, dm.Item2, dm.Item1);
                    }
                }
            }
            if (Mover.scam07.HasValue && Mover.scam07.Value > 0)
            {
                if (Mover.scam07 == StartingExpire.Month)
                {
                    var dm = TextHelper.ExtractDayMonth(Mover.scas07.ToString());
                    if (dm != null)
                    {
                        int year = StartingExpire.Year;
                        if (dm.Item2 < StartingExpire.Month)
                            year += 1;
                        return new DateTime(year, dm.Item2, dm.Item1);
                    }
                }
            }
            if (Mover.scam08.HasValue && Mover.scam08.Value > 0)
            {
                if (Mover.scam08 == StartingExpire.Month)
                {
                    var dm = TextHelper.ExtractDayMonth(Mover.scas08.ToString());
                    if (dm != null)
                    {
                        int year = StartingExpire.Year;
                        if (dm.Item2 < StartingExpire.Month)
                            year += 1;
                        return new DateTime(year, dm.Item2, dm.Item1);
                    }
                }
            }
            if (Mover.scam09.HasValue && Mover.scam09.Value > 0)
            {
                if (Mover.scam09 == StartingExpire.Month)
                {
                    var dm = TextHelper.ExtractDayMonth(Mover.scas09.ToString());
                    if (dm != null)
                    {
                        int year = StartingExpire.Year;
                        if (dm.Item2 < StartingExpire.Month)
                            year += 1;
                        return new DateTime(year, dm.Item2, dm.Item1);
                    }
                }
            }
            if (Mover.scam10.HasValue && Mover.scam10.Value > 0)
            {
                if (Mover.scam10 == StartingExpire.Month)
                {
                    var dm = TextHelper.ExtractDayMonth(Mover.scas10.ToString());
                    if (dm != null)
                    {
                        int year = StartingExpire.Year;
                        if (dm.Item2 < StartingExpire.Month)
                            year += 1;
                        return new DateTime(year, dm.Item2, dm.Item1);
                    }
                }
            }
            if (Mover.scam11.HasValue && Mover.scam11.Value > 0)
            {
                if (Mover.scam11 == StartingExpire.Month)
                {
                    var dm = TextHelper.ExtractDayMonth(Mover.scas11.ToString());
                    if (dm != null)
                    {
                        int year = StartingExpire.Year;
                        if (dm.Item2 < StartingExpire.Month)
                            year += 1;
                        return new DateTime(year, dm.Item2, dm.Item1);
                    }
                }
            }
            if (Mover.scam12.HasValue && Mover.scam12.Value > 0)
            {
                if (Mover.scam12 == StartingExpire.Month)
                {
                    var dm = TextHelper.ExtractDayMonth(Mover.scas12.ToString());
                    if (dm != null)
                    {
                        int year = StartingExpire.Year;
                        if (dm.Item2 < StartingExpire.Month)
                            year += 1;
                        return new DateTime(year, dm.Item2, dm.Item1);
                    }
                }
            }
        }

        return StartingExpire;
    }

    public bool Exists(string scacod)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM SCADENZE WHERE scacod = @scacod",
                new { scacod = scacod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(SCADENZE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO SCADENZE (scacod,scam01,scas01,scam02,scas02,scam03,scas03,scam04,scas04,scam05,scas05,scam06,scas06,scam07,scas07,scam08,scas08,scam09,scas09,scam10,scas10,scam11,scas11,scam12,scas12) OUTPUT INSERTED.rv VALUES(@scacod,@scam01,@scas01,@scam02,@scas02,@scam03,@scas03,@scam04,@scas04,@scam05,@scas05,@scam06,@scas06,@scam07,@scas07,@scam08,@scas08,@scam09,@scas09,@scam10,@scas10,@scam11,@scas11,@scam12,@scas12)",
                Model);
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

    public bool Update(SCADENZE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE SCADENZE SET scacod = @scacod,scam01 = @scam01,scas01 = @scas01,scam02 = @scam02,scas02 = @scas02,scam03 = @scam03,scas03 = @scas03,scam04 = @scam04,scas04 = @scas04,scam05 = @scam05,scas05 = @scas05,scam06 = @scam06,scas06 = @scas06,scam07 = @scam07,scas07 = @scas07,scam08 = @scam08,scas08 = @scas08,scam09 = @scam09,scas09 = @scas09,scam10 = @scam10,scas10 = @scas10,scam11 = @scam11,scas11 = @scas11,scam12 = @scam12,scas12 = @scas12 OUTPUT INSERTED.rv WHERE scacod = @scacod AND rv = @rv",
                Model);
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

    public bool Delete(SCADENZE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM SCADENZE OUTPUT DELETED.rv WHERE scacod = @scacod AND rv = @rv",
                Model);
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

    public string? Validate(SCADENZE Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.scacod) && IsInsert && !Exists(Model.scacod)) || !IsInsert)
            {
                return null;
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