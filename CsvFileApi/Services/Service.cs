namespace CsvFileApi.Services
{
    public class Service
    {
        public static string QueryApi2Linq(string input, out int status, out string message)
        {
            status = 200;
            message = "Successful!";
            string[] separators = { "+", "|" };

            if (!QueryValidation(input, separators, out message))
            {
                status = 400;
                return message;
            }

            string[] splited = input.Split(separators, StringSplitOptions.None);
            string queryLinq = "u => ";

            foreach (string x in splited)
            {
                if (x.Count(c => c == ':') != 1)
                {
                    status = 400;
                    message = "Invalid query format, must have only one ':' in each subquery";
                    return message;
                }

                var field = x.Split(":", StringSplitOptions.None);

                if (!FieldValidation(field[0]))
                {
                    status = 400;
                    message = $"Invalid field name: '{field[0]}'";
                    return message;
                }

                int index = input.IndexOf(x);
                
                if (index != 0)
                    queryLinq += input[index - 1] == '+' ? " && " : " || ";
                
                queryLinq += SplitSubQuery(field[0], field[1]);
            }
            return queryLinq;
        }

        private static string SplitSubQuery(string field, string value)
        {
            string subQuery = "";

            if (!value.Contains("*"))
                return $"string.Equals(u.{field}, \"{value}\", StringComparison.OrdinalIgnoreCase)";
            
            string[] splited = value.Split("*", StringSplitOptions.RemoveEmptyEntries);

            for(int i = 0; i < splited.Length; i++)
            {
                if (i == 0)
                {
                    if (splited.Length > 1)
                        subQuery = "(";
                }
                else
                    subQuery += " && ";

                int index = value.IndexOf(splited[i]);

                if (index == 0)
                {
                    subQuery += $"u.{field}.StartsWith(\"{splited[i]}\", System.StringComparison.OrdinalIgnoreCase)";
                }
                else if (index + splited[i].Length == value.Length)
                {
                    subQuery += $"u.{field}.EndsWith(\"{splited[i]}\", System.StringComparison.OrdinalIgnoreCase)";
                }
                else
                    subQuery += $"u.{field}.Contains(\"{splited[i]}\", System.StringComparison.OrdinalIgnoreCase)";
                
                if (i == splited.Length - 1 && splited.Length > 1)
                    subQuery += ")";
            }

            return subQuery;
        }
        private static bool FieldValidation(string field)
        {
            return new[] { "name", "city", "country", "favorite_sport" }.Contains(field.TrimEnd().TrimStart());
        }

        private static bool QueryValidation(string query, string[] separators, out string message)
        {
            message = "Successful!";
            if (query.Contains(separators[0]) && query.Contains(separators[1]))
            {
                message = "It is not allowed to mix '+' and '|' in the same query";
                return false;
            }

            if (!query.Contains(":"))
            {
                message = "It must have at least a ':'";
                return false;
            }
            
            int numSeparators = query.Count(c => c == separators[0][0] || c == separators[1][0]);
            int numFieldSeparator = query.Count(c => c == ':') - 1;
            if (numSeparators != numFieldSeparator)
            {
                message = "Invalid query format";
                return false;
            }

            if (query.Contains("++") || query.Contains("::") || query.Contains("||")
                || query.Contains("**") || query.Contains(":+") || query.Contains("+:")
                || query.Contains(":|") || query.Contains("|:"))
            {
                message = "Invalid query format";
                return false;
            }

            if ("+|:".Contains(query[0]) || "+|:".Contains(query[^1]) || query[^2..] == ":*")
            {
                message = "Invalid query format";
                return false;
            }

            return true;
        }
    }
}
