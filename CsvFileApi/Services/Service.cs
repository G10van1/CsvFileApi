namespace CsvFileApi.Services
{
    public class Service
    {
        public static string QueryApi2Linq(string input)
        {
            string[] separators = { "+", "|" };
            string[] splited = input.Split(separators, StringSplitOptions.None);
            string queryLinq = "";
            foreach (string x in splited)
            {
                var field = x.Split(":", StringSplitOptions.None);
                int index = input.IndexOf(x);
                if (index == 0)
                {
                    queryLinq += $"u => u.{field[0]}.Contains(\"{field[1]}\", System.StringComparison.OrdinalIgnoreCase)";
                }
                else
                {
                    queryLinq += input[index - 1] == '+' ? " && " : " || ";
                    queryLinq += $"u.{field[0]}.Contains(\"{field[1]}\", System.StringComparison.OrdinalIgnoreCase)";
                }
            }

            return queryLinq;
        }
    }
}
