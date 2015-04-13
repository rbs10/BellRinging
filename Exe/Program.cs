using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

using BellRinging;

namespace Exe
{
  class Program : ICompositionReceiver
  {
    string method;

    static void Main(string[] args)
    {
        var patterns = Enumerable.Range(0,2).SelectMany(upDown => Enumerable.Range(4,5).Select(len =>
            new MusicalChangeRun(1,len.ToString() +( upDown > 0 ? "Up" : "Down"),len,upDown > 0 ? 1 : -1))).ToList();

        foreach ( var pattern in patterns )
        {
            Console.WriteLine("{0} - {1}", pattern.Name, Row.AllRows.Count(r => pattern.Score(r) > 0));
        }
        Console.WriteLine();

        var rows = Row.AllRows.Where(row => patterns.Any(pattern => pattern.Score(row) > 0));
        Console.WriteLine();
        Console.WriteLine(rows.Count());
        foreach ( var row in rows )
        {
            Console.WriteLine(row.ToString() + " - " + string.Join(",", patterns.Where(pattern => pattern.Score(row) > 0).Select(p => p.Name)));
        }
        Console.WriteLine("Press any key");
        Console.ReadLine();
        return;
      SimpleComposer c = new SimpleComposer();
      c.Initialise("Bristol");
      var tables = c.Tables;

      Console.WriteLine("ComputeFalsenessTable");
      tables.ComputeFalsenessTable();

        int at = 0;
        int REPS = 100000;

        Console.WriteLine("Starting main tests");

        for ( int i = 0; i < 5; ++i )
        {
            for ( int alg = 2; alg <5; ++alg )
            {

                var comp = CreateComposition(tables);
                var sw = new Stopwatch();
                sw.Start();
                switch( alg )
                {
                    case 0:
                        for ( int rep = 0; rep < REPS; ++rep )
                        {
                            int ful = -1;
                            at = comp.RunsFalseAt(ref ful);
                        }
                        break;
                    case 1:
                        for (int rep = 0; rep < REPS; ++rep)
                        {
                            int ful = -1;
                            at = comp.RunsFalseAt2(ref ful);
                        }
                        break;
                    case 2:
                        for (int rep = 0; rep < REPS; ++rep)
                        {
                            int ful = -1;
                            at = comp.RunsFalseAt3(ref ful);
                        }
                        break;
                    case 3:
                        for (int rep = 0; rep < REPS; ++rep)
                        {
                            int ful = -1;
                            at = comp.RunsFalseAt4(ref ful);
                        }
                        break;
                    case 4:
                        for (int rep = 0; rep < REPS; ++rep)
                        {
                            int ful = -1;
                            at = comp.RunsFalseAt5(ref ful);
                        }
                        break;
                    case 999:
                        // falseness tables
                        for (int rep = 0; rep < REPS; ++rep)
                        {
                            int ful = -1;
                            at = comp.RunsFalseAt1(ref ful);
                        }
                        break;
                    default:
                        throw new Exception("Unhandled algorithm");
                }
                sw.Stop();
                var t = sw.ElapsedMilliseconds;
                Console.WriteLine("ALG {0} T = {1} False at {2}", alg, t, at);
            }
        }


        //    var str = _composition.ToString();

        //    int ful2 = -1;
        //    var false2 = _composition.RunsFalseAt(ref ful2);
        //    Console.WriteLine("Hi");
      ////{
      //  RotationalBlockComposer c = new RotationalBlockComposer();
      //  c.Initialise("Cambridge", "X38X14X1258X36X14X58X16X78-12");
      //  c.Compose();

      //  Console.WriteLine("Done");
      //  while (true) System.Threading.Thread.Sleep(10000);
      //new Program().Execute();
      //}
    }

    private static Composition CreateComposition(Tables tables)
    {
        var comp = new Composition(tables);
        var choices = comp.choices;
        var leads = comp.leads;

        string magic = "000012112000202000121120002010001211200012";
        //      //"112000202000121120002010001211200012000012";
        int p = 0;
        foreach (char ch in magic)
        {
            choices[p] = ch - '0';
            if (p + 1 < leads.Length)
            {
                leads[p + 1] = tables.leadMapping[leads[p], choices[p]];
            }
            ++p;
        }

        comp.maxLeadIndex = 41;
        return comp;
    }

    void Execute()
    {
        //Console.WriteLine(new MethodLibrary().GetNotation("Cambridge"));

        MethodLibrary lib = new MethodLibrary();
        foreach (string method in new string[] { "Deva", "Cambridge", "Yorkshire", "Pudsey", "Lincolnshire", "Superlative",
          "London", "Rutland", "Bristol", "Belfast", "Glasgow", "Lessness", "Venusium", "Dover" })
        {
          this.method = method;
          SingleMethodMostMusicalComposer c = new SingleMethodMostMusicalComposer();
          c.Initialise(method, lib.GetNotation(method));
          c.SyncCompose();
          c.Receiver = this;
          c.SyncCompose();
        }

        Console.WriteLine("Done");
        while (true) System.Threading.Thread.Sleep(10000);
    }

    #region ICompositionReceiver Members

    public void AddComposition(Composition c)
    {
      Console.WriteLine(string.Format("{0} {3} {1} {2}", method, c.Music, c, c.Changes));
    }

    #endregion
  }
}
