using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KelloTimetracker.Classes
{
    /// <summary>
    /// An upwork entry example:
    /// 14:20 -> 15.30
    /// </summary>
    class UpworkTimeEntry
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        /// <summary>
        /// Duration without rest time
        /// </summary>
        public double DurationSeconds { get; set; }

        /// <summary>
        /// The amount of rest after Start & End
        /// </summary>
        public double RestSeconds { get; set; }

        public UpworkTimeEntry(DateTime a_start, TimeSpan a_duration)
        {
            Start = a_start;
            DurationSeconds = a_duration.TotalSeconds;
            // 1.2 hours => 1 hour e.t.c.
            int periodHours = (int)(DurationSeconds / 3600);
            double periodSecondsDouble = (DurationSeconds - (periodHours * 3600));

            RestSeconds = periodSecondsDouble % 600;

            DurationSeconds -= RestSeconds;

            End = Start.AddSeconds(DurationSeconds);           
        }

        public void PrintTime()
        {
            if (DurationSeconds > 0)
            {
                string output = String.Format("{0} - {1}", Start.ToString("HH:mm"), End.ToString("HH:mm"));
                Program.WriteMessage(output, ConsoleColor.Green);
            }           

            // Check if there were any rest minutes
            int restMinutes = (int)(RestSeconds / 60);
            if (restMinutes == 0)
            {
                Console.WriteLine();
            }
            else
            {
                Program.WriteLineMessage(" (" + restMinutes + "m)", ConsoleColor.Magenta);
            }

        }

    }
}
