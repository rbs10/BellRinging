using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BellRinging
{
    public abstract class MusicalChangeBase : IMusicalChange
    {
        int _score;
        string _name;
        public MusicalChangeBase(int score, string name)
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



        public int Points
        {
            get
            {
                return _score;
            }
        }


        public abstract int Score(Row row);
    }
}
