using Dapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Shared.Generics;

namespace VulpesX.Shared.Utilities
{
    public static class TelerikGridService
    {
        public record FilterEntry(string ID, string AliasId, string Operator, string Value);

        public static string? ComputeSort(List<GenericIDDescription> SortList, List<GenericIDDescriptionType> AliasList)
        {
            if (SortList == null || SortList.Count == 0)
                return null;

            var sb = new StringBuilder("ORDER BY ");
            foreach (var sort in SortList.Where(o => !string.IsNullOrEmpty(o.Description)))
            {
                var alias = AliasList.First(w => w.ID == sort.ID);
                var columnExpr = ResolveColumnExpression(alias);
                sb.Append($"{columnExpr} {sort.Description},");
            }
            return sb.ToString().Substring(0, sb.Length - 1);
        }

        private static string ResolveColumnExpression(GenericIDDescriptionType alias)
        {
            if (alias.Type?.StartsWith("#COMPOSED#") == true)
            {
                var parts = alias.Type.Replace("#COMPOSED#", "").Split('#');
                var col1 = parts[0];
                var col2 = parts[1];
                return $"(RTRIM(CAST({col1} AS NVARCHAR(255))) + ' - ' + {col2})";
            }

            // For all other types, Description is the plain column name
            return alias.Description!;
        }

        public static void ComputeFilter(StringBuilder sb, List<FilterEntry> filters, List<GenericIDDescriptionType> aliases, DynamicParameters args)
        {
            if (filters is not { Count: > 0 }) return;

            foreach (var filter in filters)
            {
                var alias = aliases.First(a => a.ID == filter.AliasId);

                if (alias.Type?.StartsWith("#COMPOSED#") == true)
                    AppendComposedFilterWithArgs(sb, args, alias, filter);
                else if (alias.Type?.StartsWith("#BF#") == true)
                    AppendBooleanFlagFilter(sb, alias, filter);
                else
                    AppendStandardFilter(sb, args, alias, filter);
            }
        }

        private static void AppendComposedFilter(StringBuilder sb, string composedType, string op, string paramBaseName)
        {
            // Extract the two column names from "#COMPOSED#col1#col2"
            var parts = composedType.Replace("#COMPOSED#", "").Split('#');
            var col1 = parts[0]; // tipomp.tmpcod
            var col2 = parts[1]; // tipomp.tmpdes

            // Reconstruct the same expression your C# property uses
            var sqlExpr = $"(RTRIM(CAST({col1} AS NVARCHAR(255))) + ' - ' + {col2})";

            var isNullOp = op == "IS NULL" || op == "IS NOT NULL";
            var paramRef = isNullOp ? "" : $"@{paramBaseName}";

            sb.Append($" AND {sqlExpr} {op} {paramRef} ");
        }

        private static bool IsNullCheck(string op) => op is "IS NULL" or "IS NOT NULL";

        private static string NormaliseOperator(string op) => op switch
        {
            "LIKE" => "=",
            "NOT LIKE" => "<>",
            _ => op
        };

        private static string ParamName(string? description) => description?.Replace(".", "_") ?? string.Empty;

        private static string LikeWrap(string op, string value) => op is "LIKE" or "NOT LIKE" ? $"%{value}%" : value;

        private static DateTime ParseItDate(string value) => DateTime.Parse(value, new CultureInfo("it-IT").DateTimeFormat);

        private static void AppendComposedFilterWithArgs(StringBuilder sb, DynamicParameters args, GenericIDDescriptionType alias, FilterEntry filter)
        {
            var paramName = ParamName(alias.Description);
            AppendComposedFilter(sb, alias.Type!, filter.Operator, paramName);

            if (!IsNullCheck(filter.Operator))
                args.Add(paramName, LikeWrap(filter.Operator, filter.Value));
        }

        private static void AppendBooleanFlagFilter(StringBuilder sb, GenericIDDescriptionType alias, FilterEntry filter)
        {
            // Type format: "#BF#<sqlWhenTrue>#<sqlWhenFalse>"
            var body = alias.Type!.Replace("#BF#", string.Empty);
            var sepIdx = body.IndexOf('#');
            var whenTrue = body[..sepIdx];
            var whenFalse = body[(sepIdx + 1)..];

            sb.Append($" AND {(filter.Value == "True" ? whenTrue : whenFalse)}");
        }

        private static void AppendStandardFilter(StringBuilder sb, DynamicParameters args, GenericIDDescriptionType alias, FilterEntry filter)
        {
            var op = filter.Operator;
            var paramName = UniqueParamName(args, ParamName(alias.Description));
            var paramRef = IsNullCheck(op) ? string.Empty : $" @{paramName} ";

            string columnExpr = alias.Type switch
            {
                "D" => $"CAST({alias.Description} AS date)",
                "DT" => $"CONVERT(DATETIME, CONVERT(VARCHAR(20), {alias.Description}, 120))",
                var t when t?.StartsWith("#C#") == true => t.Replace("#C#", string.Empty),
                _ => alias.Description ?? string.Empty
            };

            string resolvedOp = alias.Type is "D" or "DT"
                ? NormaliseOperator(op)
                : op;

            sb.Append($" AND {columnExpr} {resolvedOp}{paramRef}");

            if (IsNullCheck(op)) return;

            object value = alias.Type switch
            {
                "D" or "DT" => ParseItDate(filter.Value),
                "N" or "M" => decimal.Parse(filter.Value.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture),
                var t when t?.StartsWith("#B#") == true => BoolToDbValue(t, filter.Value),
                _ => LikeWrap(op, filter.Value)
            };

            args.Add(paramName, value);
        }

        private static string BoolToDbValue(string aliasType, string value)
        {
            var tokens = aliasType.Replace("#B#", string.Empty).Split('/');
            return value == "True" ? tokens[0] : tokens[1];
        }

        private static string UniqueParamName(DynamicParameters args, string baseName)
        {
            var name = ParamName(baseName);
            if (!args.ParameterNames.Contains(name))
                return name;

            int i = 1;
            string candidate;
            do { candidate = $"{name}_{i++}"; }
            while (args.ParameterNames.Contains(candidate));

            return candidate;
        }
    }
}
