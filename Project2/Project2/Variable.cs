using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project2
{
    public class Variable
    {
        public string name ;
        public string[] content = new string[3];
        public List<Tuple<char, string, string>> adj = new List<Tuple<char, string, string>>();

        public string nameInRule;
        public static char count = 'A';
        public static Dictionary<string, string> convertedState = new Dictionary<string, string>();
        public List<Tuple<char, string, string>> adjInRule = new List<Tuple<char, string, string>>();

        public Variable(string startState,string endState,string between)
        {
            content[0] = startState;
            content[1] = between;
            content[2] = endState;
            name = $"(q{content[0]} {content[1]} q{content[2]})";

            if (convertedState.ContainsKey(name))
                nameInRule = convertedState[name];

            else
            {
                nameInRule = count.ToString();
                convertedState.Add(name, nameInRule);
                count++;
                if (count == 'S')
                    count++;
            }
        }
    }
}
