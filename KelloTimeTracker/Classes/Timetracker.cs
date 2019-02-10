using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KelloTimetracker.Classes
{
    class Timetracker
    {
        public DateTime m_startTime;

        public bool m_isRunning = false;

        public QuotaTracker m_quotaTracker;

        public Timetracker(QuotaTracker a_quoteTracker)
        {
            m_quotaTracker = a_quoteTracker;
        }   

        public bool IsRunning()
        {
            return m_isRunning;            
        }

        public void LogCurrentSession()
        {
            if (IsRunning())
            {
                DateTime currentTime = DateTime.Now;
                TimeSpan elapsedTime = currentTime - m_startTime;

                ElapsedResult result = new ElapsedResult(elapsedTime);                

                string output = String.Format("Worked: {0}", result.ToString());

                Console.WriteLine(output);
            }
            else
            {
                Console.WriteLine("Not tracking any time");
            }
        }

        public void Start()
        {
            if (IsRunning())
            {
                Stop();
            }

            m_startTime = DateTime.Now;
            m_isRunning = true;

            PrintStart();
        }

        public void Stop()
        {
            if (IsRunning())
            {
                DateTime ended = DateTime.Now;

                TimeSpan elapsedTime = ended - m_startTime;

                m_quotaTracker.UpdateElapsed(elapsedTime);
                               
                ElapsedResult result = new ElapsedResult(elapsedTime);

                LogToFile(result);

                m_isRunning = false;
                Console.WriteLine("Duration: " + result.ToString());
                Console.WriteLine("Stopped time at: " + ended.ToString("HH:mm"));
            }
            else
            {
                Console.WriteLine("Timetracking was not started");
            }
        }

        public void PrintStart()
        {
            Console.WriteLine("Started tracking time: " + m_startTime.ToString("HH:mm"));
        }

        public void LogToFile(ElapsedResult a_result)
        {
            if (IsRunning() )
            {                

                string year = m_startTime.Year.ToString();
                string month = m_startTime.Month.ToString();
                string day = m_startTime.Day.ToString();

                string yearFolder = m_quotaTracker.GetYearFolder();
                string folder = m_quotaTracker.GetWeekFolder();                 

                string filename = folder + "/" + day + ".txt";

                if (!Directory.Exists(yearFolder))
                {
                    Directory.CreateDirectory(yearFolder);
                }

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                using (StreamWriter tw = new StreamWriter(filename, true, Encoding.UTF8))
                {
                    tw.WriteLine("start: " + m_startTime.ToString("HH:mm:ss"));
                    tw.WriteLine("duration: " + a_result.ToString());
                    tw.WriteLine();
                }
            }            
        }
    }
}
