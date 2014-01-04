using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BellRinging
{
    public abstract class MusicalChangeBase : IMusicalChange
    {
        short _score;
        string _name;
        public MusicalChangeBase(short score, string name)
        {
            _name = name;
            _score = score;
        }
        public string Name
        {
            get
            {
                return _name;
            }
        }



        public short Points
        {
            get
            {
                return _score;
            }
        }


        public abstract short Score(Row row);
    }
}
