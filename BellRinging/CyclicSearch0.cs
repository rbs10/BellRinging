using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BellRinging
{
    public class CyclicSearch0
    {
        static MethodLibrary methodLibrary = new MethodLibrary();

        bool[] rowsRung = new bool[40320];
        List<Row> rows = new List<Row>(2000);
        int rowNum;
        Method method;
        int leadForSplice = -1;

        static MusicalPreferences musicalPreferences;

        static List<Permutation> link = new List<Permutation>();

        public static void Checks()
        {
            musicalPreferences = new MusicalPreferences();
            musicalPreferences.InitSJT();

            link.Add(Permutation.FromPlaceNotation("38", 8));
            link.Add(Permutation.FromPlaceNotation("14", 8));
            link.Add(Permutation.FromPlaceNotation("38", 8));
            link.Add(Permutation.FromPlaceNotation("14", 8));
            link.Add(Permutation.FromPlaceNotation("38", 8));
            link.Add(Permutation.FromPlaceNotation("14", 8)); 
            
            
            foreach (var method in 
                //new [] { "Norfolk"})
                methodLibrary.MethodNames)
            {
                try
                {
                    var search = new CyclicSearch0(method);

                    System.Diagnostics.Debug.WriteLine("{0} = {1}", method, "OK");
                    Console.WriteLine("{0},{1},{2},{3},{4}",method,search.rows.Count,musicalPreferences.ScoreLead(search.rows),
                        search.method.WrongPlaceCount, search.leadForSplice);
                    /*
                    Console.WriteLine();
                    Console.WriteLine("MUSIC");
                    Console.WriteLine();
                    foreach (var rowsGroupedByMusic in GetMusicalChanges(search.rows).GroupBy(x => x.Value.Name).OrderByDescending(x => x.First().Value.Points).ThenBy(x => x.First().Value.Name))
                    {
                        Console.WriteLine("{0} {1}", rowsGroupedByMusic.First().Value.Name, rowsGroupedByMusic.Count());
                    }
                    Console.WriteLine();
                    Console.WriteLine("DETAILS");
                    Console.WriteLine();
                    foreach (KeyValuePair<Row, string> rs in Music(search.rows))
                    {
                        Console.WriteLine("{0} {1}", rs.Key, rs.Value);
                    }*/
                }
                catch ( Exception ex)
                {
                  System.Diagnostics.Debug.WriteLine("{0} = {1}",method,ex.Message);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press any key");
            Console.ReadLine();
        }
        static IEnumerable<KeyValuePair<Row, IMusicalChange>> GetMusicalChanges(IEnumerable<Row> rows)
        {
            foreach (Row r in rows)
            {
                foreach (var musicalChange in musicalPreferences.EnumerateMusicalChanges(r))
                {
                    yield return new KeyValuePair<Row, IMusicalChange>(r, musicalChange);
                }
            }
        }
       static IEnumerable<KeyValuePair<Row,string>> Music(IEnumerable<Row> rows)
        {
            int i = 0;
            foreach (Row r in rows)
            {
                i++;
                foreach (string s in musicalPreferences.EnumerateMusic(r))
                {
                    yield return new KeyValuePair<Row, string>(r, s);
                }
            }
        }
        public CyclicSearch0(string methodName)
        {
            Check(methodName);
        }

        public void Check(string methodName)
        {
            var notation = methodLibrary.GetNotation(methodName);
             method = new Method(methodName, "M", notation, 8);
            Row leadEnd = new Row(8);


            for (int j = 0; j < 7; ++j)
            {

                for (int i = 0; i < 6; ++i)
                {
                    var restLines = method.CorePermuations.Apply(leadEnd);
                    leadEnd = restLines.Last().Apply(method.PlainLeadEndPermutation);
                    foreach (var line in restLines)
                    {
                        Account(line);
                    }
                    Account(leadEnd);
                    if ( j == 0 && leadEnd.ToString().EndsWith("2"))
                    {
                        leadForSplice = i;
                    }
                    if ( i == leadForSplice )
                    {
                        foreach (var perm in link)
                        {
                            leadEnd = leadEnd.Apply(perm);
                            Account(leadEnd);
                        }
                    }
                }
               
                //if ( j == 0 )
                //{
                //    if (leadEnd.ToString() != "17823456")
                //    {
                //        throw new Exception("Part end = " + leadEnd);
                //    }
                //}
            }
        }

        private void Account(Row line)
        {
            ++rowNum;
            var asNum = line.ToNumber();
            if ( rowsRung[asNum])
            {
                throw new Exception(string.Format("False at {0} on {1}", rowNum, line));
            }
            rowsRung[asNum] = true;
            rows.Add(line);
        }
    }
}
