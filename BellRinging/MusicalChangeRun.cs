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
        for (int i = 0; i <= change.Length - runLength; ++i)
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
                bool isRunPlusA = true;
                if ( i >  0)
                {                   
                    int offset = -1;
                    var bell1_2 = change[i + offset];
                    for (int r2 = 1; r2 < runLength + 1; ++r2)
                    {
                        if (change[i + r2 + offset] - bell1_2 != r2 * runDelta)
                        {
                            isRunPlusA = false;
                            // break;
                        }
                    }
                }
                else
                {
                    isRunPlusA = false;
                }
                bool isRunPlusB = true;
                if ( i + runLength < change.Length)
                {
                    int offset = 0;
                    var bell1_2 = change[i + offset];
                    for (int r2 = 1; r2 < runLength + 1; ++r2)
                    {
                        if (change[i + r2 + offset] - bell1_2 != r2 * runDelta)
                        {
                            isRunPlusB = false;
                            // break;
                        }
                    }
                }
                else
                {
                    isRunPlusB = false;
                }
                if (!(isRunPlusA||isRunPlusB))
                {
                    return Points;
                }
            }
        }
        return 0;
    } 
    }
}
