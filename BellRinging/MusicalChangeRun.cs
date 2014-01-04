using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BellRinging
{
    /// <summary>
    /// MusicalChange looking for runs
    /// </summary>
    public class MusicalChangeRun :MusicalChangeBase
    {
        public MusicalChangeRun( short score, string name, int runLength, int runDelta)
            : base(score, name)
    {
        this.runDelta = runDelta;
        this.runLength = runLength;
    }

        int runLength;
        int runDelta;

    public override short Score(Row row)
    {
        var change = row.ToString().Select(c => int.Parse(c.ToString())).ToArray();
        for (int i = 0; i < change.Length - runLength; ++i)
        {
            var bell1 = change[i];
            bool isRun = true;
            for (int r = 1; r < runLength; ++r)
            {
                if (change[i+r] - bell1 != r * runDelta)
                {
                    isRun = false;
                    // break;
                }
            }
            if (isRun)
            {
                return Points;
            }
        }
        return 0;
    } 
    }
}
