using System;
using System.Collections.Generic;
using System.Text;

namespace BellRinging
{
  public class Row 
  {
    char[] _row;
    const string _rounds = "1234567890ET";
    int _number;

   
    static int[] factorials = new int[] { Factorial(1), Factorial(2), Factorial(3), Factorial(4), Factorial(5), Factorial(6), 
      Factorial(7), Factorial(8), Factorial(9), Factorial(10), Factorial(11), Factorial(12) };

    static Row[] _allRows = GenerateAllRows();

    static public string RoundsString()
    {
      return _rounds;
    }

    static Row[] GenerateAllRows()
    {
      Row[] allRows = new Row[40320];
      SortedList<string, bool> checkUnique = new SortedList<string, bool>();
      Dictionary<string, int> ttCount = new Dictionary<string, int>();
      for (int i = 0; i < allRows.Length; ++i)
      {
        allRows[i] = Row.CalcFromNumber(i);
        checkUnique.Add(allRows[i].ToString(), true);
        if (i != allRows[i].CalcNumber())
        {
          int x = allRows[i].CalcNumber();
          Row r2 = Row.CalcFromNumber(i);
          throw new Exception("Row num calc error");
        }
        //if (i < 5040 && allRows[i].CoursingOrder().StartsWith("7"))
        //{
        //  if (allRows[i].InCourse())
        //  {
        //    ++ttInCourse;
        //  }
        //  string r = allRows[i].ToString();
        //  r = r.Replace('2', 'x');
        //  r = r.Replace('3', 'x');
        //  r = r.Replace('4', 'x');
        //  r = r.Replace('5', 'x');
        //  r = r.Replace('6', 'x');
        //  if (ttCount.ContainsKey(r))
        //  {
        //    ttCount[r] = ttCount[r] + 1;
        //  }
        //  else
        //  {
        //    ttCount[r] = 1;
        //  }
        //  Console.WriteLine(allRows[i]);
        //  ++tenorsTogether;
        //}
      }
      return allRows;
    }

    public static Row[] AllRows
    {
      get
      {
        return _allRows;
      }
    }


    private static int Factorial(int p)
    {
      if (p <= 0)
      {
        return 1;
      }
      else
      {
        return p * Factorial(p - 1);
      }
    }

 
   

    static public Row FromNumber(int number)
    {
      return _allRows[number];
    }

    static internal Row CalcFromNumber(int number)
    {
      char[] rowChars = new char[8];
      char refChar = rowChars[0];
      // the final bell can only be in one position so only worry about first N-1 bells
      for (int bell = 0; bell < rowChars.Length; ++bell)
      {
        int wantedPos;
        char bellChar = _rounds[bell];
        if (rowChars.Length - 1 - bell - 1 >= 0)
        {
          int numForBell = factorials[rowChars.Length - 1 - bell - 1];
          wantedPos = number / numForBell;
          number = number - wantedPos * numForBell;
        }
        else
        {
          wantedPos = 0;
        }
        int pos = 0;
        for ( int skipUsed = 0; skipUsed < wantedPos; ++skipUsed )
        {
          // skip over places already used          
          while(rowChars[pos] != refChar)
          {
            ++pos;
          }

          ++pos;
        }
        while (rowChars[pos] != refChar)
        {
          ++pos;
        }
        rowChars[pos] = bellChar;
      }

      return new Row(rowChars);
    }

    private int CalcNumber()
    {
      int number = 0;
      // the final bell can only be in one position so only worry about first N-1 bells
      for (int bell = 0; bell < _row.Length - 1; ++bell)
      {
        int numForBell = factorials[_row.Length - 1 - bell - 1];
        char bellChar = _rounds[bell];
        for (int i = 0; i < _row.Length; ++i)
        {
          if (_row[i] == bellChar)
          {
            break;
          }
          else if (_row[i] > bellChar)
          {
            number += numForBell;
          }
        }
      }
      return number;
    }

    private Row(char[] row)
    {
      _row = row;
      _number = CalcNumber();
      IsTenorsTogetherLeadEnd = _row[0] == _rounds[0] && CoursingOrder().StartsWith("7");
    }

    public Row(int noBells)
    {
      _row = _rounds.Substring(0, noBells).ToCharArray();
      _number = CalcNumber();
      IsTenorsTogetherLeadEnd = _row[0] == _rounds[0] && CoursingOrder().StartsWith("7");
    }

    /// <summary>
    /// True if as a lead end this row has tenors together
    /// </summary>
    public bool IsTenorsTogetherLeadEnd { get; private set; }

    public Row Apply(int[] mapping)
    {
      char[] newRow = Permutation.Apply(_row,mapping);
      return new Row(newRow);
    }

    public Row Apply(Permutation perm)
    {
      return _allRows[perm.Apply(_number)];
    }

    public override int GetHashCode()
    {
      return _number.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      Row otherRow = obj as Row;
      if (otherRow != null)
      {
        return otherRow._number == _number;
      }
      else
      {
        return false;
      }
    }

    public override string ToString()
    {
      return new string(_row);
    }

    public bool InCourse()
    {
      char[] bells = _row.Clone() as char[];
      int swaps = 0;
      for (int i = 0; i < bells.Length; ++i)
      {
        char bell = _rounds[i];
        int pos = -1;
        for ( int p = i; p < bells.Length; ++p )
        {
          if (bells[p] == bell)
          {
            pos = p;
            break;
          }
        }
        if (pos != i)
        {
          if (pos < 0) { throw new Exception("Bell " + bell + " missing in row " + _row); }
          bells[pos] = bells[i];
          bells[i] = bell;
          ++swaps;
        }
      }
      return swaps % 2 == 0;
    }

    public string CoursingOrder()
    {
      if (_row[0] != _rounds[0] )
      {
        throw new Exception("Row does not start with treble: " + _row);
      }
      int tenor = 0;
      char tenorBellChar = _rounds[_row.Length - 1];
      while ( _row[tenor] != tenorBellChar )
      {
        ++tenor;
      }
      int pos = tenor;
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < _row.Length - 2; ++i)
      {
        pos = CoursingAfter(pos, _row.Length);
        sb.Append(_row[pos]);
      }
      return sb.ToString();
    }

    public static int CoursingAfter(int pos, int noBells)
    {
      if (pos % 2 == 0)
      {
        // 0 2 4 etc. going out
        // an odd position (0 based) so going out - bell after us is at -= 2
        // 
        pos -= 2;
        if (pos < 1 ) // onto the treble
        {
          pos = 1;
        }
      }
      else
      {
        // 1 3 5 7 etc. going in
        pos += 2;
        if (pos >= noBells )
        {
          // noBells - 1 is max valid value - but must switch to the other half of this branch
          if (noBells - 1 % 2 == 0)
          {
            pos = noBells - 1;
          }
          else
          {
            pos = noBells - 2;
          }
        }
      }
      return pos;
        
    }

 

    /// <summary>
    /// Return an integer that uniquely identifies the row. Stable within program execution.
    /// 
    /// For 8 bells number with treble at front is 0...5039
    /// </summary>
    /// <returns></returns>
    internal Int16 ToNumberExTreble()
    {
     
      return (short)_number;
    }

    public int ToNumber()
    {
      return _number;
    }

    /// <summary>
    /// Return mapping that takes rounds to this change
    /// </summary>
    /// <returns></returns>
    public int[] AsIntMapping()
    {
      int[] mapping = new int[_row.Length];
      for ( int i = 0; i < _row.Length; ++i )
      {
        char bellChar = _row[i];
        int index = 0;
        while (_rounds[index] != bellChar)
        {
          ++index;
        }
        mapping[index] = i;
      }
      return mapping;
    }

    internal bool IsFromCourseEnd()
    {
      return _row[_row.Length - 1] == _rounds[_row.Length - 1];
    }

    internal bool IsRounds()
    {
      return _number == 0;
    }

    internal static Row FromString(string change)
    {
       for ( int index = 0; index < _allRows.Length; ++index )
       {
           if ( _allRows[index].ToString() == change )
           {
               return _allRows[index];
           }
       }
       throw new Exception("Row not found: " + change);
    }
  }
}
