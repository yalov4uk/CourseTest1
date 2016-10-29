using System.IO;

namespace CourseTest1
{
    class FileWriter : IWriter
    {
        public void Write(string path, string s)
        {
            if (!Directory.Exists(@"C:\logger"))
                Directory.CreateDirectory(@"C:\logger");
            File.WriteAllText(path, s);
        }
    }
}