namespace CourseTest1
{
    interface IFind
    {
        string login { get; set; }
        string password { get; set; }
        void Find(string logs);
    }
}