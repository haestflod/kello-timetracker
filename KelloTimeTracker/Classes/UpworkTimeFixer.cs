using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KelloTimetracker.Classes
{
    /// <summary>
    /// This class goes through all recorded elapsed time and puts them in 10 minute blocks.
    /// Since that is what Upwork expects.
    /// It then tries to pad the next block with the overlapped minutes.    /// 
    /// </summary>
    class UpworkTimeFixer
    {
        public double RestSeconds = 0;

        public List<UpworkTimeEntry> FixEntries(List<UpworkTimeEntry> a_entries)
        {
            if (a_entries.Count == 0)
            {
                return a_entries;
            }

            /*
            1: Remove all entries with less than 10 minute durations
            2: Make all times in 10 minute intervals  14:52 - 15.02 => 14:50 - 15.00
            3: Check if Times are overlapping and if they are merge
            4: Add Rest time 
            */            

            List<UpworkTimeEntry> result = new List<UpworkTimeEntry>();

            // Step 1: Remove all entries with less than 10 minute duration and add it to restSeconds
            for (int i = 0; i < a_entries.Count; i++)
            {
                UpworkTimeEntry entry = a_entries[i];

                if (entry.DurationSeconds < 600)
                {
                    RestSeconds += entry.DurationSeconds;
                    RestSeconds += entry.RestSeconds;
                }
                else
                {
                    // Step 2 Move times to even blocks                    

                    // Do seconds
                    int seconds = entry.Start.Second;
                    //restSeconds += seconds;
                    entry.Start = entry.Start.AddSeconds(-seconds);
                    entry.End = entry.End.AddSeconds(-seconds);
                    //entry.RestSeconds += seconds;
                    // Minutes
                    int minuteOverflow = entry.Start.Minute % 10;

                    entry.Start = entry.Start.AddMinutes(-minuteOverflow);
                    entry.End = entry.End.AddMinutes(-minuteOverflow);

                    result.Add(entry);
                    RestSeconds += entry.RestSeconds;
                    entry.RestSeconds = 0;
                }
            }

            // If all entries were removed
            if (result.Count == 0)
            {
                result.Add(a_entries[0]);
            }

            result = CheckOverlap(result);

            int restAdds = (int)(RestSeconds / 600);

            // Add 10 minutes to each entry and then repeat until there are no more entries to add to.
            while (restAdds > 0)
            {
                foreach (UpworkTimeEntry entry in result)
                {
                    entry.End = entry.End.AddMinutes(10);
                    entry.DurationSeconds += 600;
                    RestSeconds -= 600;
                    restAdds--;

                    if (restAdds <= 0)
                    {
                        break;
                    }

                }
            }

            result = CheckOverlap(result);
            
            return result;
        }

        private List<UpworkTimeEntry> CheckOverlap(List<UpworkTimeEntry> a_entries)
        {
            List<UpworkTimeEntry> result = new List<UpworkTimeEntry>();

            if (a_entries.Count > 1)
            {                
                UpworkTimeEntry currentEntry = a_entries[0];

                result.Add(currentEntry);

                // Step 3 check if start is same as end
                for (int i = 1; i < a_entries.Count; ++i)
                {
                    UpworkTimeEntry entry = a_entries[i];

                    // Check if the currentEntry ends where this new entry starts.
                    // 15:00 == 15:00
                    if (currentEntry.End == entry.Start)
                    {
                        string current = String.Format("{0} - {1}", currentEntry.Start.ToString("HH:mm"), currentEntry.End.ToString("HH:mm"));
                        string merged = String.Format("{0} - {1}", entry.Start.ToString("HH:mm"), entry.End.ToString("HH:mm"));

                        currentEntry.End = entry.End;
                        currentEntry.DurationSeconds += entry.DurationSeconds;                        

                        Program.WriteMessage("Merged 2 times -- ", ConsoleColor.Yellow);
                        Program.WriteMessage(current + " and ", ConsoleColor.Cyan);
                        Program.WriteLineMessage(merged, ConsoleColor.Magenta);
                    }
                    // This should never happen unless the file has been manually edited
                    // 15:00 > 14:50
                    else if (currentEntry.End > entry.Start)
                    {
                        Program.WriteLineMessage("END > START !! - " + currentEntry.DurationSeconds + " " + entry.DurationSeconds, ConsoleColor.Yellow);
                    }
                    // If End < Start this should be most cases
                    else
                    {
                        currentEntry = entry;
                        // Don't forget to add the entry to the result
                        result.Add(entry);
                    }
                }
            }
            else
            {
                result = a_entries;
            }

            return result;
        }
    }
}
