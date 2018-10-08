using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BellRinging;
using System.IO;

namespace ManualCheck
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


            musicalPreferences.Init();
            InitProblem();
            InitTables();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Text = Process(textBox1.Text);
            }
            catch ( Exception ex )
            {
                textBox2.Text = ex.ToString();
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
        void  InitTables()
        {


             tables = new Tables();


            foreach (string method in
          new string[] {
            "Deva",
              "Superlative",
              "London", 
              "Pudsey",
              "Yorkshire",
              "Glasgow" ,
         "Cambridge" ,
         "Bristol",
         "Lincolnshire",
         "Rutland",
         "Belfast"
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
