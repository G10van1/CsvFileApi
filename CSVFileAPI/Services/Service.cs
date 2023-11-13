namespace CsvFileApi.Services
{
    public class Service
    {
        public static string QueryApi2Linq(string input)
        {
            string[] separators = { "+", "|" };
            string[] splited = input.Split(separators, StringSplitOptions.None);
            string queryLinq = "u => ";
            foreach (string x in splited)
            {
                var field = x.Split(":", StringSplitOptions.None);
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


    }
}
