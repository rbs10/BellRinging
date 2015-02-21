using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace BellRinging
{
  public class Lead
  {
    Method _method;
    List<Row> _rows;
    int[] _rowInts;
    short _number;
    int _containsRoundsAt = -1;

    // thinking index leads by the lead head and method
    //
    // can enumerate leads that way

    public Lead(Method m, List<Row> rows)
    {
      _method = m;
      _rows = rows;
      _rowInts = new int[_rows.Count];
      int i = 0;
      int roundsInt = new Row(8).ToNumber();
      foreach ( Row row in _rows )
      {
        _rowInts[i] = row.ToNumber();
        if (_containsRoundsAt < 0 && row.ToNumber() == roundsInt)
        {
          _containsRoundsAt = i;
        }

        ++i;
      }

      // precompute for speed
      _number = (short)_rowInts[0];

      //_containsRoundsAt = -1;
    }

    public int ContainsRoundsAt
    {
      get
      {
        return _containsRoundsAt;
      }
    }

    public int[] RowsAsInts
    {
      get
      {
        return _rowInts;
      }
    }

    /// <summary>
    /// Return true if two leads are mutually false
    /// </summary>
    /// <param name="otherLead"></param>
    public bool IsFalseAgainst(Lead otherLead)
    {
      foreach (int row in _rowInts)
      {
        foreach (int otherRow in otherLead._rowInts)
        {
          if (row == otherRow)
          {
            return true;
          }
        }
      }
      return false;

      //foreach (Row row in _rows)
      //{
      //  foreach (Row otherRow in otherLead._rows)
      //  {
      //    if (row.Equals(otherRow))
      //    {
      //      return true;
      //    }
      //  }
      //}
      //return false;
    }

    public bool IsFalseAgainstIgnoringSnapCompletion(Lead otherLead)
    {
      int rounds = new Row(8).ToNumber();

      foreach (int row in _rowInts)
      {
        if (row == rounds) break; // checked everything that will ring from the lead
        foreach (int otherRow in otherLead._rowInts)
        {
          if (otherRow == rounds) break; // checked everything that will ring from the lead
          if (row == otherRow)
          {
            return true;
          }
        }
      }
      return false;
    }
    /// <summary>
    /// Convert lead to a number
    /// </summary>
    /// <returns></returns>
    public Int16 ToNumber()
    {
      return _number;
    }

    // getting to false ness
    //
    // can look at internal changes - work out permutations that make those internal changes false
    // resolve via groups
    //
    // or can just look at all leads and work out what is false against that
    //
    // just use permutations to then apply that reasoning to all other leads

    // maybe worth resolving leads by treble position to speed search of falseness
    //
    // or build index of Leads by change
    public Row LeadHead()
    {
      return _rows[0];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="permutation"></param>
    /// <returns></returns>
    internal Row NextLeadHead(Permutation leadHeadChange)
    {
      return _rows[_rows.Count - 1].Apply(leadHeadChange);
    }

    int cyclicScore = -1;
    public int CyclicScore(MusicalPreferences musicalPreferences, Row nextLeadHead)
    {
        if ( cyclicScore == -1 )
        {
            cyclicScore = CalcCyclicScrore(musicalPreferences, nextLeadHead);
        }
        return cyclicScore;
    }

      // Does not appear to exclude anything
    //public bool IsTrueToCyclic()
    //{
    //    List<Row> allRows = new List<Row>();
    //    foreach (var rowx in _rows)
    //    {
    //        var row = rowx;
    //        allRows.Add(row);

    //        for (int part = 0; part < 6; ++part)
    //        {

    //            var change = row.ToString();
    //            change = change.Replace("8", "x");
    //            change = change.Replace("7", "8");
    //            change = change.Replace("6", "7");
    //            change = change.Replace("5", "6");
    //            change = change.Replace("4", "5");
    //            change = change.Replace("3", "4");
    //            change = change.Replace("2", "3");
    //            change = change.Replace("x", "2");

    //            row = Row.FromString(change);
    //            allRows.Add(row);
    //        }
    //    }
    //    var isTrue =  allRows.Select(x => x.ToNumber())
    //        .GroupBy(x => x)
    //        .Select(g => g.Count()).Max() < 2;
    //    return isTrue;
    //}
    
    private int CalcCyclicScrore(MusicalPreferences musicalPreferences, Row nextLeadHead)
    {
        List<Row> allRows;
        if (ContainsRoundsAt <= 0)
        {
            allRows = new List<Row>(_rows);
        }
        else
        {
            allRows = new List<Row>();
            for (int i = 0; i < ContainsRoundsAt; ++i)
            {
                allRows.Add(_rows[i]);
            }
        }
        allRows.Add(nextLeadHead);

        int score  = musicalPreferences.ScoreLead(allRows);
        for (int part = 0; part < 6; ++part )
        {
            var oldRows = allRows;
            allRows = new List<Row>(oldRows.Count);
            foreach (var row in oldRows)
            {
                var change = row.ToString();
                change = change.Replace("8","x");
                change = change.Replace("7","8");
                change = change.Replace("6","7");
                change = change.Replace("5","6");
                change = change.Replace("4","5");
                change = change.Replace("3","4");
                change = change.Replace("2","3");
                change = change.Replace("x","2");
 
                var newRow = Row.FromString(change);
                allRows.Add(newRow);
            }
            score += musicalPreferences.ScoreLead(allRows);
        }
        return score;
    }


    internal short Score(MusicalPreferences musicalPreferences, Row nextLeadHead)
    {
      // the nextLeadHead lead is only relevant for cross change music (wraprounds)

      List<Row> allRows;
      if (ContainsRoundsAt <= 0)
      {
        allRows = new List<Row>(_rows);
      }
      else
      {
        allRows = new List<Row>();
        for (int i = 0; i < ContainsRoundsAt; ++i)
        {
          allRows.Add(_rows[i]);
        }
      }
      allRows.Add(nextLeadHead);
      var baseScore =  musicalPreferences.ScoreLead(allRows);

      return baseScore;

      //return (short)(baseScore + 10 * CalcWraps());
      //return musicalPreferences.Score(_rows);
    }

    public int Length
    {
      get
      {
        return _rows.Count;
      }
    }

    // TODO: maybe refactor wraps with composition
    public string ToLongString(bool spaceOutBackstrokes)
    {
      StringBuilder sb = new StringBuilder();
      string roundsString = Row.RoundsString();
      int duration = 35 * 60 * 1000 / 1260 / 8;
      bool isBackstroke = true; // start with final rounds
      int l = 0;
      foreach (Row r in _rows)
      {
        // exclude final rounds from analysis
        if (l == _containsRoundsAt && l > 0) break;
        ++l;

        foreach (char bellChar in r.ToString())
        {
          int index = roundsString.IndexOf(bellChar);
          sb.Append((char)('1' + index));
        }
        if (isBackstroke && spaceOutBackstrokes)
        {
          sb.Append(" ");
        }
        isBackstroke = !isBackstroke;
      }
      return sb.ToString();
    }

    Regex r1 = new Regex("12345678");

    public int CalcWraps()
    {
      string s = ToLongString(true);
      int count = 0;
      return r1.Matches(s).Count;
      //return r1.Match(s).Captures.Count;
    }
  }
}
