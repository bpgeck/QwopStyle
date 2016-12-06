using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QwopStyle.Source
{
    class QwopGA
    {
        readonly char[] commandChars = { 'P', 'D', 'C', 'J', 'B', 'I', 'H', 'N', 'A', 'G', 'F', 'M', 'E', 'L', 'K', 'O' }; // every possible command char

        int popSize;        // number of members of every population
        int maxSeqLength;   // the maximum number of command characters that can be generated for a sequence
        int numNeighbors;   // the number of neighbors to tak into account when performing the cellular GA
        float mutationRate; // chance that a mutation will occur
        public QwopRunner[] Runners { get; private set; } // all of the randomly generated string sequences
                                                          // Members[0][i] is i'th initialization sequence
                                                          // Members[1][i] is i'th command sequence
        public QwopRunner[] OldRunners { get; private set; } // the last set of QwopRunner's

        public QwopGA()
        {
            this.popSize = 15;
            this.maxSeqLength = 30;
            this.numNeighbors = 4;
            this.mutationRate = 0.01f;

            Runners = new QwopRunner[popSize];
            OldRunners = new QwopRunner[popSize];
            for (int i = 0; i < Runners.Length; i++)
            {
                Runners[i] = new QwopRunner();
                OldRunners[i] = new QwopRunner();
            }

            GenerateInitialMembers();
        }

        public QwopGA(int popSize, int maxSeqLength, int numNeighbors, float mutationRate)
        {
            this.popSize = popSize;
            this.maxSeqLength = maxSeqLength;
            this.numNeighbors = numNeighbors;
            this.mutationRate = mutationRate;

            Runners = new QwopRunner[popSize];
            OldRunners = new QwopRunner[popSize];
            for (int i = 0; i < Runners.Length; i++)
            {
                Runners[i] = new QwopRunner();
                OldRunners[i] = new QwopRunner();
            }

            GenerateInitialMembers();
        }

        private void GenerateInitialMembers()
        {
            Random rn = new Random();
            for (int i = 0; i < popSize; i++)
            {
                int initLength = rn.Next(1, maxSeqLength);
                int comLength = rn.Next(1, maxSeqLength);

                string temp = "";
                for (int j = 0; j < initLength; j++)
                {
                    temp += commandChars[rn.Next(0, commandChars.Length)];
                }
                Runners[i].InitSeq = temp;

                temp = "";
                for (int j = 0; j < comLength; j++)
                {
                    temp += commandChars[rn.Next(0, commandChars.Length)];
                }
                Runners[i].ComSeq = temp;
            }
        }

        private void SelectMembers()
        {
            /*
            if (Runners[0].ParentOne != null)
            {
                Console.WriteLine("Old Runners: ");
                foreach (QwopRunner r in OldRunners) Console.WriteLine(r.ToString());
                Console.WriteLine("");
                Console.WriteLine("Runners: ");
                foreach (QwopRunner r in Runners) Console.WriteLine(r.ToString());
                Console.WriteLine("");
            }
            */

            QwopRunner highest = new QwopRunner();
            float avg = 0;
            foreach (QwopRunner r in Runners)
            {
                avg += r.Fitness;
                if (r.Fitness > highest.Fitness)
                {
                    highest = r;
                }
            }
            Console.WriteLine("Avg: " + (avg / Runners.Length)):
            Console.WriteLine(highest.ToString());

            QwopRunner[] bestRunners = new QwopRunner[popSize];
            for (int i = 0; i < popSize; i++) // deep copy that shit
            {
                bestRunners[i] = Runners[i];
            }

            for (int i = 0; i < popSize; i++)
            {
                if (Runners[i].ParentOne != null)
                {
                    // child is the greatest
                    if (Runners[i].Fitness >= Runners[i].ParentOne.Fitness &&
                        Runners[i].Fitness >= Runners[i].ParentTwo.Fitness)
                    {
                        bestRunners[i] = new QwopRunner(Runners[i].InitSeq, Runners[i].ComSeq);
                        Runners[i].Fitness = 0;
                    }
                    // mom is the greatest
                    else if (Runners[i].ParentOne.Fitness >= Runners[i].Fitness &&
                             Runners[i].ParentOne.Fitness >= Runners[i].ParentTwo.Fitness)
                    {
                        bestRunners[i] = new QwopRunner(Runners[i].ParentOne.InitSeq, Runners[i].ParentOne.ComSeq);
                        Runners[i].ParentOne.Fitness = 0;
                    }
                    // dad is the greatest
                    else if (Runners[i].ParentTwo.Fitness >= Runners[i].Fitness &&
                             Runners[i].ParentTwo.Fitness >= Runners[i].ParentOne.Fitness)
                    {
                        bestRunners[i] = new QwopRunner(Runners[i].ParentTwo.InitSeq, Runners[i].ParentTwo.ComSeq);
                        Runners[i].ParentTwo.Fitness = 0;
                    }
                    else
                    {
                        Console.WriteLine("What the fuck");
                    }
                }
            }

            Runners = bestRunners;

            /*
            Console.WriteLine("Best Runners: ");
            foreach (QwopRunner r in bestRunners) Console.WriteLine(r.ToString());
            Console.WriteLine("");
            */
        }

        public void GenerateNewMembers()
        {
            SelectMembers();
            OldRunners = new QwopRunner[popSize]; // deep copy that shit
            for (int i = 0; i < popSize; i++)
            {
                OldRunners[i] = Runners[i];
            }

            Random rn = new Random();
            List<QwopRunner> available = Runners.ToList<QwopRunner>();

            QwopRunner[] newRunners = new QwopRunner[popSize];
            for (int i = 0; i < popSize; i += 2)
            {
                // choose the two mates based on fittest local neighbors (cellular GA)
                QwopRunner mateOne = available[0];
                QwopRunner mateTwo = available[1];
                int mateTwoIndex = 1;

                //foreach (QwopRunner r in available) Console.WriteLine(r.ToString());
                for (int j = 2; j <= numNeighbors && j < available.Count; j++)
                {
                    if (available[j].Fitness > mateTwo.Fitness)
                    {
                        mateTwo = available[j];
                        mateTwoIndex = j;
                    }
                }
                available.RemoveAt(0);
                available.RemoveAt(mateTwoIndex - 1); // every member can only have one mate, so remove the used members

                // time to perform some crossover
                int spliceLocOne = rn.Next(0, mateOne.InitSeq.Length);
                int spliceLocTwo = rn.Next(0, mateTwo.InitSeq.Length);
                string newInitSeqOne = mateOne.InitSeq.Substring(0, spliceLocOne) + mateTwo.InitSeq.Substring(spliceLocTwo, mateTwo.InitSeq.Length - spliceLocTwo);
                string newInitSeqTwo = mateTwo.InitSeq.Substring(0, spliceLocTwo) + mateOne.InitSeq.Substring(spliceLocOne, mateOne.InitSeq.Length - spliceLocOne);

                spliceLocOne = rn.Next(0, mateOne.ComSeq.Length);
                spliceLocTwo = rn.Next(0, mateTwo.ComSeq.Length);
                string newComSeqOne = mateOne.ComSeq.Substring(0, spliceLocOne) + mateTwo.ComSeq.Substring(spliceLocTwo, mateTwo.ComSeq.Length - spliceLocTwo);
                string newComSeqTwo = mateTwo.ComSeq.Substring(0, spliceLocTwo) + mateOne.ComSeq.Substring(spliceLocOne, mateOne.ComSeq.Length - spliceLocOne);

                // store the new population members accordingly
                newRunners[i] = new QwopRunner(newInitSeqOne, newComSeqOne, mateOne, mateTwo);
                newRunners[i + 1] = new QwopRunner(newInitSeqTwo, newComSeqTwo, mateOne, mateTwo);

                if (rn.NextDouble() < mutationRate) newRunners[i].InitSeq = MutateCode(newRunners[i].InitSeq);
                if (rn.NextDouble() < mutationRate) newRunners[i].ComSeq = MutateCode(newRunners[i].ComSeq);
                if (rn.NextDouble() < mutationRate) newRunners[i + 1].InitSeq = MutateCode(newRunners[i].InitSeq);
                if (rn.NextDouble() < mutationRate) newRunners[i + 1].ComSeq = MutateCode(newRunners[i].ComSeq);

                // Console.WriteLine(mateOne.ToString() + " + \n" + mateTwo.ToString() + " = \n" + newRunners[i].ToString() + " and \n" + newRunners[i + 1].ToString());
            }

            Runners = newRunners;
        }

        private string MutateCode(string toMutate)
        {
            StringBuilder sb = new StringBuilder(toMutate);
            Random rn = new Random();
            int index = rn.Next(0, toMutate.Length);
            char randLetter = commandChars[rn.Next(0, commandChars.Length)];
            sb[index] = randLetter;
            return sb.ToString();
        }

        public void PrintMembers()
        {
            for (int i = 0; i < popSize; i++)
            {
                Console.WriteLine(Runners[i].InitSeq + "|" + Runners[i].ComSeq + "|" + Runners[i].Fitness);
            }
        }

    }
}
