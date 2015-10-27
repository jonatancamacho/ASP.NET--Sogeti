using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace korjornalen.Utils
{
    public abstract class ErrorLogger
    {
        private static readonly string LogFileName = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "Logs\\" + "error-log.txt";

        /// <summary>
        /// Writes the information from an exception to App_Data/Logs/error-log.txt, prepended by the date and time of execution.
        /// </summary>
        /// <param name="exception"></param>
        public static void LogException(Exception exception)
        {
            ErrorLogger.LogString(DateTime.Now.ToString() + ": " + exception.Message + " " + exception.InnerException);
        }

        /// <summary>
        /// Writes the specified message to App_Data/Logs/error-log.txt, prepended by the date and time of execution.
        /// </summary>
        /// <param name="message"></param>
        public static void LogString(string message)
        {
            var sw = new System.IO.StreamWriter(LogFileName, true);
            sw.WriteLine(DateTime.Now.ToString() + ": " + message);
            sw.Close();
        }
    }
}