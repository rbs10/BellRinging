using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BellRinging;

namespace MethodCompatabilityHunt
{
    class Program
    {
        static void Main(string[] args)
        {

            MethodLibrary lib = new MethodLibrary();

            var methodNames = lib.MethodNames.OrderBy(n => Familiarity(n)).ToList();
            Method[] methods = new Method[methodNames.Count];
            //var methods = methodNames.Take(100).Select(method =>
            //    {
                  
            //    }).ToArray();
            for (int i = 0; i < methods.Length; ++i)
            {
                // create method names lazily
                string method = methodNames[i];
                var notation = lib.GetNotation(method);
                methods[i] = new Method(method, "", notation, 8);
                //if (!notation.StartsWith("X"))
                {
                    Console.WriteLine("{0}  {2}", i, method, notation);
                    var lead1 = methods[i].GenerateLead(new Row(8));
                    for (int j = 0; j < i; ++j)
                    {
                        var lead2 = methods[j].GenerateLead(new Row(8));
                        if (CheckFalseness(lead1, lead2))
                        {
                            System.Diagnostics.Debug
                            .WriteLine("{0} - {1}", methodNames[i], methodNames[j]);

                            // found the most familar partner to pair with i
                            //break;
                        }
                    }
                }
            }
        }

        private static int Familiarity(string n)
        {
            if (n == "Yorkshire") return 1;
            if (n == "Cambridge") return 2;
            if (n == "Lincolnshire") return 3;
            if (n == "Superlative") return 4;
            if (n == "Rutland") return 5;
            if (n == "Bristol") return 6;
            if (n == "London") return 7;
            if (n == "Pudsey") return 8;


            if (n == "Cornwall") return 13;
            if (n == "Cassiobury") return 13;
            if (n == "Uxbridge") return 13;
            if (n == "Lindum") return 13;
            if (n == "Cray") return 23;
            if (n == "Watford") return 23;
            if (n == "Wembley") return 23;
            if (n == "Glasgow") return 23;
            if (n == "Jersey") return 23;
            if (n == "Preston") return 23;
            if (n == "Ipswich") return 23;
            if (n == "Double Dublin") return 23;
            if (n == "Ashtead") return 13;
            if (n == "Tavistock") return 13;
            if (n == "Whalley") return 13;

            // Jim Taylors
            if (n == "Londonderry") return 24;
            if (n == "Yorkhouse") return 24;
            if (n == "Silcester") return 24;
            if (n == "Rushmoor") return 24;
            if (n == "Brockley") return 24;
            if (n == "Silcester") return 24;

            if (n == "Belfast") return 8;


            return 999;
        }

        private static bool CheckFalseness(Lead lead1, Lead lead2)
        {
            foreach (var row1 in lead1.RowsAsInts.Skip(1))
            {
                foreach (var row2 in lead2.RowsAsInts.Skip(1))
                {
                    if (row1 == row2)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
