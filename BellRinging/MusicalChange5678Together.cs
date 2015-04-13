using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BellRinging
{
    /// <summary>
    /// MusicalChange looking for runs
    /// </summary>
    public class MusicalChange5678Together :MusicalChangeBase
    {
        public MusicalChange5678Together( int score, string name, int offset)
            : base(score, name)
    {
        this.offset = offset;
    }

        int offset;

        public override int Score(Row row)
        {
            var change = row.ToString().Select(c => int.Parse(c.ToString())).ToArray();
            int total = 0;
            for (int i = offset; i < offset + 4; ++i)
            {

                total += change[i];
            }
            return (total == 5 + 6 + 7 + 8) ? Points : (int)0;
        }
    }
}
