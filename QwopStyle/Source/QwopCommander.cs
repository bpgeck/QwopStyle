using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QwopStyle.Source
{
    class QwopCommander
    {
        private string fullSeq;            // sequence to be executed at the beginning of the run
        private int initSeqLength;         // the length of the initialization sequence
        private int commandLoc;            // the location in commandSeq of the next character to send to the flashplayer window
        private Stopwatch lastCommandTime; // time elapsed since last command

        public QwopCommander(string initSeq, string gaitSeq)
        {
            fullSeq = initSeq + gaitSeq;
            initSeqLength = initSeq.Length;
            commandLoc = -1;
            lastCommandTime = new Stopwatch();
        }

        public void SendCommand()
        {
            if (commandLoc == -1 || lastCommandTime.ElapsedMilliseconds >= 150) // advance to the next command after 150ms
            {
                lastCommandTime = new Stopwatch();
                lastCommandTime.Start();
                commandLoc++;
                if (commandLoc >= fullSeq.Length) commandLoc = initSeqLength; // loop but ignore initSeq
            }

            char command = fullSeq[commandLoc];
            switch (command)
            {
                case 'P':
                    SendKeys.SendWait("");
                    break;
                case 'D':
                    SendKeys.SendWait("P");
                    break;
                case 'C':
                    SendKeys.SendWait("O");
                    break;
                case 'J':
                    SendKeys.SendWait("OP");
                    break;
                case 'B':
                    SendKeys.SendWait("W");
                    break;
                case 'I':
                    SendKeys.SendWait("WP");
                    break;
                case 'H':
                    SendKeys.SendWait("WO");
                    break;
                case 'N':
                    SendKeys.SendWait("WOP");
                    break;
                case 'A':
                    SendKeys.SendWait("Q");
                    break;
                case 'G':
                    SendKeys.SendWait("QP");
                    break;
                case 'F':
                    SendKeys.SendWait("QO");
                    break;
                case 'M':
                    SendKeys.SendWait("QOP");
                    break;
                case 'E':
                    SendKeys.SendWait("QW");
                    break;
                case 'L':
                    SendKeys.SendWait("QWP");
                    break;
                case 'K':
                    SendKeys.SendWait("QWO");
                    break;
                case 'O':
                    SendKeys.SendWait("QWOP");
                    break;
                default:
                    Console.WriteLine("Invalid Command Character: " + command);
                    break;
            }
        }

        public void KillRunner()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < 9 * 1000)
            {
                SendKeys.SendWait("QP");
            }
        }

        public void Reload()
        {
            SendKeys.SendWait(" ");
        }
    }
}
