using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QwopStyle.Source
{
    class QwopRunner
    {
        public string InitSeq { get; set; }
        public string ComSeq { get; set; }
        public float Fitness { get; set; }
        public QwopRunner ParentOne { get; set; }
        public QwopRunner ParentTwo { get; set; }
        public QwopRunner()
        {
            InitSeq = "";
            ComSeq = "";
            Fitness = 0;
            ParentOne = null;
            ParentTwo = null;
        }

        public QwopRunner(string initSeq, string comSeq)
        {
            InitSeq = initSeq;
            ComSeq = comSeq;
            Fitness = 0;
            ParentOne = null;
            ParentTwo = null;
        }

        public QwopRunner(string initSeq, string comSeq, QwopRunner parentOne, QwopRunner parentTwo)
        {
            InitSeq = initSeq;
            ComSeq = comSeq;
            Fitness = 0;
            ParentOne = parentOne;
            ParentTwo = parentTwo;
        }

        override public string ToString()
        {
            return InitSeq + "|" + ComSeq + "|" + Fitness;
        }
    }
}
