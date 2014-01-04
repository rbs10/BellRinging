using System;
using System.Collections.Generic;
using System.Text;

namespace BellRinging
{
  /// <summary>
  /// A sequence of permutations
  /// </summary>
  public class SequenceOfPermutations
  {
    List<Permutation> _permutations = new List<Permutation>();

    /// <summary>
    /// Parse a sequence from a string of the form x14.5.x.15x16
    /// </summary>
    /// <param name="notation"></param>
    /// <param name="noBells"></param>
    public SequenceOfPermutations(string baseNotation, int noBells)
    {
      int cursor = 0;
      int startOfPerm = 0;
      string notation = baseNotation.ToLower().Replace(" ", "");
      while (cursor < notation.Length)
      {
        // either a . or an x or a number
        //
        // numbers build up and get acting on later
        char c = notation[cursor];
        if ( c == '.' )
        {
          if ( startOfPerm < cursor )
          {
            _permutations.Add(Permutation.FromPlaceNotation(notation.Substring(startOfPerm, cursor - startOfPerm), noBells));
            ++cursor;
            startOfPerm = cursor;
          }
          else
          {
            throw new Exception("Got . with no preceeding notation in [" + baseNotation + "]");
          }
        }
        else if (c == 'x')
        {
          // if there is a number built up then do that
          if (startOfPerm < cursor)
          {
            _permutations.Add(Permutation.FromPlaceNotation(notation.Substring(startOfPerm, cursor - startOfPerm), noBells));
          }
          // now do the x
          _permutations.Add(Permutation.FromPlaceNotation("x", noBells));

            ++cursor;
            startOfPerm = cursor;
        }
        else
        {
          // should be a number!
          ++cursor;
        }
      }
      // if there was a residue left over
      if (startOfPerm < cursor)
      {
        _permutations.Add(Permutation.FromPlaceNotation(notation.Substring(startOfPerm, cursor - startOfPerm), noBells));
        ++cursor;
        startOfPerm = cursor;
      }
    }

    public List<Row> Apply(Row startRow)
    {
      List<Row> rows = new List<Row>();
      Row row = startRow;
      foreach ( Permutation p in _permutations )
      {
        row = row.Apply(p);
        rows.Add(row);
      }
      return rows;
    }

    /// <summary>
    /// Extend sequence with the reflection about the final permutation
    /// </summary>
    internal void ReflectAboutFinalPermutation()
    {
      for (int i = _permutations.Count - 2; i >= 0; --i)
      {
        _permutations.Add(_permutations[i]);
      }
    }

    /// <summary>
    /// Remove the first n changes form the sequence
    /// </summary>
    /// <param name="n">The number of changes to remove</param>
    internal void RemoveFirst(int n)
    {
        for (int i = 0; i < n; ++i)
        {
            _permutations.RemoveAt(0);
        }
    }
  }
}
