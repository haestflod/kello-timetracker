using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KelloTimetracker
{
    class Program
    {
        private static Classes.Timetracker timeTracker;

        private static Classes.QuotaTracker quotaTracker;

        private static Classes.AsciiKello clock;

        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected

        // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);

        static void Main(string[] args)
        {            
            quotaTracker = new Classes.QuotaTracker();
            timeTracker = new Classes.Timetracker(quotaTracker);
            clock = new Classes.AsciiKello(timeTracker);            

            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            PrintInstructions();

            while (true)
            {
                string line = Console.ReadLine();
                
                if (line == "s" || line == "e")
                {
                    if (!timeTracker.IsRunning())
                    {
                        timeTracker.Start();
                        clock.Start();
                    }
                    else
                    {
                        timeTracker.Stop();
                        clock.Stop();
                    }                   
                }               
                else if (line == "c")
                {
                    Console.Clear();
                    clock.Redraw();
                    if (timeTracker.m_isRunning)
                    {
                        timeTracker.PrintStart();
                    }
                    else
                    {
                        PrintInstructions();
                    }
                    
                }
                else if (line == "x")
                {
                    break;
                }
                else if (line.StartsWith("u"))
                {
                    Classes.UpworkTimePrinter.PrintTimes(line);
                }
                else
                {
                    Console.WriteLine("Unknown command: " + line );
                }

                Console.WriteLine();
            }

        }

        public static void PrintInstructions()
        {
            quotaTracker.EchoRemainingTime();

            Console.WriteLine("'s' to start tracking");
            Console.WriteLine("'e' to stop tracking");
            Console.WriteLine("'x' for exit");
            Console.WriteLine("'c' clear console'");
            Console.WriteLine("'u' for upwork time");
        }

        /// <summary>
        /// Write a console message with a specific color
        /// </summary>
        /// <param name="a_message"></param>
        /// <param name="a_color"></param>
        public static void WriteLineMessage(string a_message, ConsoleColor a_color)
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            Console.ForegroundColor = a_color;
            Console.WriteLine(a_message);

            Console.ForegroundColor = originalColor;
        }
        
        public static void WriteMessage(string a_message, ConsoleColor a_color)
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            Console.ForegroundColor = a_color;
            Console.Write(a_message);

            Console.ForegroundColor = originalColor;
        }

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                if (timeTracker != null)
                {
                    if (timeTracker.IsRunning())
                    {
                        timeTracker.Stop();
                    }                    
                }
            }
            return false;
        }
       
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    }
}
