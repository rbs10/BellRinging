using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BellRinging
{
    public static class StringExt
    {
        public static string Reverse(this string s)
        {
            return string.Join("", s.OfType<char>().Reverse());
        }
    }
  public class MusicalPreferences
  {
    List<IMusicalChange> _musicalChanges = new List<IMusicalChange>();
    string partMapping = null;

    public MusicalPreferences()
    {
        IgnoreRounds = false;
    }

      public bool IgnoreRounds { get; set; }
    public void InitELF()
    {
      _musicalChanges.Add(new MusicalChange("xxxx5678",1,"Back rollups"));
        _musicalChanges.Add(new MusicalChange("xxxx6578",1,"Back rollups")); 
          _musicalChanges.Add(new MusicalChange("xxxx8765",1,"Back rollups"));


          _musicalChanges.Add(new MusicalChange("5678xxxx", 1, "Front rollups"));
          _musicalChanges.Add(new MusicalChange("8765xxxx", 1, "Front rollups"));

          _musicalChanges.Add(new MusicalChange("2345xxxx", 1, "Little-bell"));
          _musicalChanges.Add(new MusicalChange("5432xxxx", 1, "Little-bell"));
          _musicalChanges.Add(new MusicalChange("xxxx2345", 1, "Little-bell"));
          _musicalChanges.Add(new MusicalChange("xxxx5432", 1, "Little-bell"));

          _musicalChanges.Add(new MusicalChange("xxxx2468", 1, "468s"));
          _musicalChanges.Add(new MusicalChange("xxxx3468", 1, "468s"));
      _musicalChanges.Add(new MusicalChange("13572468", 2, "Queens"));
      _musicalChanges.Add(new MusicalChange("12753468", 2, "Whittingtons"));

      //COM2, BAL2

    }

    int[] scores = new int[40320];
      public void Init()
    {

        Init2016();

        //InitCyclic4part();

          foreach ( var row in Row.AllRows)
          {
              scores[row.ToNumber()] = Score(row);
          }
       // InitSJT();
    }

      void
        InitCyclic4part()
      {
          Init2500();
          partMapping = "34567812";
      }
      void
        InitCyclic8part()
      {
          Init2500();
          partMapping = "23456781";
      }

    /* Stephen's musical changes */
    public void InitSJT()
    {


        _musicalChanges.Add(new MusicalChange("xxxx5678", 10, "CRU+"));
        _musicalChanges.Add(new MusicalChange("xxxx5678", 10, "CRU"));
        _musicalChanges.Add(new MusicalChange("xxxx4678", 5, "CRU"));
        _musicalChanges.Add(new MusicalChange("xxxx6478", 3, "CRU"));
        _musicalChanges.Add(new MusicalChange("xxxx5478", 3, "CRU"));
        _musicalChanges.Add(new MusicalChange("xxxx4578", 3, "CRU"));
        _musicalChanges.Add(new MusicalChange("xxxx6578", 5, "CRU"));

        ///* queens, kings, whittingtons, tittums */
        _musicalChanges.Add(new MusicalChange("13572468", 10, "Queens"));
        _musicalChanges.Add(new MusicalChange("17532468", 10, "Kings"));
        _musicalChanges.Add(new MusicalChange("12753468", 10, "Whittingtons"));
        _musicalChanges.Add(new MusicalChange("15263748", 10, "Tittums"));

        _musicalChanges.Add(new MusicalChange("xxxxx468", 1, "xxxxx468"));
        _musicalChanges.Add(new MusicalChange("xxxx7568", 1, "xxxx7568"));
        _musicalChanges.Add(new MusicalChange("xxxx7658", 1, "xxxx7658"));
        _musicalChanges.Add(new MusicalChange("xxxx2468", 5, "Queens back"));
        _musicalChanges.Add(new MusicalChange("xxxx8765", 5, "xxxx8765"));

        ///* roll-ups/downs off the front */
        ///
        _musicalChanges.Add(new MusicalChange("5678xxxx", 1, "5678xxxx"));
        _musicalChanges.Add(new MusicalChange("8765xxxx", 3, "8765xxxx"));
        _musicalChanges.Add(new MusicalChange("7568xxxx", 1, "7568xxxx"));
        _musicalChanges.Add(new MusicalChange("45678xxx", 5, "45678xxx"));
        _musicalChanges.Add(new MusicalChange("345678xx", 7, "345678xx"));
        _musicalChanges.Add(new MusicalChange("2345678x", 1, "2345678x")); // fix ?


        _musicalChanges.Add(new MusicalChangeRun(1, "Run up 4", 4, 1));
        _musicalChanges.Add(new MusicalChangeRun(1, "Run down 4", 4, -1));


        _musicalChanges.Add(new MusicalChangeRun(4, "Run up 5", 5, 1));
        _musicalChanges.Add(new MusicalChangeRun(4, "Run down 5", 5, -1));


        _musicalChanges.Add(new MusicalChangeRun(6, "Run up 6", 6, 1));
        _musicalChanges.Add(new MusicalChangeRun(6, "Run down 6", 6, -1));


        _musicalChanges.Add(new MusicalChangeRun(8, "Run up 7", 7, 1));
        _musicalChanges.Add(new MusicalChangeRun(8, "Run down 7", 7, -1));


        _musicalChanges.Add(new MusicalChangeRun(10, "Run up", 8, 1));
        _musicalChanges.Add(new MusicalChangeRun(10, "Run down", 8, -1));

        _musicalChanges.Add(new MusicalChange5678Together(1, "xxxxBBBB", 0));
        _musicalChanges.Add(new MusicalChange5678Together(1, "FFFFxxxx", 4));
    }
    /* Stephen's musical changes */
    public void InitSJT_S310()
    {

      _musicalChanges.Add(new MusicalChange("xxxx5678", 1, "CRU"));
      _musicalChanges.Add(new MusicalChange("xxxx4678", 1, "CRU"));
      _musicalChanges.Add(new MusicalChange("xxxx6478", 1, "CRU"));
      _musicalChanges.Add(new MusicalChange("xxxx5478", 1, "CRU"));
      _musicalChanges.Add(new MusicalChange("xxxx4578", 1, "CRU"));
      _musicalChanges.Add(new MusicalChange("xxxx6578", 1, "CRU"));

      ///* queens, kings, whittingtons, tittums */
      _musicalChanges.Add(new MusicalChange("13572468", 1, "Queens"));
      _musicalChanges.Add(new MusicalChange("17532468", 1, "Kings"));
      _musicalChanges.Add(new MusicalChange("12753468", 1, "Whittingtons"));
      _musicalChanges.Add(new MusicalChange("15263748", 1, "Tittums"));

      _musicalChanges.Add(new MusicalChange("xxxxx468", 1, "Other"));
      _musicalChanges.Add(new MusicalChange("xxxx7568", 1, "Other"));
      _musicalChanges.Add(new MusicalChange("xxxx7658", 1, "Other"));
      _musicalChanges.Add(new MusicalChange("xxxx2468", 1, "Other"));
      _musicalChanges.Add(new MusicalChange("xxxx8765", 1, "Other"));

      ///* roll-ups/downs off the front */
      ///
      _musicalChanges.Add(new MusicalChange("5678xxxx", 1, "Rollup/down off front"));
      _musicalChanges.Add(new MusicalChange("8765xxxx", 1, "Rollup/down off front"));
      _musicalChanges.Add(new MusicalChange("7568xxxx", 1, "Rollup/down off front"));
      _musicalChanges.Add(new MusicalChange("45678xxx", 1, "Rollup/down off front"));
      _musicalChanges.Add(new MusicalChange("345678xx", 1, "Rollup/down off front"));
      _musicalChanges.Add(new MusicalChange("2345678x", 1, "Rollup/down off front")); // fix ?


      _musicalChanges.Add(new MusicalChangeRun(1, "Run up", 4, 1));
      _musicalChanges.Add(new MusicalChangeRun(1, "Run down", 4, -1));


      _musicalChanges.Add(new MusicalChange5678Together(1, "xxxxBBBB", 0));
      _musicalChanges.Add(new MusicalChange5678Together(1, "FFFFxxxx", 4));
    }

      public void Init2015()
    {
        IgnoreRounds = true;

        _musicalChanges.Add(new MusicalChange("21345678", 10, "Near miss"));
        _musicalChanges.Add(new MusicalChange("13245678", 10, "Near miss"));
        _musicalChanges.Add(new MusicalChange("12435678", 10, "Near miss"));
        _musicalChanges.Add(new MusicalChange("12354678", 10, "Near miss"));
        _musicalChanges.Add(new MusicalChange("12346578", 10, "Near miss"));
        _musicalChanges.Add(new MusicalChange("12345768", 10, "Near miss"));
        _musicalChanges.Add(new MusicalChange("12345687", 10, "Near miss"));

        ///* queens, kings, whittingtons, tittums */
        _musicalChanges.Add(new MusicalChange("13572468", 10, "Queens"));
        _musicalChanges.Add(new MusicalChange("17532468", 10, "Kings"));
        _musicalChanges.Add(new MusicalChange("12753468", 10, "Whittingtons"));
        _musicalChanges.Add(new MusicalChange("15263748", 10, "Tittums"));

        _musicalChanges.Add(new MusicalChange("xxxx2468", 5, "Queens back"));

        _musicalChanges.Add(new MusicalChangeRun(1, "Run up 4", 4, 1));
        _musicalChanges.Add(new MusicalChangeRun(1, "Run down 4", 4, -1));


        _musicalChanges.Add(new MusicalChangeRun(4, "Run up 5", 5, 1));
        _musicalChanges.Add(new MusicalChangeRun(4, "Run down 5", 5, -1));


        _musicalChanges.Add(new MusicalChangeRun(6, "Run up 6", 6, 1));
        _musicalChanges.Add(new MusicalChangeRun(6, "Run down 6", 6, -1));


        _musicalChanges.Add(new MusicalChangeRun(8, "Run up 7", 7, 1));
        _musicalChanges.Add(new MusicalChangeRun(8, "Run down 7", 7, -1));


        _musicalChanges.Add(new MusicalChangeRun(10, "Run up", 8, 1));
        _musicalChanges.Add(new MusicalChangeRun(10, "Run down", 8, -1));
    }

      public void Init2500()
      {
          IgnoreRounds = true;

          _musicalChanges.Add(new MusicalChange("21345678", 5, "Near miss"));
          _musicalChanges.Add(new MusicalChange("13245678", 5, "Near miss"));
          _musicalChanges.Add(new MusicalChange("12435678", 5, "Near miss"));
          _musicalChanges.Add(new MusicalChange("12354678", 5, "Near miss"));
          _musicalChanges.Add(new MusicalChange("12346578", 5, "Near miss"));
          _musicalChanges.Add(new MusicalChange("12345768", 5, "Near miss"));
          _musicalChanges.Add(new MusicalChange("12345687", 5, "Near miss"));


          _musicalChanges.Add(new MusicalChange("21345678".Reverse(), 5, "Near reverse miss"));
          _musicalChanges.Add(new MusicalChange("13245678".Reverse(), 5, "Near reverse miss"));
          _musicalChanges.Add(new MusicalChange("12435678".Reverse(), 5, "Near reverse miss"));
          _musicalChanges.Add(new MusicalChange("12354678".Reverse(), 5, "Near reverse miss"));
          _musicalChanges.Add(new MusicalChange("12346578".Reverse(), 5, "Near reverse miss"));
          _musicalChanges.Add(new MusicalChange("12345768".Reverse(), 5, "Near reverse miss"));
          _musicalChanges.Add(new MusicalChange("12345687".Reverse(), 5, "Near reverse miss"));

          /////* queens, kings, whittingtons, tittums */
          _musicalChanges.Add(new MusicalChange("13572468", 1, "Queens"));
          _musicalChanges.Add(new MusicalChange("17532468", 1, "Kings"));
          _musicalChanges.Add(new MusicalChange("12753468", 1, "Whittingtons"));
          _musicalChanges.Add(new MusicalChange("15263748", 1, "Tittums"));

          _musicalChanges.Add(new MusicalChange("xxxx2468", 1, "Queens back"));

          _musicalChanges.Add(new MusicalChangeRun(1, "Run up 4", 4, 1));
          _musicalChanges.Add(new MusicalChangeRun(1, "Run down 4", 4, -1));


          _musicalChanges.Add(new MusicalChangeRun(2, "Run up 5", 5, 1));
          _musicalChanges.Add(new MusicalChangeRun(2, "Run down 5", 5, -1));


          _musicalChanges.Add(new MusicalChangeRun(3, "Run up 6", 6, 1));
          _musicalChanges.Add(new MusicalChangeRun(3, "Run down 6", 6, -1));


          _musicalChanges.Add(new MusicalChangeRun(4, "Run up 7", 7, 1));
          _musicalChanges.Add(new MusicalChangeRun(4, "Run down 7", 7, -1));


          _musicalChanges.Add(new MusicalChangeRun(5, "Run up 8", 8, 1));
          _musicalChanges.Add(new MusicalChangeRun(5, "Run down 8", 8, -1));
      }


      public void Init2016()
      {
          IgnoreRounds = true;

          _musicalChanges.Add(new MusicalChange("21345678", 5, "Near miss"));
          _musicalChanges.Add(new MusicalChange("13245678", 5, "Near miss"));
          _musicalChanges.Add(new MusicalChange("12435678", 5, "Near miss"));
          _musicalChanges.Add(new MusicalChange("12354678", 5, "Near miss"));
          _musicalChanges.Add(new MusicalChange("12346578", 5, "Near miss"));
          _musicalChanges.Add(new MusicalChange("12345768", 5, "Near miss"));
          _musicalChanges.Add(new MusicalChange("12345687", 5, "Near miss"));


          _musicalChanges.Add(new MusicalChange("21345678".Reverse(), 5, "Near reverse miss"));
          _musicalChanges.Add(new MusicalChange("13245678".Reverse(), 5, "Near reverse miss"));
          _musicalChanges.Add(new MusicalChange("12435678".Reverse(), 5, "Near reverse miss"));
          _musicalChanges.Add(new MusicalChange("12354678".Reverse(), 5, "Near reverse miss"));
          _musicalChanges.Add(new MusicalChange("12346578".Reverse(), 5, "Near reverse miss"));
          _musicalChanges.Add(new MusicalChange("12345768".Reverse(), 5, "Near reverse miss"));
          _musicalChanges.Add(new MusicalChange("12345687".Reverse(), 5, "Near reverse miss"));

          /////* queens, kings, whittingtons, tittums */
          _musicalChanges.Add(new MusicalChange("13572468", 1, "Queens"));
          _musicalChanges.Add(new MusicalChange("17532468", 1, "Kings"));
          _musicalChanges.Add(new MusicalChange("12753468", 1, "Whittingtons"));
          _musicalChanges.Add(new MusicalChange("15263748", 1, "Tittums"));

          _musicalChanges.Add(new MusicalChange("xxxx2468", 1, "Queens back"));

          _musicalChanges.Add(new MusicalChangeRun(1, "Run up 4", 4, 1));
          _musicalChanges.Add(new MusicalChangeRun(1, "Run down 4", 4, -1));


          _musicalChanges.Add(new MusicalChangeRun(2, "Run up 5", 5, 1));
          _musicalChanges.Add(new MusicalChangeRun(2, "Run down 5", 5, -1));


          _musicalChanges.Add(new MusicalChangeRun(3, "Run up 6", 6, 1));
          _musicalChanges.Add(new MusicalChangeRun(3, "Run down 6", 6, -1));


          _musicalChanges.Add(new MusicalChangeRun(4, "Run up 7", 7, 1));
          _musicalChanges.Add(new MusicalChangeRun(4, "Run down 7", 7, -1));


          _musicalChanges.Add(new MusicalChangeRun(5, "Run up 8", 8, 1));
          _musicalChanges.Add(new MusicalChangeRun(5, "Run down 8", 8, -1));
      }

      public void InitRuns()
      {
          IgnoreRounds = true;

          //_musicalChanges.Add(new MusicalChange("21345678", 10, "Near miss"));
          //_musicalChanges.Add(new MusicalChange("13245678", 10, "Near miss"));
          //_musicalChanges.Add(new MusicalChange("12435678", 10, "Near miss"));
          //_musicalChanges.Add(new MusicalChange("12354678", 10, "Near miss"));
          //_musicalChanges.Add(new MusicalChange("12346578", 10, "Near miss"));
          //_musicalChanges.Add(new MusicalChange("12345768", 10, "Near miss"));
          //_musicalChanges.Add(new MusicalChange("12345687", 10, "Near miss"));

          /////* queens, kings, whittingtons, tittums */
          //_musicalChanges.Add(new MusicalChange("13572468", 10, "Queens"));
          //_musicalChanges.Add(new MusicalChange("17532468", 10, "Kings"));
          //_musicalChanges.Add(new MusicalChange("12753468", 10, "Whittingtons"));
          //_musicalChanges.Add(new MusicalChange("15263748", 10, "Tittums"));

          //_musicalChanges.Add(new MusicalChange("xxxx2468", 5, "Queens back"));

          _musicalChanges.Add(new MusicalChangeRun(1, "Run up 4", 4, 1));
          _musicalChanges.Add(new MusicalChangeRun(1, "Run down 4", 4, -1));


          _musicalChanges.Add(new MusicalChangeRun(1, "Run up 5", 5, 1));
          _musicalChanges.Add(new MusicalChangeRun(1, "Run down 5", 5, -1));


          _musicalChanges.Add(new MusicalChangeRun(1, "Run up 6", 6, 1));
          _musicalChanges.Add(new MusicalChangeRun(1, "Run down 6", 6, -1));


          _musicalChanges.Add(new MusicalChangeRun(1, "Run up 7", 7, 1));
          _musicalChanges.Add(new MusicalChangeRun(1, "Run down 7", 7, -1));


          _musicalChanges.Add(new MusicalChangeRun(1, "Run up 8", 8, 1));
          _musicalChanges.Add(new MusicalChangeRun(1, "Run down 8", 8, -1));
      }
  
    public void InitSJT_Old()
    {
        _musicalChanges.Add(new MusicalChange("xxxx5678", 1, "CRU"));
        _musicalChanges.Add(new MusicalChange("xxxx4678", 1, "CRU"));
        _musicalChanges.Add(new MusicalChange("xxxx6478", 1, "CRU"));
        _musicalChanges.Add(new MusicalChange("xxxx5478", 1, "CRU"));
        _musicalChanges.Add(new MusicalChange("xxxx4578", 1, "CRU"));
        _musicalChanges.Add(new MusicalChange("xxxx6578", 1, "CRU"));

        ///* queens, kings, whittingtons, tittums */
        _musicalChanges.Add(new MusicalChange("13572468", 2, "Queens"));
        _musicalChanges.Add(new MusicalChange("17532468", 1, "Kings"));
        _musicalChanges.Add(new MusicalChange("12753468", 1, "Whittingtons"));
        _musicalChanges.Add(new MusicalChange("15263748", 2, "Tittums"));

        _musicalChanges.Add(new MusicalChange("xxxxx468", 1, "Other"));
        _musicalChanges.Add(new MusicalChange("xxxx7568", 1, "Other"));
        _musicalChanges.Add(new MusicalChange("xxxx7658", 1, "Other"));
        _musicalChanges.Add(new MusicalChange("xxxx2468", 1, "Other"));
        _musicalChanges.Add(new MusicalChange("xxxx8765", 1, "Other"));

        ///* roll-ups/downs off the front */
        ///
        _musicalChanges.Add(new MusicalChange("5678xxxx", 1, "Rollup/down off front"));
        _musicalChanges.Add(new MusicalChange("8765xxxx", 1, "Rollup/down off front"));
        _musicalChanges.Add(new MusicalChange("7568xxxx", 1, "Rollup/down off front"));
        _musicalChanges.Add(new MusicalChange("45678xxx", 1, "Rollup/down off front"));
        _musicalChanges.Add(new MusicalChange("345678xx", 1, "Rollup/down off front"));
    }
    internal int ScoreLead(List<Row> allRows)
    {
      // accumulate the simple score skipping the final row which is modelled as part of the
      // next lead
      int score = 0;
      //if (allRows[0].CoursingOrder().StartsWith("7"))
      //{
      //   score = 10;
      //}
      //return score;
      bool handstroke = false;
      int lastRowScore = 0;
      foreach (Row r in allRows)
      {
        score += lastRowScore;
        int rowScore = handstroke ? this.scores[r.ToNumber()] : 0;
        handstroke = !handstroke;
        lastRowScore = rowScore;
        //Console.WriteLine(r.ToString() + " " + rowScore);
      }
      return score;
    }
    private int Score(Row r)
    {
        int ret = 0;
        if ( !string.IsNullOrEmpty(partMapping) )
        {
            var rs = r.ToString();
            do
            {
                var newRow = string.Empty;
                for (int i = 0; i < partMapping.Length; ++i)
                {
                    var oldBell = rs[i];
                    char newBell = partMapping[oldBell - '1'];
                    newRow += newBell;
                }
                ret += Score0(Row.FromString(newRow));
                rs = newRow;
            }
            while (rs != r.ToString());
        }
        else
        {
        ret = Score0(r);
        }
        
        return ret;
    }
    private int Score0(Row r)
    {
        int rowScore = 0;
        if (IgnoreRounds && r.ToNumber() == 0)
        {

        }
        else
        {
            foreach (var c in _musicalChanges)
            {
                int scoreC = c.Score(r);
                if (scoreC > rowScore)
                {
                    rowScore = scoreC;
                }
            }
        }
        return rowScore;
    }

    public IEnumerable<string> EnumerateMusic(Row r)
    {
      foreach (var c in _musicalChanges)
      {
        if (c.Score(r) > 0)
        {
          yield return c.Name + " " + c.Points.ToString();
        }
      }
    }
    internal IEnumerable<IMusicalChange> EnumerateMusicalChanges(Row r)
    {
      foreach (var c in _musicalChanges)
      {
        if (c.Score(r) > 0)
        {
            yield return c;
        }
      }
    }
  }
}
