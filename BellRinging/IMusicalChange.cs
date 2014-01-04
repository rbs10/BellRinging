using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BellRinging
{
    public interface IMusicalChange
    {
        short Score(Row row);
        string Name { get; }
        short Points { get; }
    }
}
