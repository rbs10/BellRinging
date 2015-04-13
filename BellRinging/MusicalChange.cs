using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BellRinging
{
  /// <summary>
  /// Test for a musical change
  /// </summary>
  public class MusicalChange : MusicalChangeBase
  {
    Regex _regex;

    public MusicalChange(string pattern, int score, string name) : base(score,name)
    {
      _regex = new Regex(pattern.ToUpper().Replace("X","."));
    }


    public override int Score(Row row)
    {
      if (_regex.Matches(row.ToString()).Count > 0 )
      {
        return Points;
      }
      else
      {
        return 0;
      }
    }

  }
}
