using System;
// ReSharper disable InconsistentNaming

namespace NICE
{
    public static class Log
    {
        private static Level show_level = Level.TRACE;
        
        private static void Write(string hostname, Level level, string message, ConsoleColor color)
        {
            if (show_level > level)
            {
                return;
            }
            
            Console.Write($"[ {level.ToString().PadRight(10).Substring(0,5)} | {hostname} ] ");
            Console.ForegroundColor = color;
            Console.Write($"{message}\n");
            Console.ResetColor();
        }

        public enum Level
        {
            TRACE = 0,
            DEBUG = 1,
            INFO = 2,
            WARN = 3,
            ERROR = 4,
            FATAL = 5
        }

        public static void Trace(string hostname, string message)
        {
            Write(hostname, Level.TRACE, message, ConsoleColor.Cyan);
        }
        
        public static void Debug(string hostname, string message)
        {
            Write(hostname, Level.DEBUG, message, ConsoleColor.Blue);
        }
        
        public static void Info(string hostname, string message)
        {
            Write(hostname, Level.INFO, message, ConsoleColor.Green);
        }
        
        public static void Warn(string hostname, Exception e)
        {
            Write(hostname, Level.WARN, e.Message, ConsoleColor.DarkYellow);
        }
        
        public static void Error(string hostname, Exception e)
        {
            Write(hostname, Level.ERROR, e.Message, ConsoleColor.Red);
        }
        
        public static void Fatal(string hostname, Exception e)
        {
            Write(hostname, Level.FATAL, e.Message, ConsoleColor.DarkRed);
        }
        
        public static void Warn(string hostname, string message)
        {
            Write(hostname, Level.WARN, message, ConsoleColor.DarkYellow);
        }
        
        public static void Error(string hostname, string message)
        {
            Write(hostname, Level.ERROR, message, ConsoleColor.Red);
        }
        
        public static void Fatal(string hostname, string message)
        {
            Write(hostname, Level.FATAL, message, ConsoleColor.DarkRed);
        }

        public static void SetLevel(Level loglevel)
        {
            show_level = loglevel;
        }
    }
}