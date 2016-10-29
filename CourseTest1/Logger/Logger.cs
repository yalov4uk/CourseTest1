namespace CourseTest1
{
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
}