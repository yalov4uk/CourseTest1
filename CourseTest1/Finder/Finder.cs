namespace CourseTest1
{
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
}