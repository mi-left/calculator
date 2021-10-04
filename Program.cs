using System;

namespace calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Calculator launched.  Type \"exit\" to exit.\n");
            bool done = false;
            while (!done)
            {
                string s = Console.ReadLine();
                if (s.Equals("exit"))
                {
                    done = true;
                    goto Finish;
                }
                s = SuperTrim(s);
                if (CheckString(s))
                {
                    Console.Write(Calculate(s) + "\n");

                }
                else
                {
                    Console.WriteLine("Invalid expression");
                }
            Finish:;
            }
        }

        static double Calculate(string s)
        {
            if (double.TryParse(s, out double r))
            {
                return r;
            }
            for (int i = 0; i < s.Length; i++)
            {
                if (s.Substring(i, 1).Equals("("))
                {
                    string str = Convert.ToString(Parantheses(s, i, out int end));
                    int opCode = 0;
                    if (str.Equals("NaN"))
                    {
                        return double.NaN;
                    }
                    for (int j = i - 1; j >= 0 && opCode!=0; j--)
                    {
                        if ((s.Substring(j, 1).Equals("(") || s.Substring(j, 1).Equals("^") || s.Substring(j, 1).Equals("*")
                            || s.Substring(j, 1).Equals("/") || s.Substring(j, 1).Equals("+")))
                        {
                            opCode = 1;
                        }
                        if(s.Substring(j, 1).Equals("-"))
                        {
                            opCode = 2;
                        }
                    }
                    if (opCode==1)
                    {
                        s = s.Replace(s.Substring(i, (end - i) + 1), str);
                    }
                    else if (opCode == 2)
                    {
                        s = s.Replace(s.Substring(i, (end - i) + 1), "1*" + str);
                    }
                    else
                    {
                        s = s.Replace(s.Substring(i, (end - i) + 1), "*" + str);
                    }
                }
            }
            for (int i = 0; i < s.Length; i++)
            {
                if (s.Substring(i, 1).Equals("^"))
                {
                    string str = Convert.ToString(Exp(s, i, out int start, out int end));
                    s = s.Replace(s.Substring(start, (end - start) + 1), str);
                }
            }
            for (int i = 0; i < s.Length; i++)
            {
                if (s.Substring(i, 1).Equals("*") || s.Substring(i, 1).Equals("/"))
                {
                    string str = Convert.ToString(MultiDivi(s, s.Substring(i, 1), i, out int start, out int end));
                    s = s.Replace(s.Substring(start, (end - start) + 1), str);
                }
            }
            for (int i = 0; i < s.Length; i++)
            {
                if (i > 0)
                {
                    if ((s.Substring(i, 1).Equals("+") || s.Substring(i, 1).Equals("-")) && (!s.Substring(i - 1, 1).Equals("E")))
                    {
                        string str = Convert.ToString(AddSub(s, s.Substring(i, 1), i, out int start, out int end));
                        s = s.Replace(s.Substring(start, (end - start) + 1), str);
                    }
                }
                else
                {
                    if (s.Substring(i, 1).Equals("+") || s.Substring(i, 1).Equals("-"))
                    {
                        string str = Convert.ToString(AddSub(s, s.Substring(i, 1), i, out int start, out int end));
                        s = s.Replace(s.Substring(start, (end - start) + 1), str);
                    }
                }
            }
            int decCount = 0;
            for (int i = 0; i<s.Length; i++)
            {
                if (s.Substring(i, 1).Equals("."))
                {
                    decCount++;
                }
            }
            if (s.Equals("NaN") || decCount > 1)
            {
                return double.NaN;
            }
            return Calculate(s);
        }

        static double getFirstDouble(string s, int end, out int start)
        {
            start = 0;
            try
            {
                bool found = false;
                int i = end;
                while (!found && i >= 0)
                {
                    if (Int32.TryParse(s.Substring(i, 1), out int num))
                    {
                        end = i;
                        found = true;
                    }
                    i--;
                }
                found = false;
                i = end - 1;
                while (!found && i >= 0)
                {
                    if (!Int32.TryParse(s.Substring(i, 1), out int num) && !s.Substring(i, 1).Equals(".") && !s.Substring(i, 1).Equals("-"))
                    {
                        start = i + 1;
                        found = true;
                    }
                    i--;
                }
                return Convert.ToDouble(s.Substring(start, (end - start) + 1));
            }
            catch (FormatException)
            {
                start = 0;
                return double.NaN;
            }
        }

        static double getLastDouble(string s, int start, out int end)
        {
            end = s.Length - 1;
            try
            {
                bool found = false;
                int i = start;
                while (!found && i < s.Length)
                {
                    if (Int32.TryParse(s.Substring(i, 1), out int num) || s.Substring(i, 1).Equals("-"))
                    {
                        start = i;
                        found = true;
                    }
                    i++;
                }
                found = false;
                i = start + 1;
                while (!found && i < s.Length)
                {
                    if (!Int32.TryParse(s.Substring(i, 1), out int num) && !s.Substring(i, 1).Equals("."))
                    {
                        end = i - 1;
                        found = true;
                    }
                    i++;
                }
                return Convert.ToDouble(s.Substring(start, (end - start) + 1));
            }
            catch (FormatException)
            {
                end = s.Length - 1;
                return double.NaN;
            }
        }

        static double Parantheses(string s, int start, out int end)
        {
            end = s.Length - 1;
            bool hasEnd = false;
            for (int i = start; i < s.Length; i++)
            {
                if (s.Substring(i, 1).Equals(")"))
                {
                    end = i;
                    hasEnd = true;
                }
            }
            if (hasEnd)
            {
                return Calculate(s.Substring(start + 1, ((end - (1)) - (start + 1)) + 1));
            }
            else
            {
                return Calculate(s.Substring(start + 1, ((end) - (start + 1)) + 1));
            }
        }

        static double Exp(string s, int i, out int startOfOp, out int endOfOp)
        {
            double firstNum = getFirstDouble(s, i, out int start);
            double secondNum = getLastDouble(s, i, out int end);
            startOfOp = start;
            endOfOp = end;
            return Math.Pow(firstNum, secondNum);
        }

        static double MultiDivi(string s, string op, int i, out int startOfOp, out int endOfOp)
        {
            double firstNum = getFirstDouble(s, i, out int start);
            double secondNum = getLastDouble(s, i, out int end);
            startOfOp = start;
            endOfOp = end;
            if (op.Equals("*"))
            {
                if (firstNum.Equals(double.NaN))
                {
                    firstNum = 1;
                    startOfOp = i;
                }
                return firstNum * secondNum;
            }
            else if (op.Equals("/"))
            {
                if (secondNum == 0)
                {
                    Console.WriteLine("Divide by zero error");
                    return double.NaN;
                }
                return firstNum / secondNum;
            }
            return double.NaN;
        }

        static double AddSub(string s, string op, int i, out int startOfOp, out int endOfOp)
        {
            double firstNum = getFirstDouble(s, i, out int start);
            double secondNum = getLastDouble(s, i, out int end);
            startOfOp = start;
            endOfOp = end;
            if (op.Equals("-") && firstNum.Equals(double.NaN))
            {
                firstNum = 0;
                startOfOp = i;
            }
            return firstNum + secondNum;
        }

        static bool CheckString(string s)
        {
            if (s.Equals("") || s.Equals(".") || s.Contains(".-"))
            {
                return false;
            }
            int closeCount = 0;
            foreach (char c in s)
            {
                if ((c > 32 && c < 40) || (c == 44) || (c > 57 && c < 94) || (c > 94))
                {
                    return false;
                }
                if (c == 41)
                {
                    closeCount++;
                }
            }
            int openCount = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s.Substring(i, 1).Equals("("))
                {
                    openCount++;
                }
            }
            if (closeCount > openCount)
            {
                return false;
            }
            return true;
        }

        static string SuperTrim(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if(s.Substring(i,1).Equals(" "))
                {
                    s = s.Remove(i, 1);
                    return SuperTrim(s);
                }
            }
            return s;
        }
    }
}