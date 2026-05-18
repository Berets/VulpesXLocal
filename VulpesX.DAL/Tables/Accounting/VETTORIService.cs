using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.Tables.Accounting;

public interface IVETTORIRepository
{
    ObservableCollection<VETTORI>? GetList();

    VETTORI? Get(int vetcod);

    bool Exists(int vetcod);

    ObservableCollection<VETTORISPESE1>? GetExpenses(int vetcod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    string INSERT_QUERY_EXPENSES { get; }
    bool Insert(VETTORI Model);

    bool Insert(VETTORI Model, ObservableCollection<VETTORISPESE1>? Expenses);

    bool Update(VETTORI Model);

    bool Update(VETTORI Model, ObservableCollection<VETTORISPESE1>? Expenses);

    bool Delete(VETTORI Model);

    string? Validate(VETTORI Model, bool IsInsert);

    string? ValidateExpenses(List<VETTORISPESE1>? Expenses, VETTORISPESE1 Expense);
    #endregion
}

public class VETTORIRepository : RepositoryBase, IVETTORIRepository
{
    public VETTORIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<VETTORI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<VETTORI, ABE, VETTORI>(
                @"SELECT v.*, a.abers1 FROM VETTORI AS v
                        LEFT OUTER JOIN ABE AS a ON a.abecod = v.vetfor",
                (vet, abe) =>
                {
                    if (abe != null)
                        vet.SupplierFullDescription = $"{vet.vetfor} {abe.abers1}";
                    else
                        vet.SupplierFullDescription = "Nessun fornitore collegato";
                    return vet;
                },
                splitOn: "abers1");

            return new ObservableCollection<VETTORI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public VETTORI? Get(int vetcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<VETTORI>(
                "SELECT * FROM VETTORI WHERE vetcod = @vetcod",
                new { vetcod = vetcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int vetcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM VETTORI WHERE vetcod = @vetcod",
                new { vetcod = vetcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public ObservableCollection<VETTORISPESE1>? GetExpenses(int vetcod)
    {
        throw new NotImplementedException();
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO VETTORI (vetcod,vetdes,vetind,vetloc,vetpro,vetcap,vetfor,vetcalb,vetpiva) OUTPUT INSERTED.rv VALUES(@vetcod,@vetdes,@vetind,@vetloc,@vetpro,@vetcap,@vetfor,@vetcalb,@vetpiva)";
    public string UPDATE_QUERY => "UPDATE VETTORI SET vetcod = @vetcod,vetdes = @vetdes,vetind = @vetind,vetloc = @vetloc,vetpro = @vetpro,vetcap = @vetcap,vetfor = @vetfor,vetcalb = @vetcalb,vetpiva = @vetpiva OUTPUT INSERTED.rv WHERE vetcod = @vetcod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM VETTORI OUTPUT DELETED.rv WHERE vetcod = @vetcod AND rv = @rv";

    public string INSERT_QUERY_EXPENSES => string.Empty;

    public bool Insert(VETTORI Model)
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

    public bool Insert(VETTORI Model, ObservableCollection<VETTORISPESE1>? Expenses)
    {
        throw new NotImplementedException();
    }

    public bool Update(VETTORI Model)
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

    public bool Update(VETTORI Model, ObservableCollection<VETTORISPESE1>? Expenses)
    {
        throw new NotImplementedException();
    }

    public bool Delete(VETTORI Model)
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

    public string? Validate(VETTORI Model, bool IsInsert)
    {
        try
        {
            if ((Model.vetcod > 0 && IsInsert && !Exists(Model.vetcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.vetdes))
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

    public string? ValidateExpenses(List<VETTORISPESE1>? Expenses, VETTORISPESE1 Expense)
    {
        throw new NotImplementedException();
    }
    #endregion
}

public class VETTORIUfpRepository : RepositoryBase, IVETTORIRepository
{
    public VETTORIUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<VETTORI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<VETTORI>(
                $@"SELECT v.vetcod as vetcod,
                            v.vetdes as vetdes,
v.vetind as vetind,
v.vetloc as vetloc, 
v.vetpro as vetpro,
v.vetpca as vetpca,
v.vetpiva as vetpivadec,
v.vetcalb as vetcalb
                        FROM VETTORI AS v");

            return new ObservableCollection<VETTORI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public VETTORI? Get(int vetcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<VETTORI>(
                "SELECT * FROM VETTORI WHERE vetcod = @vetcod",
                new { vetcod = vetcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int vetcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM VETTORI WHERE vetcod = @vetcod",
                new { vetcod = vetcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public ObservableCollection<VETTORISPESE1>? GetExpenses(int vetcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<VETTORISPESE1>(
                "SELECT * FROM VETTORISPESE1 WHERE vetcod = @vetcod",
                new { vetcod = vetcod });

            return new ObservableCollection<VETTORISPESE1>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO VETTORI (vetcod,vetdes,vetind,vetloc,vetpro,vetcap,vetcalb,vetpiva) OUTPUT INSERTED.rv VALUES(@vetcod,@vetdes,@vetind,@vetloc,@vetpro,@vetcap,,@vetcalb,@vetpiva)";
    public string UPDATE_QUERY => "UPDATE VETTORI SET vetcod = @vetcod,vetdes = @vetdes,vetind = @vetind,vetloc = @vetloc,vetpro = @vetpro,vetcap = @vetcap,vetcalb = @vetcalb,vetpiva = @vetpiva OUTPUT INSERTED.rv WHERE vetcod = @vetcod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM VETTORI OUTPUT DELETED.rv WHERE vetcod = @vetcod AND rv = @rv";

    public string INSERT_QUERY_EXPENSES => "INSERT INTO VETTORISPESE1 (vetcod, vetiso,Vetapes, vetpes, vetpre, vetcos) VALUES(@vetcod, @vetiso,@Vetapes, @vetpes, @vetpre, @vetcos)";

    public bool Insert(VETTORI Model)
    {
        throw new NotImplementedException();
    }

    public bool Insert(VETTORI Model, ObservableCollection<VETTORISPESE1>? Expenses)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // clean expenses
                    connection.Execute("DELETE FROM VETTORISPESE1 WHERE vetcod=@vetcod",
                        new { vetcod = Model.vetcod }, transaction);

                    foreach (var group in Expenses ?? new ObservableCollection<VETTORISPESE1>())
                    {
                        connection.Execute(INSERT_QUERY_EXPENSES, group, transaction);
                    }

                    var result = connection.Execute(INSERT_QUERY, Model, transaction);
                    transaction.Commit();

                    return true;
                }
                catch
                {
                    transaction.Rollback();
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

    public bool Update(VETTORI Model)
    {
        throw new NotImplementedException();
    }

    public bool Update(VETTORI Model, ObservableCollection<VETTORISPESE1>? Expenses)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // clean expenses
                    connection.Execute("DELETE FROM VETTORISPESE1 WHERE vetcod=@vetcod",
                        new { vetcod = Model.vetcod }, transaction);

                    foreach (var group in Expenses ?? new ObservableCollection<VETTORISPESE1>())
                    {
                        connection.Execute(INSERT_QUERY_EXPENSES, group, transaction);
                    }

                    var result = connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);
                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message);
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

    public bool Delete(VETTORI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // clean expenses
                    connection.Execute("DELETE FROM VETTORISPESE1 WHERE vetcod=@vetcod",
                        new { vetcod = Model.vetcod }, transaction);

                    var result = connection.Execute(DELETE_QUERY, Model);
                    transaction.Commit();

                    return true;
                }
                catch
                {
                    transaction.Rollback();
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

    public string? Validate(VETTORI Model, bool IsInsert)
    {
        try
        {
            if ((Model.vetcod > 0 && IsInsert && !Exists(Model.vetcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.vetdes))
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

    public string? ValidateExpenses(List<VETTORISPESE1>? Expenses, VETTORISPESE1 Expense)
    {
        try
        {
            if (Expense.SelectedISO == null)
            {
                return "Codico ISO obbligatorio";
            }

            if (Expense.Vetapes == null || Expense.vetpes == null)
            {
                return "Da peso e A peso obbligatori";
            }

            if (Expense.IsInsert)
            {
                var exist = (Expenses ?? new List<VETTORISPESE1>()).Where(o => o.vetiso == Expense.SelectedISO?.isocod && o.Vetapes == Expense.Vetapes && o.vetpes == Expense.vetpes).Count() > 0;
                if (exist)
                {
                    return $"Spesa già esistente";
                }
            }
            else
            {
                var exist = (Expenses ?? new List<VETTORISPESE1>()).Where(o => o.vetiso == Expense.SelectedISO?.isocod && o.Vetapes == Expense.Vetapes && o.vetpes == Expense.vetpes).Count() > 1;
                if (exist)
                {
                    return $"Spesa già esistente";
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}