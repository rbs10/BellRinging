using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BellRinging
{
    public class CyclicSearch3
    {
        static MethodLibrary methodLibrary = new MethodLibrary();

        bool[] rowsRung = new bool[40320];
        List<Row> rows = new List<Row>(2000);
        int rowNum;
        int leadForSplice = -1;

        static int bestScore = 0;
        static  object myLock = new object();
        static MusicalPreferences musicalPreferences;

        static List<Permutation> link = new List<Permutation>();

        //static Method linkMethod = new Method("Link", "L", "3.4.3.4.3.4", 8);

        public static void Checks()
        {
            musicalPreferences = new MusicalPreferences();
            musicalPreferences.InitSJT();

           /* link.Add(Permutation.FromPlaceNotation("38", 8));
            link.Add(Permutation.FromPlaceNotation("14", 8));
            link.Add(Permutation.FromPlaceNotation("38", 8));
            link.Add(Permutation.FromPlaceNotation("14", 8));
            link.Add(Permutation.FromPlaceNotation("38", 8));
            link.Add(Permutation.FromPlaceNotation("14", 8));*/

            
            link.Add(Permutation.FromPlaceNotation("x", 8));
            link.Add(Permutation.FromPlaceNotation("16", 8));
            link.Add(Permutation.FromPlaceNotation("x", 8));
            link.Add(Permutation.FromPlaceNotation("14", 8));
            link.Add(Permutation.FromPlaceNotation("x", 8));
            link.Add(Permutation.FromPlaceNotation("16", 8));
            link.Add(Permutation.FromPlaceNotation("x", 8));
            link.Add(Permutation.FromPlaceNotation("16", 8));

            var stdMethodNames = new[]
                {
                    //STD 8
                     "Yorkshire","Cambridge", "Superlative",
                    "Rutland", "Lincolnshire", "Pudsey"
                    //,
                    , "Cassiobury", "Cornwall", "Lindum", "Ashtead","Uxbridge"
                    , "Bristol",  "Glasgow"
                    // STD 8 - wrong
                    ,"London","Double Dublin","Belfast",
                    //// SMITHS
                    //,
                    "Whalley" ,
                    "Watford", "Tavistock",
                    "Wembley",
                    "Jersey", "Preston", "Ipswich", "Cray",
                    // 4-SPLICED
                    //,
                    // Promising
                    "Norfolk",
                    // JIM TAYLOR
                    "Yorkhouse", "Rushmoor", "Silchester", "Londonderry", "Hersham", "Brockley",
                    // CHANDLERS
                    "Huddersfield", "Malpas", "Essex", "Caterham","Buckfastleigh","Yeading","Chertsey","Sussex","Colnbrook","Sonning","Moulton","Richmond",
                    "Newcastle","Willesden","Chesterfield","Northampton","Claybrooke","Newlyn"
                };

            //stdMethodNames = new[] { "Preston", "Norfolk" };
            var start = DateTime.UtcNow;
            var stdMethods = stdMethodNames.Select(x => new Method(x, "X",
                methodLibrary.GetNotation(x), 8)).ToList();
            //int worthKeeping = int.MaxValue;

            // ensure all methods are fully initiallised
            {

                List<Task> tasks = stdMethods.Select(
                    m => new Task(() =>
                        {
                            int x = m.AllLeads.Count();
                        })).ToList();
                tasks.ForEach(t => t.Start());
                Task.WaitAll(tasks.ToArray(), int.MaxValue);
            }


            var rounds = new Row(8);
            var startMethodBits = stdMethods.Select(
                m =>
                {
                    var mb1 = new MethodBit(m, rounds);
                    return mb1;
                }
                ).ToList();
           
            //if (leadEnd.ToString() != "17856342")
            //                                {
            //                                    return "Wrong lead pattern";
            //                                //}
            var targetLeadEnd = Row.FromString("17856342");
            //1	8	6	7	4	5	2	3
            targetLeadEnd = Row.FromString("18674523");

            /*
            // ERKMEI
            int i1 = 2;
            int i2 = 15;
            int i3 = 19;
            int i4 = 13;
            int i5 = i1;
            int i6 = 18;
            // Uxbridge	Preston	Ipswich	Ipswich	Preston	Uxbridge
            i1 = i6 = 1;
            i2 = i5 = 17;
            i3 = i4 = 18;
             */
            {
                List<Task> tasks = new List<Task>();
                foreach (var mb1 in startMethodBits)
                {
                    var method1 = mb1.method;

                    var task = new Task(() =>
                    {
                        int worthKeeping = 1;
                        //

                        //Console.WriteLine("{0}", method1.Name);

                        for (int i2 = 0; i2 < stdMethods.Count && worthKeeping > 0; ++i2)
                        {
                            worthKeeping = 2;
                            var method2 = stdMethods[i2];
                            var mb2 = new MethodBit(method2, mb1.leadEnd1);
                            if (
                                            mb2.leadEnd1 != rounds )
                            for (int i3 = 0; i3 < stdMethods.Count && worthKeeping > 1; ++i3)
                            {
                                worthKeeping = 3;
                                var method3 = stdMethods[i3];
                                var mb3 = new MethodBit(method3, mb2.leadEnd1);
                                //Console.WriteLine("{0},{1},{2}",
                                //                        method1.Name, method2.Name, method3.Name); 
                                // check against previous leads - not checking against immediate one because only using normal methods so 
                                // lead alway goes somewhere
                                if (
                                            mb3.leadEnd1 != rounds  &&
                                            mb3.leadEnd1 != mb1.leadEnd1)
                                for (int i4 = 0; i4 < stdMethods.Count && worthKeeping > 2; ++i4)
                                {
                                    worthKeeping = 4;
                                    var method4 = stdMethods[i4];
                                    var mb4 = new MethodBit(method4, mb3.leadEnd1); 
                                    if (
                                            mb4.leadEnd1 != rounds && mb4.leadEnd1 != mb2.leadEnd1 &&
                                            mb4.leadEnd1 != mb1.leadEnd1)
                                    for (int i5 = 0; i5 < stdMethods.Count && worthKeeping > 3; ++i5)
                                    {
                                        worthKeeping = 5;
                                        var method5 = stdMethods[i5];
                                        var mb5 = new MethodBit(method5, mb4.leadEnd1);
                                        if (
                                            mb5.leadEnd1 != rounds && mb5.leadEnd1 != mb3.leadEnd1 && mb5.leadEnd1 != mb2.leadEnd1 &&
                                            mb5.leadEnd1 != mb1.leadEnd1 )
                                        for (int i6 = 0; i6 < stdMethods.Count && worthKeeping > 4; ++i6)
                                        {
                                            worthKeeping = 6;
                                            var method6 = stdMethods[i6];
                                            var mb6 = new MethodBit(method6, mb5.leadEnd1);



                                            if (mb6.leadEnd1 == targetLeadEnd)
                                            {
                                                try
                                                {
                                                    var search = new CyclicSearch3();
                                                    var res = search.Search(mb1, mb2, mb3, mb4, mb5, mb6, out worthKeeping);

                                                    //System.Diagnostics.Debug.WriteLine("{0},{1},{2},{3},{4},{5} = {6}",
                                                    //    method1.Name, method2.Name, method3.Name,
                                                    //    method4.Name, method5.Name, method6.Name,
                                                    //    res ?? "OK");
                                                    if (res == null)
                                                    {
                                                        var score = musicalPreferences.ScoreLead(search.rows);
                                                        lock (myLock)
                                                        {
                                                            if (score > bestScore)
                                                            {
                                                                bestScore = score;
                                                                Console.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8:F2}",
                                                                    method1.Name, method2.Name, method3.Name,
                                                                    method4.Name, method5.Name, method6.Name,
                                                                    search.rows.Count, score,
                                                                    (DateTime.UtcNow - start).TotalMinutes);
                                                            }
                                                        }
                                                        if (false)
                                                        {
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
                                                            }
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    System.Diagnostics.Debug.WriteLine("{0},{1},{2},{3},{4},{5} = {6}",
                                                      method1.Name, method2.Name, method3.Name,
                                                      method4.Name, method5.Name, method6.Name,
                                                  ex.Message);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });
                    tasks.Add(task);
                    task.Start();
                }

                Task.WaitAll(tasks.ToArray(), int.MaxValue);
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

       public CyclicSearch3()
       {

       }

        public class MethodBit
        {
            public Method method;
            public Lead lead1;
            public Row leadEnd1;
            public Row initialLeadEnd;
            
            public MethodBit(Method method, Row initialLeadEnd)
            {
                this.method = method;
                //this.initialLeadEnd = initialLeadEnd;
                this.lead1 = method.Lead(initialLeadEnd);
                this.leadEnd1 = lead1.NextLeadHead(method.PlainLeadEndPermutation);            }

            // Did not appear to exclude anything
            //internal bool IsTrueToCyclic()
            //{
            //    return lead1.IsTrueToCyclic();
            //}
        }

        public string Search(MethodBit method1,
MethodBit method2,
MethodBit method3,
MethodBit method4,
MethodBit method5,
MethodBit method6,
           out int worthKeeping)
       {

           Row leadEnd = new Row(8);
           worthKeeping = int.MaxValue;

          
           // now checked outside
           //if (leadEnd.ToString() != "17856342")
           //{
           //    return "Wrong lead pattern";
           //}
          

               //foreach (var perm in link)
               //{
               //    leadEnd = leadEnd.Apply(perm);
               //    //Account(leadEnd);
               //}

           leadEnd = new Row(8);

           //if ( leadEnd.ToString() != "17823456")
           //{
           //    return "Wrong part end after right before";
           //}
           for (int j = 0; j < 7; ++j)
           {
               foreach (var method in new[] { method1, method2, method3, method4, method5, method6 })
               {
                   Lead lead = method.method.Lead(leadEnd);
                   leadEnd = lead.NextLeadHead(method.method.PlainLeadEndPermutation);

                   //var restLines = method.CorePermuations.Apply(leadEnd);
                   //leadEnd = restLines.Last().Apply(method.PlainLeadEndPermutation);
                   foreach (var line in lead.RowsAsInts)
                   {
                      if ( ! Account(line) )
                      {

                          if ( j == 0 )
                          {
                              var linesDone = rows.Count;
                              worthKeeping = linesDone / 32;
                          }
                          return "False";
                      }
                   }
                   //if ( !Account(leadEnd.ToNumber()) )
                   //{
                   //    //if lead end is issue odd because did a pass on lead ends earlier
                   //    return "False";
                   //}

               } 
               foreach (var perm in link)
               {
                   if (!Account(leadEnd.ToNumber()))
                   {
                       return "False";
                   }
                   leadEnd = leadEnd.Apply(perm);
                  
               }

               //if (j == 0)
               //{
               //    //if (leadEnd.ToString() != "17823456")
               //        if (leadEnd.ToString() != "14567823")
               //    {
               //        throw new Exception("Part end = " + leadEnd);
               //    }
               ////}
           }
           List<Lead> leads = new List<Lead>(100);
           List<Row> leadEnds = new List<Row>(10);

           foreach (var method in new[] { method1, method2, method3, method4, method5, method6 })
           {
               //Lead lead = method.lead1;
               //leadEnd = method.leadEnd1;


               Lead lead = method.method.Lead(leadEnd);
               leadEnd = lead.NextLeadHead(method.method.PlainLeadEndPermutation);

               //this.lead1 = method.Lead(initialLeadEnd);
               //this.leadEnd1 = lead1.NextLeadHead(method.PlainLeadEndPermutation);     

               // now checked outside
               //for ( int i = 0; i < leads.Count; ++i )
               //{
               //    if ( leads[i].LeadHead().ToNumber() == leadEnd.ToNumber())
               //    {
               //        worthKeeping = leads.Count;
               //        return "False in plain course";
               //    }
               //}

               leadEnds.Add(leadEnd);
               leads.Add(lead);
           }

           int baseScore = 0;
           for (int l = 0; l < leadEnds.Count; ++l)
           {
               baseScore += leads[l].CyclicScore(musicalPreferences, leadEnds[l]);
           }
           ////Console.WriteLine(baseScore);

           //Console.WriteLine("{0},{1},{2},{3},{4},{5},{6}",
           //    baseScore,
           //    method1.Name, method2.Name, method3.Name,
           //    method4.Name, method5.Name, method6.Name);
           //if (baseScore < 450)
           //{
           //    return "Not musical enough";
           //}

           return null;
       }

       private bool Account(int asNum)
        {
            ++rowNum;
            //var asNum = line.ToNumber();
            if ( rowsRung[asNum])
            {
                return false;
                //throw new Exception(string.Format("False at {0} on {1}", rowNum, line));
            }
            rowsRung[asNum] = true;
            rows.Add(Row.FromNumber(asNum));
            return true;
        }
    }
}
