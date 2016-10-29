namespace CourseTest1
{
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