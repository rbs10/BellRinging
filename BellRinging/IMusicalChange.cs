using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BellRinging
{
    public interface IMusicalChange
    {
        int Score(Row row);
        string Name { get; }
        int Points { get; }
    }
}
