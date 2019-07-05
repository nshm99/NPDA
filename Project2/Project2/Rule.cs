using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2
{
    class Rule
    {
        public string S;
        private string Nt;

        public Rule(string s, string n)
        {
            S = s;
            Nt = n;
        }

        public bool Check(string c)
        {
            for (int i = 0; i < Nt.Length; i++)
                if (c == Nt[i].ToString())
                    return true;
            return false;
        }

        public bool Check(Rule X, Rule Y)
        {
            if (Nt[0].ToString() == X.S && Nt[1].ToString() == Y.S)
                return true;
            return false;
        }
    }
}
