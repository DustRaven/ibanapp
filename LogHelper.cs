namespace ibanapp
{
    public static class LogHelper
    {

        private static LogBase logger = null;

        public static void Log(string message)
        {
            logger = new FileLogger();
            logger.Log(message);
        }
    }
}
