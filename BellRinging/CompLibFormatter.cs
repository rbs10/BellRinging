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
      bool spliced = composition.COM > 0;

      // just take calling positions for now - good for single methods
      var baseString = composition.ToString().Split(' ').First()
        .Replace("4", "F");
      var methodPart = composition.ToString().Split(' ').Skip(1).First();
      var calls = baseString.Replace("s","").Distinct().ToList();
      var header = string.Join("\t",calls);
      if ( spliced )
      {
        header += "\tMethods";
      }
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
        var callChar = isSingle ? "s" : "-";
        var linesBits = calls.Select(call => call == c ? callChar : "");
        var line = string.Join("\t", linesBits);
        if ( spliced )
        {
          var callPos = methodPart.IndexOf(callChar);
          var methods = methodPart.Substring(0, callPos);
          methodPart = methodPart.Substring(callPos + 1);
          line += "\t" + methods;
          methods = string.Empty;
        }
        sb.AppendLine(line);
      }
      return sb.ToString();
    }
  }
}
