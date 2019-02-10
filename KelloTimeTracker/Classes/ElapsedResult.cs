using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KelloTimetracker.Classes
{
    class ElapsedResult
    {
        public string hours;
        public string minutes;
        public string seconds;

        public ElapsedResult(TimeSpan a_elapsed)
        {
            int minutesElapsed = (int)a_elapsed.Minutes;
            int secondsElapsed = (int)a_elapsed.Seconds;

            hours = ((int)a_elapsed.Hours).ToString();
            minutes = minutesElapsed.ToString();
            seconds = secondsElapsed.ToString();
            if (minutesElapsed < 10)
            {
                minutes = "0" + minutes;
            }
            if (secondsElapsed < 10)
            {
                seconds = "0" + seconds;
            }
        }

        public string GetHHMM()
        {
            return String.Format("{0}:{1}", hours, minutes);
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}:{2}", hours, minutes, seconds);
        }

        public static string GetTotalHHMM(TimeSpan a_elapsed)
        {
            int minutesElapsed = (int)a_elapsed.Minutes;
            int secondsElapsed = (int)a_elapsed.Seconds;

            string hours = ((int)a_elapsed.TotalHours).ToString();
            string minutes = minutesElapsed.ToString();
            
            if (minutesElapsed < 10)
            {
                minutes = "0" + minutes;
            }
            
            return String.Format("{0}:{1}", hours, minutes);
        }
    }
}
