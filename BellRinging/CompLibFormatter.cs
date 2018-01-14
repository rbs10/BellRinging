using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BellRinging
{
  class CompLibFormatter
  {
    private Composition composition;

    public CompLibFormatter(Composition composition)
    {
      this.composition = composition;
    }

    internal string Write()
    {
      // just take calling positions for now - good for single methods
      var baseString = composition.ToString().Split(' ').First()
        .Replace("4", "F");
      var calls = baseString.Replace("s","").Distinct().ToList();
      var header = string.Join("\t",calls);
      var sb = new StringBuilder();
      sb.AppendLine(header);
      int pos = 0;
      while ( pos < baseString.Length )
      {
        bool isSingle = false;
        var c = baseString[pos++];
        if ( c == 's')
        {
          isSingle = true;
          c = baseString[pos++];
        }
        var linesBits = calls.Select(call => call == c ? (isSingle ? "s" : "-") : "");
        var line = string.Join("\t", linesBits);
        sb.AppendLine(line);
      }
      return sb.ToString();
    }
  }
}
