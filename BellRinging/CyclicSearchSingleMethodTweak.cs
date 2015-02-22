using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BellRinging
{
    /// <summary>
    /// Hunt for 6 leads - cyclic tweaked to semi-efficiently look for single method compositions
    /// </summary>
    public class CyclicSearchSingleMethodTweak
    {
        static MethodLibrary methodLibrary = new MethodLibrary();

        List<Row> rows = new List<Row>(2000);
        int rowNum;
        int leadForSplice = -1;

        static int bestScore = 0;
        static  object myLock = new object();
        static MusicalPreferences musicalPreferences;

        static List<Permutation> link = new List<Permutation>();

        //static Method linkMethod = new Method("Link", "L", "3.4.3.4.3.4", 8);

        static HashSet<Row> cyclicPartEnds;
            
        public static void Checks()
        {
            cyclicPartEnds = new HashSet<Row>();
            cyclicPartEnds.Add(Row.FromString("13456782"));
            cyclicPartEnds.Add(Row.FromString("14567823"));
            cyclicPartEnds.Add(Row.FromString("15678234"));
            cyclicPartEnds.Add(Row.FromString("16782345"));
            cyclicPartEnds.Add(Row.FromString("17823456"));
            cyclicPartEnds.Add(Row.FromString("18234567"));

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
"London","Rutland"
/*,
                   //STD 8
                    "Rutland", "Cassiobury", "Cornwall",   "Bristol", 
                     "Yorkshire","Cambridge", "Superlative" ,
                    "Lincolnshire", "Pudsey"  ,"Jersey", "Preston"
                   //,
                    , "Lindum", "Ashtead","Uxbridge"
                   ,  "Glasgow"
                    // STD 8 - wrong
                    ,"London","Double Dublin","Belfast"
                 /// SMITHS
                    //,
                    ,"Whalley" ,
                    "Watford", "Tavistock",
                    "Wembley",
                   "Ipswich", "Cray",
                    // 4-SPLICED
                    //,
                    // Promising
                    "Norfolk",
                    // JIM TAYLOR
                    "Yorkhouse", "Rushmoor", "Silchester", "Londonderry", "Hersham", "Brockley",
                    // CHANDLERS
                    "Huddersfield", "Malpas", "Essex", "Caterham","Buckfastleigh","Yeading","Chertsey","Sussex","Colnbrook","Sonning","Moulton","Richmond",
                    "Newcastle","Willesden","Chesterfield","Northampton","Claybrooke","Newlyn"*/
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

            var bobLeadEndPermutation = Permutation.FromPlaceNotation("14", 8);
            var singleLeadEndPermutation = Permutation.FromPlaceNotation("1234", 8);
            var rounds = new Row(8);
            var startMethodBits = stdMethods.SelectMany(
                m =>
                    new[] { m.PlainLeadEndPermutation, bobLeadEndPermutation, singleLeadEndPermutation
                     }.
                        Select(perm =>
                        {
                            var mb1 = new MethodBit(m, rounds, perm);
                            return mb1;
                        })
                
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
                foreach (var method1_ext in startMethodBits)
                {
                    var method1 = method1_ext;
                    var mb1 = method1;
                    var task = new Task(() =>
                    {
                        int truthNumber = 0;
                        int[] truthTable = new int[40320];
                        int worthKeeping = 2;
                        //

                        //Console.WriteLine("{0}", method1.Name);

                        for (int i2 = 3; i2 < startMethodBits.Count && worthKeeping > 1; ++i2)
                        {
                            worthKeeping = 3;
                            var method2 = startMethodBits[i2];
                            var mb2 = new MethodBit(method2, mb1.leadEnd1);
                            if (
                                            mb2.leadEnd1 != rounds)
                               // for (int i3 = 0; i3 < startMethodBits.Count && worthKeeping > 2; ++i3))
                                for (int i3 = i2-3; i3 < startMethodBits.Count && worthKeeping > 2 && i3 < i2 + 3; ++i3)
                            {
                                worthKeeping = 4;
                                var method3 = startMethodBits[i3];
                                var mb3 = new MethodBit(method3, mb2.leadEnd1);
                                //Console.WriteLine("{0},{1},{2}",
                                //                        method1.Name, method2.Name, method3.Name); 
                                // check against previous leads - not checking against immediate one because only using normal methods so 
                                // lead alway goes somewhere
                                if (
                                            mb3.leadEnd1 != rounds  &&
                                            mb3.leadEnd1 != mb1.leadEnd1)
                                    for (int i4 = i2-3; i4 < startMethodBits.Count && worthKeeping > 3 && i4 < i2 + 3; ++i4)
                                {
                                    worthKeeping = 5;
                                    var method4 = startMethodBits[i4];
                                    var mb4 = new MethodBit(method4, mb3.leadEnd1); 
                                    if (
                                            mb4.leadEnd1 != rounds && mb4.leadEnd1 != mb2.leadEnd1 &&
                                            mb4.leadEnd1 != mb1.leadEnd1)
                                        for (int i5 = i2-3; i5 < startMethodBits.Count && worthKeeping > 4 && i5 < i2 + 3; ++i5)
                                    {
                                        worthKeeping = 6;
                                        var method5 = startMethodBits[i5];
                                        var mb5 = new MethodBit(method5, mb4.leadEnd1);
                                        if (
                                            mb5.leadEnd1 != rounds && mb5.leadEnd1 != mb3.leadEnd1 && mb5.leadEnd1 != mb2.leadEnd1 &&
                                            mb5.leadEnd1 != mb1.leadEnd1 )
                                            for (int i6 = i2-3; i6 < startMethodBits.Count && worthKeeping > 5 && i6 < i2 + 3; ++i6)
                                        {
                                            //worthKeeping = 6;
                                            var method6 = startMethodBits[i6];
                                            var mb6 = new MethodBit(method6, mb5.leadEnd1);

                                            var methodBits = new[] { method1, method2, method3, method4, method5, method6 };

                                            if ( cyclicPartEnds.Contains(mb6.leadEnd1) && methodBits.Select(m=>m.method ).Distinct().Count() == 1 )
                                            {
                                                try
                                                {
                                                    //System.Diagnostics.Debug.WriteLine("{0},{1},{2},{3},{4},{5} ",
                                                    //    method1.Name, method2.Name, method3.Name,
                                                    //    method4.Name, method5.Name, method6.Name);
                                                    int score = 0;
                                                    foreach (var method in methodBits)
                                                    {
                                                        //Lead lead = method.lead1;
                                                        //leadEnd = method.leadEnd1;


                                                        Lead lead = method.lead1;
                                                        var leadEnd = method.leadEnd1;
                                                        score += lead.CyclicScore(musicalPreferences, leadEnd);


                                                    }
                                                    //if (score > bestScore)
                                                    {


                                                        var search = new CyclicSearchSingleMethodTweak();
                                                        var res = search.Search(mb1, mb2, mb3, mb4, mb5, mb6, out worthKeeping,
                                                            truthTable, ref truthNumber);

                                                        //System.Diagnostics.Debug.WriteLine("{0},{1},{2},{3},{4},{5} = {6}",
                                                        //    method1.Name, method2.Name, method3.Name,
                                                        //    method4.Name, method5.Name, method6.Name,
                                                        //    res ?? "OK");
                                                        if (res == null)
                                                        {
                                                            //var score = musicalPreferences.ScoreLead(search.rows);
                                                            lock (myLock)
                                                            {
                                                                if (score > bestScore)
                                                                {
                                                                    bestScore = score;
                                                                    
                                                                }
                                                                Console.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8:F2},{9}",
                                                                        method1.Name, method2.Name, method3.Name,
                                                                        method4.Name, method5.Name, method6.Name,
                                                                        search.rows.Count, score,
                                                                        (DateTime.UtcNow - start).TotalMinutes,
                                                                        mb6.leadEnd1);
                                                            }
                                                            if (true)
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


        public class MethodBit
        {
            public Method method;
            public Lead lead1;
            public Row leadEnd1;
            //public Row initialLeadEnd;
            public Permutation perm;
            public int cyclicScore;

            public string Name { get { return method.Name + " " + perm.ToString(); } }
            
            public MethodBit(Method method, Row initialLeadEnd,Permutation perm)
            {
                this.method = method;
                this.perm = perm;
                //this.initialLeadEnd = initialLeadEnd;
                this.lead1 = method.Lead(initialLeadEnd);
                this.leadEnd1 = lead1.NextLeadHead(perm); 
            }

            public MethodBit(MethodBit methodBet, Row initialLeadEnd)
            {
                this.method = methodBet.method;
                this.perm = methodBet.perm;
                //this.initialLeadEnd = initialLeadEnd;
                this.lead1 = method.Lead(initialLeadEnd);
                this.leadEnd1 = lead1.NextLeadHead(perm);
            }

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
           out int worthKeeping,
            int [] truthTable, ref int truthNumber
           )
       {

           ++truthNumber;
            if ( truthNumber == int.MaxValue)
            {
                for ( int idx = 0; idx < truthTable.Length; ++idx)
                {
                    truthTable[idx] = 0;
                }
                truthNumber = 1;
            }

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
                   leadEnd = lead.NextLeadHead(method.perm);

                   //var restLines = method.CorePermuations.Apply(leadEnd);
                   //leadEnd = restLines.Last().Apply(method.PlainLeadEndPermutation);
                   foreach (var line in lead.RowsAsInts)
                   {
                       ++rowNum;
                       //var asNum = line.ToNumber();
                       if (truthTable[line] == truthNumber)
                       {
                           if (j == 0)
                           {
                               var linesDone = rows.Count;
                               worthKeeping = linesDone / 32;
                           }
                           return "False";
                       }
                       truthTable[line] = truthNumber;
                       rows.Add(Row.FromNumber(line));

                   }
                   //if ( !Account(leadEnd.ToNumber()) )
                   //{
                   //    //if lead end is issue odd because did a pass on lead ends earlier
                   //    return "False";
                   //}

               } 
               //foreach (var perm in link)
               //{
               //    if (!Account(leadEnd.ToNumber()))
               //    {
               //        return "False";
               //    }
               //    leadEnd = leadEnd.Apply(perm);
                  
               //}

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

      
    }
}
