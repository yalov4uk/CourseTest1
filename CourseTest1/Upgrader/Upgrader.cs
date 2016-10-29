namespace CourseTest1
{
    class Upgrader
    {
        public IUpgrade upgrader;
        public Upgrader(IUpgrade upgrader)
        {
            this.upgrader = upgrader;
        }
        public string Upgrade(string sym)
        {
            return upgrader.Upgrade(sym);
        }
    }
}