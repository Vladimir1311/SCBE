using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocsToPictures
{
    class ConsoleReader
    {
        private static Thread inputThread;
        private static AutoResetEvent getInput, gotInput;
        private static string input;

        static ConsoleReader()
        {
            getInput = new AutoResetEvent(false);
            gotInput = new AutoResetEvent(false);
            inputThread = new Thread(Reader)
            {
                IsBackground = true
            };
            inputThread.Start();
        }

        private static void Reader()
        {
            while (true)
            {
                getInput.WaitOne();
                input = Console.ReadLine();
                gotInput.Set();
            }
        }

        // omit the parameter to read a line without a timeout
        public static bool TryReadLine(out string readed, TimeSpan timeOut)
        {
            getInput.Set();
            bool success = gotInput.WaitOne(timeOut);
            if (success)
            {
                readed = input;
                return true;
            }
            else
            {
                readed = null;
                return false;
            }
        }
    }
}
