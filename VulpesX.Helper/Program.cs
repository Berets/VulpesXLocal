using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Data;
using System.Text;
using System.Xml.Linq;
using VulpesX.Shared.Utilities;

while (true)
{
    Console.Clear();
    Console.WriteLine(">>> VulpesX -- Helper\n\n");
    Console.WriteLine("[E] Scaffold DB");
    Console.WriteLine("[S] Scaffold Single Table");
    Console.WriteLine("[C] Encrypt CS");

    var selection = Console.ReadLine();

    if (!string.IsNullOrEmpty(selection))
    {
        switch (selection.ToLower())
        {
            case ("e"):
                Scaffold();
                break;
            case ("s"):
                ScaffoldSingle();
                break;
            case ("c"):
                EncryptCS();
                break;
            case "q":
                Environment.Exit(0);
                break;
            default:
                Environment.Exit(-1);
                break;
        }
    }
}

static void Scaffold()
{
    var cs = "Server=tcp:vulpesx.database.windows.net,1433;Database=Chemie;User ID=azsa@vulpesx.database.windows.net;Password=Pidocchio!2022;Trusted_Connection=False;Encrypt=True;MultipleActiveResultSets=True;";

    Console.Clear();
    Console.WriteLine($"# Scaffold database{Environment.NewLine}{Environment.NewLine}");
    Console.WriteLine("< Percorso dove creare i files: ");
    var path = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
    {
        Console.WriteLine($"> Importazione in corso{Environment.NewLine}");

        using var connection = new SqlConnection(cs);
        try
        {
            connection.Open();
            var schema = connection.GetSchema("Tables");
            foreach (DataRow table in schema.Rows)
            {
                string? tableName = table[2].ToString();
                List<string> exclusions = new List<string>() {
                            "TWORKST", "VAIX", "WEB_OFFERTE", "WEB_OFFERTE_DETTAGLI", "WEB_RATES", "WEB_TEXTS",
                            "VULPERS", "SPEFAX", "PROGRAMMI", "LogApp", "ETICHETTE", "DYNAMIC",
                            "ATTIVO", "Bas_Utenti", "BAS_Ruoli", "BAS_Programmi","BAS_Programmi_Utente", "BAS_Logs",
                             "BAS_Password_Utenti", "BAS_ImpostazioniBase", "BAS_LocazioniSoftware",
                            "BAS_Aziende", "PNCOMMESSESV0910", "VOCIMEN", "BAS_Gruppi", "BAS_Applicativi",
                            "BAS_Applicativi_Utenti", "BAS_AutorizzazioniWeb", "CBICC", "UMITA00F", "TMENU",
                            "TMPANALISI", "TMPBILANCIO", "TMPCICLAV", "TMPCREDITI", "TMPINTCREDITI",
                            "TMPINTDETTCLI", "TMPINTERESSI", "TMPMANDATI", "TMPPRIMA", "TMPPRIMA1", "TMPPRIMP",
                            "TMPRICONC", "TmpSaldiBil", "TmpSaldiBilCee", "TMPSETTORE", "PEFILE", "FatTes01", "FatDet01"
                        };

                if (!string.IsNullOrEmpty(tableName))
                {
                    if (table[3].ToString() == "BASE TABLE" && !exclusions.Contains(tableName))
                    {
                        using var command = new SqlCommand($"select * from {tableName}", connection);
                        Console.WriteLine($"Creazione di {tableName}.cs");
                        StringBuilder sb = new StringBuilder($"namespace VulpesX.Models.Default;{Environment.NewLine}");
                        sb.Append($"using VulpesX.Shared;{Environment.NewLine}");

                        sb.Append($"public partial class {tableName} : Base {Environment.NewLine}{{{Environment.NewLine}");
                        using (var reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
                        {
                            var columnSchema = reader.GetColumnSchema();
                            foreach (var column in columnSchema)
                            {
                                string? dataType = null;

                                if (column != null)
                                {
                                    switch (column.DataTypeName?.Substring(0, 3))
                                    {
                                        case "cha":
                                        case "nch":
                                        case "var":
                                        case "nva":
                                            if (column.DataTypeName != "varbinary")
                                                dataType = "string";
                                            else
                                                dataType = "byte[]";
                                            break;
                                        case "dat":
                                            dataType = "DateTime";
                                            break;
                                        case "bit":
                                            dataType = "bool";
                                            break;
                                        case "int":
                                            dataType = "int";
                                            break;
                                        case "tin":
                                            if (column.DataTypeName == "tinyint")
                                                dataType = "int";
                                            break;
                                        case "sma":
                                            if (column.DataTypeName == "smallint")
                                                dataType = "int";
                                            if (column.DataTypeName == "smallmoney")
                                                dataType = "decimal";
                                            break;
                                        case "big":
                                            dataType = "long";
                                            break;
                                        case "dec":
                                        case "rea":
                                        case "mon":
                                        case "flo":
                                            dataType = "decimal";
                                            break;
                                        case "tim":
                                            dataType = "byte[]";
                                            break;
                                        case "uni":
                                            dataType = "Guid";
                                            break;
                                        default:
                                            dataType = "UNKNOWN";
                                            break;
                                    }

                                    // check nullable
                                    if ((column.AllowDBNull.HasValue && column.AllowDBNull.Value) || dataType == "byte[]")
                                        dataType += "?";

                                    //force null for private only 
                                    string nullForce = string.Empty;
                                    if (column.AllowDBNull.HasValue && !column.AllowDBNull.Value && dataType == "string")
                                        nullForce = $" = null!";

                                    sb.Append($"\tprivate {dataType} _{column.ColumnName}{nullForce};{Environment.NewLine}");

                                    //required only for public
                                    if (column.AllowDBNull.HasValue && !column.AllowDBNull.Value && dataType == "string")
                                        dataType = $"required {dataType}";

                                    sb.Append($"\tpublic {dataType} {column.ColumnName} {{ get => _{column.ColumnName}; set {{ if (_{column.ColumnName} != value) {{ _{column.ColumnName} = value; NotifyPropertyChanged();}} }} }}{Environment.NewLine}");
                                }
                            }
                        }
                        sb.Append($"}}");
                        FileStream fs = new FileStream(path + $@"\{tableName}.cs", FileMode.Create);
                        byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                        fs.Close();
                    }
                }
            }
            Console.WriteLine($"{Environment.NewLine}THE END!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CreateEntities] #ERR# - {ex.Message}");
        }
        finally
        {
            if (connection.State != System.Data.ConnectionState.Closed)
                connection.Close();
        }
    }
    else
    {
        Console.WriteLine("[CreateEntities] #ERR# - Percorso non valido!");
    }

    Console.ReadLine();
}

static void ScaffoldSingle()
{
    var cs = "Server=VULPES;Database=Vulpes;User ID=sa;Password=ufp@SQL23;Trusted_Connection=False;Encrypt=True;MultipleActiveResultSets=True;TrustServerCertificate=True;";

    Console.Clear();
    Console.WriteLine($"# Scaffold database{Environment.NewLine}{Environment.NewLine}");
    Console.WriteLine("< Percorso dove creare il file: ");
    var path = Console.ReadLine();

    Console.WriteLine("< Nome della tabella: ");
    var name = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
    {
        if (!string.IsNullOrEmpty(name))
        {
            Console.WriteLine($"> Importazione in corso{Environment.NewLine}");

            using var connection = new SqlConnection(cs);
            try
            {
                connection.Open();
                var schema = connection.GetSchema("Tables");
                foreach (DataRow table in schema.Rows)
                {
                    string? tableName = table[2].ToString();
                    
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        if (table[3].ToString() == "BASE TABLE" && name == tableName)
                        {
                            using var command = new SqlCommand($"select * from {tableName}", connection);
                            Console.WriteLine($"Creazione di {tableName}.cs");
                            StringBuilder sb = new StringBuilder($"namespace VulpesX.Models.Ufp;{Environment.NewLine}");
                            sb.Append($"using VulpesX.Shared;{Environment.NewLine}");

                            sb.Append($"public partial class {tableName} : Base {Environment.NewLine}{{{Environment.NewLine}");
                            using (var reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
                            {
                                var columnSchema = reader.GetColumnSchema();
                                foreach (var column in columnSchema)
                                {
                                    string? dataType = null;

                                    if (column != null)
                                    {
                                        switch (column.DataTypeName?.Substring(0, 3))
                                        {
                                            case "cha":
                                            case "nch":
                                            case "var":
                                            case "nva":
                                                if (column.DataTypeName != "varbinary")
                                                    dataType = "string";
                                                else
                                                    dataType = "byte[]";
                                                break;
                                            case "dat":
                                                dataType = "DateTime";
                                                break;
                                            case "bit":
                                                dataType = "bool";
                                                break;
                                            case "int":
                                                dataType = "int";
                                                break;
                                            case "tin":
                                                if (column.DataTypeName == "tinyint")
                                                    dataType = "int";
                                                break;
                                            case "sma":
                                                if (column.DataTypeName == "smallint")
                                                    dataType = "int";
                                                if (column.DataTypeName == "smallmoney")
                                                    dataType = "decimal";
                                                break;
                                            case "big":
                                                dataType = "long";
                                                break;
                                            case "dec":
                                            case "rea":
                                            case "mon":
                                            case "flo":
                                                dataType = "decimal";
                                                break;
                                            case "tim":
                                                dataType = "byte[]";
                                                break;
                                            case "uni":
                                                dataType = "Guid";
                                                break;
                                            default:
                                                dataType = "UNKNOWN";
                                                break;
                                        }

                                        // check nullable
                                        if ((column.AllowDBNull.HasValue && column.AllowDBNull.Value) || dataType == "byte[]")
                                            dataType += "?";

                                        //force null for private only 
                                        string nullForce = string.Empty;
                                        if (column.AllowDBNull.HasValue && !column.AllowDBNull.Value && dataType == "string")
                                            nullForce = $" = null!";

                                        sb.Append($"\tprivate {dataType} _{column.ColumnName}{nullForce};{Environment.NewLine}");

                                        //required only for public
                                        if (column.AllowDBNull.HasValue && !column.AllowDBNull.Value && dataType == "string")
                                            dataType = $"required {dataType}";

                                        sb.Append($"\tpublic {dataType} {column.ColumnName} {{ get => _{column.ColumnName}; set {{ if (_{column.ColumnName} != value) {{ _{column.ColumnName} = value; NotifyPropertyChanged();}} }} }}{Environment.NewLine}");
                                    }
                                }
                            }
                            sb.Append($"}}");
                            FileStream fs = new FileStream(path + $@"\{tableName}.cs", FileMode.Create);
                            byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
                            fs.Write(data, 0, data.Length);
                            fs.Flush();
                            fs.Close();
                        }
                    }
                }
                Console.WriteLine($"{Environment.NewLine}THE END!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CreateEntities] #ERR# - {ex.Message}");
            }
            finally
            {
                if (connection.State != System.Data.ConnectionState.Closed)
                    connection.Close();
            }
        }
    }
    else
    {
        Console.WriteLine("[CreateEntities] #ERR# - Percorso non valido!");
    }

    Console.ReadLine();
}

static void EncryptCS()
{
    string pk = "pOli";
    string? kp = TextHelper.ReverseAndScrumble(pk);

    string cs = "Server=XPS;Database=Baruffaldi;User ID=sa;Password=mozart!8;Trusted_Connection=False;Encrypt=True;MultipleActiveResultSets=True;TrustServerCertificate=True;";
    byte[] PKData = Encoding.UTF8.GetBytes(pk + kp);

    Console.WriteLine(CryptoHelper.CSEncrypt(cs, PKData, "@baruffaldisrl.it"));
    Console.ReadKey();
}