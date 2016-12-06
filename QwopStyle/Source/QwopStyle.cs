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
    class QwopStyle
    {
        [DllImport("User32.dll")]
        private static extern Int32 SetForegroundWindow(int hWnd);

        static int maxHeats = 30;

        static void Main(string[] args)
        {
            QwopOcr ocr = new QwopOcr(@"C:\Users\Brendan\Desktop\Test.png");
            ocr.UpdateScore();
            QwopGA ga = new QwopGA(30, 30, 5, 0.1f);

            for (int i = 0; i < maxHeats; i++)
            {
                for (int j = 0; j < ga.Runners.Length; j++)
                {
                    QwopCommander qc = new QwopCommander(ga.Runners[j].InitSeq, ga.Runners[j].ComSeq);
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    while (sw.ElapsedMilliseconds < 60 * 1000)
                    {
                        qc.SendCommand();
                    }

                    ocr.UpdateScore();
                    if (ocr.GetFinalState() == FinalState.CRASHED)
                    {
                        ga.Runners[j].Fitness = 0;
                    }
                    else
                    {
                        try
                        {
                            ga.Runners[j].Fitness = float.Parse(ocr.Score);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Couldn't parse score: " + ocr.Score);
                            ga.Runners[j].Fitness = 0;
                        }
                    }

                    qc.KillRunner();
                    qc.Reload();
                }

                /*
                Random rn = new Random();
                foreach (QwopRunner r in ga.Runners) r.Fitness = rn.Next(0, 5);
                */

                //Console.WriteLine("");
                //ga.PrintMembers();
                //Console.WriteLine("");
                ga.GenerateNewMembers();
            }
        }
    }
}
