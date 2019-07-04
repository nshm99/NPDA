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
            //read Input => phase 0

            string[] lines = File.ReadAllLines("../../input.txt");

            int stateNumber = int.Parse(lines[0]);
            string[] alphabet = lines[1].Split(new char[] { ',' }).ToArray();
            string[] stackContent = lines[2].Split(new char[] { ',' }).ToArray();
            Stack<string> stack = new Stack<string>();
            stack.Push(lines[3]);
            char initialState = ' ';
            string startVariable = "";
            string stateChar;
            string final;

            List<Variable> var = new List<Variable>();

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

                        }
                        else
                        {
                            var.Add(new Variable(input[0][3].ToString(), input[4][1].ToString(), input[1][0].ToString()));
                            var[var.Count - 1].adj.Add(new Tuple<char, string, string>(input[1][0], "", ""));
                            var[var.Count - 1].adjInRule.Add(new Tuple<char, string, string>(input[1][0], "", ""));
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
                        }
                        else
                        {
                            var.Add(new Variable(input[0][1].ToString(), input[4][1].ToString(), input[1][0].ToString()));
                            var[var.Count - 1].adj.Add(new Tuple<char, string, string>(input[1][0], "", ""));
                            var[var.Count - 1].adjInRule.Add(new Tuple<char, string, string>(input[1][0], "", ""));
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



            PrintGrammer(var);
            EditStartVariable(var, startVariable);
            PrintConvertedGrammer(var);
            Console.WriteLine("..................................");
            List<Variable> chamskyVar = new List<Variable>();
            chamskyVar = var;
            MakeChamsky(chamskyVar);
            PrintConvertedGrammer(chamskyVar);
        }

        private static void MakeChamsky(List<Variable> chamskyVar)
        {
            int count = chamskyVar.Count();
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < chamskyVar[i].adjInRule.Count(); j++)
                    if (chamskyVar[i].adjInRule[j].Item2 != "")
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

        private static void PrintGrammer(List<Variable> var)
        {
            foreach (var variable in var)
                foreach (var adj in variable.adj)
                    Console.WriteLine($"{variable.name} -> {adj.Item1} {adj.Item2} {adj.Item3}");


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
