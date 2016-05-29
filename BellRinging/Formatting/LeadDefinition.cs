using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BellRinging
{
    /// <summary>
    /// Representation of a call in a composition
    /// </summary>
    public class LeadDescription
    {
        /// <summary>
        /// The name of the position at the end of the lead
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// The call made
        /// </summary>
        public string Call { get; set; }

        /// <summary>
        /// The definition of this lead to put into MicroSiril
        /// </summary>
        public string LeadDefinition { get; set; }

        public string CourseNameBit
        {
            get
            {
                string ret = string.Empty;
                if ( HasCall)
                {
                    var callBit = Call;
                    var posBit = Position;
                    if ( callBit == "b" )
                    {
                        callBit = string.Empty;
                    }
                    if (Position == "2nds")
                    {
                        if (Call == "s")
                        {
                            posBit = "T"; // "two"- 2 gets used as a repeater in MicroSiril I think
                        }
                        else
                        {
                            posBit = "I";
                        }
                    }
                   
                    if ( Call == "@" )
                    {
                        callBit = "handstrokeStart";
                    }
                    ret = callBit + posBit ;
                }
                return ret;           
            }
        }

        public bool HasCall { get { return !string.IsNullOrWhiteSpace(Call); } }
    }
}
