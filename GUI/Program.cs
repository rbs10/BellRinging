using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GUI
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      //Application.Run(new Form1());

      Form form = null;
      foreach (string method in
      new string[] { 

       // "Cooktown Orchid", ### DELIGHT! ###
       // "Frodsham", "Turramurra"

        //"Eggybread",
        "xxx"
        // "Deva", "Bristol", "Yorkshire", "Cambridge", "Superlative", "London", "Lessness", "Cornwall"
         //, "Bristol", "Deva", "Cornwall", "Lessness", "Superlative", "Cambridge", "London"
         //, "Superlative", "Cambridge", "Lincolnshire", "Pudsey", "Bristol",
        //  "Ashtead", "Cassiobury", "Cornwall","Tavistock",
        //  "Jersey",
        //  "Glasgow",
        //  "Dereham", "Deva",
        //  "Uxbridge", "Lindum", "London", "Lessness", "Watford", "Wembley", "Rutland"

           //"Yorkshire", "Superlative","Cambridge",
           //, "Superlative","Bristol",
          //"Superlative"
           //"Cray", "Yorkshire", 
           //"Venusium",
          // "Deva" 
           //"Bristol"
           //,  "Cornwall"

          // "Ashtead", "Cassiobury", "Cornwall", "Lessness", "Uxbridge", "Tavistock", "Northampton", "Cray"

          //"Glasgow",
          //"Yorkshire",
          //"London", 
          //"Cambridge", 
          //"Bristol",
          //"Pudsey", "Lincolnshire", "Superlative",
          //"Rutland",
          //"Belfast"
           })
      {
           form = new Form1(method);
           form.Text = method + "";
          form.Show();
      }

      Application.Run(form);
    }
  }
}