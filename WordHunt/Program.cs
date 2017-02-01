using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using BellRinging;

namespace WordHunt
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {

            musicalPreferences.Init();
            InitProblem();
            InitTables();

            HashSet<string> strings = new HashSet<string>();
            //using (var wordReader = new StreamReader(@"C:\Users\rbso\Documents\words.txt"))
            using (var wordReader = new StreamReader(@"C:\Users\rbso\Documents\50kgaz2016.txt"))
            {
                string line;
                while ((line = wordReader.ReadLine()) != null)
                {
                    foreach ( var s in line.Split(':'))
                    {
                        strings.Add(s);
                    }
                }
            }
                foreach ( var place in strings.OrderBy(x =>x))
                { 
                ProcessLine(place.ToLower());}
            
        }

        // G = Plain Bob = 1
        // RL ... = -1
        // C etc = 2
        // A = 4
        // O = 3
        // E = 5
         string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
         string index = "46265612425662320623666020";

         int AtoN(char c)
        {
            int ret = 0;
            var n = c - 'a';
            if ( n >= 0 && n < index.Length)
            {
                ret = index[n] - '0';
            }
            return ret;
        }
        private  void ProcessLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return;

            var score = line.Select(c => AtoN(c));
            var total = score.DefaultIfEmpty(0).Sum();
            if ( total % 7 == 0 )
            {
                bool[] leads = new bool[8];
                int n = 0;
                foreach ( var lead in score )
                {
                    // unknown method
                    if (lead == 0) return;
                    n += lead;
                    n = n % 7;
                    if ( leads[n]) return; // false
                    leads[n] = true;
                }
                var comp = Process(line.ToUpper());
                if (!comp.Contains("FALSE"))
                {
                    Console.WriteLine(line);
                }
            }
        }


        MethodLibrary lib = new MethodLibrary();
        MusicalPreferences musicalPreferences = new MusicalPreferences();
        Problem problem;
        Tables tables;

        void InitProblem()
        {
            // set up a problem that will create bits for anything we might put in the composition
            problem = new Problem();
            problem.MusicalPreferences = musicalPreferences;
            problem.Reverse = false;
            problem.TenorsTogether = false;
            problem.AllowSingles = true;
            problem.MaxLeads = 5040 / 32; // need this so have choices in tables
            problem.BlockLength = 0;
        }
        void InitTables()
        {


            tables = new Tables();


            foreach (string method in
          new string[] {
              "Superlative",
              "London", 
              "Pudsey",
              "Yorkshire",
              "Glasgow" ,
         "Cambridge" ,
         "Bristol",
         "Lincolnshire",
         "Rutland",
         "Belfast",
         "Double Dublin",
         "Cassiobury",
         "Cornwall",
         "Ipswich",
         "Uxbridge",
         "Whalley",
         "Watford",
         "Wembley",
         "Lindum",
         "Cray",
         "Jersey",
         "Preston",
         "Ashtead", "Tavistock"
              //,
              //"Bastow"
             // "Superlative",
            // "Bastow"//,"Bastow"
          
         // ,"Superlative"
         // 
         // ,"London"
         // ,"Cambridge"
         // ,"Bristol"
         //, "Pudsey", "Lincolnshire"
         // ,"Rutland"
          //"Belfast", //"Glasgow",
          //"Londonderry",
           })
            {
                char letter = method[0];
                if (method == "Lincolnshire") letter = 'N';
                if (method == "Belfast") letter = 'F';
                if (method == "Cray") letter = 'K';
                if (method == "Lindum") letter = 'M';
                if (method == "Wembley") letter = 'X';
                if (method == "Cassiobury") letter = 'O';
                if (method == "Preston") letter = 'H';
                if (method == "Whalley") letter = 'V';
                if (method == "Cornwall") letter = 'E';
                Method methodObject;
                if (method == "Bastow")
                {

                    methodObject = new Method(method, "β", "X12-18", 8, problem.AllowSingles);
                }
                else
                {

                    methodObject = new Method(method, letter.ToString(), lib.GetNotation(method), 8, problem.AllowSingles);
                }
                if (method == "Bastow")
                {
                    //methodObject.FirstLeadOnly();
                    methodObject.LastLeadOnly();
                }
                problem.AddMethod(methodObject);
            }

            tables.Initialise(problem);
        }
        private string Process(string p)
        {



            var comp = new Composition(tables);
            comp.Problem = problem;

            comp.maxLeadIndex = -1;
            //for (int i = 0; i < 7; ++i)
            {
                int lineNo = 0;
                foreach (var token in CompositionTokenizer.Tokenize(p))
                {
                    ++lineNo;
                    try
                    {
                        var methChar = token[0];
                        var method = problem.Methods.FirstOrDefault(m => m.Letter == methChar.ToString());
                        int methodIndex = problem.Methods.ToList().IndexOf(method);
                        int choice = methodIndex * 3;
                        if (token.EndsWith("-"))
                        {
                            ++choice;
                        }
                        if (token.EndsWith("s"))
                        {
                            choice += 2;
                        }
                        comp.choices[++comp.maxLeadIndex] = choice;
                    }
                    catch (Exception ex)
                    {
                        return string.Format("Error at token {0} : '{1}'\r\n\r\n{2}",
                            lineNo, token, ex);
                    }
                }
                comp.Problem.BlockLength = lineNo; // for block compositions
                comp.Problem.BlockLength = 1;
            }

            return comp.WriteDetails();
        }
    }
}
