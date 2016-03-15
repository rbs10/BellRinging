using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BellRinging
{
    public static class CompositionTokenizer 
    {
        public static IEnumerable<string> Tokenize(string source)
        {
            StringBuilder next = new StringBuilder();
            foreach ( var ch in source )
            {
                // white space delimits tokens but is not included in them
               if ( Char.IsWhiteSpace(ch))
               {
                   if (next.Length > 0)
                   {
                       yield return next.ToString();
                       next = new StringBuilder();
                   }
               }
                // calls end token
               else if ( ch == 's' || ch == '-')
                {
                    next.Append(ch);
                    yield return next.ToString();
                    next = new StringBuilder();
                }
                   // anything else is start of a new token
               else
               {
                   if (next.Length > 0)
                   {
                       yield return next.ToString();
                       next = new StringBuilder();
                   }
                   next.Append(ch);
               }
            }
            if ( next.Length > 0)
            {
                yield return next.ToString();
            }
        }
    }
}
