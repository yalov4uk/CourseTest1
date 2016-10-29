using System.Text;
using System.Text.RegularExpressions;

namespace CourseTest1
{
    class SkypeFind : IFind
    {
        public string login { get; set; }
        public string password { get; set; }
        public void Find(string logs)
        {
            int start = logs.IndexOf(" - ") + 3, end = logs.Length;
            login = logs.Substring(start, end - start);

            string pattern = "mark[A-Za-z0-9]{8,200}mark";
            Regex repas = new Regex(pattern, RegexOptions.Compiled);
            foreach (Match match in repas.Matches(logs))
            {
                StringBuilder temp = new StringBuilder(match.Value);
                temp.Replace("mark", "");
                if (temp.Length > 7)
                    password = temp.ToString();
            }
        }
    }
}