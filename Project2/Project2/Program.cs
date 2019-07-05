using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Project2
{
    class Program
    {
        static void Main(string[] args)
        {
            //---------------------------------------------------------------------------------------------------------------------------//
            //=> phase 0 + phase 1

            string[] lines = File.ReadAllLines("../../input.txt");

            int stateNumber = int.Parse(lines[0]);
            string[] alphabet = lines[1].Split(new char[] { ',' }).ToArray();
            string[] stackContent = lines[2].Split(new char[] { ',' }).ToArray();
            Stack<string> stack = new Stack<string>();
            stack.Push(lines[3]);
            char initialState = ' ';
            string startVariable = "";

            List<Variable> var = new List<Variable>();
            List<Variable> helpToSimplification = new List<Variable>();

            for (int i = 4; i < lines.Length; i++)
            {
                string[] input = lines[i].Split(new char[] { ',' }).ToArray();

                if (input[3] == "_")        //a, b->e.....(q0,q,$,_,q1)
                {
                    //avali -> dashte bashe
                    if (input[0].Length == 4)
                    {
                        initialState = input[0][3];
                        //akhari * dashte bashe
                        if (input[4].Length == 3)
                        {
                            var.Add(new Variable(input[0][3].ToString(), input[4][2].ToString(), input[2][0].ToString()));
                            var[var.Count - 1].adj.Add(new Tuple<char, string, string>(input[1][0], "", ""));
                            var[var.Count - 1].adjInRule.Add(new Tuple<char, string, string>(input[1][0], "", ""));
                            if (input[2] == "$")
                                startVariable = var[var.Count - 1].name;

                            helpToSimplification.Add(var[var.Count - 1]);
                        }
                        else
                        {
                            var.Add(new Variable(input[0][3].ToString(), input[4][1].ToString(), input[2][0].ToString()));
                            var[var.Count - 1].adj.Add(new Tuple<char, string, string>(input[1][0], "", ""));
                            var[var.Count - 1].adjInRule.Add(new Tuple<char, string, string>(input[1][0], "", ""));

                            helpToSimplification.Add(var[var.Count - 1]);
                        }

                    }
                    else
                    {
                        //akhari * dashte bashe
                        if (input[4].Length == 3)
                        {
                            var.Add(new Variable(input[0][1].ToString(), input[4][2].ToString(), input[2][0].ToString()));
                            var[var.Count - 1].adj.Add(new Tuple<char, string, string>(input[1][0], "", ""));
                            var[var.Count - 1].adjInRule.Add(new Tuple<char, string, string>(input[1][0], "", ""));

                            if (input[0][1] == initialState && input[2] == "$")
                                startVariable = var[var.Count - 1].name;

                            helpToSimplification.Add(var[var.Count - 1]);
                        }
                        else
                        {
                            var.Add(new Variable(input[0][1].ToString(), input[4][1].ToString(), input[2][0].ToString()));
                            var[var.Count - 1].adj.Add(new Tuple<char, string, string>(input[1][0], "", ""));
                            var[var.Count - 1].adjInRule.Add(new Tuple<char, string, string>(input[1][0], "", ""));

                            helpToSimplification.Add(var[var.Count - 1]);
                        }
                    }
                }

                else
                {
                    //avali -> dashte bashe
                    if (input[0].Length == 4)
                    {
                        initialState = input[0][3];
                        //akhari * dashte bashe
                        if (input[4].Length == 3)
                        {
                            AddAdjancy(var, 3, 2, input, stateNumber);
                        }

                        else
                        {
                            AddAdjancy(var, 3, 1, input, stateNumber);
                        }

                    }

                    else
                    {
                        //akhari * dashte bashe
                        if (input[4].Length == 3)
                        {
                            AddAdjancy(var, 1, 2, input, stateNumber);
                        }
                        else
                        {
                            AddAdjancy(var, 1, 1, input, stateNumber);
                        }
                    }
                }
            }


            string output = "";
            PrintGrammer(var, ref output);
            File.WriteAllText("../../output.txt", output);

            EditStartVariable(var, startVariable);
            var = Simplified(var, helpToSimplification);
            var = RemoveNullable(var, helpToSimplification);

            //PrintConvertedGrammer(var);
            //=> phase 2

            Console.WriteLine("..................................");
            List<Variable> chamskyVar = new List<Variable>();
            chamskyVar = var;
            MakeChamsky(chamskyVar);
            //Console.WriteLine("//////////////////////////////////////////////////////////////");

            List<Rule> rules = new List<Rule>();
            foreach (var c in chamskyVar)
            {
                for (int i = 0; i < c.adjInRule.Count(); i++)
                {
                    string right = "";
                    if (c.adjInRule[i].Item1 != ' ')
                        right += c.adjInRule[i].Item1;
                    if (c.adjInRule[i].Item2 != " ")
                        right += c.adjInRule[i].Item2 + c.adjInRule[i].Item3;
                    rules.Add(new Rule(c.nameInRule, right));
                   // Console.WriteLine($"{c.nameInRule} -> {right}");//?????delete shavad
                }

            }

            //Console.WriteLine("start................................");
            string word = "ab";
            CYK parser = new CYK(word, rules);
            parser.Parse();
            parser.PrintTable();
            string not = "";

            Console.WriteLine(parser.GetResult());
            /*
            if (!parser.GetResult())
                not = "'t";

            if (not == "t")
                Console.WriteLine("false");
            else
                Console.WriteLine("true");
            Console.WriteLine("\"{0}\" can" + not + " be produced!", word);
            */

        }

        private static List<Variable> RemoveNullable(List<Variable> var, List<Variable> helpToSimplification)
        {
            List<Variable> removeNullable = new List<Variable>();
            string nullableVariable = "";

            foreach (var variable in helpToSimplification)
            {
                foreach (var adj in variable.adjInRule)
                    if (adj.Item1 == '_')
                        nullableVariable = variable.nameInRule;
            }

            foreach (var variable in var)
                removeNullable.Add(variable);

            for (int i = 0; i < var.Count; i++)
            {
                for (int j = 0; j < var[i].adjInRule.Count; j++)
                {
                    if (var[i].adjInRule[j].Item2 == "S" && var[i].adjInRule[j].Item3 != "S")
                    {
                        removeNullable[i].adjInRule.Add(new Tuple<char, string, string>(var[i].adjInRule[j].Item1, var[i].adjInRule[j].Item2, ""));
                    }

                    if (var[i].adjInRule[j].Item3 == "S" && var[i].adjInRule[j].Item2 != "S")
                    {
                        removeNullable[i].adjInRule.Add(new Tuple<char, string, string>(var[i].adjInRule[j].Item1, var[i].adjInRule[j].Item2, ""));
                    }
                }
            }

            return removeNullable;
        }

        private static List<Variable> Simplified(List<Variable> var, List<Variable> helpToSimplification)
        {
            List<Variable> simplified = new List<Variable>();
            List<string> nameInSimple = new List<string>();

            foreach (var variable in helpToSimplification)
            {
                simplified.Add(variable);
                nameInSimple.Add(variable.nameInRule);
            }

            bool change = true;

            do
            {
                foreach (var variable in var)
                {
                    change = false;
                    foreach (var adj in variable.adjInRule)
                    {
                        if (nameInSimple.Contains(adj.Item2) && nameInSimple.Contains(adj.Item3))
                        {
                            simplified.Add(new Variable(variable.nameInRule));
                            simplified[simplified.Count - 1].adjInRule.Add(adj);
                            nameInSimple.Add(variable.nameInRule);
                            change = true;
                        }

                    }
                }

            } while (change);

            return simplified;
        }

        private static void MakeChamsky(List<Variable> chamskyVar)
        {
            int count = chamskyVar.Count();
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < chamskyVar[i].adjInRule.Count(); j++)
                    if (chamskyVar[i].adjInRule[j].Item2 != "")
                    {
                        if (chamskyVar[i].adjInRule[j].Item3 != "")
                        {
                            string letter = chamskyVar[i].adjInRule[j].Item1.ToString();
                            chamskyVar.Add(new Variable(letter, "", ""));
                            chamskyVar[chamskyVar.Count - 1].adjInRule.Add(new Tuple<char, string, string>(char.Parse(letter), "", ""));
                            letter = chamskyVar[chamskyVar.Count - 1].nameInRule;
                            string adjVariable = chamskyVar[i].adjInRule[j].Item2;
                            chamskyVar.Add(new Variable(letter, adjVariable, ""));
                            chamskyVar[chamskyVar.Count - 1].adjInRule.Add(new Tuple<char, string, string>(char.Parse(letter), adjVariable, ""));
                            adjVariable = chamskyVar[i].adjInRule[j].Item3;
                            chamskyVar[i].adjInRule[j] =
                                new Tuple<char, string, string>
                                (' ', chamskyVar[chamskyVar.Count - 1].nameInRule,
                                adjVariable);
                        }
                        else
                        {
                            string letter = chamskyVar[i].adjInRule[j].Item1.ToString();
                            chamskyVar.Add(new Variable(letter, "", ""));
                            chamskyVar[chamskyVar.Count - 1].adjInRule.Add
                                (new Tuple<char, string, string>(char.Parse(letter), "", ""));
                            string adjVariable = chamskyVar[i].adjInRule[j].Item2;
                            chamskyVar[i].adjInRule[j] =
                                new Tuple<char, string, string>
                                (' ', chamskyVar[chamskyVar.Count - 1].nameInRule,
                                adjVariable);
                        }
                    }
            }
        }

        private static void EditStartVariable(List<Variable> var, string startVariable)
        {
            for (int i = 0; i < var.Count; i++)
            {
                if (startVariable == var[i].name)
                    var[i].nameInRule = "S";

                for (int j = 0; j < var[i].adj.Count; j++)
                {
                    if (startVariable == var[i].adj[j].Item2 && startVariable == var[i].adj[j].Item3)
                    {
                        Tuple<char, string, string> temp = var[i].adjInRule[j];
                        var[i].adjInRule[j] = new Tuple<char, string, string>(temp.Item1, "S", "S");
                    }

                    if (startVariable == var[i].adj[j].Item2)
                    {
                        Tuple<char, string, string> temp = var[i].adjInRule[j];
                        var[i].adjInRule[j] = new Tuple<char, string, string>(temp.Item1, "S", temp.Item3);
                    }

                    if (startVariable == var[i].adj[j].Item3)
                    {
                        Tuple<char, string, string> temp = var[i].adjInRule[j];
                        var[i].adjInRule[j] = new Tuple<char, string, string>(temp.Item1, temp.Item2, "S");
                    }
                }
            }
        }

        private static void PrintConvertedGrammer(List<Variable> var)
        {
            foreach (var variable in var)
                foreach (var adj in variable.adjInRule)
                    Console.WriteLine($"{variable.nameInRule} -> {adj.Item1} {adj.Item2} {adj.Item3}");
                    
                
        }

        private static void PrintGrammer(List<Variable> var, ref string output)
        {
            foreach (var variable in var)
                foreach (var adj in variable.adj)
                {
                    Console.WriteLine($"{variable.name} -> {adj.Item1} {adj.Item2} {adj.Item3}");
                    output += $"{variable.name} -> {adj.Item1} {adj.Item2} {adj.Item3}   ";
                }


        }

        private static void AddAdjancy(List<Variable> var, int firstIndex, int secondIndex, string[] input, int stateNumber)
        {
            for (int i = 0; i < stateNumber; i++)
            {
                var.Add(new Variable(input[0][firstIndex].ToString(), i.ToString(), input[2]));
                int index = var.Count - 1;
                for (int l = 0; l < stateNumber; l++)
                {
                    var.Add(new Variable(input[4][secondIndex].ToString(), l.ToString(), input[3][0].ToString()));
                    var.Add(new Variable(l.ToString(), i.ToString(), input[3][1].ToString()));
                    var[index].adj.Add
                        (new Tuple<char, string, string>(input[1][0], var[var.Count - 2].name, var[var.Count - 1].name));
                    var[index].adjInRule.Add
                        (new Tuple<char, string, string>(input[1][0], var[var.Count - 2].nameInRule, var[var.Count - 1].nameInRule));
                }
            }
        }
    }
}
