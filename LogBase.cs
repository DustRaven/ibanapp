using System;
using System.Globalization;
using System.IO;

namespace ibanapp
{
    public abstract class LogBase
    {
        protected readonly object LockObj = new object();
        public abstract void Log(string message);
    }

    public class FileLogger : LogBase
    {
        public string filePath = "Log.txt";
        public override void Log(string message)
        {
            lock (LockObj)
            {
                using (StreamWriter streamWriter = new StreamWriter(filePath, true))
                {
                    message = DateTime.Now.ToString(new CultureInfo("de_DE")) + " | " + message;
                    streamWriter.WriteLine(message);
                    streamWriter.Close();
                }
            }
        }
    }
}
