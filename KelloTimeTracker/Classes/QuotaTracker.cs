using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KelloTimetracker.Classes
{
    class QuotaTracker
    {
        public const string QuotaFile = "currentquota.txt";
        // 30 hours * 60 minutes * 60 seconds
        public static int TotalSeconds = 30 * 60 * 60;

        public string m_currentYear = "0";        
        public string m_currentMonth = "Jan";
        public string m_currentWeek = "1";

        public int m_elapsedSeconds;       

        public TimeSpan Remaining
        {
            get
            {
                int remainingSeconds = TotalSeconds - m_elapsedSeconds;
                
                TimeSpan remaining = TimeSpan.FromSeconds(remainingSeconds);
                return remaining;
            }
        }

        public QuotaTracker()
        {
            m_elapsedSeconds = 0;

            LoadQuotaFromFile();     

            DateTime now = DateTime.Now;
            m_currentYear = now.Year.ToString();
            m_currentMonth = now.ToString("MMM");
            m_currentWeek = GetIso8601WeekOfYear(now).ToString();

            if (m_currentMonth.EndsWith("."))
            {
                m_currentMonth = m_currentMonth.Substring(0, m_currentMonth.Length - 1);
            }

            SearchRemainingTime();
        }

        public void EchoRemainingTime()
        {
            Console.WriteLine("Time left: " + ElapsedResult.GetTotalHHMM( Remaining ));
        }        

        public string GetYearFolder()
        {
            return m_currentYear.ToString();
        }

        public string GetWeekFolder()
        {            
            return m_currentYear + "/" + m_currentMonth + "." + m_currentWeek;
        }        

        /// <summary>
        /// Loads the quota from file if the quota file exists, otherwise default is 30h
        /// </summary>
        public void LoadQuotaFromFile()
        {
            try
            {
                if (File.Exists(QuotaFile))
                {
                    using (StreamReader sr = new StreamReader(QuotaFile))
                    {
                        TotalSeconds = int.Parse(sr.ReadLine()) * 60 * 60;
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch(Exception e)
            {
                TotalSeconds = 30 * 60 * 60;
            }
            
        }

        public void SearchRemainingTime()
        {
            var folder = GetWeekFolder();

            if (Directory.Exists( folder ))
            {
                string[] files = Directory.GetFiles(folder);

                foreach (string file in files)
                {
                    HandleFile(file);
                }
            }
            else
            {
                m_elapsedSeconds = 0;
            }
        }

        

        public void UpdateElapsed(TimeSpan a_elapsed)
        {
            m_elapsedSeconds += (int)a_elapsed.TotalSeconds;

            EchoRemainingTime();
        }

        private void HandleFile(string a_file)
        {           
            using (StreamReader sr = new StreamReader(a_file))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();                   

                    string[] parts = line.Split(' ');

                    if (parts.Length > 1)
                    {
                        if (parts[0] == "duration:")
                        {
                            string[] timeParts = parts[1].Split(':');
                            int hours = int.Parse(timeParts[0]);
                            int minutes = int.Parse(timeParts[1]);
                            int seconds = int.Parse(timeParts[2]);

                            m_elapsedSeconds += hours * 3600;
                            m_elapsedSeconds += minutes * 60;
                            m_elapsedSeconds += seconds;                            
                        }
                    }                   
                }                
            }
        }


        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}
