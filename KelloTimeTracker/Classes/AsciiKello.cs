using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace KelloTimetracker.Classes
{
    // This means ascii clock
    class AsciiKello
    {
        private Timetracker m_timetracker;
        private Timer m_timer;

        private LetterType[] m_storedLetters = new LetterType[7] { LetterType.Unknown, LetterType.Unknown, LetterType.Unknown, LetterType.Unknown, LetterType.Unknown, LetterType.Unknown, LetterType.Unknown };

        private const int StartX = 50;
        private const int StartY = 18;

        public enum LetterType
        {            
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Colon = 10,
            Unknown = 11
        }

        public AsciiKello(Timetracker a_timetracker)
        {
            m_timetracker = a_timetracker;

            m_timer = new Timer();
            m_timer.Elapsed += new ElapsedEventHandler(Tick);
            m_timer.Interval = 1000;            
        }

        public void Start()
        {
            m_timer.Start();
            Redraw();
        }

        public void Stop()
        {
            m_timer.Stop();            
        }

        public void Redraw()
        {
            m_storedLetters = new LetterType[7] {
                // Hours
                LetterType.Unknown,
                // Colon
                LetterType.Unknown,
                // Minutes
                LetterType.Unknown,LetterType.Unknown,
                // Colon
                LetterType.Unknown,
                // Seconds
                LetterType.Unknown, LetterType.Unknown };
        }

        private void Tick(object source, ElapsedEventArgs e)
        {
            if (m_timetracker.m_isRunning)
            {
                DateTime currentTime = DateTime.Now;
                TimeSpan elapsedTime = currentTime - m_timetracker.m_startTime;

                ElapsedResult result = new ElapsedResult(elapsedTime);

                int cursorLeft = Console.CursorLeft;
                int cursorTop = Console.CursorTop;
                Draw(result);
                // After drawing set the cursor back to what it was!
                Console.SetCursorPosition(cursorLeft, cursorTop);                
            }
        }

        /// <summary>
        /// Clear the drawn clock.
        /// </summary>
        private void Clear()
        {
            int totalLength = 0;
            foreach (LetterType type in m_storedLetters)
            {
                totalLength += GetLetterLength(type);
            }

            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;            

            string clearValue = new string(' ', totalLength);

            for (int i = 0; i < 5; i++)
            {
                Console.SetCursorPosition(StartX, StartY + i);
                Console.Write(clearValue);
            }

            // After drawing set the cursor back to what it was!
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }

        /// <summary>
        /// Draw the ascii clock, only redraws what has changed.
        /// </summary>
        /// <param name="a_elapsed"></param>
        private void Draw(ElapsedResult a_elapsed)
        {           
            int posX = StartX;
            int posY = StartY;            

            int storedPosition = 0;
            
            foreach (char letter in a_elapsed.hours)
            {
                LetterType type = GetLetterType(letter);
                TryDraw(type, ref posX, posY, ref storedPosition);
            }
            
            TryDraw(LetterType.Colon, ref posX, posY, ref storedPosition);

            foreach (char letter in a_elapsed.minutes)
            {
                LetterType type = GetLetterType(letter);
                TryDraw(type, ref posX, posY, ref storedPosition);
            }            

            TryDraw(LetterType.Colon, ref posX, posY, ref storedPosition);

            foreach (char letter in a_elapsed.seconds)
            {
                LetterType type = GetLetterType(letter);
                TryDraw(type, ref posX, posY, ref storedPosition);
            }
        }

        /// <summary>
        /// It tries to draw the letter type. 
        /// It will only draw if the letter types are different.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a_storedPosition"></param>
        private void TryDraw(LetterType type, ref int x, int y, ref int a_storedPosition)
        {
            if (type != m_storedLetters[a_storedPosition])
            {
                DrawLetterType(type, x, y);
                m_storedLetters[a_storedPosition] = type;
            }

            a_storedPosition++;
            x += GetLetterLength(type);
        }

        private int GetLetterLength(LetterType a_type)
        {
            switch (a_type)
            {
                case LetterType.Colon:
                    return 2;
                default:
                    return 4;
            }
        }

        private LetterType GetLetterType(char letter)
        {
            int number = (int)char.GetNumericValue(letter);
            return (LetterType)number;
        }

        private void DrawLetterType(LetterType letter, int x, int y)
        {            
            string[] letters = new string[5];
            
            switch (letter)
            {                
                case LetterType.Zero:
                    letters[0] = "###";
                    letters[1] = "# #";
                    letters[2] = "# #";
                    letters[3] = "# #";
                    letters[4] = "###";                                  
                    break;
                case LetterType.One:
                    letters[0] = " # ";
                    letters[1] = "## ";
                    letters[2] = " # ";
                    letters[3] = " # ";
                    letters[4] = "###";
                    break;
                case LetterType.Two:
                    letters[0] = " ##";
                    letters[1] = "# #";
                    letters[2] = "  #";
                    letters[3] = " # ";
                    letters[4] = "###";                    
                    break;
                case LetterType.Three:
                    letters[0] = "###";
                    letters[1] = "  #";
                    letters[2] = " ##";
                    letters[3] = "  #";
                    letters[4] = "###";
                    break;
                case LetterType.Four:
                    letters[0] = "# #";
                    letters[1] = "# #";
                    letters[2] = "###";
                    letters[3] = "  #";
                    letters[4] = "  #";
                    break;
                case LetterType.Five:
                    letters[0] = "###";
                    letters[1] = "#  ";
                    letters[2] = "###";
                    letters[3] = "  #";
                    letters[4] = "###";                    
                    break;
                case LetterType.Six:
                    letters[0] = "###";
                    letters[1] = "#  ";
                    letters[2] = "###";
                    letters[3] = "# #";
                    letters[4] = "###";                    
                    break;
                case LetterType.Seven:
                    letters[0] = "###";
                    letters[1] = "  #";
                    letters[2] = "  #";
                    letters[3] = "  #";
                    letters[4] = "  #";                    
                    break;
                case LetterType.Eight:
                    letters[0] = "###";
                    letters[1] = "# #";
                    letters[2] = "###";
                    letters[3] = "# #";
                    letters[4] = "###";                    
                    break;
                case LetterType.Nine:
                    letters[0] = "###";
                    letters[1] = "# #";
                    letters[2] = "###";
                    letters[3] = "  #";
                    letters[4] = "###";                    
                    break;
                case LetterType.Colon:
                    letters[0] = "*";
                    letters[1] = "*";
                    letters[2] = " ";
                    letters[3] = "*";
                    letters[4] = "*";                    
                    break;
            }            

            for (int i = 0; i < letters.Length; i++)
            {
                DrawLine(letters[i], x, y + i);                           
            }            
        }

        private void DrawLine(string line, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(line);
        }
    }
}
