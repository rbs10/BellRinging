using System;
using System.Collections.Generic;
using System.Text;

using BellRinging;

namespace Exe
{
  class Program : ICompositionReceiver
  {
    string method;

    static void Main(string[] args)
    {
      SimpleComposer c = new SimpleComposer();
      c.InitialiseStandardMethods();
      c.Compose();
      return;
      ////{
      //  RotationalBlockComposer c = new RotationalBlockComposer();
      //  c.Initialise("Cambridge", "X38X14X1258X36X14X58X16X78-12");
      //  c.Compose();

      //  Console.WriteLine("Done");
      //  while (true) System.Threading.Thread.Sleep(10000);
      //new Program().Execute();
      //}
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
